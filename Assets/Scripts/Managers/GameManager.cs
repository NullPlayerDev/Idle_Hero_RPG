using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string selectionSceneName = "PlayerSelection";
    [SerializeField] private string combatSceneName    = "CombatScene";

    [Header("UI")]
    [SerializeField] private GameObject      resultPanel;
    [SerializeField] private GameObject      winningPanel;
    [SerializeField] private GameObject      gameSelectionPanel;
    [SerializeField] private TextMeshProUGUI levelCounterText;

    [Header("Reward Calculator")]
    [SerializeField] private RewardCalculator rewardCalculator;

    // ── Hero selection buttons ─────────────────────────────────────────────────
    // Index 0 = button shown at Level 1
    // Index 1 = button shown at Level 2
    // etc.
    [Header("Hero Selection Buttons (index 0 = Level 1)")]
    [SerializeField] private List<GameObject> heroSelectionButtonList;

    // ── Runtime state ─────────────────────────────────────────────────────────
    private int _currentLevel   = 1;
    private int _totalStagesWon = 0;

    public int CurrentLevel
    {
        get => _currentLevel;
        set { _currentLevel = value; RefreshLevelUI(); }
    }

    public int TotalStagesWon
    {
        get => _totalStagesWon;
        set => _totalStagesWon = value;
    }

    // ── Events ────────────────────────────────────────────────────────────────
    public event Action<int> OnSelectionPhaseStarted;
    public event Action      OnCombatStarted;
    public event Action      OnStageEnded;

    // ─────────────────────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load save first — sets _currentLevel, gold, gems
        SaveSystem.Load();
        SaveConfigLoader.ProcessConfig();

        // Now show the correct button for the loaded level
        RefreshLevelUI();
        ShowButtonForLevel(_currentLevel);
        BeginSelectionPhase(_currentLevel);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Game flow
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called to restart the selection phase (e.g. after a stage ends).
    /// </summary>
    public void GameLoop()
    {
        DestroyAllObjects();

        winningPanel.SetActive(false);
        gameSelectionPanel.SetActive(true);

        CombatSystem.Instance.BattleStarted = false;
        CombatSystem.Instance.IsStageEnded  = false;

        ShowButtonForLevel(_currentLevel);
        BeginSelectionPhase(_currentLevel);
    }

    public void HandleStageEnded()
    {
        _totalStagesWon++;
        if (_totalStagesWon >= 10) _totalStagesWon = 0;

        OnStageEnded?.Invoke();
        rewardCalculator.CalculateReward();

        _currentLevel++;
        RefreshLevelUI();

        SaveSystem.Save();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Button logic — THE FIX
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Hides ALL hero selection buttons, then shows only the one
    /// that matches the current level.
    /// Level 1 → index 0, Level 2 → index 1, etc.
    /// </summary>
    private void ShowButtonForLevel(int level)
    {
        if (heroSelectionButtonList == null || heroSelectionButtonList.Count == 0)
        {
            Debug.LogWarning("[GameManager] heroSelectionButtonList is empty — no buttons to show.");
            return;
        }

        // FIX 1 & 2: Hide ALL buttons first, then show only the correct one
        /*for (int i = 0; i < heroSelectionButtonList.Count; i++)
        {
            if (heroSelectionButtonList[i] != null)
                heroSelectionButtonList[i].SetActive(false);
        }*/

        // FIX 1: Use level - 1 as the index (Level 1 = index 0)
        int index = level;

        // Clamp to last button once all heroes are unlocked
        //index = Mathf.Clamp(index, 0, heroSelectionButtonList.Count - 1);

        if (heroSelectionButtonList[index] != null && _currentLevel<=4)
        {
            for (int i = 0; i < _currentLevel; i++)
            {
                heroSelectionButtonList[i].SetActive(true);
                Debug.Log($"[GameManager] Showing hero button index {index} for Level {level}.");
            }

        }
        else
        {
            for (int i = 0; i < heroSelectionButtonList.Count; i++)
            {
                heroSelectionButtonList[i].SetActive(true);
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Save buttons (wire in Inspector)
    // ─────────────────────────────────────────────────────────────────────────

    public void OnSaveButton()
    {
        SaveSystem.Save();
        Debug.Log("[GameManager] Manual save triggered.");
    }

    public void OnDeleteSaveButton()
    {
        SaveSystem.DeleteAll();
        RefreshLevelUI();
        ShowButtonForLevel(_currentLevel);
        BeginSelectionPhase(_currentLevel);
        Debug.Log("[GameManager] Save deleted — game reset.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────────────

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

    private void RefreshLevelUI()
    {
        if (levelCounterText != null)
            levelCounterText.text = $"Level {_currentLevel}";
    }

    public void DestroyAllObjects()
    {
        foreach (var hero  in GameObject.FindGameObjectsWithTag("Hero"))  Destroy(hero);
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) Destroy(enemy);
        CombatSystem.Instance.CleanLists();
    }
}