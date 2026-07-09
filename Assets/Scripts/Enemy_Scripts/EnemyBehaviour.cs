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
    [SerializeField] private ParticleSystem particles;

    public CameraShaking cameraShaking;
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
    
    [SerializeField] private float attackDashDuration = 0.25f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float slowMotionScale = 0.2f;
    [SerializeField] private float slowMotionDuration = 0.3f;

    private Vector3 originalPosition;
    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
        rewardWallet = FindObjectOfType<RewardWallet>();
        cameraShaking = FindObjectOfType<CameraShaking>();
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

    originalPosition = transform.position;

    enemyAnimator.SetBool("isAttacking", true);
   
    // Wait until attack animation reaches the hit frame
    yield return new WaitUntil(() => attackHitFrame);

    HeroBehaviour target = combatSystem.GetLowestHealthHero();

    if (target != null && !target.IsDead)
    {
        // Slow motion
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        Vector3 attackPosition;

        // Move toward the target's actual position (X and Y), stopping
        // attackDistance short of it, so we visibly approach whoever we're
        // attacking instead of only sliding along X.
        Vector3 targetPos = target.transform.position;
        Vector3 direction = targetPos - originalPosition;
        direction.z = 0f; // ignore Z so we don't mess with sprite sorting/depth

        float dist = direction.magnitude;
        Vector3 dirNormalized = dist > 0.001f ? direction / dist : Vector3.left;

        attackPosition = targetPos - dirNormalized * attackDistance;
        attackPosition.z = originalPosition.z; // keep our own depth/sorting
        // Dash toward enemy
        float elapsed = 0f;

        while (elapsed < attackDashDuration)
        {
            elapsed += Time.deltaTime;

            transform.position = Vector3.Lerp(
                originalPosition,
                attackPosition,
                elapsed / attackDashDuration);

            yield return null;
        }

        transform.position = attackPosition;
        //transform.position = attackPosition;
        
        ////////////////////////     
        // Restore normal speed
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        yield return new WaitUntil(() => attackFinished);

        // Damage
        int damage = enemyData.GetAttackDamage();
        Debug.Log("About to damage hero");
      
        target.TakeDamage(damage);

        Debug.Log("Hero damaged");
        Debug.Log("Damage = " + damage);

        /*particles.Play();*/

        Debug.Log($"[Enemy] {enemyData.Name} hit {target.name} for {damage}");
   
       
        enemyAnimator.SetBool("isAttacking", false);

        textMeshPro.text = $"{enemyData.Name} HP: {currentHealth}";
        //////////////////////////////
        // Return to original position
        elapsed = 0f;

        Vector3 startReturnPos = transform.position;

        while (elapsed < attackDashDuration)
        {
            elapsed += Time.deltaTime;

            transform.position = Vector3.Lerp(
                startReturnPos,
                originalPosition,
                elapsed / attackDashDuration);

            yield return null;
        }

        Debug.Log($"Attack Distance: {attackDistance}");
//        Debug.Log($"Target Position: {target.transform.position}");
        Debug.Log($"Attack Position: {attackPosition}");

  
    }


    transform.position = originalPosition;
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

        if (cameraShaking == null)
            cameraShaking = FindObjectOfType<CameraShaking>();

        if (cameraShaking != null)
            StartCoroutine(cameraShaking.ShakingTime());
        else
            Debug.LogWarning("[EnemyBehaviour] CameraShaking still not found in scene — skipping shake.");
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
        if (this.enemyData.hpType == EnemyEnums.HP.HIGH || this.enemyData.hpType == EnemyEnums.HP.VERY_HIGH)
        {
            Instantiate(chestRewardsPrefab, transform.position, Quaternion.identity);
        }

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