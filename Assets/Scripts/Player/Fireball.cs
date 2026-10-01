using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public bool isCritical = false;
    private Vector2 direction;

    public void Initialize(Vector2 dir, bool crit = false)
    {
        direction = dir.normalized;
        isCritical = crit;

        if (isCritical)
        {
            transform.localScale *= 1.35f;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 0.45f, 0.1f, 1f);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.gameObject.name.Contains("Enemy"))
        {
            // 1. Primary path: EnemyController handles health, death, and drops
            EnemyController enemyCtrl = other.GetComponent<EnemyController>();
            if (enemyCtrl != null)
            {
                enemyCtrl.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 2. Legacy fallback
            Enemy legacyEnemy = other.GetComponent<Enemy>();
            if (legacyEnemy != null)
            {
                legacyEnemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // 3. Fallback destroy for non-controller targets
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}