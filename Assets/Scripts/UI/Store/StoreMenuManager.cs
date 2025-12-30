using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

[System.Serializable]
public class StoreButton
{
    public Button button;               // The actual button
    public GameObject selectedVisual;   // Object to show/hide when selected
    public RectTransform rectTransform; // RectTransform to adjust left offset
    public string localizedItemKey;
}

public class StoreMenuManager : MonoBehaviour
{
    [Header("Store Items")]
    public List<StoreButton> storeButtons; // Assign buttons in Inspector

    [Header("Localize Text")]
    public LocalizeStringEvent storeItemCaption; // Updates localized text

    [Header("Selection Settings")]
    public float selectedOffsetX = -10f; // Left offset when selected

    [Header("Audio")]
    public AudioSource clickSound; // sound to play on click

    private int selectedIndex = -1;
    private Dictionary<StoreButton, float> originalLeftOffsets = new Dictionary<StoreButton, float>();

    void Start()
    {
        // Save original left offsets and register button clicks
        foreach (var btn in storeButtons)
        {
            if (btn.rectTransform != null)
                originalLeftOffsets[btn] = btn.rectTransform.offsetMin.x;

            int index = storeButtons.IndexOf(btn);
            if (btn.button != null)
                btn.button.onClick.AddListener(() => OnItemClicked(index));
        }
    }

    public void OnItemClicked(int index)
    {
        if (selectedIndex == index)
            return;

        selectedIndex = index;

        // Play click sound
        if (clickSound != null)
            clickSound.Play();

        UpdateSelectionVisuals();
        UpdateLocalizedText();

        Debug.Log($"Item {index} selected");
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < storeButtons.Count; i++)
        {
            var btn = storeButtons[i];

            // Activate or deactivate the "selected" visual
            if (btn.selectedVisual != null)
                btn.selectedVisual.SetActive((i == selectedIndex));

            // Adjust the left offset
            if (btn.rectTransform != null && originalLeftOffsets.ContainsKey(btn))
            {
                Vector2 offsetMin = btn.rectTransform.offsetMin;
                offsetMin.x = originalLeftOffsets[btn] + ((i == selectedIndex) ? selectedOffsetX : 0f);
                btn.rectTransform.offsetMin = offsetMin;
            }
        }
    }

    private void UpdateLocalizedText()
    {
        if (storeItemCaption != null && selectedIndex >= 0 && selectedIndex < storeButtons.Count)
        {
            string key = storeButtons[selectedIndex].localizedItemKey;
            storeItemCaption.StringReference.TableEntryReference = key;
            storeItemCaption.RefreshString();
        }
    }

    public void DeselectAll()
    {
        selectedIndex = -1;
        UpdateSelectionVisuals();

        if (storeItemCaption != null)
        {
            storeItemCaption.StringReference.TableEntryReference = "";
            storeItemCaption.RefreshString();
        }
    }
}