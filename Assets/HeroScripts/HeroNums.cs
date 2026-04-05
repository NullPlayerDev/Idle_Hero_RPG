using UnityEngine;

public class HeroNums : MonoBehaviour
{
    //VisualType
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
    public int HealthUpdate(HeroNums.HP health)
    {
        //health bar will decrease 
        switch (health)
        {
            case HeroNums.HP.LOW: return 60;
            case HeroNums.HP.MEDIUM: return 80;
            case HeroNums.HP.HIGH: return 90;
            case HeroNums.HP.VERY_HIGH: return 120;
            default: return 20;
                
        }
    }
    
   public float GetAttackCooldown(HeroNums.AttackSpeed speed)
    {
        switch (speed)
        { 
            case HeroNums.AttackSpeed.VERY_FAST: return 0.5f;
            case HeroNums.AttackSpeed.MEDIUM: return 1.0f;
            case HeroNums.AttackSpeed.SLOW: return 1.5f;
            default: return 1.0f;
        }
    }
    public int AttackDamageMethod(HeroNums.AttackDamage  damage)
    {
        switch (damage)
        {
            case HeroNums.AttackDamage.LOW_PER_HIT:  return 10;
            case HeroNums.AttackDamage.MEDIUM_HIGH:  return 15;
            case HeroNums.AttackDamage.STANDARD: return 20;
            case HeroNums.AttackDamage.HIGH_PER_HIT:  return 25;
            default: return 5;
        }
    }

}
