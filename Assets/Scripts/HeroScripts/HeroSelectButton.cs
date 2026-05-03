/*using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Replaces Heroselectionui.cs — delete the old one.
/// Attach to each hero button in the PlayerSelection scene.
/// Set heroId in the Inspector to match this hero's HeroData.ID.
/// </summary>
public class HeroSelectButton : MonoBehaviour
{
    [Header("Must match HeroData.ID on this hero's ScriptableObject")]
    [SerializeField] private int heroId;

    [Header("Visual feedback (all optional)")]
    [SerializeField] private Image      buttonImage;
    [SerializeField] private Color      normalColor   = Color.white;
    [SerializeField] private Color      selectedColor = Color.yellow;
    [SerializeField] private Color      lockedColor   = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private GameObject selectedIndicator; // child glow/checkmark object

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClicked);

        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (HeroSelectionManager.Instance != null)
            HeroSelectionManager.Instance.OnSelectionChanged += RefreshVisual;

        RefreshVisual();
    }

    void OnDestroy()
    {
        if (HeroSelectionManager.Instance != null)
            HeroSelectionManager.Instance.OnSelectionChanged -= RefreshVisual;
    }

    private void OnClicked()
    {
        HeroSelectionManager.Instance?.ToggleHero(heroId);
    }

    private void RefreshVisual()
    {
        if (HeroSelectionManager.Instance == null) return;

        bool isUnlocked = HeroSelectionManager.Instance.UnlockedHeroIds.Contains(heroId);
        bool isSelected = HeroSelectionManager.Instance.SelectedHeroIds.Contains(heroId);
        bool slotsFull  = HeroSelectionManager.Instance.SelectedHeroIds.Count
                          >= HeroSelectionManager.Instance.MaxSelectable;

        _button.interactable = isUnlocked && (isSelected || !slotsFull);

        if (buttonImage != null)
        {
            if      (!isUnlocked) buttonImage.color = lockedColor;
            else if (isSelected)  buttonImage.color = selectedColor;
            else                  buttonImage.color = normalColor;
        }

        if (selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);
    }
}*/