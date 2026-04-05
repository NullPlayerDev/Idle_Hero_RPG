using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Scriptable Objects/HeroData")]
public class HeroData : ScriptableObject
{
    [Header("Basic Information")]
    public string Name;
    public HeroNums.VisualTypes VisualType;
    
    [Header("Stats Information")]
    public HeroNums.AttackSpeed attackSpeed;
    public HeroNums.AttackDamage attackDamage;
    [Header("RunTime Information")]
    public int Health;
    public int AttackRate;
    public int Damage;
    
        
    [Header("Health Information")]
    public HeroNums.HP hpType;
}
