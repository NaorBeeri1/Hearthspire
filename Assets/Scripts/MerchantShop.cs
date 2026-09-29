using UnityEngine;

public class MerchantShop : MonoBehaviour
{
    private bool isNearMerchant = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearMerchant = true;
            Debug.Log("Press 'E' to open/close the Soul Merchant Shop GUI!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearMerchant = false;
            // Automatically close shop UI if player walks away
            if (ShopUIController.Instance != null)
            {
                // Add a close check if needed
            }
            Debug.Log("Left the merchant.");
        }
    }

    void Update()
    {
        if (isNearMerchant && Input.GetKeyDown(KeyCode.E))
        {
            if (ShopUIController.Instance != null)
            {
                ShopUIController.Instance.ToggleShop();
            }
            else
            {
                // Fallback lookup in case it just spawned
                ShopUIController fallback = FindAnyObjectByType<ShopUIController>();
                if (fallback != null)
                {
                    fallback.ToggleShop();
                }
                else
                {
                    Debug.LogError("ShopUIController instance not found in scene!");
                }
            }
        }
    }
}