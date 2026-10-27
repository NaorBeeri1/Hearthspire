using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null)
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null) player = pObj.transform;
            return;
        }

        // Follow the player's X and Y position, keeping Z at -10
        Vector3 newPos = player.position;
        newPos.z = -10f;
        transform.position = newPos;
    }
}