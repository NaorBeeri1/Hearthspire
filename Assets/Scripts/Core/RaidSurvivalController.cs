using UnityEngine;
using TMPro;

public class RaidSurvivalController : MonoBehaviour
{
    public static RaidSurvivalController Instance;

    public enum RaidState
    {
        SurvivalTimer,
        BossFight,
        ExtractionReady
    }

    [Header("--- RAID STATE ---")]
    public RaidState currentState = RaidState.SurvivalTimer;

    [Header("--- TIMER SETTINGS ---")]
    [Tooltip("Survival countdown before boss awakens (seconds).")]
    public float raidDuration = 60f;
    private float timeRemaining;

    [Header("--- BOSS SPAWN SETTINGS ---")]
    [Tooltip("Drag the VoidReaperBoss prefab here.")]
    public GameObject bossPrefab;
    [Tooltip("Distance away from the player to spawn the boss.")]
    public float bossSpawnDistance = 6f;
    private GameObject activeBossInstance;
    private EnemyController activeBossController;

    [Header("--- PENDING RUN REWARDS (ESCROW) ---")]
    public int pendingCoins = 0;
    public int pendingSouls = 0;
    private bool rewardsBanked = false;

    [Header("--- UI HUD REFERENCES ---")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI pendingLootText;

    [Header("--- PORTAL REFERENCE ---")]
    public ReturnPortal returnPortal;

    [Header("--- HUD COLOR PALETTES ---")]
    public Color timerActiveColor = new Color(1f, 0.85f, 0.3f, 1f);   // #FFD94D Warm Amber
    public Color timerUrgentColor = new Color(1f, 0.25f, 0.25f, 1f);   // #FF4040 Warning Red
    public Color bossAlertColor = new Color(1f, 0.15f, 0.15f, 1f);     // #FF2626 Crimson
    public Color portalUnlockedColor = new Color(0.2f, 1f, 0.4f, 1f);  // #33FF66 Neon Green
    public Color pendingLootColor = new Color(0.9f, 0.9f, 0.95f, 1f);

    public bool IsPortalUnlocked => currentState == RaidState.ExtractionReady;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentState = RaidState.SurvivalTimer;
        timeRemaining = raidDuration;
        rewardsBanked = false;
        pendingCoins = 0;
        pendingSouls = 0;

        if (returnPortal != null)
        {
            returnPortal.SetPortalLockState(true);
        }

        UpdateHUD();
    }

    void Update()
    {
        switch (currentState)
        {
            case RaidState.SurvivalTimer:
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0f)
                {
                    timeRemaining = 0f;
                    TriggerBossPhase();
                }
                break;

            case RaidState.BossFight:
                // Check if the boss has been destroyed
                if (activeBossInstance == null)
                {
                    OnBossDefeated();
                }
                break;

            case RaidState.ExtractionReady:
                // Extraction portal active
                break;
        }

        UpdateHUD();
    }

    public void AddPendingRewards(int coins, int souls)
    {
        pendingCoins += coins;
        pendingSouls += souls;
        UpdateHUD();
        Debug.Log($"[RAID LOOT] Added pending loot! Escrow: {pendingCoins} Coins, {pendingSouls} Souls.");
    }

    void TriggerBossPhase()
    {
        currentState = RaidState.BossFight;

        // Spawn Boss near player
        Transform player = GameObject.FindWithTag("Player")?.transform ?? GameObject.Find("Player")?.transform;
        Vector3 spawnPos = Vector3.zero;

        if (player != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            spawnPos = player.position + new Vector3(randomDir.x * bossSpawnDistance, randomDir.y * bossSpawnDistance, 0f);
        }

        if (bossPrefab != null)
        {
            activeBossInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            activeBossController = activeBossInstance.GetComponent<EnemyController>();
            Debug.Log("[RAID] VOID REAPER HAS AWOKEN!");
        }
        else
        {
            Debug.LogWarning("[RAID] Boss prefab not assigned! Unlocking portal directly.");
            OnBossDefeated();
        }
    }

    void OnBossDefeated()
    {
        currentState = RaidState.ExtractionReady;

        if (returnPortal != null)
        {
            returnPortal.SetPortalLockState(false);
        }

        Debug.Log("[RAID VICTORY] Void Reaper defeated! Extraction Portal opened!");
    }

    void UpdateHUD()
    {
        // 1. Timer Text
        if (timerText != null)
        {
            if (currentState == RaidState.SurvivalTimer)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60F);
                int seconds = Mathf.FloorToInt(timeRemaining % 60F);
                timerText.text = $"Survive: {minutes:00}:{seconds:00}";
                timerText.color = timeRemaining <= 15f ? timerUrgentColor : timerActiveColor;
            }
            else if (currentState == RaidState.BossFight)
            {
                timerText.text = "BOSS: VOID REAPER";
                timerText.color = bossAlertColor;
            }
            else
            {
                timerText.text = "VICTORY ACHIEVED";
                timerText.color = portalUnlockedColor;
            }
        }

        // 2. Objective Text
        if (objectiveText != null)
        {
            if (currentState == RaidState.SurvivalTimer)
            {
                objectiveText.text = "SURVIVE THE HORDE UNTIL THE SUMMONING";
                objectiveText.color = Color.white;
            }
            else if (currentState == RaidState.BossFight)
            {
                int hp = activeBossController != null ? activeBossController.currentHealth : 0;
                int maxHp = activeBossController != null ? activeBossController.maxHealth : 300;
                objectiveText.text = $"ELIMINATE THE VOID REAPER ({hp} / {maxHp} HP)";
                objectiveText.color = bossAlertColor;
            }
            else
            {
                objectiveText.text = "PORTAL UNLOCKED! ESCAPE TO COMPLETE THE RAID!";
                objectiveText.color = portalUnlockedColor;
            }
        }

        // 3. Pending Loot Text
        if (pendingLootText != null)
        {
            pendingLootText.text = $"Pending Loot: +{pendingCoins} Coins | +{pendingSouls} Souls";
            pendingLootText.color = (currentState == RaidState.ExtractionReady) ? portalUnlockedColor : pendingLootColor;
        }
    }

    public void BankRewards()
    {
        if (rewardsBanked) return;
        rewardsBanked = true;

        if (GameManager.Instance != null)
        {
            if (pendingCoins > 0) GameManager.Instance.AddScore(pendingCoins);
            if (pendingSouls > 0) GameManager.Instance.AddSouls(pendingSouls);
            GameManager.Instance.UpdateAllUI();
            Debug.Log($"[EXTRACTION SUCCESS] Banked {pendingCoins} Coins and {pendingSouls} Souls into persistent storage!");
        }

        pendingCoins = 0;
        pendingSouls = 0;
    }
}