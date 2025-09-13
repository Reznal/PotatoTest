using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PotatoFarm.Core;
using PotatoFarm.Systems;

namespace PotatoFarm.UI
{
    public class MainGameUI : MonoBehaviour
    {
        [Header("Main UI Canvas")]
        public Canvas mainCanvas;
        
        [Header("Resource Display")]
        public GameObject resourcePanel;
        public TextMeshProUGUI potatoCountText;
        public TextMeshProUGUI cashCountText;
        public TextMeshProUGUI starchCountText;
        public TextMeshProUGUI goldenPotatoCountText;
        
        [Header("Production Display")]
        public TextMeshProUGUI potatoRateText;
        public TextMeshProUGUI cashRateText;
        
        [Header("Tap Farm")]
        public GameObject tapFarmPanel;
        public Button tapButton;
        public TextMeshProUGUI tapPowerText;
        public Image farmImage;
        
        [Header("Processor Control")]
        public GameObject processorControlPanel;
        public Button processorToggleButton;
        public TextMeshProUGUI processorStatusText;
        public GameObject processorListContainer;
        
        [Header("Navigation")]
        public GameObject navigationPanel;
        public Button farmsButton;
        public Button upgradesButton;
        public Button processingButton;
        public Button prestigeButton;
        public Button communityButton;
        
        [Header("Content Panels")]
        public GameObject farmsPanel;
        public GameObject upgradesPanel;
        public GameObject processingPanel;
        public GameObject prestigePanel;
        public GameObject communityPanel;
        
        [Header("Events Display")]
        public GameObject eventsPanel;
        public Transform eventListParent;
        public GameObject eventItemPrefab;
        
        [Header("Settings")]
        public Color activeButtonColor = Color.green;
        public Color inactiveButtonColor = Color.gray;
        
        private GameObject currentActivePanel;
        private List<GameObject> processorDisplayItems = new List<GameObject>();
        
        private void Start()
        {
            Debug.Log("MainGameUI Start() called");
            SetupMainUI();
            SubscribeToEvents();
            // Don't show any panel by default - let user choose
            // ShowPanel(farmsPanel);
            Debug.Log($"MainGameUI setup complete. Canvas: {mainCanvas?.name}, TapButton: {tapButton != null}");
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void SetupMainUI()
        {
            // Create main canvas if it doesn't exist
            if (mainCanvas == null)
            {
                GameObject canvasObj = new GameObject("MainCanvas");
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            // Ensure canvas is configured for interactions
            mainCanvas.overrideSorting = false;
            var canvasGroup = mainCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = mainCanvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            
            CreateUIElements();
            SetupButtonListeners();
        }
        
        private void CreateUIElements()
        {
            // Create resource panel at top
            if (resourcePanel == null)
            {
                resourcePanel = CreatePanel("ResourcePanel", mainCanvas.transform);
                var rect = resourcePanel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0.85f);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                CreateResourceDisplays();
            }
            
            // Create tap farm panel in center
            if (tapFarmPanel == null)
            {
                tapFarmPanel = CreatePanel("TapFarmPanel", mainCanvas.transform);
                var rect = tapFarmPanel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.2f, 0.3f);
                rect.anchorMax = new Vector2(0.8f, 0.7f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                CreateTapFarm();
            }
            
            // Create processor control panel on the left side
            if (processorControlPanel == null)
            {
                processorControlPanel = CreatePanel("ProcessorControlPanel", mainCanvas.transform);
                var rect = processorControlPanel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.05f, 0.3f);
                rect.anchorMax = new Vector2(0.18f, 0.7f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                CreateProcessorControl();
            }
            
            // Create navigation panel at bottom
            if (navigationPanel == null)
            {
                navigationPanel = CreatePanel("NavigationPanel", mainCanvas.transform);
                var rect = navigationPanel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0.08f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                CreateNavigationButtons();
            }
            
            // Create content panels
            CreateContentPanels();
        }
        
        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            
            var image = panel.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            var rect = panel.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            
            return panel;
        }
        
        private void CreateResourceDisplays()
        {
            var layout = resourcePanel.AddComponent<HorizontalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 10;
            
            potatoCountText = CreateResourceText(resourcePanel, "Potatoes: 0");
            cashCountText = CreateResourceText(resourcePanel, "Cash: $0");
            starchCountText = CreateResourceText(resourcePanel, "Starch: 0");
            goldenPotatoCountText = CreateResourceText(resourcePanel, "Golden: 0");
            
            // Add production rates
            var ratesPanel = CreatePanel("RatesPanel", resourcePanel.transform);
            var ratesLayout = ratesPanel.AddComponent<VerticalLayoutGroup>();
            
            potatoRateText = CreateResourceText(ratesPanel, "0/sec");
            cashRateText = CreateResourceText(ratesPanel, "$0/sec");
        }
        
        private TextMeshProUGUI CreateResourceText(GameObject parent, string text)
        {
            GameObject textObj = new GameObject("ResourceText");
            textObj.transform.SetParent(parent.transform, false);
            
            var textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 16;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;
            
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
        
        private void CreateTapFarm()
        {
            // Create tap button
            GameObject buttonObj = new GameObject("TapButton");
            buttonObj.transform.SetParent(tapFarmPanel.transform);
            
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.6f, 0.2f, 1f); // Potato-like color
            
            tapButton = buttonObj.AddComponent<Button>();
            tapButton.targetGraphic = buttonImage;
            tapButton.interactable = true;  // Explicitly enable interaction
            
            var buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localScale = Vector3.one;
            buttonRect.anchorMin = new Vector2(0.3f, 0.3f);
            buttonRect.anchorMax = new Vector2(0.7f, 0.7f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // Add button text
            var buttonText = CreateResourceText(buttonObj, "TAP POTATO!");
            buttonText.fontSize = 20;
            buttonText.fontStyle = FontStyles.Bold;
            
            // Add tap power display
            tapPowerText = CreateResourceText(tapFarmPanel, "Tap Power: 1");
            var tapPowerRect = tapPowerText.GetComponent<RectTransform>();
            tapPowerRect.anchorMin = new Vector2(0, 0);
            tapPowerRect.anchorMax = new Vector2(1, 0.2f);
            tapPowerRect.offsetMin = Vector2.zero;
            tapPowerRect.offsetMax = Vector2.zero;
        }
        
        private void CreateProcessorControl()
        {
            // Use vertical layout for processor control panel
            var layout = processorControlPanel.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 10;
            layout.padding = new RectOffset(5, 5, 10, 10);
            
            // Title text
            var titleText = CreateResourceText(processorControlPanel, "Processors");
            titleText.fontSize = 14;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            
            // Toggle button
            GameObject buttonObj = new GameObject("ProcessorToggleButton");
            buttonObj.transform.SetParent(processorControlPanel.transform, false);
            
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.6f, 0.3f, 1f); // Green when ON
            
            processorToggleButton = buttonObj.AddComponent<Button>();
            processorToggleButton.targetGraphic = buttonImage;
            processorToggleButton.interactable = true;
            
            // Set fixed height for button
            var layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 60;
            
            // Button text
            var buttonText = CreateResourceText(buttonObj, "ON");
            buttonText.fontSize = 16;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.alignment = TextAlignmentOptions.Center;
            
            // Create container for individual processor displays
            processorListContainer = new GameObject("ProcessorListContainer");
            processorListContainer.transform.SetParent(processorControlPanel.transform, false);
            
            var containerLayout = processorListContainer.AddComponent<VerticalLayoutGroup>();
            containerLayout.childControlWidth = true;
            containerLayout.childControlHeight = false;
            containerLayout.childForceExpandWidth = true;
            containerLayout.spacing = 5;
            containerLayout.padding = new RectOffset(0, 0, 0, 0);
        }
        
        private void CreateNavigationButtons()
        {
            var layout = navigationPanel.AddComponent<HorizontalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.spacing = 5;
            layout.padding = new RectOffset(5, 5, 5, 5);
            
            farmsButton = CreateNavButton(navigationPanel, "Farms");
            upgradesButton = CreateNavButton(navigationPanel, "Upgrades");
            processingButton = CreateNavButton(navigationPanel, "Processing");
            prestigeButton = CreateNavButton(navigationPanel, "Prestige");
            communityButton = CreateNavButton(navigationPanel, "Community");
        }
        
        private Button CreateNavButton(GameObject parent, string text)
        {
            GameObject buttonObj = new GameObject($"{text}Button");
            buttonObj.transform.SetParent(parent.transform);
            
            var image = buttonObj.AddComponent<Image>();
            image.color = inactiveButtonColor;
            
            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;
            button.interactable = true;  // Explicitly enable interaction
            
            // Properly configure RectTransform for button
            var rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            
            var buttonText = CreateResourceText(buttonObj, text);
            buttonText.fontSize = 12;  // Reduced font size for better fit
            
            return button;
        }
        
        private void CreateContentPanels()
        {
            // Create panels for each section
            farmsPanel = CreateContentPanel("FarmsPanel");
            upgradesPanel = CreateContentPanel("UpgradesPanel");
            processingPanel = CreateContentPanel("ProcessingPanel");
            prestigePanel = CreateContentPanel("PrestigePanel");
            communityPanel = CreateContentPanel("CommunityPanel");
            
            // Add scroll views to panels
            AddScrollViewToPanel(farmsPanel);
            AddScrollViewToPanel(upgradesPanel);
            AddScrollViewToPanel(processingPanel);
            AddScrollViewToPanel(prestigePanel);
            AddScrollViewToPanel(communityPanel);
        }
        
        private GameObject CreateContentPanel(string name)
        {
            GameObject panel = CreatePanel(name, mainCanvas.transform);
            var rect = panel.GetComponent<RectTransform>();
            
            // Set the panel to fill the center area between navigation and resource panels
            // Navigation is now 8% and resource is 15%, so content gets more space
            rect.anchorMin = new Vector2(0, 0.08f);
            rect.anchorMax = new Vector2(1, 0.85f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            
            // Ensure proper layering
            panel.transform.SetAsLastSibling();
            
            panel.SetActive(false);
            return panel;
        }
        
        private void AddScrollViewToPanel(GameObject panel)
        {
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(panel.transform, false);
            
            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollView.AddComponent<Image>().color = new Color(0, 0, 0, 0.1f);
            
            var rect = scrollView.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            
            // Create viewport (proper Unity ScrollView structure)
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            
            var viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            viewportRect.localScale = Vector3.one;
            viewportRect.localPosition = Vector3.zero;
            
            // Add Mask component to viewport for proper clipping
            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 1f); // Explicitly white with full alpha (255)
            
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            
            // Create content as child of viewport
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            
            // Ensure the content has a RectTransform component
            var contentRect = content.GetComponent<RectTransform>();
            if (contentRect == null)
            {
                contentRect = content.AddComponent<RectTransform>();
            }
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.localScale = Vector3.one;
            contentRect.localPosition = Vector3.zero;
            
            var gridLayout = content.AddComponent<GridLayoutGroup>();
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 3; // 3 columns for better layout
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.padding = new RectOffset(50, 50, 15, 15);  // Fixed padding: 50 left/right instead of -50
            
            // Set default cell sizes - will be overridden per panel type
            gridLayout.cellSize = new Vector2(350, 250);
            
            var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // Configure ScrollRect with proper viewport and content references
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            
            // Add specific panel components
            if (panel == farmsPanel)
            {
                var farmPanel = content.AddComponent<FarmPanel>();
                farmPanel.farmListParent = content.transform;
                
                // Configure grid for farms: 350x250 with 3 columns
                gridLayout.cellSize = new Vector2(350, 250);
                gridLayout.constraintCount = 3; // Three columns
            }
            else if (panel == upgradesPanel)
            {
                var upgradePanel = content.AddComponent<UpgradePanel>();
                upgradePanel.upgradeListParent = content.transform;
                
                // Configure grid for upgrades: 350x250 with 3 columns
                gridLayout.cellSize = new Vector2(350, 250);
                gridLayout.constraintCount = 3; // Three columns
            }
            else if (panel == processingPanel)
            {
                var processingPanelComp = content.AddComponent<ProcessingPanel>();
                processingPanelComp.processingListParent = content.transform;
                
                // Configure grid for processing: 350x250 with 3 columns
                gridLayout.cellSize = new Vector2(350, 250);
                gridLayout.constraintCount = 3; // Three columns
            }
            else if (panel == prestigePanel)
            {
                var prestigePanelComp = content.AddComponent<PrestigePanel>();
                prestigePanelComp.prestigeContentParent = content.transform;
                
                // Prestige panel should NOT use grid - remove GridLayoutGroup and add VerticalLayoutGroup
                Object.DestroyImmediate(gridLayout);
                var verticalLayout = content.AddComponent<VerticalLayoutGroup>();
                verticalLayout.childControlWidth = true;
                verticalLayout.childControlHeight = false;
                verticalLayout.childForceExpandWidth = true;
                verticalLayout.spacing = 15;
                verticalLayout.padding = new RectOffset(50, 50, 15, 15);
            }
            else if (panel == communityPanel)
            {
                var communityPanelComp = content.AddComponent<CommunityPanel>();
                communityPanelComp.communityContentParent = content.transform;
                
                // Configure grid for community: 350x250 with 3 columns
                gridLayout.cellSize = new Vector2(350, 250);
                gridLayout.constraintCount = 3; // Three columns
            }
        }
        
        private void SetupButtonListeners()
        {
            Debug.Log("Setting up button listeners...");
            if (tapButton != null)
            {
                tapButton.onClick.AddListener(OnTapButtonClicked);
                Debug.Log("Tap button listener added");
            }
            else
            {
                Debug.LogWarning("Tap button is null!");
            }
            
            if (farmsButton != null)
            {
                farmsButton.onClick.AddListener(() => ShowPanel(farmsPanel));
                Debug.Log("Farms button listener added");
            }
            
            if (upgradesButton != null)
            {
                upgradesButton.onClick.AddListener(() => ShowPanel(upgradesPanel));
                Debug.Log("Upgrades button listener added");
            }
            
            if (processingButton != null)
            {
                processingButton.onClick.AddListener(() => ShowPanel(processingPanel));
                Debug.Log("Processing button listener added");
            }
            
            if (prestigeButton != null)
            {
                prestigeButton.onClick.AddListener(() => ShowPanel(prestigePanel));
                Debug.Log("Prestige button listener added");
            }
            
            if (communityButton != null)
            {
                communityButton.onClick.AddListener(() => ShowPanel(communityPanel));
                Debug.Log("Community button listener added");
            }
            
            if (processorToggleButton != null)
            {
                processorToggleButton.onClick.AddListener(OnProcessorToggleClicked);
                Debug.Log("Processor toggle button listener added");
            }
            
            Debug.Log("Button listeners setup complete");
        }
        
        private void SubscribeToEvents()
        {
            if (GameManager.Instance?.resourceManager != null)
            {
                GameManager.Instance.resourceManager.OnResourceChanged += OnResourceChanged;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (GameManager.Instance?.resourceManager != null)
            {
                GameManager.Instance.resourceManager.OnResourceChanged -= OnResourceChanged;
            }
        }
        
        private void Update()
        {
            UpdateResourceDisplays();
            UpdateTapPowerDisplay();
            UpdateProcessorControlDisplay();
        }
        
        private void UpdateResourceDisplays()
        {
            if (GameManager.Instance?.resourceManager == null) return;
            
            var rm = GameManager.Instance.resourceManager;
            
            if (potatoCountText != null)
                potatoCountText.text = $"Potatoes: {FormatNumber(rm.GetResource(ResourceType.Potatoes))}";
            
            if (cashCountText != null)
                cashCountText.text = $"Cash: ${FormatNumber(rm.GetResource(ResourceType.Cash))}";
            
            if (starchCountText != null)
                starchCountText.text = $"Starch: {FormatNumber(rm.GetResource(ResourceType.Starch))}";
            
            if (goldenPotatoCountText != null)
                goldenPotatoCountText.text = $"Golden: {FormatNumber(rm.GetResource(ResourceType.GoldenPotatoes))}";
            
            if (potatoRateText != null)
                potatoRateText.text = $"{FormatNumber(rm.GetResourceRate(ResourceType.Potatoes))}/sec";
            
            if (cashRateText != null)
                cashRateText.text = $"${FormatNumber(rm.GetResourceRate(ResourceType.Cash))}/sec";
        }
        
        private void UpdateTapPowerDisplay()
        {
            if (GameManager.Instance?.farmManager == null || tapPowerText == null) return;
            
            var fm = GameManager.Instance.farmManager;
            double tapPower = fm.tapPower * fm.clickMultiplier;
            
            if (GameManager.Instance.prestigeManager != null)
            {
                tapPower *= GameManager.Instance.prestigeManager.GetTotalMultiplier();
            }
            
            tapPowerText.text = $"Tap Power: {FormatNumber(tapPower)}";
        }
        
        private void UpdateProcessorControlDisplay()
        {
            if (GameManager.Instance?.processingManager == null || processorListContainer == null) return;
            
            var pm = GameManager.Instance.processingManager;
            bool anyRunning = pm.AreAnyProcessorsRunning();
            
            // Update button color and text
            if (processorToggleButton != null && processorToggleButton.targetGraphic is Image buttonImage)
            {
                buttonImage.color = anyRunning ? 
                    new Color(0.3f, 0.6f, 0.3f, 1f) : // Green when ON
                    new Color(0.6f, 0.3f, 0.3f, 1f);  // Red when OFF
                
                var buttonText = processorToggleButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = anyRunning ? "ON" : "OFF";
                }
            }
            
            // Clear existing processor displays
            foreach (var item in processorDisplayItems)
            {
                if (item != null)
                    Destroy(item);
            }
            processorDisplayItems.Clear();
            
            // Create individual processor displays for active processors
            int buildingCount = pm.GetBuildingCount();
            for (int i = 0; i < buildingCount; i++)
            {
                var building = pm.GetBuilding(i);
                if (building != null && building.isUnlocked && building.level > 0 && building.isProcessing)
                {
                    CreateProcessorDisplayItem(building);
                }
            }
        }
        
        private void CreateProcessorDisplayItem(ProcessingBuilding building)
        {
            GameObject item = new GameObject($"ProcessorDisplay_{building.name}");
            item.transform.SetParent(processorListContainer.transform, false);
            
            // Add background for the processor item
            var bgImage = item.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
            
            // Set layout element for consistent sizing
            var layoutElement = item.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 25;
            
            // Use horizontal layout for name and progress bar
            var horizontalLayout = item.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = true;
            horizontalLayout.spacing = 5;
            horizontalLayout.padding = new RectOffset(5, 5, 2, 2);
            
            // Processor name text
            GameObject nameTextObj = new GameObject("ProcessorName");
            nameTextObj.transform.SetParent(item.transform, false);
            
            var nameText = nameTextObj.AddComponent<TextMeshProUGUI>();
            nameText.text = building.name;
            nameText.fontSize = 10;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.MidlineLeft;
            
            var nameLayout = nameTextObj.AddComponent<LayoutElement>();
            nameLayout.preferredWidth = 80;
            nameLayout.flexibleWidth = 0;
            
            // Progress bar container
            GameObject progressContainer = new GameObject("ProgressContainer");
            progressContainer.transform.SetParent(item.transform, false);
            
            var progressLayout = progressContainer.AddComponent<LayoutElement>();
            progressLayout.flexibleWidth = 1;
            progressLayout.preferredHeight = 10;
            
            // Progress bar background
            var progressBg = progressContainer.AddComponent<Image>();
            progressBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            // Progress bar fill
            GameObject progressFill = new GameObject("ProgressFill");
            progressFill.transform.SetParent(progressContainer.transform, false);
            
            var progressFillImage = progressFill.AddComponent<Image>();
            progressFillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            progressFillImage.type = Image.Type.Filled;
            progressFillImage.fillMethod = Image.FillMethod.Horizontal;
            progressFillImage.fillAmount = (float)building.GetProcessingProgress();
            
            var progressRect = progressFillImage.rectTransform;
            progressRect.anchorMin = Vector2.zero;
            progressRect.anchorMax = Vector2.one;
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = Vector2.zero;
            
            processorDisplayItems.Add(item);
        }
        
        private void OnResourceChanged(ResourceType type, double amount)
        {
            // Resources are updated in Update() method
        }
        
        private void OnTapButtonClicked()
        {
            Debug.Log("Tap button clicked!");
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.TapFarm();
                Debug.Log("TapFarm called successfully");
            }
            else
            {
                Debug.LogWarning("GameManager.Instance or farmManager is null");
            }
        }
        
        private void OnProcessorToggleClicked()
        {
            Debug.Log("Processor toggle button clicked!");
            if (GameManager.Instance?.processingManager != null)
            {
                bool newState = GameManager.Instance.processingManager.ToggleAllProcessors();
                Debug.Log($"Processors toggled to: {(newState ? "ON" : "OFF")}");
            }
            else
            {
                Debug.LogWarning("GameManager.Instance or processingManager is null");
            }
        }
        
        private void ShowPanel(GameObject panel)
        {
            Debug.Log($"ShowPanel called for: {panel?.name}");
            
            // If clicking the same panel that's already active, close it
            if (currentActivePanel == panel && panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
                currentActivePanel = null;
                UpdateNavigationButtonColors(null);
                Debug.Log($"Closed panel: {panel.name}");
                return;
            }
            
            // Hide current panel
            if (currentActivePanel != null)
            {
                currentActivePanel.SetActive(false);
                Debug.Log($"Hiding previous panel: {currentActivePanel.name}");
            }
            
            // Show new panel
            if (panel != null)
            {
                panel.SetActive(true);
                currentActivePanel = panel;
                
                // Ensure panel is properly positioned and visible
                var rect = panel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0.08f);
                rect.anchorMax = new Vector2(1, 0.85f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.localPosition = Vector3.zero;
                
                Debug.Log($"Showing panel: {panel.name}");
            }
            
            // Update button colors
            UpdateNavigationButtonColors(panel);
        }
        
        private void UpdateNavigationButtonColors(GameObject activePanel)
        {
            // Reset all buttons to inactive color
            SetButtonColor(farmsButton, inactiveButtonColor);
            SetButtonColor(upgradesButton, inactiveButtonColor);
            SetButtonColor(processingButton, inactiveButtonColor);
            SetButtonColor(prestigeButton, inactiveButtonColor);
            SetButtonColor(communityButton, inactiveButtonColor);
            
            // Set active button color only if there's an active panel
            if (activePanel == farmsPanel)
                SetButtonColor(farmsButton, activeButtonColor);
            else if (activePanel == upgradesPanel)
                SetButtonColor(upgradesButton, activeButtonColor);
            else if (activePanel == processingPanel)
                SetButtonColor(processingButton, activeButtonColor);
            else if (activePanel == prestigePanel)
                SetButtonColor(prestigeButton, activeButtonColor);
            else if (activePanel == communityPanel)
                SetButtonColor(communityButton, activeButtonColor);
        }
        
        private void SetButtonColor(Button button, Color color)
        {
            if (button != null && button.targetGraphic is Image image)
            {
                image.color = color;
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