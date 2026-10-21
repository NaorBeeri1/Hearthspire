using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    public int soulValue = 1;

    public void Initialize(int value)
    {
        soulValue = Mathf.Max(1, value);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddSouls(soulValue);
            }
            Debug.Log($"[SOUL COLLECTED] +{soulValue} Souls!");
            Destroy(gameObject);
        }
    }
}