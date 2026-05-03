using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lives in CombatScene.
/// GameManager calls SpawnSelectedHeroes() after the scene loads.
/// DO NOT call SpawnSelectedHeroes() from Start() — GameManager owns that call.
///
/// Inspector setup:
///   - heroPrefabs: drag in one prefab per hero type. Each prefab must have a
///     HeroBehaviour component whose heroData asset has the correct ID set.
///   - spawnPoints: 4 Transforms (order: BOTTOM_LEFT, BOTTOM_RIGHT, TOP_LEFT, TOP_RIGHT)
///   - combatSystem: drag in or leave null (auto-found via FindObjectOfType)
/// </summary>
public class HeroSpawner : MonoBehaviour
{
    [Header("Hero Prefabs — HeroBehaviour.heroData.ID must match HeroData.ID")]
    [SerializeField] private List<GameObject> heroPrefabs = new List<GameObject>();

    [Header("Spawn Points (BOTTOM_LEFT, BOTTOM_RIGHT, TOP_LEFT, TOP_RIGHT)")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("References")]
    [SerializeField] private CombatSystem combatSystem;

    private void Awake()
    {
        // Resolve CombatSystem early so SpawnSelectedHeroes() can call
        // SetExpectedHeroes() even before any Start() has run.
        if (combatSystem == null)
            combatSystem = FindObjectOfType<CombatSystem>();
    }

    private void Start()
    {
        // FIX: Do NOT call SpawnSelectedHeroes() here.
        // GameManager.SpawnLevel() is the single place that drives spawning.
        // Calling it here as well caused heroes to be registered twice and
        // expectedHeroes to be overwritten mid-spawn.
    }

    public void SpawnSelectedHeroes()
    {
        if (HeroSelectionManager.Instance == null)
        {
            Debug.LogError("[HeroSpawner] HeroSelectionManager not found! Make sure it exists in the PlayerSelection scene and persists via DontDestroyOnLoad.");
            return;
        }

        for (int i = 0; i < combatSystem.Heroes.Count; i++)
        {
            HeroBehaviour hb = combatSystem.Heroes[i].gameObject.GetComponent<HeroBehaviour>();
            //Transform spawnPoint = spawnPoints[i];
            Transform spawnPoint = spawnPoints[i];
            GameObject hero = Instantiate(hb.HeroPrefab, spawnPoint.position, spawnPoint.rotation);
            
            //Debug.Log($"[HeroSpawner] Spawned hero ID {hero.GetComponent<HeroData>().ID} at slot {i} → {spawnPoint.position}");
        }
        /*List<int> selectedIds = HeroSelectionManager.Instance.SelectedHeroIds;

        if (selectedIds == null || selectedIds.Count == 0)
        {
            Debug.LogWarning("[HeroSpawner] No heroes selected — nothing to spawn.");
            return;
        }

        // Tell CombatSystem how many heroes to wait for BEFORE instantiating any,
        // so TryStartBattle() doesn't fire early if a hero registers instantly.
        combatSystem.SetExpectedHeroes(selectedIds.Count);

        for (int i = 0; i < selectedIds.Count; i++)
        {
            int heroId = selectedIds[i];
            GameObject prefab = GetPrefabById(heroId);

            if (prefab == null)
            {
                Debug.LogError($"[HeroSpawner] No prefab found for hero ID {heroId}. " +
                               $"Make sure a prefab in the HeroPrefabs list has a HeroBehaviour " +
                               $"whose HeroData asset has ID = {heroId}.");
                continue;
            }

            Transform spawnPoint = i < spawnPoints.Count ? spawnPoints[i] : transform;
            GameObject hero = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            hero.name = $"Hero_ID{heroId}_Slot{i}";

            Debug.Log($"[HeroSpawner] Spawned hero ID {heroId} at slot {i} → {spawnPoint.position}");
        }*/

        //Debug.Log($"[HeroSpawner] {selectedIds.Count} hero(es) spawned for combat.");
    }

    /// <summary>
    /// FIX: HeroData is a ScriptableObject — it is NOT a Component on the prefab.
    /// The correct path is: prefab → HeroBehaviour component → heroData field → ID.
    /// GetComponent<HeroData>() always returned null, so nothing ever spawned.
    /// </summary>
    public GameObject GetPrefabById(int id)
    {
        foreach (GameObject prefab in heroPrefabs)
        {
            if (prefab == null) continue;

            HeroBehaviour hb = prefab.GetComponent<HeroBehaviour>();
            if (hb == null)
            {
                Debug.LogWarning($"[HeroSpawner] Prefab '{prefab.name}' has no HeroBehaviour component — skipping.");
                continue;
            }

            if (hb.HeroData == null)
            {
                Debug.LogWarning($"[HeroSpawner] Prefab '{prefab.name}' HeroBehaviour has no HeroData assigned — skipping.");
                continue;
            }

            if (hb.HeroData.ID == id)
                return prefab;
        }

        return null;
    }
}