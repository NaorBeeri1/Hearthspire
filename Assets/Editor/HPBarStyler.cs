using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HPBarStyler : EditorWindow
{
    [MenuItem("Tools/Create Epic Player HP Bar")]
    public static void CreateHPBar()
    {
        // 1. Find or create a Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("UI Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // Clean up old instance if it exists
        Transform existingBar = canvas.transform.Find("PlayerHPBarContainer");
        if (existingBar != null)
        {
            DestroyImmediate(existingBar.gameObject);
        }

        // 2. Main Container (Top-Center of screen)
        GameObject container = new GameObject("PlayerHPBarContainer", typeof(RectTransform), typeof(CanvasRenderer), typeof(PersistentUI));
        container.transform.SetParent(canvas.transform, false);
        
        RectTransform containerRt = container.GetComponent<RectTransform>();
        containerRt.anchorMin = new Vector2(0.5f, 1f);
        containerRt.anchorMax = new Vector2(0.5f, 1f);
        containerRt.pivot = new Vector2(0.5f, 1f);
        containerRt.anchoredPosition = new Vector2(0f, -30f);
        containerRt.sizeDelta = new Vector2(500f, 110f); // Sized to match your gothic frame proportions

        // 3. Health Bar Fill (The Red Bar underneath the frame)
        // Placed first in hierarchy so the frame renders ON TOP of the red bar
        GameObject fill = new GameObject("PlayerHPBarFill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fill.transform.SetParent(container.transform, false);
        RectTransform fillRt = fill.GetComponent<RectTransform>();
        
        // Adjust these anchors to fit precisely inside the empty slot of your gothic frame graphic
        fillRt.anchorMin = new Vector2(0.18f, 0.32f); 
        fillRt.anchorMax = new Vector2(0.92f, 0.62f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        Image fillImg = fill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 1f;
        fillImg.color = new Color(0.85f, 0.12f, 0.12f, 1f); // Deep blood-red fill color

        // 4. Gothic Frame Background (The decorative border image you showed)
        GameObject frameObj = new GameObject("HPBarFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        frameObj.transform.SetParent(container.transform, false);
        RectTransform frameRt = frameObj.GetComponent<RectTransform>();
        frameRt.anchorMin = Vector2.zero;
        frameRt.anchorMax = Vector2.one;
        frameRt.offsetMin = Vector2.zero;
        frameRt.offsetMax = Vector2.zero;

        Image frameImg = frameObj.GetComponent<Image>();
        frameImg.color = Color.white; // Keep white so your sprite colors show fully
        // Note: Drag your gothic frame sprite into this Image component's "Source Image" slot in Unity!

        // 5. Label Text ("PLAYER VITALITY")
        GameObject labelObj = new GameObject("HPLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObj.transform.SetParent(container.transform, false);
        RectTransform labelRt = labelObj.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0.18f, 0.68f);
        labelRt.anchorMax = new Vector2(0.92f, 0.98f);
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = labelObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "PLAYER VITALITY";
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(1f, 0.84f, 0.31f); // Gold accent
        tmp.alignment = TextAlignmentOptions.Center;

        EditorUtility.SetDirty(canvas.gameObject);
        EditorUtility.DisplayDialog("Epic HP Bar Created!", "Centered, persistent top-center HP bar generated! Drag your gothic frame sprite into the 'HPBarFrame' Image Source slot.", "OK");
    }
}