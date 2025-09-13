using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;
using System.Collections.Generic;

namespace PotatoFarm.UI
{
    public class ProcessingPanel : MonoBehaviour
    {
        [Header("Processing List")]
        public Transform processingListParent;
        
        private List<GameObject> processingItems = new List<GameObject>();

        private void Start()
        {
            RefreshProcessingList();
            
            // Subscribe to events
            if (GameManager.Instance?.processingManager != null)
            {
                GameManager.Instance.processingManager.OnBuildingUpgraded += OnBuildingUpgraded;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance?.processingManager != null)
            {
                GameManager.Instance.processingManager.OnBuildingUpgraded -= OnBuildingUpgraded;
            }
        }

        private void OnBuildingUpgraded(int buildingIndex)
        {
            RefreshProcessingList();
        }

        private void RefreshProcessingList()
        {
            if (GameManager.Instance?.processingManager == null) return;

            // Clear existing items
            foreach (var item in processingItems)
            {
                if (item != null)
                    Destroy(item);
            }
            processingItems.Clear();

            var processingManager = GameManager.Instance.processingManager;

            for (int i = 0; i < processingManager.GetBuildingCount(); i++)
            {
                var building = processingManager.GetBuilding(i);
                if (building == null) continue;

                CreateProcessingItem(building, i);
            }
        }

        private void CreateProcessingItem(ProcessingBuilding building, int index)
        {
            GameObject item = new GameObject($"Processing_{index}");
            item.transform.SetParent(processingListParent, false);
            processingItems.Add(item);

            // Add layout components
            var layoutElement = item.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 140;

            var horizontalLayout = item.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 10;
            horizontalLayout.padding = new RectOffset(10, 10, 10, 10);

            // Building info
            var infoPanel = new GameObject("Info");
            infoPanel.transform.SetParent(item.transform, false);
            var infoLayout = infoPanel.AddComponent<LayoutElement>();
            infoLayout.preferredWidth = 280;

            var infoVertical = infoPanel.AddComponent<VerticalLayoutGroup>();
            infoVertical.childControlWidth = true;
            infoVertical.childControlHeight = false;
            infoVertical.childForceExpandWidth = true;
            infoVertical.spacing = 2;

            // Building name
            var nameText = CreateText(infoPanel, building.name);
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;

            // Building level and speed
            var levelText = CreateText(infoPanel, $"Level: {building.level}");
            var speedText = CreateText(infoPanel, $"Speed: {building.GetProcessingSpeed():F1}/sec");
            var efficiencyText = CreateText(infoPanel, $"Efficiency: {building.GetEfficiency():F1}%");

            // Input/Output info
            var inputText = CreateText(infoPanel, $"Input: {building.inputType}");
            var outputText = CreateText(infoPanel, $"Output: {building.outputType}");

            // Progress bar if processing
            if (building.isProcessing)
            {
                var progressObj = new GameObject("Progress");
                progressObj.transform.SetParent(infoPanel.transform, false);
                
                var progressBg = progressObj.AddComponent<Image>();
                progressBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                var progressLayout = progressObj.AddComponent<LayoutElement>();
                progressLayout.preferredHeight = 20;
                
                var progressBarObj = new GameObject("ProgressBar");
                progressBarObj.transform.SetParent(progressObj.transform, false);
                
                var progressBar = progressBarObj.AddComponent<Image>();
                progressBar.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                
                var progressRect = progressBar.rectTransform;
                progressRect.anchorMin = Vector2.zero;
                progressRect.anchorMax = new Vector2(building.GetProcessingProgress(), 1f);
                progressRect.offsetMin = Vector2.zero;
                progressRect.offsetMax = Vector2.zero;
            }

            // Actions panel
            var actionPanel = new GameObject("Actions");
            actionPanel.transform.SetParent(item.transform, false);
            var actionLayout = actionPanel.AddComponent<LayoutElement>();
            actionLayout.preferredWidth = 150;

            var actionVertical = actionPanel.AddComponent<VerticalLayoutGroup>();
            actionVertical.childControlWidth = true;
            actionVertical.childControlHeight = false;
            actionVertical.childForceExpandWidth = true;
            actionVertical.spacing = 5;

            if (building.isUnlocked)
            {
                // Upgrade cost
                var costText = CreateText(actionPanel, $"Upgrade: ${building.GetUpgradeCost():F0}");
                costText.fontSize = 12;
                costText.fontStyle = FontStyles.Bold;

                // Upgrade button
                bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.GetUpgradeCost());
                var upgradeButton = CreateButton(actionPanel, "UPGRADE");
                
                if (!canAfford)
                {
                    upgradeButton.interactable = false;
                    upgradeButton.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    upgradeButton.interactable = true;
                    upgradeButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.processingManager.UpgradeBuilding(index));
                }

                // Start/Stop processing button
                var processButton = CreateButton(actionPanel, building.isProcessing ? "STOP" : "START");
                processButton.GetComponent<Image>().color = building.isProcessing ? 
                    new Color(0.8f, 0.2f, 0.2f, 1f) : 
                    new Color(0.2f, 0.8f, 0.2f, 1f);
                
                processButton.onClick.AddListener(() => {
                    if (building.isProcessing)
                        GameManager.Instance.processingManager.StopProcessing(index);
                    else
                        GameManager.Instance.processingManager.StartProcessing(index);
                });
            }
            else
            {
                // Unlock cost
                var unlockCostText = CreateText(actionPanel, $"Unlock: ${building.cost:F0}");
                unlockCostText.fontSize = 12;
                unlockCostText.fontStyle = FontStyles.Bold;

                // Unlock button
                bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.cost);
                var unlockButton = CreateButton(actionPanel, "UNLOCK");
                
                if (!canAfford)
                {
                    unlockButton.interactable = false;
                    unlockButton.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    unlockButton.interactable = true;
                    unlockButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
                    unlockButton.onClick.AddListener(() => GameManager.Instance.processingManager.UnlockBuilding(index));
                }
            }

            // Background
            var background = item.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
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
            buttonLayout.preferredHeight = 35;

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

        private void Update()
        {
            // Refresh processing status and affordability periodically
            if (Time.frameCount % 60 == 0) // Every 60 frames
            {
                RefreshProcessingList();
            }
        }
    }
}