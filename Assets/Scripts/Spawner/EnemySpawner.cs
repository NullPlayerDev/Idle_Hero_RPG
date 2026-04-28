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
    // Level property — set by GameManager before SpawnEnemiesForLevel() is called
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

    private void Awake()
    {
        // FIX: Resolve CombatSystem in Awake (not Start) so it is ready
        // when GameManager calls SpawnEnemiesForLevel() immediately on scene load,
        // which can happen before Start() has run on this object.
        if (combatSystem == null)
            combatSystem = FindObjectOfType<CombatSystem>();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    public void SpawnEnemiesForLevel()
    {
        if (combatSystem == null)
        {
            Debug.LogError("[EnemySpawner] CombatSystem reference is null — cannot spawn.");
            return;
        }

        List<GameObject> toSpawn = BuildWaveForLevel(level);

        if (toSpawn.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawner] BuildWaveForLevel({level}) returned an empty list. Check that your enemy prefabs are assigned in the Inspector.");
            return;
        }

        // Tell CombatSystem how many enemies to wait for BEFORE instantiating any.
        combatSystem.SetExpectedEnemies(toSpawn.Count);

        for (int i = 0; i < toSpawn.Count; i++)
        {
            if (toSpawn[i] == null)
            {
                Debug.LogError($"[EnemySpawner] Prefab at wave index {i} is null. Assign it in the Inspector.");
                continue;
            }

            Transform spawnPoint = (spawnPoints != null && i < spawnPoints.Count)
                ? spawnPoints[i]
                : transform;

            GameObject enemy = Instantiate(toSpawn[i], spawnPoint.position, spawnPoint.rotation);
            enemy.name = $"{toSpawn[i].name}_L{level}_{i}";

            Debug.Log($"[EnemySpawner] Spawned {enemy.name} at {spawnPoint.position}");
        }

        Debug.Log($"[EnemySpawner] Level {level} wave ready — {toSpawn.Count} enemy/enemies.");
    }

    // -------------------------------------------------------------------------
    // Wave Definitions
    // -------------------------------------------------------------------------

    private List<GameObject> BuildWaveForLevel(int lvl)
    {
        List<GameObject> wave = new List<GameObject>();

        switch (lvl)
        {
            case 1:
                wave.Add(fastEnemyPrefab);
                break;

            case 2:
                wave.Add(fastEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                break;

            case 3:
                wave.Add(fastEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                break;

            case 4:
                wave.Add(fastEnemyPrefab);
                wave.Add(fastEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            case 5:
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
                wave.Add(magicEnemyPrefab);
                wave.Add(tankEnemyPrefab);
                wave.Add(rangedEnemyPrefab);
                break;

            default:
                wave.Add(magicEnemyPrefab);
                int extras = Mathf.Min(lvl - 10, spawnPoints.Count - 1);
                for (int i = 0; i < extras; i++)
                    wave.Add(tankEnemyPrefab);
                break;
        }

        return wave;
    }
}