using UnityEngine;

public class EnemyEnums : MonoBehaviour
{
    //VisualType
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
    //AttackSpeed
    public enum AttackSpeed
    {
        VERY_FAST,
        SLOW,
        MEDIUM
        
    }
    //AttaclDamage
    public enum AttackDamage
    {
        LOW_PER_HIT,
        HIGH_PER_HIT,
        MEDIUM_HIGH,
        STANDARD
    }

    //Spawning Time
    /*public enum SpawningTime
    {
        EARLY, //STAGE (1 TO 4)
        MEDIUM, //STAGE (5 TO 7)
        LATE, //STAGE (8 TO 10)
        BOSS_APPEARING_TIME, //(11th stage)
        
    }*/
    //Death
    public enum DEATHTYPE
    {
        EASY,
        NORMAL,
        HARD
    }   
    
    /*
 * Function related to Enemy Enums

 */
    public int HealthUpdate(EnemyEnums.HP health)
    {
        //health bar will decrease 
        switch (health)
        {
            case EnemyEnums.HP.LOW: return 40;
            case EnemyEnums.HP.MEDIUM: return 60;
            case EnemyEnums.HP.HIGH: return 80;
            case EnemyEnums.HP.VERY_HIGH: return 100;
            default: return 20;
                
        }
    }
   public float GetAttackCooldown(EnemyEnums.AttackSpeed speed)
    {
        switch (speed)
        { 
            case EnemyEnums.AttackSpeed.VERY_FAST: return 0.5f;
            case EnemyEnums.AttackSpeed.MEDIUM: return 1.0f;
            case EnemyEnums.AttackSpeed.SLOW: return 1.5f;
            default: return 1.0f;
        }
    }
    public int AttackDamageMethod(EnemyEnums.AttackDamage  damage)
    {
        switch (damage)
        {
            case EnemyEnums.AttackDamage.LOW_PER_HIT:  return 10;
            case EnemyEnums.AttackDamage.MEDIUM_HIGH:  return 15;
            case EnemyEnums.AttackDamage.STANDARD: return 20;
            case EnemyEnums.AttackDamage.HIGH_PER_HIT:  return 25;
            default: return 5;
        }
    }
}
