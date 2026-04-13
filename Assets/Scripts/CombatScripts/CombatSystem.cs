using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatSystem : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private List<HeroBehaviour> heroes = new List<HeroBehaviour>();
    [SerializeField] private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI heroResultText;
    [SerializeField] private TextMeshProUGUI enemyResultText;

    [Header("Turn Settings")]
    [Tooltip("How many total heroes are in this scene — battle starts once all register")]
    [SerializeField] private int expectedHeroes = 1;
    [Tooltip("How many total enemies are in this scene — battle starts once all register")]
    [SerializeField] private int expectedEnemies = 1;
    [Tooltip("Pause between hero phase and enemy phase")]
    [SerializeField] private float delayBetweenTurns = 0.5f;

    [SerializeField] private RewardCalculator rewardCalculator;

    public event Action OnStageEnded;

    private bool isStageEnded = false;
    private bool battleStarted = false;
    private int total_Stages_Won = 0;

    private int heroAttacksPending = 0;
    private int enemyAttacksPending = 0;

    public bool IsStageEnded => isStageEnded;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        rewardCalculator = FindAnyObjectByType<RewardCalculator>();
    }

    // -------------------------------------------------------------------------
    // Registration — heroes and enemies call these in their own Start()
    // Once everyone has registered, the battle loop starts automatically
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

    // Starts the battle only once ALL expected combatants have checked in
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
            // HERO PHASE 
            Debug.Log("[CombatSystem] >>> HERO PHASE");

            CleanLists();
            if (isStageEnded) break;

            heroAttacksPending = heroes.Count;
            Debug.Log($"[CombatSystem] {heroAttacksPending} hero(es) will attack.");

            foreach (HeroBehaviour hero in new List<HeroBehaviour>(heroes))
            {
                EnemyBehaviour target = GetRandomEnemy();
                if (target != null)
                {
                    hero.ExecuteAttack(target, OnHeroAttackFinished);
                }
                else
                {
                    heroAttacksPending--;
                }
            }

            yield return new WaitUntil(() => heroAttacksPending <= 0);
            Debug.Log("[CombatSystem] <<< Hero phase complete.");

            if (isStageEnded) break;
            yield return new WaitForSeconds(delayBetweenTurns);

            // ── ENEMY PHASE ──────────────────────────────────────────────────
            Debug.Log("[CombatSystem] >>> ENEMY PHASE");

            CleanLists();
            if (isStageEnded) break;

            enemyAttacksPending = enemies.Count;
            Debug.Log($"[CombatSystem] {enemyAttacksPending} enemy/enemies will attack.");

            foreach (EnemyBehaviour enemy in new List<EnemyBehaviour>(enemies))
            {
                HeroBehaviour target = GetRandomHero();
                if (target != null)
                {
                    enemy.ExecuteAttack(target, OnEnemyAttackFinished);
                }
                else
                {
                    enemyAttacksPending--;
                }
            }

            yield return new WaitUntil(() => enemyAttacksPending <= 0);
            Debug.Log("[CombatSystem] <<< Enemy phase complete.");

            if (isStageEnded) break;
            yield return new WaitForSeconds(delayBetweenTurns);
        }
    }

    private void OnHeroAttackFinished()
    {
        heroAttacksPending--;
        Debug.Log($"[CombatSystem] Hero attack finished. Pending: {heroAttacksPending}");
    }

    private void OnEnemyAttackFinished()
    {
        enemyAttacksPending--;
        Debug.Log($"[CombatSystem] Enemy attack finished. Pending: {enemyAttacksPending}");
    }

    // -------------------------------------------------------------------------
    // Death Handling
    // -------------------------------------------------------------------------

    public void OnHeroDied(HeroBehaviour hero)
    {
        heroes.Remove(hero);
        // Decrement so the WaitUntil doesn't hang if a hero dies mid-animation
        heroAttacksPending = Mathf.Max(0, heroAttacksPending - 1);
        CheckBattleEnd();
    }

    public void OnEnemyDied(EnemyBehaviour enemy)
    {
        enemies.Remove(enemy);
        enemyAttacksPending = Mathf.Max(0, enemyAttacksPending - 1);
        CheckBattleEnd();
    }

    // -------------------------------------------------------------------------
    // Target Selection
    // -------------------------------------------------------------------------

    public EnemyBehaviour GetRandomEnemy()
    {
        CleanLists();
        if (enemies.Count == 0) return null;
        return enemies[Random.Range(0, enemies.Count)];
    }

    public HeroBehaviour GetRandomHero()
    {
        CleanLists();
        if (heroes.Count == 0) return null;
        return heroes[Random.Range(0, heroes.Count)];
    }

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