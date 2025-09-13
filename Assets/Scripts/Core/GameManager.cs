using UnityEngine;
using System;

namespace PotatoFarm.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game State")]
        public bool isGameActive = true;
        public float gameTime = 0f;
        
        [Header("Managers")]
        public ResourceManager resourceManager;
        public UpgradeManager upgradeManager;
        public PrestigeManager prestigeManager;
        public FarmManager farmManager;
        
        [Header("Systems")]
        public PotatoFarm.Systems.ProcessingManager processingManager;
        public PotatoFarm.Systems.SaveManager saveManager;
        public PotatoFarm.Systems.EventManager eventManager;
        public PotatoFarm.Systems.CommunityManager communityManager;
        public PotatoFarm.Systems.AudioManager audioManager;
        
        public event Action OnGameTick;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Start the game tick
            InvokeRepeating(nameof(GameTick), 0f, 0.1f);
        }
        
        private void InitializeGame()
        {
            // Initialize all managers
            if (resourceManager == null)
                resourceManager = gameObject.AddComponent<ResourceManager>();
            if (upgradeManager == null)
                upgradeManager = gameObject.AddComponent<UpgradeManager>();
            if (prestigeManager == null)
                prestigeManager = gameObject.AddComponent<PrestigeManager>();
            if (farmManager == null)
                farmManager = gameObject.AddComponent<FarmManager>();
                
            // Initialize all systems
            if (processingManager == null)
                processingManager = gameObject.AddComponent<PotatoFarm.Systems.ProcessingManager>();
            if (saveManager == null)
                saveManager = gameObject.AddComponent<PotatoFarm.Systems.SaveManager>();
            if (eventManager == null)
                eventManager = gameObject.AddComponent<PotatoFarm.Systems.EventManager>();
            if (communityManager == null)
                communityManager = gameObject.AddComponent<PotatoFarm.Systems.CommunityManager>();
            if (audioManager == null)
                audioManager = gameObject.AddComponent<PotatoFarm.Systems.AudioManager>();
        }
        
        private void GameTick()
        {
            if (!isGameActive) return;
            
            gameTime += 0.1f;
            OnGameTick?.Invoke();
        }
        
        public void PauseGame()
        {
            isGameActive = false;
        }
        
        public void ResumeGame()
        {
            isGameActive = true;
        }
    }
}