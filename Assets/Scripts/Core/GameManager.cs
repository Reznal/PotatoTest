using UnityEngine;
using System;
using PotatoFarm.Systems;

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
        public ProcessingManager processingManager;
        public SaveManager saveManager;
        public EventManager eventManager;
        public CommunityManager communityManager;
        public AudioManager audioManager;
        
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
                resourceManager = GetOrAddComponent<ResourceManager>();
            if (upgradeManager == null)
                upgradeManager = GetOrAddComponent<UpgradeManager>();
            if (prestigeManager == null)
                prestigeManager = GetOrAddComponent<PrestigeManager>();
            if (farmManager == null)
                farmManager = GetOrAddComponent<FarmManager>();
                
            // Initialize all systems
            if (processingManager == null)
                processingManager = GetOrAddComponent<ProcessingManager>();
            if (saveManager == null)
                saveManager = GetOrAddComponent<SaveManager>();
            if (eventManager == null)
                eventManager = GetOrAddComponent<EventManager>();
            if (communityManager == null)
                communityManager = GetOrAddComponent<CommunityManager>();
            if (audioManager == null)
                audioManager = GetOrAddComponent<AudioManager>();
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

        private T GetOrAddComponent<T>() where T : Component
        {
            if (!TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}