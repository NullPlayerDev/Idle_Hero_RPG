using UnityEngine;

public class RewardCalculator : MonoBehaviour
{
    private RewardEnums rewardEnum;
    private int stage, currentGold,finalGold;
    private bool isFinalStage;
    private int totalGold;
    private bool isConditionTooGood,isConditionTooBad;
    private RewardEnums.GoldRewardTier goldTier;
    [SerializeField] private RewardWallet _rewardWallet;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rewardEnum = GetComponent<RewardEnums>();
    }

    // Update is called once per frame
    void Update()
    {
        //CalculateReward();
    }

    public void CalculateReward()
    {
        CalculateGoldsReward();
        CalculateGemsReward();
    }

    public void RandomlyReward()
    {
        /*
         
         */
    }
    
    //Reward per kill
    public int CalculateBasicReward(RewardEnums.GoldRewardTier rewardType)
    {
        //it will be done from the stage type
        return RewardEnums.GoldReward(rewardType);
    }
    public int  CalculateGoldsReward()
    {
        goldTier = RewardEnums.GoldTier(stage);
        /*
         * So the final gold would be
         * FinalGold = (BaseGold/currentGold + Gold Based on Stages + Variance) * RewardEnum.GoldBonus(rewardtier.type)
         */
       //finalGold = currentGold + CalculateBasicReward() + Variance() + RewardEnums.GoldBonus();
       int multiplier = RewardEnums.GoldBonus(RewardEnums.GoldBonus(stage));
       finalGold = currentGold + CalculateBasicReward(goldTier) + Variance() * multiplier;
       //saving the gold in the wallet
       //adding the gold in the wallet
       _rewardWallet.CurrentGold=finalGold;
       _rewardWallet.AddGold(finalGold);
       // finalGold = currentGold + Variance();
        return finalGold;
    }

    public int Variance()
    {
        /*
         * If total so far is 100 gold
         * if palyer's condition isTooGood =>(-10 to -20)
         * if player's condition isTooBad => (+10 to +20)
         * else Normal => finalGold = totalGold 
         */
        if (totalGold >= 100 && isConditionTooGood)
        {
            totalGold -= Random.Range(10, 20);
        }
        else if (isConditionTooBad)
        {
            totalGold += Random.Range(10, 20);
        }

        if (totalGold % 5 != 0)
        {
            Mathf.Ceil(totalGold);
        }
        finalGold = totalGold;
        return finalGold;
        
    }
    public void CalculateGemsReward()
    {
        /*
         * 
         */
    }
    
}
