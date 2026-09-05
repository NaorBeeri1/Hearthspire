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
        Debug.Log("Entering Wilderness Raid Zone!");
        SceneManager.LoadScene("WildernessRaidScene");
    }
}