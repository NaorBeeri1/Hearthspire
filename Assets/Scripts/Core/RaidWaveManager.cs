using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class RaidWaveManager : MonoBehaviour
{
    public static RaidWaveManager Instance;

    public enum WaveState
    {
        PrepPhase,
        WaveInProgress,
        BossEncounter,
        DecisionPhase
    }

    [Header("--- STATE & WAVE CONFIG ---")]
    public WaveState currentState = WaveState.PrepPhase;
    public int currentWave = 1;
    public int totalWaveEnemies;
    public int remainingEnemies;
    public int highestWaveReached = 1;

    [Header("--- COIN ESCROW & VAULT ---")]
    public int securedCoins = 0;
    public int currentWaveCoins = 0;

    [Header("--- EXTRACTION / DECISION TIMER ---")]
    public float extractionWindowDuration = 10f;
    private float decisionTimeRemaining = 0f;

    [Header("--- PREFABS & SPAWN SETTINGS ---")]
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 8f;
    public float bossSpawnDistance = 7f;

    [Header("--- UI HUD REFERENCES ---")]
    public TextMeshProUGUI waveNumberText;
    public TextMeshProUGUI enemyCounterText;
    public TextMeshProUGUI enemyLevelText;
    public TextMeshProUGUI objectiveBannerText;
    public TextMeshProUGUI pendingLootText;
    public TextMeshProUGUI highestWaveText;
    [Tooltip("Dedicated countdown indicator showing how long until gate locks.")]
    public TextMeshProUGUI gateCountdownText;
    [Tooltip("Button to immediately close gate and start next wave.")]
    public Button skipTimerButton;

    [Header("--- PORTAL REFERENCE ---")]
    public ReturnPortal returnPortal;

    [Header("--- HUD COLOR THEMES ---")]
    public Color waveAmberColor = new Color(1f, 0.85f, 0.3f, 1f);       // #FFD94D Amber
    public Color bossAlertColor = new Color(1f, 0.15f, 0.15f, 1f);       // #FF2626 Crimson
    public Color portalUnlockedColor = new Color(0.2f, 1f, 0.4f, 1f);    // #33FF66 Neon Green
    public Color softWhiteColor = new Color(0.9f, 0.9f, 0.95f, 1f);      // Soft White
    public Color recordGoldColor = new Color(1f, 0.84f, 0f, 1f);         // #FFD700 Gold
    public Color urgentTimerColor = new Color(1f, 0.3f, 0.3f, 1f);       // #FF4D4D Warning Red

    private GameObject activeBossInstance;
    private EnemyController activeBossController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentWave = 1;

        // Apply Permanent Upgrade: Starting Wealth bonus
        int startingBonus = PermanentUpgrades.GetStartingCoinsBonus();
        securedCoins = startingBonus;
        currentWaveCoins = 0;

        if (GameManager.Instance != null)
        {
            highestWaveReached = GameManager.Instance.highestWaveReached;
        }

        if (returnPortal != null)
        {
            returnPortal.SetPortalLockState(true);
        }

        if (gateCountdownText != null)
        {
            gateCountdownText.gameObject.SetActive(false);
        }

        if (skipTimerButton != null)
        {
            skipTimerButton.onClick.RemoveListener(SkipDecisionTimerAndAdvance);
            skipTimerButton.onClick.AddListener(SkipDecisionTimerAndAdvance);
            skipTimerButton.gameObject.SetActive(false);
        }

        StartWave(currentWave);
    }

    void Update()
    {
        if (currentState == WaveState.BossEncounter)
        {
            if (activeBossInstance == null)
            {
                OnBossDefeated();
            }
        }
        else if (currentState == WaveState.DecisionPhase)
        {
            decisionTimeRemaining -= Time.deltaTime;

            if (decisionTimeRemaining <= 0f)
            {
                decisionTimeRemaining = 0f;
                AdvanceToNextWave();
            }
        }

        UpdateHUD();
    }

    void StartWave(int wave)
    {
        currentWave = wave;
        currentWaveCoins = 0;

        if (currentWave == 1) totalWaveEnemies = 3;
        else if (currentWave == 2) totalWaveEnemies = 6;
        else if (currentWave == 3) totalWaveEnemies = 8;
        else if (currentWave == 4) totalWaveEnemies = 9;
        else totalWaveEnemies = 10;

        remainingEnemies = totalWaveEnemies;

        if (returnPortal != null)
        {
            returnPortal.SetPortalLockState(true);
        }

        if (gateCountdownText != null)
        {
            gateCountdownText.gameObject.SetActive(false);
        }

        if (skipTimerButton != null)
        {
            skipTimerButton.gameObject.SetActive(false);
        }

        if (currentWave > highestWaveReached)
        {
            highestWaveReached = currentWave;
        }

        UpdateHUD();
        StartCoroutine(StartWaveRoutine());
    }

    IEnumerator StartWaveRoutine()
    {
        currentState = WaveState.PrepPhase;
        UpdateHUD();

        yield return new WaitForSeconds(1.0f);

        currentState = WaveState.WaveInProgress;
        SpawnHorde(totalWaveEnemies, currentWave);
        UpdateHUD();
    }

    void SpawnHorde(int count, int waveLevel)
    {
        Transform player = GameObject.FindWithTag("Player")?.transform ?? GameObject.Find("Player")?.transform;
        if (player == null) return;

        GameObject enemyToSpawn = null;
        if (enemyPrefabs != null && enemyPrefabs.Length > 0 && enemyPrefabs[0] != null)
        {
            enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }
        else
        {
            enemyToSpawn = Resources.Load<GameObject>("PatrolEnemy");
        }

        if (enemyToSpawn == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPos = player.position + new Vector3(randomDir.x * dist, randomDir.y * dist, 0f);

            GameObject enemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
            EnemyController ctrl = enemy.GetComponent<EnemyController>();
            if (ctrl != null)
            {
                ctrl.isBoss = false;
                ctrl.detectionRange = 15f;
                ctrl.ApplyLevelScaling(waveLevel);
            }
        }
    }

    public void OnEnemyKilled(EnemyController enemy)
    {
        // Greed multiplier from permanent Altar upgrades
        float greedMult = PermanentUpgrades.GetDropRateMultiplier();

        // Rift Mutator multipliers
        float riftCoinMult = (RiftManager.Instance != null) ? RiftManager.Instance.GetCoinMultiplier() : 1.0f;
        float riftSoulMult = (RiftManager.Instance != null) ? RiftManager.Instance.GetSoulMultiplier() : 1.0f;

        int finalCoins = Mathf.RoundToInt(enemy.coinsReward * greedMult * riftCoinMult);
        int finalSouls = Mathf.RoundToInt(enemy.soulsReward * greedMult * riftSoulMult);

        currentWaveCoins += finalCoins;

        // DO NOT add souls automatically to GameManager here.
        // The physical drop on the ground will award the souls when picked up.
        if (enemy.dropOnDeathPrefab != null && finalSouls > 0)
        {
            GameObject drop = Instantiate(enemy.dropOnDeathPrefab, enemy.transform.position, Quaternion.identity);
            SoulPickup pickup = drop.GetComponent<SoulPickup>();
            if (pickup != null)
            {
                pickup.Initialize(finalSouls);
            }
        }

        if (currentState == WaveState.WaveInProgress)
        {
            remainingEnemies--;
            remainingEnemies = Mathf.Max(0, remainingEnemies);

            if (remainingEnemies <= 0)
            {
                TriggerBossPhase();
            }
        }

        UpdateHUD();
    }

    void TriggerBossPhase()
    {
        currentState = WaveState.BossEncounter;

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
            if (activeBossController != null)
            {
                activeBossController.isBoss = true;
                activeBossController.detectionRange = 25f;
                activeBossController.ApplyLevelScaling(currentWave);
            }
        }
        else
        {
            OnBossDefeated();
        }

        UpdateHUD();
    }

    void OnBossDefeated()
    {
        securedCoins += currentWaveCoins;
        currentWaveCoins = 0;

        currentState = WaveState.DecisionPhase;
        decisionTimeRemaining = extractionWindowDuration;

        if (returnPortal != null)
        {
            returnPortal.SetPortalLockState(false);
        }

        if (gateCountdownText != null)
        {
            gateCountdownText.gameObject.SetActive(true);
        }

        if (skipTimerButton != null)
        {
            skipTimerButton.gameObject.SetActive(true);
        }

        UpdateHUD();
    }

    public void SkipDecisionTimerAndAdvance()
    {
        if (currentState != WaveState.DecisionPhase) return;

        decisionTimeRemaining = 0f;
        AdvanceToNextWave();
    }

    void AdvanceToNextWave()
    {
        if (gateCountdownText != null)
        {
            gateCountdownText.gameObject.SetActive(false);
        }

        if (skipTimerButton != null)
        {
            skipTimerButton.gameObject.SetActive(false);
        }

        StartWave(currentWave + 1);
    }

    void UpdateHUD()
    {
        // 1. Current Wave Number
        if (waveNumberText != null)
        {
            waveNumberText.text = $"Wave: {currentWave}";
            waveNumberText.color = waveAmberColor;
        }

        // 2. Highest Wave Reached Record
        if (highestWaveText != null)
        {
            highestWaveText.text = $"Best Wave: {highestWaveReached}";
            highestWaveText.color = recordGoldColor;
        }

        // 3. Enemy Counter / Boss / Status
        if (enemyCounterText != null)
        {
            if (currentState == WaveState.PrepPhase)
            {
                enemyCounterText.text = "INCOMING HORDE...";
                enemyCounterText.color = waveAmberColor;
            }
            else if (currentState == WaveState.WaveInProgress)
            {
                enemyCounterText.text = $"Enemies Left: {remainingEnemies} / {totalWaveEnemies}";
                enemyCounterText.color = Color.white;
            }
            else if (currentState == WaveState.BossEncounter)
            {
                int hp = activeBossController != null ? activeBossController.currentHealth : 0;
                int maxHp = activeBossController != null ? activeBossController.maxHealth : 300;
                enemyCounterText.text = $"BOSS HP: {hp} / {maxHp}";
                enemyCounterText.color = bossAlertColor;
            }
            else if (currentState == WaveState.DecisionPhase)
            {
                enemyCounterText.text = "PORTAL UNLOCKED";
                enemyCounterText.color = portalUnlockedColor;
            }
        }

        // 4. Dedicated Gate Countdown Indicator
        if (gateCountdownText != null)
        {
            if (currentState == WaveState.DecisionPhase)
            {
                int sec = Mathf.CeilToInt(decisionTimeRemaining);
                gateCountdownText.text = $"Gate Closes In: {sec}s";
                gateCountdownText.color = decisionTimeRemaining <= 3f ? urgentTimerColor : portalUnlockedColor;
            }
        }

        // 5. Enemy Level Indicator
        if (enemyLevelText != null)
        {
            enemyLevelText.text = $"Enemy Lvl: {currentWave}";
            enemyLevelText.color = (currentWave > 5) ? bossAlertColor : softWhiteColor;
        }

        // 6. Objective Banner
        if (objectiveBannerText != null)
        {
            if (currentState == WaveState.PrepPhase)
            {
                objectiveBannerText.text = "PREPARE YOURSELF";
                objectiveBannerText.color = waveAmberColor;
            }
            else if (currentState == WaveState.WaveInProgress)
            {
                objectiveBannerText.text = "ELIMINATE ALL HOSTILES";
                objectiveBannerText.color = softWhiteColor;
            }
            else if (currentState == WaveState.BossEncounter)
            {
                objectiveBannerText.text = "DEFEAT THE VOID REAPER!";
                objectiveBannerText.color = bossAlertColor;
            }
            else if (currentState == WaveState.DecisionPhase)
            {
                objectiveBannerText.text = "EXTRACT NOW VIA PORTAL [E] OR ENTER NEXT WAVE";
                objectiveBannerText.color = portalUnlockedColor;
            }
        }

        // 7. Coins Display (Secured vs At Risk)
        if (pendingLootText != null)
        {
            if (currentState == WaveState.DecisionPhase)
            {
                pendingLootText.text = $"Secured: +{securedCoins} Coins";
                pendingLootText.color = portalUnlockedColor;
            }
            else
            {
                pendingLootText.text = $"Secured: {securedCoins} | At Risk: +{currentWaveCoins}";
                pendingLootText.color = softWhiteColor;
            }
        }
    }

    public void OnPlayerDefeatedInRaid()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDied(securedCoins, highestWaveReached);
        }

        // Clear active rift mutators on death
        if (RiftManager.Instance != null)
        {
            RiftManager.Instance.ClearRift();
        }
    }

    public void BankRewardsAndExtract(int playerCurrentHp)
    {
        int totalCoins = securedCoins + currentWaveCoins;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRaidExtracted(totalCoins, currentWave, playerCurrentHp);
            GameManager.Instance.UpdateAllUI();
        }

        // Clear active rift mutators after returning safely
        if (RiftManager.Instance != null)
        {
            RiftManager.Instance.ClearRift();
        }

        securedCoins = 0;
        currentWaveCoins = 0;
    }
}