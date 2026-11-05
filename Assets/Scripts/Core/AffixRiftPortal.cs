using UnityEngine;
using UnityEngine.SceneManagement;

public class AffixRiftPortal : MonoBehaviour
{
    [Header("UI Interactions")]
    public GameObject promptIndicator; // "Press E to enter Affix Rift"
    
    private bool isPlayerInRange = false;

    void Start()
    {
        if (promptIndicator != null)
        {
            promptIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EnterAffixRift();
        }
    }

    void EnterAffixRift()
    {
        // Randomize the active Rifts & Mutators before loading
        if (RiftManager.Instance != null)
        {
            RiftManager.Instance.GenerateNewRift();
        }

        Debug.Log("<color=#FF4D4D>[AFFIX RIFT ENTERED] Volatile raid initiated with active mutators!</color>");
        
        // Hide UI and load the wild scene
        if (promptIndicator != null) promptIndicator.SetActive(false);
        
        // Load target raid scene (e.g., WildernessRaidScene)
        SceneManager.LoadScene("WildernessRaidScene");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Player"))
        {
            isPlayerInRange = true;
            if (promptIndicator != null)
            {
                promptIndicator.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Player"))
        {
            isPlayerInRange = false;
            if (promptIndicator != null)
            {
                promptIndicator.SetActive(false);
            }
        }
    }
}