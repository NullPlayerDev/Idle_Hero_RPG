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

    private int currentHealth;
    private bool isDead = false;
    private CombatSystem combatSystem;

    // Set by Animation Events on the attack clip:
    //   OnAttackHit  → at the weapon-connects frame
    //   OnAttackEnd  → at the very last frame of the clip
    private bool attackHitFrame = false;
    private bool attackFinished = false;

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        if (combatSystem == null)
        {
            Debug.LogError("[EnemyBehaviour] CombatSystem not found!");
            return;
        }

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

    public void OnAttackHit()  { attackHitFrame = true; }
    public void OnAttackEnd()  { attackFinished = true; }

    // -------------------------------------------------------------------------
    // Called by CombatSystem during Enemy Phase
    // `suggestedTarget` is the lowest-HP hero at the moment this enemy's turn
    // begins; we re-check at the actual hit frame in case it died mid-swing.
    // -------------------------------------------------------------------------

    public void ExecuteAttack(HeroBehaviour suggestedTarget, Action onFinished)
    {
        if (isDead) { onFinished?.Invoke(); return; }
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
        if (isDead) return;
        isDead = true;
        // Unblock any coroutine still waiting on the animation flags
        attackHitFrame = true;
        attackFinished = true;
        enemyAnimator.SetBool("isAttacking", false);
        Debug.Log($"[Enemy] {enemyData.Name} died.");
        combatSystem?.OnEnemyDied(this);
        Destroy(gameObject);
    }
}