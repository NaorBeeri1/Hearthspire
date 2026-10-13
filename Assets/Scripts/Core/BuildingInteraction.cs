using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingInteraction : MonoBehaviour
{
    private bool isNearDoor = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearDoor = true;
            Debug.Log("Press 'E' to enter the house!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearDoor = false;
            Debug.Log("Left the door.");
        }
    }

    void Update()
    {
        if (isNearDoor && Input.GetKeyDown(KeyCode.E))
        {
            EnterBuilding();
        }
    }

    void EnterBuilding()
    {
        Debug.Log("Entering the house interior!");
        SceneManager.LoadScene("HouseInteriorScene");
    }
}