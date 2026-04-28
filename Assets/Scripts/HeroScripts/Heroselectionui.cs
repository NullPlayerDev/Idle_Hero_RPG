using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Heroselectionui : MonoBehaviour
{
    [SerializeField] private int id;
     private CombatSystem combatSystem;
    [SerializeField] private GameObject heroPlayer;
    private HeroData heroData;
    private HeroBehaviour heroBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSystem  = GameObject.Find("CombatManager").GetComponent<CombatSystem>();
        heroBehaviour = heroPlayer.GetComponent<HeroBehaviour>();
        heroData = heroBehaviour.HeroData;
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectHero()
    {
        combatSystem.Heroes.Add(heroBehaviour);
        heroData.isSelected = true;
        //return id;
    }
}
