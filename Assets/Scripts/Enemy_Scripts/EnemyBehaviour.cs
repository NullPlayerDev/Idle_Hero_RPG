
using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    // Runtime health — initialised from enemyData on Start.
    private int currentHealth;
    private bool isDead = false;
    private Coroutine attackCoroutine;

    // Reference to the CombatSystem manager — found at Start via FindObjectOfType.
    private CombatSystem combatSystem;

    public bool IsDead => isDead;
    public int CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        // CombatSystem lives on a separate manager GameObject — never GetComponent on self.
        combatSystem = FindObjectOfType<CombatSystem>();

        if (combatSystem == null)
            Debug.LogError($"[EnemyBehaviour] {enemyData.Name}: CombatSystem not found in scene!");

        // Initialise runtime health from the ScriptableObject.
        currentHealth = enemyData.GetStartingHealth();

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
    private void AttackHero()
    {
        if (isDead) return;

        // Ask the CombatSystem for a valid hero target.
        HeroBehaviour target = combatSystem.GetRandomHero();

        if (target == null || target.IsDead) return;

        int damage = enemyData.GetAttackDamage();
        target.TakeDamage(damage);
        textMeshPro.text = $"[Enemy] {enemyData.Name} Health: {currentHealth}";
        Debug.Log($"[Enemy] {enemyData.Name} attacks {target.name} for {damage} damage.");
    }

    // Called by EnemyBehaviour itself or by CombatSystem when this enemy takes a hit.
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"[Enemy] {enemyData.Name} took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;   // Guard: Die() can only execute once.

        isDead = true;
        Debug.Log($"[Enemy] {enemyData.Name} died.");

        // Notify the CombatSystem before destroying.
        combatSystem?.OnEnemyDied(this);

        Destroy(gameObject);
    }

    // -------------------------------------------------------------------------
    // Coroutines
    // -------------------------------------------------------------------------

    private IEnumerator AttackRoutine()
    {
        float cooldown = enemyData.GetAttackCooldown();

        // Small initial delay so the scene has time to fully initialise.
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            AttackHero();
            yield return new WaitForSeconds(cooldown);
        }
    }
}
