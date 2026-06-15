using UnityEngine;

/// <summary>
/// Central definition of all gear rarities, gear types, and their gameplay effects.
/// 
/// RARITY SYSTEM OVERVIEW:
///   Common      → lowest drop weight, small stat bonuses, no reward boost
///   Uncommon    → moderate drop weight, minor stat bonuses, tiny reward boost
///   Rare        → medium drop weight, solid stat bonuses, small reward boost
///   Epic        → low drop weight, strong stat bonuses, good reward boost
///   Legendary   → very rare, huge stat bonuses, large reward boost
/// 
/// Everything that scales with rarity is defined HERE — keep balance tweaks in one place.
/// </summary>
public static class GearEnums
{
    // ── Rarity ────────────────────────────────────────────────────────────────

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    // ── Gear Slots ────────────────────────────────────────────────────────────

    public enum GearSlot
    {
        Weapon,     // boosts attack damage
        Armor,      // boosts max HP
        Boots       // boosts attack speed (reduces cooldown)
    }
    // ─────────────────────────────────────────────────────────────────────────
    // Drop weight — higher = more common.
    // Used by ChestDropper to do a weighted random roll.
    // ─────────────────────────────────────────────────────────────────────────

    public static int GetDropWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 50;
            case Rarity.Uncommon:  return 28;
            case Rarity.Rare:      return 14;
            case Rarity.Epic:      return 6;
            case Rarity.Legendary: return 2;
            default:               return 50;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Hero stat multipliers — applied on top of base HeroData stats.
    //   HP bonus       → flat HP added to GetStartingHealth()
    //   Damage bonus   → flat damage added to GetAttackDamage()
    //   Speed bonus    → fraction SUBTRACTED from GetAttackCooldown() (never < 0.5 s)
    // ─────────────────────────────────────────────────────────────────────────

    public static int GetHpBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 5;
            case Rarity.Uncommon:  return 15;
            case Rarity.Rare:      return 25;
            case Rarity.Epic:      return 45;
            case Rarity.Legendary: return 70;
            default:               return 0;
        }
    }

    public static int GetDamageBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 1;
            case Rarity.Uncommon:  return 3;
            case Rarity.Rare:      return 6;
            case Rarity.Epic:      return 12;
            case Rarity.Legendary: return 20;
            default:               return 0;
        }
    }

    /// <summary>Seconds removed from attack cooldown. Result clamped to ≥ 0.5 s by HeroBehaviour.</summary>
    public static float GetSpeedBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 0f;
            case Rarity.Uncommon:  return 0.3f;
            case Rarity.Rare:      return 0.8f;
            case Rarity.Epic:      return 1.5f;
            case Rarity.Legendary: return 2.5f;
            default:               return 0f;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Gold & Gem reward multipliers — multiplied into RewardCalculator results.
    // ─────────────────────────────────────────────────────────────────────────

    public static float GetGoldMultiplier(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 1.00f;
            case Rarity.Uncommon:  return 1.10f;
            case Rarity.Rare:      return 1.25f;
            case Rarity.Epic:      return 1.50f;
            case Rarity.Legendary: return 2.00f;
            default:               return 1.00f;
        }
    }

    public static float GetGemMultiplier(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return 1.00f;
            case Rarity.Uncommon:  return 1.05f;
            case Rarity.Rare:      return 1.15f;
            case Rarity.Epic:      return 1.35f;
            case Rarity.Legendary: return 1.75f;
            default:               return 1.00f;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Display helpers
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Unity rich-text hex colour for each rarity — use in UI labels.</summary>
    public static string GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:    return "#B0B0B0"; // grey
            case Rarity.Uncommon:  return "#4CAF50"; // green
            case Rarity.Rare:      return "#2196F3"; // blue
            case Rarity.Epic:      return "#9C27B0"; // purple
            case Rarity.Legendary: return "#FF9800"; // orange/gold
            default:               return "#FFFFFF";
        }
    }

    public static string GetRarityLabel(Rarity rarity)
    {
        return $"<color={GetRarityColor(rarity)}>{rarity}</color>";
    }
}