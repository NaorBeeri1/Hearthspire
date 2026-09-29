// ----------------------------------------
// FILE: Assets/Editor/PauseMenuBuilder.cs
// ----------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PauseMenuBuilder : EditorWindow
{
    [MenuItem("Tools/Rebuild Professional Pause Menu (1-Click)")]
    public static void RebuildPauseMenu()
    {
        // 1. Clean up old pause panels completely
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == "PauseMenuPanel" && obj.hideFlags == HideFlags.None && AssetDatabase.GetAssetPath(obj) == "")
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }

        // 2. Find Persistent UI Canvas and ensure GraphicRaycaster exists
        Canvas targetCanvas = null;
        foreach (Canvas c in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (c.gameObject.name == "PersistentUICanvas" && c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                targetCanvas = c;
                break;
            }
        }

        if (targetCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'PersistentUICanvas' in the scene!", "OK");
            return;
        }

        if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        // Ensure a valid EventSystem exists for UI clicks
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
        }

        // Find GameMenuController reference
        GameMenuController menuController = Object.FindFirstObjectByType<GameMenuController>();
        if (menuController == null)
        {
            GameObject managerObj = new GameObject("GameMenuController", typeof(GameMenuController));
            menuController = managerObj.GetComponent<GameMenuController>();
        }

        // 3. Create Root Full-Screen Panel
        GameObject panelObj = new GameObject("PauseMenuPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panelObj, "Create Professional Pause Menu");
        panelObj.transform.SetParent(targetCanvas.transform, false);

        RectTransform rt = panelObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Translucent greyed-out full screen overlay
        Image panelImg = panelObj.GetComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.75f);
        panelImg.raycastTarget = false; // CRITICAL: Must be false so background doesn't block button clicks!

        // 4. Create Header Text ("GAME PAUSED" at Top)
        CreateTextMeshPro(panelObj.transform, "GamePaused", "GAME PAUSED", 96, new Vector2(0.5f, 0.78f), new Vector2(1200f, 180f), new Color(1f, 0.92f, 0.7f));

        // 5. Create Resume Button (Middle)
        GameObject resumeBtn = CreateButton(panelObj.transform, "ResumeButton", "Resume", 56, new Vector2(0.5f, 0.50f), new Vector2(850f, 130f));
        Button rBtn = resumeBtn.GetComponent<Button>();
        rBtn.onClick.AddListener(() => menuController.ResumeGame());

        // 6. Create Main Menu Button (Bottom)
        GameObject menuBtn = CreateButton(panelObj.transform, "MainMenuButton", "Main Menu", 56, new Vector2(0.5f, 0.32f), new Vector2(850f, 130f));
        Button mBtn = menuBtn.GetComponent<Button>();
        mBtn.onClick.AddListener(() => menuController.LoadMainMenu());

        // Link to Controller
        menuController.pauseMenuUI = panelObj;
        EditorUtility.SetDirty(menuController);

        // Keep disabled by default on startup
        panelObj.SetActive(false);

        EditorUtility.DisplayDialog("Professional Cleanup Complete!", "Pause menu rebuilt successfully. Button click raycasts are now fully unblocked.", "OK");
    }

    private static void CreateTextMeshPro(Transform parent, string name, string text, int fontSize, Vector2 anchorPos, Vector2 size, Color textColor)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(parent, false);

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = anchorPos;
        rt.anchorMax = anchorPos;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;
        
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;

        tmp.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
        tmp.outlineColor = new Color32(15, 8, 3, 255);
        tmp.outlineWidth = 0.3f;
    }

    private static GameObject CreateButton(Transform parent, string name, string labelText, int fontSize, Vector2 anchorPos, Vector2 size)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = anchorPos;
        rt.anchorMax = anchorPos;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(0.18f, 0.18f, 0.26f, 0.95f);
        img.raycastTarget = true; // Enables clicking on the button body

        CreateTextMeshPro(btnObj.transform, "Text (TMP)", labelText, fontSize, new Vector2(0.5f, 0.5f), size, Color.white);

        return btnObj;
    }
}