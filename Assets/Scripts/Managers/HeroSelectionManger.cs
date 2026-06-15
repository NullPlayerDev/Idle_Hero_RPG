using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that persists from PlayerSelection scene into CombatScene.
/// Stores which hero IDs the player picked and how many slots are available
/// for the current level.
///
/// Unlock schedule:
///   Level 1  → 1 slot   (only hero ID 0 available)
///   Level 2  → 1 slot   (IDs 0–1 available)
///   Level 3  → 2 slots  (IDs 0–2 available)
///   Level 4  → 3 slots  (IDs 0–3 available)
///   Level 5+ → 4 slots  (all IDs available)
/// </summary>
public class HeroSelectionManager : MonoBehaviour
{
    public static HeroSelectionManager Instance { get; private set; }

    public event Action OnSelectionChanged;

    public List<int> SelectedHeroIds { get; private set; } = new List<int>();
    public int MaxSelectable { get; private set; } = 1;
    public int MaxUnlockedId { get; private set; } = 0;
    public int CurrentLevel  { get; private set; } = 1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void SetupForLevel(int level)
    {
        CurrentLevel = level;
        SelectedHeroIds.Clear();

        if      (level <= 1) { MaxSelectable = 1; MaxUnlockedId = 0;  }
        else if (level == 2) { MaxSelectable = 1; MaxUnlockedId = 1;  }
        else if (level == 3) { MaxSelectable = 2; MaxUnlockedId = 2;  }
        else if (level == 4) { MaxSelectable = 3; MaxUnlockedId = 3;  }
        else                 { MaxSelectable = 4; MaxUnlockedId = 99; }

        Debug.Log($"[HeroSelectionManager] Level {level}: {MaxSelectable} slot(s), IDs 0–{MaxUnlockedId} unlocked.");
        OnSelectionChanged?.Invoke();
    }

    public bool IsUnlocked(int heroId) => heroId <= MaxUnlockedId;
    public bool IsSelected(int heroId) => SelectedHeroIds.Contains(heroId);

    public bool ToggleHero(int heroId)
    {
        if (!IsUnlocked(heroId))
        {
            Debug.LogWarning($"[HeroSelectionManager] Hero ID {heroId} is locked at level {CurrentLevel}.");
            return false;
        }

        if (SelectedHeroIds.Contains(heroId))
        {
            SelectedHeroIds.Remove(heroId);
            Debug.Log($"[HeroSelectionManager] Deselected hero ID {heroId}.");
            OnSelectionChanged?.Invoke();
            return true;
        }

        if (SelectedHeroIds.Count >= MaxSelectable)
        {
            Debug.LogWarning($"[HeroSelectionManager] All {MaxSelectable} slot(s) filled.");
            return false;
        }

        SelectedHeroIds.Add(heroId);
        Debug.Log($"[HeroSelectionManager] Selected hero ID {heroId}. ({SelectedHeroIds.Count}/{MaxSelectable})");
        OnSelectionChanged?.Invoke();
        return true;
    }

    public bool HasMinimumSelection => SelectedHeroIds.Count > 0;
    public bool IsSelectionFull     => SelectedHeroIds.Count >= MaxSelectable;
}