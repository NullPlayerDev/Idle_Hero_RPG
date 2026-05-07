// Place this file in any folder named "Editor" inside your Assets folder.
// Example: Assets/Editor/SaveSystemEditorTools.cs
// It will NOT be included in your final build — Editor scripts are dev-only.

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveSystemEditorTools
{
    private const string FILE_NAME   = "gamesave.json";
    private const string CONFIG_FILE = "save_config.txt";

    [MenuItem("Tools/Save System/Reset Save (Delete gamesave.json)")]
    private static void ResetSave()
    {
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Save deleted → {path}");
            EditorUtility.DisplayDialog("Save Reset", "gamesave.json deleted.\nGame will start fresh on next Play.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("No Save Found", "No gamesave.json found — game is already fresh.", "OK");
        }
    }

    [MenuItem("Tools/Save System/Open Save Folder")]
    private static void OpenSaveFolder()
    {
        // Creates the folder if it doesn't exist, then opens it in Explorer/Finder
        Directory.CreateDirectory(Application.persistentDataPath);
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/Save System/Print Save Path")]
    private static void PrintSavePath()
    {
        Debug.Log($"[SaveSystem] Save folder → {Application.persistentDataPath}");
    }
}
#endif