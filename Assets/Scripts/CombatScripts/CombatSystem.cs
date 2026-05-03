using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private List<HeroBehaviour> heroes = new List<HeroBehaviour>();
    [SerializeField] private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    [SerializeField] private EnemySpawner enemySpawner;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI heroResultText;
    [SerializeField] private TextMeshProUGUI enemyResultText;

    [Header("Turn Settings")]
    [Tooltip("How many total heroes are in this scene — battle starts once all register")]
    [SerializeField] private int expectedHeroes = 1;
    [Tooltip("How many total enemies are in this scene — battle starts once all register")]
    [SerializeField] private int expectedEnemies = 1;
    [Tooltip("Pause between each individual attacker")]
    [SerializeField] private float delayBetweenAttackers = 0.2f;
    [Tooltip("Pause between hero phase and enemy phase")]
    [SerializeField] private float delayBetweenPhases = 0.5f;

    [SerializeField] private RewardCalculator rewardCalculator;

    public event Action OnStageEnded;

    private bool isStageEnded = false;
    private bool battleStarted = false;
    private int total_Stages_Won = 0;

    public bool IsStageEnded => isStageEnded;
    [SerializeField] private GameObject heroSelectionPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    public List<HeroBehaviour> Heroes
    {
        get => heroes;
        set => heroes = value;
    }

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        rewardCalculator = FindAnyObjectByType<RewardCalculator>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
    }

    // -------------------------------------------------------------------------
    // Registration
    // -------------------------------------------------------------------------

    public void RegisterHero(HeroBehaviour hero)
    {
        if (!heroes.Contains(hero))
        {
            heroes.Add(hero);
            Debug.Log($"[CombatSystem] Hero registered: {hero.name}. Total: {heroes.Count}/{expectedHeroes}");
        }
        TryStartBattle();
    }

    public void RegisterEnemy(EnemyBehaviour enemy)
    {
        // FIX: was incorrectly wrapped in a heroes.Count loop,
        // causing enemies to never register when heroes hadn't spawned yet.
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"[CombatSystem] Enemy registered: {enemy.name}. Total: {enemies.Count}/{expectedEnemies}");
        }
        TryStartBattle();
    }

    /// <summary>Call this from HeroSpawner BEFORE instantiating heroes.</summary>
    public void SetExpectedHeroes(int count) => expectedHeroes = count;

    /// <summary>Call this from EnemySpawner BEFORE instantiating enemies.</summary>
    public void SetExpectedEnemies(int count) => expectedEnemies = count;

    public void TryStartBattle()
    {
        if (battleStarted) return;
        if (heroes.Count < expectedHeroes) return;
        if (enemies.Count < expectedEnemies) return;
        heroSelectionPanel.SetActive(false);
        //enemySpawner.BuildLevelBasedOnHeroNumber();
        //enemySpawner.SpawnEnemiesForLevel();
        battleStarted = true;
        Debug.Log("[CombatSystem] All combatants registered — starting battle!");
        //StartCoroutine(BattleLoop());
    }

    // -------------------------------------------------------------------------
    // Core Turn Loop
    // -------------------------------------------------------------------------
    public void PanelOff()
    {
        heroSelectionPanel.SetActive(false);
        StartCoroutine(BattleLoop());
    }
    private IEnumerator BattleLoop()
    {
        heroSelectionPanel.gameObject.SetActive(false);
        while (!isStageEnded)
        {
            // ── HERO PHASE ────────────────────────────────────────────────────
            Debug.Log("[CombatSystem] >>> HERO PHASE");
            CleanLists();
            if (isStageEnded) break;

            foreach (HeroBehaviour hero in new List<HeroBehaviour>(heroes))
            {
                if (isStageEnded) break;
                if (hero == null || hero.IsDead) continue;

                EnemyBehaviour target = GetLowestHealthEnemy();
                if (target == null) break;

                bool attackDone = false;
                hero.ExecuteAttack(target, () => attackDone = true);
                yield return new WaitUntil(() => attackDone);

                yield return new WaitForSeconds(delayBetweenAttackers);
            }

            Debug.Log("[CombatSystem] <<< Hero phase complete.");
            if (isStageEnded) break;

            yield return new WaitForSeconds(delayBetweenPhases);

            // ── ENEMY PHASE ───────────────────────────────────────────────────
            Debug.Log("[CombatSystem] >>> ENEMY PHASE");
            CleanLists();
            if (isStageEnded) break;

            foreach (EnemyBehaviour enemy in new List<EnemyBehaviour>(enemies))
            {
                if (isStageEnded) break;
                if (enemy == null || enemy.IsDead) continue;

                HeroBehaviour target = GetLowestHealthHero();
                if (target == null) break;

                bool attackDone = false;
                enemy.ExecuteAttack(target, () => attackDone = true);
                yield return new WaitUntil(() => attackDone);

                yield return new WaitForSeconds(delayBetweenAttackers);
            }
            Debug.Log("[CombatSystem] <<< Enemy phase complete.");
            if (isStageEnded) break;
            yield return new WaitForSeconds(delayBetweenPhases);
        }
    }

    // -------------------------------------------------------------------------
    // Death Handling
    // -------------------------------------------------------------------------

    public void OnHeroDied(HeroBehaviour hero)
    {
        heroes.Remove(hero);
        CheckBattleEnd();
    }

    public void OnEnemyDied(EnemyBehaviour enemy)
    {
        enemies.Remove(enemy);
        CheckBattleEnd();
    }

    // -------------------------------------------------------------------------
    // Target Selection — lowest HP first
    // -------------------------------------------------------------------------

    /// <summary>Returns the living enemy with the lowest current HP.</summary>
    public EnemyBehaviour GetLowestHealthEnemy()
    {
        CleanLists();
        if (enemies.Count == 0) return null;
        return enemies.OrderBy(e => e.CurrentHealth).First();
    }

    /// <summary>Returns the living hero with the lowest current HP.</summary>
    public HeroBehaviour GetLowestHealthHero()
    {
        CleanLists();
        if (heroes.Count == 0) return null;
        return heroes.OrderBy(h => h.CurrentHealth).First();
    }

    // Keep legacy random methods in case other scripts reference them
    public EnemyBehaviour GetRandomEnemy() => GetLowestHealthEnemy();
    public HeroBehaviour GetRandomHero() => GetLowestHealthHero();

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void CleanLists()
    {
        // FIX: only remove from the lists here — each behaviour's DeathRoutine
        // handles Destroy(gameObject) after the death animation finishes.
        // Never call Destroy() on the GameObjects here — that caused
        // "Destroying assets is not permitted" errors.
        heroes.RemoveAll(h => h == null || h.IsDead);
        enemies.RemoveAll(e => e == null || e.IsDead);
    }

    private void CheckBattleEnd()
    {
        CleanLists();
       
        if (enemies.Count == 0 && heroes.Count > 0)
        {
            resultPanel.SetActive(true);
            rewardCalculator.IsStageWon = true;
            total_Stages_Won++;
            //heroResultText.text = "Hero wins!";
            resultText.text = "Hero Wins!";
            EndStage();
        }
        else if (heroes.Count == 0)
        {
            resultPanel.SetActive(true);
            //enemyResultText.text = "Enemy wins!";
            resultText.text = "Enemy wins!";
            EndStage();
        }

        GameManager.Instance.HandleStageEnded();
    }

    private void EndStage()
    {
        if (isStageEnded) return;
        isStageEnded = true;
        Debug.Log("[CombatSystem] Stage ended.");
        OnStageEnded?.Invoke();
    }

    public bool ISConditionGood()
    {
        return rewardCalculator.IsConditionTooGood = true;
    }
}