using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private EnemyEnums _enemyEnums;
    //[SerializeField] private HeroData heroData;
    private float timer;
    private float attackCooldown, lastAttackTime;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon");
        StartCoroutine(AttackRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /*
     * Normal Functions

     */
    
    public void SpawnPosition()
    {
        //Spawn position
        //is the place allocated/not empty return 
        // spawn
        
    }
    public void AttackHero()
    {
        if (_enemyData.Health <= 0) Death();
        if (_enemyData.AttackRate <= 0) return;
        //if(heroData.Health<=0) return;
        /*
         if(heroData.heroHP>=0)
         {
            //hero data theke hero er HP kombe
            //attackhero function e parameter pass korte hobe
         }

         */
    }
    public void DamageFromHeroUi()
    {
        /*
          * Ui will update 
         */
        
    }

    public void DamageHero()
    {
        
    }

    public void Death()
    {
        //The enemy death
        Destroy(gameObject); 
        Debug.Log("DEAD");
        
    }
    
    //attack hero after sometime
    IEnumerator AttackRoutine()
    {
        float cooldown = _enemyEnums.GetAttackCooldown(_enemyData.attackSpeed);

        while (true)
        {
            AttackHero();
            yield return new WaitForSeconds(cooldown);
        }
    }
    
    
    //Hero Health Decrease
    //Damage Hero
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //heroes health will decrease
            //HealthUpdate();
            if (_enemyData.Health <= 0)
            {
                Death();
            }
        }
      
    }
}
