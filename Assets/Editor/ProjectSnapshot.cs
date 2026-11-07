using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System.Reflection;

public class ProjectSnapshot : EditorWindow
{
    [MenuItem("Tools/Export Full Project Snapshot to Clipboard")]
    public static void ExportSnapshot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== FULL UNITY PROJECT DETAILED SNAPSHOT ===");
        sb.AppendLine($"Unity Version : {Application.unityVersion}");
        sb.AppendLine($"Active Scene  : {SceneManager.GetActiveScene().name} (Path: {SceneManager.GetActiveScene().path})");
        sb.AppendLine($"Export Date   : {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

        // 1. Build Settings Scenes
        sb.AppendLine("--- BUILD SETTINGS SCENES ---");
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            sb.AppendLine($"[{i}] Enabled: {scene.enabled,-5} | Path: {scene.path}");
        }
        sb.AppendLine();

        // 2. Scene Hierarchy & Component Details
        sb.AppendLine("--- DETAILED SCENE HIERARCHY ---");
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            DumpGameObject(root, sb, 0);
        }
        sb.AppendLine();

        // 3. C# Source Code Dump
        sb.AppendLine("--- C# SCRIPTS SOURCE CODE ---");
        string scriptsPath = Application.dataPath + "/Scripts";
        if (Directory.Exists(scriptsPath))
        {
            string[] scriptFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);
            foreach (string file in scriptFiles)
            {
                // Skip editor snapshot tool itself to keep output lean
                if (file.EndsWith("ProjectSnapshot.cs")) continue;

                string relativePath = file.Replace(Application.dataPath, "Assets");
                sb.AppendLine($"\n// ----------------------------------------");
                sb.AppendLine($"// FILE: {relativePath}");
                sb.AppendLine($"// ----------------------------------------\n");
                sb.AppendLine(File.ReadAllText(file));
            }
        }

        EditorGUIUtility.systemCopyBuffer = sb.ToString();
        EditorUtility.DisplayDialog("Snapshot Ready!", "Detailed project snapshot has been copied to your clipboard.", "OK");
    }

    private static void DumpGameObject(GameObject obj, StringBuilder sb, int indent)
    {
        string pad = new string(' ', indent * 2);
        string activeState = obj.activeSelf ? "Active" : "Inactive";
        sb.AppendLine($"{pad}- {obj.name} [{activeState}] (Tag: {obj.tag}, Layer: {LayerMask.LayerToName(obj.layer)})");

        // Transform details
        Vector3 pos = obj.transform.localPosition;
        Vector3 rot = obj.transform.localEulerAngles;
        Vector3 scl = obj.transform.localScale;
        sb.AppendLine($"{pad}    Transform: Pos=({pos.x:F2}, {pos.y:F2}, {pos.z:F2}) Rot=({rot.x:F1}, {rot.y:F1}, {rot.z:F1}) Scale=({scl.x:F2}, {scl.y:F2}, {scl.z:F2})");

        // RectTransform details (if UI)
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            sb.AppendLine($"{pad}    RectTransform: AnchorsMin=({rt.anchorMin.x:F2}, {rt.anchorMin.y:F2}) AnchorsMax=({rt.anchorMax.x:F2}, {rt.anchorMax.y:F2}) AnchoredPos=({rt.anchoredPosition.x:F1}, {rt.anchoredPosition.y:F1}) Size=({rt.sizeDelta.x:F1}, {rt.sizeDelta.y:F1})");
        }

        // Component breakdown & inspector fields
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp == null)
            {
                sb.AppendLine($"{pad}    [Component: MISSING / BROKEN SCRIPT]");
                continue;
            }

            if (comp is Transform || comp is RectTransform) continue;

            sb.AppendLine($"{pad}    [Component: {comp.GetType().Name}]");
            DumpSerializedFields(comp, sb, pad + "      ");
        }

        foreach (Transform child in obj.transform)
        {
            DumpGameObject(child.gameObject, sb, indent + 1);
        }
    }

    private static void DumpSerializedFields(Component comp, StringBuilder sb, string pad)
    {
        SerializedObject so = new SerializedObject(comp);
        SerializedProperty prop = so.GetIterator();

        bool enterChildren = true;
        int displayedFields = 0;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false; // Stay top-level for clean output
            if (prop.name == "m_Script") continue;

            string valueStr = GetPropertyValueString(prop);
            if (!string.IsNullOrEmpty(valueStr))
            {
                sb.AppendLine($"{pad}{prop.name}: {valueStr}");
                displayedFields++;
                if (displayedFields >= 12) // Cap per component to prevent prompt overflow
                {
                    sb.AppendLine($"{pad}... (more fields truncated)");
                    break;
                }
            }
        }
    }

    private static string GetPropertyValueString(SerializedProperty prop)
    {
        return prop.propertyType switch
        {
            SerializedPropertyType.Integer => prop.intValue.ToString(),
            SerializedPropertyType.Boolean => prop.boolValue.ToString(),
            SerializedPropertyType.Float => prop.floatValue.ToString("F2"),
            SerializedPropertyType.String => $"\"{prop.stringValue}\"",
            SerializedPropertyType.Color => prop.colorValue.ToString(),
            SerializedPropertyType.ObjectReference => prop.objectReferenceValue != null ? $"[{prop.objectReferenceValue.name}]" : "null (Unassigned)",
            SerializedPropertyType.Enum => prop.enumDisplayNames.Length > prop.enumValueIndex && prop.enumValueIndex >= 0 ? prop.enumDisplayNames[prop.enumValueIndex] : prop.enumValueIndex.ToString(),
            SerializedPropertyType.Vector2 => $"({prop.vector2Value.x:F1}, {prop.vector2Value.y:F1})",
            SerializedPropertyType.Vector3 => $"({prop.vector3Value.x:F1}, {prop.vector3Value.y:F1}, {prop.vector3Value.z:F1})",
            _ => null
        };
    }
}