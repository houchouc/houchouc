using UnityEngine;
using System;

public class TruckSystem : MonoBehaviour
{
    [SerializeField] private Transform truckTransform;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float baseSpeedUpgradeBonus = 0.05f;
    [SerializeField] private float baseSellRange = 120f;
    [SerializeField] private float sellRangeUpgradeBonus = 5f;
    [SerializeField] private float roadLeftBound = -300f;
    [SerializeField] private float roadRightBound = 300f;
    [SerializeField] private SpriteRenderer truckVisuals;
    [SerializeField] private CircleCollider2D sellRangeCollider;
    [SerializeField] private GameObject sellRangeIndicator;

    private float currentSpeed;
    private float currentSellRange;
    private float horizontalVelocity = 1f;
    private bool isInitialized = false;

    public event Action OnTruckUpgraded;

    public void Initialize()
    {
        if (truckTransform == null)
            truckTransform = GetComponentInChildren<SpriteRenderer>()?.transform;

        if (sellRangeCollider == null)
            sellRangeCollider = GetComponent<CircleCollider2D>();

        UpdateTruckStats();
        isInitialized = true;
    }

    public void Tick(float deltaTime)
    {
        if (!isInitialized) return;

        MoveTruck(deltaTime);
    }

    private void MoveTruck(float deltaTime)
    {
        Vector3 pos = truckTransform.position;
        pos.x += horizontalVelocity * currentSpeed * deltaTime;

        // Loop truck around
        if (pos.x > roadRightBound)
        {
            pos.x = roadLeftBound;
        }
        else if (pos.x < roadLeftBound)
        {
            pos.x = roadRightBound;
        }

        truckTransform.position = pos;

        // Flip sprite based on direction
        if (truckVisuals != null)
        {
            truckVisuals.flipX = horizontalVelocity < 0;
        }
    }

    public void UpdateTruckStats()
    {
        int speedLevel = GameManager.Instance.Upgrades.GetUpgradeLevel("turbo_tires");
        currentSpeed = baseSpeed * (1f + (speedLevel * baseSpeedUpgradeBonus));

        int rangeLevel = GameManager.Instance.Upgrades.GetUpgradeLevel("wider_window");
        currentSellRange = baseSellRange + (rangeLevel * sellRangeUpgradeBonus);

        if (sellRangeCollider != null)
        {
            sellRangeCollider.radius = currentSellRange / 100f;
        }

        OnTruckUpgraded?.Invoke();
    }

    public Vector3 GetTruckPosition() => truckTransform.position;
    public float GetSellRange() => currentSellRange;
    public float GetCurrentSpeed() => currentSpeed;

    public bool IsCustomerInRange(Vector3 customerPos)
    {
        float distance = Vector3.Distance(truckTransform.position, customerPos);
        return distance <= currentSellRange;
    }

    public void SetUpgradeLevel(string upgradeId, int level)
    {
        // Called by upgrade system
        UpdateTruckStats();
    }
}
