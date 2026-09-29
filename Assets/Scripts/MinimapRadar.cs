using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimapRadar : MonoBehaviour
{
    public Transform player;
    public RectTransform radarPanel; // Drag your MinimapDisplay here
    
    [Header("Icon Sprites")]
    public Sprite playerSprite;
    public Sprite merchantSprite;
    public Sprite houseSprite;
    public Sprite portalSprite;

    [Header("Icon Colors")]
    public Color playerColor = Color.cyan;
    public Color merchantColor = Color.yellow;
    public Color houseColor = new Color(0.8f, 0.5f, 0.2f);
    public Color portalColor = Color.magenta;
    public Color compassColor = Color.white;

    private GameObject playerMarker;
    private GameObject merchantMarker;
    private GameObject houseMarker;
    private GameObject portalMarker;

    private GameObject northText;
    private GameObject southText;
    private GameObject eastText;
    private GameObject westText;

    public float mapScale = 3f;
    public float iconSize = 44f; // Large icons

    void Start()
    {
        if (player == null)
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }

        if (radarPanel != null)
        {
            // Create game entity markers
            playerMarker = CreateMarker("PlayerMarker", playerSprite, playerColor);
            merchantMarker = CreateMarker("MerchantMarker", merchantSprite, merchantColor);
            houseMarker = CreateMarker("HouseMarker", houseSprite, houseColor);
            portalMarker = CreateMarker("PortalMarker", portalSprite, portalColor);

            // Create larger N, S, E, W compass labels around the perimeter
            float radius = (radarPanel.rect.width / 2f) - 18f;
            northText = CreateCompassLabel("Label_N", "N", new Vector2(0, radius));
            southText = CreateCompassLabel("Label_S", "S", new Vector2(0, -radius));
            eastText = CreateCompassLabel("Label_E", "E", new Vector2(radius, 0));
            westText = CreateCompassLabel("Label_W", "W", new Vector2(-radius, 0));
        }
    }

    GameObject CreateMarker(string markerName, Sprite sprite, Color color)
    {
        GameObject obj = new GameObject(markerName, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(radarPanel, false);
        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(iconSize, iconSize);
        return obj;
    }

    GameObject CreateCompassLabel(string labelName, string text, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(labelName, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(radarPanel, false);
        
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24; // Increased font size for clarity!
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = compassColor;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(36f, 36f); // Expanded box so text doesn't cut off
        rt.anchoredPosition = anchoredPos;
        return obj;
    }

    void Update()
    {
        if (player == null || radarPanel == null) return;

        UpdateMarkerPosition(playerMarker, player.position, player.position, true, true);

        GameObject merchantObj = GameObject.Find("SoulMerchant");
        UpdateMarkerPosition(merchantMarker, player.position, merchantObj != null ? merchantObj.transform.position : Vector3.zero, merchantObj != null);

        GameObject houseObj = GameObject.Find("House_Residential");
        UpdateMarkerPosition(houseMarker, player.position, houseObj != null ? houseObj.transform.position : Vector3.zero, houseObj != null);

        GameObject portalObj = GameObject.Find("RaidPortalToWilderness");
        UpdateMarkerPosition(portalMarker, player.position, portalObj != null ? portalObj.transform.position : Vector3.zero, portalObj != null);
    }

    void UpdateMarkerPosition(GameObject marker, Vector3 playerPos, Vector3 targetPos, bool isActive = true, bool isPlayer = false)
    {
        if (marker == null) return;
        marker.SetActive(isActive);
        if (!isActive) return;

        RectTransform rt = marker.GetComponent<RectTransform>();

        if (isPlayer)
        {
            rt.anchoredPosition = Vector2.zero;
            return;
        }

        Vector3 offset = targetPos - playerPos;
        Vector2 localPos = new Vector2(offset.x * mapScale, offset.y * mapScale);

        // Circular boundary clamping so icons stay neatly inside the round radar frame
        float maxRadius = (radarPanel.rect.width / 2f) - (iconSize / 2f);
        if (localPos.magnitude > maxRadius)
        {
            localPos = localPos.normalized * maxRadius;
        }

        rt.anchoredPosition = localPos;
    }
}