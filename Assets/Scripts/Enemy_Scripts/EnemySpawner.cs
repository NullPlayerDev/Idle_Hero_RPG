using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector
    // -------------------------------------------------------------------------

    [Header("References")]
    [SerializeField] private CombatSystem combatSystem;

    [Header("Enemy Prefabs")]
    [Tooltip("Prefab that has EnemyBehaviour + EnemyData assigned for each type")]
    [SerializeField] private List<GameObject> enemyPrefab;
   [SerializeField] List<GameObject> wave = new List<GameObject>();
    /*[SerializeField] private GameObject tankEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private GameObject magicEnemyPrefab;*/
    [Header("Spawn Points")]
    [Tooltip("World positions where enemies will appear (assigned in order)")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    public List<GameObject> Wave
    {
        get => wave;
        set => wave = value;
    }
    // -------------------------------------------------------------------------
    // Level property — set by GameManager before SpawnEnemiesForLevel() is called
    // -------------------------------------------------------------------------

   // [SerializeField] private int level = 1;
    /*public int Level
    {
        get => level;
        set => level = value;
    }*/

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        // FIX: Resolve CombatSystem in Awake (not Start) so it is ready
        // when GameManager calls SpawnEnemiesForLevel() immediately on scene load,
        // which can happen before Start() has run on this object.
        if (combatSystem == null)
            combatSystem = FindAnyObjectByType<CombatSystem>();
    }

    private void Update()
    {
        if (combatSystem == null)
        {
            combatSystem = FindAnyObjectByType<CombatSystem>();
        }
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

        List<GameObject> toSpawn = wave;

        if (toSpawn.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawner] BuildWaveForLevel({GameManager.Instance.CurrentLevel}) returned an empty list. Check that your enemy prefabs are assigned in the Inspector.");
            return;
        }

        if (GameManager.Instance.CurrentLevel % 10 == 0)
        {
            Transform spawnPoint = spawnPoints[2];
            GameObject enemy = Instantiate(toSpawn[4], spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            // Tell CombatSystem how many enemies to wait for BEFORE instantiating any.
            //combatSystem.SetExpectedEnemies(wave.Count);
            Debug.LogWarning("Total wave count is: " + wave.Count);
            for (int i = 0; i < wave.Count; i++)
            {
                if (wave[i] == null)
                {
                    Debug.LogError($"[EnemySpawner] Prefab at wave index {i} is null. Assign it in the Inspector.");
                    continue;
                }

                Transform spawnPoint = (spawnPoints != null && i < spawnPoints.Count)
                    ? spawnPoints[i]
                    : transform;

                GameObject enemy = Instantiate(toSpawn[i], spawnPoint.position, spawnPoint.rotation);
                enemy.name = $"{toSpawn[i].name}_L{GameManager.Instance.CurrentLevel}_{i}";

                Debug.Log($"[EnemySpawner] Spawned {enemy.name} at {spawnPoint.position}");
            }
        }

        Debug.Log($"[EnemySpawner] Level {GameManager.Instance.CurrentLevel} wave ready — {toSpawn.Count} enemy/enemies.");
    }

    // -------------------------------------------------------------------------
    // The enemies will be spawned based on 
    // Heroes Number
    // -------------------------------------------------------------------------
    public void  BuildLevelBasedOnHeroNumber()
    {
      int k= Random.Range(0, 3);
      wave.Add(enemyPrefab[k]);
        //return wave;
    }
    
    
    //--------------------------------------------
    // This one is based on the Level
    //----------------------------------------------
    
    /*private List<GameObject> BuildWaveForLevel(int lvl)
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
    }*/
}