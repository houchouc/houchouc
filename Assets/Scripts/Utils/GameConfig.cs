using UnityEngine;

// Game configuration constants as per GDD
public static class GameConfig
{
    // Economy
    public const double BASE_SALE_VALUE = 5.0;
    public const float VALUE_UPGRADE_MULTIPLIER = 0.25f;
    public const float COMBO_RATE_EARLY = 0.02f;
    public const float COMBO_CAP_EARLY = 0.50f;
    public const float COMBO_TIMEOUT = 3f;

    // Offline
    public const float OFFLINE_EFFICIENCY_EARLY = 0.25f;
    public const int OFFLINE_CAP_MINUTES_EARLY = 120;

    // Truck
    public const float BASE_TRUCK_SPEED = 1f;
    public const float BASE_SPEED_UPGRADE_BONUS = 0.05f;
    public const float BASE_SELL_RANGE = 120f;
    public const float SELL_RANGE_UPGRADE_BONUS = 5f;

    // Customer Spawning
    public const float BASE_SPAWN_INTERVAL = 2.5f;
    public const float MIN_SPAWN_INTERVAL = 0.75f;
    public const float SPAWN_REDUCTION_PER_LEVEL = 0.04f;

    // Customer Patience
    public const float NORMAL_PATIENCE = 5f;
    public const float IMPATIENT_PATIENCE = 3f;
    public const float IMPATIENT_SPAWN_CHANCE = 0.30f;

    // VIP
    public const float VIP_BASE_CHANCE = 0.01f;
    public const double VIP_MULTIPLIER = 10.0;

    // Unlock Goals (GDD 14.15)
    public const double SPRINKLE_BOMB_UNLOCK_CASH = 750;
    public const int SPRINKLE_BOMB_UNLOCK_CUSTOMERS = 50;

    // First Session Pacing (GDD 9.16)
    public const double FIRST_UPGRADE_COST = 25;
    public const float FIRST_UPGRADE_TIME_TARGET = 35f;
    public const float FIRST_COMBO_TIME_TARGET = 120f;
    public const float FIVE_MINUTE_SURVIVAL_TARGET = 300f;

    // Upgrade Costs (GDD 14.13)
    public const double BIGGER_CONE_BASE_COST = 25;
    public const double FASTER_SCOOP_BASE_COST = 50;
    public const double LOUDER_BELL_BASE_COST = 75;
    public const double WIDER_WINDOW_BASE_COST = 100;
    public const double COMBO_CUP_BASE_COST = 150;
    public const double AUTO_BELL_BASE_COST = 500;

    // Upgrade Growth Rates
    public const float BIGGER_CONE_GROWTH = 1.18f;
    public const float FASTER_SCOOP_GROWTH = 1.20f;
    public const float LOUDER_BELL_GROWTH = 1.20f;
    public const float WIDER_WINDOW_GROWTH = 1.22f;
    public const float COMBO_CUP_GROWTH = 1.25f;

    // Retention Targets (GDD 18.2)
    public const float D1_RETENTION_TARGET = 0.40f; // 40%
    public const float D7_RETENTION_TARGET = 0.18f; // 18%
    public const float FIRST_SESSION_LENGTH_MIN = 360f; // 6 minutes
    public const float FIRST_SESSION_LENGTH_MAX = 600f; // 10 minutes
}
