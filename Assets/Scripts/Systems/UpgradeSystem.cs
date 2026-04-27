using UnityEngine;
using System;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeData
    {
        public string id;
        public string name;
        public double baseCost;
        public float growthRate;
        public int currentLevel;
    }

    private Dictionary<string, UpgradeData> upgrades = new();

    public event Action<string, int> OnUpgradePurchased; // id, level

    public void Initialize()
    {
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        // MVP Upgrades as per GDD section 14.12
        upgrades["bigger_cone"] = new UpgradeData { id = "bigger_cone", name = "Bigger Cone", baseCost = 25, growthRate = 1.18f, currentLevel = 0 };
        upgrades["faster_scoop"] = new UpgradeData { id = "faster_scoop", name = "Faster Scoop", baseCost = 50, growthRate = 1.20f, currentLevel = 0 };
        upgrades["louder_bell"] = new UpgradeData { id = "louder_bell", name = "Louder Bell", baseCost = 75, growthRate = 1.20f, currentLevel = 0 };
        upgrades["wider_window"] = new UpgradeData { id = "wider_window", name = "Wider Window", baseCost = 100, growthRate = 1.22f, currentLevel = 0 };
        upgrades["combo_cup"] = new UpgradeData { id = "combo_cup", name = "Combo Cup", baseCost = 150, growthRate = 1.25f, currentLevel = 0 };
        upgrades["auto_bell"] = new UpgradeData { id = "auto_bell", name = "Auto Bell", baseCost = 500, growthRate = 1.0f, currentLevel = 0 };
        upgrades["turbo_tires"] = new UpgradeData { id = "turbo_tires", name = "Turbo Tires", baseCost = 0, growthRate = 1.0f, currentLevel = 0 }; // Internal use only
    }

    public bool TryPurchaseUpgrade(string upgradeId)
    {
        if (!upgrades.ContainsKey(upgradeId))
        {
            Debug.LogWarning($"Upgrade {upgradeId} not found");
            return false;
        }

        UpgradeData upgrade = upgrades[upgradeId];
        double cost = CalculateUpgradeCost(upgradeId, upgrade.currentLevel);

        if (cost == 0) // auto_bell has cost 0 until purchased once
        {
            if (upgrade.currentLevel > 0) return false; // Can only unlock once
            cost = 500;
        }

        if (!GameManager.Instance.Economy.TrySpendCash(cost))
        {
            return false;
        }

        upgrade.currentLevel++;
        OnUpgradePurchased?.Invoke(upgradeId, upgrade.currentLevel);

        GameManager.Instance.Analytics.TrackEvent("upgrade_purchase", new() {
            { "id", upgradeId },
            { "level", upgrade.currentLevel.ToString() },
            { "cost", cost.ToString("F0") }
        });

        // Notify relevant systems
        if (upgradeId == "bigger_cone" || upgradeId == "louder_bell" || upgradeId == "wider_window" || upgradeId == "turbo_tires")
        {
            GameManager.Instance.Truck.SetUpgradeLevel(upgradeId, upgrade.currentLevel);
        }

        return true;
    }

    public double CalculateUpgradeCost(string upgradeId, int currentLevel)
    {
        if (!upgrades.ContainsKey(upgradeId))
            return 0;

        UpgradeData upgrade = upgrades[upgradeId];

        if (upgrade.baseCost == 0) return 0; // Prestige or special upgrades

        double cost = upgrade.baseCost * System.Math.Pow(upgrade.growthRate, currentLevel);
        return System.Math.Round(cost);
    }

    public int GetUpgradeLevel(string upgradeId)
    {
        if (upgrades.ContainsKey(upgradeId))
            return upgrades[upgradeId].currentLevel;
        return 0;
    }

    public string GetUpgradeName(string upgradeId)
    {
        if (upgrades.ContainsKey(upgradeId))
            return upgrades[upgradeId].name;
        return "";
    }

    public List<UpgradeData> GetAllUpgrades() => new List<UpgradeData>(upgrades.Values);

    public double GetNextUpgradeCost(string upgradeId)
    {
        return CalculateUpgradeCost(upgradeId, GetUpgradeLevel(upgradeId));
    }

    public void SetUpgradeLevel(string upgradeId, int level)
    {
        if (upgrades.ContainsKey(upgradeId))
        {
            upgrades[upgradeId].currentLevel = level;
        }
    }

    public Dictionary<string, int> GetUpgradeLevels()
    {
        Dictionary<string, int> levels = new();
        foreach (var kvp in upgrades)
        {
            levels[kvp.Key] = kvp.Value.currentLevel;
        }
        return levels;
    }
}
