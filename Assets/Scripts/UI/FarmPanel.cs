using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;

namespace PotatoFarm.UI
{
    public class FarmPanel : MonoBehaviour
    {
        [Header("Farm UI")]
        public Transform farmListParent;
        public GameObject farmItemPrefab;
        
        private void Start()
        {
            RefreshFarmList();
            
            // Subscribe to events
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.OnFarmUpgraded += OnFarmUpgraded;
            }
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.OnFarmUpgraded -= OnFarmUpgraded;
            }
        }
        
        private void OnFarmUpgraded(int farmIndex)
        {
            RefreshFarmList();
        }
        
        private void RefreshFarmList()
        {
            if (GameManager.Instance?.farmManager == null || farmListParent == null) return;
            
            // Clear existing items
            foreach (Transform child in farmListParent)
            {
                Destroy(child.gameObject);
            }
            
            var farmManager = GameManager.Instance.farmManager;
            
            for (int i = 0; i < farmManager.GetFarmCount(); i++)
            {
                var farm = farmManager.GetFarm(i);
                if (farm == null) continue;
                
                CreateFarmItem(farm, i);
            }
        }
        
        private void CreateFarmItem(Farm farm, int index)
        {
            GameObject item = new GameObject($"Farm_{index}");
            item.transform.SetParent(farmListParent);
            
            // Add layout components
            var layoutElement = item.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 100;
            
            var horizontalLayout = item.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 10;
            horizontalLayout.padding = new RectOffset(10, 10, 10, 10);
            
            // Farm info
            var infoPanel = new GameObject("Info");
            infoPanel.transform.SetParent(item.transform);
            var infoLayout = infoPanel.AddComponent<LayoutElement>();
            infoLayout.preferredWidth = 200;
            
            var infoVertical = infoPanel.AddComponent<VerticalLayoutGroup>();
            infoVertical.childControlWidth = true;
            infoVertical.childControlHeight = false;
            infoVertical.childForceExpandWidth = true;
            
            // Farm name
            var nameText = CreateText(infoPanel, $"{farm.name}");
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            
            // Farm level and production
            var levelText = CreateText(infoPanel, $"Level: {farm.level}");
            var productionText = CreateText(infoPanel, $"Production: {farm.GetCurrentProduction():F1}/sec");
            var soilText = CreateText(infoPanel, $"Soil: {farm.soilType}");
            
            // Buttons panel
            var buttonPanel = new GameObject("Buttons");
            buttonPanel.transform.SetParent(item.transform);
            var buttonLayout = buttonPanel.AddComponent<LayoutElement>();
            buttonLayout.preferredWidth = 150;
            
            var buttonVertical = buttonPanel.AddComponent<VerticalLayoutGroup>();
            buttonVertical.childControlWidth = true;
            buttonVertical.childControlHeight = false;
            buttonVertical.childForceExpandWidth = true;
            buttonVertical.spacing = 5;
            
            if (farm.isUnlocked)
            {
                // Upgrade button
                var upgradeButton = CreateButton(buttonPanel, $"Upgrade (${farm.GetUpgradeCost():F0})");
                upgradeButton.onClick.AddListener(() => GameManager.Instance.farmManager.UpgradeFarm(index));
                
                // Automation status
                var autoText = CreateText(buttonPanel, farm.hasAutomation ? "Auto: ON" : "Auto: OFF");
                autoText.color = farm.hasAutomation ? Color.green : Color.red;
            }
            else
            {
                // Unlock button
                var unlockButton = CreateButton(buttonPanel, $"Unlock (${farm.cost:F0})");
                unlockButton.onClick.AddListener(() => GameManager.Instance.farmManager.UnlockFarm(index));
            }
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
            buttonLayout.preferredHeight = 30;
            
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