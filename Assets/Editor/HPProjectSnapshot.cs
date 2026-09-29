using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class HPProjectSnapshot : EditorWindow
{
    [MenuItem("Tools/Export HP System Snapshot to Clipboard")]
    public static void ExportHPSnapshot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== HP SYSTEM & UI SNAPSHOT ===");
        sb.AppendLine($"Export Date: {System.DateTime.Now}\n");

        // 1. Scan Canvas & HP Bar Structure
        sb.AppendLine("--- HP UI HIERARCHY ---");
        GameObject container = GameObject.Find("PlayerHPBarContainer");
        if (container != null)
        {
            DumpHPObject(container.transform, sb, 0);
        }
        else
        {
            sb.AppendLine("PlayerHPBarContainer NOT FOUND in active scene!");
        }
        sb.AppendLine();

        // 2. Scan Player & Health Components
        sb.AppendLine("--- PLAYER & HEALTH COMPONENTS ---");
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            sb.AppendLine($"- Player Object Found (Active: {player.activeSelf})");
            PlayerHealth hpComp = player.GetComponent<PlayerHealth>();
            if (hpComp != null)
            {
                sb.AppendLine($"  [Component: PlayerHealth]");
                sb.AppendLine($"    - Max Health: {hpComp.maxHealth}");
                sb.AppendLine($"    - Current Health: {hpComp.currentHealth}");
                sb.AppendLine($"    - Health Bar Fill Assigned: {(hpComp.healthBarFill != null ? hpComp.healthBarFill.name : "NULL")}");
            }
            else
            {
                sb.AppendLine("  [Missing Component: PlayerHealth script not attached to Player!]");
            }
        }
        else
        {
            sb.AppendLine("Player object NOT FOUND in active scene!");
        }

        // Copy everything straight to the system clipboard
        EditorGUIUtility.systemCopyBuffer = sb.ToString();

        EditorUtility.DisplayDialog("HP Snapshot Copied!", "The HP system details are now in your clipboard. Just click into our chat and press Ctrl+V to paste them!", "OK");
    }

    private static void DumpHPObject(Transform objTransform, StringBuilder sb, int indent)
    {
        string indentation = new string(' ', indent * 2);
        GameObject obj = objTransform.gameObject;
        sb.AppendLine($"{indentation}- {obj.name} (Active: {obj.activeSelf})");

        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null)
            {
                sb.AppendLine($"{indentation}    [Component: {comp.GetType().Name}]");
                
                if (comp is Image img)
                {
                    sb.AppendLine($"{indentation}        - Sprite: {(img.sprite != null ? img.sprite.name : "None")}");
                    sb.AppendLine($"{indentation}        - Color: {img.color}");
                    sb.AppendLine($"{indentation}        - Fill Method: {img.fillMethod}, Fill Amount: {img.fillAmount}");
                }
                else if (comp is TextMeshProUGUI tmp)
                {
                    sb.AppendLine($"{indentation}        - Text: \"{tmp.text}\"");
                }
            }
        }

        foreach (Transform child in objTransform)
        {
            DumpHPObject(child, sb, indent + 1);
        }
    }
}