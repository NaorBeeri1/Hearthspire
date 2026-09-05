using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class ProjectSnapshot : EditorWindow
{
    [MenuItem("Tools/Export Full Project Snapshot to Clipboard")]
    public static void ExportSnapshot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== FULL UNITY PROJECT SNAPSHOT ===");
        sb.AppendLine($"Active Scene: {SceneManager.GetActiveScene().name}");
        sb.AppendLine($"Export Date: {System.DateTime.Now}\n");

        // 1. Scene Hierarchy Dump
        sb.AppendLine("--- SCENE HIERARCHY & COMPONENTS ---");
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            DumpGameObject(root, sb, 0);
        }
        sb.AppendLine();

        // 2. Scripts Dump
        sb.AppendLine("--- C# SCRIPTS SOURCE CODE ---");
        string scriptsPath = Application.dataPath + "/Scripts";
        if (Directory.Exists(scriptsPath))
        {
            string[] scriptFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);
            foreach (string file in scriptFiles)
            {
                string relativePath = file.Replace(Application.dataPath, "Assets");
                sb.AppendLine($"\n// ----------------------------------------");
                sb.AppendLine($"// FILE: {relativePath}");
                sb.AppendLine($"// ----------------------------------------\n");
                sb.AppendLine(File.ReadAllText(file));
            }
        }

        // Copy everything straight to the system clipboard
        EditorGUIUtility.systemCopyBuffer = sb.ToString();

        EditorUtility.DisplayDialog("Copied to Clipboard!", "The full project snapshot is now in your clipboard. Just click into our chat and press Ctrl+V to paste it!", "OK");
    }

    private static void DumpGameObject(GameObject obj, StringBuilder sb, int indent)
    {
        string indentation = new string(' ', indent * 2);
        sb.AppendLine($"{indentation}- {obj.name} (Active: {obj.activeSelf})");
        
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null)
            {
                sb.AppendLine($"{indentation}    [Component: {comp.GetType().Name}]");
            }
        }

        foreach (Transform child in obj.transform)
        {
            DumpGameObject(child.gameObject, sb, indent + 1);
        }
    }
}