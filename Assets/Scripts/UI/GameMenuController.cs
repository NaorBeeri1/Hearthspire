using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject victoryUI;

    [Header("Save Button Feedback")]
    public Button saveGameButton;
    private TextMeshProUGUI saveButtonText;
    private float resetSaveButtonTime = -1f;

    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isVictory = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Start()
    {
        CleanupDuplicateEventSystems();
        AutoFindPanels();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isVictory = false;

        CleanupDuplicateEventSystems();

        if (scene.name == "MainMenuScene")
        {
            if (PersistentUI.Instance != null) PersistentUI.Instance.gameObject.SetActive(false);
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            if (gameOverUI != null) gameOverUI.SetActive(false);
            if (victoryUI != null) victoryUI.SetActive(false);

            HookMainMenuButtons();
            return;
        }
        else
        {
            // Ensure PersistentUI canvas and its parent game object are active in gameplay scenes
            if (PersistentUI.Instance != null)
            {
                PersistentUI.Instance.gameObject.SetActive(true);
                PersistentUI.Instance.enabled = true;
            }
        }

        AutoFindPanels();
    }

    void CleanupDuplicateEventSystems()
    {
        EventSystem[] allEventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (allEventSystems.Length > 1)
        {
            for (int i = 1; i < allEventSystems.Length; i++)
            {
                Destroy(allEventSystems[i].gameObject);
            }
        }
        else if (allEventSystems.Length == 0)
        {
            GameObject esObj = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            DontDestroyOnLoad(esObj);
        }
    }

    void HookMainMenuButtons()
    {
        GameObject startBtnObj = GameObject.Find("StartButton") ?? GameObject.Find("Start Game");
        if (startBtnObj != null)
        {
            Button btn = startBtnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(StartGame);
                btn.onClick.AddListener(StartGame);
            }
        }

        GameObject resetBtnObj = GameObject.Find("ResetProgressButton") ?? GameObject.Find("Reset Progress");
        if (resetBtnObj != null)
        {
            Button btn = resetBtnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(OnResetProgressClicked);
                btn.onClick.AddListener(OnResetProgressClicked);
            }
        }

        GameObject quitBtnObj = GameObject.Find("QuitButton") ?? GameObject.Find("Quit Game");
        if (quitBtnObj != null)
        {
            Button btn = quitBtnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(QuitGame);
                btn.onClick.AddListener(QuitGame);
            }
        }
    }

    public void OnResetProgressClicked()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetAllGameProgress();
        }

        GameObject resetBtnObj = GameObject.Find("ResetProgressButton") ?? GameObject.Find("Reset Progress");
        if (resetBtnObj != null)
        {
            TextMeshProUGUI txt = resetBtnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                txt.text = "Progress Reset!";
                txt.color = new Color(1f, 0.3f, 0.3f, 1f);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ShopUIController.Instance != null && (ShopUIController.Instance.isShopOpen || ShopUIController.Instance.lastClosedFrame == Time.frameCount))
                return;

            if (BlacksmithUIController.Instance != null && (BlacksmithUIController.Instance.isShopOpen || BlacksmithUIController.Instance.lastClosedFrame == Time.frameCount))
                return;

            if (!isGameOver && !isVictory)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }

        if (resetSaveButtonTime > 0f && Time.unscaledTime >= resetSaveButtonTime)
        {
            if (saveButtonText != null)
            {
                saveButtonText.text = "Save Game";
                saveButtonText.color = new Color(1f, 0.85f, 0.3f, 1f);
            }
            resetSaveButtonTime = -1f;
        }
    }

    public void AutoFindPanels()
    {
        pauseMenuUI = null;
        gameOverUI = null;
        victoryUI = null;

        if (PersistentUI.Instance != null)
        {
            PersistentUI.Instance.gameObject.SetActive(true);

            Transform pTr = PersistentUI.Instance.transform.Find("PauseMenuPanel");
            if (pTr != null) pauseMenuUI = pTr.gameObject;

            Transform gTr = PersistentUI.Instance.transform.Find("GameOverPanel");
            if (gTr != null) gameOverUI = gTr.gameObject;

            Transform vTr = PersistentUI.Instance.transform.Find("VictoryPanel");
            if (vTr != null) victoryUI = vTr.gameObject;
        }

        if (pauseMenuUI == null || gameOverUI == null || victoryUI == null)
        {
            Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas c in allCanvases)
            {
                if (c.gameObject.name.Contains("PersistentUI"))
                {
                    c.gameObject.SetActive(true);
                    if (pauseMenuUI == null)
                    {
                        Transform pTr = c.transform.Find("PauseMenuPanel");
                        if (pTr != null) pauseMenuUI = pTr.gameObject;
                    }
                    if (gameOverUI == null)
                    {
                        Transform gTr = c.transform.Find("GameOverPanel");
                        if (gTr != null) gameOverUI = gTr.gameObject;
                    }
                    if (victoryUI == null)
                    {
                        Transform vTr = c.transform.Find("VictoryPanel");
                        if (vTr != null) victoryUI = vTr.gameObject;
                    }
                }
            }
        }

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (victoryUI != null) victoryUI.SetActive(false);

        HookButtonListeners();
    }

    void HookButtonListeners()
    {
        if (pauseMenuUI != null)
        {
            Button[] pauseButtons = pauseMenuUI.GetComponentsInChildren<Button>(true);
            foreach (Button btn in pauseButtons)
            {
                string btnName = btn.gameObject.name.ToLower();

                if (btnName.Contains("resume"))
                {
                    btn.onClick.RemoveListener(ResumeGame);
                    btn.onClick.AddListener(ResumeGame);
                }
                else if (btnName.Contains("save") || btnName.Contains("savegame"))
                {
                    saveGameButton = btn;
                    saveButtonText = btn.GetComponentInChildren<TextMeshProUGUI>();
                    btn.onClick.RemoveListener(OnSaveClicked);
                    btn.onClick.AddListener(OnSaveClicked);
                }
                else if (btnName.Contains("mainmenu") || btnName.Contains("menu"))
                {
                    btn.onClick.RemoveListener(LoadMainMenu);
                    btn.onClick.AddListener(LoadMainMenu);
                }
            }
        }

        if (gameOverUI != null)
        {
            Button[] gameOverButtons = gameOverUI.GetComponentsInChildren<Button>(true);
            foreach (Button btn in gameOverButtons)
            {
                string btnName = btn.gameObject.name.ToLower();

                if (btnName.Contains("tryagain") || btnName.Contains("playagain"))
                {
                    btn.onClick.RemoveListener(RestartLevel);
                    btn.onClick.AddListener(RestartLevel);
                }
                else if (btnName.Contains("mainmenu") || btnName.Contains("menu"))
                {
                    btn.onClick.RemoveListener(LoadMainMenu);
                    btn.onClick.AddListener(LoadMainMenu);
                }
            }
        }

        if (victoryUI != null)
        {
            Button[] victoryButtons = victoryUI.GetComponentsInChildren<Button>(true);
            foreach (Button btn in victoryButtons)
            {
                string btnName = btn.gameObject.name.ToLower();

                if (btnName.Contains("playagain") || btnName.Contains("tryagain"))
                {
                    btn.onClick.RemoveListener(RestartLevel);
                    btn.onClick.AddListener(RestartLevel);
                }
                else if (btnName.Contains("mainmenu") || btnName.Contains("menu"))
                {
                    btn.onClick.RemoveListener(LoadMainMenu);
                    btn.onClick.AddListener(LoadMainMenu);
                }
            }
        }
    }

    void OnSaveClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();

            if (saveButtonText != null)
            {
                saveButtonText.text = "Saved!";
                saveButtonText.color = new Color(0.2f, 1f, 0.4f, 1f);
                resetSaveButtonTime = Time.unscaledTime + 1.5f;
            }
        }
    }

    public void TriggerVictory()
    {
        isVictory = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (victoryUI == null) AutoFindPanels();

        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("HometownScene");
        }
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        if (gameOverUI == null) AutoFindPanels();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("HometownScene");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuUI == null) AutoFindPanels();

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            
            // Force Canvas to override sorting and sit on top when paused
            Canvas canvas = pauseMenuUI.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 999;
            }

            pauseMenuUI.transform.SetAsLastSibling();
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("[GameMenuController] Game Paused. TimeScale set to 0.");
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isVictory = false;

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (victoryUI != null) victoryUI.SetActive(false);

        SceneManager.LoadScene("HometownScene");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isVictory = false;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (victoryUI != null) victoryUI.SetActive(false);

        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        isVictory = false;
        SceneManager.LoadScene("HometownScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}