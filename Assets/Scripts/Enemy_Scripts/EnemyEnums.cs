
public static class EnemyEnums
{
    public enum VisualTypes
    {
        FAST,
        TANK,
        RANGED,
        BOSS
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
            case HP.LOW:       return 40;
            case HP.MEDIUM:    return 60;
            case HP.HIGH:      return 80;
            case HP.VERY_HIGH: return 100;
            default:           return 20;
        }
    }

    // Returns attack cooldown in seconds for the given speed.
    public static float GetAttackCooldown(AttackSpeed speed)
    {
        switch (speed)
        {
            case AttackSpeed.VERY_FAST: return 4f;
            case AttackSpeed.MEDIUM:   return 8f;
            case AttackSpeed.SLOW:     return 10f;
            default:                   return 6f;
        }
    }

    // Returns flat damage value for the given damage type.
    public static int GetAttackDamage(AttackDamage damage)
    {
        switch (damage)
        {
            case AttackDamage.LOW_PER_HIT:  return 2;
            case AttackDamage.MEDIUM_HIGH:  return 5;
            case AttackDamage.STANDARD:     return 10;
            case AttackDamage.HIGH_PER_HIT: return 15;
            default:                        return 20;
        }
    }
}
