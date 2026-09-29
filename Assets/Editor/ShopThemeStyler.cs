using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ShopThemeStyler : EditorWindow
{
    [MenuItem("Tools/Apply Epic Dark Fantasy Shop Theme")]
    public static void ApplyTheme()
    {
        GameObject shopCanvas = GameObject.Find("ShopCanvas");
        if (shopCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'ShopCanvas' in the active scene!", "OK");
            return;
        }

        Transform panelTransform = shopCanvas.transform.Find("ShopPanel");
        if (panelTransform != null)
        {
            panelTransform.gameObject.SetActive(true);

            // Stretch ShopPanel across all borders (Full Screen Overlay)
            RectTransform panelRt = panelTransform.GetComponent<RectTransform>();
            if (panelRt != null)
            {
                panelRt.anchorMin = Vector2.zero;
                panelRt.anchorMax = Vector2.one;
                panelRt.offsetMin = Vector2.zero;
                panelRt.offsetMax = Vector2.zero;
            }

            Image panelImage = panelTransform.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0.05f, 0.04f, 0.08f, 0.96f);
            }

            VerticalLayoutGroup vlg = panelTransform.GetComponent<VerticalLayoutGroup>();
            if (vlg != null) DestroyImmediate(vlg);

            ContentSizeFitter csf = panelTransform.GetComponent<ContentSizeFitter>();
            if (csf != null) DestroyImmediate(csf);

            // Style Title (Scaled down to fit cleanly)
            Transform titleTransform = panelTransform.Find("ShopTitleText");
            if (titleTransform != null)
            {
                RectTransform rt = titleTransform.GetComponent<RectTransform>();
                TextMeshProUGUI tmp = titleTransform.GetComponentInChildren<TextMeshProUGUI>();
                
                if (rt)
                {
                    rt.anchorMin = new Vector2(0.5f, 0.80f);
                    rt.anchorMax = new Vector2(0.5f, 0.80f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.sizeDelta = new Vector2(700f, 75f); // Scaled down width/height
                }
                if (titleTransform.GetComponent<Button>() != null) DestroyImmediate(titleTransform.GetComponent<Button>());
                if (titleTransform.GetComponent<Image>() != null) titleTransform.GetComponent<Image>().enabled = false;

                if (tmp)
                {
                    tmp.text = "SOUL MERCHANT";
                    tmp.color = new Color(1f, 0.84f, 0.31f); // Gold
                    tmp.fontSize = 42; // Scaled down so it fits comfortably
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            // Style Blue Currency Text
            Transform balanceTransform = panelTransform.Find("PlayerBalanceText");
            if (balanceTransform != null)
            {
                RectTransform rt = balanceTransform.GetComponent<RectTransform>();
                TextMeshProUGUI tmp = balanceTransform.GetComponentInChildren<TextMeshProUGUI>();
                
                if (rt)
                {
                    rt.anchorMin = new Vector2(0.5f, 0.72f);
                    rt.anchorMax = new Vector2(0.5f, 0.72f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.sizeDelta = new Vector2(700f, 60f);
                }
                if (balanceTransform.GetComponent<Button>() != null) DestroyImmediate(balanceTransform.GetComponent<Button>());
                if (balanceTransform.GetComponent<Image>() != null) balanceTransform.GetComponent<Image>().enabled = false;

                if (tmp)
                {
                    tmp.color = new Color(0.55f, 0.78f, 1f); // Ice Blue
                    tmp.fontSize = 32;
                    tmp.fontStyle = FontStyles.Bold;
                    tmp.alignment = TextAlignmentOptions.Center;
                }
            }

            // Style Buttons
            string[] buttonOrder = { "TradeButton", "BuySkinButton", "CloseButton" };
            float startY = 10f; 
            float spacingY = 115f; 

            Color buttonBgColor = new Color(0.18f, 0.15f, 0.28f, 1f);
            Color buttonHoverColor = new Color(0.32f, 0.26f, 0.48f, 1f);

            for (int i = 0; i < buttonOrder.Length; i++)
            {
                Transform btnTransform = panelTransform.Find(buttonOrder[i]);
                if (btnTransform != null)
                {
                    RectTransform rt = btnTransform.GetComponent<RectTransform>();
                    TextMeshProUGUI tmp = btnTransform.GetComponentInChildren<TextMeshProUGUI>();
                    Button btn = btnTransform.GetComponent<Button>();
                    Image img = btnTransform.GetComponent<Image>();

                    if (rt)
                    {
                        rt.anchorMin = new Vector2(0.5f, 0.5f);
                        rt.anchorMax = new Vector2(0.5f, 0.5f);
                        rt.pivot = new Vector2(0.5f, 0.5f);
                        rt.anchoredPosition = new Vector2(0f, startY - (i * spacingY));
                        rt.sizeDelta = new Vector2(640f, 95f);
                    }

                    if (img != null)
                    {
                        img.enabled = true;
                        img.color = buttonBgColor;
                    }

                    if (btn != null)
                    {
                        btn.interactable = true;
                        ColorBlock colors = btn.colors;
                        colors.normalColor = buttonBgColor;
                        colors.highlightedColor = buttonHoverColor;
                        colors.pressedColor = new Color(0.12f, 0.10f, 0.18f, 1f);
                        colors.selectedColor = buttonBgColor;
                        colors.colorMultiplier = 1f;
                        btn.colors = colors;
                    }

                    if (tmp)
                    {
                        tmp.color = new Color(0.95f, 0.93f, 0.85f);
                        tmp.fontSize = 28;
                        tmp.fontStyle = FontStyles.Bold;
                        tmp.alignment = TextAlignmentOptions.Center;
                    }
                }
            }

            panelTransform.gameObject.SetActive(false);
        }

        // Save Prefab Asset
        string prefabPath = "Assets/Prefabs/ShopCanvas.prefab";
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset != null)
        {
            PrefabUtility.SaveAsPrefabAsset(shopCanvas, prefabPath);
        }

        EditorUtility.SetDirty(shopCanvas);
        EditorUtility.DisplayDialog("Perfected!", "Title resized to fit comfortably, and top-left HUD currency now hides automatically when shopping!", "OK");
    }
}