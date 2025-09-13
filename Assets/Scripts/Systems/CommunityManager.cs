using UnityEngine;
using System.Collections.Generic;

namespace PotatoFarm.Systems
{
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public double score;
        public int rank;
        public System.DateTime timestamp;
    }
    
    [System.Serializable]
    public class CommunityGoal
    {
        public string name;
        public string description;
        public double targetValue;
        public double currentValue;
        public bool isCompleted;
        public System.DateTime deadline;
        public List<RewardItem> rewards;
    }
    
    [System.Serializable]
    public class RewardItem
    {
        public string name;
        public double amount;
        public PotatoFarm.Core.ResourceType resourceType;
    }
    
    public class CommunityManager : MonoBehaviour
    {
        [Header("Leaderboards")]
        public List<LeaderboardEntry> weeklyLeaderboard = new List<LeaderboardEntry>();
        public List<LeaderboardEntry> allTimeLeaderboard = new List<LeaderboardEntry>();
        
        [Header("Community Goals")]
        public List<CommunityGoal> activeCommunityGoals = new List<CommunityGoal>();
        public List<CommunityGoal> completedCommunityGoals = new List<CommunityGoal>();
        
        [Header("Settings")]
        public string playerName = "Player";
        public float leaderboardUpdateInterval = 300f; // 5 minutes
        
        public System.Action<LeaderboardEntry> OnLeaderboardUpdated;
        public System.Action<CommunityGoal> OnCommunityGoalCompleted;
        
        private void Start()
        {
            InitializeCommunityGoals();
            InvokeRepeating(nameof(UpdateLeaderboards), leaderboardUpdateInterval, leaderboardUpdateInterval);
            InvokeRepeating(nameof(UpdateCommunityGoals), 60f, 60f); // Check every minute
        }
        
        private void InitializeCommunityGoals()
        {
            if (activeCommunityGoals.Count == 0)
            {
                activeCommunityGoals.Add(new CommunityGoal
                {
                    name = "Million Potato Challenge",
                    description = "Community goal: Harvest 1 billion potatoes together!",
                    targetValue = 1000000000,
                    currentValue = 0,
                    deadline = System.DateTime.Now.AddDays(7),
                    rewards = new List<RewardItem>
                    {
                        new RewardItem { name = "Golden Potatoes", amount = 100, resourceType = PotatoFarm.Core.ResourceType.GoldenPotatoes },
                        new RewardItem { name = "Starch Bonus", amount = 50, resourceType = PotatoFarm.Core.ResourceType.Starch }
                    }
                });
                
                activeCommunityGoals.Add(new CommunityGoal
                {
                    name = "Processing Marathon",
                    description = "Community goal: Process 100 million potatoes!",
                    targetValue = 100000000,
                    currentValue = 0,
                    deadline = System.DateTime.Now.AddDays(3),
                    rewards = new List<RewardItem>
                    {
                        new RewardItem { name = "Cash Bonus", amount = 10000, resourceType = PotatoFarm.Core.ResourceType.Cash }
                    }
                });
            }
        }
        
        private void UpdateLeaderboards()
        {
            // Simulate leaderboard update with fake data for demonstration
            // In a real game, this would connect to a server
            UpdatePlayerScore();
            GenerateFakeLeaderboardEntries();
        }
        
        private void UpdatePlayerScore()
        {
            if (GameManager.Instance?.resourceManager == null) return;
            
            double playerScore = GameManager.Instance.resourceManager.GetResource(PotatoFarm.Core.ResourceType.Potatoes);
            
            // Update or add player entry
            var playerEntry = weeklyLeaderboard.Find(e => e.playerName == playerName);
            if (playerEntry != null)
            {
                playerEntry.score = playerScore;
                playerEntry.timestamp = System.DateTime.Now;
            }
            else
            {
                weeklyLeaderboard.Add(new LeaderboardEntry
                {
                    playerName = playerName,
                    score = playerScore,
                    timestamp = System.DateTime.Now
                });
            }
            
            // Sort and update ranks
            weeklyLeaderboard.Sort((a, b) => b.score.CompareTo(a.score));
            for (int i = 0; i < weeklyLeaderboard.Count; i++)
            {
                weeklyLeaderboard[i].rank = i + 1;
            }
            
            OnLeaderboardUpdated?.Invoke(playerEntry ?? weeklyLeaderboard[weeklyLeaderboard.Count - 1]);
        }
        
        private void GenerateFakeLeaderboardEntries()
        {
            // Generate some fake entries for demonstration
            string[] names = { "PotatoKing", "FarmMaster", "TuberLord", "ChipChamp", "SpudHero", "TaterTitan" };
            
            while (weeklyLeaderboard.Count < 10)
            {
                weeklyLeaderboard.Add(new LeaderboardEntry
                {
                    playerName = names[Random.Range(0, names.Length)] + Random.Range(100, 999),
                    score = Random.Range(1000, 1000000),
                    timestamp = System.DateTime.Now.AddMinutes(-Random.Range(1, 1440))
                });
            }
        }
        
        private void UpdateCommunityGoals()
        {
            foreach (var goal in activeCommunityGoals)
            {
                if (goal.isCompleted) continue;
                
                // Simulate community progress
                goal.currentValue += Random.Range(1000, 10000);
                
                if (goal.currentValue >= goal.targetValue)
                {
                    CompleteCommunityGoal(goal);
                }
                else if (System.DateTime.Now > goal.deadline)
                {
                    FailCommunityGoal(goal);
                }
            }
        }
        
        private void CompleteCommunityGoal(CommunityGoal goal)
        {
            goal.isCompleted = true;
            goal.currentValue = goal.targetValue;
            
            // Give rewards to player
            foreach (var reward in goal.rewards)
            {
                GameManager.Instance.resourceManager.AddResource(reward.resourceType, reward.amount);
            }
            
            completedCommunityGoals.Add(goal);
            activeCommunityGoals.Remove(goal);
            
            OnCommunityGoalCompleted?.Invoke(goal);
            Debug.Log($"Community Goal Completed: {goal.name}!");
        }
        
        private void FailCommunityGoal(CommunityGoal goal)
        {
            activeCommunityGoals.Remove(goal);
            Debug.Log($"Community Goal Failed: {goal.name}");
        }
        
        public void ContributeToGoal(string goalName, double amount)
        {
            var goal = activeCommunityGoals.Find(g => g.name == goalName);
            if (goal != null && !goal.isCompleted)
            {
                goal.currentValue += amount;
            }
        }
        
        public List<LeaderboardEntry> GetWeeklyLeaderboard(int count = 10)
        {
            var result = new List<LeaderboardEntry>(weeklyLeaderboard);
            return result.GetRange(0, Mathf.Min(count, result.Count));
        }
        
        public List<CommunityGoal> GetActiveCommunityGoals()
        {
            return new List<CommunityGoal>(activeCommunityGoals);
        }
        
        public int GetPlayerRank()
        {
            var playerEntry = weeklyLeaderboard.Find(e => e.playerName == playerName);
            return playerEntry?.rank ?? -1;
        }
    }
}