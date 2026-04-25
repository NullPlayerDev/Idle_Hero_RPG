using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector
    // -------------------------------------------------------------------------

    [Header("References")]
    [SerializeField] private CombatSystem combatSystem;

    [Header("Enemy Prefabs")]
    [Tooltip("Prefab that has EnemyBehaviour + EnemyData assigned for each type")]
    [SerializeField] private GameObject fastEnemyPrefab;
    [SerializeField] private GameObject tankEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private GameObject magicEnemyPrefab;

    [Header("Spawn Points")]
    [Tooltip("World positions where enemies will appear (assigned in order)")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    // -------------------------------------------------------------------------
    // Level property — set this before SpawnEnemiesForLevel() is called
    // -------------------------------------------------------------------------
    [SerializeField] private int level = 1;
    public int Level
    {
        get => level;
        set => level = value;
    }

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        //SpawnEnemiesForLevel();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Destroys any existing enemies then spawns a fresh wave for the current level.
    /// Call this whenever you advance to a new stage/level.
    /// </summary>
    public void SpawnEnemiesForLevel()
    {
        // Build the list of prefabs that should appear on this level
        List<GameObject> toSpawn = BuildWaveForLevel(level);

        // Tell CombatSystem how many enemies to wait for before starting the battle
        combatSystem.SetExpectedEnemies(toSpawn.Count);

        for (int i = 0; i < toSpawn.Count; i++)
        {
            // Pick a spawn point; fall back to this transform if we run out
            Transform spawnPoint = (spawnPoints != null && i < spawnPoints.Count)
                ? spawnPoints[i]
                : transform;

            GameObject enemy = Instantiate(toSpawn[i], spawnPoint.position, spawnPoint.rotation);

            // Name the object so logs are readable
            enemy.name = $"{toSpawn[i].name}_L{level}_{i}";

            // EnemyBehaviour.Start() auto-registers with CombatSystem,
            // but we must make sure expectedEnemies on CombatSystem is set
            // to toSpawn.Count BEFORE instantiation (do that in the Inspector
            // or via a SetExpectedEnemies() method on CombatSystem).
            Debug.Log($"[EnemySpawner] Spawned {enemy.name} at {spawnPoint.position}");
        }

        Debug.Log($"[EnemySpawner] Level {level} wave ready — {toSpawn.Count} enemy/enemies.");
    }

    // -------------------------------------------------------------------------
    // Wave Definitions — edit these to design your levels
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns an ordered list of prefabs to spawn for the given level.
    /// Add as many cases as you need.
    /// </summary>
    private List<GameObject> BuildWaveForLevel(int lvl)
    {
        List<GameObject> wave = new List<GameObject>();

        switch (lvl)
        {
            case 1:
                // One basic fast enemy
                wave.Add(fastEnemyPrefab);
                break;

            case 2:
                // Two fast enemies
                wave.Add(fastEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                break;

            case 3:
                // A fast enemy + a tank
                wave.Add(fastEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                break;

            case 4:
                // Two fast + one ranged
                wave.Add(fastEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 5:
                // Harder mix
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 6:
                wave.Add(fastEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 7:
                wave.Add(tankEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 8:
                wave.Add(fastEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 9:
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                break;

            case 10:
                // Boss level
                wave.Add(magicEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            default:
                // For levels beyond 10 keep scaling: add one extra tank per extra level
                wave.Add(magicEnemyPrefab);
                int extras = Mathf.Min(lvl - 10, spawnPoints.Count - 1);
                for (int i = 0; i < extras; i++)
                    wave.Add(tankEnemyPrefab);
                break;
        }

        return wave;
    }
}