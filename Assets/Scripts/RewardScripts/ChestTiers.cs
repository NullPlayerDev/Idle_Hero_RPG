using UnityEngine;

public class ChestTiers : MonoBehaviour
{
    [SerializeField] private RewardWallet rewardWallet;
    private ChestRewards chestRewards;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChestDrop()
    {
        rewardWallet.CurrentGems += chestRewards.gem;
        rewardWallet.CurrentGold += chestRewards.gold;
    }
}
