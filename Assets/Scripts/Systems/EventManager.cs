using UnityEngine;
using System.Collections.Generic;
using PotatoFarm.Core;

namespace PotatoFarm.Systems
{
    [System.Serializable]
    public class GameEvent
    {
        public string name;
        public string description;
        public bool isActive;
        public float duration;
        public float timeRemaining;
        public EventType eventType;
        public double multiplier;
        public EventTarget target;
    }
    
    public enum EventType
    {
        ProductionBonus,
        TapBonus,
        SellBonus,
        GoldenPotatoBonus,
        FrenzyStorm
    }
    
    public enum EventTarget
    {
        All,
        Farms,
        Processing,
        Tap
    }
    
    public class EventManager : MonoBehaviour
    {
        [Header("Events")]
        public List<GameEvent> availableEvents = new List<GameEvent>();
        public List<GameEvent> activeEvents = new List<GameEvent>();
        
        [Header("Settings")]
        public float eventCheckInterval = 300f; // Check for new events every 5 minutes
        public float eventChance = 0.1f; // 10% chance per check
        
        public System.Action<GameEvent> OnEventStarted;
        public System.Action<GameEvent> OnEventEnded;
        
        private void Start()
        {
            InitializeEvents();
            InvokeRepeating(nameof(CheckForNewEvent), eventCheckInterval, eventCheckInterval);
            GameManager.Instance.OnGameTick += UpdateActiveEvents;
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameTick -= UpdateActiveEvents;
        }
        
        private void InitializeEvents()
        {
            if (availableEvents.Count == 0)
            {
                availableEvents.Add(new GameEvent
                {
                    name = "Potato Frenzy",
                    description = "All potato production increased by 200%!",
                    duration = 300f, // 5 minutes
                    eventType = EventType.ProductionBonus,
                    multiplier = 2.0,
                    target = EventTarget.Farms
                });
                
                availableEvents.Add(new GameEvent
                {
                    name = "Golden Hour",
                    description = "Golden potato chance increased by 500%!",
                    duration = 180f, // 3 minutes
                    eventType = EventType.GoldenPotatoBonus,
                    multiplier = 5.0,
                    target = EventTarget.All
                });
                
                availableEvents.Add(new GameEvent
                {
                    name = "Super Fingers",
                    description = "Tap power increased by 1000%!",
                    duration = 120f, // 2 minutes
                    eventType = EventType.TapBonus,
                    multiplier = 10.0,
                    target = EventTarget.Tap
                });
                
                availableEvents.Add(new GameEvent
                {
                    name = "Market Boom",
                    description = "All selling prices increased by 300%!",
                    duration = 240f, // 4 minutes
                    eventType = EventType.SellBonus,
                    multiplier = 3.0,
                    target = EventTarget.Processing
                });
                
                availableEvents.Add(new GameEvent
                {
                    name = "Lightning Storm",
                    description = "Everything goes CRAZY! All bonuses increased!",
                    duration = 60f, // 1 minute
                    eventType = EventType.FrenzyStorm,
                    multiplier = 5.0,
                    target = EventTarget.All
                });
            }
        }
        
        private void CheckForNewEvent()
        {
            if (Random.Range(0f, 1f) < eventChance)
            {
                StartRandomEvent();
            }
        }
        
        public void StartRandomEvent()
        {
            if (availableEvents.Count == 0) return;
            
            // Don't start new events if we already have 2 active
            if (activeEvents.Count >= 2) return;
            
            var eventToStart = availableEvents[Random.Range(0, availableEvents.Count)];
            StartEvent(eventToStart);
        }
        
        public void StartEvent(GameEvent gameEvent)
        {
            // Create a copy of the event
            var newEvent = new GameEvent
            {
                name = gameEvent.name,
                description = gameEvent.description,
                isActive = true,
                duration = gameEvent.duration,
                timeRemaining = gameEvent.duration,
                eventType = gameEvent.eventType,
                multiplier = gameEvent.multiplier,
                target = gameEvent.target
            };
            
            activeEvents.Add(newEvent);
            OnEventStarted?.Invoke(newEvent);
            
            Debug.Log($"Event Started: {newEvent.name} - {newEvent.description}");
        }
        
        private void UpdateActiveEvents()
        {
            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                var gameEvent = activeEvents[i];
                gameEvent.timeRemaining -= 0.1f; // Subtract tick time
                
                if (gameEvent.timeRemaining <= 0)
                {
                    EndEvent(gameEvent);
                    activeEvents.RemoveAt(i);
                }
            }
        }
        
        private void EndEvent(GameEvent gameEvent)
        {
            gameEvent.isActive = false;
            OnEventEnded?.Invoke(gameEvent);
            
            Debug.Log($"Event Ended: {gameEvent.name}");
        }
        
        public double GetEventMultiplier(EventType eventType, EventTarget target = EventTarget.All)
        {
            double multiplier = 1.0;
            
            foreach (var gameEvent in activeEvents)
            {
                if (gameEvent.isActive && 
                    (gameEvent.eventType == eventType || gameEvent.eventType == EventType.FrenzyStorm) &&
                    (gameEvent.target == target || gameEvent.target == EventTarget.All || target == EventTarget.All))
                {
                    multiplier *= (1 + gameEvent.multiplier);
                }
            }
            
            return multiplier;
        }
        
        public List<GameEvent> GetActiveEvents()
        {
            return new List<GameEvent>(activeEvents);
        }
        
        public bool HasActiveEvent(EventType eventType)
        {
            foreach (var gameEvent in activeEvents)
            {
                if (gameEvent.isActive && 
                    (gameEvent.eventType == eventType || gameEvent.eventType == EventType.FrenzyStorm))
                {
                    return true;
                }
            }
            return false;
        }
    }
}