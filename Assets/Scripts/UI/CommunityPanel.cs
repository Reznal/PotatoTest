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

            // Title
            var titleText = CreateText(communityContentParent.gameObject, "COMMUNITY FEATURES");
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.2f, 0.8f, 1f, 1f);
            
            var titleLayout = titleText.gameObject.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 50;

            // Leaderboard section
            var leaderboardPanel = CreatePanel("Leaderboard", communityContentParent);
            var leaderboardLayout = leaderboardPanel.AddComponent<LayoutElement>();
            leaderboardLayout.preferredHeight = 200;
            
            var leaderboardVertical = leaderboardPanel.AddComponent<VerticalLayoutGroup>();
            leaderboardVertical.childControlWidth = true;
            leaderboardVertical.childControlHeight = false;
            leaderboardVertical.childForceExpandWidth = true;
            leaderboardVertical.spacing = 5;
            leaderboardVertical.padding = new RectOffset(15, 15, 15, 15);

            var leaderboardTitle = CreateText(leaderboardPanel, "Global Leaderboard");
            leaderboardTitle.fontSize = 18;
            leaderboardTitle.fontStyle = FontStyles.Bold;
            leaderboardTitle.color = new Color(1f, 0.8f, 0.2f, 1f);

            // Sample leaderboard entries
            CreateText(leaderboardPanel, "1. PotatoMaster - 1.2T Potatoes");
            CreateText(leaderboardPanel, "2. FarmKing - 987B Potatoes");
            CreateText(leaderboardPanel, "3. TuberLord - 654B Potatoes");
            CreateText(leaderboardPanel, "...");
            CreateText(leaderboardPanel, "You: 0 Potatoes (Rank: Unranked)");

            // Events section
            var eventsPanel = CreatePanel("Events", communityContentParent);
            var eventsLayout = eventsPanel.AddComponent<LayoutElement>();
            eventsLayout.preferredHeight = 150;
            
            var eventsVertical = eventsPanel.AddComponent<VerticalLayoutGroup>();
            eventsVertical.childControlWidth = true;
            eventsVertical.childControlHeight = false;
            eventsVertical.childForceExpandWidth = true;
            eventsVertical.spacing = 5;
            eventsVertical.padding = new RectOffset(15, 15, 15, 15);

            var eventsTitle = CreateText(eventsPanel, "Special Events");
            eventsTitle.fontSize = 18;
            eventsTitle.fontStyle = FontStyles.Bold;
            eventsTitle.color = new Color(1f, 0.8f, 0.2f, 1f);

            CreateText(eventsPanel, "• Double Tap Event (Active: 2h 15m)");
            CreateText(eventsPanel, "• Golden Potato Weekend (Starts in: 3d)");
            CreateText(eventsPanel, "• Harvest Festival (Coming Soon)");

            // Challenges section
            var challengesPanel = CreatePanel("Challenges", communityContentParent);
            var challengesLayout = challengesPanel.AddComponent<LayoutElement>();
            challengesLayout.preferredHeight = 180;
            
            var challengesVertical = challengesPanel.AddComponent<VerticalLayoutGroup>();
            challengesVertical.childControlWidth = true;
            challengesVertical.childControlHeight = false;
            challengesVertical.childForceExpandWidth = true;
            challengesVertical.spacing = 5;
            challengesVertical.padding = new RectOffset(15, 15, 15, 15);

            var challengesTitle = CreateText(challengesPanel, "Daily Challenges");
            challengesTitle.fontSize = 18;
            challengesTitle.fontStyle = FontStyles.Bold;
            challengesTitle.color = new Color(1f, 0.8f, 0.2f, 1f);

            CreateText(challengesPanel, "✓ Tap 100 times (100/100) - Completed!");
            CreateText(challengesPanel, "○ Earn $10K (Progress: $0/10K)");
            CreateText(challengesPanel, "○ Harvest 50K Potatoes (Progress: 0/50K)");
            CreateText(challengesPanel, "○ Buy 5 Upgrades (Progress: 0/5)");

            // Social section
            var socialPanel = CreatePanel("Social", communityContentParent);
            var socialLayout = socialPanel.AddComponent<LayoutElement>();
            socialLayout.preferredHeight = 120;
            
            var socialVertical = socialPanel.AddComponent<VerticalLayoutGroup>();
            socialVertical.childControlWidth = true;
            socialVertical.childControlHeight = false;
            socialVertical.childForceExpandWidth = true;
            socialVertical.spacing = 5;
            socialVertical.padding = new RectOffset(15, 15, 15, 15);

            var socialTitle = CreateText(socialPanel, "Share Your Progress");
            socialTitle.fontSize = 18;
            socialTitle.fontStyle = FontStyles.Bold;
            socialTitle.color = new Color(1f, 0.8f, 0.2f, 1f);

            // Share buttons
            var shareButton = CreateButton(socialPanel, "Share on Social Media");
            shareButton.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);

            var inviteButton = CreateButton(socialPanel, "Invite Friends");
            inviteButton.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.8f, 1f);
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);
            
            var image = panel.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
            
            return panel;
        }

        private TextMeshProUGUI CreateText(GameObject parent, string text)
        {
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(parent.transform);

            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 14;
            textComponent.color = Color.white;

            return textComponent;
        }

        private Button CreateButton(GameObject parent, string text)
        {
            var buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(parent.transform);

            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.8f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            var buttonLayout = buttonObj.AddComponent<LayoutElement>();
            buttonLayout.preferredHeight = 40;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

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