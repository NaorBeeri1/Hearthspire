using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageAmount = 25;
    public float damageCooldown = 0.8f;
    private float lastDamageTime = -1f;

    void Awake()
    {
        // Enforce 25 damage if old prefab data is still saved in the Inspector
        if (damageAmount < 25)
        {
            damageAmount = 25;
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

    void TryDealDamage(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name == "Player")
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                PlayerHealth health = other.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    lastDamageTime = Time.time;
                    health.TakeDamage(damageAmount);
                }
            }
        }
    }
}