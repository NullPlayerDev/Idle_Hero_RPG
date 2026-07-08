using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI gemText;

    [Header("Menu UI")]
    [SerializeField] private TextMeshProUGUI menuGoldText;
    [SerializeField] private TextMeshProUGUI menuGemText;

    public TextMeshProUGUI GoldText
    {
        get => goldText;
        set => goldText = value;
    }

    public TextMeshProUGUI GemText
    {
        get => gemText;
        set => gemText = value;
    }

    private void Start()
    {
        if (goldText == null || gemText == null)
        {
            Debug.LogError("[RewardUI] Gameplay Gold/Gem Text references are missing!");
            enabled = false;
            return;
        }

        if (RewardWallet.Instance == null)
        {
            Debug.LogError("[RewardUI] RewardWallet.Instance is null.");
            enabled = false;
            return;
        }

        RewardWallet.Instance.OnGoldChanged += UpdateGoldText;
        RewardWallet.Instance.OnGemsChanged += UpdateGemText;

        UpdateGoldText(RewardWallet.Instance.CurrentGold);
        UpdateGemText(RewardWallet.Instance.CurrentGems);
    }

    private void OnDestroy()
    {
        if (RewardWallet.Instance == null)
            return;

        RewardWallet.Instance.OnGoldChanged -= UpdateGoldText;
        RewardWallet.Instance.OnGemsChanged -= UpdateGemText;
    }

    private void UpdateGoldText(int gold)
    {
        if (goldText != null)
            goldText.text = $"{gold}";

        if (menuGoldText != null)
            menuGoldText.text = $"{gold}";
    }

    private void UpdateGemText(int gems)
    {
        if (gemText != null)
            gemText.text = $"{gems}";

        if (menuGemText != null)
            menuGemText.text = $"{gems}";
    }
}