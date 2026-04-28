using UnityEngine;
using System;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour
{
    [System.Serializable]
    public class AnalyticsEvent
    {
        public string eventName;
        public long timestamp;
        public Dictionary<string, string> parameters = new();
    }

    private List<AnalyticsEvent> eventQueue = new();
    private bool isInitialized = false;

    public void Initialize()
    {
        isInitialized = true;
        Debug.Log("Analytics initialized");
    }

    public void TrackEvent(string eventName, Dictionary<string, string> parameters = null)
    {
        if (!isInitialized) return;

        var analyticsEvent = new AnalyticsEvent
        {
            eventName = eventName,
            timestamp = System.DateTime.Now.Ticks,
            parameters = parameters ?? new()
        };

        eventQueue.Add(analyticsEvent);

        // Debug log
        string paramStr = "";
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                paramStr += $" {kvp.Key}={kvp.Value}";
            }
        }

        Debug.Log($"[Analytics] {eventName}{paramStr}");

        // In a real game, send to analytics service (Firebase, GameAnalytics, etc)
        // For MVP, just logging locally
    }

    public void Flush()
    {
        // Send queued events to analytics service
        if (eventQueue.Count > 0)
        {
            Debug.Log($"Flushing {eventQueue.Count} analytics events");
            eventQueue.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        Flush();
    }

    // Track common MVP events with helper methods
    public void TrackFirstSale(float timeToComplete)
    {
        TrackEvent("first_sale", new() { { "time_since_start", timeToComplete.ToString("F1") } });
    }

    public void TrackUpgradePurchase(string upgradeId, int level, double cost)
    {
        TrackEvent("upgrade_purchase", new() {
            { "upgrade_id", upgradeId },
            { "level", level.ToString() },
            { "cost", cost.ToString("F0") }
        });
    }

    public void TrackSessionMilestone(string milestone)
    {
        TrackEvent($"milestone_{milestone}", new() { { "time", Time.time.ToString("F1") } });
    }

    public void TrackRetention(int dayNumber, double cash, int upgrades)
    {
        TrackEvent($"d{dayNumber}_retention", new() {
            { "cash", cash.ToString("F0") },
            { "upgrades", upgrades.ToString() }
        });
    }
}
