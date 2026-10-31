using UnityEngine;

public class AltarInteraction : MonoBehaviour
{
    private bool isNearAltar = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearAltar = true;
            Debug.Log("[ALTAR] Near Ancient Altar. Press 'E' to commune with the souls!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name == "Player")
        {
            isNearAltar = false;

            if (AltarUIController.Instance != null)
            {
                AltarUIController.Instance.CloseAltarUI();
            }

            Debug.Log("[ALTAR] Walked away from Ancient Altar.");
        }
    }

    void Update()
    {
        if (isNearAltar && Input.GetKeyDown(KeyCode.E))
        {
            if (AltarUIController.Instance != null)
            {
                AltarUIController.Instance.OpenAltarUI();
            }
            else
            {
                Debug.LogError("[ALTAR ERROR] AltarUIController instance not found in scene!");
            }
        }
    }
}