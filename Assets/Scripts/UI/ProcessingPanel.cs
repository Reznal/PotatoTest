using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;
using System.Collections.Generic;
using PotatoFarm.Systems;

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

            // Add background to show it's a separate item
            var backgroundImage = item.AddComponent<Image>();
            backgroundImage.color = new Color(0.25f, 0.15f, 0.25f, 0.8f); // Dark purple background for processing

            // The GridLayoutGroup on the parent will handle sizing (350x250)
            // Use vertical layout for internal organization
            var verticalLayout = item.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.spacing = 4;
            verticalLayout.padding = new RectOffset(12, 12, 12, 12);

            // Building name
            var nameText = CreateText(item, building.name);
            nameText.fontSize = 15;
            nameText.fontStyle = FontStyles.Bold;

            // Building level and speed
            var levelText = CreateText(item, $"Level: {building.level}");
            var speedText = CreateText(item, $"Speed: {building.GetProcessingSpeed():F1}/sec");
            var efficiencyText = CreateText(item, $"Efficiency: {building.GetEfficiency():F1}%");

            // Input/Output info
            var ioText = CreateText(item, $"{building.inputType} → {building.outputType}");
            ioText.fontSize = 12;
            ioText.color = Color.cyan;

            // Progress bar if processing
            if (building.isProcessing)
            {
                var progressObj = new GameObject("Progress");
                progressObj.transform.SetParent(item.transform, false);
                
                var progressBg = progressObj.AddComponent<Image>();
                progressBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                
                var progressLayout = progressObj.AddComponent<LayoutElement>();
                progressLayout.preferredHeight = 15;
                
                var progressBarObj = new GameObject("ProgressBar");
                progressBarObj.transform.SetParent(progressObj.transform, false);
                
                var progressBar = progressBarObj.AddComponent<Image>();
                progressBar.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                
                var progressRect = progressBar.rectTransform;
                progressRect.anchorMin = Vector2.zero;
                progressRect.anchorMax = new Vector2((float)building.GetProcessingProgress(), 1f);
                progressRect.offsetMin = Vector2.zero;
                progressRect.offsetMax = Vector2.zero;
            }

            // Button area
            var buttonArea = new GameObject("ButtonArea");
            buttonArea.transform.SetParent(item.transform, false);
            
            var buttonLayout = buttonArea.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = false;
            buttonLayout.childForceExpandWidth = true;
            buttonLayout.spacing = 5;

            if (building.isUnlocked)
            {
                // Upgrade button
                bool canAffordUpgrade = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.GetUpgradeCost());
                var upgradeButton = CreateButton(buttonArea, $"Upgrade\n${building.GetUpgradeCost():F0}");
                
                if (!canAffordUpgrade)
                {
                    upgradeButton.interactable = false;
                    upgradeButton.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    upgradeButton.interactable = true;
                    upgradeButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
                    upgradeButton.onClick.AddListener(() => {
                        bool success = GameManager.Instance.processingManager.UpgradeBuilding(index);
                        if (success)
                        {
                            Debug.Log($"Successfully upgraded processing building {index}");
                            RefreshProcessingList(); // Refresh only on structural change
                        }
                        else
                        {
                            Debug.Log($"Failed to upgrade processing building {index}");
                        }
                    });
                }

                // Start/Stop processing button
                var processButton = CreateButton(buttonArea, building.isProcessing ? "STOP" : "START");
                processButton.interactable = true; // Explicitly enable interaction
                processButton.GetComponent<Image>().color = building.isProcessing ? 
                    new Color(0.8f, 0.2f, 0.2f, 1f) : 
                    new Color(0.2f, 0.8f, 0.2f, 1f);
                
                processButton.onClick.AddListener(() => {
                    if (building.isProcessing)
                    {
                        GameManager.Instance.processingManager.StopProcessing(index);
                        Debug.Log($"Stopped processing building {index}");
                    }
                    else
                    {
                        GameManager.Instance.processingManager.StartProcessing(index);
                        Debug.Log($"Started processing building {index}");
                    }
                    // Update only the button states, not the entire UI
                    UpdateProcessingAffordability();
                });
            }
            else
            {
                // Unlock button
                bool canAffordUnlock = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.cost);
                var unlockButton = CreateButton(buttonArea, $"Unlock\n${building.cost:F0}");
                
                if (!canAffordUnlock)
                {
                    unlockButton.interactable = false;
                    unlockButton.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    unlockButton.interactable = true;
                    unlockButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
                    unlockButton.onClick.AddListener(() => {
                        bool success = GameManager.Instance.processingManager.UnlockBuilding(index);
                        if (success)
                        {
                            Debug.Log($"Successfully unlocked processing building {index}");
                            RefreshProcessingList(); // Refresh only on structural change
                        }
                        else
                        {
                            Debug.Log($"Failed to unlock processing building {index}");
                        }
                    });
                }
            }
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
            button.interactable = true; // Explicitly enable interaction

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
            // Update processing status and affordability every second (60 frames at 60 FPS)
            if (Time.frameCount % 60 == 0) // Every 60 frames (1 second)
            {
                UpdateProcessingAffordability();
            }
        }

        private void UpdateProcessingAffordability()
        {
            if (GameManager.Instance?.processingManager == null || GameManager.Instance?.resourceManager == null) return;

            for (int i = 0; i < processingItems.Count; i++)
            {
                if (processingItems[i] == null) continue;

                var building = GameManager.Instance.processingManager.GetBuilding(i);
                if (building == null) continue;

                // Find the button area
                var buttonArea = processingItems[i].transform.Find("ButtonArea");
                if (buttonArea == null) continue;

                var buttons = buttonArea.GetComponentsInChildren<Button>();
                
                if (building.isUnlocked)
                {
                    // First button should be upgrade, second should be start/stop
                    if (buttons.Length >= 1)
                    {
                        var upgradeButton = buttons[0];
                        bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.GetUpgradeCost());
                        upgradeButton.interactable = canAfford;
                        upgradeButton.GetComponent<Image>().color = canAfford ? 
                            new Color(0.2f, 0.8f, 0.2f, 1f) : 
                            Color.red;
                    }
                    
                    if (buttons.Length >= 2)
                    {
                        var processButton = buttons[1];
                        processButton.interactable = true;
                        
                        // Update button text and color based on processing state
                        var buttonText = processButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = building.isProcessing ? "STOP" : "START";
                        }
                        
                        processButton.GetComponent<Image>().color = building.isProcessing ? 
                            new Color(0.8f, 0.2f, 0.2f, 1f) : 
                            new Color(0.2f, 0.8f, 0.2f, 1f);
                    }
                }
                else
                {
                    // Only unlock button
                    if (buttons.Length >= 1)
                    {
                        var unlockButton = buttons[0];
                        bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, building.cost);
                        unlockButton.interactable = canAfford;
                        unlockButton.GetComponent<Image>().color = canAfford ? 
                            new Color(0.2f, 0.8f, 0.2f, 1f) : 
                            Color.red;
                    }
                }

                // Update progress bars for active processing
                var progressTransform = processingItems[i].transform.Find("Progress");
                if (progressTransform != null)
                {
                    var progressBar = progressTransform.Find("ProgressBar");
                    if (progressBar != null && building.isProcessing)
                    {
                        var progressRect = progressBar.GetComponent<RectTransform>();
                        if (progressRect != null)
                        {
                            progressRect.anchorMax = new Vector2((float)building.GetProcessingProgress(), 1f);
                        }
                    }
                }
            }
        }
    }
}