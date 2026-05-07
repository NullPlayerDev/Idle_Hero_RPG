using System.IO;
using UnityEngine;

/// <summary>
/// Reads save_config.txt and executes any commands inside.
/// Called as a static method from GameManager.Start() — NOT a MonoBehaviour Awake().
/// This ensures all singletons are ready before any config commands run.
///
/// HOW TO USE:
///   delete    = true      Wipes save and resets to defaults
///   set_level = 5         Overrides the player's current level
///   set_gold  = 1000      Overrides gold
///   set_gems  = 50        Overrides gems
///
/// After commands run the file resets automatically so they don't fire again.
/// </summary>
public static class SaveConfigLoader
{
    private const string CONFIG_FILE = "save_config.txt";

    private static string ConfigPath =>
        Path.Combine(Application.persistentDataPath, CONFIG_FILE);

    // Called by GameManager.Start() after SaveSystem.Load()
    public static void ProcessConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            WriteDefaultConfig();
            return;
        }

        string[] lines = File.ReadAllLines(ConfigPath);
        bool     dirty = false;

        foreach (string raw in lines)
        {
            string line = raw.Split('#')[0].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            int eq = line.IndexOf('=');
            if (eq < 0) continue;

            string key   = line.Substring(0, eq).Trim().ToLower();
            string value = line.Substring(eq + 1).Trim().ToLower();

            switch (key)
            {
                case "delete":
                    if (value == "true")
                    {
                        SaveSystem.DeleteAll();
                        Debug.Log("[SaveConfig] delete = true → save wiped.");
                        dirty = true;
                    }
                    break;

                case "set_level":
                    if (int.TryParse(value, out int lvl) && GameManager.Instance != null)
                    {
                        GameManager.Instance.CurrentLevel = lvl;
                        SaveSystem.Save();
                        Debug.Log($"[SaveConfig] set_level = {lvl}");
                        dirty = true;
                    }
                    break;

                case "set_gold":
                    if (int.TryParse(value, out int gold) && RewardWallet.Instance != null)
                    {
                        RewardWallet.Instance.SetGold(gold);
                        SaveSystem.Save();
                        Debug.Log($"[SaveConfig] set_gold = {gold}");
                        dirty = true;
                    }
                    break;

                case "set_gems":
                    if (int.TryParse(value, out int gems) && RewardWallet.Instance != null)
                    {
                        RewardWallet.Instance.SetGems(gems);
                        SaveSystem.Save();
                        Debug.Log($"[SaveConfig] set_gems = {gems}");
                        dirty = true;
                    }
                    break;

                default:
                    Debug.LogWarning($"[SaveConfig] Unknown key '{key}' — ignored.");
                    break;
            }
        }

        if (dirty) WriteDefaultConfig();
    }

    private static void WriteDefaultConfig()
    {
        string content =
@"# save_config.txt  —  Game Save Controller
# Edit this file to control the save system.
# Commands run ONCE on next game launch, then this file resets automatically.
#
# ── Commands ──────────────────────────────────────────────────────────
#   delete    = true      Wipe save + reset to defaults
#   set_level = 5         Set current level
#   set_gold  = 1000      Set gold amount
#   set_gems  = 50        Set gems amount
#
# ── How to use ────────────────────────────────────────────────────────
#   1. Remove the '#' from a line to activate it
#   2. Set the value you want
#   3. Relaunch the game — command fires once, then resets
#
# ── File location ─────────────────────────────────────────────────────
#   Windows : %APPDATA%\..\LocalLow\<Company>\<Product>\
#   Android : /data/data/<bundle_id>/files/
#   iOS     : <AppHome>/Documents/
# ──────────────────────────────────────────────────────────────────────

# delete    = true
# set_level = 1
# set_gold  = 0
# set_gems  = 0
";
        File.WriteAllText(ConfigPath, content);
        Debug.Log($"[SaveConfig] Config written → {ConfigPath}");
    }
}