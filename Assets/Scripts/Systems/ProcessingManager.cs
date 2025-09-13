using UnityEngine;
using System.Collections.Generic;
using System;
using PotatoFarm.Core;

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
        public bool isProcessing;
        public double processingProgress; // 0.0 to 1.0
        
        public double GetProcessingSpeed()
        {
            return baseSpeed * level;
        }
        
        public double GetUpgradeCost()
        {
            return cost * Math.Pow(costMultiplier, level);
        }
        
        public double GetEfficiency()
        {
            return 100.0 * (1.0 + level * 0.1); // 10% efficiency increase per level
        }
        
        public string inputType
        {
            get { return "Potatoes"; }
        }
        
        public string outputType
        {
            get 
            { 
                switch (type)
                {
                    case ProcessingType.Washer: return "Clean Potatoes";
                    case ProcessingType.Fryer: return "Fried Potatoes";
                    case ProcessingType.ChipFactory: return "Potato Chips";
                    default: return "Processed Potatoes";
                }
            }
        }
        
        public double GetProcessingProgress()
        {
            return processingProgress;
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
                
                if (building.isProcessing)
                {
                    // Update processing progress
                    double speed = building.GetProcessingSpeed() * processingEfficiency;
                    double progressIncrease = speed * 0.1 / 5.0; // 5 seconds to complete one cycle (faster for better gameplay)
                    building.processingProgress += progressIncrease;
                    
                    if (building.processingProgress >= 1.0)
                    {
                        building.processingProgress = 0.0;
                        
                        // Complete processing cycle
                        double potatoesNeeded = building.inputRequired;
                        var resourceManager = GameManager.Instance.resourceManager;
                        
                        if (resourceManager.CanAfford(PotatoFarm.Core.ResourceType.Potatoes, potatoesNeeded))
                        {
                            resourceManager.SpendResource(PotatoFarm.Core.ResourceType.Potatoes, potatoesNeeded);
                            double cashGained = building.outputValue;
                            resourceManager.AddResource(PotatoFarm.Core.ResourceType.Cash, cashGained);
                            
                            Debug.Log($"{building.name} processed {potatoesNeeded} potatoes into ${cashGained} cash!");
                            OnItemProcessed?.Invoke(building.type, 1);
                        }
                        else
                        {
                            // Stop processing if not enough resources
                            building.isProcessing = false;
                            building.processingProgress = 0.0;
                        }
                    }
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
        
        public bool StartProcessing(int buildingIndex)
        {
            if (buildingIndex < 0 || buildingIndex >= buildings.Count) return false;
            
            var building = buildings[buildingIndex];
            if (!building.isUnlocked || building.level <= 0) return false;
            
            building.isProcessing = true;
            building.processingProgress = 0.0;
            Debug.Log($"Started processing at {building.name}");
            return true;
        }
        
        public bool StopProcessing(int buildingIndex)
        {
            if (buildingIndex < 0 || buildingIndex >= buildings.Count) return false;
            
            var building = buildings[buildingIndex];
            building.isProcessing = false;
            building.processingProgress = 0.0;
            Debug.Log($"Stopped processing at {building.name}");
            return true;
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
        
        public bool ToggleAllProcessors()
        {
            bool anyProcessing = false;
            
            // Check if any processor is currently running
            foreach (var building in buildings)
            {
                if (building.isUnlocked && building.level > 0 && building.isProcessing)
                {
                    anyProcessing = true;
                    break;
                }
            }
            
            // If any are processing, stop all. Otherwise, start all.
            bool targetState = !anyProcessing;
            
            foreach (var building in buildings)
            {
                if (building.isUnlocked && building.level > 0)
                {
                    building.isProcessing = targetState;
                    if (!targetState)
                    {
                        building.processingProgress = 0.0;
                    }
                }
            }
            
            Debug.Log($"Toggled all processors: {(targetState ? "ON" : "OFF")}");
            return targetState;
        }
        
        public bool AreAnyProcessorsRunning()
        {
            foreach (var building in buildings)
            {
                if (building.isUnlocked && building.level > 0 && building.isProcessing)
                {
                    return true;
                }
            }
            return false;
        }
        
        public int GetActiveProcessorCount()
        {
            int count = 0;
            foreach (var building in buildings)
            {
                if (building.isUnlocked && building.level > 0 && building.isProcessing)
                {
                    count++;
                }
            }
            return count;
        }
    }
}