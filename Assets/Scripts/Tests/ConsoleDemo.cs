using UnityEngine;

namespace PotatoFarm.Tests
{
    /// <summary>
    /// Simple console-based demo of the potato farming game.
    /// This can be used to test the game mechanics without Unity UI.
    /// </summary>
    public class ConsoleDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        public bool runDemoOnStart = false;
        public float demoSpeed = 1.0f;
        
        private void Start()
        {
            if (runDemoOnStart)
            {
                StartCoroutine(RunDemo());
            }
        }
        
        private System.Collections.IEnumerator RunDemo()
        {
            yield return new WaitForSeconds(1f); // Wait for initialization
            
            Debug.Log("=== POTATO FARM DEMO STARTING ===");
            
            // Show initial state
            LogGameState("Initial State");
            yield return new WaitForSeconds(demoSpeed);
            
            // Demonstrate tapping
            Debug.Log("\n--- Tapping 10 times ---");
            for (int i = 0; i < 10; i++)
            {
                GameManager.Instance.farmManager.TapFarm();
                yield return new WaitForSeconds(demoSpeed * 0.1f);
            }
            LogGameState("After Tapping");
            yield return new WaitForSeconds(demoSpeed);
            
            // Try to buy an upgrade
            Debug.Log("\n--- Attempting to buy upgrade ---");
            bool purchased = GameManager.Instance.upgradeManager.PurchaseUpgrade(0);
            Debug.Log($"Upgrade purchased: {purchased}");
            LogGameState("After Upgrade Attempt");
            yield return new WaitForSeconds(demoSpeed);
            
            // Add some cash and try again
            Debug.Log("\n--- Adding cash and buying upgrade ---");
            GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Cash, 100);
            purchased = GameManager.Instance.upgradeManager.PurchaseUpgrade(0);
            Debug.Log($"Upgrade purchased: {purchased}");
            LogGameState("After Successful Upgrade");
            yield return new WaitForSeconds(demoSpeed);
            
            // Test automation
            Debug.Log("\n--- Testing automation ---");
            var autoUpgrade = GameManager.Instance.upgradeManager.GetUpgrade(2); // Auto Harvester
            if (autoUpgrade != null)
            {
                GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Cash, 1000);
                GameManager.Instance.upgradeManager.PurchaseUpgrade(2);
                Debug.Log("Automation unlocked! Waiting 5 seconds...");
                yield return new WaitForSeconds(5f);
                LogGameState("After 5 seconds of automation");
            }
            
            // Test prestige calculation
            Debug.Log("\n--- Testing prestige ---");
            GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Potatoes, 2000000);
            var pm = GameManager.Instance.prestigeManager;
            bool canPrestige = pm.CanPrestige();
            double starchGain = pm.CalculateStarchGain();
            Debug.Log($"Can prestige: {canPrestige}, Starch gain: {starchGain}");
            
            // Test processing
            Debug.Log("\n--- Testing processing ---");
            if (GameManager.Instance.processingManager != null)
            {
                var building = GameManager.Instance.processingManager.GetBuilding(0);
                if (building != null)
                {
                    Debug.Log($"Washer: Level {building.level}, Speed {building.GetProcessingSpeed()}");
                }
            }
            
            Debug.Log("\n=== DEMO COMPLETED ===");
            LogGameState("Final State");
        }
        
        private void LogGameState(string label)
        {
            var rm = GameManager.Instance.resourceManager;
            var fm = GameManager.Instance.farmManager;
            
            Debug.Log($"\n{label}:");
            Debug.Log($"  Potatoes: {rm.GetResource(PotatoFarm.Core.ResourceType.Potatoes):F0}");
            Debug.Log($"  Cash: ${rm.GetResource(PotatoFarm.Core.ResourceType.Cash):F0}");
            Debug.Log($"  Starch: {rm.GetResource(PotatoFarm.Core.ResourceType.Starch):F0}");
            Debug.Log($"  Tap Power: {fm.tapPower:F1}");
            Debug.Log($"  Click Multiplier: {fm.clickMultiplier:F1}");
            Debug.Log($"  Potato Rate: {rm.GetResourceRate(PotatoFarm.Core.ResourceType.Potatoes):F1}/sec");
        }
        
        [ContextMenu("Run Quick Demo")]
        public void RunQuickDemo()
        {
            StartCoroutine(RunDemo());
        }
        
        [ContextMenu("Log Current State")]
        public void LogCurrentState()
        {
            LogGameState("Current State");
        }
        
        [ContextMenu("Simulate 1 Hour Offline")]
        public void SimulateOfflineTime()
        {
            Debug.Log("Simulating 1 hour offline...");
            
            // Calculate what would be earned in 1 hour
            var rm = GameManager.Instance.resourceManager;
            var fm = GameManager.Instance.farmManager;
            
            double totalProduction = 0;
            for (int i = 0; i < fm.GetFarmCount(); i++)
            {
                var farm = fm.GetFarm(i);
                if (farm.isUnlocked && farm.hasAutomation && farm.level > 0)
                {
                    totalProduction += farm.GetCurrentProduction();
                }
            }
            
            double offlineProduction = totalProduction * 3600 * 0.5; // 1 hour at 50% efficiency
            rm.AddResource(PotatoFarm.Core.ResourceType.Potatoes, offlineProduction);
            
            Debug.Log($"Gained {offlineProduction:F0} potatoes from 1 hour offline!");
            LogCurrentState();
        }
    }
}