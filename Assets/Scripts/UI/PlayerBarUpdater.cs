using UnityEngine;
using UnityEngine.UI;

public class PlayerBarUpdater : MonoBehaviour
{
    private Image fillImage;
    private PlayerHealth playerHealth;

    void Start()
    {
        FindReferences();
    }

    void Update()
    {
        if (fillImage == null || playerHealth == null)
        {
            FindReferences();
            return;
        }

        float currentHP = playerHealth.currentHealth;
        float maxHP = playerHealth.maxHealth;

        if (maxHP > 0)
        {
            fillImage.fillAmount = currentHP / maxHP;
        }
    }

    void FindReferences()
    {
        if (fillImage == null)
        {
            Transform fillTr = transform.Find("PlayerHPBarFill");
            if (fillTr != null) fillImage = fillTr.GetComponent<Image>();
        }

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag("Player") ?? GameObject.Find("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
    }
}