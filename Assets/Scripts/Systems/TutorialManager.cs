using UnityEngine;
using System;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState
    {
        NONE = 0,
        SHOW_CUSTOMER = 1,
        SHOW_TAP_PROMPT = 2,
        FIRST_SALE_COMPLETE = 3,
        SHOW_UPGRADE_BUTTON = 4,
        FIRST_UPGRADE_COMPLETE = 5,
        SHOW_COMBO = 6,
        SHOW_OFFLINE = 7,
        TUTORIAL_COMPLETE = 8
    }

    private TutorialState currentState = TutorialState.NONE;
    private float tutorialStartTime;
    private GameObject tutorialPrompt;
    private bool hasSeenFirstSale = false;
    private bool hasSeenFirstUpgrade = false;

    public event Action<TutorialState> OnTutorialStateChanged;

    public void Initialize()
    {
        tutorialStartTime = Time.time;
        SetTutorialState(TutorialState.SHOW_CUSTOMER);

        GameManager.Instance.Economy.OnSale += HandleSale;
        GameManager.Instance.Upgrades.OnUpgradePurchased += HandleUpgradePurchased;
    }

    public void Tick(float deltaTime)
    {
        // Tutorial state machine
        if (!hasSeenFirstSale && Time.time - tutorialStartTime > 10f)
        {
            // Timeout - move past waiting for first sale
            hasSeenFirstSale = true;
        }

        if (!hasSeenFirstUpgrade && hasSeenFirstSale && Time.time - tutorialStartTime > 20f)
        {
            // Timeout - move past waiting for first upgrade
            hasSeenFirstUpgrade = true;
        }
    }

    private void HandleSale(double amount, bool isVIP)
    {
        if (currentState == TutorialState.SHOW_TAP_PROMPT && !hasSeenFirstSale)
        {
            hasSeenFirstSale = true;
            SetTutorialState(TutorialState.FIRST_SALE_COMPLETE);
            GameManager.Instance.Analytics.TrackEvent("first_customer_tapped", new() {
                { "time_since_start", (Time.time - tutorialStartTime).ToString("F1") }
            });
        }
    }

    private void HandleUpgradePurchased(string upgradeId, int level)
    {
        if (currentState == TutorialState.SHOW_UPGRADE_BUTTON && !hasSeenFirstUpgrade)
        {
            hasSeenFirstUpgrade = true;
            SetTutorialState(TutorialState.FIRST_UPGRADE_COMPLETE);
            GameManager.Instance.Analytics.TrackEvent("first_upgrade_purchased", new() {
                { "upgrade_id", upgradeId },
                { "time", (Time.time - tutorialStartTime).ToString("F1") }
            });

            // Check if reached 5-minute milestone
            if (Time.time - tutorialStartTime > 300f)
            {
                GameManager.Instance.Analytics.TrackEvent("five_minute_session", new());
                SetTutorialState(TutorialState.TUTORIAL_COMPLETE);
            }
        }
    }

    private void SetTutorialState(TutorialState newState)
    {
        if (newState == currentState) return;

        currentState = newState;
        OnTutorialStateChanged?.Invoke(currentState);

        switch (newState)
        {
            case TutorialState.SHOW_CUSTOMER:
                Debug.Log("[Tutorial] Showing customer");
                break;
            case TutorialState.SHOW_TAP_PROMPT:
                Debug.Log("[Tutorial] Showing tap prompt");
                break;
            case TutorialState.FIRST_SALE_COMPLETE:
                Debug.Log("[Tutorial] First sale complete");
                break;
            case TutorialState.SHOW_UPGRADE_BUTTON:
                Debug.Log("[Tutorial] Showing upgrade button");
                break;
            case TutorialState.FIRST_UPGRADE_COMPLETE:
                Debug.Log("[Tutorial] First upgrade complete");
                break;
            case TutorialState.SHOW_COMBO:
                Debug.Log("[Tutorial] Showing combo");
                break;
            case TutorialState.TUTORIAL_COMPLETE:
                Debug.Log("[Tutorial] Tutorial complete!");
                break;
        }
    }

    public TutorialState GetTutorialStateEnum() => currentState;

    public int GetTutorialState() => (int)currentState;

    public void SetTutorialState(int state)
    {
        currentState = (TutorialState)state;
    }

    public bool IsTutorialComplete() => currentState == TutorialState.TUTORIAL_COMPLETE;

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Economy.OnSale -= HandleSale;
            GameManager.Instance.Upgrades.OnUpgradePurchased -= HandleUpgradePurchased;
        }
    }
}
