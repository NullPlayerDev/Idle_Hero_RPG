
using System.Collections;
using UnityEngine;

public class HeroBehaviour : MonoBehaviour
{
    [SerializeField] private HeroData heroData;

    // Runtime health — initialised from heroData on Start.
    private int currentHealth;
    private bool isHeroDead = false;
    private Coroutine attackCoroutine;

    // Reference to the CombatSystem manager — found at Start via FindObjectOfType.
    private CombatSystem combatSystem;

    public bool IsDead => isHeroDead;
    public int CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        // CombatSystem lives on a separate manager GameObject — never GetComponent on self.
        combatSystem = FindObjectOfType<CombatSystem>();

        if (combatSystem == null)
            Debug.LogError($"[HeroBehaviour] {heroData.Name}: CombatSystem not found in scene!");

        // Initialise runtime health from the ScriptableObject.
        currentHealth = heroData.GetStartingHealth();

        // Start the auto-attack loop.
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    void OnDestroy()
    {
        // Stop the coroutine cleanly when the GameObject is destroyed.
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }

    // -------------------------------------------------------------------------
    // Combat
    // -------------------------------------------------------------------------

    // Called by the attack coroutine every cooldown interval.
    private void AttackEnemy()
    {
        if (isHeroDead) return;

        // Ask the CombatSystem for a valid enemy target.
        EnemyBehaviour target = combatSystem.GetRandomEnemy();

        if (target == null || target.IsDead) return;

        int damage = heroData.GetAttackDamage();
        target.TakeDamage(damage);

        Debug.Log($"[Hero] {heroData.Name} attacks {target.name} for {damage} damage.");
    }

    // Called by CombatSystem or EnemyBehaviour when this hero takes a hit.
    public void TakeDamage(int damage)
    {
        if (isHeroDead) return;

        currentHealth -= damage;
        Debug.Log($"[Hero] {heroData.Name} took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isHeroDead) return;   // Guard: Die() can only execute once.

        isHeroDead = true;
        Debug.Log($"[Hero] {heroData.Name} died.");

        // Notify the CombatSystem before destroying.
        combatSystem?.OnHeroDied(this);

        Destroy(gameObject);
    }

    // -------------------------------------------------------------------------
    // Coroutines
    // -------------------------------------------------------------------------

    private IEnumerator AttackRoutine()
    {
        float cooldown = heroData.GetAttackCooldown();

        // Small initial delay so the scene has time to fully initialise.
        yield return new WaitForSeconds(1f);

        while (!isHeroDead)
        {
            AttackEnemy();
            yield return new WaitForSeconds(cooldown);
        }
    }
}
