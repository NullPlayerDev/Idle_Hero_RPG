using System.IO;
using UnityEngine;

/// <summary>
/// Static save/load utility. Writes JSON to Application.persistentDataPath.
/// No MonoBehaviour needed — call from anywhere.
///
/// Usage:
///   SaveSystem.Save(data);
///   SaveData data = SaveSystem.Load();
///   SaveSystem.Delete();
///   bool exists = SaveSystem.SaveExists();
/// </summary>
public static class SaveSystem
{
    private const string FILE_NAME    = "gamesave.json";
    private const int    SAVE_VERSION = 1;

    private static string SavePath => Path.Combine(Application.persistentDataPath, FILE_NAME);

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>Serializes <paramref name="data"/> to JSON and writes it to disk.</summary>
    public static void Save(SaveData data)
    {
        data.StampDate();
        data.saveVersion = SAVE_VERSION;

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveSystem] Game saved → {SavePath}");
    }

    /// <summary>
    /// Reads the save file and returns a populated <see cref="SaveData"/>.
    /// Returns a fresh default SaveData if no file exists.
    /// </summary>
    public static SaveData Load()
    {
        if (!SaveExists())
        {
            Debug.Log("[SaveSystem] No save file found — returning defaults.");
            return CreateDefaultSave();
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Version migration hook — add cases here as schema evolves
        if (data.saveVersion < SAVE_VERSION)
        {
            Debug.LogWarning($"[SaveSystem] Old save version ({data.saveVersion}) — migrating.");
            data = MigrateSave(data);
        }

        Debug.Log($"[SaveSystem] Save loaded (level {data.currentLevel}, " +
                  $"gold {data.gold}, gems {data.gems})");
        return data;
    }

    /// <summary>Deletes the save file from disk.</summary>
    public static void Delete()
    {
        if (!SaveExists()) return;
        File.Delete(SavePath);
        Debug.Log("[SaveSystem] Save file deleted.");
    }

    /// <returns>True if a save file exists on disk.</returns>
    public static bool SaveExists() => File.Exists(SavePath);

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static SaveData CreateDefaultSave()
    {
        return new SaveData
        {
            gold          = 0,
            gems          = 0,
            currentLevel  = 1,
            totalStagesWon = 0,
            saveVersion   = SAVE_VERSION
        };
    }

    /// <summary>
    /// Add migration logic here when SaveData schema changes between versions.
    /// </summary>
    private static SaveData MigrateSave(SaveData old)
    {
        // Example: if (old.saveVersion == 0) { old.newField = defaultValue; }
        old.saveVersion = SAVE_VERSION;
        return old;
    }
}