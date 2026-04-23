using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private CombatSystem combatSystem;
    private int level;

    public int Level
    {
        get => level;
        set => level = value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSystem = FindObjectOfType<CombatSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RandomEnemySpawner()
    {
        
    }
}
