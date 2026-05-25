using UnityEngine;

/// <summary>
/// ScriptableObject that represents one gear item.
/// Create via: Assets ▶ Create ▶ Scriptable Objects ▶ GearItem
///
/// Stat bonuses are NOT stored here — they are derived at runtime from
/// GearEnums so that balance tweaks in GearEnums automatically propagate
/// to every existing GearItem asset.
/// </summary>
[CreateAssetMenu(fileName = "NewGearItem", menuName = "Scriptable Objects/GearItem")]
public class GearItem : ScriptableObject
{
    [Header("Identity")]
    public int          id;
    public string       itemName;
    [TextArea] public string description;

    [Header("Gear")]
    public GearEnums.Rarity   rarity;
    public GearEnums.GearSlot slot;

    [Header("Visuals")]
    public Sprite icon;   // drag your gear sprite here in the Inspector

    // ── Derived stat helpers ──────────────────────────────────────────────────
    // Always read these instead of hardcoding values — they pull from GearEnums.

    public int   HpBonus     => GearEnums.GetHpBonus(rarity);
    public int   DamageBonus => GearEnums.GetDamageBonus(rarity);
    public float SpeedBonus  => GearEnums.GetSpeedBonus(rarity);

    public float GoldMultiplier => GearEnums.GetGoldMultiplier(rarity);
    public float GemMultiplier  => GearEnums.GetGemMultiplier(rarity);

    // ── Display ───────────────────────────────────────────────────────────────

    /// <summary>Returns a rich-text summary suitable for a tooltip or reward popup.</summary>
    public string GetTooltip()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"{GearEnums.GetRarityLabel(rarity)} {itemName}");
        sb.AppendLine($"Slot: {slot}");

        if (slot == GearEnums.GearSlot.Armor  || HpBonus     > 0)
            sb.AppendLine($"+{HpBonus} HP");
        if (slot == GearEnums.GearSlot.Weapon || DamageBonus > 0)
            sb.AppendLine($"+{DamageBonus} Damage");
        if (slot == GearEnums.GearSlot.Boots  || SpeedBonus  > 0)
            sb.AppendLine($"-{SpeedBonus:F1}s Attack Cooldown");

        if (GoldMultiplier > 1f)
            sb.AppendLine($"x{GoldMultiplier:F2} Gold Rewards");
        if (GemMultiplier > 1f)
            sb.AppendLine($"x{GemMultiplier:F2} Gem Rewards");

        return sb.ToString().TrimEnd();
    }

    public override string ToString() => $"[{rarity}] {itemName} ({slot})";
}