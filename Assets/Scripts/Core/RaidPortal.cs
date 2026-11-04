using UnityEngine;
using UnityEngine.SceneManagement;

public class RaidPortal : MonoBehaviour
{
    private bool isNearPortal = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = true;
            Debug.Log("Press 'E' to start the Wilderness Raid!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = false;
            Debug.Log("Left the raid portal.");
        }
    }

    void Update()
    {
        if (isNearPortal && Input.GetKeyDown(KeyCode.E))
        {
            StartRaid();
        }
    }

    void StartRaid()
    {
        // איפוס מוטטורים כדי להבטיח ריצה רגילה לחלוטין
        if (RiftManager.Instance != null)
        {
            RiftManager.Instance.ClearRift();
        }

        Debug.Log("Entering Standard Wilderness Raid Zone!");
        SceneManager.LoadScene("WildernessRaidScene");
    }
}