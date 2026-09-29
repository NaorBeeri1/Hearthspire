using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PauseMenuSetupTool : EditorWindow
{
    [MenuItem("Tools/Fix Pause Menu Layout Automatically")]
    public static void FixPauseMenu()
    {
        // Find PauseMenuPanel even if it's inactive in the scene
        GameObject pausePanel = null;
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == "PauseMenuPanel" && obj.hideFlags == HideFlags.None && AssetDatabase.GetAssetPath(obj) == "")
            {
                pausePanel = obj;
                break;
            }
        }

        if (pausePanel == null)
        {
            EditorUtility.DisplayDialog("Not Found", "Could not find 'PauseMenuPanel' anywhere in the active scene!", "OK");
            return;
        }

        Undo.RecordObject(pausePanel.transform, "Fix Pause Menu Layout");

        // 1. Fix RectTransform anchoring to center
        RectTransform rt = pausePanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(500f, 400f);
        }

        // 2. Setup Background Image
        Image img = pausePanel.GetComponent<Image>();
        if (img == null) img = pausePanel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);
        img.raycastTarget = true;

        // 3. Setup Vertical Layout Group
        VerticalLayoutGroup layout = pausePanel.GetComponent<VerticalLayoutGroup>();
        if (layout == null) layout = pausePanel.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 20f;
        layout.padding = new RectOffset(40, 40, 40, 40);

        // 4. Setup Content Size Fitter
        ContentSizeFitter fitter = pausePanel.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = pausePanel.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 5. Ensure children have correct dimensions and active states
        foreach (Transform child in pausePanel.transform)
        {
            Undo.RecordObject(child.gameObject, "Fix Child Element");
            child.gameObject.SetActive(true);

            RectTransform childRt = child.GetComponent<RectTransform>();
            if (childRt != null)
            {
                childRt.sizeDelta = new Vector2(350f, 60f);
            }
        }

        EditorUtility.SetDirty(pausePanel);
        EditorUtility.DisplayDialog("Success!", "PauseMenuPanel layout has been fully automated and fixed!", "OK");
    }
}