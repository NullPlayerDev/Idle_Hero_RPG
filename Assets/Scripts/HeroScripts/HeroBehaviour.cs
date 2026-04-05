using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HeroBehaviour : MonoBehaviour
{
    [SerializeField] private HeroData _heroData;
    [SerializeField] private HeroNums _heroENums;
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
        if (_heroData.Health <= 0) Death();
        if (_heroData.AttackRate <= 0) return;
        //if(heroData.Health<=0) return;
        /*
         if(heroData.heroHP>=0)
         {
            //enemy data theke enemy er HP kombe
            //attackEnemy function e parameter pass korte hobe
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
        float cooldown = _heroENums.GetAttackCooldown(_heroData.attackSpeed);

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
        if (collision.collider.CompareTag("Enemy"))
        {
            //heroes health will decrease
            _heroData.Health--;
            if (_heroData.Health <= 0)
            {
                Death();
            }
        }
      
    }
}