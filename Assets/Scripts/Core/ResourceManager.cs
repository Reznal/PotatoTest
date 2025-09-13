using UnityEngine;
using System;
using System.Collections.Generic;

namespace PotatoFarm.Core
{
    [System.Serializable]
    public class ResourceData
    {
        public double potatoes = 0;
        public double cash = 100; // Starting cash
        public double starch = 0;
        public double goldenPotatoes = 0;
        
        // Rates per second
        public double potatoRate = 0;
        public double cashRate = 0;
        public double starchRate = 0;
    }
    
    public class ResourceManager : MonoBehaviour
    {
        [Header("Resources")]
        public ResourceData resources = new ResourceData();
        
        [Header("Settings")]
        public double maxPotatoes = 1e12;
        public double maxCash = 1e15;
        public double maxStarch = 1e9;
        public double maxGoldenPotatoes = 1e6;
        
        public event Action<ResourceType, double> OnResourceChanged;
        public event Action<ResourceType, double> OnResourceRateChanged;
        
        private void Start()
        {
            GameManager.Instance.OnGameTick += ProcessResourceGeneration;
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameTick -= ProcessResourceGeneration;
        }
        
        private void ProcessResourceGeneration()
        {
            // Generate resources based on rates
            if (resources.potatoRate > 0)
            {
                AddResource(ResourceType.Potatoes, resources.potatoRate * 0.1);
            }
            
            if (resources.cashRate > 0)
            {
                AddResource(ResourceType.Cash, resources.cashRate * 0.1);
            }
            
            if (resources.starchRate > 0)
            {
                AddResource(ResourceType.Starch, resources.starchRate * 0.1);
            }
        }
        
        public bool CanAfford(ResourceType type, double amount)
        {
            return GetResource(type) >= amount;
        }
        
        public bool SpendResource(ResourceType type, double amount)
        {
            if (!CanAfford(type, amount)) return false;
            
            switch (type)
            {
                case ResourceType.Potatoes:
                    resources.potatoes = Math.Max(0, resources.potatoes - amount);
                    break;
                case ResourceType.Cash:
                    resources.cash = Math.Max(0, resources.cash - amount);
                    break;
                case ResourceType.Starch:
                    resources.starch = Math.Max(0, resources.starch - amount);
                    break;
                case ResourceType.GoldenPotatoes:
                    resources.goldenPotatoes = Math.Max(0, resources.goldenPotatoes - amount);
                    break;
            }
            
            OnResourceChanged?.Invoke(type, GetResource(type));
            return true;
        }
        
        public void AddResource(ResourceType type, double amount)
        {
            switch (type)
            {
                case ResourceType.Potatoes:
                    resources.potatoes = Math.Min(maxPotatoes, resources.potatoes + amount);
                    break;
                case ResourceType.Cash:
                    resources.cash = Math.Min(maxCash, resources.cash + amount);
                    break;
                case ResourceType.Starch:
                    resources.starch = Math.Min(maxStarch, resources.starch + amount);
                    break;
                case ResourceType.GoldenPotatoes:
                    resources.goldenPotatoes = Math.Min(maxGoldenPotatoes, resources.goldenPotatoes + amount);
                    break;
            }
            
            OnResourceChanged?.Invoke(type, GetResource(type));
        }
        
        public double GetResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Potatoes: return resources.potatoes;
                case ResourceType.Cash: return resources.cash;
                case ResourceType.Starch: return resources.starch;
                case ResourceType.GoldenPotatoes: return resources.goldenPotatoes;
                default: return 0;
            }
        }
        
        public void SetResourceRate(ResourceType type, double rate)
        {
            switch (type)
            {
                case ResourceType.Potatoes:
                    resources.potatoRate = rate;
                    break;
                case ResourceType.Cash:
                    resources.cashRate = rate;
                    break;
                case ResourceType.Starch:
                    resources.starchRate = rate;
                    break;
            }
            
            OnResourceRateChanged?.Invoke(type, rate);
        }
        
        public double GetResourceRate(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Potatoes: return resources.potatoRate;
                case ResourceType.Cash: return resources.cashRate;
                case ResourceType.Starch: return resources.starchRate;
                default: return 0;
            }
        }
    }
    
    public enum ResourceType
    {
        Potatoes,
        Cash,
        Starch,
        GoldenPotatoes
    }
}