using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;

namespace PotatoFarm.UI
{
    public class CommunityPanel : MonoBehaviour
    {
        [Header("Community UI")]
        public Transform communityContentParent;
        
        private void Start()
        {
            CreateCommunityUI();
        }

        private void CreateCommunityUI()
        {
            if (communityContentParent == null) return;

            // Player Statistics Card
            var statsCard = CreateEnhancedPanel("Player Stats", communityContentParent);
            var statsVertical = statsCard.AddComponent<VerticalLayoutGroup>();
            statsVertical.childControlWidth = true;
            statsVertical.childControlHeight = false;
            statsVertical.childForceExpandWidth = true;
            statsVertical.spacing = 8;
            statsVertical.padding = new RectOffset(20, 20, 20, 20);

            var statsTitle = CreateText(statsCard, "🏆 YOUR ACHIEVEMENTS");
            statsTitle.fontSize = 16;
            statsTitle.fontStyle = FontStyles.Bold;
            statsTitle.color = new Color(1f, 0.8f, 0.2f, 1f);

            CreateText(statsCard, "Total Potatoes Harvested: " + GetFormattedNumber(GetTotalPotatoesHarvested()));
            CreateText(statsCard, "Total Cash Earned: $" + GetFormattedNumber(GetTotalCashEarned()));
            CreateText(statsCard, "Play Time: " + GetPlayTimeFormatted());
            CreateText(statsCard, "Prestige Level: " + GetPrestigeLevel().ToString());

            // Daily Rewards Card
            var rewardsCard = CreateEnhancedPanel("Daily Rewards", communityContentParent);
            var rewardsVertical = rewardsCard.AddComponent<VerticalLayoutGroup>();
            rewardsVertical.childControlWidth = true;
            rewardsVertical.childControlHeight = false;
            rewardsVertical.childForceExpandWidth = true;
            rewardsVertical.spacing = 8;
            rewardsVertical.padding = new RectOffset(20, 20, 20, 20);

            var rewardsTitle = CreateText(rewardsCard, "🎁 DAILY REWARDS");
            rewardsTitle.fontSize = 16;
            rewardsTitle.fontStyle = FontStyles.Bold;
            rewardsTitle.color = new Color(0.2f, 1f, 0.2f, 1f);

            CreateText(rewardsCard, "Streak: 3 days");
            CreateText(rewardsCard, "Today's Reward: 500 Potatoes + $100");
            
            var claimButton = CreateButton(rewardsCard, "🎁 CLAIM REWARD");
            claimButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
            claimButton.onClick.AddListener(() => ClaimDailyReward());

            // Leaderboard Card
            var leaderboardCard = CreateEnhancedPanel("Global Leaderboard", communityContentParent);
            var leaderboardVertical = leaderboardCard.AddComponent<VerticalLayoutGroup>();
            leaderboardVertical.childControlWidth = true;
            leaderboardVertical.childControlHeight = false;
            leaderboardVertical.childForceExpandWidth = true;
            leaderboardVertical.spacing = 5;
            leaderboardVertical.padding = new RectOffset(20, 20, 20, 20);

            var leaderboardTitle = CreateText(leaderboardCard, "🌟 TOP FARMERS");
            leaderboardTitle.fontSize = 16;
            leaderboardTitle.fontStyle = FontStyles.Bold;
            leaderboardTitle.color = new Color(0.2f, 0.8f, 1f, 1f);

            CreateText(leaderboardCard, "🥇 PotatoMaster - 1.2T Potatoes");
            CreateText(leaderboardCard, "🥈 FarmKing - 987B Potatoes");
            CreateText(leaderboardCard, "🥉 TuberLord - 654B Potatoes");
            CreateText(leaderboardCard, "4. GoldenHarvester - 321B Potatoes");
            CreateText(leaderboardCard, "5. PotatoProud - 156B Potatoes");
            CreateText(leaderboardCard, "...");
            CreateText(leaderboardCard, "🎯 You: Rank #999+ (Keep growing!)");

            // Events & Challenges Card
            var eventsCard = CreateEnhancedPanel("Active Events", communityContentParent);
            var eventsVertical = eventsCard.AddComponent<VerticalLayoutGroup>();
            eventsVertical.childControlWidth = true;
            eventsVertical.childControlHeight = false;
            eventsVertical.childForceExpandWidth = true;
            eventsVertical.spacing = 8;
            eventsVertical.padding = new RectOffset(20, 20, 20, 20);

            var eventsTitle = CreateText(eventsCard, "⚡ SPECIAL EVENTS");
            eventsTitle.fontSize = 16;
            eventsTitle.fontStyle = FontStyles.Bold;
            eventsTitle.color = new Color(1f, 0.4f, 0.8f, 1f);

            CreateText(eventsCard, "🔥 Double Tap Bonus (2h 15m left)");
            CreateText(eventsCard, "✨ Golden Potato Weekend (Starts: 3 days)");
            CreateText(eventsCard, "🎪 Harvest Festival (Coming Soon!)");
            CreateText(eventsCard, "🚀 Mega Multiplier Monday (6 days)");

            // Social Sharing Card
            var socialCard = CreateEnhancedPanel("Share Progress", communityContentParent);
            var socialVertical = socialCard.AddComponent<VerticalLayoutGroup>();
            socialVertical.childControlWidth = true;
            socialVertical.childControlHeight = false;
            socialVertical.childForceExpandWidth = true;
            socialVertical.spacing = 10;
            socialVertical.padding = new RectOffset(20, 20, 20, 20);

            var socialTitle = CreateText(socialCard, "📱 SHARE & INVITE");
            socialTitle.fontSize = 16;
            socialTitle.fontStyle = FontStyles.Bold;
            socialTitle.color = new Color(0.8f, 0.4f, 1f, 1f);

            var shareButton = CreateButton(socialCard, "📸 Share Screenshot");
            shareButton.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);
            shareButton.onClick.AddListener(() => ShareProgress());

            var inviteButton = CreateButton(socialCard, "👥 Invite Friends (+50% bonus!)");
            inviteButton.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.8f, 1f);
            inviteButton.onClick.AddListener(() => InviteFriends());

            // Tips & Tricks Card
            var tipsCard = CreateEnhancedPanel("Pro Tips", communityContentParent);
            var tipsVertical = tipsCard.AddComponent<VerticalLayoutGroup>();
            tipsVertical.childControlWidth = true;
            tipsVertical.childControlHeight = false;
            tipsVertical.childForceExpandWidth = true;
            tipsVertical.spacing = 5;
            tipsVertical.padding = new RectOffset(20, 20, 20, 20);

            var tipsTitle = CreateText(tipsCard, "💡 FARMING TIPS");
            tipsTitle.fontSize = 16;
            tipsTitle.fontStyle = FontStyles.Bold;
            tipsTitle.color = new Color(1f, 1f, 0.2f, 1f);

            CreateText(tipsCard, "• Use potato washers to convert potatoes to cash!");
            CreateText(tipsCard, "• Buy farms early for passive income");
            CreateText(tipsCard, "• Prestige when you have 1M+ potatoes");
            CreateText(tipsCard, "• Check back often for better idle progress");
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
            
            return panel;
        }

        private GameObject CreateEnhancedPanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = new Color(0.05f, 0.05f, 0.15f, 0.9f);
            
            // Add subtle border effect
            var outline = panel.AddComponent<Outline>();
            outline.effectColor = new Color(0.3f, 0.6f, 1f, 0.5f);
            outline.effectDistance = new Vector2(2, 2);
            
            return panel;
        }

        // Helper methods for community features
        private double GetTotalPotatoesHarvested()
        {
            // This would normally come from save data/statistics
            if (GameManager.Instance?.resourceManager != null)
                return GameManager.Instance.resourceManager.GetResource(PotatoFarm.Core.ResourceType.Potatoes);
            return 0;
        }

        private double GetTotalCashEarned()
        {
            // This would normally come from save data/statistics  
            if (GameManager.Instance?.resourceManager != null)
                return GameManager.Instance.resourceManager.GetResource(PotatoFarm.Core.ResourceType.Cash);
            return 0;
        }

        private string GetPlayTimeFormatted()
        {
            // Simple approximation based on Time.realtimeSinceStartup
            float playTime = Time.realtimeSinceStartup;
            int hours = (int)(playTime / 3600);
            int minutes = (int)((playTime % 3600) / 60);
            return $"{hours}h {minutes}m";
        }

        private int GetPrestigeLevel()
        {
            if (GameManager.Instance?.prestigeManager != null)
                return GameManager.Instance.prestigeManager.GetPrestigeLevel();
            return 0;
        }

        private string GetFormattedNumber(double number)
        {
            if (number >= 1000000000000) return (number / 1000000000000).ToString("F1") + "T";
            if (number >= 1000000000) return (number / 1000000000).ToString("F1") + "B";
            if (number >= 1000000) return (number / 1000000).ToString("F1") + "M";
            if (number >= 1000) return (number / 1000).ToString("F1") + "K";
            return number.ToString("F0");
        }

        private void ClaimDailyReward()
        {
            // Add daily reward logic
            if (GameManager.Instance?.resourceManager != null)
            {
                GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Potatoes, 500);
                GameManager.Instance.resourceManager.AddResource(PotatoFarm.Core.ResourceType.Cash, 100);
                Debug.Log("Daily reward claimed: +500 Potatoes, +$100!");
            }
        }

        private void ShareProgress()
        {
            Debug.Log("Sharing progress to social media...");
            // This would integrate with platform-specific sharing APIs
        }

        private void InviteFriends()
        {
            Debug.Log("Opening friend invitation system...");
            // This would integrate with platform-specific invitation systems
        }

        private TextMeshProUGUI CreateText(GameObject parent, string text)
        {
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(parent.transform, false);

            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 14;
            textComponent.color = Color.white;

            // Properly configure RectTransform for text
            var rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            return textComponent;
        }

        private Button CreateButton(GameObject parent, string text)
        {
            var buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(parent.transform, false);

            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.8f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            var buttonLayout = buttonObj.AddComponent<LayoutElement>();
            buttonLayout.preferredHeight = 15; // Reduced to 1/3 of original height (40/3 ≈ 13-15)

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 12;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;

            var textRect = textComponent.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }
    }
}