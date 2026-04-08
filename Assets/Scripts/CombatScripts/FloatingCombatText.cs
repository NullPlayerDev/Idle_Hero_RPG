using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FloatingCombatText : MonoBehaviour
{
    public static FloatingCombatText Instance;

    [SerializeField] private TextMeshProUGUI textPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Pass attacker transform directly
    public void Show(string message, Transform attackerTransform)
    {
        if (attackerTransform == null) return;

        // Convert world → screen position
   
            Vector2 pos = new Vector2(transform.position.x, transform.position.y-900f);
        // Create new text
        TextMeshProUGUI newText = Instantiate(textPrefab, pos, Quaternion.identity).GetComponent<TextMeshProUGUI>();

        newText.text = message;
        newText.alpha = 1f;
        newText.transform.position = transform.position;

        // Animate
        Sequence seq = DOTween.Sequence();

        seq.Append(newText.transform.DOMoveY(newText.transform.position.y + 80f, 1f));
        seq.Join(newText.DOFade(0f, 5f));

        // Destroy after animation
        seq.OnComplete(() => Destroy(newText.gameObject));
    }
}