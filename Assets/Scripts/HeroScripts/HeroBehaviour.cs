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

    private bool attackHitFrame = false;
    private bool attackFinished = false;

    // ── Gear-boosted stats (set once in Start) ────────────────────────────────
    private int   _effectiveDamage;
    private float _effectiveCooldown;
    [SerializeField] private float attackDashDuration = 0.25f;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float slowMotionScale = 0.2f;
    [SerializeField] private float slowMotionDuration = 0.3f;
    public CameraShaking cameraShaking;

    private Vector3 originalPosition;
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
    public bool IsDead       => isHeroDead;
    public int  CurrentHealth => currentHealth;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------
    private void Awake()
    {
        cameraShaking = FindAnyObjectByType<CameraShaking>();
    }

    void Start()
    {
        if (cameraShaking == null)
        {
            Debug.LogError("CameraShaking NOT FOUND!");
        }
        else
        {
            Debug.Log("Found CameraShaking on: " + cameraShaking.gameObject.name);
        }
        combatSystem = FindObjectOfType<CombatSystem>();
     
        if (combatSystem == null)
        {
            Debug.LogError("[HeroBehaviour] CombatSystem not found!");
            return;
        }

        ApplyGearStats();

        _heroHealthBar.maxValue = currentHealth;
        _heroHealthBar.value    = currentHealth;
        heroText.text = $"{heroData.Name} HP: {currentHealth}";

        combatSystem.RegisterHero(this);
    }

    // -------------------------------------------------------------------------
    // Gear stat application
    // Reads equipped gear from GearInventory and adds bonuses on top of HeroData.
    // Called once per spawn — gear changes take effect next combat.
    // -------------------------------------------------------------------------

    private void ApplyGearStats()
    {
        int   baseHp       = heroData.GetStartingHealth();
        int   baseDamage   = heroData.GetAttackDamage();
        float baseCooldown = heroData.GetAttackCooldown();

        if (GearInventory.Instance != null)
        {
            int   hpBonus    = GearInventory.Instance.TotalHpBonus();
            int   dmgBonus   = GearInventory.Instance.TotalDamageBonus();
            float spdBonus   = GearInventory.Instance.TotalSpeedBonus();

            currentHealth      = baseHp + hpBonus;
            _effectiveDamage   = baseDamage + dmgBonus;
            // Clamp cooldown so it never drops below 0.5 seconds
            _effectiveCooldown = Mathf.Max(0.5f, baseCooldown - spdBonus);

            Debug.Log($"[HeroBehaviour] {heroData.Name} gear stats → " +
                      $"HP:{currentHealth}(+{hpBonus})  DMG:{_effectiveDamage}(+{dmgBonus})  " +
                      $"Cooldown:{_effectiveCooldown:F2}s(-{spdBonus:F2}s)");
        }
        else
        {
            // No GearInventory in scene — use raw base stats
            currentHealth      = baseHp;
            _effectiveDamage   = baseDamage;
            _effectiveCooldown = baseCooldown;
        }
    }

    // -------------------------------------------------------------------------
    // Animation Events
    // -------------------------------------------------------------------------

    public void OnAttackHit() { attackHitFrame = true; }
    public void OnAttackEnd() { attackFinished = true; }

    // -------------------------------------------------------------------------
    // Called by CombatSystem during Hero Phase
    // -------------------------------------------------------------------------

    public void ExecuteAttack(EnemyBehaviour suggestedTarget, Action onFinished)
    {
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

    originalPosition = transform.position;

    heroAnimator.SetBool("isAttacking", true);

    yield return new WaitUntil(() => attackHitFrame);

    EnemyBehaviour target = combatSystem.GetLowestHealthEnemy();

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
        Vector3 dirNormalized = dist > 0.001f ? direction / dist : Vector3.right;

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

        /////////////////////////////    
        // Restore time
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        yield return new WaitUntil(() => attackFinished);

        // Damage
        int damage = _effectiveDamage;
        target.TakeDamage(damage);

        Debug.Log($"[Hero] {heroData.Name} hit {target.name} for {damage}");

        heroAnimator.SetBool("isAttacking", false);

        heroText.text = $"{heroData.Name} HP: {currentHealth}";
        //////////////////////
        
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

        transform.position = originalPosition;

    }



    onFinished?.Invoke();
}
    // -------------------------------------------------------------------------
    // Receiving Damage
    // -------------------------------------------------------------------------

    public void TakeDamage(int damage)
    {
        if (isHeroDead)
            return;

        currentHealth -= damage;

        Debug.Log($"Hero HP after damage = {currentHealth}");

        _heroHealthBar.value = currentHealth;

        if (cameraShaking == null)
            cameraShaking = FindAnyObjectByType<CameraShaking>();

        if (cameraShaking != null)
            StartCoroutine(cameraShaking.ShakingTime());
        else
            Debug.LogWarning("[HeroBehaviour] CameraShaking still not found in scene — skipping shake.");
        var go = Instantiate(textPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = $"-{damage}";
        FloatingCombatText.Instance.Show(damage.ToString(), transform);
        
        heroText.text = $"{heroData.Name} HP: {currentHealth}";
        if (currentHealth <= 0)
        {
            Debug.Log("Hero died");
            Die();
        }
    }

    // -------------------------------------------------------------------------
    // Death
    // -------------------------------------------------------------------------

    private void Die()
    {
        if (isHeroDead) return;
        isHeroDead = true;

        attackHitFrame = true;
        attackFinished = true;

        heroAnimator.SetBool("isAttacking", false);
        Debug.Log($"[Hero] {heroData.Name} died.");

        combatSystem?.OnHeroDied(this);
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        heroAnimator.SetTrigger("isDead");
        float clipLength = GetAnimationClipLength("Death");
        yield return new WaitForSeconds(clipLength > 0f ? clipLength : 0.8f);
        Destroy(gameObject);
    }

    private float GetAnimationClipLength(string clipName)
    {
        if (heroAnimator == null || heroAnimator.runtimeAnimatorController == null) return 0f;
        foreach (AnimationClip clip in heroAnimator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0f;
    }
}