# Save System — Setup Guide

## Files included
| File | What it does |
|---|---|
| `SaveData.cs` | Plain data class — gold, gems, level, heroes |
| `SaveSystem.cs` | Reads/writes the JSON file. One call: `SaveSystem.Save()` / `SaveSystem.Load()` |
| `SaveConfigLoader.cs` | Reads `save_config.txt` at startup and applies any commands |
| `RewardWallet.cs` | Fixed wallet — no more PlayerPrefs, fires events correctly |
| `GameManager.cs` | Clean GameManager with save/delete buttons wired in |
| `save_config.txt` | The control file — edit this to delete or override saves |

---

## Unity Setup (one-time)

### 1. Replace scripts
Drop all `.cs` files into your `Assets/Scripts/` folders, replacing the old ones.

### 2. Create a "SaveManager" GameObject
- In your first scene (PlayerSelection), create an empty GameObject named **SaveManager**
- Add the **SaveConfigLoader** component to it

> `SaveConfigLoader.Awake()` calls `SaveSystem.Load()` automatically.  
> Do **not** call `SaveSystem.Load()` anywhere else.

### 3. Wire the buttons in GameManager
In the Inspector on your **GameManager** GameObject:
- **Save Button** → `GameManager.OnSaveButton()`
- **Delete Save Button** → `GameManager.OnDeleteSaveButton()`

### 4. Add `GetSelectedHeroIDs` and `RestoreSelectedHeroes` to HeroSelectionManager
SaveSystem calls these two methods. Add them to your `HeroSelectionManger.cs`:

```csharp
// Returns the IDs of all currently selected heroes
public List<int> GetSelectedHeroIDs()
{
    var ids = new List<int>();
    foreach (var hero in _selectedHeroes)   // use whatever your list is called
        ids.Add(hero.ID);
    return ids;
}

// Restores hero selection from saved IDs on load
public void RestoreSelectedHeroes(List<int> ids)
{
    // Match IDs to your HeroData ScriptableObjects and re-select them
    foreach (int id in ids)
    {
        // Example: find and select the hero with this ID
        // _selectedHeroes.Add(heroDataWithID(id));
    }
}
```

---

## How saves work

```
Game starts
    └─ SaveConfigLoader.Awake()
           ├─ SaveSystem.Load()          ← restores level, gold, gems, heroes
           └─ ProcessConfig()            ← checks save_config.txt for commands

Level won
    └─ GameManager.HandleStageEnded()
           └─ SaveSystem.Save()          ← auto-saves everything

Player presses "Save" button
    └─ GameManager.OnSaveButton()
           └─ SaveSystem.Save()

Player presses "Delete Save" button
    └─ GameManager.OnDeleteSaveButton()
           └─ SaveSystem.DeleteAll()     ← deletes file + resets to defaults
```

---

## Controlling saves remotely via save_config.txt

The file lives in Unity's **persistent data folder**:

| Platform | Path |
|---|---|
| Windows | `%APPDATA%\..\LocalLow\<Company>\<Product>\save_config.txt` |
| Android | `/data/data/<bundle_id>/files/save_config.txt` |
| iOS | `<AppHome>/Documents/save_config.txt` |

**To delete all progress:** edit the file and uncomment:
```
delete = true
```
Relaunch the game → save is wiped → file resets automatically.

**To set a specific level:**
```
set_level = 5
```

**To give gold/gems:**
```
set_gold = 9999
set_gems = 100
```

Multiple commands work at once:
```
delete    = true
set_level = 3
set_gold  = 500
```

---

## Problems this fixes vs the old code

| Old Problem | Fix |
|---|---|
| `PlayerSaveData` struct with `List<GameObject>` — GameObjects can't be serialized | Removed entirely. Save only stores primitive IDs |
| `RewardWallet` used `PlayerPrefs` while `SaveSystem` used JSON — two separate systems | `RewardWallet` now uses `SaveSystem` only |
| `SaveSystem.SaveLevel` / `LoadLevel` used a different `PlayerSaveData` class than `SaveData` | One class (`SaveData`), one system |
| Gems event fired `OnGoldChanged` instead of `OnGemsChanged` | Fixed in new `RewardWallet` |
| `AddGems` called `SaveGold()` so gems were never saved | Fixed |
| No control file — had to touch code to delete saves | `save_config.txt` handles it |