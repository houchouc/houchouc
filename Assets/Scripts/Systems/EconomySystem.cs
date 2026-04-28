using UnityEngine;
using System;
using System.Collections.Generic;

public class EconomySystem : MonoBehaviour
{
    private double cash = 0;
    private double lifetimeCash = 0;
    private int comboCount = 0;
    private float lastSaleTime = -5f;
    private int consecutiveMisses = 0;

    private long lastOfflineClaimTime = 0;
    private float averageCashPerMinute = 0;
    private float firstSaleTime = -1f;

    public event Action<double> OnCashChanged;
    public event Action<int> OnComboChanged;
    public event Action<double> OnLifetimeCashChanged;
    public event Action<double, bool> OnSale; // (amount, isVIP)

    public void Initialize()
    {
        lastOfflineClaimTime = System.DateTime.Now.Ticks;
    }

    public void ProcessSale(Vector3 salePosition = default, bool isVIP = false)
    {
        double saleValue = CalculateSaleValue(isVIP);
        AddCash(saleValue);
        UpdateCombo();
        OnSale?.Invoke(saleValue, isVIP);

        // Show visual feedback
        if (salePosition != Vector3.zero && VisualEffectsManager.Instance != null)
        {
            VisualEffectsManager.Instance.ShowCashPopup(salePosition, saleValue, GetComboCount() > 1, isVIP);
        }

        // Track first sale
        if (firstSaleTime < 0)
        {
            firstSaleTime = Time.time;
            GameManager.Instance.Analytics.TrackEvent("first_sale", new() {
                { "time_since_start", Time.time.ToString("F1") }
            });
        }

        GameManager.Instance.Analytics.TrackEvent("customer_tap", new() {
            { "type", isVIP ? "vip" : "normal" },
            { "success", "true" }
        });
    }

    public void ProcessMiss()
    {
        comboCount = Mathf.Max(0, comboCount - 1);
        OnComboChanged?.Invoke(comboCount);

        GameManager.Instance.Analytics.TrackEvent("customer_missed", new());
    }

    private double CalculateSaleValue(bool isVIP)
    {
        double baseValue = GameConfig.BASE_SALE_VALUE;
        double valueUpgradeBonus = 1.0 + (GameManager.Instance.Upgrades.GetUpgradeLevel("bigger_cone") * GameConfig.VALUE_UPGRADE_MULTIPLIER);
        double comboMultiplier = GetComboMultiplier();
        double vipMultiplier = isVIP ? GameConfig.VIP_MULTIPLIER : 1.0;

        double total = baseValue * valueUpgradeBonus * comboMultiplier * vipMultiplier;
        return System.Math.Round(total);
    }

    private void UpdateCombo()
    {
        lastSaleTime = Time.time;
        comboCount++;
        consecutiveMisses = 0;
        OnComboChanged?.Invoke(comboCount);
    }

    public float GetComboMultiplier()
    {
        if (Time.time - lastSaleTime > GameConfig.COMBO_TIMEOUT)
        {
            comboCount = 0;
            return 1f;
        }

        return (float)(1.0 + System.Math.Min(comboCount * GameConfig.COMBO_RATE_EARLY, GameConfig.COMBO_CAP_EARLY));
    }

    public int GetComboCount()
    {
        if (Time.time - lastSaleTime > COMBO_TIMEOUT)
        {
            comboCount = 0;
        }
        return comboCount;
    }

    public void AddCash(double amount)
    {
        cash += amount;
        lifetimeCash += amount;
        OnCashChanged?.Invoke(cash);
        OnLifetimeCashChanged?.Invoke(lifetimeCash);

        // Update average cash per minute for offline calculations
        averageCashPerMinute = (float)(lifetimeCash / ((Time.time / 60f) + 0.1f));
    }

    public bool TrySpendCash(double amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            OnCashChanged?.Invoke(cash);
            return true;
        }
        return false;
    }

    public double GetCash() => cash;
    public double GetLifetimeCash() => lifetimeCash;
    public int GetComboCount() => comboCount;

    public void ProcessOfflineEarnings()
    {
        long nowTicks = System.DateTime.Now.Ticks;
        long ticksDiff = nowTicks - lastOfflineClaimTime;
        double minutesOffline = new System.TimeSpan(ticksDiff).TotalMinutes;

        if (minutesOffline < 0.5) return; // Ignore very short gaps

        double baselineIncome = averageCashPerMinute > 0 ? averageCashPerMinute : GameConfig.BASE_SALE_VALUE;
        int minutesCapped = Mathf.Min((int)minutesOffline, GameConfig.OFFLINE_CAP_MINUTES_EARLY);
        double offlineEarnings = baselineIncome * minutesCapped * GameConfig.OFFLINE_EFFICIENCY_EARLY;

        AddCash(offlineEarnings);

        GameManager.Instance.Analytics.TrackEvent("offline_claim", new() {
            { "amount", offlineEarnings.ToString("F0") },
            { "minutes", minutesCapped.ToString() }
        });

        lastOfflineClaimTime = nowTicks;
    }

    public double GetOfflineEarningsEstimate(int minutes)
    {
        double baselineIncome = averageCashPerMinute > 0 ? averageCashPerMinute : GameConfig.BASE_SALE_VALUE;
        int minutesCapped = Mathf.Min(minutes, GameConfig.OFFLINE_CAP_MINUTES_EARLY);
        return baselineIncome * minutesCapped * GameConfig.OFFLINE_EFFICIENCY_EARLY;
    }

    public void SetCash(double amount)
    {
        cash = amount;
        OnCashChanged?.Invoke(cash);
    }

    public void SetLifetimeCash(double amount)
    {
        lifetimeCash = amount;
        OnLifetimeCashChanged?.Invoke(lifetimeCash);
    }

    public void ResetCombo()
    {
        comboCount = 0;
        OnComboChanged?.Invoke(0);
    }
}
