using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private List<HeroBehaviour> heroes = new List<HeroBehaviour>();
    [SerializeField] private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();
    [SerializeField]private EnemySpawner enemySpawner;
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

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        rewardCalculator = FindAnyObjectByType<RewardCalculator>();
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
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"[CombatSystem] Enemy registered: {enemy.name}. Total: {enemies.Count}/{expectedEnemies}");
        }
        TryStartBattle();
    }

    private void TryStartBattle()
    {
        if (battleStarted) return;
        if (heroes.Count < expectedHeroes) return;
        if (enemies.Count < expectedEnemies) return;

        battleStarted = true;
        Debug.Log("[CombatSystem] All combatants registered — starting battle!");
        StartCoroutine(BattleLoop());
    }

    // -------------------------------------------------------------------------
    // Core Turn Loop
    // -------------------------------------------------------------------------

    private IEnumerator BattleLoop()
    {
        while (!isStageEnded)
        {
            // ── HERO PHASE ────────────────────────────────────────────────────
            Debug.Log("[CombatSystem] >>> HERO PHASE");
            CleanLists();
            if (isStageEnded) break;

            // Each hero attacks one at a time, sequentially
            foreach (HeroBehaviour hero in new List<HeroBehaviour>(heroes))
            {
                if (isStageEnded) break;
                if (hero == null || hero.IsDead) continue;

                // Target the enemy with the LOWEST current health
                EnemyBehaviour target = GetLowestHealthEnemy();
                if (target == null) break; // no enemies left

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

            // Each enemy attacks one at a time, sequentially
            foreach (EnemyBehaviour enemy in new List<EnemyBehaviour>(enemies))
            {
                if (isStageEnded) break;
                if (enemy == null || enemy.IsDead) continue;

                // Target the hero with the LOWEST current health
                HeroBehaviour target = GetLowestHealthHero();
                if (target == null) break; // no heroes left

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
        heroes.RemoveAll(h => h == null || h.IsDead);
        enemies.RemoveAll(e => e == null || e.IsDead);
    }

    private void CheckBattleEnd()
    {
        CleanLists();

        if (enemies.Count == 0 && heroes.Count > 0)
        {
            rewardCalculator.IsStageWon = true;
            total_Stages_Won++;
            heroResultText.text = "Hero wins!";
            EndStage();
        }
        else if (heroes.Count == 0)
        {
            enemyResultText.text = "Enemy wins!";
            EndStage();
        }

        enemySpawner.Level++;
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