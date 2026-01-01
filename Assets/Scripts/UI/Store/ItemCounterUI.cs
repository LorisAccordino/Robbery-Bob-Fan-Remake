using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemCounterUI : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        [Header("Save Settings")]
        [Tooltip("PlayerPrefs key that stores the amount of this item")]
        public string saveKey;

        [Header("UI")]
        [Tooltip("TMP label to display the amount")]
        public TextMeshProUGUI label;

        [Header("Debug")]
        [SerializeField] public int currentAmount;
    }

    [Header("Items")]
    public List<ItemData> items = new List<ItemData>();

    void Start()
    {
        RefreshAll();
    }

    /// <summary>
    /// Refresh all item amounts (callable from UnityEvent)
    /// </summary>
    public void RefreshAll()
    {
        foreach (var item in items)
        {
            RefreshItem(item);
        }
    }

    /// <summary>
    /// Refresh a single item's amount
    /// </summary>
    private void RefreshItem(ItemData item)
    {
        if (item == null)
            return;

        // Load amount from PlayerPrefs (default 0)
        item.currentAmount = PlayerPrefs.GetInt(item.saveKey, 0);

        // Update UI label
        UpdateLabel(item);
    }

    private void UpdateLabel(ItemData item)
    {
        if (item.label == null)
            return;

        item.label.text = item.currentAmount.ToString();
    }

    /// <summary>
    /// Increment the amount of an item and save it
    /// </summary>
    public void AddItem(string saveKey, int amount = 1)
    {
        var item = items.Find(i => i.saveKey == saveKey);
        if (item == null)
        {
            Debug.LogWarning($"ItemCounterUI: No item found with key '{saveKey}'");
            return;
        }

        item.currentAmount += amount;
        PlayerPrefs.SetInt(item.saveKey, item.currentAmount);
        PlayerPrefs.Save();

        UpdateLabel(item);
    }
}