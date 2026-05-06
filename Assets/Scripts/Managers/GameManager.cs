using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private CombatSystem  _combatSystem;
    [Header("Scene Names — must match Build Settings exactly")]
    [SerializeField] private string selectionSceneName = "PlayerSelection";
    [SerializeField] private string combatSceneName    = "CombatScene";
    [SerializeField] private GameObject resultPanel;
    [Header("Reward Calculator (assign if in same scene as GameManager)")]
    [SerializeField] private RewardCalculator rewardCalculator;

    [SerializeField] private GameObject winningPanel;
    [SerializeField] private int _currentLevel   = 1;
    private int _totalStagesWon = 0;
    [SerializeField] private GameObject gameSelectionPanel;
    [SerializeField] private TextMeshProUGUI levelCounterText;
    public event Action<int> OnSelectionPhaseStarted;
    public event Action      OnCombatStarted;
    public event Action      OnStageEnded;

    [SerializeField] private List<GameObject> heroSelectionButtonList;
    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /*private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;*/

    private void Start()
    {
        // Game always starts in PlayerSelection scene
        BeginSelectionPhase(_currentLevel);
        levelCounterText.text = $"Level {_currentLevel}";
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
    public void GameLoop()
    {
        DestroyAllObjects();
        OnSelectionPhaseStarted?.Invoke(_currentLevel);
        //SceneManager.LoadScene("CombatScene");
        gameSelectionPanel.SetActive(true);
        if (_currentLevel <= 5)
        {
            heroSelectionButtonList[_currentLevel].SetActive(true);
        }
        winningPanel.SetActive(false);
        BeginSelectionPhase(_currentLevel);
        CombatSystem.Instance.BattleStarted = false;
        CombatSystem.Instance.IsStageEnded = false;
        //CombatSystem.Instance.TryStartBattle();
    }
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
        //DestroyAllObjects();
        _totalStagesWon++;
        OnStageEnded?.Invoke();
        
        //This is needed because the totalStagesWon will give extra 
        // Tier or power to the player
        if (_totalStagesWon >= 10)
        {
            _totalStagesWon = 0;
        }
        rewardCalculator.CalculateReward();
        _currentLevel++;
        levelCounterText.text = $"Level {_currentLevel}";

        //SceneManager.LoadScene(selectionSceneName);
    }

    public void DestroyAllObjects()
    {
        foreach (var hero in GameObject.FindGameObjectsWithTag("Hero"))
        {
            Destroy(hero);
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        CombatSystem.Instance.CleanLists();
    }
    public void DeleteGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("[GameManager] Save data cleared.");
    }
}