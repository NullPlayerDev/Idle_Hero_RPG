using UnityEngine;
using DG.Tweening;
public class ChestTiers : MonoBehaviour
{
    [SerializeField] private float moveDistance = 15f;
    [SerializeField] private float duration = 15f;
    [SerializeField] private ParticleSystem particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       particles.Play();
        transform.DOMoveY(transform.position.y + moveDistance, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
        
    }


}
