// ----------------------------------------
// FILE: Assets/Scripts/UIFixerTool.cs
// ----------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIFixerTool : EditorWindow
{
    [MenuItem("Tools/Fix All UI Panels & Buttons (1-Click)")]
    public static void FixUI()
    {
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        int fixedCount = 0;

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace) continue;

            // 1. Force Canvas Settings for proper overlay rendering
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            // 2. Fix all panels (PauseMenuPanel, GameOverPanel, etc.)
            foreach (Transform child in canvas.transform)
            {
                if (child.name.Contains("Panel") || child.name.Contains("UI"))
                {
                    RectTransform rt = child.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        // Perfect full-screen stretch anchors and zeroed offsets
                        rt.anchorMin = Vector2.zero;
                        rt.anchorMax = Vector2.one;
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = Vector2.zero;
                    }

                    // Configure background image for greyed-out effect
                    Image img = child.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = new Color(0f, 0f, 0f, 0.75f); // 75% dark overlay
                        img.raycastTarget = true;
                    }

                    // 3. Force all child buttons and text to be active and clickable
                    foreach (Transform subChild in child)
                    {
                        subChild.gameObject.SetActive(true);

                        Image subImg = subChild.GetComponent<Image>();
                        if (subImg != null)
                        {
                            subImg.raycastTarget = true;
                        }
                    }

                    fixedCount++;
                }
            }
        }

        // 4. Automatically link panels to GameMenuController if found in scene
        GameMenuController controller = Object.FindFirstObjectByType<GameMenuController>();
        if (controller != null)
        {
            if (controller.pauseMenuUI == null)
            {
                GameObject pPanel = GameObject.Find("PauseMenuPanel");
                if (pPanel != null) controller.pauseMenuUI = pPanel;
            }
            if (controller.gameOverUI == null)
            {
                GameObject gPanel = GameObject.Find("GameOverPanel");
                if (gPanel != null) controller.gameOverUI = gPanel;
            }
            EditorUtility.SetDirty(controller);
        }

        EditorUtility.DisplayDialog("UI Fully Repaired!", $"Successfully fixed canvases, stretched panels, set backgrounds, and activated all buttons across {canvases.Length} canvases ({fixedCount} panels fixed)!", "OK");
    }
}