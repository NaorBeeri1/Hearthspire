using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    public int soulValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddSouls(soulValue);
            }
            Destroy(gameObject);
        }
    }
}