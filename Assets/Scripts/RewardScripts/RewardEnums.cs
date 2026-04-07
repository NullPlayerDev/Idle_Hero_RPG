using UnityEngine;

public class RewardEnums
{
    public enum GoldRewardTier
    {
        NORMAL,  //Basic Gold for stage completion ->5
        STANDARD,  //Basic Gold for stage completion ->10
        RICH, //Basic Gold for stage completion ->20
        BOSS // Basic Gold for stage completion ->40

    }

    public enum GoldBonusType
    {
        NONE,  // None means the exact golds
        DOUBLE, // double amount gold
        TRIPLE  //Triple amount of gold
    }

    public static GoldRewardTier GoldTier(int stage)
    {
       if(stage>=1 && stage<=3) return GoldRewardTier.NORMAL;
        if(stage>=4 && stage<=6) return GoldRewardTier.STANDARD;
        if(stage>=7 && stage<=9) return GoldRewardTier.RICH;
        return GoldRewardTier.BOSS;
    }

    public static GoldBonusType GoldBonus(int stage)
    {
        if(stage>=1 && stage<=3) return GoldBonusType.NONE;
        if(stage>=4 && stage<=6) return GoldBonusType.TRIPLE;
        if (stage >= 7 && stage <= 9) return GoldBonusType.DOUBLE;
        return GoldBonusType.NONE;
    }
    public static int GoldReward(GoldRewardTier type)
    {
        switch (type)
        {
            //Stage 1-3
            case GoldRewardTier.NORMAL: return 5;
            //stage 4-6
            case GoldRewardTier.STANDARD: return 10;
            //stage 7-9
            case GoldRewardTier.RICH: return 15;
            //stage 10 or Final stage
            case GoldRewardTier.BOSS: return 20;
            default: return 2;
        }   
    }

    //it will be multiplied by the stage's final gold
    public static int GoldBonus(GoldBonusType type)
    {
        switch (type)
        {
            case GoldBonusType.NONE: return 1;
            case GoldBonusType.DOUBLE: return 2;
            case GoldBonusType.TRIPLE: return 3;
            default: return 1;
        }
    }
}
