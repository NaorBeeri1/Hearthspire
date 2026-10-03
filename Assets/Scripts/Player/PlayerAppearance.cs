using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAppearance : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite wizardSprite; 

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ApplyAppearance();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyAppearance();
    }

    void Update()
    {
        ApplyAppearance();
    }

    public void ApplyAppearance()
    {
        if (spriteRenderer == null) 
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (GameManager.Instance != null && GameManager.Instance.hasPurchasedWizardSkin && GameManager.Instance.isWizardSkinEquipped)
        {
            if (wizardSprite != null && spriteRenderer.sprite != wizardSprite)
            {
                spriteRenderer.sprite = wizardSprite;
            }
        }
        else
        {
            if (defaultSprite != null && spriteRenderer.sprite != defaultSprite)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }
}