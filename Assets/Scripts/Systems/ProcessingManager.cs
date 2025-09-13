using UnityEngine;
using System.Collections.Generic;
using System;

namespace PotatoFarm.Systems
{
    [System.Serializable]
    public class ProcessingBuilding
    {
        public string name;
        public ProcessingType type;
        public bool isUnlocked;
        public int level;
        public double baseSpeed; // Items per second
        public double cost;
        public double costMultiplier = 1.2;
        public double inputRequired; // Potatoes needed per output
        public double outputValue; // Cash per output
        
        public double GetProcessingSpeed()
        {
            return baseSpeed * level;
        }
        
        public double GetUpgradeCost()
        {
            return cost * Math.Pow(costMultiplier, level);
        }
    }
    
    public enum ProcessingType
    {
        Washer,
        Fryer,
        ChipFactory
    }
    
    public class ProcessingManager : MonoBehaviour
    {
        [Header("Buildings")]
        public List<ProcessingBuilding> buildings = new List<ProcessingBuilding>();
        
        [Header("Settings")]
        public double processingEfficiency = 1.0;
        
        public event Action<ProcessingType, int> OnItemProcessed;
        public event Action<int> OnBuildingUpgraded;
        
        private void Start()
        {
            InitializeBuildings();
            GameManager.Instance.OnGameTick += ProcessItems;
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameTick -= ProcessItems;
        }
        
        private void InitializeBuildings()
        {
            if (buildings.Count == 0)
            {
                buildings.Add(new ProcessingBuilding
                {
                    name = "Potato Washer",
                    type = ProcessingType.Washer,
                    isUnlocked = true,
                    level = 1,
                    baseSpeed = 1.0,
                    cost = 100,
                    inputRequired = 10,
                    outputValue = 15
                });
                
                buildings.Add(new ProcessingBuilding
                {
                    name = "Potato Fryer",
                    type = ProcessingType.Fryer,
                    isUnlocked = false,
                    level = 0,
                    baseSpeed = 0.5,
                    cost = 500,
                    inputRequired = 20,
                    outputValue = 50
                });
                
                buildings.Add(new ProcessingBuilding
                {
                    name = "Chip Factory",
                    type = ProcessingType.ChipFactory,
                    isUnlocked = false,
                    level = 0,
                    baseSpeed = 0.2,
                    cost = 2500,
                    inputRequired = 50,
                    outputValue = 200
                });
            }
        }
        
        private void ProcessItems()
        {
            foreach (var building in buildings)
            {
                if (!building.isUnlocked || building.level <= 0) continue;
                
                double speed = building.GetProcessingSpeed() * processingEfficiency;
                double itemsToProcess = speed * 0.1; // Per tick (0.1 seconds)
                double potatoesNeeded = itemsToProcess * building.inputRequired;
                
                var resourceManager = GameManager.Instance.resourceManager;
                
                if (resourceManager.CanAfford(PotatoFarm.Core.ResourceType.Potatoes, potatoesNeeded))
                {
                    resourceManager.SpendResource(PotatoFarm.Core.ResourceType.Potatoes, potatoesNeeded);
                    double cashGained = itemsToProcess * building.outputValue;
                    resourceManager.AddResource(PotatoFarm.Core.ResourceType.Cash, cashGained);
                    
                    OnItemProcessed?.Invoke(building.type, (int)itemsToProcess);
                }
            }
        }
        
        public bool UnlockBuilding(int buildingIndex)
        {
            if (buildingIndex < 0 || buildingIndex >= buildings.Count) return false;
            
            var building = buildings[buildingIndex];
            if (building.isUnlocked) return false;
            
            double cost = building.cost;
            
            if (GameManager.Instance.resourceManager.SpendResource(PotatoFarm.Core.ResourceType.Cash, cost))
            {
                building.isUnlocked = true;
                building.level = 1;
                return true;
            }
            
            return false;
        }
        
        public bool UpgradeBuilding(int buildingIndex)
        {
            if (buildingIndex < 0 || buildingIndex >= buildings.Count) return false;
            
            var building = buildings[buildingIndex];
            if (!building.isUnlocked) return false;
            
            double cost = building.GetUpgradeCost();
            
            if (GameManager.Instance.resourceManager.SpendResource(PotatoFarm.Core.ResourceType.Cash, cost))
            {
                building.level++;
                OnBuildingUpgraded?.Invoke(buildingIndex);
                return true;
            }
            
            return false;
        }
        
        public ProcessingBuilding GetBuilding(int index)
        {
            if (index < 0 || index >= buildings.Count) return null;
            return buildings[index];
        }
        
        public int GetBuildingCount()
        {
            return buildings.Count;
        }
        
        public double GetTotalProcessingRate()
        {
            double totalRate = 0;
            
            foreach (var building in buildings)
            {
                if (building.isUnlocked && building.level > 0)
                {
                    totalRate += building.GetProcessingSpeed() * building.outputValue;
                }
            }
            
            return totalRate * processingEfficiency;
        }
    }
}