using UnityEngine;
using System.Collections;

public class VisualEffectsManager : MonoBehaviour
{
    public static VisualEffectsManager Instance { get; private set; }

    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private float cashPopupDuration = 0.7f;
    [SerializeField] private float cashPopupMoveDistance = 60f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowCashPopup(Vector3 worldPosition, double amount, bool isCombo = false, bool isVIP = false)
    {
        StartCoroutine(AnimateCashPopup(worldPosition, amount, isCombo, isVIP));
    }

    private IEnumerator AnimateCashPopup(Vector3 worldPosition, double amount, bool isCombo, bool isVIP)
    {
        // Create canvas world space popup
        GameObject popupObj = new GameObject("CashPopup");

        if (worldCanvas == null)
        {
            worldCanvas = FindObjectOfType<Canvas>();
        }

        if (worldCanvas != null)
        {
            popupObj.transform.SetParent(worldCanvas.transform);
        }

        RectTransform popupRect = popupObj.AddComponent<RectTransform>();
        popupRect.position = worldPosition + Vector3.up * 20;

        TextMeshProUGUI popupText = popupObj.AddComponent<TextMeshProUGUI>();
        popupText.text = isVIP ? $"${amount:F0}★" : $"+${amount:F0}";
        popupText.fontSize = isVIP ? 48 : 32;
        popupText.alignment = TextAlignmentOptions.Center;
        popupText.color = isVIP ? Color.yellow : (isCombo ? Color.red : Color.white);

        float elapsed = 0f;
        while (elapsed < cashPopupDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / cashPopupDuration;

            Vector3 newPos = popupRect.position + Vector3.up * (cashPopupMoveDistance * Time.deltaTime);
            popupRect.position = newPos;

            Color color = popupText.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            popupText.color = color;

            yield return null;
        }

        Destroy(popupObj);
    }

    public void ShowUpgradeEffect(Vector3 position)
    {
        // Particle burst effect
        // Will implement when particle system added
    }

    public void PlayComboEffect(int comboCount)
    {
        // Combo flame effect
        // Will implement when animation system added
    }

    public void PlayVIPEntrance()
    {
        // VIP fanfare effect
        // Will implement when animation system added
    }

    public void ScreenShake(float intensity = 0.1f, float duration = 0.1f)
    {
        StartCoroutine(DoScreenShake(intensity, duration));
    }

    private IEnumerator DoScreenShake(float intensity, float duration)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            Vector3 shakeOffset = Random.insideUnitCircle * intensity * (1f - progress);
            mainCamera.transform.position = originalPos + shakeOffset;

            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }
}
