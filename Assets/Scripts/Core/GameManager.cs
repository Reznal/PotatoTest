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