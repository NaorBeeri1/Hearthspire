using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitBuilding : MonoBehaviour
{
    private bool isNearExit = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearExit = true;
            Debug.Log("Press 'E' to exit back to the town!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearExit = false;
            Debug.Log("Left the exit.");
        }
    }

    void Update()
    {
        if (isNearExit && Input.GetKeyDown(KeyCode.E))
        {
            ExitHouse();
        }
    }

    void ExitHouse()
    {
        Debug.Log("Returning to HometownScene!");
        SceneManager.LoadScene("HometownScene");
    }
}