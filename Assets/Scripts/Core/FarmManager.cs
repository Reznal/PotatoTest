using UnityEngine;
using System.Collections.Generic;
using System;

namespace PotatoFarm.Core
{
    [System.Serializable]
    public class Farm
    {
        public string name;
        public bool isUnlocked;
        public int level;
        public double baseProduction;
        public double cost;
        public double costMultiplier = 1.15;
        public SoilType soilType = SoilType.Basic;
        public bool hasAutomation;
        
        public double GetCurrentProduction()
        {
            double production = baseProduction * level;
            
            // Apply soil bonuses
            switch (soilType)
            {
                case SoilType.Rich:
                    production *= 1.5;
                    break;
                case SoilType.Fertile:
                    production *= 2.0;
                    break;
                case SoilType.Magical:
                    production *= 3.0;
                    break;
            }
            
            return production;
        }
        
        public double GetUpgradeCost()
        {
            return cost * Math.Pow(costMultiplier, level);
        }
    }
    
    public enum SoilType
    {
        Basic,
        Rich,
        Fertile,
        Magical
    }
    
    public class FarmManager : MonoBehaviour
    {
        [Header("Farms")]
        public List<Farm> farms = new List<Farm>();
        
        [Header("Settings")]
        public double tapPower = 1.0;
        public double clickMultiplier = 1.0;
        
        public event Action<int> OnFarmUpgraded;
        public event Action<double> OnPotatoHarvested;
        
        private void Start()
        {
            InitializeFarms();
            GameManager.Instance.OnGameTick += ProcessAutoFarming;
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameTick -= ProcessAutoFarming;
        }
        
        private void InitializeFarms()
        {
            if (farms.Count == 0)
            {
                farms.Add(new Farm
                {
                    name = "Basic Field",
                    isUnlocked = true,
                    level = 1,
                    baseProduction = 1.0,
                    cost = 10.0,
                    soilType = SoilType.Basic
                });
                
                farms.Add(new Farm
                {
                    name = "Potato Garden",
                    isUnlocked = false,
                    level = 0,
                    baseProduction = 5.0,
                    cost = 100.0,
                    soilType = SoilType.Basic
                });
                
                farms.Add(new Farm
                {
                    name = "Potato Plantation",
                    isUnlocked = false,
                    level = 0,
                    baseProduction = 25.0,
                    cost = 1000.0,
                    soilType = SoilType.Basic
                });
            }
        }
        
        public void TapFarm()
        {
            double potatoes = tapPower * clickMultiplier;
            
            // Apply prestige multiplier if available
            if (GameManager.Instance.prestigeManager != null)
            {
                potatoes *= GameManager.Instance.prestigeManager.GetTotalMultiplier();
            }
            
            GameManager.Instance.resourceManager.AddResource(ResourceType.Potatoes, potatoes);
            OnPotatoHarvested?.Invoke(potatoes);
        }
        
        public bool UpgradeFarm(int farmIndex)
        {
            if (farmIndex < 0 || farmIndex >= farms.Count) return false;
            
            Farm farm = farms[farmIndex];
            if (!farm.isUnlocked) return false;
            
            double cost = farm.GetUpgradeCost();
            
            if (GameManager.Instance.resourceManager.SpendResource(ResourceType.Cash, cost))
            {
                farm.level++;
                OnFarmUpgraded?.Invoke(farmIndex);
                UpdateProductionRates();
                return true;
            }
            
            return false;
        }
        
        public bool UnlockFarm(int farmIndex)
        {
            if (farmIndex < 0 || farmIndex >= farms.Count) return false;
            
            Farm farm = farms[farmIndex];
            if (farm.isUnlocked) return false;
            
            double cost = farm.cost;
            
            if (GameManager.Instance.resourceManager.SpendResource(ResourceType.Cash, cost))
            {
                farm.isUnlocked = true;
                farm.level = 1;
                UpdateProductionRates();
                return true;
            }
            
            return false;
        }
        
        public void UpgradeSoil(int farmIndex, SoilType newSoilType)
        {
            if (farmIndex < 0 || farmIndex >= farms.Count) return;
            
            Farm farm = farms[farmIndex];
            if (!farm.isUnlocked) return;
            
            double cost = GetSoilUpgradeCost(newSoilType);
            
            if (GameManager.Instance.resourceManager.SpendResource(ResourceType.Cash, cost))
            {
                farm.soilType = newSoilType;
                UpdateProductionRates();
            }
        }
        
        private double GetSoilUpgradeCost(SoilType soilType)
        {
            switch (soilType)
            {
                case SoilType.Rich: return 500;
                case SoilType.Fertile: return 2500;
                case SoilType.Magical: return 10000;
                default: return 0;
            }
        }
        
        private void ProcessAutoFarming()
        {
            double totalProduction = 0;
            
            foreach (Farm farm in farms)
            {
                if (farm.isUnlocked && farm.hasAutomation && farm.level > 0)
                {
                    totalProduction += farm.GetCurrentProduction();
                }
            }
            
            if (totalProduction > 0)
            {
                // Apply prestige multiplier
                if (GameManager.Instance.prestigeManager != null)
                {
                    totalProduction *= GameManager.Instance.prestigeManager.GetTotalMultiplier();
                }
                
                GameManager.Instance.resourceManager.AddResource(ResourceType.Potatoes, totalProduction * 0.1);
            }
        }
        
        private void UpdateProductionRates()
        {
            double totalRate = 0;
            
            foreach (Farm farm in farms)
            {
                if (farm.isUnlocked && farm.hasAutomation && farm.level > 0)
                {
                    totalRate += farm.GetCurrentProduction();
                }
            }
            
            GameManager.Instance.resourceManager.SetResourceRate(ResourceType.Potatoes, totalRate);
        }
        
        public Farm GetFarm(int index)
        {
            if (index < 0 || index >= farms.Count) return null;
            return farms[index];
        }
        
        public int GetFarmCount()
        {
            return farms.Count;
        }
    }
}