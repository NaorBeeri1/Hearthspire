using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class ShopUIController : MonoBehaviour
{
    public static ShopUIController Instance;

    [Header("UI References")]
    public GameObject shopPanel;
    public TextMeshProUGUI balanceText;
    public Button tradeButton;
    public Button buySkinButton;
    public Button buyHealthUpgradeButton;
    public TextMeshProUGUI healthUpgradeButtonText;
    public Button closeButton;

    [HideInInspector]
    public bool isShopOpen = false;

    [HideInInspector]
    public int lastClosedFrame = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);

        if (tradeButton != null) tradeButton.onClick.AddListener(OnTradeClicked);
        if (buySkinButton != null) buySkinButton.onClick.AddListener(OnBuySkinClicked);
        if (buyHealthUpgradeButton != null) buyHealthUpgradeButton.onClick.AddListener(OnHealthUpgradeClicked);
        if (closeButton != null) closeButton.onClick.AddListener(ToggleShop);
    }

    void Update()
    {
        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShop();
        }

        if (isShopOpen)
        {
            UpdateUI();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;

        if (!isShopOpen)
        {
            lastClosedFrame = Time.frameCount;
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
        }

        if (isShopOpen)
        {
            EnsureActiveEventSystem();
            UpdateUI();
            Debug.Log("[Shop] UI Opened.");
        }
        else
        {
            Debug.Log("[Shop] UI Closed.");
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.scoreText != null)
                GameManager.Instance.scoreText.gameObject.SetActive(!isShopOpen);

            if (GameManager.Instance.soulsText != null)
                GameManager.Instance.soulsText.gameObject.SetActive(!isShopOpen);

            if (!isShopOpen)
            {
                GameManager.Instance.UpdateAllUI();
            }
        }
    }

    private void EnsureActiveEventSystem()
    {
        if (EventSystem.current == null)
        {
            EventSystem existing = FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
            if (existing != null)
            {
                existing.gameObject.SetActive(true);
                existing.enabled = true;
                DontDestroyOnLoad(existing.gameObject);
                EventSystem.current = existing;
            }
            else
            {
                GameObject esObj = new GameObject("EventSystem_AutoRestored", typeof(EventSystem), typeof(InputSystemUIInputModule));
                DontDestroyOnLoad(esObj);
            }
        }
        else
        {
            EventSystem.current.gameObject.SetActive(true);
            EventSystem.current.enabled = true;
        }
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // 1. Balance Display
        if (balanceText != null)
        {
            balanceText.text = $"Souls: {GameManager.Instance.totalSouls}  |  Coins: {GameManager.Instance.score}";
        }

        // 2. Skin Button State
        if (buySkinButton != null)
        {
            TextMeshProUGUI skinBtnText = buySkinButton.GetComponentInChildren<TextMeshProUGUI>();
            if (GameManager.Instance.hasPurchasedWizardSkin)
            {
                buySkinButton.interactable = false;
                if (skinBtnText != null) skinBtnText.text = "Wizard Skin Unlocked";
            }
            else
            {
                buySkinButton.interactable = (GameManager.Instance.totalSouls >= 20);
                if (skinBtnText != null) skinBtnText.text = "Buy Wizard Skin (20 Souls)";
            }
        }

        // 3. Health Upgrade Button State
        if (buyHealthUpgradeButton != null && healthUpgradeButtonText != null)
        {
            int currentMaxHP = GameManager.Instance.maxPlayerHealth;
            int nextMaxHP = currentMaxHP + GameManager.Instance.healthPerTier;
            int soulCost = GameManager.Instance.NextHealthSoulCost;

            healthUpgradeButtonText.text = $"Upgrade HP (+25 -> {nextMaxHP}) [{soulCost} Souls]";
            buyHealthUpgradeButton.interactable = (GameManager.Instance.totalSouls >= soulCost);
        }
    }

    void OnTradeClicked()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.SpendSouls(5))
            {
                GameManager.Instance.AddScore(10);
                Debug.Log("UI Trade Successful! Exchanged 5 Souls for 10 Coins.");
                UpdateUI();
            }
            else
            {
                Debug.Log("Not enough souls to trade!");
            }
        }
    }

    void OnBuySkinClicked()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.hasPurchasedWizardSkin)
        {
            Debug.Log("[Shop] Wizard skin already owned!");
            return;
        }

        if (GameManager.Instance.SpendSouls(20))
        {
            GameManager.Instance.hasPurchasedWizardSkin = true;
            GameManager.Instance.isWizardSkinEquipped = true;
            GameManager.Instance.SaveGame();

            PlayerAppearance playerApp = FindAnyObjectByType<PlayerAppearance>();
            if (playerApp != null)
            {
                playerApp.ApplyAppearance();
            }

            Debug.Log("[Shop] Purchase Successful! Unlocked and equipped Special Wizard Skin.");
            UpdateUI();
        }
        else
        {
            Debug.Log("[Shop] Not enough souls to buy skin!");
        }
    }

    void OnHealthUpgradeClicked()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.UpgradeHealthInfinite())
        {
            UpdateUI();
        }
    }
}