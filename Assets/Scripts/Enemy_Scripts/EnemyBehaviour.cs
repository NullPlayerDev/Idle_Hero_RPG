using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Slider _enemyHealthBar;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private GameObject chestRewardsPrefab;
    private int currentHealth;
  
    private bool isDead = false;
    private CombatSystem combatSystem;
    private GameObject  chest;
    // Set by Animation Events on the attack clip:
    //   OnAttackHit  → at the weapon-connects frame
    //   OnAttackEnd  → at the very last frame of the clip
    private bool attackHitFrame = false;
    private bool attackFinished = false;
    private ChestTiers chestTiers;
    [SerializeField] private ChestRewards chestRewards;
    private RewardWallet rewardWallet;
    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;
    

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        rewardWallet = FindObjectOfType<RewardWallet>();
        if (combatSystem == null)
        {
            Debug.LogError("[EnemyBehaviour] CombatSystem not found!");
            return;
        }
        chestTiers =  FindObjectOfType<ChestTiers>();
        currentHealth = enemyData.GetStartingHealth();
        _enemyHealthBar.maxValue = currentHealth;
        _enemyHealthBar.value = currentHealth;
        textMeshPro.text = $"{enemyData.Name} HP: {currentHealth}";

        combatSystem.RegisterEnemy(this);
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
    // Called by CombatSystem during Enemy Phase
    // -------------------------------------------------------------------------

    public void ExecuteAttack(HeroBehaviour suggestedTarget, Action onFinished)
    {
        // Guard: don't start a coroutine on a dead or inactive GameObject
        if (isDead || !gameObject.activeInHierarchy)
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

        enemyAnimator.SetBool("isAttacking", true);

        // Wait for the hit frame
        yield return new WaitUntil(() => attackHitFrame);

        // Re-fetch the current lowest-HP hero at the moment of impact
        HeroBehaviour target = combatSystem.GetLowestHealthHero();
        if (target != null && !target.IsDead)
        {
            int damage = enemyData.GetAttackDamage();
            target.TakeDamage(damage);
            Debug.Log($"[Enemy] {enemyData.Name} hit {target.name} for {damage}.");
        }

        // Wait for the animation to fully finish
        yield return new WaitUntil(() => attackFinished);

        enemyAnimator.SetBool("isAttacking", false);
        textMeshPro.text = $"{enemyData.Name} HP: {currentHealth}";

        onFinished?.Invoke();
    }

    // -------------------------------------------------------------------------
    // Receiving Damage
    // -------------------------------------------------------------------------

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        _enemyHealthBar.value = currentHealth;

        var go = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = $"-{damage}";
        FloatingCombatText.Instance.Show(damage.ToString(), transform);

        textMeshPro.text = $"{enemyData.Name} HP: {currentHealth}";
        Debug.Log($"[Enemy] {enemyData.Name} took {damage}. HP left: {currentHealth}");

        if (currentHealth <= 0) Die();
    }

    // -------------------------------------------------------------------------
    // Death
    // -------------------------------------------------------------------------

    private void Die()
    {
        //chestTiers.ChestDrop();
         Instantiate(chestRewardsPrefab, transform.position, Quaternion.identity);
        rewardWallet.CurrentGems += chestRewards.gem;
        rewardWallet.CurrentGold += chestRewards.gold;
        if (isDead) return;
        isDead = true;

        // Unblock any coroutine still waiting on the animation flags
        attackHitFrame = true;
        attackFinished = true;

        enemyAnimator.SetBool("isAttacking", false);
        Debug.Log($"[Enemy] {enemyData.Name} died.");

        // Notify CombatSystem immediately so it stops targeting this enemy
        combatSystem?.OnEnemyDied(this);

        // Play death animation, then destroy
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Trigger your death animation — change "isDead" to match your Animator parameter name
        enemyAnimator.SetTrigger("isDead");

        // Wait for the death clip to finish before removing the GameObject
        float clipLength = GetAnimationClipLength("Death"); // change "Death" to your clip name
        yield return new WaitForSeconds(clipLength > 0f ? clipLength : 0.8f);
        Destroy(gameObject);
    }

    // Returns the length of an animation clip by name, or 0 if not found
    private float GetAnimationClipLength(string clipName)
    {
        if (enemyAnimator == null || enemyAnimator.runtimeAnimatorController == null) return 0f;
        foreach (AnimationClip clip in enemyAnimator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }
}