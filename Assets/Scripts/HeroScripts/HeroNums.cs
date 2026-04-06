// HeroNums.cs
// Pure data class — no MonoBehaviour needed. Holds all hero enums and their value lookups.

public static class HeroNums
{
    public enum VisualTypes
    {
        FAST,
        TANK,
        RANGED,
        BOSS
    }

    public enum SpawnPosition
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }

    public enum HP
    {
        LOW,
        MEDIUM,
        HIGH,
        VERY_HIGH
    }

    public enum AttackSpeed
    {
        VERY_FAST,
        MEDIUM,
        SLOW
    }

    public enum AttackDamage
    {
        LOW_PER_HIT,
        MEDIUM_HIGH,
        STANDARD,
        HIGH_PER_HIT
    }

    public enum DEATHTYPE
    {
        EASY,
        NORMAL,
        HARD
    }

    // Returns the max HP value for the given HP type.
    public static int GetMaxHealth(HP health)
    {
        switch (health)
        {
            case HP.LOW:       return 60;
            case HP.MEDIUM:    return 80;
            case HP.HIGH:      return 90;
            case HP.VERY_HIGH: return 120;
            default:           return 20;
        }
    }

    // Returns attack cooldown in seconds for the given speed.
    public static float GetAttackCooldown(AttackSpeed speed)
    {
        switch (speed)
        {
            case AttackSpeed.VERY_FAST: return 0.5f;
            case AttackSpeed.MEDIUM:   return 1.0f;
            case AttackSpeed.SLOW:     return 1.5f;
            default:                   return 1.0f;
        }
    }

    // Returns flat damage value for the given damage type.
    public static int GetAttackDamage(AttackDamage damage)
    {
        switch (damage)
        {
            case AttackDamage.LOW_PER_HIT:  return 10;
            case AttackDamage.MEDIUM_HIGH:  return 15;
            case AttackDamage.STANDARD:     return 20;
            case AttackDamage.HIGH_PER_HIT: return 25;
            default:                        return 5;
        }
    }
}
