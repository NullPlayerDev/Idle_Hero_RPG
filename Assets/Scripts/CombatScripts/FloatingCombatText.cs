using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FloatingCombatText : MonoBehaviour
{
    public static FloatingCombatText Instance;
    [SerializeField] private GameObject textPrefab;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Update()
    {
        
    }

    // Pass attacker transform directly
    public void Show(string message, Transform attackerTransform)
    {
        if (textPrefab == null) return;
        if (attackerTransform == null) return;

        // Convert world → screen position
   
           // Vector2 pos = new Vector2(transform.position.x, transform.position.y-900f);
        // Create new text
       //GameObject newText= Instantiate(textPrefab, pos, Quaternion.identity);
        // Animate
        Sequence seq = DOTween.Sequence();

        seq.Append(textPrefab.transform.DOMoveY(textPrefab.transform.position.y + 80f, 15f));
       //seq.Join(textPrefab.DOFade(0f, 5f));

        // Destroy after animation
        seq.OnComplete(() => Destroy(textPrefab.gameObject));
    }
}