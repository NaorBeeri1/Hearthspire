using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("--- CURRENCY ---")]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public int totalSouls = 0;
    public TextMeshProUGUI soulsText;

    [Header("--- CUSTOMIZATION ---")]
    public bool hasPurchasedWizardSkin = false;
    public bool isWizardSkinEquipped = false; // Tracks active outfit preference

    [Header("--- ROGUELIKE RAID PROGRESSION ---")]
    [Tooltip("Current wave in the active run. Always resets to 1 on fresh run.")]
    public int currentRaidWave = 1;
    [Tooltip("Highest wave ever cleared across all runs.")]
    public int highestWaveReached = 1;

    [Header("--- INFINITE WEAPON PROGRESSION ---")]
    public int fireballUpgradeLevel = 0;
    public int baseFireballDamage = 50;
    public int fireballDamagePerTier = 25;
    public int baseWeaponCoinCost = 20;
    public int baseWeaponSoulCost = 5;

    public int CurrentFireballDamage => baseFireballDamage + (fireballUpgradeLevel * fireballDamagePerTier);
    public int NextWeaponCoinCost => baseWeaponCoinCost + (fireballUpgradeLevel * 15);
    public int NextWeaponSoulCost => baseWeaponSoulCost + (fireballUpgradeLevel * 3);

    [Header("--- INFINITE HEALTH PROGRESSION ---")]
    public int healthUpgradeLevel = 0;
    public int basePlayerHealth = 100;
    public int healthPerTier = 25;
    public int baseHealthSoulCost = 10;

    public int maxPlayerHealth => basePlayerHealth + (healthUpgradeLevel * healthPerTier);
    public int savedPlayerHealth = 100;
    public int NextHealthSoulCost => baseHealthSoulCost + (healthUpgradeLevel * 8);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            LoadGame();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Start()
    {
        FindTextReferences();
        UpdateAllUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreText = null;
        soulsText = null;
        FindTextReferences();
        UpdateAllUI();
    }

    void Update()
    {
        if (soulsText == null || (SceneManager.GetActiveScene().name != "WildernessRaidScene" && scoreText == null))
        {
            FindTextReferences();
        }
    }

    void FindTextReferences()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "WildernessRaidScene")
        {
            GameObject coinObj = GameObject.Find("CoinsText");
            if (coinObj != null) coinObj.SetActive(false);

            GameObject raidSoulObj = GameObject.Find("RaidSoulsText");
            if (raidSoulObj != null)
            {
                soulsText = raidSoulObj.GetComponent<TextMeshProUGUI>();
            }
        }
        else
        {
            if (scoreText == null)
            {
                GameObject coinObj = GameObject.Find("CoinsText");
                if (coinObj != null) scoreText = coinObj.GetComponent<TextMeshProUGUI>();
            }

            if (soulsText == null)
            {
                GameObject soulObj = GameObject.Find("SoulsText");
                if (soulObj != null) soulsText = soulObj.GetComponent<TextMeshProUGUI>();
            }
        }

        UpdateAllUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public bool SpendScore(int amount)
    {
        if (score >= amount)
        {
            score -= amount;
            UpdateScoreText();
            return true;
        }
        return false;
    }

    public void AddSouls(int amount)
    {
        totalSouls += amount;
        UpdateSoulsText();
    }

    public bool SpendSouls(int amount)
    {
        if (totalSouls >= amount)
        {
            totalSouls -= amount;
            UpdateSoulsText();
            return true;
        }
        return false;
    }

    public bool UpgradeWeaponInfinite()
    {
        int coinCost = NextWeaponCoinCost;
        int soulCost = NextWeaponSoulCost;

        if (score >= coinCost && totalSouls >= soulCost)
        {
            SpendScore(coinCost);
            SpendSouls(soulCost);
            fireballUpgradeLevel++;
            UpdateAllUI();
            SaveGame();
            return true;
        }
        return false;
    }

    public bool UpgradeHealthInfinite()
    {
        int soulCost = NextHealthSoulCost;

        if (totalSouls >= soulCost)
        {
            SpendSouls(soulCost);
            healthUpgradeLevel++;
            savedPlayerHealth += healthPerTier;

            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ApplyMaxHealth(maxPlayerHealth);
            }

            UpdateAllUI();
            SaveGame();
            return true;
        }
        return false;
    }

    public void OnPlayerDied(int securedCoinsRetained, int highestWaveReachedInRun)
    {
        if (securedCoinsRetained > 0)
        {
            AddScore(securedCoinsRetained);
            Debug.Log($"[GameManager] Retained {securedCoinsRetained} coins secured from previous waves!");
        }

        if (highestWaveReachedInRun > highestWaveReached)
        {
            highestWaveReached = highestWaveReachedInRun;
        }

        savedPlayerHealth = 25;
        currentRaidWave = 1;
        SaveGame();
    }

    public void OnRaidExtracted(int totalCoinsToBank, int waveCleared, int currentHpAtExtraction)
    {
        if (totalCoinsToBank > 0)
        {
            AddScore(totalCoinsToBank);
        }

        savedPlayerHealth = currentHpAtExtraction;

        if (waveCleared > highestWaveReached)
        {
            highestWaveReached = waveCleared;
            Debug.Log($"[GameManager] New record! Highest wave reached: {highestWaveReached}");
        }

        currentRaidWave = 1;
        SaveGame();
    }

    public void ResetAllGameProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        score = 0;
        totalSouls = 0;
        fireballUpgradeLevel = 0;
        healthUpgradeLevel = 0;
        highestWaveReached = 1;
        currentRaidWave = 1;
        savedPlayerHealth = basePlayerHealth;
        hasPurchasedWizardSkin = false;
        isWizardSkinEquipped = false;

        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.maxHealth = basePlayerHealth;
            playerHealth.currentHealth = basePlayerHealth;
            playerHealth.SendMessage("UpdateHealthDisplay", SendMessageOptions.DontRequireReceiver);
        }

        UpdateAllUI();
        Debug.Log("<color=#33FF66>[GameManager] ALL SAVE DATA HAS BEEN PURGED AND RESET TO ZERO!</color>");
    }

    public void UpdateAllUI()
    {
        UpdateScoreText();
        UpdateSoulsText();
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Coins: " + score;
        }
    }

    public void UpdateSoulsText()
    {
        if (soulsText != null)
        {
            soulsText.text = "Souls: " + totalSouls;
        }
    }

    // --- SAVE & LOAD SYSTEM ---
    public void SaveGame()
    {
        PlayerPrefs.SetInt("Save_Score", score);
        PlayerPrefs.SetInt("Save_Souls", totalSouls);
        PlayerPrefs.SetInt("Save_SavedHP", savedPlayerHealth);
        PlayerPrefs.SetInt("Save_WeaponLevel", fireballUpgradeLevel);
        PlayerPrefs.SetInt("Save_HealthLevel", healthUpgradeLevel);
        PlayerPrefs.SetInt("Save_HighestWave", highestWaveReached);
        PlayerPrefs.SetInt("Save_WizardSkin", hasPurchasedWizardSkin ? 1 : 0);
        PlayerPrefs.SetInt("Save_WizardEquipped", isWizardSkinEquipped ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("<color=#33FF66>[GameManager] Game Saved Successfully!</color>");
    }

    public void LoadGame()
    {
        // Use a persistent general check or load with default fallbacks
        score = PlayerPrefs.GetInt("Save_Score", 0);
        totalSouls = PlayerPrefs.GetInt("Save_Souls", 0);
        fireballUpgradeLevel = PlayerPrefs.GetInt("Save_WeaponLevel", 0);
        healthUpgradeLevel = PlayerPrefs.GetInt("Save_HealthLevel", 0);
        highestWaveReached = PlayerPrefs.GetInt("Save_HighestWave", 1);
        savedPlayerHealth = PlayerPrefs.GetInt("Save_SavedHP", maxPlayerHealth);
        hasPurchasedWizardSkin = PlayerPrefs.GetInt("Save_WizardSkin", 0) == 1;
        isWizardSkinEquipped = PlayerPrefs.GetInt("Save_WizardEquipped", 0) == 1;
        
        Debug.Log("<color=#66CCFF>[GameManager] Game Loaded from disk! WizardSkin Unlocked: " + hasPurchasedWizardSkin + ", Equipped: " + isWizardSkinEquipped + "</color>");
    }
}