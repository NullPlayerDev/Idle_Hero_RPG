
using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;

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
        VERY_HIGH,
        BOSS_HP
    }

    public enum AttackSpeed
    {
        VERY_FAST,
        MEDIUM,
        SLOW,
        BOSS_SPEED
    }

    public enum AttackDamage
    {
        LOW_PER_HIT,
        MEDIUM_HIGH,
        STANDARD,
        HIGH_PER_HIT,
        BOSS_HIT
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
        int lvl = GameManager.Instance.CurrentLevel;
        int enemyHp = 30 * (int)Mathf.Pow(1.8f, lvl);
        switch (health)
        {
            case HP.LOW:
                return enemyHp;
            case HP.MEDIUM:
                 enemyHp = 50 *(int) Mathf.Pow(1.6f, lvl);
                return enemyHp;
            case HP.HIGH: 
                enemyHp = 60 * (int)Mathf.Pow(1.5f, lvl);
                return enemyHp;
            case HP.VERY_HIGH:
                return 100;
            case HP.BOSS_HP:
                enemyHp = Random.Range(350,400) *(int) Mathf.Pow(1.1f, lvl);
                return enemyHp;
            default:     
                return 20;
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
            case AttackSpeed.BOSS_SPEED: return 30f;
            default:                   return 6f;
        }
    }

    // Returns flat damage value for the given damage type.
    public static int GetAttackDamage(AttackDamage damage)
    {
        switch (damage)
        {
            case AttackDamage.LOW_PER_HIT:  return Random.Range(2,5);
            case AttackDamage.MEDIUM_HIGH:  return Random.Range(5,8);
            case AttackDamage.STANDARD:     return Random.Range(10,15);
            case AttackDamage.HIGH_PER_HIT: return Random.Range(15,18);
            case AttackDamage.BOSS_HIT:     return Random.Range(40,50);
            default:                        return 20;
        }
    }
}
