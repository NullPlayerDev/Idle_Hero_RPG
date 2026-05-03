using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Heroselectionui : MonoBehaviour
{
    [SerializeField] private int id;
     private CombatSystem combatSystem;
     private EnemySpawner enemySpawner;
    [SerializeField] private GameObject heroPlayer;
    //[SerializeField] private Transform spawnPoint;
    private HeroData heroData;
    private HeroBehaviour heroBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSystem  = GameObject.Find("CombatManager").GetComponent<CombatSystem>();
        heroBehaviour = heroPlayer.GetComponent<HeroBehaviour>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        heroData = heroBehaviour.HeroData;
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectHero()
    {
        combatSystem.Heroes.Add(heroBehaviour);
        enemySpawner.BuildLevelBasedOnHeroNumber();
        heroData.isSelected = true;
        //return id;
    }
}
