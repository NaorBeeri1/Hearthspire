using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        
        // Calculate the angle in degrees from the direction vector and rotate the fireball sprite
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Destroy fireball after 3 seconds if it hits nothing
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
            Destroy(other.gameObject);
            Debug.Log("Enemy destroyed by fireball!");
            Destroy(gameObject);
        }
    }
}