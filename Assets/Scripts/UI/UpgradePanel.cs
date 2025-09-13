using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;
using System.Collections.Generic;

namespace PotatoFarm.UI
{
    public class UpgradePanel : MonoBehaviour
    {
        [Header("Upgrade List")]
        public Transform upgradeListParent;
        public GameObject upgradeItemPrefab;
        
        private List<GameObject> upgradeItems = new List<GameObject>();

        private void Start()
        {
            RefreshUpgradeList();
            
            // Subscribe to events
            if (GameManager.Instance?.upgradeManager != null)
            {
                GameManager.Instance.upgradeManager.OnUpgradePurchased += OnUpgradePurchased;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance?.upgradeManager != null)
            {
                GameManager.Instance.upgradeManager.OnUpgradePurchased -= OnUpgradePurchased;
            }
        }

        private void OnUpgradePurchased(int upgradeIndex)
        {
            RefreshUpgradeList();
        }

        private void RefreshUpgradeList()
        {
            if (GameManager.Instance?.upgradeManager == null) return;

            // Clear existing items
            foreach (var item in upgradeItems)
            {
                if (item != null)
                    Destroy(item);
            }
            upgradeItems.Clear();

            var upgradeManager = GameManager.Instance.upgradeManager;

            for (int i = 0; i < upgradeManager.GetUpgradeCount(); i++)
            {
                var upgrade = upgradeManager.GetUpgrade(i);
                if (upgrade == null) continue;

                CreateUpgradeItem(upgrade, i);
            }
        }

        private void CreateUpgradeItem(Upgrade upgrade, int index)
        {
            GameObject item = new GameObject($"Upgrade_{index}");
            item.transform.SetParent(upgradeListParent, false);
            upgradeItems.Add(item);

            // Add layout components
            var layoutElement = item.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 120;

            var horizontalLayout = item.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = false;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = false;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 10;
            horizontalLayout.padding = new RectOffset(10, 10, 10, 10);

            // Upgrade info
            var infoPanel = new GameObject("Info");
            infoPanel.transform.SetParent(item.transform, false);
            var infoLayout = infoPanel.AddComponent<LayoutElement>();
            infoLayout.preferredWidth = 300;

            var infoVertical = infoPanel.AddComponent<VerticalLayoutGroup>();
            infoVertical.childControlWidth = true;
            infoVertical.childControlHeight = false;
            infoVertical.childForceExpandWidth = true;
            infoVertical.spacing = 2;

            // Upgrade name
            var nameText = CreateText(infoPanel, upgrade.name);
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;

            // Upgrade description
            var descText = CreateText(infoPanel, upgrade.description);
            descText.fontSize = 12;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // Upgrade level and effect
            var levelText = CreateText(infoPanel, $"Level: {upgrade.level}");
            var effectText = CreateText(infoPanel, $"Effect: {upgrade.GetCurrentEffect():F1}x");

            // Cost and button panel
            var actionPanel = new GameObject("Actions");
            actionPanel.transform.SetParent(item.transform);
            var actionLayout = actionPanel.AddComponent<LayoutElement>();
            actionLayout.preferredWidth = 150;

            var actionVertical = actionPanel.AddComponent<VerticalLayoutGroup>();
            actionVertical.childControlWidth = true;
            actionVertical.childControlHeight = false;
            actionVertical.childForceExpandWidth = true;
            actionVertical.spacing = 5;

            // Cost display
            var costText = CreateText(actionPanel, $"Cost: ${upgrade.GetCurrentCost():F0}");
            costText.fontSize = 14;
            costText.fontStyle = FontStyles.Bold;

            // Purchase button
            bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, upgrade.GetCurrentCost());
            var purchaseButton = CreateButton(actionPanel, upgrade.isMaxed ? "MAXED" : "BUY");
            
            if (upgrade.isMaxed)
            {
                purchaseButton.interactable = false;
                purchaseButton.GetComponent<Image>().color = Color.gray;
            }
            else if (!canAfford)
            {
                purchaseButton.interactable = false;
                purchaseButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                purchaseButton.interactable = true;
                purchaseButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);
                purchaseButton.onClick.AddListener(() => GameManager.Instance.upgradeManager.PurchaseUpgrade(index));
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
            buttonLayout.preferredHeight = 40;

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
            // Refresh upgrade affordability periodically
            if (Time.frameCount % 30 == 0) // Every 30 frames
            {
                UpdateUpgradeAffordability();
            }
        }

        private void UpdateUpgradeAffordability()
        {
            if (GameManager.Instance?.upgradeManager == null || GameManager.Instance?.resourceManager == null) return;

            for (int i = 0; i < upgradeItems.Count; i++)
            {
                if (upgradeItems[i] == null) continue;

                var upgrade = GameManager.Instance.upgradeManager.GetUpgrade(i);
                if (upgrade == null) continue;

                var button = upgradeItems[i].GetComponentInChildren<Button>();
                if (button == null) continue;

                if (upgrade.isMaxed)
                {
                    button.interactable = false;
                    button.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, upgrade.GetCurrentCost());
                    button.interactable = canAfford;
                    button.GetComponent<Image>().color = canAfford ? 
                        new Color(0.2f, 0.8f, 0.2f, 1f) : 
                        Color.red;
                }
            }
        }
    }
}