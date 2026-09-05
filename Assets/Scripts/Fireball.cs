using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public GameObject soulPrefab; // Reference to our soul drop prefab
    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        
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
            // Spawn soul at enemy position if prefab is assigned
            if (soulPrefab != null)
            {
                Instantiate(soulPrefab, other.transform.position, Quaternion.identity);
            }

            Destroy(other.gameObject);
            Debug.Log("Enemy destroyed by fireball and dropped a soul!");
            Destroy(gameObject);
        }
    }
}