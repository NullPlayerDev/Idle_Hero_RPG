// HeroData.cs
// ScriptableObject that stores all design-time stats for a hero type.
// Create via: Assets > Create > Scriptable Objects > HeroData

using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Scriptable Objects/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("Basic Information")] 
    public int ID;
    public string Name;
    public HeroNums.VisualTypes VisualType;

    [Header("Stats (Design Time)")]
    public HeroNums.AttackSpeed attackSpeed;
    public HeroNums.AttackDamage attackDamage;
    public HeroNums.HP hpType;
    public bool isSelected;
    // Returns a fresh runtime health value based on hpType.
    // Call this when spawning the hero — do NOT read it as a persistent field.
    public int GetStartingHealth()
    {
        return HeroNums.GetMaxHealth(hpType);
    }

    public float GetAttackCooldown()
    {
        return HeroNums.GetAttackCooldown(attackSpeed);
    }

    public int GetAttackDamage()
    {
        return HeroNums.GetAttackDamage(attackDamage);
    }
}
