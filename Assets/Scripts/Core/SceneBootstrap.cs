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

        // 4. Ensure exactly one valid EventSystem exists, is active, and persists
        if (EventSystem.current == null)
        {
            EventSystem existing = FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
            if (existing != null)
            {
                existing.gameObject.SetActive(true);
                existing.enabled = true;
                DontDestroyOnLoad(existing.gameObject);
                EventSystem.current = existing;
            }
            else
            {
                GameObject esObj = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                DontDestroyOnLoad(esObj);
            }
        }
        else
        {
            EventSystem.current.gameObject.SetActive(true);
            EventSystem.current.enabled = true;
            DontDestroyOnLoad(EventSystem.current.gameObject);
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