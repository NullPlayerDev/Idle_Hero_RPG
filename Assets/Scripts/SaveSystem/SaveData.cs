using System;
using System.Collections.Generic;

/// <summary>
/// Plain serializable snapshot of everything that needs to survive between sessions.
/// No MonoBehaviour — just data. JsonUtility can serialize this directly.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Wallet ────────────────────────────────────────────────────────────────
    public int gold;
    public int gems;

    // ── Progression ───────────────────────────────────────────────────────────
    public int currentLevel;
    public int totalStagesWon;

    // ── Hero selection (stores HeroData.ID of each selected hero) ────────────
    public List<int> selectedHeroIDs = new List<int>();

    // ── Meta ──────────────────────────────────────────────────────────────────
    public string saveDate;          // human-readable timestamp for the slot UI
    public int saveVersion = 1;      // bump this when the schema changes

    /// <summary>Stamps the current UTC time onto this save.</summary>
    public void StampDate() => saveDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
}