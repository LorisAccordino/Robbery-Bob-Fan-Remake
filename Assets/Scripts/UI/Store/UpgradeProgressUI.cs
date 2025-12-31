using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeProgressUI : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeData
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
        [SerializeField] public int currentUpgrade;
    }

    [Header("Upgrades")]
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    void Start()
    {
        RefreshAll();
    }

    /// <summary>
    /// Refresh all upgrades (callable via UnityEvent)
    /// </summary>
    public void RefreshAll()
    {
        foreach (var upgrade in upgrades)
        {
            RefreshUpgrade(upgrade);
        }
    }

    /// <summary>
    /// Refresh a single upgrade
    /// </summary>
    private void RefreshUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null)
            return;

        upgrade.currentUpgrade = PlayerPrefs.GetInt(upgrade.saveKey, 0);
        upgrade.currentUpgrade = Mathf.Clamp(upgrade.currentUpgrade, 0, upgrade.maxUpgrades);

        UpdateFill(upgrade);
    }

    private void UpdateFill(UpgradeData upgrade)
    {
        if (upgrade.fillImage == null || upgrade.maxUpgrades <= 0)
            return;

        float normalizedValue = (float)upgrade.currentUpgrade / upgrade.maxUpgrades;
        upgrade.fillImage.fillAmount = normalizedValue;
    }
}