using UnityEngine;
using System.Collections.Generic;
using System;

namespace PotatoFarm.Core
{
    [System.Serializable]
    public class PrestigeData
    {
        public int totalPrestiges = 0;
        public double totalStarchEarned = 0;
        public double starchSpent = 0;
        public bool hasPrestiged = false;
    }
    
    [System.Serializable]
    public class PotatoGod
    {
        public string name;
        public string description;
        public double cost;
        public bool isUnlocked;
        public double multiplier;
        public PotatoGodType type;
    }
    
    public enum PotatoGodType
    {
        Production,
        Tap,
        Processing,
        Offline,
        Golden
    }
    
    public class PrestigeManager : MonoBehaviour
    {
        [Header("Prestige")]
        public PrestigeData prestigeData = new PrestigeData();
        
        [Header("Potato Gods")]
        public List<PotatoGod> potatoGods = new List<PotatoGod>();
        
        [Header("Settings")]
        public double starchPerPrestige = 1.0;
        public double prestigeRequirement = 1000000; // 1 million potatoes
        
        public event Action OnPrestigePerformed;
        public event Action<int> OnPotatoGodUnlocked;
        
        private void Start()
        {
            InitializePotatoGods();
        }
        
        private void InitializePotatoGods()
        {
            if (potatoGods.Count == 0)
            {
                potatoGods.Add(new PotatoGod
                {
                    name = "Spudicus the Great",
                    description = "Increases all production by 200%",
                    cost = 10,
                    multiplier = 2.0,
                    type = PotatoGodType.Production
                });
                
                potatoGods.Add(new PotatoGod
                {
                    name = "Tapmaster Supreme",
                    description = "Increases tap power by 500%",
                    cost = 25,
                    multiplier = 5.0,
                    type = PotatoGodType.Tap
                });
                
                potatoGods.Add(new PotatoGod
                {
                    name = "The Processing King",
                    description = "Increases processing speed by 300%",
                    cost = 50,
                    multiplier = 3.0,
                    type = PotatoGodType.Processing
                });
                
                potatoGods.Add(new PotatoGod
                {
                    name = "Sleepy Potato",
                    description = "Increases offline earnings by 400%",
                    cost = 75,
                    multiplier = 4.0,
                    type = PotatoGodType.Offline
                });
                
                potatoGods.Add(new PotatoGod
                {
                    name = "Golden Tuber",
                    description = "Increases golden potato chance by 1000%",
                    cost = 100,
                    multiplier = 10.0,
                    type = PotatoGodType.Golden
                });
            }
        }
        
        public bool CanPrestige()
        {
            return GameManager.Instance.resourceManager.GetResource(ResourceType.Potatoes) >= prestigeRequirement;
        }
        
        public double CalculateStarchGain()
        {
            if (!CanPrestige()) return 0;
            
            double potatoes = GameManager.Instance.resourceManager.GetResource(ResourceType.Potatoes);
            double starch = Math.Sqrt(potatoes / prestigeRequirement) * starchPerPrestige;
            return Math.Floor(starch);
        }
        
        public void PerformPrestige()
        {
            if (!CanPrestige()) return;
            
            double starchGain = CalculateStarchGain();
            
            // Reset game state
            ResetGameState();
            
            // Add starch
            GameManager.Instance.resourceManager.AddResource(ResourceType.Starch, starchGain);
            
            // Update prestige data
            prestigeData.totalPrestiges++;
            prestigeData.totalStarchEarned += starchGain;
            prestigeData.hasPrestiged = true;
            
            OnPrestigePerformed?.Invoke();
        }
        
        private void ResetGameState()
        {
            // Reset resources except starch
            var resourceManager = GameManager.Instance.resourceManager;
            resourceManager.resources.potatoes = 0;
            resourceManager.resources.cash = 100; // Starting cash
            resourceManager.resources.goldenPotatoes = 0;
            
            // Reset farms
            var farmManager = GameManager.Instance.farmManager;
            foreach (var farm in farmManager.farms)
            {
                if (farm.name == "Basic Field")
                {
                    farm.level = 1;
                    farm.isUnlocked = true;
                }
                else
                {
                    farm.level = 0;
                    farm.isUnlocked = false;
                }
                farm.hasAutomation = false;
            }
            
            // Reset upgrades
            var upgradeManager = GameManager.Instance.upgradeManager;
            foreach (var upgrade in upgradeManager.upgrades)
            {
                upgrade.level = 0;
            }
            
            // Reset farm manager values
            farmManager.tapPower = 1.0;
            farmManager.clickMultiplier = 1.0;
        }
        
        public bool UnlockPotatoGod(int godIndex)
        {
            if (godIndex < 0 || godIndex >= potatoGods.Count) return false;
            
            PotatoGod god = potatoGods[godIndex];
            if (god.isUnlocked) return false;
            
            if (GameManager.Instance.resourceManager.SpendResource(ResourceType.Starch, god.cost))
            {
                god.isUnlocked = true;
                prestigeData.starchSpent += god.cost;
                OnPotatoGodUnlocked?.Invoke(godIndex);
                return true;
            }
            
            return false;
        }
        
        public double GetTotalMultiplier()
        {
            double multiplier = 1.0;
            
            // Base prestige bonus
            if (prestigeData.hasPrestiged)
            {
                multiplier += prestigeData.totalPrestiges * 0.1; // 10% per prestige
            }
            
            // Potato god bonuses
            foreach (var god in potatoGods)
            {
                if (god.isUnlocked)
                {
                    switch (god.type)
                    {
                        case PotatoGodType.Production:
                            multiplier *= (1 + god.multiplier);
                            break;
                        case PotatoGodType.Tap:
                            // This would be applied specifically to tap power
                            break;
                    }
                }
            }
            
            return multiplier;
        }
        
        public double GetPotatoGodMultiplier(PotatoGodType type)
        {
            double multiplier = 1.0;
            
            foreach (var god in potatoGods)
            {
                if (god.isUnlocked && god.type == type)
                {
                    multiplier *= (1 + god.multiplier);
                }
            }
            
            return multiplier;
        }
        
        public PotatoGod GetPotatoGod(int index)
        {
            if (index < 0 || index >= potatoGods.Count) return null;
            return potatoGods[index];
        }
        
        public int GetPotatoGodCount()
        {
            return potatoGods.Count;
        }
        
        public int GetPrestigeLevel()
        {
            return prestigeData.totalPrestiges;
        }
    }
}