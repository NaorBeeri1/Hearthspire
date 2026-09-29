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
            Debug.Log("Press 'E' to return safely to the hometown!");
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
        Debug.Log("Returning to HometownScene!");
        SceneManager.LoadScene("HometownScene"); 
    }
}