using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnPortal : MonoBehaviour
{
    [Header("Portal Lock State")]
    public bool isLocked = true;

    [Header("Visual Feedback Colors")]
    public Color lockedColor = new Color(0.4f, 0.4f, 0.5f, 0.65f); // #666680 Dim
    public Color unlockedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); // #FFFFFF Bright

    private bool isNearPortal = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        UpdatePortalVisuals();
    }

    public void SetPortalLockState(bool locked)
    {
        isLocked = locked;
        UpdatePortalVisuals();
    }

    void UpdatePortalVisuals()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isLocked ? lockedColor : unlockedColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = false;
        }
    }

    void Update()
    {
        if (isNearPortal && Input.GetKeyDown(KeyCode.E))
        {
            if (isLocked)
            {
                Debug.LogWarning("[PORTAL] Cannot extract! Defeat the wave boss first.");
            }
            else
            {
                CompleteRaidExtraction();
            }
        }
    }

    void CompleteRaidExtraction()
    {
        int currentHp = 100;
        PlayerHealth pHealth = FindAnyObjectByType<PlayerHealth>();
        if (pHealth != null)
        {
            currentHp = pHealth.currentHealth;
        }

        if (RaidWaveManager.Instance != null)
        {
            RaidWaveManager.Instance.BankRewardsAndExtract(currentHp);
        }

        if (GameMenuController.Instance != null)
        {
            GameMenuController.Instance.TriggerVictory();
        }
        else
        {
            SceneManager.LoadScene("HometownScene");
        }
    }
}