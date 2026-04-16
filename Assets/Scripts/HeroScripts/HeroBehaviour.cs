using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroBehaviour : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private TextMeshProUGUI heroText;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Slider _heroHealthBar;
    [SerializeField] private Animator heroAnimator;
    [SerializeField] private ParticleSystem particles;
    private int currentHealth;
    private bool isHeroDead = false;
    private CombatSystem combatSystem;

    // These two flags are SET by Animation Events on the attack clip.
    // The coroutine simply waits on them — no timers, no state name checks.
    private bool attackHitFrame = false;   // set at the "hit" frame of the swing
    private bool attackFinished = false;   // set at the very last frame of the clip

    public bool IsDead => isHeroDead;
    public int CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindFirstObjectByType<CombatSystem>();
        if (combatSystem == null)
        {
            Debug.LogError("[HeroBehaviour] CombatSystem not found!");
            return;
        }

        currentHealth = heroData.GetStartingHealth();
        _heroHealthBar.maxValue = currentHealth;
        _heroHealthBar.value = currentHealth;
        heroText.text = $"{heroData.Name} HP: {currentHealth}";

        combatSystem.RegisterHero(this);
    }

    // -------------------------------------------------------------------------
    // Animation Events — add these two events to your attack animation clip:
    //
    //   1. At the "hit" frame (when weapon connects):
    //      Function: OnAttackHit
    //
    //   2. At the very last frame of the clip:
//        Function: OnAttackEnd
    //
    // To add them: select the animation clip → open Animation window →
    // drag the playhead to the right frame → click "Add Event" → type the function name.
    // -------------------------------------------------------------------------

    // Called by Animation Event at the hit frame
    public void OnAttackHit()
    {
        attackHitFrame = true;
    }

    // Called by Animation Event at the last frame
    public void OnAttackEnd()
    {
        attackFinished = true;
    }

    // -------------------------------------------------------------------------
    // Called by CombatSystem during Hero Phase
    // -------------------------------------------------------------------------

    public void ExecuteAttack(EnemyBehaviour target, Action onFinished)
    {
        if (isHeroDead) { onFinished?.Invoke(); return; }
        StartCoroutine(AttackCoroutine(target, onFinished));
    }

    private IEnumerator AttackCoroutine(EnemyBehaviour target, Action onFinished)
    {
        attackHitFrame = false;
        attackFinished = false;
        particles.Stop();
        heroAnimator.SetBool("isAttacking", true);

        // Wait one frame for the Animator to transition into the Attack state
        yield return null;
        yield return null;

        // Wait until the attack state is actually playing
        yield return new WaitUntil(() =>
            heroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        AnimatorStateInfo stateInfo = heroAnimator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;

        // Hit at ~55% through the clip (matches your OnAttackHit at 0.85s / 1.53s ≈ 55%)
        yield return new WaitUntil(() =>
            heroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.55f
            || !heroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        if (target != null && !target.IsDead)
        {
            int damage = heroData.GetAttackDamage();
            target.TakeDamage(damage);
            Debug.Log($"[Hero] {heroData.Name} hit {target.name} for {damage}.");
        }

        // Wait until the clip is 98% done (don't wait for 100% - it may loop)
        yield return new WaitUntil(() =>
            heroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f
            || !heroAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        heroAnimator.SetBool("isAttacking", false);
        heroText.text = $"{heroData.Name} HP: {currentHealth}";
        onFinished?.Invoke();
    }
    // -------------------------------------------------------------------------
    // Receiving Damage
    // -------------------------------------------------------------------------

    public void TakeDamage(int damage)
    {
        if (isHeroDead) return;

        currentHealth -= damage;
        _heroHealthBar.value = currentHealth;
        particles.Play();
        var go = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = $"-{damage}";
        FloatingCombatText.Instance.Show(damage.ToString(), transform);

        heroText.text = $"{heroData.Name} HP: {currentHealth}";
        Debug.Log($"[Hero] {heroData.Name} took {damage}. HP left: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    // -------------------------------------------------------------------------
    // Death
    // -------------------------------------------------------------------------

    private void Die()
    {
        if (isHeroDead) return;
        isHeroDead = true;
        heroAnimator.SetBool("isAttacking", false);
        Debug.Log($"[Hero] {heroData.Name} died.");
        combatSystem?.OnHeroDied(this);
        Destroy(gameObject);
    }
}