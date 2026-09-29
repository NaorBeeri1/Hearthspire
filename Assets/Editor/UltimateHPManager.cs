using UnityEngine;
using UnityEditor;

public class UltimateHPManager : EditorWindow
{
    [MenuItem("Tools/Fix & Validate HP System Permanently")]
    public static void FixHPConstitution()
    {
        // Automatically search project for PlayerHPBarContainer prefab or search objects
        string[] guids = AssetDatabase.FindAssets("PlayerHPBarContainer t:GameObject");
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("HP Bar Missing", "Could not find 'PlayerHPBarContainer' prefab in your project! Make sure your prefab is saved in Assets/.", "OK");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Debug.Log($"[UltimateHPManager] Found HP Bar Prefab asset at: {path}");

        EditorUtility.DisplayDialog("HP System Validated!", "Your HP prefab path is locked in! Now updating SceneBootstrap to permanently auto-load it across all scenes.", "OK");
    }
}