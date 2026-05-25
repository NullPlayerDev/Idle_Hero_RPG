using System;
using System.Collections.Generic;

/// <summary>
/// Plain serializable snapshot of everything that needs to survive between sessions.
/// No MonoBehaviour — just data. JsonUtility can serialize this directly.
/// 
/// v2 adds gear inventory and equipped gear IDs.
/// </summary>
[Serializable]
public class SaveData
{
    // ── Wallet ────────────────────────────────────────────────────────────────
    public int gold;
    public int gems;

    // ── Progression ───────────────────────────────────────────────────────────
    public int  currentLevel   = 1;
    public int  totalStagesWon = 0;

    // ── Hero selection (stores HeroData.ID of each selected hero) ────────────
    public List<int> selectedHeroIDs = new List<int>();

    // ── Gear ──────────────────────────────────────────────────────────────────
    // IDs of every GearItem sitting in the player's bag (not equipped)
    public List<int> inventoryGearIDs = new List<int>();

    // IDs of equipped gear, indexed by GearSlot enum order:
    //   index 0 = Weapon, 1 = Armor, 2 = Boots
    // -1 means the slot is empty.
    public List<int> equippedGearIDs = new List<int>() { -1, -1, -1 };

    // ── Meta ──────────────────────────────────────────────────────────────────
    public string saveDate;       // human-readable timestamp shown in UI
    public int    saveVersion = 2;

    /// <summary>Stamps the current UTC time onto this save.</summary>
    public void StampDate() => saveDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
}