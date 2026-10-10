using UnityEngine;

public class DamageDiagnostic : MonoBehaviour
{
    private float lastDamageTime = -1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        LogCollision("Collision Enter", collision.gameObject, collision.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        LogCollision("Trigger Enter", other.gameObject, other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Log stay events if they happen within a fraction of a second to catch rapid multi-hits
        if (Time.time - lastDamageTime > 0.2f)
        {
            LogCollision("Trigger Stay (Rapid Check)", other.gameObject, other);
        }
    }

    void LogCollision(string eventType, GameObject hitObject, Collider2D hitCollider)
    {
        // Check if the object hitting us is an enemy or has damage components
        if (hitObject.name.Contains("Enemy") || hitObject.GetComponent<EnemyDamage>() != null || hitObject.GetComponent<PatrolEnemy>() != null)
        {
            lastDamageTime = Time.time;
            
            Debug.Log($"<color=yellow>[DIAGNOSTIC] Hit detected by '{hitObject.name}' via [{eventType}].</color>");
            Debug.Log($" - Collider IsTrigger: {hitCollider.isTrigger}, Rigidbody2D BodyType: {hitCollider.attachedRigidbody?.bodyType}");

            // Check current player health state right now
            PlayerHealth healthScript = GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                // Using reflection or standard logging to inspect internal health if accessible, 
                // otherwise we watch the console logs outputted by PlayerHealth.cs
                Debug.Log($" - PlayerHealth component found on {gameObject.name}. Current time: {Time.time:F2}s");
            }
            else
            {
                Debug.LogWarning($"<color=red> - WARNING: No 'PlayerHealth' script found on player object!</color>");
            }
        }
    }
}