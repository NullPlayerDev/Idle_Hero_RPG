using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles all save / load / delete operations.
/// Single save slot — one JSON file on disk.
///
/// IMPORTANT — Execution order:
///   SaveSystem is a static class. It only touches other singletons
///   (GameManager, RewardWallet, HeroSelectionManager) when explicitly
///   called. GameManager.Start() is the correct place to call Load().
/// </summary>
public static class SaveSystem
{
    private const string FILE_NAME    = "gamesave.json";
    private const int    SAVE_VERSION = 1;

    private static string SavePath => Path.Combine(Application.persistentDataPath, FILE_NAME);

    // ─── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Gathers current state from all singletons and writes to disk.
    /// Safe to call any time after Start().
    /// </summary>
    public static void Save()
    {
        SaveData data = CollectCurrentState();
        WriteToFile(data);
    }

    /// <summary>
    /// Reads the save file and pushes data into all singletons.
    /// Call once from GameManager.Start() — NOT from Awake().
    /// </summary>
    public static void Load()
    {
        SaveData data = ReadFromFile();
        ApplyToGame(data);
    }

    /// <summary>
    /// Deletes the save file and resets all singletons to defaults.
    /// </summary>
    public static void DeleteAll()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveSystem] Save file deleted.");
        }
        ApplyToGame(CreateDefaultSave());
        Debug.Log("[SaveSystem] Game reset to defaults.");
    }

    public static bool SaveExists() => File.Exists(SavePath);

    // ─── Collect ──────────────────────────────────────────────────────────────

    private static SaveData CollectCurrentState()
    {
        SaveData data = new SaveData();

        if (GameManager.Instance != null)
        {
            data.currentLevel   = GameManager.Instance.CurrentLevel;
            data.totalStagesWon = GameManager.Instance.TotalStagesWon;
        }
        else Debug.LogWarning("[SaveSystem] Save: GameManager.Instance is null — level not saved.");

        if (RewardWallet.Instance != null)
        {
            data.gold = RewardWallet.Instance.CurrentGold;
            data.gems = RewardWallet.Instance.CurrentGems;
        }
        else Debug.LogWarning("[SaveSystem] Save: RewardWallet.Instance is null — wallet not saved.");

        if (HeroSelectionManager.Instance != null)
        {
            // Use the existing SelectedHeroIds list directly — no extra method needed
            data.selectedHeroIDs = new List<int>(HeroSelectionManager.Instance.SelectedHeroIds);
        }

        data.StampDate();
        data.saveVersion = SAVE_VERSION;
        return data;
    }

    // ─── Apply ────────────────────────────────────────────────────────────────

    private static void ApplyToGame(SaveData data)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentLevel   = data.currentLevel;
            GameManager.Instance.TotalStagesWon = data.totalStagesWon;
        }
        else Debug.LogWarning("[SaveSystem] Load: GameManager.Instance is null — level not applied.");

        if (RewardWallet.Instance != null)
        {
            RewardWallet.Instance.SetGold(data.gold);
            RewardWallet.Instance.SetGems(data.gems);
        }
        else Debug.LogWarning("[SaveSystem] Load: RewardWallet.Instance is null — wallet not applied.");

        if (HeroSelectionManager.Instance != null)
        {
            HeroSelectionManager.Instance.SelectedHeroIds.Clear();
            HeroSelectionManager.Instance.SelectedHeroIds.AddRange(data.selectedHeroIDs);
        }

        Debug.Log($"[SaveSystem] Applied → Level {data.currentLevel}, Gold {data.gold}, Gems {data.gems}");
    }

    // ─── File I/O ─────────────────────────────────────────────────────────────

    private static void WriteToFile(SaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveSystem] Saved → {SavePath}");
    }

    private static SaveData ReadFromFile()
    {
        if (!SaveExists())
        {
            Debug.Log("[SaveSystem] No save file found — using defaults.");
            return CreateDefaultSave();
        }

        string   json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.saveVersion < SAVE_VERSION)
        {
            Debug.LogWarning($"[SaveSystem] Old version ({data.saveVersion}) — migrating.");
            data = MigrateSave(data);
        }

        return data;
    }

    // ─── Defaults & migration ─────────────────────────────────────────────────

    private static SaveData CreateDefaultSave() => new SaveData
    {
        gold           = 0,
        gems           = 0,
        currentLevel   = 1,
        totalStagesWon = 0,
        saveVersion    = SAVE_VERSION
    };

    private static SaveData MigrateSave(SaveData old)
    {
        old.saveVersion = SAVE_VERSION;
        return old;
    }
}