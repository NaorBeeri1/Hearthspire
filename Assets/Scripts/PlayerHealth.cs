using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Reference")]
    public Image healthBarFill;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Auto-find fill bar if not assigned in Inspector
        if (healthBarFill == null)
        {
            GameObject fillObj = GameObject.Find("PlayerHPBarFill");
            if (fillObj != null) healthBarFill = fillObj.GetComponent<Image>();
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // Don't drop below 0
        
        // Explicit console log showing exact HP after attack
        Debug.Log($"[PLAYER DAMAGED] Took {amount} damage! Current Health: {currentHealth} / {maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[PLAYER HEALED] Current Health: {currentHealth} / {maxHealth}");
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        
        // Trigger the Game Over UI panel via our manager if available
        if (GameMenuController.Instance != null)
        {
            GameMenuController.Instance.TriggerGameOver();
        }
        else
        {
            // Fallback if menu controller isn't in scene yet
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}