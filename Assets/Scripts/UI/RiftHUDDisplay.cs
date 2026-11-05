using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;

public class RiftHUDDisplay : MonoBehaviour
{
    [Header("Corner HUD Reference")]
    public GameObject riftBannerRoot;
    public TextMeshProUGUI mutatorsText;

    [Header("Center Intro Announcement")]
    public GameObject introAnnouncementPanel;
    public TextMeshProUGUI introTitleText;
    public TextMeshProUGUI introDetailsText;
    public CanvasGroup introCanvasGroup;

    [Header("Lighting References")]
    public Light2D globalLight;

    void Start()
    {
        UpdateRiftVisuals();
    }

    public void UpdateRiftVisuals()
    {
        if (globalLight == null)
        {
            globalLight = FindAnyObjectByType<Light2D>();
        }

        // If no active rift, restore full lighting and hide UI
        if (RiftManager.Instance == null || !RiftManager.Instance.isRiftActive || RiftManager.Instance.activeAffixes.Count == 0)
        {
            if (riftBannerRoot != null) riftBannerRoot.SetActive(false);
            if (introAnnouncementPanel != null) introAnnouncementPanel.SetActive(false);
            ResetGlobalLighting();
            return;
        }

        float coinMult = RiftManager.Instance.GetCoinMultiplier();
        float soulMult = RiftManager.Instance.GetSoulMultiplier();

        // 1. Compact Corner HUD
        if (riftBannerRoot != null)
        {
            riftBannerRoot.SetActive(true);
            if (mutatorsText != null)
            {
                string hudSummary = "<color=#FF4D4D><b>VOLATILE RIFT</b></color>\n";
                foreach (var affix in RiftManager.Instance.activeAffixes)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(affix.displayColor);
                    hudSummary += $"<color=#{hexColor}>• {affix.affixName}</color>\n";
                }
                hudSummary += $"<color=#FFD700>{coinMult:F1}x Coins | {soulMult:F1}x Souls</color>";
                mutatorsText.text = hudSummary.TrimEnd();
            }
        }

        // 2. Darkness Falls Lighting Check
        if (RiftManager.Instance.HasAffix(RiftAffixType.DarknessFalls))
        {
            if (globalLight != null)
            {
                globalLight.intensity = 0.28f; // Dim ambient light
                globalLight.color = new Color(0.35f, 0.25f, 0.5f, 1f); // Deep atmospheric twilight
            }
        }
        else
        {
            ResetGlobalLighting();
        }

        // 3. Center Intro Splash Announcement
        if (introAnnouncementPanel != null)
        {
            if (introTitleText != null)
            {
                introTitleText.text = "VOLATILE RIFT MUTATORS";
            }

            if (introDetailsText != null)
            {
                string details = "";
                foreach (var affix in RiftManager.Instance.activeAffixes)
                {
                    string hexColor = ColorUtility.ToHtmlStringRGB(affix.displayColor);
                    details += $"<color=#{hexColor}><b>[{affix.affixName.ToUpper()}]</b></color>\n{affix.description}\n\n";
                }
                details += $"<color=#FFD700><b>Loot Multipliers:</b> {coinMult:F1}x Coins  •  {soulMult:F1}x Souls</color>";
                introDetailsText.text = details.TrimEnd();
            }

            StopAllCoroutines();
            StartCoroutine(ShowIntroSplashRoutine());
        }
    }

    void ResetGlobalLighting()
    {
        if (globalLight != null)
        {
            globalLight.intensity = 1.0f;
            globalLight.color = Color.white;
        }
    }

    IEnumerator ShowIntroSplashRoutine()
    {
        introAnnouncementPanel.SetActive(true);

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < 0.3f)
            {
                elapsed += Time.unscaledDeltaTime;
                introCanvasGroup.alpha = Mathf.Clamp01(elapsed / 0.3f);
                yield return null;
            }
            introCanvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(2.8f);

            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.unscaledDeltaTime;
                introCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / 0.5f);
                yield return null;
            }
            introCanvasGroup.alpha = 0f;
        }
        else
        {
            yield return new WaitForSecondsRealtime(3.0f);
        }

        introAnnouncementPanel.SetActive(false);
    }
}