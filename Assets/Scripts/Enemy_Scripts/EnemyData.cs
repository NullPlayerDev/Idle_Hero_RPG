using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Information")]
    public string Name;
    public EnemyEnums.VisualTypes VisualType;
    
    [Header("Stats Information")]
    public EnemyEnums.AttackSpeed attackSpeed;
    public EnemyEnums.AttackDamage attackDamage;
    //public EnemyEnums.SpawningTime spawningTime;
    
    [Header("RunTime Information")]
    public int Health;
    public int AttackRate;
    public int Damage;
    
    [Header("Health Information")]
    public EnemyEnums.HP hpType;
    //public EnemyEnums.DEATHTYPE deathType;
}



