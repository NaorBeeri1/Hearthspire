using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float fireballSpeed = 10f;
    private Vector2 lastDirection = Vector2.right; // Default facing direction

    void Update()
    {
        // Track last movement direction based on input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastDirection = new Vector2(moveX, moveY).normalized;
        }

        // Shoot on Spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fireballPrefab != null)
        {
            GameObject fireballObj = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            Fireball fireball = fireballObj.GetComponent<Fireball>();
            if (fireball != null)
            {
                // Pass direction or let it move forward
                fireball.Initialize(lastDirection);
            }
        }
        else
        {
            Debug.Log("Fireball prefab not assigned on Player!");
        }
    }
}