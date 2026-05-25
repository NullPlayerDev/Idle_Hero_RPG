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

    void Start() { }
    void Update() { }

    public void CalculateReward()
    {
        CalculateGoldsReward();
        RewardInWallet();
    }

    public void RandomlyReward() { }

    public int CalculateBasicReward(RewardEnums.GoldRewardTier rewardType)
    {
        return RewardEnums.GoldReward(rewardType);
    }

    // ── Gold ──────────────────────────────────────────────────────────────────

    public int CalculateGoldsReward()
    {
        goldTier = RewardEnums.GoldTier(stage);
        int multiplier = RewardEnums.GoldBonus(RewardEnums.GoldBonus(stage));
        int baseGold   = currentGold + CalculateBasicReward(goldTier) + Variance() * multiplier;

        // Apply gear gold multiplier (uses best equipped item, not additive stack)
        float gearMultiplier = GearInventory.Instance != null
            ? GearInventory.Instance.BestGoldMultiplier()
            : 1f;

        finalGold = Mathf.RoundToInt(baseGold * gearMultiplier);

        if (gearMultiplier > 1f)
            Debug.Log($"[RewardCalculator] Gold gear bonus x{gearMultiplier:F2}: {baseGold} → {finalGold}");

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

    // ── Gems ──────────────────────────────────────────────────────────────────

    public int CalculateGemsReward()
    {
        int earned = 1; // base: 1 gem per stage completion

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

        // Apply gear gem multiplier
        float gearMultiplier = GearInventory.Instance != null
            ? GearInventory.Instance.BestGemMultiplier()
            : 1f;

        int boostedEarned = Mathf.RoundToInt(earned * gearMultiplier);

        if (gearMultiplier > 1f)
            Debug.Log($"[RewardCalculator] Gem gear bonus x{gearMultiplier:F2}: {earned} → {boostedEarned}");

        Debug.Log("totalGems earned this stage: " + boostedEarned);
        return boostedEarned;
    }

    public void RewardInWallet()
    {
        int gemsEarned = CalculateGemsReward();
        if (gemsEarned > 0)
            RewardWallet.Instance.AddGems(gemsEarned);
    }
}