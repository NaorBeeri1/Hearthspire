using UnityEngine;
using TMPro;

public class UIHudManager : MonoBehaviour
{
    private TextMeshProUGUI coinsTextMesh;
    private TextMeshProUGUI soulsTextMesh;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        CleanAndSetupTextElements();

        if (coinsTextMesh != null)
        {
            PositionAndStyleText(coinsTextMesh.rectTransform, new Vector2(40f, -20f));
        }
        if (soulsTextMesh != null)
        {
            PositionAndStyleText(soulsTextMesh.rectTransform, new Vector2(40f, -80f));
        }
    }

    void CleanAndSetupTextElements()
    {
        TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

        foreach (var tmp in allTexts)
        {
            if (tmp.gameObject.name == "CoinsText")
            {
                if (coinsTextMesh == null)
                {
                    coinsTextMesh = tmp;
                    SetupPrimaryText(coinsTextMesh);
                }
                else if (tmp != coinsTextMesh)
                {
                    Destroy(tmp.gameObject);
                }
            }
            else if (tmp.gameObject.name == "SoulsText")
            {
                if (soulsTextMesh == null)
                {
                    soulsTextMesh = tmp;
                    SetupPrimaryText(soulsTextMesh);
                }
                else if (tmp != soulsTextMesh)
                {
                    Destroy(tmp.gameObject);
                }
            }
        }
    }

    void SetupPrimaryText(TextMeshProUGUI tmp)
    {
        if (tmp.transform.parent != transform)
        {
            tmp.transform.SetParent(transform, false);
        }

        tmp.gameObject.SetActive(true);

        tmp.fontSize = 42;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = new Color(1f, 0.92f, 0.7f);
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        tmp.fontSharedMaterial.EnableKeyword("OUTLINE_ON");
        tmp.outlineColor = new Color32(20, 10, 5, 255);
        tmp.outlineWidth = 0.2f;
    }

    void PositionAndStyleText(RectTransform rt, Vector2 anchoredPos)
    {
        if (rt == null) return;

        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(400f, 60f);
    }
}