// ----------------------------------------
// FILE: Assets/Scripts/GameMenuController.cs
// ----------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;

    private bool isPaused = false;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        AutoFindPanels();
        HookButtonListeners();
    }

    void Update()
    {
        // Use unscaled check so escape works reliably when timeScale is 0
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOver)
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void AutoFindPanels()
    {
        if (pauseMenuUI == null)
        {
            GameObject p = GameObject.Find("PauseMenuPanel");
            if (p != null) pauseMenuUI = p;
        }

        if (gameOverUI == null)
        {
            GameObject g = GameObject.Find("GameOverPanel");
            if (g != null) gameOverUI = g;
        }

        HookButtonListeners();
    }

    void HookButtonListeners()
    {
        if (pauseMenuUI != null)
        {
            // Safely find and re-hook buttons in case the panel was reinstantiated
            Transform resumeBtnTransform = pauseMenuUI.transform.Find("ResumeButton");
            if (resumeBtnTransform != null)
            {
                Button btn = resumeBtnTransform.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveListener(ResumeGame);
                    btn.onClick.AddListener(ResumeGame);
                }
            }

            Transform menuBtnTransform = pauseMenuUI.transform.Find("MainMenuButton");
            if (menuBtnTransform != null)
            {
                Button btn = menuBtnTransform.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveListener(LoadMainMenu);
                    btn.onClick.AddListener(LoadMainMenu);
                }
            }
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("[GameMenuController] Game Resumed.");
    }

    public void PauseGame()
    {
        // If the reference is missing, search the whole scene (including hidden/inactive objects)
        if (pauseMenuUI == null)
        {
            // Find all GameObjects in loaded scenes, including inactive ones (Unity 6 feature)
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj.name == "PauseMenuPanel" && obj.scene.isLoaded)
                {
                    pauseMenuUI = obj;
                    break;
                }
            }
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("[GameMenuController] Game Paused.");
        }
        else
        {
            Debug.LogError("[GameMenuController] PauseMenuPanel is missing!");
        }
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}