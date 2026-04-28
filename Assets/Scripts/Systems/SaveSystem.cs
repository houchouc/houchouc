using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class GameSaveData
    {
        public double cash;
        public double lifetimeCash;
        public Dictionary<string, int> upgradeLevels = new();
        public long lastSessionTick;
        public int tutorialState;
    }

    private const string SAVE_KEY = "snack_truck_save";
    private GameSaveData currentSaveData;

    public void SaveGame()
    {
        if (GameManager.Instance == null) return;

        currentSaveData = new GameSaveData
        {
            cash = GameManager.Instance.Economy.GetCash(),
            lifetimeCash = GameManager.Instance.Economy.GetLifetimeCash(),
            upgradeLevels = GameManager.Instance.Upgrades.GetUpgradeLevels(),
            lastSessionTick = System.DateTime.Now.Ticks,
            tutorialState = GameManager.Instance.Tutorial.GetTutorialState()
        };

        string json = JsonUtility.ToJson(currentSaveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Game saved: " + json);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save file found, starting fresh");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        currentSaveData = JsonUtility.FromJson<GameSaveData>(json);

        if (GameManager.Instance == null) return;

        // Load economy
        GameManager.Instance.Economy.SetCash(currentSaveData.cash);
        GameManager.Instance.Economy.SetLifetimeCash(currentSaveData.lifetimeCash);

        // Load upgrades
        foreach (var kvp in currentSaveData.upgradeLevels)
        {
            GameManager.Instance.Upgrades.SetUpgradeLevel(kvp.Key, kvp.Value);
        }

        // Load tutorial state
        GameManager.Instance.Tutorial.SetTutorialState(currentSaveData.tutorialState);

        Debug.Log("Game loaded: " + json);
    }

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
    }

    public GameSaveData GetCurrentSaveData()
    {
        return currentSaveData;
    }
}
