using UnityEngine;

public class BlacksmithShop : MonoBehaviour
{
    public BlacksmithUIController blacksmithUI;
    private bool isNearBlacksmith = false;

    void Start()
    {
        FindController();
    }

    void FindController()
    {
        if (blacksmithUI == null)
        {
            if (BlacksmithUIController.Instance != null)
            {
                blacksmithUI = BlacksmithUIController.Instance;
            }
            else
            {
                // Finds the controller even if the canvas was loaded inactive
                blacksmithUI = FindAnyObjectByType<BlacksmithUIController>(FindObjectsInactive.Include);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearBlacksmith = true;
            Debug.Log("Press 'E' to open the Blacksmith Forge!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearBlacksmith = false;

            if (blacksmithUI != null && blacksmithUI.isShopOpen)
            {
                blacksmithUI.ToggleShop();
            }

            Debug.Log("Left the blacksmith.");
        }
    }

    void Update()
    {
        if (isNearBlacksmith && Input.GetKeyDown(KeyCode.E))
        {
            if (blacksmithUI == null)
            {
                FindController();
            }

            if (blacksmithUI != null)
            {
                blacksmithUI.ToggleShop();
            }
            else
            {
                Debug.LogError("BlacksmithUIController instance not found in scene! Make sure BlacksmithCanvas exists.");
            }
        }
    }
}