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
    [SerializeField] private GameObject heroPrefab;
    private int currentHealth;
    private bool isHeroDead = false;
    private CombatSystem combatSystem;

    // Set by Animation Events on the attack clip:
    //   OnAttackHit  → at the weapon-connects frame
    //   OnAttackEnd  → at the very last frame of the clip
    private bool attackHitFrame = false;
    private bool attackFinished = false;

    public GameObject HeroPrefab
    {
        get => heroPrefab;
        set => heroPrefab = value;
    }
    public HeroData HeroData
    {
        get => heroData;
        set => heroData = value;
    }
    public bool IsDead => isHeroDead;
    public int CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
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
    // Animation Events
    // Add these two events to your attack animation clip:
    //   1. At the "hit" frame  → Function: OnAttackHit
    //   2. At the last frame   → Function: OnAttackEnd
    // -------------------------------------------------------------------------

    public void OnAttackHit() { attackHitFrame = true; }
    public void OnAttackEnd() { attackFinished = true; }

    // -------------------------------------------------------------------------
    // Called by CombatSystem during Hero Phase
    // -------------------------------------------------------------------------

    public void ExecuteAttack(EnemyBehaviour suggestedTarget, Action onFinished)
    {
        // Guard: don't start a coroutine on a dead or inactive GameObject
        if (isHeroDead || !gameObject.activeInHierarchy)
        {
            onFinished?.Invoke();
            return;
        }
        StartCoroutine(AttackCoroutine(onFinished));
    }

    private IEnumerator AttackCoroutine(Action onFinished)
    {
        attackHitFrame = false;
        attackFinished = false;

        heroAnimator.SetBool("isAttacking", true);

        // Wait for the hit frame
        yield return new WaitUntil(() => attackHitFrame);

        // Re-fetch the current lowest-HP enemy at the moment of impact
        EnemyBehaviour target = combatSystem.GetLowestHealthEnemy();
        if (target != null && !target.IsDead)
        {
            int damage = heroData.GetAttackDamage();
            target.TakeDamage(damage);
            Debug.Log($"[Hero] {heroData.Name} hit {target.name} for {damage}.");
        }

        // Wait for the animation to fully finish
        yield return new WaitUntil(() => attackFinished);

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

        // Unblock any coroutine still waiting on the animation flags
        attackHitFrame = true;
        attackFinished = true;

        heroAnimator.SetBool("isAttacking", false);
        Debug.Log($"[Hero] {heroData.Name} died.");

        // Notify CombatSystem immediately so it stops targeting this hero
        combatSystem?.OnHeroDied(this);

        // Play death animation, then destroy
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Trigger your death animation — change "isDead" to match your Animator parameter name
        heroAnimator.SetTrigger("isDead");

        // Wait for the death clip to finish before removing the GameObject
        float clipLength = GetAnimationClipLength("Death"); // change "Death" to your clip name
        yield return new WaitForSeconds(clipLength > 0f ? clipLength : 0.8f);

        Destroy(gameObject);
    }

    // Returns the length of an animation clip by name, or 0 if not found
    private float GetAnimationClipLength(string clipName)
    {
        if (heroAnimator == null || heroAnimator.runtimeAnimatorController == null) return 0f;
        foreach (AnimationClip clip in heroAnimator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }
}