using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    
    private Vector2 startPos;
    private bool movingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        // Simple back and forth patrolling logic
        float displacement = transform.position.x - startPos.x;

        if (displacement > patrolDistance)
        {
            movingRight = false;
        }
        else if (displacement < -patrolDistance)
        {
            movingRight = true;
        }

        float direction = movingRight ? 1f : -1f;
        rb.MovePosition(rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0f));
    }
}