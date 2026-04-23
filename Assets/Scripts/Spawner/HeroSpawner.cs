using System.Collections.Generic;
using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector
    // -------------------------------------------------------------------------

    [Header("References")]
    [SerializeField] private CombatSystem combatSystem;

    [Header("Hero Prefabs")]
    [Tooltip("Prefab that has HeroBehaviour + HeroData assigned for each type")]
    [SerializeField] private GameObject fastHeroPrefab;
    [SerializeField] private GameObject tankHeroPrefab;
    [SerializeField] private GameObject rangedHeroPrefab;
    [SerializeField] private GameObject bossHeroPrefab;

    [Header("Spawn Points")]
    [Tooltip("Index matches HeroNums.SpawnPosition: 0=TOP_LEFT, 1=TOP_RIGHT, 2=BOTTOM_LEFT, 3=BOTTOM_RIGHT")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        SpawnHeroesForStage();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Spawns the hero lineup for the current stage.
    /// Call this once per combat scene, or again after a re-roll.
    /// </summary>
    public void SpawnHeroesForStage()
    {
        List<HeroSpawnEntry> lineup = BuildHeroLineup();

        // Tell CombatSystem how many heroes to wait for before starting the battle
        combatSystem.SetExpectedHeroes(lineup.Count);

        foreach (HeroSpawnEntry entry in lineup)
        {
            Transform spawnPoint = GetSpawnPoint(entry.Position);

            GameObject hero = Instantiate(entry.Prefab, spawnPoint.position, spawnPoint.rotation);
            hero.name = $"{entry.Prefab.name}_{entry.Position}";

            Debug.Log($"[HeroSpawner] Spawned {hero.name} at {entry.Position}");
        }

        Debug.Log($"[HeroSpawner] {lineup.Count} hero(es) ready for battle.");
    }

    // -------------------------------------------------------------------------
    // Lineup Definition — edit this to change which heroes appear
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns the list of heroes and their spawn positions for this stage.
    /// Add, remove, or reorder entries freely — just make sure each
    /// SpawnPosition is used at most once (two heroes can't share a slot).
    /// </summary>
    private List<HeroSpawnEntry> BuildHeroLineup()
    {
        return new List<HeroSpawnEntry>
        {
            new HeroSpawnEntry(fastHeroPrefab,   HeroNums.SpawnPosition.BOTTOM_LEFT),
            new HeroSpawnEntry(tankHeroPrefab,   HeroNums.SpawnPosition.BOTTOM_RIGHT),
            new HeroSpawnEntry(rangedHeroPrefab, HeroNums.SpawnPosition.TOP_LEFT),
            new HeroSpawnEntry(bossHeroPrefab,   HeroNums.SpawnPosition.TOP_RIGHT),
        };
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Looks up the spawn point by casting SpawnPosition to its int index.
    /// Falls back to this transform if the list slot is missing.
    /// </summary>
    private Transform GetSpawnPoint(HeroNums.SpawnPosition position)
    {
        int index = (int)position;

        if (index >= 0 && index < spawnPoints.Count && spawnPoints[index] != null)
            return spawnPoints[index];

        Debug.LogWarning($"[HeroSpawner] No spawn point at index {index} ({position}), falling back to spawner position.");
        return transform;
    }

    // -------------------------------------------------------------------------
    // Inner type — bundles a prefab with its desired spawn slot
    // -------------------------------------------------------------------------

    private struct HeroSpawnEntry
    {
        public GameObject Prefab;
        public HeroNums.SpawnPosition Position;

        public HeroSpawnEntry(GameObject prefab, HeroNums.SpawnPosition position)
        {
            Prefab   = prefab;
            Position = position;
        }
    }
}