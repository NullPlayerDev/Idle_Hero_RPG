using UnityEngine;
using UnityEngine.UI;   // FIX: was UnityEngine.UIElements (wrong namespace — breaks button clicks)

public class Heroselectionui : MonoBehaviour
{
    [SerializeField] private int id;

    private CombatSystem  combatSystem;
    private EnemySpawner  enemySpawner;
    private HeroBehaviour heroBehaviour;
    private HeroData      heroData;

    [SerializeField] private GameObject heroPlayer;

    void Start()
    {
        combatSystem  = GameObject.Find("CombatManager").GetComponent<CombatSystem>();
        enemySpawner  = FindAnyObjectByType<EnemySpawner>();
        heroBehaviour = heroPlayer.GetComponent<HeroBehaviour>();
        heroData      = heroBehaviour.HeroData;
    }

    /// <summary>
    /// Called by the UI Button's OnClick event.
    /// Adds this hero to the combat list and marks it selected.
    /// </summary>
    public void SelectHero()
    {
        if (heroData == null)
        {
            Debug.LogError($"[HeroSelectionUI] heroData is null on button ID {id}.");
            return;
        }

        // Register in HeroSelectionManager (tracks IDs for save/load)
        HeroSelectionManager.Instance?.ToggleHero(heroData.ID);

        // Add to combat system hero list
        if (!combatSystem.Heroes.Contains(heroBehaviour))
            combatSystem.Heroes.Add(heroBehaviour);

        // Rebuild enemy wave based on hero count
        enemySpawner.BuildLevelBasedOnHeroNumber();

        heroData.isSelected = true;

        Debug.Log($"[HeroSelectionUI] Hero ID {heroData.ID} selected.");
    }
}