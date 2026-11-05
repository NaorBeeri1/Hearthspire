using UnityEngine;
using System.Collections.Generic;

public enum RiftAffixType
{
    None,
    GlassCannon, // Deals +50% Damage, Max HP Halved
    SoulStorm,   // Enemies move 20% faster, drop 2x souls
    DarknessFalls, // Dark vision radius, 1.5x coin rewards
    EnragedHost,  // Enemies hit 25% harder, but extra 1.3x coins and souls
    Steelclad    // Enemies have double HP, but drop 2x Coins
}

[System.Serializable]
public class RiftAffix
{
    public RiftAffixType type;
    public string affixName;
    public string description;
    public float coinMultiplier = 1.0f;
    public float soulMultiplier = 1.0f;
    public Color displayColor = Color.white;

    public RiftAffix(RiftAffixType type, string affixName, string description, float coins, float souls, Color color)
    {
        this.type = type;
        this.affixName = affixName;
        this.description = description;
        this.coinMultiplier = coins;
        this.soulMultiplier = souls;
        this.displayColor = color;
    }
}

public class RiftManager : MonoBehaviour
{
    private static RiftManager _instance;
    public static RiftManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<RiftManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("RiftManager");
                    _instance = obj.AddComponent<RiftManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    [Header("Rift Status")]
    public bool isRiftActive = false;
    public List<RiftAffix> activeAffixes = new List<RiftAffix>();

    private List<RiftAffix> allPossibleAffixes = new List<RiftAffix>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeAffixes();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAffixes()
    {
        allPossibleAffixes.Clear();
        allPossibleAffixes.Add(new RiftAffix(RiftAffixType.GlassCannon, "Glass Cannon", "+50% Damage dealt, but Max Health is halved!", 1.3f, 1.3f, new Color(1f, 0.4f, 0.4f)));
        allPossibleAffixes.Add(new RiftAffix(RiftAffixType.SoulStorm, "Soul Storm", "Enemies move +20% faster, but drop double Souls!", 1.0f, 2.0f, new Color(0.4f, 0.8f, 1f)));
        allPossibleAffixes.Add(new RiftAffix(RiftAffixType.DarknessFalls, "Darkness Falls", "Substantially reduced vision range, but +50% Coins!", 1.5f, 1.2f, new Color(0.7f, 0.4f, 1f)));
        allPossibleAffixes.Add(new RiftAffix(RiftAffixType.EnragedHost, "Enraged Host", "Enemies deal +25% damage, but rewards are boosted 1.3x!", 1.3f, 1.3f, new Color(1f, 0.6f, 0.2f)));
        allPossibleAffixes.Add(new RiftAffix(RiftAffixType.Steelclad, "Steelclad", "Enemies have double HP, but drop 2x Coins!", 2.0f, 1.0f, new Color(0.7f, 0.7f, 0.8f)));
    }

    public void GenerateNewRift()
    {
        if (allPossibleAffixes == null || allPossibleAffixes.Count == 0)
        {
            InitializeAffixes();
        }

        isRiftActive = true;
        activeAffixes.Clear();

        int numAffixes = Random.Range(1, 3);
        List<RiftAffix> pool = new List<RiftAffix>(allPossibleAffixes);

        for (int i = 0; i < numAffixes; i++)
        {
            if (pool.Count == 0) break;
            int idx = Random.Range(0, pool.Count);
            activeAffixes.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        Debug.Log($"<color=#FF4D4D>[RIFT GENERATED] Active Affixes Count: {activeAffixes.Count}</color>");
        foreach (var affix in activeAffixes)
        {
            Debug.Log($"<color=orange> -> Affix: {affix.affixName} | {affix.description}</color>");
        }
    }

    public void ClearRift()
    {
        isRiftActive = false;
        activeAffixes.Clear();
        Debug.Log("[RIFT] Cleared active mutators.");
    }

    public float GetCoinMultiplier()
    {
        if (!isRiftActive) return 1.0f;
        float mult = 1.0f;
        foreach (var affix in activeAffixes) mult *= affix.coinMultiplier;
        return mult;
    }

    public float GetSoulMultiplier()
    {
        if (!isRiftActive) return 1.0f;
        float mult = 1.0f;
        foreach (var affix in activeAffixes) mult *= affix.soulMultiplier;
        return mult;
    }

    public bool HasAffix(RiftAffixType type)
    {
        if (!isRiftActive) return false;
        foreach (var affix in activeAffixes)
        {
            if (affix.type == type) return true;
        }
        return false;
    }
}