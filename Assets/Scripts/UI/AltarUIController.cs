using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Add this line

public class AltarUIController : MonoBehaviour
{
    public static AltarUIController Instance;

    [Header("--- UI PANEL & GENERAL ---")]
    public GameObject altarUIPanel;
    public TextMeshProUGUI availableSoulsText;

    [Header("--- GREED UPGRADE ---")]
    public TextMeshProUGUI greedStatusText;
    public Button buyGreedButton;
    public TextMeshProUGUI greedCostText;

    [Header("--- STARTING WEALTH ---")]
    public TextMeshProUGUI wealthStatusText;
    public Button buyWealthButton;
    public TextMeshProUGUI wealthCostText;

    [Header("--- SECOND WIND ---")]
    public TextMeshProUGUI secondWindStatusText;
    public Button buySecondWindButton;
    public TextMeshProUGUI secondWindCostText;

    [Header("--- CRIT MASTERY ---")]
    public TextMeshProUGUI masteryStatusText;
    public Button buyMasteryButton;
    public TextMeshProUGUI masteryCostText;

    [Header("--- CLOSE BUTTON ---")]
    public Button closeButton;

    private bool isAltarOpen = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (altarUIPanel != null) altarUIPanel.SetActive(false);

        if (buyGreedButton != null) buyGreedButton.onClick.AddListener(UpgradeGreed);
        if (buyWealthButton != null) buyWealthButton.onClick.AddListener(UpgradeStartingWealth);
        if (buySecondWindButton != null) buySecondWindButton.onClick.AddListener(UpgradeSecondWind);
        if (buyMasteryButton != null) buyMasteryButton.onClick.AddListener(UpgradeMastery);

        if (closeButton != null) closeButton.onClick.AddListener(CloseAltarUI);

        UpdateAllUI();
    }

    void Update()
    {
        if (isAltarOpen && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseAltarUI();
        }
    }

    public void OpenAltarUI()
    {
        isAltarOpen = true;
        if (altarUIPanel != null) altarUIPanel.SetActive(true);
        UpdateAllUI();

        // Ensure EventSystem is active when opening UI
        if (EventSystem.current == null)
        {
            GameObject es = GameObject.Find("EventSystem");
            if (es != null) es.SetActive(true);
        }

        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player != null)
        {
            var pMovement = player.GetComponent<PlayerMovement>();
            if (pMovement != null) pMovement.enabled = false;
        }
    }

    public void CloseAltarUI()
    {
        isAltarOpen = false;
        if (altarUIPanel != null) altarUIPanel.SetActive(false);

        Time.timeScale = 1f;

        Transform player = GameObject.FindWithTag("Player")?.transform;
        if (player != null)
        {
            var pMovement = player.GetComponent<PlayerMovement>();
            if (pMovement != null) pMovement.enabled = true;
        }
    }

    public void UpdateAllUI()
    {
        int totalSouls = 0;
        if (GameManager.Instance != null)
        {
            totalSouls = GameManager.Instance.totalSouls;
        }

        if (availableSoulsText != null)
        {
            availableSoulsText.text = $"Available Souls: {totalSouls}";
        }

        // 1. Greed UI (Infinite)
        if (greedStatusText != null)
        {
            float bonus = (PermanentUpgrades.GetDropRateMultiplier() - 1f) * 100f;
            string tierLabel = PermanentUpgrades.GreedLevel >= PermanentUpgrades.CoreLevelCap ? "(Paragon)" : "";
            greedStatusText.text = $"Greed: Level {PermanentUpgrades.GreedLevel} {tierLabel}\n(+{bonus:F0}% Drop Rate)";
        }
        if (greedCostText != null)
        {
            int cost = PermanentUpgrades.GetGreedCost();
            greedCostText.text = $"{cost} Souls";
            if (buyGreedButton != null) buyGreedButton.interactable = (totalSouls >= cost);
        }

        // 2. Starting Wealth UI (Infinite)
        if (wealthStatusText != null)
        {
            int gold = PermanentUpgrades.GetStartingCoinsBonus();
            string tierLabel = PermanentUpgrades.StartingWealthLevel >= PermanentUpgrades.CoreLevelCap ? "(Paragon)" : "";
            wealthStatusText.text = $"Wealth: Level {PermanentUpgrades.StartingWealthLevel} {tierLabel}\n(Start with {gold} Coins)";
        }
        if (wealthCostText != null)
        {
            int cost = PermanentUpgrades.GetWealthCost();
            wealthCostText.text = $"{cost} Souls";
            if (buyWealthButton != null) buyWealthButton.interactable = (totalSouls >= cost);
        }

        // 3. Second Wind UI (Cap 1)
        if (secondWindStatusText != null)
        {
            string status = PermanentUpgrades.SecondWindLevel > 0 ? "UNLOCKED (Revive at 25% HP)" : "LOCKED";
            secondWindStatusText.text = $"Second Wind: {status}";
        }
        if (secondWindCostText != null)
        {
            if (PermanentUpgrades.SecondWindLevel >= PermanentUpgrades.MaxSecondWindLevel)
            {
                secondWindCostText.text = "MAXED";
                if (buySecondWindButton != null) buySecondWindButton.interactable = false;
            }
            else
            {
                int cost = PermanentUpgrades.GetSecondWindCost();
                secondWindCostText.text = $"{cost} Souls";
                if (buySecondWindButton != null) buySecondWindButton.interactable = (totalSouls >= cost);
            }
        }

        // 4. Fire Mastery UI (Infinite)
        if (masteryStatusText != null)
        {
            float crit = PermanentUpgrades.GetCritChanceBonus() * 100f;
            string tierLabel = PermanentUpgrades.MasteryLevel >= PermanentUpgrades.CoreLevelCap ? "(Paragon)" : "";
            masteryStatusText.text = $"Fire Mastery: Level {PermanentUpgrades.MasteryLevel} {tierLabel}\n(+{crit:F1}% Crit Chance)";
        }
        if (masteryCostText != null)
        {
            int cost = PermanentUpgrades.GetMasteryCost();
            masteryCostText.text = $"{cost} Souls";
            if (buyMasteryButton != null) buyMasteryButton.interactable = (totalSouls >= cost);
        }
    }

    private void UpgradeGreed()
    {
        if (GameManager.Instance == null) return;
        int currentSouls = GameManager.Instance.totalSouls;
        if (PermanentUpgrades.BuyGreedRank(ref currentSouls))
        {
            GameManager.Instance.totalSouls = currentSouls;
            GameManager.Instance.SaveGame();
            GameManager.Instance.UpdateAllUI();
            UpdateAllUI();
        }
    }

    private void UpgradeStartingWealth()
    {
        if (GameManager.Instance == null) return;
        int currentSouls = GameManager.Instance.totalSouls;
        if (PermanentUpgrades.BuyWealthRank(ref currentSouls))
        {
            GameManager.Instance.totalSouls = currentSouls;
            GameManager.Instance.SaveGame();
            GameManager.Instance.UpdateAllUI();
            UpdateAllUI();
        }
    }

    private void UpgradeSecondWind()
    {
        if (GameManager.Instance == null) return;
        int currentSouls = GameManager.Instance.totalSouls;
        if (PermanentUpgrades.BuySecondWindRank(ref currentSouls))
        {
            GameManager.Instance.totalSouls = currentSouls;
            GameManager.Instance.SaveGame();
            GameManager.Instance.UpdateAllUI();
            UpdateAllUI();
        }
    }

    private void UpgradeMastery()
    {
        if (GameManager.Instance == null) return;
        int currentSouls = GameManager.Instance.totalSouls;
        if (PermanentUpgrades.BuyMasteryRank(ref currentSouls))
        {
            GameManager.Instance.totalSouls = currentSouls;
            GameManager.Instance.SaveGame();
            GameManager.Instance.UpdateAllUI();
            UpdateAllUI();
        }
    }
}