using UnityEngine;
using System.Collections.Generic;
using System;

namespace PotatoFarm.Core
{
    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public string description;
        public double baseCost;
        public double costMultiplier = 1.5;
        public int level;
        public int maxLevel = -1; // -1 means no limit
        public UpgradeType type;
        public double effectValue;
        public bool isUnlocked = true;
        
        public double GetCurrentCost()
        {
            return baseCost * Math.Pow(costMultiplier, level);
        }
        
        public double GetCurrentEffect()
        {
            return effectValue * (level + 1); // Base effect plus level multiplier
        }
        
        public bool isMaxed
        {
            get { return maxLevel != -1 && level >= maxLevel; }
        }
        
        public bool CanUpgrade()
        {
            return isUnlocked && (maxLevel == -1 || level < maxLevel);
        }
    }
    
    public enum UpgradeType
    {
        TapPower,
        ClickMultiplier,
        AutomationSpeed,
        ProcessingSpeed,
        SellPrice,
        ProductionMultiplier,
        OfflineEarnings
    }
    
    public class UpgradeManager : MonoBehaviour
    {
        [Header("Upgrades")]
        public List<Upgrade> upgrades = new List<Upgrade>();
        
        public event Action<int> OnUpgradePurchased;
        
        private void Start()
        {
            InitializeUpgrades();
        }
        
        private void InitializeUpgrades()
        {
            if (upgrades.Count == 0)
            {
                upgrades.Add(new Upgrade
                {
                    name = "Stronger Hands",
                    description = "Increases tap power by 100%",
                    baseCost = 50,
                    costMultiplier = 2.0,
                    type = UpgradeType.TapPower,
                    effectValue = 1.0
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Golden Gloves",
                    description = "Increases click multiplier by 50%",
                    baseCost = 200,
                    costMultiplier = 2.5,
                    type = UpgradeType.ClickMultiplier,
                    effectValue = 0.5
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Auto Harvester",
                    description = "Unlocks automation for farms",
                    baseCost = 500,
                    type = UpgradeType.AutomationSpeed,
                    effectValue = 1.0,
                    maxLevel = 1
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Processing Efficiency",
                    description = "Increases processing speed by 25%",
                    baseCost = 1000,
                    costMultiplier = 3.0,
                    type = UpgradeType.ProcessingSpeed,
                    effectValue = 0.25
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Market Connections",
                    description = "Increases sell price by 20%",
                    baseCost = 2000,
                    costMultiplier = 2.8,
                    type = UpgradeType.SellPrice,
                    effectValue = 0.2
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Super Fertilizer",
                    description = "Increases all production by 50%",
                    baseCost = 5000,
                    costMultiplier = 4.0,
                    type = UpgradeType.ProductionMultiplier,
                    effectValue = 0.5
                });
                
                upgrades.Add(new Upgrade
                {
                    name = "Offline Manager",
                    description = "Increases offline earnings by 25%",
                    baseCost = 10000,
                    costMultiplier = 3.5,
                    type = UpgradeType.OfflineEarnings,
                    effectValue = 0.25
                });
            }
        }
        
        public bool PurchaseUpgrade(int upgradeIndex)
        {
            if (upgradeIndex < 0 || upgradeIndex >= upgrades.Count) return false;
            
            Upgrade upgrade = upgrades[upgradeIndex];
            if (!upgrade.CanUpgrade()) return false;
            
            double cost = upgrade.GetCurrentCost();
            
            if (GameManager.Instance.resourceManager.SpendResource(ResourceType.Cash, cost))
            {
                upgrade.level++;
                ApplyUpgradeEffect(upgrade);
                OnUpgradePurchased?.Invoke(upgradeIndex);
                return true;
            }
            
            return false;
        }
        
        private void ApplyUpgradeEffect(Upgrade upgrade)
        {
            switch (upgrade.type)
            {
                case UpgradeType.TapPower:
                    GameManager.Instance.farmManager.tapPower += upgrade.effectValue;
                    break;
                    
                case UpgradeType.ClickMultiplier:
                    GameManager.Instance.farmManager.clickMultiplier += upgrade.effectValue;
                    break;
                    
                case UpgradeType.AutomationSpeed:
                    if (upgrade.level == 1) // First level unlocks automation
                    {
                        EnableAutomationForAllFarms();
                    }
                    break;
                    
                case UpgradeType.ProcessingSpeed:
                    // This would affect processing buildings
                    break;
                    
                case UpgradeType.SellPrice:
                    // This would affect the selling system
                    break;
                    
                case UpgradeType.ProductionMultiplier:
                    // This would be a global multiplier
                    break;
                    
                case UpgradeType.OfflineEarnings:
                    // This would affect offline calculations
                    break;
            }
        }
        
        private void EnableAutomationForAllFarms()
        {
            for (int i = 0; i < GameManager.Instance.farmManager.GetFarmCount(); i++)
            {
                var farm = GameManager.Instance.farmManager.GetFarm(i);
                if (farm != null && farm.isUnlocked)
                {
                    farm.hasAutomation = true;
                }
            }
        }
        
        public Upgrade GetUpgrade(int index)
        {
            if (index < 0 || index >= upgrades.Count) return null;
            return upgrades[index];
        }
        
        public int GetUpgradeCount()
        {
            return upgrades.Count;
        }
        
        public double GetUpgradeMultiplier(UpgradeType type)
        {
            double multiplier = 1.0;
            
            foreach (var upgrade in upgrades)
            {
                if (upgrade.type == type && upgrade.level > 0)
                {
                    multiplier += upgrade.effectValue * upgrade.level;
                }
            }
            
            return multiplier;
        }
    }
}