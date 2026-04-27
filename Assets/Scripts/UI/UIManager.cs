using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI unlockProgressText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;
    [SerializeField] private TextMeshProUGUI upgradeButtonCostText;
    [SerializeField] private TextMeshProUGUI upgradeButtonEffectText;
    [SerializeField] private Image progressBar;
    [SerializeField] private CanvasGroup comboPanelGroup;
    [SerializeField] private CanvasGroup unlockPanelGroup;
    [SerializeField] private Canvas mainCanvas;

    private string currentUpgradeId = "bigger_cone";
    private int upgradeIndex = 0;
    private int maxUpgrades = 6;

    public void Initialize()
    {
        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }

        GameManager.Instance.Economy.OnCashChanged += UpdateCashDisplay;
        GameManager.Instance.Economy.OnComboChanged += UpdateComboDisplay;
        GameManager.Instance.Upgrades.OnUpgradePurchased += HandleUpgradePurchased;

        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        UpdateCashDisplay(GameManager.Instance.Economy.GetCash());
        UpdateComboDisplay(GameManager.Instance.Economy.GetComboCount());
        UpdateUpgradeDisplay();
    }

    private void UpdateCashDisplay(double cash)
    {
        if (cashText != null)
        {
            cashText.text = $"${FormatNumber(cash)}";
        }
    }

    private void UpdateComboDisplay(int combo)
    {
        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"x{combo}";
                comboText.color = Color.yellow;
                if (comboPanelGroup != null) comboPanelGroup.alpha = 1f;
            }
            else
            {
                if (comboPanelGroup != null) comboPanelGroup.alpha = 0.3f;
            }
        }
    }

    private void UpdateUnlockProgress()
    {
        double lifetime = GameManager.Instance.Economy.GetLifetimeCash();
        double targetCash = 750; // From GDD section 14.15
        double progress = Mathf.Min(1f, (float)(lifetime / targetCash));

        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        if (unlockProgressText != null)
        {
            int progressPercent = (int)(progress * 100);
            unlockProgressText.text = $"Sprinkle Bomb: {progressPercent}%";

            if (progress >= 1f)
            {
                unlockProgressText.text = "Sprinkle Bomb Unlocked!";
                unlockProgressText.color = Color.green;
            }
        }
    }

    private void UpdateUpgradeDisplay()
    {
        // Cycle through upgrades
        string[] upgradeIds = { "bigger_cone", "faster_scoop", "louder_bell", "wider_window", "combo_cup", "auto_bell" };
        currentUpgradeId = upgradeIds[upgradeIndex];

        string upgradeName = GameManager.Instance.Upgrades.GetUpgradeName(currentUpgradeId);
        double cost = GameManager.Instance.Upgrades.GetNextUpgradeCost(currentUpgradeId);
        int level = GameManager.Instance.Upgrades.GetUpgradeLevel(currentUpgradeId);

        if (upgradeButtonText != null)
        {
            upgradeButtonText.text = upgradeName;
        }

        if (upgradeButtonCostText != null)
        {
            upgradeButtonCostText.text = $"${FormatNumber(cost)}";
        }

        if (upgradeButtonEffectText != null)
        {
            upgradeButtonEffectText.text = $"Lv. {level}";
        }

        // Check if affordable
        bool canAfford = GameManager.Instance.Economy.GetCash() >= cost;
        upgradeButton.interactable = canAfford;
    }

    private void OnUpgradeButtonClicked()
    {
        if (GameManager.Instance.Upgrades.TryPurchaseUpgrade(currentUpgradeId))
        {
            GameManager.Instance.Audio.PlaySound("upgrade");
            UpdateUpgradeDisplay();

            GameManager.Instance.Analytics.TrackEvent("first_upgrade_purchased", new() {
                { "upgrade_id", currentUpgradeId },
                { "time", Time.time.ToString("F1") }
            });
        }
    }

    private void HandleUpgradePurchased(string upgradeId, int level)
    {
        UpdateUpgradeDisplay();
        UpdateUnlockProgress();

        // Tutorial events
        if (upgradeId == "bigger_cone" && level == 1)
        {
            GameManager.Instance.Analytics.TrackEvent("tutorial_complete", new() {
                { "time_to_complete", Time.time.ToString("F1") }
            });
        }
    }

    private void Update()
    {
        // Check if can afford current upgrade
        double cost = GameManager.Instance.Upgrades.GetNextUpgradeCost(currentUpgradeId);
        bool canAfford = GameManager.Instance.Economy.GetCash() >= cost;

        if (upgradeButton != null)
        {
            upgradeButton.interactable = canAfford;
        }

        UpdateUnlockProgress();
    }

    public void CycleUpgrade()
    {
        upgradeIndex = (upgradeIndex + 1) % maxUpgrades;
        UpdateUpgradeDisplay();
    }

    private string FormatNumber(double number)
    {
        if (number >= 1_000_000)
            return (number / 1_000_000).ToString("F1") + "M";
        else if (number >= 1_000)
            return (number / 1_000).ToString("F1") + "K";
        else
            return number.ToString("F0");
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Economy.OnCashChanged -= UpdateCashDisplay;
            GameManager.Instance.Economy.OnComboChanged -= UpdateComboDisplay;
            GameManager.Instance.Upgrades.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }
    }
}
