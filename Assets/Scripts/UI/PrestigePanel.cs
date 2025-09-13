using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;

namespace PotatoFarm.UI
{
    public class PrestigePanel : MonoBehaviour
    {
        [Header("Prestige UI")]
        public Transform prestigeContentParent;
        
        private TextMeshProUGUI currentStarchText;
        private TextMeshProUGUI potatoRequirementText;
        private TextMeshProUGUI starchGainText;
        private TextMeshProUGUI multiplierText;
        private Button prestigeButton;

        private void Start()
        {
            CreatePrestigeUI();
        }

        private void CreatePrestigeUI()
        {
            if (prestigeContentParent == null) return;

            // Title
            var titleText = CreateText(prestigeContentParent.gameObject, "PRESTIGE SYSTEM");
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(1f, 0.8f, 0.2f, 1f);
            
            var titleLayout = titleText.gameObject.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 50;

            // Description
            var descText = CreateText(prestigeContentParent.gameObject, "Reset your progress to gain Starch, which provides permanent multipliers!");
            descText.fontSize = 14;
            descText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            
            var descLayout = descText.gameObject.AddComponent<LayoutElement>();
            descLayout.preferredHeight = 40;

            // Current stats panel
            var statsPanel = new GameObject("StatsPanel");
            statsPanel.transform.SetParent(prestigeContentParent);
            
            var statsBackground = statsPanel.AddComponent<Image>();
            statsBackground.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);
            
            var statsLayout = statsPanel.AddComponent<LayoutElement>();
            statsLayout.preferredHeight = 120;
            
            var statsVertical = statsPanel.AddComponent<VerticalLayoutGroup>();
            statsVertical.childControlWidth = true;
            statsVertical.childControlHeight = false;
            statsVertical.childForceExpandWidth = true;
            statsVertical.spacing = 5;
            statsVertical.padding = new RectOffset(15, 15, 15, 15);

            // Current starch
            currentStarchText = CreateText(statsPanel, "Current Starch: 0");
            currentStarchText.fontSize = 16;
            currentStarchText.fontStyle = FontStyles.Bold;
            currentStarchText.color = new Color(0.8f, 0.6f, 1f, 1f);

            // Current multiplier
            multiplierText = CreateText(statsPanel, "Total Multiplier: 1.00x");
            multiplierText.fontSize = 16;
            multiplierText.color = new Color(0.6f, 1f, 0.6f, 1f);

            // Potato requirement
            potatoRequirementText = CreateText(statsPanel, "Requirement: 1,000,000 Potatoes");
            potatoRequirementText.fontSize = 14;
            potatoRequirementText.color = new Color(1f, 0.6f, 0.6f, 1f);

            // Spacer
            var spacer = new GameObject("Spacer");
            spacer.transform.SetParent(prestigeContentParent);
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.preferredHeight = 20;

            // Prestige action panel
            var actionPanel = new GameObject("ActionPanel");
            actionPanel.transform.SetParent(prestigeContentParent);
            
            var actionBackground = actionPanel.AddComponent<Image>();
            actionBackground.color = new Color(0.2f, 0.1f, 0.1f, 0.8f);
            
            var actionLayout = actionPanel.AddComponent<LayoutElement>();
            actionLayout.preferredHeight = 100;
            
            var actionVertical = actionPanel.AddComponent<VerticalLayoutGroup>();
            actionVertical.childControlWidth = true;
            actionVertical.childControlHeight = false;
            actionVertical.childForceExpandWidth = true;
            actionVertical.spacing = 10;
            actionVertical.padding = new RectOffset(15, 15, 15, 15);

            // Starch gain display
            starchGainText = CreateText(actionPanel, "Starch Gain: 0");
            starchGainText.fontSize = 18;
            starchGainText.fontStyle = FontStyles.Bold;
            starchGainText.color = new Color(1f, 0.8f, 0.2f, 1f);

            // Prestige button
            var buttonObj = new GameObject("PrestigeButton");
            buttonObj.transform.SetParent(actionPanel.transform);

            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);

            prestigeButton = buttonObj.AddComponent<Button>();
            prestigeButton.targetGraphic = buttonImage;

            var buttonLayout = buttonObj.AddComponent<LayoutElement>();
            buttonLayout.preferredHeight = 50;

            var buttonText = CreateText(buttonObj, "PRESTIGE NOW!");
            buttonText.fontSize = 18;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;

            var buttonTextRect = buttonText.rectTransform;
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;

            // Set up button listener
            prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);

            // Spacer
            var spacer2 = new GameObject("Spacer2");
            spacer2.transform.SetParent(prestigeContentParent);
            var spacer2Layout = spacer2.AddComponent<LayoutElement>();
            spacer2Layout.preferredHeight = 20;

            // Benefits explanation
            var benefitsPanel = new GameObject("BenefitsPanel");
            benefitsPanel.transform.SetParent(prestigeContentParent);
            
            var benefitsBackground = benefitsPanel.AddComponent<Image>();
            benefitsBackground.color = new Color(0.1f, 0.2f, 0.1f, 0.8f);
            
            var benefitsLayout = benefitsPanel.AddComponent<LayoutElement>();
            benefitsLayout.preferredHeight = 200;
            
            var benefitsVertical = benefitsPanel.AddComponent<VerticalLayoutGroup>();
            benefitsVertical.childControlWidth = true;
            benefitsVertical.childControlHeight = false;
            benefitsVertical.childForceExpandWidth = true;
            benefitsVertical.spacing = 5;
            benefitsVertical.padding = new RectOffset(15, 15, 15, 15);

            var benefitsTitle = CreateText(benefitsPanel, "PRESTIGE BENEFITS");
            benefitsTitle.fontSize = 16;
            benefitsTitle.fontStyle = FontStyles.Bold;
            benefitsTitle.color = new Color(0.6f, 1f, 0.6f, 1f);

            CreateText(benefitsPanel, "• Starch provides permanent multipliers to all production");
            CreateText(benefitsPanel, "• Each Starch = +10% production multiplier");
            CreateText(benefitsPanel, "• Starch persists through all future prestiges");
            CreateText(benefitsPanel, "• Higher potato counts = more Starch gained");
            CreateText(benefitsPanel, "• Progression becomes much faster after prestiging");
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

        private void Update()
        {
            UpdatePrestigeDisplay();
        }

        private void UpdatePrestigeDisplay()
        {
            if (GameManager.Instance?.prestigeManager == null || GameManager.Instance?.resourceManager == null) return;

            var pm = GameManager.Instance.prestigeManager;
            var rm = GameManager.Instance.resourceManager;

            // Update current starch
            if (currentStarchText != null)
                currentStarchText.text = $"Current Starch: {FormatNumber(rm.GetResource(ResourceType.Starch))}";

            // Update total multiplier
            if (multiplierText != null)
                multiplierText.text = $"Total Multiplier: {pm.GetTotalMultiplier():F2}x";

            // Update potato requirement
            if (potatoRequirementText != null)
            {
                double currentPotatoes = rm.GetResource(ResourceType.Potatoes);
                bool hasEnough = currentPotatoes >= pm.prestigeRequirement;
                potatoRequirementText.text = $"Requirement: {FormatNumber(pm.prestigeRequirement)} Potatoes";
                potatoRequirementText.color = hasEnough ? 
                    new Color(0.6f, 1f, 0.6f, 1f) : 
                    new Color(1f, 0.6f, 0.6f, 1f);
            }

            // Update starch gain
            if (starchGainText != null)
            {
                double starchGain = pm.CalculateStarchGain();
                starchGainText.text = $"Starch Gain: {FormatNumber(starchGain)}";
                starchGainText.color = starchGain > 0 ? 
                    new Color(1f, 0.8f, 0.2f, 1f) : 
                    new Color(0.6f, 0.6f, 0.6f, 1f);
            }

            // Update prestige button
            if (prestigeButton != null)
            {
                bool canPrestige = pm.CanPrestige();
                prestigeButton.interactable = canPrestige;
                var buttonImage = prestigeButton.GetComponent<Image>();
                buttonImage.color = canPrestige ? 
                    new Color(0.8f, 0.2f, 0.2f, 1f) : 
                    new Color(0.4f, 0.4f, 0.4f, 1f);
            }
        }

        private void OnPrestigeButtonClicked()
        {
            if (GameManager.Instance?.prestigeManager != null)
            {
                GameManager.Instance.prestigeManager.PerformPrestige();
            }
        }

        private string FormatNumber(double number)
        {
            if (number < 1000)
                return number.ToString("F0");
            else if (number < 1000000)
                return (number / 1000).ToString("F1") + "K";
            else if (number < 1000000000)
                return (number / 1000000).ToString("F1") + "M";
            else if (number < 1000000000000)
                return (number / 1000000000).ToString("F1") + "B";
            else
                return (number / 1000000000000).ToString("F1") + "T";
        }
    }
}