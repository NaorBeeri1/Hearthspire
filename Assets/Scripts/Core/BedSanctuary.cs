using UnityEngine;
using TMPro;

public class BedSanctuary : MonoBehaviour
{
    [Header("Resting Cost & Settings")]
    [Tooltip("Coins required to rest and heal to full.")]
    public int sleepCost = 5;

    [Header("UI Message Settings")]
    [Tooltip("Floating or overhead notification text.")]
    public TextMeshProUGUI notificationText;
    public float messageDisplayDuration = 2.5f;

    [Header("Visual Feedback Colors")]
    public Color promptColor = new Color(1.0f, 0.92f, 0.016f, 1.0f); // #FFEB04 (Golden Yellow)
    public Color healedColor = new Color(0.2f, 1.0f, 0.4f, 1.0f);    // #33FF66 (Vibrant Green)
    public Color errorColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);     // #FF4D4D (Warning Red)

    private bool isNearBed = false;
    private float hideMessageTime = -1f;

    void Start()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isNearBed && Input.GetKeyDown(KeyCode.E))
        {
            TryRestAndHeal();
        }

        if (hideMessageTime > 0f && Time.time >= hideMessageTime)
        {
            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
            hideMessageTime = -1f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearBed = true;
            ShowPrompt($"Press 'E' to Sleep & Restore HP ({sleepCost} Coins)", promptColor, -1f);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearBed = false;
            if (notificationText != null)
            {
                notificationText.gameObject.SetActive(false);
            }
            hideMessageTime = -1f;
        }
    }

    void TryRestAndHeal()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth == null) return;

        int missingHealth = playerHealth.maxHealth - playerHealth.currentHealth;
        if (missingHealth <= 0)
        {
            ShowPrompt("You are already at full health!", promptColor, messageDisplayDuration);
            return;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.SpendScore(sleepCost))
            {
                playerHealth.Heal(missingHealth);
                ShowPrompt($"Rested peacefully! Restored {missingHealth} HP for {sleepCost} Coins.", healedColor, messageDisplayDuration);
                Debug.Log($"[BED] Rested! -{sleepCost} Coins, +{missingHealth} HP.");
            }
            else
            {
                ShowPrompt($"Not enough coins! Need {sleepCost} coins to sleep.", errorColor, messageDisplayDuration);
            }
        }
    }

    void ShowPrompt(string message, Color textColor, float duration)
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = message;
            notificationText.color = textColor;

            if (duration > 0f)
            {
                hideMessageTime = Time.time + duration;
            }
            else
            {
                hideMessageTime = -1f;
            }
        }
    }
}