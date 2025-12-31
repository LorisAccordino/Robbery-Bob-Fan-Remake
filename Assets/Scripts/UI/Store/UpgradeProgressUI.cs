using UnityEngine;
using UnityEngine.UI;

public class UpgradeProgressUI : MonoBehaviour
{
    [Header("Save Settings")]
    [Tooltip("PlayerPrefs key that stores the current upgrade level")]
    public string saveKey;

    [Tooltip("Maximum number of upgrades")]
    public int maxUpgrades = 5;

    [Header("UI")]
    [Tooltip("Filled image (Image Type must be Filled, Horizontal, Left)")]
    public Image fillImage;

    [Header("Debug")]
    [SerializeField] private int currentUpgrade;

    void Start()
    {
        Refresh();
    }

    /// <summary>
    /// Public method callable via UnityEvent
    /// </summary>
    public void Refresh()
    {
        currentUpgrade = PlayerPrefs.GetInt(saveKey, 0);
        currentUpgrade = Mathf.Clamp(currentUpgrade, 0, maxUpgrades);

        UpdateFill();
    }

    private void UpdateFill()
    {
        if (fillImage == null || maxUpgrades <= 0)
            return;

        float normalizedValue = (float)currentUpgrade / maxUpgrades;
        fillImage.fillAmount = normalizedValue;
    }
}