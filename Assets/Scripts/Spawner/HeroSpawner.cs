using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lives in CombatScene.
/// On Start() reads SelectedHeroIds from HeroSelectionManager,
/// finds the matching prefab by ID, and spawns them at the spawn points.
///
/// Inspector setup:
///   - heroPrefabs list: one entry per hero. Set heroId to match HeroData.ID,
///     drag in the prefab.
///   - spawnPoints: 4 Transforms (order: BOTTOM_LEFT, BOTTOM_RIGHT, TOP_LEFT, TOP_RIGHT)
///   - combatSystem: drag in or leave null (auto-found via FindObjectOfType)
/// </summary>
public class HeroSpawner : MonoBehaviour
{
    /*
    [System.Serializable]
    public class HeroPrefabEntry
    {
        [Tooltip("Must match the HeroData.ID on this hero's prefab")]
        public int        heroId;
        public GameObject prefab;
    }
    */

    [Header("Hero Prefabs — ID must match HeroData.ID")]
    [SerializeField] private List<GameObject> heroPrefabs = new List<GameObject>();

    [Header("Spawn Points (BOTTOM_LEFT, BOTTOM_RIGHT, TOP_LEFT, TOP_RIGHT)")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("References")]
    [SerializeField] private CombatSystem combatSystem;

    private void Start()
    {
        if (combatSystem == null)
            combatSystem = FindObjectOfType<CombatSystem>();

        SpawnSelectedHeroes();
    }

    public void SpawnSelectedHeroes()
    {
        if (HeroSelectionManager.Instance == null)
        {
            Debug.LogError("[HeroSpawner] HeroSelectionManager not found! Make sure it exists in PlayerSelection scene and persists.");
            return;
        }

        List<int> selectedIds = HeroSelectionManager.Instance.SelectedHeroIds;

        if (selectedIds == null || selectedIds.Count == 0)
        {
            Debug.LogWarning("[HeroSpawner] No heroes selected — nothing to spawn.");
            return;
        }

        combatSystem.SetExpectedHeroes(selectedIds.Count);

        for (int i = 0; i < selectedIds.Count; i++)
        {
            int heroId = selectedIds[i];
            GameObject prefab = GetPrefabById(heroId);

            if (prefab == null)
            {
                Debug.LogError($"[HeroSpawner] No prefab for hero ID {heroId}. Add it to the HeroPrefabs list in the Inspector.");
                continue;
            }

            Transform spawnPoint = i < spawnPoints.Count ? spawnPoints[i] : transform;
            GameObject hero = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            hero.name = $"Hero_ID{heroId}_Slot{i}";

            Debug.Log($"[HeroSpawner] Spawned hero ID {heroId} at slot {i}.");
        }

        Debug.Log($"[HeroSpawner] {selectedIds.Count} hero(es) spawned for combat.");
    }

    private GameObject GetPrefabById(int id)
    {
        foreach (GameObject entry in heroPrefabs)
            if (entry.GetComponent<HeroData>().ID== id)
                return entry;
        return null;
    }
}