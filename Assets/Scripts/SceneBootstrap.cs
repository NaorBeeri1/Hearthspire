using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneBootstrap : MonoBehaviour
{
    public GameObject managersPrefab;
    public GameObject minimapCanvasPrefab;
    public GameObject radarManagerPrefab;
    public GameObject shopCanvasPrefab;
    public GameObject hpBarPrefab; 

    void Awake()
    {
        // 1. Initialize Managers
        if (GameManager.Instance == null)
        {
            if (managersPrefab != null) Instantiate(managersPrefab);
        }

        // 2. Initialize Minimap & Radar
        if (FindObjectsByType<MinimapRadar>(FindObjectsSortMode.None).Length == 0)
        {
            if (minimapCanvasPrefab != null) Instantiate(minimapCanvasPrefab);
            if (radarManagerPrefab != null) Instantiate(radarManagerPrefab);
        }

        // 3. Initialize Shop Controller
        if (FindObjectsByType<ShopUIController>(FindObjectsSortMode.None).Length == 0)
        {
            if (shopCanvasPrefab != null) Instantiate(shopCanvasPrefab);
        }

        // 4. Ensure a persistent top-level UI Canvas exists globally
        Canvas activeCanvas = null;
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
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
            DontDestroyOnLoad(canvasObj); 

            activeCanvas = canvasObj.GetComponent<Canvas>();
            activeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            activeCanvas.sortingOrder = 999; 
            
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // 5. Build or Instantiate the Player HP Bar with your custom PNG art
        GameObject existingHPBar = GameObject.Find("PlayerHPBarContainer");
        if (existingHPBar == null)
        {
            GameObject targetPrefab = hpBarPrefab;
            if (targetPrefab == null)
            {
                targetPrefab = Resources.Load<GameObject>("PlayerHPBarContainer");
            }

            if (targetPrefab != null)
            {
                GameObject hpBar = Instantiate(targetPrefab, activeCanvas.transform, false);
                hpBar.name = "PlayerHPBarContainer";
                FinalizeHPBar(hpBar);
            }
            else
            {
                // Build container programmatically and inject Assets/Art/HpBar.png dynamically
                GameObject hpBar = new GameObject("PlayerHPBarContainer", typeof(RectTransform), typeof(CanvasRenderer));
                hpBar.transform.SetParent(activeCanvas.transform, false);

                // Load your exact HpBar sprite asset
                Sprite barSprite = LoadCustomArtSprite();

                // Build Frame / Background
                GameObject frameObj = new GameObject("HPBarFrame", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                frameObj.transform.SetParent(hpBar.transform, false);
                Image frameImg = frameObj.GetComponent<Image>();
                if (barSprite != null) frameImg.sprite = barSprite;

                // Build Fill Bar
                GameObject fillObj = new GameObject("PlayerHPBarFill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                fillObj.transform.SetParent(hpBar.transform, false);
                Image fillImg = fillObj.GetComponent<Image>();
                fillImg.type = Image.Type.Filled;
                fillImg.fillMethod = Image.FillMethod.Horizontal;
                fillImg.color = Color.white; // Keep white so texture colors show fully
                if (barSprite != null) fillImg.sprite = barSprite;

                FinalizeHPBar(hpBar);
                Debug.Log("[SceneBootstrap] Built runtime gothic HP Bar using Assets/Art/HpBar.png successfully!");
            }
        }
    }

    Sprite LoadCustomArtSprite()
    {
        Sprite loadedSprite = null;

#if UNITY_EDITOR
        // Target your exact file asset name from Assets/Art/
        string[] guids = AssetDatabase.FindAssets("HpBar t:Sprite");
        if (guids.Length == 0) guids = AssetDatabase.FindAssets("HpBar t:Texture2D");

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            loadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
#endif
        return loadedSprite;
    }

    void FinalizeHPBar(GameObject hpBar)
    {
        hpBar.SetActive(true);
        
        Transform rootTransform = hpBar.transform.root;
        if (rootTransform != null && rootTransform.GetComponent<PersistentUI>() == null)
        {
            rootTransform.gameObject.AddComponent<PersistentUI>();
        }

        RectTransform rt = hpBar.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -30f);
            rt.sizeDelta = new Vector2(500f, 110f);
        }
    }
}