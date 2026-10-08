using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Chasing Settings")]
    public float detectionRange = 5f; // Distance at which enemy spots player
    public float chaseSpeed = 3.5f;   // Speed when hunting player

    private Vector2 startPos;
    private bool movingRight = true;
    private Rigidbody2D rb;
    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Keeps physics stable when colliding with player
        startPos = transform.position;

        // Find player reference
        GameObject pObj = GameObject.FindWithTag("Player");
        if (pObj == null) pObj = GameObject.Find("Player");
        if (pObj != null) playerTransform = pObj.transform;
    }

    void FixedUpdate()
    {
        if (playerTransform == null)
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj == null) pObj = GameObject.Find("Player");
            if (pObj != null) playerTransform = pObj.transform;
            
            PerformPatrol();
            return;
        }

        // Check distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // AGGRESSIVE CHASE MODE
            Vector2 targetPos = Vector2.MoveTowards(rb.position, playerTransform.position, chaseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPos);
        }
        else
        {
            // STANDARD PATROL MODE
            PerformPatrol();
        }
    }

    void PerformPatrol()
    {
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
        Vector2 patrolPos = rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(patrolPos);
    }
}