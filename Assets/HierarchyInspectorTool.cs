using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;

public class HierarchyInspectorTool : EditorWindow
{
    [MenuItem("Tools/Inspect Scene Hierarchy & Inspector Properties")]
    public static void InspectScene()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== DETAILED SCENE & INSPECTOR SNAPSHOT ===");
        sb.AppendLine($"Scene: {SceneManager.GetActiveScene().name}");
        sb.AppendLine($"Time: {System.DateTime.Now}\n");

        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            InspectGameObject(root, sb, 0);
        }

        string result = sb.ToString();
        EditorGUIUtility.systemCopyBuffer = result;
        Debug.Log(result);

        EditorUtility.DisplayDialog("Inspection Complete", "The full hierarchy and inspector property breakdown has been printed to the Console and copied to your clipboard!", "OK");
    }

    private static void InspectGameObject(GameObject obj, StringBuilder sb, int indent)
    {
        string indentation = new string('-', indent * 2);
        sb.AppendLine($"{indentation} GameObject: {obj.name} (Active: {obj.activeSelf}) [Tag: {obj.tag}] [Layer: {obj.layer}]");

        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp == null) continue;

            sb.AppendLine($"{indentation}   -> Component: {comp.GetType().Name}");

            SerializedObject serializedObj = new SerializedObject(comp);
            SerializedProperty prop = serializedObj.GetIterator();

            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                if (prop.name == "m_Script") 
                {
                    enterChildren = false;
                    continue;
                }

                string propValue = GetPropertyValue(prop);
                sb.AppendLine($"{indentation}      [Field] {prop.name}: {propValue}");
                enterChildren = false; 
            }
        }

        foreach (Transform child in obj.transform)
        {
            InspectGameObject(child.gameObject, sb, indent + 1);
        }
    }

    private static string GetPropertyValue(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.Float: return prop.floatValue.ToString();
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Color: return prop.colorValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue != null ? prop.objectReferenceValue.name : "None";
            case SerializedPropertyType.Vector2: return prop.vector2Value.ToString();
            case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
            case SerializedPropertyType.Enum:
                if (prop.enumNames != null && prop.enumValueIndex >= 0 && prop.enumValueIndex < prop.enumNames.Length)
                {
                    return prop.enumNames[prop.enumValueIndex];
                }
                return prop.enumValueIndex.ToString();
            default: return $"({prop.propertyType})";
        }
    }
}