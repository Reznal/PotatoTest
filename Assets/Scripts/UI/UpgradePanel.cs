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

            // Add background to show it's a separate item
            var backgroundImage = item.AddComponent<Image>();
            backgroundImage.color = new Color(0.15f, 0.25f, 0.15f, 0.8f); // Dark green background for upgrades

            // The GridLayoutGroup on the parent will handle sizing (350x250)
            // Use vertical layout for internal organization
            var verticalLayout = item.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childControlWidth = true;
            verticalLayout.childControlHeight = false;
            verticalLayout.childForceExpandWidth = true;
            verticalLayout.spacing = 5;
            verticalLayout.padding = new RectOffset(12, 12, 12, 12);

            // Upgrade name
            var nameText = CreateText(item, upgrade.name);
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;

            // Upgrade description
            var descText = CreateText(item, upgrade.description);
            descText.fontSize = 11;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // Upgrade level and effect
            var levelText = CreateText(item, $"Level: {upgrade.level}");
            var effectText = CreateText(item, $"Effect: {upgrade.GetCurrentEffect():F1}x");

            // Cost display
            var costText = CreateText(item, $"Cost: ${upgrade.GetCurrentCost():F0}");
            costText.fontSize = 14;
            costText.fontStyle = FontStyles.Bold;
            costText.color = Color.yellow;

            // Purchase button
            bool canAfford = GameManager.Instance.resourceManager.CanAfford(ResourceType.Cash, upgrade.GetCurrentCost());
            var purchaseButton = CreateButton(item, upgrade.isMaxed ? "MAXED" : "BUY");
            
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