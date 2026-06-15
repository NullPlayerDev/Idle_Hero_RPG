using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the weighted random rarity roll when a chest is opened.
/// 
/// SETUP IN INSPECTOR:
///   - Assign one or more GearItem ScriptableObjects to each rarity bucket list.
///   - Call ChestDropper.Instance.OpenChest() when the player opens a chest.
///   - The result is added to GearInventory automatically.
/// 
/// DROP WEIGHT (defined in GearEnums.GetDropWeight):
///   Common 50 | Uncommon 28 | Rare 14 | Epic 6 | Legendary 2  → total 100
/// </summary>
public class ChestDropper : MonoBehaviour
{
    public static ChestDropper Instance { get; private set; }

    [Header("Gear Pools — assign GearItem assets per rarity")]
    [SerializeField] private List<GearItem> commonPool;
    [SerializeField] private List<GearItem> uncommonPool;
    [SerializeField] private List<GearItem> rarePool;
    [SerializeField] private List<GearItem> epicPool;
    [SerializeField] private List<GearItem> legendaryPool;

    // ── Events ────────────────────────────────────────────────────────────────
    public delegate void GearDroppedHandler(GearItem item);
    public event GearDroppedHandler OnGearDropped;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Performs one weighted rarity roll, picks a random item from that rarity's pool,
    /// adds it to GearInventory, and fires OnGearDropped.
    /// Returns null if the rarity pool is empty (log warning).
    /// </summary>
    public GearItem OpenChest()
    {
        GearEnums.Rarity rarity = RollRarity();
        GearItem item = PickFromPool(rarity);

        if (item == null)
        {
            Debug.LogWarning($"[ChestDropper] Rolled {rarity} but pool is empty — no gear dropped.");
            return null;
        }

        GearInventory.Instance.AddToInventory(item);
        OnGearDropped?.Invoke(item);

        Debug.Log($"[ChestDropper] Chest opened → {item}");
        return item;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Weighted rarity roll
    // ─────────────────────────────────────────────────────────────────────────

    private GearEnums.Rarity RollRarity()
    {
        // Build cumulative weight table
        GearEnums.Rarity[] rarities = (GearEnums.Rarity[])System.Enum.GetValues(typeof(GearEnums.Rarity));

        int totalWeight = 0;
        foreach (GearEnums.Rarity r in rarities)
            totalWeight += GearEnums.GetDropWeight(r);

        int roll = Random.Range(0, totalWeight); // [0, totalWeight)
        int cumulative = 0;

        foreach (GearEnums.Rarity r in rarities)
        {
            cumulative += GearEnums.GetDropWeight(r);
            if (roll < cumulative)
                return r;
        }

        return GearEnums.Rarity.Common; // fallback (should never reach here)
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Pool selection
    // ─────────────────────────────────────────────────────────────────────────

    private GearItem PickFromPool(GearEnums.Rarity rarity)
    {
        List<GearItem> pool = GetPool(rarity);

        if (pool == null || pool.Count == 0) return null;

        return pool[Random.Range(0, pool.Count)];
    }

    private List<GearItem> GetPool(GearEnums.Rarity rarity)
    {
        switch (rarity)
        {
            case GearEnums.Rarity.Common:    return commonPool;
            case GearEnums.Rarity.Uncommon:  return uncommonPool;
            case GearEnums.Rarity.Rare:      return rarePool;
            case GearEnums.Rarity.Epic:      return epicPool;
            case GearEnums.Rarity.Legendary: return legendaryPool;
            default:                         return commonPool;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Editor helper — preview odds in the console
    // ─────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR
    [ContextMenu("Log Drop Odds")]
    private void LogDropOdds()
    {
        GearEnums.Rarity[] rarities = (GearEnums.Rarity[])System.Enum.GetValues(typeof(GearEnums.Rarity));
        int total = 0;
        foreach (GearEnums.Rarity r in rarities) total += GearEnums.GetDropWeight(r);

        System.Text.StringBuilder sb = new System.Text.StringBuilder("[ChestDropper] Drop Odds:\n");
        foreach (GearEnums.Rarity r in rarities)
        {
            float pct = (float)GearEnums.GetDropWeight(r) / total * 100f;
            sb.AppendLine($"  {r,-12} {pct:F1}%  (pool size: {GetPool(r)?.Count ?? 0})");
        }
        Debug.Log(sb.ToString());
    }
#endif
}