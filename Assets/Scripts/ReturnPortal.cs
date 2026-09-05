using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnPortal : MonoBehaviour
{
    private bool isNearPortal = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = true;
            Debug.Log("Press 'E' to return safely to town!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearPortal = false;
            Debug.Log("Left the return portal.");
        }
    }

    void Update()
    {
        if (isNearPortal && Input.GetKeyDown(KeyCode.E))
        {
            ReturnToTown();
        }
    }

    void ReturnToTown()
    {
        Debug.Log("Returning to town safe zone!");
        // Change "HometownScene" to whatever your main town/house scene name is
        SceneManager.LoadScene("HouseInteriorScene"); 
    }
}