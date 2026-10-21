using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
            }
            Debug.Log("Coin collected!");
            Destroy(gameObject);
        }
    }
}