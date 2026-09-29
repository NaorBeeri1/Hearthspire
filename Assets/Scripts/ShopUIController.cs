using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    public static ShopUIController Instance;

    [Header("UI References")]
    public GameObject shopPanel;
    public TextMeshProUGUI balanceText;
    public Button tradeButton;
    public Button buySkinButton;
    public Button closeButton;

    private bool isShopOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <-- Keep shop alive across scenes!
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Ensure static instance is always tied to the active controller
        Instance = this;
    }

    void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);

        if (tradeButton != null) tradeButton.onClick.AddListener(OnTradeClicked);
        if (buySkinButton != null) buySkinButton.onClick.AddListener(OnBuySkinClicked);
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
            UpdateBalanceDisplay();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
        }

        // Safely toggle top-left HUD text visibility without losing GameManager references
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.scoreText != null)
                GameManager.Instance.scoreText.gameObject.SetActive(!isShopOpen);

            if (GameManager.Instance.soulsText != null)
                GameManager.Instance.soulsText.gameObject.SetActive(!isShopOpen);
        }

        if (isShopOpen)
        {
            UpdateBalanceDisplay();
            Debug.Log("Shop UI Opened.");
        }
        else
        {
            Debug.Log("Shop UI Closed.");
        }
    }

    void UpdateBalanceDisplay()
    {
        if (balanceText != null && GameManager.Instance != null)
        {
            balanceText.text = $"Souls: {GameManager.Instance.totalSouls}  |  Coins: {GameManager.Instance.score}";
        }
    }

    void OnTradeClicked()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.totalSouls >= 5)
            {
                GameManager.Instance.totalSouls -= 5;
                GameManager.Instance.AddScore(10);
                Debug.Log("UI Trade Successful! Exchanged 5 Souls for 10 Coins.");
                UpdateBalanceDisplay();
            }
            else
            {
                Debug.Log("Not enough souls to trade!");
            }
        }
    }

    void OnBuySkinClicked()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.totalSouls >= 20)
            {
                GameManager.Instance.totalSouls -= 20;
                Debug.Log("UI Purchase Successful! Unlocked Special Wizard Skin.");
                UpdateBalanceDisplay();
            }
            else
            {
                Debug.Log("Not enough souls to buy skin!");
            }
        }
    }
}