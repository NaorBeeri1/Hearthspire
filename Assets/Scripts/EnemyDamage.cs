using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageAmount = 1;
    private float lastDamageTime = -1f;
    public float damageCooldown = 0.5f; // Prevents multi-hit flooding

    void OnTriggerStay2D(Collider2D other)
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