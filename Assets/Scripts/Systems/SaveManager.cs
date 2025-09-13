using UnityEngine;
using System;
using System.IO;

namespace PotatoFarm.Systems
{
    [System.Serializable]
    public class SaveData
    {
        public double potatoes;
        public double cash;
        public double starch;
        public double goldenPotatoes;
        public float gameTime;
        public DateTime lastSaveTime;
        
        // Farm data
        public FarmSaveData[] farms;
        
        // Upgrade data
        public UpgradeSaveData[] upgrades;
        
        // Prestige data
        public PrestigeSaveData prestige;
        
        // Processing data
        public ProcessingSaveData[] buildings;
    }
    
    [System.Serializable]
    public class FarmSaveData
    {
        public bool isUnlocked;
        public int level;
        public int soilType;
        public bool hasAutomation;
    }
    
    [System.Serializable]
    public class UpgradeSaveData
    {
        public int level;
    }
    
    [System.Serializable]
    public class PrestigeSaveData
    {
        public int totalPrestiges;
        public double totalStarchEarned;
        public double starchSpent;
        public bool hasPrestiged;
        public bool[] unlockedGods;
    }
    
    [System.Serializable]
    public class ProcessingSaveData
    {
        public bool isUnlocked;
        public int level;
    }
    
    public class SaveManager : MonoBehaviour
    {
        [Header("Settings")]
        public float autoSaveInterval = 30f;
        public string saveFileName = "potato_save.json";
        
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);
        
        public event Action OnGameLoaded;
        public event Action OnGameSaved;
        
        private void Start()
        {
            LoadGame();
            InvokeRepeating(nameof(SaveGame), autoSaveInterval, autoSaveInterval);
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveGame();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                SaveGame();
        }
        
        public void SaveGame()
        {
            if (GameManager.Instance == null) return;
            
            try
            {
                SaveData saveData = CreateSaveData();
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SaveFilePath, json);
                
                OnGameSaved?.Invoke();
                Debug.Log($"Game saved to: {SaveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }
        
        public void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.Log("No save file found, starting new game");
                return;
            }
            
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                ApplySaveData(saveData);
                CalculateOfflineProgress(saveData);
                
                OnGameLoaded?.Invoke();
                Debug.Log("Game loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
            }
        }
        
        private SaveData CreateSaveData()
        {
            var saveData = new SaveData();
            var rm = GameManager.Instance.resourceManager;
            var fm = GameManager.Instance.farmManager;
            var um = GameManager.Instance.upgradeManager;
            var pm = GameManager.Instance.prestigeManager;
            
            // Resources
            saveData.potatoes = rm.resources.potatoes;
            saveData.cash = rm.resources.cash;
            saveData.starch = rm.resources.starch;
            saveData.goldenPotatoes = rm.resources.goldenPotatoes;
            saveData.gameTime = GameManager.Instance.gameTime;
            saveData.lastSaveTime = DateTime.Now;
            
            // Farms
            saveData.farms = new FarmSaveData[fm.GetFarmCount()];
            for (int i = 0; i < fm.GetFarmCount(); i++)
            {
                var farm = fm.GetFarm(i);
                saveData.farms[i] = new FarmSaveData
                {
                    isUnlocked = farm.isUnlocked,
                    level = farm.level,
                    soilType = (int)farm.soilType,
                    hasAutomation = farm.hasAutomation
                };
            }
            
            // Upgrades
            saveData.upgrades = new UpgradeSaveData[um.GetUpgradeCount()];
            for (int i = 0; i < um.GetUpgradeCount(); i++)
            {
                var upgrade = um.GetUpgrade(i);
                saveData.upgrades[i] = new UpgradeSaveData
                {
                    level = upgrade.level
                };
            }
            
            // Prestige
            saveData.prestige = new PrestigeSaveData
            {
                totalPrestiges = pm.prestigeData.totalPrestiges,
                totalStarchEarned = pm.prestigeData.totalStarchEarned,
                starchSpent = pm.prestigeData.starchSpent,
                hasPrestiged = pm.prestigeData.hasPrestiged,
                unlockedGods = new bool[pm.GetPotatoGodCount()]
            };
            
            for (int i = 0; i < pm.GetPotatoGodCount(); i++)
            {
                var god = pm.GetPotatoGod(i);
                saveData.prestige.unlockedGods[i] = god.isUnlocked;
            }
            
            return saveData;
        }
        
        private void ApplySaveData(SaveData saveData)
        {
            var rm = GameManager.Instance.resourceManager;
            var fm = GameManager.Instance.farmManager;
            var um = GameManager.Instance.upgradeManager;
            var pm = GameManager.Instance.prestigeManager;
            
            // Resources
            rm.resources.potatoes = saveData.potatoes;
            rm.resources.cash = saveData.cash;
            rm.resources.starch = saveData.starch;
            rm.resources.goldenPotatoes = saveData.goldenPotatoes;
            GameManager.Instance.gameTime = saveData.gameTime;
            
            // Farms
            if (saveData.farms != null)
            {
                for (int i = 0; i < saveData.farms.Length && i < fm.GetFarmCount(); i++)
                {
                    var farm = fm.GetFarm(i);
                    var farmData = saveData.farms[i];
                    
                    farm.isUnlocked = farmData.isUnlocked;
                    farm.level = farmData.level;
                    farm.soilType = (PotatoFarm.Core.SoilType)farmData.soilType;
                    farm.hasAutomation = farmData.hasAutomation;
                }
            }
            
            // Upgrades
            if (saveData.upgrades != null)
            {
                for (int i = 0; i < saveData.upgrades.Length && i < um.GetUpgradeCount(); i++)
                {
                    var upgrade = um.GetUpgrade(i);
                    upgrade.level = saveData.upgrades[i].level;
                }
            }
            
            // Prestige
            if (saveData.prestige != null)
            {
                pm.prestigeData.totalPrestiges = saveData.prestige.totalPrestiges;
                pm.prestigeData.totalStarchEarned = saveData.prestige.totalStarchEarned;
                pm.prestigeData.starchSpent = saveData.prestige.starchSpent;
                pm.prestigeData.hasPrestiged = saveData.prestige.hasPrestiged;
                
                if (saveData.prestige.unlockedGods != null)
                {
                    for (int i = 0; i < saveData.prestige.unlockedGods.Length && i < pm.GetPotatoGodCount(); i++)
                    {
                        var god = pm.GetPotatoGod(i);
                        god.isUnlocked = saveData.prestige.unlockedGods[i];
                    }
                }
            }
        }
        
        private void CalculateOfflineProgress(SaveData saveData)
        {
            TimeSpan offlineTime = DateTime.Now - saveData.lastSaveTime;
            double offlineSeconds = offlineTime.TotalSeconds;
            
            // Cap offline time to prevent exploitation
            offlineSeconds = Math.Min(offlineSeconds, 24 * 3600); // Max 24 hours
            
            if (offlineSeconds > 60) // Only calculate if offline for more than 1 minute
            {
                double offlineProduction = 0;
                var fm = GameManager.Instance.farmManager;
                
                for (int i = 0; i < fm.GetFarmCount(); i++)
                {
                    var farm = fm.GetFarm(i);
                    if (farm.isUnlocked && farm.hasAutomation && farm.level > 0)
                    {
                        offlineProduction += farm.GetCurrentProduction();
                    }
                }
                
                // Apply offline efficiency (reduced production while offline)
                double offlineEfficiency = 0.5; // 50% efficiency while offline
                offlineProduction *= offlineEfficiency;
                
                double offlinePotatoes = offlineProduction * offlineSeconds;
                
                if (offlinePotatoes > 0)
                {
                    GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Potatoes, offlinePotatoes);
                    Debug.Log($"Offline for {offlineTime.TotalMinutes:F1} minutes. Gained {offlinePotatoes:F0} potatoes!");
                }
            }
        }
        
        public void DeleteSave()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("Save file deleted");
            }
        }
    }
}