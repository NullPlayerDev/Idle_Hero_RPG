using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Combatant Lists (populate via Inspector or spawning system)")]
    [SerializeField] private List<HeroBehaviour> heroes   = new List<HeroBehaviour>();
    [SerializeField] private List<EnemyBehaviour> enemies = new List<EnemyBehaviour>();

    [SerializeField] private TextMeshProUGUI heroResultText;
    [SerializeField] private TextMeshProUGUI enemyResultText;
    // -------------------------------------------------------------------------
    // Registration — called by a spawning system at runtime
    // -------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------
    // Death Callbacks — called by HeroBehaviour / EnemyBehaviour before Destroy
    // -------------------------------------------------------------------------

    public void OnHeroDied(HeroBehaviour hero)
    {
        heroes.Remove(hero);
        Debug.Log($"[CombatSystem] Hero removed. Heroes remaining: {heroes.Count}");
        CheckBattleEnd();
    }

    public void OnEnemyDied(EnemyBehaviour enemy)
    {
        enemies.Remove(enemy);
        Debug.Log($"[CombatSystem] Enemy removed. Enemies remaining: {enemies.Count}");
        CheckBattleEnd();
    }

    // -------------------------------------------------------------------------
    // Target Selection — called by HeroBehaviour and EnemyBehaviour
    // -------------------------------------------------------------------------

    // Returns a random living enemy, or null if none exist.
    public EnemyBehaviour GetRandomEnemy()
    {
        // Clean out any nulls that slipped through (e.g. destroyed between frames).
        enemies.RemoveAll(e => e == null || e.IsDead);

        if (enemies.Count == 0) return null;

        int index = Random.Range(0, enemies.Count);
        return enemies[index];
    }

    // Returns a random living hero, or null if none exist.
    public HeroBehaviour GetRandomHero()
    {
        // Clean out any nulls that slipped through.
        heroes.RemoveAll(h => h == null || h.IsDead);

        if (heroes.Count == 0) return null;

        int index = Random.Range(0, heroes.Count);
        return heroes[index];
    }

    // -------------------------------------------------------------------------
    // Win / Loss Detection
    // -------------------------------------------------------------------------

    private void CheckBattleEnd()
    {
        if (enemies.Count == 0 && heroes.Count > 0)
        {
            heroResultText.text = $"Hero wins!";
            Debug.Log("[CombatSystem] *** HEROES WIN! ***");
            OnHeroesWin();
        }
        else if (heroes.Count == 0 && enemies.Count > 0)
        {
            heroResultText.text = $"Enemy wins!";
            Debug.Log("[CombatSystem] *** ENEMIES WIN! ***");
            OnEnemiesWin();
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
