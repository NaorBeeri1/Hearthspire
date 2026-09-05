using UnityEngine;

public class MerchantShop : MonoBehaviour
{
    private bool isNearMerchant = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearMerchant = true;
            Debug.Log("Press 'E' to open the Soul Merchant Shop!");
            Debug.Log("Controls inside shop: Press '1' to Trade 5 Souls -> 10 Coins | Press '2' to Buy Skin for 20 Souls");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearMerchant = false;
            Debug.Log("Left the merchant.");
        }
    }

    void Update()
    {
        if (isNearMerchant)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenShopMenu();
            }

            // Trade Souls for Coins (Option 1)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TradeSoulsForCoins(5, 10);
            }

            // Buy Skin (Option 2)
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BuySkin(20);
            }
        }
    }

    void OpenShopMenu()
    {
        Debug.Log("=== SOUL MERCHANT SHOP OPEN ===");
        if (GameManager.Instance != null)
        {
            Debug.Log($"Current Balance -> Souls: {GameManager.Instance.totalSouls} | Coins: {GameManager.Instance.score}");
        }
        Debug.Log("Press [1] to trade 5 Souls for 10 Coins.");
        Debug.Log("Press [2] to buy a Special Wizard Skin for 20 Souls.");
    }

    void TradeSoulsForCoins(int soulsRequired, int coinsReward)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.totalSouls >= soulsRequired)
            {
                // Deduct souls and add coins via GameManager
                GameManager.Instance.totalSouls -= soulsRequired;
                GameManager.Instance.AddScore(coinsReward);
                Debug.Log($"Trade Successful! Exchanged {soulsRequired} Souls for {coinsReward} Coins.");
            }
            else
            {
                Debug.Log("Not enough souls! Go on a raid to collect more.");
            }
        }
    }

    void BuySkin(int soulsRequired)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.totalSouls >= soulsRequired)
            {
                GameManager.Instance.totalSouls -= soulsRequired;
                Debug.Log("Purchase Successful! You unlocked a Special Wizard Skin!");
                // Future expansion: change player sprite renderer color or sprite here!
            }
            else
            {
                Debug.Log("Not enough souls to buy the skin!");
            }
        }
    }
}