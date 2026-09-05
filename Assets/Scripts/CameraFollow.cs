using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Background Reference")]
    public SpriteRenderer backgroundSprite; // Drag your HouseInterior_Layout here

    private float minX, maxX, minY, maxY;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        CalculateBounds();
    }

    void CalculateBounds()
    {
        if (backgroundSprite == null) return;

        // Calculate camera dimensions in world units
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Get background bounds
        Bounds bgBounds = backgroundSprite.bounds;

        // Set camera movement limits so the camera view never goes past the background edges
        minX = bgBounds.min.x + horzExtent;
        maxX = bgBounds.max.x - horzExtent;
        minY = bgBounds.min.y + vertExtent;
        maxY = bgBounds.max.y - vertExtent;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // If background is assigned, clamp to its edges; otherwise follow freely
        if (backgroundSprite != null)
        {
            float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);
            desiredPosition = new Vector3(clampedX, clampedY, desiredPosition.z);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}