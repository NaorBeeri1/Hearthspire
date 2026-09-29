using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ShopLayoutFixer : EditorWindow
{
    [MenuItem("Tools/Fix Shop UI Layout")]
    public static void FixLayout()
    {
        // Find the ShopPanel in the active scene
        GameObject shopPanelObj = GameObject.Find("ShopPanel");
        if (shopPanelObj == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'ShopPanel' in the active scene!", "OK");
            return;
        }

        // 1. Add or configure Vertical Layout Group on ShopPanel
        VerticalLayoutGroup layoutGroup = shopPanelObj.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = shopPanelObj.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.spacing = 20f; // Space between elements

        // 2. Add Content Size Fitter if missing
        ContentSizeFitter sizeFitter = shopPanelObj.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = shopPanelObj.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 3. Ensure children have correct sizing and anchor settings
        foreach (Transform child in shopPanelObj.transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Set reasonable dimensions for UI elements
                rt.sizeDelta = new Vector2(400f, 70f);
            }
        }

        EditorUtility.SetDirty(shopPanelObj);
        EditorUtility.DisplayDialog("Success!", "Shop UI layout has been automatically fixed and spaced out!", "OK");
    }
}