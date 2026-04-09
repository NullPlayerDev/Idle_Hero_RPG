using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardCalculator : MonoBehaviour
{

    private int stage, currentGold, finalGold;
    private bool isFinalStage;
    private int totalGold;
    private bool isConditionTooGood, isConditionTooBad, isStageWon, isTheStageFinished;
    private RewardEnums.GoldRewardTier goldTier;
    [SerializeField] private RewardWallet _rewardWallet;
    private int earned = 1;
    public bool IsTheStageFinished
    {
        get => isTheStageFinished;
        set => isTheStageFinished = value;
    }

    public bool IsStageWon
    {
        get => isStageWon;
        set => isStageWon = value;
    }

    public bool IsConditionTooBad
    {
        get => isConditionTooBad;
        set => isConditionTooBad = value;
    }

    public bool IsConditionTooGood
    {
        get => isConditionTooGood;
        set => isConditionTooGood = value;
    }
    void Start()
    {
       
    }

    void Update() { }

    public void CalculateReward()
    {
        CalculateGoldsReward();
        CalculateGemsReward();
    }

    public void RandomlyReward() { }

    // Reward per kill
    public int CalculateBasicReward(RewardEnums.GoldRewardTier rewardType)
    {
        return RewardEnums.GoldReward(rewardType);
    }

    // MAIN function for gold rewards
    public int CalculateGoldsReward()
    {
        goldTier = RewardEnums.GoldTier(stage);
        int multiplier = RewardEnums.GoldBonus(RewardEnums.GoldBonus(stage));
        finalGold = currentGold + CalculateBasicReward(goldTier) + Variance() * multiplier;
        _rewardWallet.AddGold(finalGold);
        return finalGold;
    }

    public int Variance()
    {
        if (totalGold >= 10 && isConditionTooGood)
        {
            totalGold -= Random.Range(10, 20);
            isConditionTooGood = false;
        }
        else if (isConditionTooBad)
        {
            totalGold += Random.Range(30, 40);
            isConditionTooBad = false;
        }
        else
        {
            totalGold += 5;
        }
        if (totalGold % 5 != 0)
            Mathf.Ceil(totalGold);

        finalGold = totalGold;
        return finalGold;
    }

    // Returns gems earned THIS stage only (never a running total).
    // Resets all flags — call exactly once per stage end.
    public int CalculateGemsReward()
    {
        //if (!isTheStageFinished) return 0; // stage not finished — earn nothing

        //int earned = 1; // base: 1 gem per stage completion
        isTheStageFinished = false;

        if (isStageWon)
        {
            earned += 2;
            isStageWon = false;
        }

        if (totalGold >= 100 && isConditionTooGood)
        {
            earned += 5;
        }
        else if (isConditionTooBad)
        {
            earned += 10;
        }
        else
        {
            earned += 2;
        }
        Debug.Log("totalGems: " + earned);
        return earned;
    }

    // Called once per stage end by GameManager.
    // Calculates and immediately deposits gems into the wallet.
    public void RewardInWallet()
    {
        int gemsEarned = CalculateGemsReward();
        if (gemsEarned > 0)
            RewardWallet.Instance.AddGems(gemsEarned);
    }
}