using UnityEngine;
using PotatoFarm.Core;

namespace PotatoFarm.Tests
{
    public class GameTester : MonoBehaviour
    {
        [Header("Test Settings")]
        public bool runTestsOnStart = true;
        public bool logDetails = true;
        
        private void Start()
        {
            if (runTestsOnStart)
            {
                StartCoroutine(RunTests());
            }
        }
        
        private System.Collections.IEnumerator RunTests()
        {
            yield return new WaitForSeconds(1f); // Wait for game to initialize
            
            if (logDetails) Debug.Log("=== Starting Potato Farm Game Tests ===");
            
            TestResourceManager();
            yield return new WaitForSeconds(0.1f);
            
            TestFarmManager();
            yield return new WaitForSeconds(0.1f);
            
            TestUpgradeManager();
            yield return new WaitForSeconds(0.1f);
            
            TestPrestigeManager();
            yield return new WaitForSeconds(0.1f);
            
            if (logDetails) Debug.Log("=== All Tests Completed ===");
        }
        
        private void TestResourceManager()
        {
            if (logDetails) Debug.Log("Testing Resource Manager...");
            
            var rm = GameManager.Instance.resourceManager;
            
            // Test adding resources
            rm.AddResource(ResourceType.Potatoes, 100);
            rm.AddResource(ResourceType.Cash, 50);
            
            // Test spending resources
            bool canAfford = rm.CanAfford(ResourceType.Cash, 25);
            bool spent = rm.SpendResource(ResourceType.Cash, 25);
            
            if (logDetails)
            {
                Debug.Log($"Potatoes: {rm.GetResource(ResourceType.Potatoes)}");
                Debug.Log($"Cash: {rm.GetResource(ResourceType.Cash)}");
                Debug.Log($"Can afford 25 cash: {canAfford}, Spent: {spent}");
            }
        }
        
        private void TestFarmManager()
        {
            if (logDetails) Debug.Log("Testing Farm Manager...");
            
            var fm = GameManager.Instance.farmManager;
            
            // Test tapping
            double potatoesBefore = GameManager.Instance.resourceManager.GetResource(ResourceType.Potatoes);
            fm.TapFarm();
            double potatoesAfter = GameManager.Instance.resourceManager.GetResource(ResourceType.Potatoes);
            
            if (logDetails)
            {
                Debug.Log($"Tap generated: {potatoesAfter - potatoesBefore} potatoes");
                Debug.Log($"Farm count: {fm.GetFarmCount()}");
                
                for (int i = 0; i < fm.GetFarmCount(); i++)
                {
                    var farm = fm.GetFarm(i);
                    Debug.Log($"Farm {i}: {farm.name}, Level: {farm.level}, Unlocked: {farm.isUnlocked}");
                }
            }
        }
        
        private void TestUpgradeManager()
        {
            if (logDetails) Debug.Log("Testing Upgrade Manager...");
            
            var um = GameManager.Instance.upgradeManager;
            
            if (logDetails)
            {
                Debug.Log($"Upgrade count: {um.GetUpgradeCount()}");
                
                for (int i = 0; i < um.GetUpgradeCount(); i++)
                {
                    var upgrade = um.GetUpgrade(i);
                    Debug.Log($"Upgrade {i}: {upgrade.name}, Level: {upgrade.level}, Cost: {upgrade.GetCurrentCost()}");
                }
            }
        }
        
        private void TestPrestigeManager()
        {
            if (logDetails) Debug.Log("Testing Prestige Manager...");
            
            var pm = GameManager.Instance.prestigeManager;
            
            bool canPrestige = pm.CanPrestige();
            double starchGain = pm.CalculateStarchGain();
            
            if (logDetails)
            {
                Debug.Log($"Can prestige: {canPrestige}");
                Debug.Log($"Starch gain if prestiged: {starchGain}");
                Debug.Log($"Total prestiges: {pm.prestigeData.totalPrestiges}");
                Debug.Log($"Potato gods count: {pm.GetPotatoGodCount()}");
            }
        }
        
        [ContextMenu("Run Manual Test")]
        public void RunManualTest()
        {
            StartCoroutine(RunTests());
        }
        
        [ContextMenu("Test Tap 10 Times")]
        public void TestTapping()
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.Instance.farmManager.TapFarm();
            }
            Debug.Log($"After 10 taps: {GameManager.Instance.resourceManager.GetResource(ResourceType.Potatoes)} potatoes");
        }
        
        [ContextMenu("Add Test Resources")]
        public void AddTestResources()
        {
            var rm = GameManager.Instance.resourceManager;
            rm.AddResource(ResourceType.Potatoes, 10000);
            rm.AddResource(ResourceType.Cash, 5000);
            rm.AddResource(ResourceType.Starch, 100);
            Debug.Log("Added test resources");
        }
    }
}