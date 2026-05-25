using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that owns every GearItem the player has collected.
/// 
/// EQUIPPED GEAR RULES:
///   - One item per GearSlot (Weapon, Armor, Boots).
///   - Equipping a new item in an occupied slot replaces the old one
///     (old item goes back into the inventory list).
///   - Stats are applied to heroes by HeroBehaviour reading GearInventory.Instance.
/// 
/// SAVE INTEGRATION:
///   - SaveData stores gear as int IDs.
///   - GearInventory.SetFromSave() / CollectForSave() hook into SaveSystem.
///   - You must call GearInventory.Instance.Initialise(allGearItems) once on startup
///     (pass the full GearItem[] from your Resources folder or a ScriptableObject list).
/// </summary>
public class GearInventory : MonoBehaviour
{
    public static GearInventory Instance { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;

    // ── Internal state ────────────────────────────────────────────────────────
  private GearItem[] _allGearItems;   // master list loaded at startup

    // items in the bag (not equipped)
   private List<GearItem> _inventory = new List<GearItem>();

    // one slot per GearSlot enum value
    private Dictionary<GearEnums.GearSlot, GearItem> _equipped =
        new Dictionary<GearEnums.GearSlot, GearItem>();

    // ── Public read-only views ────────────────────────────────────────────────
    public IReadOnlyList<GearItem> Inventory => _inventory;
    public IReadOnlyDictionary<GearEnums.GearSlot, GearItem> Equipped => _equipped;

    // ─────────────────────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        //if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call once from GameManager.Start() AFTER SaveSystem.Load().
    /// Pass in every GearItem ScriptableObject that exists in the project.
    /// Example: Resources.LoadAll<GearItem>("GearItems")
    /// </summary>
    public void Initialise(GearItem[] allItems)
    {
        _allGearItems = allItems;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Inventory operations
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Adds a dropped/chest gear item to the player's bag.</summary>
    public void AddToInventory(GearItem item)
    {
        if (item == null) return;
        _inventory.Add(item);
        OnInventoryChanged?.Invoke();
        SaveSystem.Save();
        Debug.Log($"[GearInventory] Added {item} to inventory.");
    }

    /// <summary>
    /// Equips an item from the bag.
    /// If the slot is already occupied, the old item goes back into the bag.
    /// </summary>
    public void Equip(GearItem item)
    {
        if (item == null) return;
        if (!_inventory.Contains(item))
        {
            Debug.LogWarning($"[GearInventory] Tried to equip {item} but it is not in the inventory.");
            return;
        }

        GearEnums.GearSlot slot = item.slot;

        // Unequip existing item in that slot first
        if (_equipped.TryGetValue(slot, out GearItem current) && current != null)
        {
            _inventory.Add(current);
            Debug.Log($"[GearInventory] Unequipped {current} → returned to bag.");
        }

        _inventory.Remove(item);
        _equipped[slot] = item;

        OnEquipmentChanged?.Invoke();
        SaveSystem.Save();
        Debug.Log($"[GearInventory] Equipped {item} in slot {slot}.");
    }

    /// <summary>Unequips the item in the given slot and returns it to the bag.</summary>
    public void Unequip(GearEnums.GearSlot slot)
    {
        if (!_equipped.TryGetValue(slot, out GearItem item) || item == null) return;

        _equipped[slot] = null;
        _inventory.Add(item);

        OnEquipmentChanged?.Invoke();
        SaveSystem.Save();
        Debug.Log($"[GearInventory] Unequipped {item}.");
    }

    public GearItem GetEquipped(GearEnums.GearSlot slot)
    {
        _equipped.TryGetValue(slot, out GearItem item);
        return item;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Aggregate stat helpers — used by HeroBehaviour & RewardCalculator
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Total flat HP bonus from all equipped gear.</summary>
    public int TotalHpBonus()
    {
        int total = 0;
        foreach (var kv in _equipped)
            if (kv.Value != null) total += kv.Value.HpBonus;
        return total;
    }

    /// <summary>Total flat damage bonus from all equipped gear.</summary>
    public int TotalDamageBonus()
    {
        int total = 0;
        foreach (var kv in _equipped)
            if (kv.Value != null) total += kv.Value.DamageBonus;
        return total;
    }

    /// <summary>Total attack-cooldown reduction from all equipped gear (seconds).</summary>
    public float TotalSpeedBonus()
    {
        float total = 0f;
        foreach (var kv in _equipped)
            if (kv.Value != null) total += kv.Value.SpeedBonus;
        return total;
    }

    /// <summary>Highest gold multiplier among equipped gear (not additive — take the best).</summary>
    public float BestGoldMultiplier()
    {
        float best = 1f;
        foreach (var kv in _equipped)
            if (kv.Value != null && kv.Value.GoldMultiplier > best)
                best = kv.Value.GoldMultiplier;
        return best;
    }

    /// <summary>Highest gem multiplier among equipped gear (not additive — take the best).</summary>
    public float BestGemMultiplier()
    {
        float best = 1f;
        foreach (var kv in _equipped)
            if (kv.Value != null && kv.Value.GemMultiplier > best)
                best = kv.Value.GemMultiplier;
        return best;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Save / Load integration
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Returns all owned gear IDs for SaveData (inventory + equipped).</summary>
    public List<int> CollectForSave()
    {
        List<int> ids = new List<int>();
        foreach (GearItem g in _inventory)
            if (g != null) ids.Add(g.id);
        foreach (var kv in _equipped)
            if (kv.Value != null) ids.Add(kv.Value.id);
        return ids;
    }

    /// <summary>Returns equipped gear slot data for SaveData.</summary>
    public List<int> CollectEquippedForSave()
    {
        List<int> ids = new List<int>();
        foreach (var kv in _equipped)
            ids.Add(kv.Value != null ? kv.Value.id : -1);   // -1 = empty slot
        return ids;
    }

    /// <summary>Rebuilds inventory + equipped from saved IDs. Call after Initialise().</summary>
    public void SetFromSave(List<int> inventoryIds, List<int> equippedIds)
    {
        _inventory.Clear();
        _equipped.Clear();

        if (_allGearItems == null)
        {
            Debug.LogError("[GearInventory] SetFromSave called before Initialise() — cannot restore gear.");
            return;
        }

        // Restore bag
        foreach (int id in inventoryIds)
        {
            GearItem found = FindById(id);
            if (found != null) _inventory.Add(found);
        }

        // Restore equipped (order matches GearSlot enum: Weapon=0, Armor=1, Boots=2)
        GearEnums.GearSlot[] slots = (GearEnums.GearSlot[])System.Enum.GetValues(typeof(GearEnums.GearSlot));
        for (int i = 0; i < slots.Length && i < equippedIds.Count; i++)
        {
            if (equippedIds[i] < 0) continue;
            GearItem found = FindById(equippedIds[i]);
            if (found != null) _equipped[slots[i]] = found;
        }

        OnInventoryChanged?.Invoke();
        OnEquipmentChanged?.Invoke();
        Debug.Log($"[GearInventory] Loaded {_inventory.Count} bag items, {_equipped.Count} equipped.");
    }

    private GearItem FindById(int id)
    {
        foreach (GearItem g in _allGearItems)
            if (g != null && g.id == id) return g;
        Debug.LogWarning($"[GearInventory] GearItem with id {id} not found in master list.");
        return null;
    }
}