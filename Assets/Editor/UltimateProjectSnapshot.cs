// ----------------------------------------
// FILE: Assets/Editor/UltimateProjectSnapshot.cs
// ----------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class UltimateProjectSnapshot : EditorWindow
{
    [MenuItem("Tools/Export ULTIMATE Project Snapshot (Full Details)")]
    public static void ExportUltimateSnapshot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("=== ULTIMATE UNITY PROJECT ATOMIC SNAPSHOT ===");
        sb.AppendLine("==================================================");
        sb.AppendLine($"Export Timestamp: {System.DateTime.Now}");
        sb.AppendLine($"Active Scene: {SceneManager.GetActiveScene().name}\n");

        // 1. Project Directory Structure & File Inventory
        sb.AppendLine("--- 1. FULL PROJECT FILE INVENTORY ---");
        string assetsPath = Application.dataPath;
        DumpDirectoryRecursive(new DirectoryInfo(assetsPath), sb, 0);
        sb.AppendLine("\n");

        // 2. Comprehensive Scene Hierarchy & Detailed Component Inspector Dump
        sb.AppendLine("--- 2. ACTIVE SCENE HIERARCHY & COMPONENT INSPECTOR VALUES ---");
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            DumpGameObjectDeep(root, sb, 0);
        }
        sb.AppendLine("\n");

        // 3. Raw YAML/Asset Contents (Prefabs, Scenes, ScriptableObjects)
        sb.AppendLine("--- 3. RAW ASSET SERIALIZATION DATA (.unity & .prefab) ---");
        string[] assetFiles = Directory.GetFiles(assetsPath, "*.*", SearchOption.AllDirectories);
        foreach (string file in assetFiles)
        {
            if (file.EndsWith(".unity") || file.EndsWith(".prefab") || file.EndsWith(".asset"))
            {
                string relPath = file.Replace(Application.dataPath, "Assets");
                sb.AppendLine($"\n// [ASSET FILE]: {relPath}");
                sb.AppendLine(File.ReadAllText(file));
            }
        }
        sb.AppendLine("\n");

        // 4. Complete C# Source Code Dump (All scripts in project)
        sb.AppendLine("--- 4. COMPLETE C# SCRIPTS SOURCE CODE ---");
        string[] scriptFiles = Directory.GetFiles(assetsPath, "*.cs", SearchOption.AllDirectories);
        foreach (string file in scriptFiles)
        {
            string relPath = file.Replace(Application.dataPath, "Assets");
            sb.AppendLine($"\n// ----------------------------------------");
            sb.AppendLine($"// FILE: {relPath}");
            sb.AppendLine($"// ----------------------------------------\n");
            sb.AppendLine(File.ReadAllText(file));
        }

        // Copy everything straight to the system clipboard
        EditorGUIUtility.systemCopyBuffer = sb.ToString();

        EditorUtility.DisplayDialog(
            "Ultimate Snapshot Exported!", 
            "The complete atomic project snapshot (Hierarchy, Inspector components, raw asset files, and all source scripts) has been copied to your clipboard. Press Ctrl+V to paste it!", 
            "OK"
        );
    }

    private static void DumpDirectoryRecursive(DirectoryInfo dir, StringBuilder sb, int indent)
    {
        string indentStr = new string(' ', indent * 2);
        sb.AppendLine($"{indentStr}[DIR] {dir.Name}");
        
        foreach (FileInfo file in dir.GetFiles())
        {
            if (!file.Name.EndsWith(".meta"))
            {
                sb.AppendLine($"{indentStr}  - {file.Name} ({file.Length} bytes)");
            }
        }

        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            DumpDirectoryRecursive(subDir, sb, indent + 1);
        }
    }

    private static void DumpGameObjectDeep(GameObject obj, StringBuilder sb, int indent)
    {
        string indentation = new string(' ', indent * 2);
        sb.AppendLine($"{indentation}- GameObject: {obj.name} (ActiveSelf: {obj.activeSelf}, Tag: {obj.tag}, Layer: {LayerMask.LayerToName(obj.layer)})");
        
        // Transform Inspector details
        Transform t = obj.transform;
        sb.AppendLine($"{indentation}    [Transform] Position: {t.position} | Rotation: {t.eulerAngles} | Scale: {t.localScale}");

        // All Components
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null && !(comp is Transform))
            {
                sb.AppendLine($"{indentation}    [Component: {comp.GetType().Name}]");
                
                // Use SerializedObject to extract every single public/private serialized field and its current Inspector value
                SerializedObject serializedObj = new SerializedObject(comp);
                SerializedProperty prop = serializedObj.GetIterator();
                bool enterChildren = true;
                while (prop.NextVisible(enterChildren))
                {
                    // Skip script reference property itself to reduce noise
                    if (prop.name == "m_Script") { enterChildren = false; continue; }

                    string val = GetSerializedPropertyValue(prop);
                    sb.AppendLine($"{indentation}        {prop.name} ({prop.type}) = {val}");
                    enterChildren = false; // only top level properties of the component
                }
            }
        }

        foreach (Transform child in obj.transform)
        {
            DumpGameObjectDeep(child.gameObject, sb, indent + 1);
        }
    }

    private static string GetSerializedPropertyValue(SerializedProperty prop)
    {
        try
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer: return prop.intValue.ToString();
                case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
                case SerializedPropertyType.Float: return prop.floatValue.ToString();
                case SerializedPropertyType.String: return prop.stringValue;
                case SerializedPropertyType.Color: return prop.colorValue.ToString();
                case SerializedPropertyType.ObjectReference: 
                    return prop.objectReferenceValue != null ? $"{prop.objectReferenceValue.name} ({prop.objectReferenceValue.GetType().Name})" : "null";
                case SerializedPropertyType.Vector2: return prop.vector2Value.ToString();
                case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
                case SerializedPropertyType.Enum: 
                    if (prop.enumDisplayNames != null && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumDisplayNames.Length)
                    {
                        return prop.enumDisplayNames[prop.enumValueIndex];
                    }
                    return prop.intValue.ToString();
                default: return $"[{prop.propertyType}]";
            }
        }
        catch
        {
            return "[Error reading property]";
        }
    }
}