using TMPro;
using UnityEngine;
using UnityEngine.UI;

// =============================================================================
// GearInventoryRow — attach this to your inventory row prefab.
// One row per item: shows name, rarity, stats, and an Equip / Unequip button.`
// =============================================================================

public class GearInventoryRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image           icon;
    [SerializeField] private Button          actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonLabel;

    private GearItem _item;
    private bool     _isEquipped;

    public void Setup(GearItem item, bool isEquipped)
    {
        _item       = item;
        _isEquipped = isEquipped;

        nameText.text  = GearEnums.GetRarityLabel(item.rarity) + " " + item.itemName;
        statsText.text = item.GetTooltip();

        if (icon != null && item.icon != null)
            icon.sprite = item.icon;

        if (actionButton != null)
        {
            actionButtonLabel.text = isEquipped ? "Unequip" : "Equip";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionClicked);
        }
    }

    private void OnActionClicked()
    {
        if (_item == null || GearInventory.Instance == null) return;

        if (_isEquipped)
            GearInventory.Instance.Unequip(_item.slot);
        else
            GearInventory.Instance.Equip(_item);
    }
}