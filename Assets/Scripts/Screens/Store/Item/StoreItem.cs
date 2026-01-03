using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoreItem
{
    public Button button;                 // Item button
    public Button buyButton;              // Purchase button
    public GameObject selectedVisual;     // Selection highlight
    public RectTransform rectTransform;   // RectTransform to move
    public string localizedItemKey;       // Localization key

    [Header("Purchase Settings")]
    public List<int> costs;               // Purchase costs (progressive)
    public bool repeatLastCost;           // Can buy multiple times?
    public TextMeshProUGUI costLabel;     // UI label for cost
    public string saveKey;                // Unique save key for this item
}