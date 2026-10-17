using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wardrobe : MonoBehaviour
{
    [Header("UI References")]
    public GameObject wardrobePanel;          // Drag your WardrobePanel here
    public Button selectDefaultButton;        // Drag SelectDefaultButton here
    public Button selectWizardButton;         // Drag SelectWizardButton here
    public Button closeButton;                // Drag CloseWardrobeButton here

    [Header("Interaction Prompt")]
    public TextMeshProUGUI notificationText;
    public Color promptColor = new Color(1.0f, 0.92f, 0.016f, 1.0f); // #FFEB04
    public Color errorColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);      // #FF4D4D

    private bool isNearWardrobe = false;
    private bool isPanelOpen = false;

    void Start()
    {
        if (wardrobePanel != null) wardrobePanel.SetActive(false);
        if (notificationText != null) notificationText.gameObject.SetActive(false);

        if (selectDefaultButton != null) selectDefaultButton.onClick.AddListener(SelectDefaultSkin);
        if (selectWizardButton != null) selectWizardButton.onClick.AddListener(SelectWizardSkin);
        if (closeButton != null) closeButton.onClick.AddListener(CloseWardrobe);
    }

    void Update()
    {
        if (isNearWardrobe && Input.GetKeyDown(KeyCode.E) && !isPanelOpen)
        {
            OpenWardrobe();
        }

        if (isPanelOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWardrobe();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearWardrobe = true;
            ShowPrompt("Press 'E' to open wardrobe", promptColor);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearWardrobe = false;
            CloseWardrobe();
            if (notificationText != null) notificationText.gameObject.SetActive(false);
        }
    }

    void OpenWardrobe()
    {
        isPanelOpen = true;
        if (wardrobePanel != null) wardrobePanel.SetActive(true);
        if (notificationText != null) notificationText.gameObject.SetActive(false);

        UpdateOutfitButtons();
        Time.timeScale = 0f; // Pause game while managing outfits
        Debug.Log("[Wardrobe] Panel Opened.");
    }

    public void CloseWardrobe()
    {
        isPanelOpen = false;
        if (wardrobePanel != null) wardrobePanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
        Debug.Log("[Wardrobe] Panel Closed.");
    }

    void UpdateOutfitButtons()
    {
        bool isWizardEquipped = (GameManager.Instance != null && GameManager.Instance.isWizardSkinEquipped);
        bool hasUnlockedWizard = (GameManager.Instance != null && GameManager.Instance.hasPurchasedWizardSkin);

        // 1. Default button is disabled if default is already equipped
        if (selectDefaultButton != null)
        {
            selectDefaultButton.interactable = isWizardEquipped;
        }

        // 2. Wizard button is disabled if not purchased OR already equipped
        if (selectWizardButton != null)
        {
            selectWizardButton.interactable = hasUnlockedWizard && !isWizardEquipped;

            TextMeshProUGUI wizardBtnText = selectWizardButton.GetComponentInChildren<TextMeshProUGUI>();
            if (wizardBtnText != null)
            {
                if (!hasUnlockedWizard)
                {
                    wizardBtnText.text = "Wizard (Locked)";
                }
                else if (isWizardEquipped)
                {
                    wizardBtnText.text = "Wizard (Equipped)";
                }
                else
                {
                    wizardBtnText.text = "Select Wizard";
                }
            }
        }
    }

    void SelectDefaultSkin()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isWizardSkinEquipped = false;
            GameManager.Instance.SaveGame();
        }

        PlayerAppearance playerApp = FindAnyObjectByType<PlayerAppearance>();
        if (playerApp != null)
        {
            playerApp.ApplyAppearance();
        }

        CloseWardrobe();
    }

    void SelectWizardSkin()
    {
        if (GameManager.Instance == null || !GameManager.Instance.hasPurchasedWizardSkin)
        {
            Debug.LogWarning("[Wardrobe] Cannot equip wizard skin - not unlocked!");
            return;
        }

        GameManager.Instance.isWizardSkinEquipped = true;
        GameManager.Instance.SaveGame();

        PlayerAppearance playerApp = FindAnyObjectByType<PlayerAppearance>();
        if (playerApp != null)
        {
            playerApp.ApplyAppearance();
        }

        CloseWardrobe();
    }

    void ShowPrompt(string message, Color textColor)
    {
        if (notificationText != null && !isPanelOpen)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = message;
            notificationText.color = textColor;
        }
    }
}