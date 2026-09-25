using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("UI References")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;

    private bool isDead = false;
    private bool secondWindUsedThisRun = false;

    void Awake()
    {
        isDead = false;
        secondWindUsedThisRun = false;
    }

    void Start()
    {
        InitializeHealth();
        FindUIReferences();
        UpdateHealthDisplay();
    }

    public void InitializeHealth()
    {
        int baseHp = 100;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.maxPlayerHealth > 0)
            {
                baseHp = GameManager.Instance.maxPlayerHealth;
            }
        }

        // Apply Glass Cannon modifier if active in Rift
        if (RiftManager.Instance != null && RiftManager.Instance.HasAffix(RiftAffixType.GlassCannon))
        {
            maxHealth = Mathf.Max(50, Mathf.RoundToInt(baseHp * 0.5f));
            Debug.Log($"<color=#FF4D4D>[RIFT AFFIX] Glass Cannon Active! Max HP halved to: {maxHealth} HP</color>");
        }
        else
        {
            maxHealth = Mathf.Max(50, baseHp);
        }

        // Restore saved health or default to max
        if (GameManager.Instance != null && GameManager.Instance.savedPlayerHealth > 0)
        {
            currentHealth = Mathf.Clamp(GameManager.Instance.savedPlayerHealth, 25, maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }

        SaveHealthToGameManager();
    }

    void Update()
    {
        if (healthBarFill == null || healthText == null)
        {
            FindUIReferences();
        }
    }

    public void ApplyMaxHealth(int newMaxHealth)
    {
        int diff = newMaxHealth - maxHealth;
        maxHealth = newMaxHealth;
        currentHealth += Mathf.Max(0, diff);
        SaveHealthToGameManager();
        UpdateHealthDisplay();
    }

    void FindUIReferences()
    {
        if (healthBarFill == null)
        {
            GameObject fillObj = GameObject.Find("PlayerHPBarFill");
            if (fillObj != null) healthBarFill = fillObj.GetComponent<Image>();
        }

        if (healthText == null)
        {
            GameObject textObj = GameObject.Find("PlayerHPText");
            if (textObj != null) healthText = textObj.GetComponent<TextMeshProUGUI>();
        }

        UpdateHealthDisplay();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        SaveHealthToGameManager();

        Debug.Log($"[PLAYER DAMAGED] Took {amount} damage! Current Health: {currentHealth} / {maxHealth}");

        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            if (PermanentUpgrades.HasSecondWind() && !secondWindUsedThisRun)
            {
                TriggerSecondWind();
                return;
            }

            Die();
        }
    }

    void TriggerSecondWind()
    {
        secondWindUsedThisRun = true;
        int reviveHp = Mathf.Max(10, Mathf.RoundToInt(maxHealth * 0.25f));
        currentHealth = reviveHp;
        SaveHealthToGameManager();
        UpdateHealthDisplay();

        Debug.Log($"<color=#33FF66>[SECOND WIND ACTIVATED!] Restored to {reviveHp} HP!</color>");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        SaveHealthToGameManager();
        Debug.Log($"[PLAYER HEALED] Current Health: {currentHealth} / {maxHealth}");
        UpdateHealthDisplay();
    }

    void SaveHealthToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.savedPlayerHealth = currentHealth;
        }
    }

    void UpdateHealthDisplay()
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PLAYER DIED] Defeated in combat!");

        if (RaidWaveManager.Instance != null)
        {
            RaidWaveManager.Instance.OnPlayerDefeatedInRaid();
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied(0, 1);
        }

        if (GameMenuController.Instance != null)
        {
            GameMenuController.Instance.TriggerGameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}