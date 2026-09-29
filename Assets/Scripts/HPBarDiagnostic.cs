using UnityEngine;
using UnityEngine.UI;

public class HPBarDiagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== RUNTIME HP BAR DIAGNOSTIC ===");

        // 1. Check Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[DIAGNOSTIC] NO CANVAS FOUND IN THE SCENE!");
        }
        else
        {
            Debug.Log($"[DIAGNOSTIC] Found Canvas: {canvas.name} (RenderMode: {canvas.renderMode})");
        }

        // 2. Check HP Bar Container
        GameObject container = GameObject.Find("PlayerHPBarContainer");
        if (container == null)
        {
            Debug.LogError("[DIAGNOSTIC] 'PlayerHPBarContainer' is missing from the scene!");
        }
        else
        {
            Debug.Log($"[DIAGNOSTIC] Found 'PlayerHPBarContainer'. ActiveSelf: {container.activeSelf}, Parent: {(container.transform.parent != null ? container.transform.parent.name : "None (Root)")}");
            
            // Check CanvasRenderer
            if (container.GetComponent<CanvasRenderer>() == null)
                Debug.LogError("[DIAGNOSTIC] 'PlayerHPBarContainer' is missing a CanvasRenderer component!");
        }

        // 3. Check Fill Bar & Sprite
        GameObject fill = GameObject.Find("PlayerHPBarFill");
        if (fill == null)
        {
            Debug.LogError("[DIAGNOSTIC] 'PlayerHPBarFill' object not found!");
        }
        else
        {
            Image fillImg = fill.GetComponent<Image>();
            if (fillImg == null)
                Debug.LogError("[DIAGNOSTIC] 'PlayerHPBarFill' is missing an Image component!");
            else
                Debug.Log($"[DIAGNOSTIC] Fill Image found. Sprite assigned: {(fillImg.sprite != null ? fillImg.sprite.name : "NONE (THIS IS WHY IT'S INVISIBLE OR BLANK)")}, Color: {fillImg.color}, FillAmount: {fillImg.fillAmount}");
        }

        // 4. Check Frame
        GameObject frame = GameObject.Find("HPBarFrame");
        if (frame == null)
        {
            Debug.LogWarning("[DIAGNOSTIC] 'HPBarFrame' object not found.");
        }
        else
        {
            Image frameImg = frame.GetComponent<Image>();
            Debug.Log($"[DIAGNOSTIC] Frame Image found. Sprite assigned: {(frameImg.sprite != null ? frameImg.sprite.name : "NONE")}");
        }

        Debug.Log("======================================");
    }
}