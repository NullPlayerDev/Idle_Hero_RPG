using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatSystem : MonoBehaviour
{
    [Header("Combatants")]
    [SerializeField] private List<HeroBehaviour> heroes = new List<HeroBehaviour>();
    [SerializeField] private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();

    [SerializeField] private TextMeshProUGUI heroResultText;
    [SerializeField] private TextMeshProUGUI enemyResultText;

    // -------------------------
    // Registration
    // -------------------------
    public void RegisterHero(HeroBehaviour hero)
    {
        if (!heroes.Contains(hero))
            heroes.Add(hero);
    }

    public void RegisterEnemy(EnemyBehaviour enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    // -------------------------
    // Death Handling
    // -------------------------
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

    // -------------------------
    // Target Selection
    // -------------------------
    public EnemyBehaviour GetRandomEnemy()
    {
        enemies.RemoveAll(e => e == null || e.IsDead);
        if (enemies.Count == 0) return null;

        return enemies[Random.Range(0, enemies.Count)];
    }

    public HeroBehaviour GetRandomHero()
    {
        heroes.RemoveAll(h => h == null || h.IsDead);
        if (heroes.Count == 0) return null;

        return heroes[Random.Range(0, heroes.Count)];
    }

    // -------------------------
    // Win Condition
    // -------------------------
    private void CheckBattleEnd()
    {
        if (enemies.Count == 0 && heroes.Count > 0)
        {
            heroResultText.text = "Hero wins!";
        }
        else if (heroes.Count == 0 && enemies.Count > 0)
        {
            enemyResultText.text = "Enemy wins!";
        }
    }

    private void OnHeroesWin()
    {
        // TODO: trigger win screen, load next stage, award XP, etc.
    }

    private void OnEnemiesWin()
    {
        // TODO: trigger loss screen, reset stage, etc.
    }
}
