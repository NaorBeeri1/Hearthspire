using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class SceneBootstrap : MonoBehaviour
{
    public GameObject managersPrefab;
    public GameObject minimapCanvasPrefab;
    public GameObject radarManagerPrefab;
    public GameObject shopCanvasPrefab;
    public GameObject persistentUIPrefab;

    void Awake()
    {
        // 1. Ensure GameManager exists
        if (GameManager.Instance == null)
        {
            GameManager existingGM = FindAnyObjectByType<GameManager>();
            if (existingGM == null && managersPrefab != null)
            {
                Instantiate(managersPrefab);
            }
        }

        // 2. Ensure Persistent UI Canvas exists
        if (PersistentUI.Instance == null)
        {
            PersistentUI existingUI = FindAnyObjectByType<PersistentUI>();
            if (existingUI == null && persistentUIPrefab != null)
            {
                Instantiate(persistentUIPrefab);
            }
        }

        // 3. Ensure GameMenuController exists and persists across scenes
        if (GameMenuController.Instance == null)
        {
            GameMenuController existingMenu = FindAnyObjectByType<GameMenuController>();
            if (existingMenu == null)
            {
                GameObject menuObj = new GameObject("GameManagerUI_Controller", typeof(GameMenuController));
                DontDestroyOnLoad(menuObj);
            }
            else
            {
                if (existingMenu.transform.parent != null)
                {
                    existingMenu.transform.SetParent(null);
                }
                DontDestroyOnLoad(existingMenu.gameObject);
            }
        }

        // 4. Ensure exactly one valid EventSystem exists and is active
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
        
        if (EventSystem.current == null)
        {
            GameObject esObj = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            DontDestroyOnLoad(esObj);
        }

        // 5. Spawn Minimap and Shop if prefabs are assigned
        if (minimapCanvasPrefab != null && FindObjectsByType<MinimapRadar>(FindObjectsSortMode.None).Length == 0)
        {
            Instantiate(minimapCanvasPrefab);
            if (radarManagerPrefab != null) Instantiate(radarManagerPrefab);
        }

        if (shopCanvasPrefab != null && ShopUIController.Instance == null)
        {
            if (FindObjectsByType<ShopUIController>(FindObjectsSortMode.None).Length == 0)
            {
                Instantiate(shopCanvasPrefab);
            }
        }
    }
}