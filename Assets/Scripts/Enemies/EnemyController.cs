using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("--- ENEMY IDENTITY & SCALING ---")]
    [Tooltip("Check this true for the Void Reaper Boss prefab.")]
    public bool isBoss = false;
    public int enemyLevel = 1;

    [Header("--- HEALTH SETTINGS ---")]
    [Tooltip("Base maximum health of this enemy. 100 for minions, 300 for the boss.")]
    public int baseMaxHealth = 100;
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int currentHealth;

    [Header("--- HEALTH BAR UI ---")]
    public Image healthBarFill;

    [Header("--- MOVEMENT SPEEDS ---")]
    public float basePatrolSpeed = 2f;
    public float baseChaseSpeed = 3.5f;
    public float patrolDistance = 3f;

    private float currentPatrolSpeed;
    private float currentChaseSpeed;

    [Header("--- DETECTION SETTINGS ---")]
    public float detectionRange = 5f;
    public bool flipSpriteWithMovement = true;

    [Header("--- ATTACK & DAMAGE ---")]
    public int baseAttackDamage = 25;
    public float baseAttackCooldown = 0.8f;

    private int currentAttackDamage;
    private float currentAttackCooldown;

    [Header("--- KILL REWARDS ---")]
    public int baseSoulsReward = 1;
    public int baseCoinsReward = 2;
    [HideInInspector]
    public int soulsReward;
    [HideInInspector]
    public int coinsReward;

    [Header("--- LOOT / DROPS (OPTIONAL) ---")]
    public GameObject dropOnDeathPrefab;

    // Internal state
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform playerTransform;
    private Vector2 spawnPosition;
    private bool movingRight = true;
    private float lastAttackTime = -1f;
    private bool isDead = false;

    void Awake()
    {
        InitializeStats();
    }

    void InitializeStats()
    {
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
        currentPatrolSpeed = basePatrolSpeed;
        currentChaseSpeed = baseChaseSpeed;
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
        soulsReward = baseSoulsReward;
        coinsReward = baseCoinsReward;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        spawnPosition = transform.position;

        FindPlayer();
        UpdateHealthBar();
    }

    public void ApplyLevelScaling(int waveLevel)
    {
        enemyLevel = waveLevel;
        InitializeStats();

        // Enforce base boss stats
        if (isBoss)
        {
            if (baseMaxHealth < 300) baseMaxHealth = 300;
            maxHealth = 300;
            currentHealth = 300;
            baseCoinsReward = 20;
            baseSoulsReward = 10;
            coinsReward = 20;
            soulsReward = 10;
        }

        // Post-Wave 5 scaling
        if (waveLevel > 5)
        {
            int extraTiers = waveLevel - 5;
            if (isBoss)
            {
                maxHealth = baseMaxHealth + (extraTiers * 60);
                currentAttackDamage = baseAttackDamage + (extraTiers * 10);
                coinsReward = baseCoinsReward + (extraTiers * 5);
                currentPatrolSpeed = basePatrolSpeed + (extraTiers * 0.2f);
                currentChaseSpeed = baseChaseSpeed + (extraTiers * 0.35f);
                currentAttackCooldown = Mathf.Max(0.4f, baseAttackCooldown - (extraTiers * 0.05f));
            }
            else
            {
                maxHealth = baseMaxHealth + (extraTiers * 20);
                currentAttackDamage = baseAttackDamage + (extraTiers * 5);
                coinsReward = baseCoinsReward + (extraTiers * 1);
                currentPatrolSpeed = basePatrolSpeed + (extraTiers * 0.15f);
                currentChaseSpeed = baseChaseSpeed + (extraTiers * 0.25f);
                currentAttackCooldown = Mathf.Max(0.35f, baseAttackCooldown - (extraTiers * 0.05f));
            }
        }

        // --- APPLY ACTIVE RIFT AFFIXES ---
        if (RiftManager.Instance != null && RiftManager.Instance.isRiftActive)
        {
            // 1. Soul Storm: Enemies move 20% faster
            if (RiftManager.Instance.HasAffix(RiftAffixType.SoulStorm))
            {
                currentChaseSpeed *= 1.20f;
                currentPatrolSpeed *= 1.20f;
            }

            // 2. Enraged Host: Enemies deal +25% damage
            if (RiftManager.Instance.HasAffix(RiftAffixType.EnragedHost))
            {
                currentAttackDamage = Mathf.RoundToInt(currentAttackDamage * 1.25f);
            }

            // 3. Steelclad: Enemies have double max health
            if (RiftManager.Instance.HasAffix(RiftAffixType.Steelclad))
            {
                maxHealth *= 2;
            }
        }

        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void FindPlayer()
    {
        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj == null) pObj = GameObject.Find("Player");
        if (pObj != null) playerTransform = pObj.transform;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (playerTransform == null)
        {
            FindPlayer();
            PerformPatrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            PerformChase();
        }
        else
        {
            PerformPatrol();
        }
    }

    void PerformPatrol()
    {
        float displacement = transform.position.x - spawnPosition.x;

        if (displacement > patrolDistance)
        {
            movingRight = false;
        }
        else if (displacement < -patrolDistance)
        {
            movingRight = true;
        }

        float direction = movingRight ? 1f : -1f;
        Vector2 targetPos = rb.position + new Vector2(direction * currentPatrolSpeed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(targetPos);

        HandleFacing(direction);
    }

    void PerformChase()
    {
        Vector2 targetPos = Vector2.MoveTowards(rb.position, playerTransform.position, currentChaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(targetPos);

        float horizontalDelta = playerTransform.position.x - transform.position.x;
        HandleFacing(horizontalDelta);
    }

    void HandleFacing(float horizontalDirection)
    {
        if (!flipSpriteWithMovement || sr == null) return;

        if (horizontalDirection > 0.05f)
        {
            sr.flipX = false;
        }
        else if (horizontalDirection < -0.05f)
        {
            sr.flipX = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDealDamage(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDealDamage(collision.collider);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDealDamage(collision.collider);
    }

    void TryDealDamage(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") || other.name == "Player")
        {
            if (Time.time - lastAttackTime >= currentAttackCooldown)
            {
                PlayerHealth pHealth = other.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    lastAttackTime = Time.time;
                    pHealth.TakeDamage(currentAttackDamage);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (RaidWaveManager.Instance != null)
        {
            RaidWaveManager.Instance.OnEnemyKilled(this);
        }
        else
        {
            // Fallback for non-raid scenes
            if (dropOnDeathPrefab != null)
            {
                GameObject drop = Instantiate(dropOnDeathPrefab, transform.position, Quaternion.identity);
                SoulPickup pickup = drop.GetComponent<SoulPickup>();
                if (pickup != null)
                {
                    pickup.Initialize(soulsReward);
                }
            }

            if (GameManager.Instance != null && coinsReward > 0)
            {
                GameManager.Instance.AddScore(coinsReward);
            }
        }

        Destroy(gameObject);
    }
}