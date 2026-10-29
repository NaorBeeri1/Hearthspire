using UnityEngine;

public static class PermanentUpgrades
{
    // Threshold where minor diminishing returns begin
    public const int CoreLevelCap = 5;

    // --- 1. GREED ---
    // Levels 1-5: +5% per level (up to +25%)
    // Level 6+: +1% per level infinitely
    public static int GreedLevel { get; private set; } = 0;
    public static int GreedBaseCost = 50;
    public static int GreedCostIncrement = 50;
    public static int GreedParagonCostMultiplier = 80;

    // --- 2. STARTING WEALTH ---
    // Levels 1-5: +10 coins per level (up to 50 coins)
    // Level 6+: +2 coins per level infinitely
    public static int StartingWealthLevel { get; private set; } = 0;
    public static int WealthBaseCost = 40;
    public static int WealthCostIncrement = 40;
    public static int WealthParagonCostMultiplier = 70;

    // --- 3. SECOND WIND ---
    // Binary unlock: 0 or 1 (Revive once per run at 25% HP)
    public static int SecondWindLevel { get; private set; } = 0;
    public const int MaxSecondWindLevel = 1;
    public static int SecondWindCost = 150;

    // --- 4. FIRE MASTERY (CRIT) ---
    // Levels 1-5: +5% crit per level (up to 25%)
    // Level 6+: +0.5% crit per level infinitely (soft capped at 75%)
    public static int MasteryLevel { get; private set; } = 0;
    public static int MasteryBaseCost = 60;
    public static int MasteryCostIncrement = 60;
    public static int MasteryParagonCostMultiplier = 100;

    static PermanentUpgrades()
    {
        LoadUpgrades();
    }

    public static void LoadUpgrades()
    {
        GreedLevel = PlayerPrefs.GetInt("Upgrade_Greed", 0);
        StartingWealthLevel = PlayerPrefs.GetInt("Upgrade_StartingWealth", 0);
        SecondWindLevel = PlayerPrefs.GetInt("Upgrade_SecondWind", 0);
        MasteryLevel = PlayerPrefs.GetInt("Upgrade_Mastery", 0);
    }

    public static void SaveUpgrades()
    {
        PlayerPrefs.SetInt("Upgrade_Greed", GreedLevel);
        PlayerPrefs.SetInt("Upgrade_StartingWealth", StartingWealthLevel);
        PlayerPrefs.SetInt("Upgrade_SecondWind", SecondWindLevel);
        PlayerPrefs.SetInt("Upgrade_Mastery", MasteryLevel);
        PlayerPrefs.Save();
    }

    // --- DYNAMIC COST CALCULATIONS ---
    public static int GetGreedCost()
    {
        if (GreedLevel < CoreLevelCap)
            return GreedBaseCost + (GreedLevel * GreedCostIncrement);
        
        int extra = GreedLevel - CoreLevelCap;
        return (GreedBaseCost + (CoreLevelCap * GreedCostIncrement)) + (extra * GreedParagonCostMultiplier);
    }

    public static int GetWealthCost()
    {
        if (StartingWealthLevel < CoreLevelCap)
            return WealthBaseCost + (StartingWealthLevel * WealthCostIncrement);

        int extra = StartingWealthLevel - CoreLevelCap;
        return (WealthBaseCost + (CoreLevelCap * WealthCostIncrement)) + (extra * WealthParagonCostMultiplier);
    }

    public static int GetSecondWindCost() => SecondWindCost;

    public static int GetMasteryCost()
    {
        if (MasteryLevel < CoreLevelCap)
            return MasteryBaseCost + (MasteryLevel * MasteryCostIncrement);

        int extra = MasteryLevel - CoreLevelCap;
        return (MasteryBaseCost + (CoreLevelCap * MasteryCostIncrement)) + (extra * MasteryParagonCostMultiplier);
    }

    // --- STAT EFFECT CALCULATIONS ---
    public static float GetDropRateMultiplier()
    {
        if (GreedLevel <= CoreLevelCap)
            return 1f + (GreedLevel * 0.05f);

        int extra = GreedLevel - CoreLevelCap;
        return 1f + 0.25f + (extra * 0.01f); // +25% + 1% per extra rank
    }

    public static int GetStartingCoinsBonus()
    {
        if (StartingWealthLevel <= CoreLevelCap)
            return StartingWealthLevel * 10;

        int extra = StartingWealthLevel - CoreLevelCap;
        return 50 + (extra * 2); // 50 coins + 2 per extra rank
    }

    public static bool HasSecondWind() => SecondWindLevel > 0;

    public static float GetCritChanceBonus()
    {
        if (MasteryLevel <= CoreLevelCap)
            return MasteryLevel * 0.05f;

        int extra = MasteryLevel - CoreLevelCap;
        float total = 0.25f + (extra * 0.005f); // 25% + 0.5% per extra rank
        return Mathf.Min(0.75f, total); // Cap at 75% max crit
    }

    // --- PURCHASE ACTIONS ---
    public static bool BuyGreedRank(ref int currentSouls)
    {
        int cost = GetGreedCost();
        if (currentSouls >= cost)
        {
            currentSouls -= cost;
            GreedLevel++;
            SaveUpgrades();
            return true;
        }
        return false;
    }

    public static bool BuyWealthRank(ref int currentSouls)
    {
        int cost = GetWealthCost();
        if (currentSouls >= cost)
        {
            currentSouls -= cost;
            StartingWealthLevel++;
            SaveUpgrades();
            return true;
        }
        return false;
    }

    public static bool BuySecondWindRank(ref int currentSouls)
    {
        if (SecondWindLevel >= MaxSecondWindLevel) return false;
        int cost = GetSecondWindCost();
        if (currentSouls >= cost)
        {
            currentSouls -= cost;
            SecondWindLevel++;
            SaveUpgrades();
            return true;
        }
        return false;
    }

    public static bool BuyMasteryRank(ref int currentSouls)
    {
        int cost = GetMasteryCost();
        if (currentSouls >= cost)
        {
            currentSouls -= cost;
            MasteryLevel++;
            SaveUpgrades();
            return true;
        }
        return false;
    }

    public static void ResetAll()
    {
        GreedLevel = 0;
        StartingWealthLevel = 0;
        SecondWindLevel = 0;
        MasteryLevel = 0;
        SaveUpgrades();
    }
}