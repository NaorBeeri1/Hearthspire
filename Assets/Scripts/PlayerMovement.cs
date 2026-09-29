using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public SpriteRenderer backgroundSprite; // Drag your TownGroundBackground here in the Inspector

    private Rigidbody2D rb;
    private Vector2 movement;
    private float minX, maxX, minY, maxY;
    private float playerWidth = 0.5f;
    private float playerHeight = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateBounds();
    }

    void CalculateBounds()
    {
        if (backgroundSprite == null)
        {
            // Fallback: try to find it automatically if not assigned
            GameObject bgObj = GameObject.Find("TownGroundBackground");
            if (bgObj != null)
            {
                backgroundSprite = bgObj.GetComponent<SpriteRenderer>();
            }
        }

        if (backgroundSprite != null)
        {
            Bounds bgBounds = backgroundSprite.bounds;
            minX = bgBounds.min.x + playerWidth;
            maxX = bgBounds.max.x - playerWidth;
            minY = bgBounds.min.y + playerHeight;
            maxY = bgBounds.max.y - playerHeight;
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        if (backgroundSprite == null) CalculateBounds();

        Vector2 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        if (backgroundSprite != null)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        rb.MovePosition(newPosition);
    }
}