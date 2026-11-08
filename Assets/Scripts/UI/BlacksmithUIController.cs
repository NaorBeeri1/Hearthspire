using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class BlacksmithUIController : MonoBehaviour
{
    public static BlacksmithUIController Instance;

    [Header("UI References")]
    public GameObject shopPanel;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI weaponStatusText;
    public Button upgradeDamageButton;
    public TextMeshProUGUI upgradeButtonText;
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

        if (upgradeDamageButton != null)
        {
            upgradeDamageButton.onClick.RemoveListener(OnUpgradeClicked);
            upgradeDamageButton.onClick.AddListener(OnUpgradeClicked);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ToggleShop);
            closeButton.onClick.AddListener(ToggleShop);
        }

        UpdateUI();
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
            Debug.Log("[Blacksmith] Shop Opened.");
        }
        else
        {
            Debug.Log("[Blacksmith] Shop Closed.");
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

        // 1. Balance Text
        if (balanceText != null)
        {
            balanceText.text = $"Coins: {GameManager.Instance.score}  |  Souls: {GameManager.Instance.totalSouls}";
        }

        // 2. Status & Next Level Info
        int currentDmg = GameManager.Instance.CurrentFireballDamage;
        int nextDmg = currentDmg + GameManager.Instance.fireballDamagePerTier;
        int coinCost = GameManager.Instance.NextWeaponCoinCost;
        int soulCost = GameManager.Instance.NextWeaponSoulCost;

        if (weaponStatusText != null)
        {
            weaponStatusText.text = $"Fireball Damage: {currentDmg} (Level {GameManager.Instance.fireballUpgradeLevel})";
        }

        // 3. Button Interactability & Text
        if (upgradeDamageButton != null)
        {
            bool canAfford = (GameManager.Instance.score >= coinCost && GameManager.Instance.totalSouls >= soulCost);
            upgradeDamageButton.interactable = canAfford;

            if (upgradeButtonText != null)
            {
                upgradeButtonText.text = $"Forge Damage (+25 -> {nextDmg}) [{coinCost} Coins, {soulCost} Souls]";
            }
        }
    }

    public void OnUpgradeClicked()
    {
        Debug.Log("<color=#33FF66>[BLACKSMITH] Upgrade button clicked!</color>");

        if (GameManager.Instance == null)
        {
            Debug.LogError("<color=red>[BLACKSMITH ERROR] GameManager.Instance is NULL!</color>");
            return;
        }

        bool success = GameManager.Instance.UpgradeWeaponInfinite();
        if (success)
        {
            Debug.Log("<color=#33FF66>[BLACKSMITH SUCCESS] Weapon upgraded successfully!</color>");
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("<color=orange>[BLACKSMITH WARNING] Upgrade failed! Check currency values in GameManager.</color>");
        }
    }
}