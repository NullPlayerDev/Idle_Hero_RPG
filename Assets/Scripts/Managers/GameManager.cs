using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sits on a DontDestroyOnLoad GameObject.
///
/// Scene flow:
///   PlayerSelection  →  SetupForLevel() unlocks buttons
///                       Player clicks HeroSelectButtons (by ID)
///                       Player clicks "Start Battle" → ConfirmHeroSelection()
///                       → LoadScene("CombatScene")
///
///   CombatScene      →  OnSceneLoaded fires → SpawnLevel()
///                       HeroSpawner reads IDs from HeroSelectionManager
///                       EnemySpawner reads current level
///                       CombatSystem runs the fight
///                       OnStageEnded → rewards → LoadScene("PlayerSelection")
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names — must match Build Settings exactly")]
    [SerializeField] private string selectionSceneName = "PlayerSelection";
    [SerializeField] private string combatSceneName    = "CombatScene";
    [SerializeField] private GameObject resultPanel;
    [Header("Reward Calculator (assign if in same scene as GameManager)")]
    [SerializeField] private RewardCalculator rewardCalculator;
    
    private int _currentLevel   = 1;
    private int _totalStagesWon = 0;

    public event Action<int> OnSelectionPhaseStarted;
    public event Action      OnCombatStarted;
    public event Action      OnStageEnded;

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    /*private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;*/

    private void Start()
    {
        // Game always starts in PlayerSelection scene
        BeginSelectionPhase(_currentLevel);
    }

    // -------------------------------------------------------------------------
    // Scene load hook
    // -------------------------------------------------------------------------

    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == combatSceneName)
        {
            CombatSystem cs = FindObjectOfType<CombatSystem>();
            if (cs != null) cs.OnStageEnded += HandleStageEnded;

            SpawnLevel();
            OnCombatStarted?.Invoke();
        }
        else if (scene.name == selectionSceneName)
        {
            BeginSelectionPhase(_currentLevel);
        }
    }

    // -------------------------------------------------------------------------
    // Public — wired to "Start Battle" button in PlayerSelection
    // -------------------------------------------------------------------------

    public void ConfirmHeroSelection()
    {
        if (HeroSelectionManager.Instance == null) return;

        if (!HeroSelectionManager.Instance.HasMinimumSelection)
        {
            Debug.LogWarning("[GameManager] Select at least one hero first.");
            return;
        }

        Debug.Log($"[GameManager] Confirmed. Loading CombatScene for level {_currentLevel}.");
        SceneManager.LoadScene(combatSceneName);
    }
    */

    // -------------------------------------------------------------------------
    // Internal
    // -------------------------------------------------------------------------

    private void BeginSelectionPhase(int level)
    {
        if (HeroSelectionManager.Instance == null)
        {
            Debug.LogError("[GameManager] HeroSelectionManager not found!");
            return;
        }
        HeroSelectionManager.Instance.SetupForLevel(level);
        OnSelectionPhaseStarted?.Invoke(level);
    }

    private void SpawnLevel()
    {
        /*
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.Level = _currentLevel;
            enemySpawner.BuildLevelBasedOnHeroNumber();
        }
        else Debug.LogWarning("[GameManager] EnemySpawner not found.");
        */

        /*
        HeroSpawner heroSpawner = FindObjectOfType<HeroSpawner>();
        if (heroSpawner != null)
            heroSpawner.SpawnSelectedHeroes();
        else Debug.LogWarning("[GameManager] HeroSpawner not found.");*/
    }

    public void HandleStageEnded()
    {
        /*resultPanel.SetActive(true);
        if (rewardCalculator != null)
        {
            rewardCalculator.CalculateGoldsReward();
            rewardCalculator.RewardInWallet();
        }*/

        _totalStagesWon++;
        OnStageEnded?.Invoke();
        if (_totalStagesWon >= 10)
        {
            _totalStagesWon = 0;
            _currentLevel   = 1;
        }
        else
        {
            _currentLevel++;
        }

        //SceneManager.LoadScene(selectionSceneName);
    }

    public void DeleteGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("[GameManager] Save data cleared.");
    }
}