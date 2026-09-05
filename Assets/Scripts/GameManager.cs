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
    public TextMeshProUGUI soulsText; // Optional: drag UI text here later if you want

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps it alive across scenes!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
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