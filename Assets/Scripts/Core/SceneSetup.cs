using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneSetup : MonoBehaviour
{
    // This script sets up the entire MVP scene programmatically
    // You can assign it to any GameObject in the scene

    private void Awake()
    {
        SetupScene();
    }

    private void SetupScene()
    {
        // Create Game Manager
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManagerObj.AddComponent<EconomySystem>();
        gameManagerObj.AddComponent<TruckSystem>();
        gameManagerObj.AddComponent<CustomerSystem>();
        gameManagerObj.AddComponent<UpgradeSystem>();
        gameManagerObj.AddComponent<SaveSystem>();
        gameManagerObj.AddComponent<AnalyticsManager>();
        gameManagerObj.AddComponent<AudioManager>();
        gameManagerObj.AddComponent<TutorialManager>();

        // Create Canvas for UI
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        // Create UI Manager
        UIManager uiManager = canvasObj.AddComponent<UIManager>();

        // Create HUD elements on canvas
        CreateHUDElements(canvasObj.transform, uiManager);

        // Create Road with Truck
        GameObject roadObj = new GameObject("Road");
        roadObj.transform.SetParent(null);

        GameObject truckObj = new GameObject("Truck");
        truckObj.transform.SetParent(roadObj.transform);
        truckObj.transform.localPosition = Vector3.zero;
        SpriteRenderer truckSprite = truckObj.AddComponent<SpriteRenderer>();
        truckSprite.color = Color.white; // Will be replaced with actual sprite
        truckSprite.sprite = CreatePlaceholderSprite(64, 32);

        // Assign truck to TruckSystem
        TruckSystem truckSystem = gameManagerObj.GetComponent<TruckSystem>();
        truckSystem.GetType().GetField("truckTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(truckSystem, truckObj.transform);
        truckSystem.GetType().GetField("truckVisuals", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(truckSystem, truckSprite);

        // Create Input Handler
        gameManagerObj.AddComponent<InputHandler>();

        Debug.Log("[Setup] MVP scene created successfully");
    }

    private void CreateHUDElements(Transform canvasTransform, UIManager uiManager)
    {
        // Cash Text
        GameObject cashTextObj = new GameObject("CashText");
        cashTextObj.transform.SetParent(canvasTransform);
        RectTransform cashRect = cashTextObj.AddComponent<RectTransform>();
        cashRect.anchorMin = new Vector2(0, 1);
        cashRect.anchorMax = new Vector2(0, 1);
        cashRect.offsetMin = new Vector2(20, -60);
        cashRect.offsetMax = new Vector2(300, -20);
        TextMeshProUGUI cashText = cashTextObj.AddComponent<TextMeshProUGUI>();
        cashText.text = "$0";
        cashText.fontSize = 36;

        // Combo Text
        GameObject comboTextObj = new GameObject("ComboText");
        comboTextObj.transform.SetParent(canvasTransform);
        RectTransform comboRect = comboTextObj.AddComponent<RectTransform>();
        comboRect.anchorMin = new Vector2(0.5f, 1);
        comboRect.anchorMax = new Vector2(0.5f, 1);
        comboRect.offsetMin = new Vector2(-50, -60);
        comboRect.offsetMax = new Vector2(50, -20);
        TextMeshProUGUI comboText = comboTextObj.AddComponent<TextMeshProUGUI>();
        comboText.text = "x1";
        comboText.fontSize = 32;
        comboText.alignment = TextAlignmentOptions.Center;

        // Unlock Progress Text
        GameObject unlockTextObj = new GameObject("UnlockText");
        unlockTextObj.transform.SetParent(canvasTransform);
        RectTransform unlockRect = unlockTextObj.AddComponent<RectTransform>();
        unlockRect.anchorMin = new Vector2(1, 1);
        unlockRect.anchorMax = new Vector2(1, 1);
        unlockRect.offsetMin = new Vector2(-300, -60);
        unlockRect.offsetMax = new Vector2(-20, -20);
        TextMeshProUGUI unlockText = unlockTextObj.AddComponent<TextMeshProUGUI>();
        unlockText.text = "Sprinkle Bomb: 0%";
        unlockText.fontSize = 24;
        unlockText.alignment = TextAlignmentOptions.TopRight;

        // Progress Bar
        GameObject progressBarObj = new GameObject("ProgressBar");
        progressBarObj.transform.SetParent(canvasTransform);
        RectTransform progressRect = progressBarObj.AddComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(1, 1);
        progressRect.anchorMax = new Vector2(1, 1);
        progressRect.offsetMin = new Vector2(-300, -85);
        progressRect.offsetMax = new Vector2(-20, -75);
        Image progressImage = progressBarObj.AddComponent<Image>();
        progressImage.color = Color.green;

        // Upgrade Button
        GameObject upgradeButtonObj = new GameObject("UpgradeButton");
        upgradeButtonObj.transform.SetParent(canvasTransform);
        RectTransform upgradeButtonRect = upgradeButtonObj.AddComponent<RectTransform>();
        upgradeButtonRect.anchorMin = new Vector2(1, 0);
        upgradeButtonRect.anchorMax = new Vector2(1, 0);
        upgradeButtonRect.offsetMin = new Vector2(-300, 20);
        upgradeButtonRect.offsetMax = new Vector2(-20, 120);

        Image upgradeButtonImage = upgradeButtonObj.AddComponent<Image>();
        upgradeButtonImage.color = Color.cyan;

        UnityEngine.UI.Button upgradeButton = upgradeButtonObj.AddComponent<UnityEngine.UI.Button>();
        upgradeButton.targetGraphic = upgradeButtonImage;

        // Upgrade Button Text
        GameObject upgradeTextObj = new GameObject("Text");
        upgradeTextObj.transform.SetParent(upgradeButtonObj.transform);
        RectTransform upgradeTextRect = upgradeTextObj.AddComponent<RectTransform>();
        upgradeTextRect.offsetMin = Vector2.zero;
        upgradeTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI upgradeText = upgradeTextObj.AddComponent<TextMeshProUGUI>();
        upgradeText.text = "Bigger Cone\n$25";
        upgradeText.fontSize = 20;
        upgradeText.alignment = TextAlignmentOptions.Center;

        // Assign to UIManager
        uiManager.GetType().GetField("cashText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, cashText);
        uiManager.GetType().GetField("comboText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, comboText);
        uiManager.GetType().GetField("unlockProgressText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, unlockText);
        uiManager.GetType().GetField("progressBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, progressImage);
        uiManager.GetType().GetField("upgradeButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, upgradeButton);
        uiManager.GetType().GetField("upgradeButtonText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, upgradeText);
        uiManager.GetType().GetField("upgradeButtonCostText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(uiManager, upgradeText);
    }

    private Sprite CreatePlaceholderSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
    }
}
