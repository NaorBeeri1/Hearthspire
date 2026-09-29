using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Coins / Score")]
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Souls (Raid Currency)")]
    public int totalSouls = 0;
    public TextMeshProUGUI soulsText;

    void Awake()
    {
        // Prevent duplicates across scene transitions
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If a GameManager already exists, destroy the newly loaded duplicate 
            // BUT ensure we don't lose data if needed, or simply let the original persist.
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindTextReferences();
        UpdateScoreText();
        UpdateSoulsText();
    }

    void Update()
    {
        if (scoreText == null || soulsText == null)
        {
            FindTextReferences();
        }
    }

    void FindTextReferences()
    {
        if (scoreText == null)
        {
            GameObject coinObj = GameObject.Find("CoinsText");
            if (coinObj != null) scoreText = coinObj.GetComponent<TextMeshProUGUI>();
        }

        if (soulsText == null)
        {
            GameObject soulObj = GameObject.Find("SoulsText");
            if (soulObj != null) soulsText = soulObj.GetComponent<TextMeshProUGUI>();
        }
        
        UpdateScoreText();
        UpdateSoulsText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void AddSouls(int amount)
    {
        totalSouls += amount;
        UpdateSoulsText();
        Debug.Log("Total Souls Collected: " + totalSouls);
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Coins: " + score;
        }
    }

    void UpdateSoulsText()
    {
        if (soulsText != null)
        {
            soulsText.text = "Souls: " + totalSouls;
        }
    }
}