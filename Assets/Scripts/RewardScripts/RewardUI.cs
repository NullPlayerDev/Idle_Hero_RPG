using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI gemText;

    private void Start()
    {
        // Guard: Inspector references not assigned
        if (goldText == null || gemText == null)
        {
            Debug.LogError("[RewardUI] goldText or gemText is not assigned in the Inspector!");
            enabled = false;   // stops Update from running and spamming errors
            return;
        }

        // Guard: Wallet singleton not ready yet
        if (RewardWallet.Instance == null)
        {
            Debug.LogError("[RewardUI] RewardWallet.Instance is null — make sure RewardWallet exists in the scene before RewardUI.");
            enabled = false;
            return;
        }

        // Subscribe to events so UI only updates when values actually change
        RewardWallet.Instance.OnGoldChanged += UpdateGoldText;
        RewardWallet.Instance.OnGemsChanged += UpdateGemText;

        // Populate immediately with current values
        UpdateGoldText(RewardWallet.Instance.CurrentGold);
        UpdateGemText(RewardWallet.Instance.CurrentGems);
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid ghost callbacks after scene unload
        if (RewardWallet.Instance == null) return;
        RewardWallet.Instance.OnGoldChanged -= UpdateGoldText;
        RewardWallet.Instance.OnGemsChanged -= UpdateGemText;
    }

    private void UpdateGoldText(int gold) => goldText.text = $"Total Gold: {gold}";
    private void UpdateGemText(int gems)  => gemText.text  = $"Total Gems: {gems}";
}