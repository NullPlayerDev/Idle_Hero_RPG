using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles two UI panels:
/// 
///  1. DROP POPUP — appears when a chest is opened, shows the item dropped.
///     Wire ChestDropper.OnGearDropped → this in the Inspector or in Awake().
/// 
///  2. INVENTORY PANEL — scrollable list of all owned gear with equip buttons.
///     Toggle via OpenInventory() / CloseInventory() (wire to a button in Inspector).
/// 
/// INSPECTOR SETUP:
///   dropPopupRoot     — the parent panel that slides in/fades for the drop reveal
///   dropItemName      — TextMeshProUGUI for item name + rarity colour
///   dropItemSlot      — TextMeshProUGUI for slot text
///   dropItemStats     — TextMeshProUGUI for the full tooltip
///   dropItemIcon      — Image for the gear sprite
///   inventoryRoot     — the inventory panel root (hidden by default)
///   inventoryContent  — the ScrollRect content transform (items instantiated here)
///   inventoryRowPrefab— prefab with GearInventoryRow component
/// </summary>
public class GearUI : MonoBehaviour
{
    [Header("Drop Popup")]
    [SerializeField] private GameObject         dropPopupRoot;
    [SerializeField] private TextMeshProUGUI    dropItemName;
    [SerializeField] private TextMeshProUGUI    dropItemSlot;
    [SerializeField] private TextMeshProUGUI    dropItemStats;
    [SerializeField] private Image              dropItemIcon;
    [SerializeField] private Button             dropPopupCloseButton;

    [Header("Inventory Panel")]
    [SerializeField] private GameObject         inventoryRoot;
    [SerializeField] private Transform          inventoryContent;
    [SerializeField] private GameObject         inventoryRowPrefab; // see GearInventoryRow below
    [SerializeField] private Button             inventoryCloseButton;

    // ─────────────────────────────────────────────────────────────────────────

// Change this:


// To this:
    private void Awake()
    {
        if (dropPopupCloseButton != null) dropPopupCloseButton.onClick.AddListener(CloseDropPopup);
        if (inventoryCloseButton != null) inventoryCloseButton.onClick.AddListener(CloseInventory);
        if (dropPopupRoot  != null) dropPopupRoot.SetActive(false);
        if (inventoryRoot  != null) inventoryRoot.SetActive(false);
    }

    private void Start()
    {
        // Singletons are guaranteed to exist by Start()
        if (ChestDropper.Instance != null)
            ChestDropper.Instance.OnGearDropped += ShowDropPopup;

        if (GearInventory.Instance != null)
        {
            GearInventory.Instance.OnInventoryChanged += RefreshInventoryPanel;
            GearInventory.Instance.OnEquipmentChanged += RefreshInventoryPanel;
        }
    }

    private void OnDestroy()
    {
        if (ChestDropper.Instance  != null) ChestDropper.Instance.OnGearDropped -= ShowDropPopup;
        if (GearInventory.Instance != null)
        {
            GearInventory.Instance.OnInventoryChanged -= RefreshInventoryPanel;
            GearInventory.Instance.OnEquipmentChanged -= RefreshInventoryPanel;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Drop Popup
    // ─────────────────────────────────────────────────────────────────────────

    private void ShowDropPopup(GearItem item)
    {
        if (item == null || dropPopupRoot == null) return;

        dropItemName.text  = GearEnums.GetRarityLabel(item.rarity) + " " + item.itemName;
        dropItemSlot.text  = $"Slot: {item.slot}";
        dropItemStats.text = item.GetTooltip();

        if (dropItemIcon != null)
            dropItemIcon.sprite = item.icon;

        dropPopupRoot.SetActive(true);
    }

    private void CloseDropPopup()
    {
        if (dropPopupRoot != null) dropPopupRoot.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Inventory Panel
    // ─────────────────────────────────────────────────────────────────────────

    public void OpenInventory()
    {
        RefreshInventoryPanel();
        
    }

    public void CloseInventory()
    {
        if (inventoryRoot != null) inventoryRoot.SetActive(false);
    }

    public void RefreshInventoryPanel()
    {
        //if (inventoryContent == null || inventoryRowPrefab == null) return;
        if (inventoryRoot != null) inventoryRoot.SetActive(true);
        // Clear old rows
      /*foreach (Transform child in inventoryContent)
            Destroy(child.gameObject);*/
        // if (GearInventory.Instance == null) return;

        // --- Equipped section ---
        foreach (var kv in GearInventory.Instance.Equipped)
        {
            if (kv.Value == null) continue;
            SpawnRow(kv.Value, isEquipped: true);
        }

        // --- Bag section ---
        foreach (GearItem item in GearInventory.Instance.Inventory)
        {
            if (item == null) continue;
            SpawnRow(item, isEquipped: false);
        }
    }

    private void SpawnRow(GearItem item, bool isEquipped)
    {
        GameObject go  = Instantiate(inventoryRowPrefab, inventoryContent);
        var        row = go.GetComponent<GearInventoryRow>();
        if (row != null) row.Setup(item, isEquipped);
    }
}

