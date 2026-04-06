using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Information")]
    public string Name;
    public EnemyEnums.VisualTypes VisualType;

    [Header("Stats (Design Time)")]
    public EnemyEnums.AttackSpeed attackSpeed;
    public EnemyEnums.AttackDamage attackDamage;
    public EnemyEnums.HP hpType;

    // Returns a fresh runtime health value based on hpType.
    // Call this when spawning the enemy — do NOT read it as a persistent field.
    public int GetStartingHealth()
    {
        return EnemyEnums.GetMaxHealth(hpType);
    }

    public float GetAttackCooldown()
    {
        return EnemyEnums.GetAttackCooldown(attackSpeed);
    }

    public int GetAttackDamage()
    {
        return EnemyEnums.GetAttackDamage(attackDamage);
    }
}
