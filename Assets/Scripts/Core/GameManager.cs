using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float gameSpeed = 1f;

    public static GameManager Instance { get; private set; }

    public EconomySystem Economy { get; private set; }
    public TruckSystem Truck { get; private set; }
    public CustomerSystem Customers { get; private set; }
    public UpgradeSystem Upgrades { get; private set; }
    public UIManager UI { get; private set; }
    public SaveSystem Save { get; private set; }
    public AnalyticsManager Analytics { get; private set; }
    public AudioManager Audio { get; private set; }
    public TutorialManager Tutorial { get; private set; }

    public event Action OnGameInitialized;

    private bool isInitialized = false;
    private float sessionStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGameSystems();
        sessionStartTime = Time.time;
    }

    private void InitializeGameSystems()
    {
        Analytics = GetComponent<AnalyticsManager>();
        if (Analytics == null) Analytics = gameObject.AddComponent<AnalyticsManager>();
        Analytics.Initialize();

        Save = GetComponent<SaveSystem>();
        if (Save == null) Save = gameObject.AddComponent<SaveSystem>();

        Economy = GetComponent<EconomySystem>();
        if (Economy == null) Economy = gameObject.AddComponent<EconomySystem>();

        Truck = GetComponent<TruckSystem>();
        if (Truck == null) Truck = gameObject.AddComponent<TruckSystem>();

        Customers = GetComponent<CustomerSystem>();
        if (Customers == null) Customers = gameObject.AddComponent<CustomerSystem>();

        Upgrades = GetComponent<UpgradeSystem>();
        if (Upgrades == null) Upgrades = gameObject.AddComponent<UpgradeSystem>();

        Audio = GetComponent<AudioManager>();
        if (Audio == null) Audio = gameObject.AddComponent<AudioManager>();

        UI = FindObjectOfType<UIManager>();
        if (UI == null) Debug.LogError("UIManager not found in scene!");

        Tutorial = GetComponent<TutorialManager>();
        if (Tutorial == null) Tutorial = gameObject.AddComponent<TutorialManager>();

        // Load saved data
        Save.LoadGame();

        // Initialize systems
        Economy.Initialize();
        Truck.Initialize();
        Customers.Initialize();
        Upgrades.Initialize();
        if (UI != null) UI.Initialize();
        Tutorial.Initialize();

        isInitialized = true;
        OnGameInitialized?.Invoke();

        Analytics.TrackEvent("tutorial_start", new() { { "session_id", SystemInfo.deviceUniqueIdentifier } });
    }

    private void Update()
    {
        if (!isInitialized) return;

        Truck.Tick(Time.deltaTime * gameSpeed);
        Customers.Tick(Time.deltaTime * gameSpeed);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save.SaveGame();
            Analytics.TrackEvent("session_end", new() { { "length", Time.time - sessionStartTime } });
        }
        else
        {
            Economy.ProcessOfflineEarnings();
            sessionStartTime = Time.time;
        }
    }

    private void OnApplicationQuit()
    {
        Save.SaveGame();
        Analytics.TrackEvent("session_end", new() { { "length", Time.time - sessionStartTime } });
    }

    public void PauseGame()
    {
        gameSpeed = 0f;
    }

    public void ResumeGame()
    {
        gameSpeed = 1f;
    }
}
