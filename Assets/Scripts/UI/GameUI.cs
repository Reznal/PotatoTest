using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PotatoFarm.Core;

namespace PotatoFarm.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Resource Display")]
        public TextMeshProUGUI potatoesText;
        public TextMeshProUGUI cashText;
        public TextMeshProUGUI starchText;
        public TextMeshProUGUI goldenPotatoesText;
        
        [Header("Production Display")]
        public TextMeshProUGUI potatoRateText;
        public TextMeshProUGUI cashRateText;
        
        [Header("Tap Button")]
        public Button tapButton;
        public TextMeshProUGUI tapPowerText;
        
        [Header("Prestige")]
        public Button prestigeButton;
        public TextMeshProUGUI prestigeText;
        
        [Header("Panels")]
        public GameObject farmPanel;
        public GameObject upgradePanel;
        public GameObject prestigePanel;
        public GameObject processingPanel;
        
        [Header("Panel Buttons")]
        public Button farmPanelButton;
        public Button upgradePanelButton;
        public Button prestigePanelButton;
        public Button processingPanelButton;
        
        private void Start()
        {
            // Subscribe to events
            if (GameManager.Instance != null && GameManager.Instance.resourceManager != null)
            {
                GameManager.Instance.resourceManager.OnResourceChanged += UpdateResourceDisplay;
                GameManager.Instance.resourceManager.OnResourceRateChanged += UpdateRateDisplay;
            }
            
            if (GameManager.Instance != null && GameManager.Instance.farmManager != null)
            {
                GameManager.Instance.farmManager.OnPotatoHarvested += OnPotatoHarvested;
            }
            
            // Setup button listeners
            if (tapButton != null)
                tapButton.onClick.AddListener(OnTapButtonClicked);
            
            if (prestigeButton != null)
                prestigeButton.onClick.AddListener(OnPrestigeButtonClicked);
            
            if (farmPanelButton != null)
                farmPanelButton.onClick.AddListener(() => ShowPanel(farmPanel));
            
            if (upgradePanelButton != null)
                upgradePanelButton.onClick.AddListener(() => ShowPanel(upgradePanel));
            
            if (prestigePanelButton != null)
                prestigePanelButton.onClick.AddListener(() => ShowPanel(prestigePanel));
            
            if (processingPanelButton != null)
                processingPanelButton.onClick.AddListener(() => ShowPanel(processingPanel));
            
            // Show farm panel by default
            ShowPanel(farmPanel);
            
            // Initial update
            UpdateAllDisplays();
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null && GameManager.Instance.resourceManager != null)
            {
                GameManager.Instance.resourceManager.OnResourceChanged -= UpdateResourceDisplay;
                GameManager.Instance.resourceManager.OnResourceRateChanged -= UpdateRateDisplay;
            }
            
            if (GameManager.Instance != null && GameManager.Instance.farmManager != null)
            {
                GameManager.Instance.farmManager.OnPotatoHarvested -= OnPotatoHarvested;
            }
        }
        
        private void Update()
        {
            UpdateAllDisplays();
        }
        
        private void UpdateAllDisplays()
        {
            UpdateResourceDisplays();
            UpdateRateDisplays();
            UpdateTapDisplay();
            UpdatePrestigeDisplay();
        }
        
        private void UpdateResourceDisplays()
        {
            if (GameManager.Instance?.resourceManager == null) return;
            
            var rm = GameManager.Instance.resourceManager;
            
            if (potatoesText != null)
                potatoesText.text = $"Potatoes: {FormatNumber(rm.GetResource(ResourceType.Potatoes))}";
            
            if (cashText != null)
                cashText.text = $"Cash: ${FormatNumber(rm.GetResource(ResourceType.Cash))}";
            
            if (starchText != null)
                starchText.text = $"Starch: {FormatNumber(rm.GetResource(ResourceType.Starch))}";
            
            if (goldenPotatoesText != null)
                goldenPotatoesText.text = $"Golden: {FormatNumber(rm.GetResource(ResourceType.GoldenPotatoes))}";
        }
        
        private void UpdateRateDisplays()
        {
            if (GameManager.Instance?.resourceManager == null) return;
            
            var rm = GameManager.Instance.resourceManager;
            
            if (potatoRateText != null)
                potatoRateText.text = $"{FormatNumber(rm.GetResourceRate(ResourceType.Potatoes))}/sec";
            
            if (cashRateText != null)
                cashRateText.text = $"${FormatNumber(rm.GetResourceRate(ResourceType.Cash))}/sec";
        }
        
        private void UpdateTapDisplay()
        {
            if (GameManager.Instance?.farmManager == null) return;
            
            var fm = GameManager.Instance.farmManager;
            double tapPower = fm.tapPower * fm.clickMultiplier;
            
            if (GameManager.Instance.prestigeManager != null)
            {
                tapPower *= GameManager.Instance.prestigeManager.GetTotalMultiplier();
            }
            
            if (tapPowerText != null)
                tapPowerText.text = $"Tap Power: {FormatNumber(tapPower)}";
        }
        
        private void UpdatePrestigeDisplay()
        {
            if (GameManager.Instance?.prestigeManager == null) return;
            
            var pm = GameManager.Instance.prestigeManager;
            bool canPrestige = pm.CanPrestige();
            double starchGain = pm.CalculateStarchGain();
            
            if (prestigeButton != null)
                prestigeButton.interactable = canPrestige;
            
            if (prestigeText != null)
            {
                if (canPrestige)
                    prestigeText.text = $"Prestige (+{FormatNumber(starchGain)} Starch)";
                else
                    prestigeText.text = "Prestige (Need 1M Potatoes)";
            }
        }
        
        private void UpdateResourceDisplay(ResourceType type, double amount)
        {
            // This will be called automatically through the Update loop
        }
        
        private void UpdateRateDisplay(ResourceType type, double rate)
        {
            // This will be called automatically through the Update loop
        }
        
        private void OnPotatoHarvested(double amount)
        {
            // Could add visual effects here
        }
        
        private void OnTapButtonClicked()
        {
            if (GameManager.Instance?.farmManager != null)
            {
                GameManager.Instance.farmManager.TapFarm();
            }
        }
        
        private void OnPrestigeButtonClicked()
        {
            if (GameManager.Instance?.prestigeManager != null)
            {
                GameManager.Instance.prestigeManager.PerformPrestige();
            }
        }
        
        private void ShowPanel(GameObject panel)
        {
            // Hide all panels
            if (farmPanel != null) farmPanel.SetActive(false);
            if (upgradePanel != null) upgradePanel.SetActive(false);
            if (prestigePanel != null) prestigePanel.SetActive(false);
            if (processingPanel != null) processingPanel.SetActive(false);
            
            // Show selected panel
            if (panel != null) panel.SetActive(true);
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