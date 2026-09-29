using UnityEngine;
using UnityEngine.UI;

public class GlobalGameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeGame()
    {
        if (GameManager.Instance == null)
        {
            GameObject managersObj = new GameObject("Managers_Global");
            managersObj.AddComponent<GameManager>();
            Object.DontDestroyOnLoad(managersObj);
        }

        Canvas activeCanvas = null;
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in canvases)
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay && c.gameObject.name.Contains("Persistent"))
            {
                activeCanvas = c;
                break;
            }
        }

        if (activeCanvas == null)
        {
            GameObject canvasObj = new GameObject("PersistentUICanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Object.DontDestroyOnLoad(canvasObj); 

            activeCanvas = canvasObj.GetComponent<Canvas>();
            activeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            activeCanvas.sortingOrder = 999; 
            
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        GameObject existingHPBar = GameObject.Find("PlayerHPBarContainer");
        bool isMainMenu = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenuScene";

        if (existingHPBar != null)
        {
            existingHPBar.SetActive(!isMainMenu);
        }
        else if (!isMainMenu)
        {
            GameObject hpBar = new GameObject("PlayerHPBarContainer", typeof(RectTransform), typeof(CanvasRenderer), typeof(PlayerBarUpdater));
            hpBar.transform.SetParent(activeCanvas.transform, false);

            Sprite frameSprite = null;
            Sprite fillSprite = null;

#if UNITY_EDITOR
            string[] frameGuids = UnityEditor.AssetDatabase.FindAssets("HpBarFrame t:Sprite");
            if (frameGuids.Length > 0)
            {
                frameSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(UnityEditor.AssetDatabase.GUIDToAssetPath(frameGuids[0]));
            }

            string[] fillGuids = UnityEditor.AssetDatabase.FindAssets("HpBarFill t:Sprite");
            if (fillGuids.Length > 0)
            {
                fillSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(UnityEditor.AssetDatabase.GUIDToAssetPath(fillGuids[0]));
            }
#endif

            GameObject frameObj = new GameObject("HPBarFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            frameObj.transform.SetParent(hpBar.transform, false);
            Image frameImg = frameObj.GetComponent<Image>();
            if (frameSprite != null) frameImg.sprite = frameSprite;
            
            RectTransform frameRt = frameObj.GetComponent<RectTransform>();
            frameRt.anchorMin = Vector2.zero;
            frameRt.anchorMax = Vector2.one;
            frameRt.sizeDelta = Vector2.zero;

            GameObject fillObj = new GameObject("PlayerHPBarFill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fillObj.transform.SetParent(hpBar.transform, false);
            Image fillImg = fillObj.GetComponent<Image>();
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = 0;
            fillImg.fillAmount = 1f;
            if (fillSprite != null) fillImg.sprite = fillSprite;

            RectTransform fillRt = fillObj.GetComponent<RectTransform>();
            fillRt.anchorMin = new Vector2(0.26f, 0.35666f);
            fillRt.anchorMax = new Vector2(0.92f, 0.65f);
            fillRt.offsetMin = new Vector2(-71f, 15f);
            fillRt.offsetMax = new Vector2(-3f, -5f);
            fillRt.localScale = new Vector3(0.95f, 3.5f, 1f);

            // Removed DontDestroyOnLoad call to fix non-root warning

            RectTransform rt = hpBar.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0f, -30f); 
                rt.sizeDelta = new Vector2(1000f, 300f);     
            }
        }
    }
}