using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class StoreMenuManager : MonoBehaviour
{
    [Header("Store Items")]
    public List<StoreButton> storeButtons;
    [HideInInspector] public StoreButton SelectedButton => storeButtons[selectedIndex];

    [Header("Events")]
    public UnityEvent onNotEnoughCoins;
    public UnityEvent onPurchaseSuccess;
    public UnityEvent onItemMaxed;
    public UnityEvent onItemClicked;

    [Header("Scroll")]
    public SmoothScrollToItem smoothScroll;

    [Header("Localize Text")]
    public LocalizeStringEvent storeItemCaption;

    [Header("Selection Settings")]
    public float selectedOffsetX = -10f;

    [Header("Audio")]
    public AudioSource clickSound;


    private int coins;
    private int selectedIndex = -1;
    private Dictionary<StoreButton, Vector2> originalAnchoredPositions = new Dictionary<StoreButton, Vector2>();

    void Start()
    {
        LoadCoins();

        for (int i = 0; i < storeButtons.Count; i++)
        {
            var btn = storeButtons[i];

            if (btn.rectTransform != null)
                originalAnchoredPositions[btn] = btn.rectTransform.anchoredPosition;

            int index = i;
            btn.button.onClick.AddListener(() => ClickItem(index));

            // Safe link to the buy button
            if (btn.buyButton != null)
            {
                StoreButton capturedBtn = btn;
                btn.buyButton.onClick.AddListener(() => BuyItem(capturedBtn));
            }

            UpdateCostUI(btn);
        }
    }


    public void ClickItem(int index)
    {
        if (selectedIndex == index)
            return;

        selectedIndex = index;

        UpdateSelectionVisuals();
        UpdateLocalizedText();

        // Smooth scroll to center
        if (smoothScroll != null) smoothScroll.ScrollTo(SelectedButton);
        onItemClicked?.Invoke();
    }

    public void BuySelectedItem()
    {
        if (selectedIndex < 0)
            return;

        StoreButton item = SelectedButton;
        int purchaseIndex = GetPurchaseIndex(item);

        int cost = GetCurrentCost(item, purchaseIndex);

        // Item fully purchased
        if (cost < 0)
        {
            onItemMaxed?.Invoke();
            return;
        }

        // Not enough coins
        if (coins < cost)
        {
            onNotEnoughCoins?.Invoke();
            return;
        }

        // Purchase success
        coins -= cost;
        SaveCoins();
        PlayerPrefs.SetInt(item.saveKey, purchaseIndex + 1);
        UpdateCostUI(item);
        onPurchaseSuccess?.Invoke();
    }

    public void BuyItem(StoreButton clickedItem)
    {
        int clickedIndex = storeButtons.IndexOf(clickedItem);
        if (clickedIndex < 0)
            return;

        // If the clicked item is NOT selected, select it
        if (selectedIndex != clickedIndex)
        {
            ClickItem(clickedIndex);
            return;
        }

        BuySelectedItem();
    }

    public void DeselectAll()
    {
        selectedIndex = -1;

        // Reset visual selection
        for (int i = 0; i < storeButtons.Count; i++)
        {
            var btn = storeButtons[i];

            if (btn.selectedVisual != null)
                btn.selectedVisual.SetActive(false);

            if (btn.rectTransform != null && originalAnchoredPositions.ContainsKey(btn))
                btn.rectTransform.anchoredPosition = originalAnchoredPositions[btn];
        }
    }


    private int GetCurrentCost(StoreButton item, int purchaseIndex)
    {
        if (item.costs == null || item.costs.Count == 0)
            return 0;

        if (purchaseIndex < item.costs.Count)
            return item.costs[purchaseIndex];

        return item.repeatLastCost ? item.costs[item.costs.Count - 1] : -1;
    }

    private int GetPurchaseIndex(StoreButton item)
    {
        return PlayerPrefs.GetInt(item.saveKey, 0);
    }

    private void UpdateCostUI(StoreButton item)
    {
        int purchaseIndex = GetPurchaseIndex(item);
        int cost = GetCurrentCost(item, purchaseIndex);

        bool canBuy = cost >= 0;

        if (item.costLabel != null)
            item.costLabel.text = canBuy ? cost.ToString() : "MAX";

        if (!canBuy)
        {
            var button = item.costLabel.GetComponentInParent<Button>();
            if (button != null)
                button.gameObject.SetActive(false);
        }
    }

    private void UpdateSelectionVisuals()
    {
        for (int i = 0; i < storeButtons.Count; i++)
        {
            var btn = storeButtons[i];

            if (btn.selectedVisual != null)
                btn.selectedVisual.SetActive(i == selectedIndex);

            if (btn.rectTransform != null)
            {
                Vector2 basePos = originalAnchoredPositions[btn];
                btn.rectTransform.anchoredPosition =
                    (i == selectedIndex)
                        ? new Vector2(basePos.x + selectedOffsetX, basePos.y)
                        : basePos;
            }
        }
    }

    private void UpdateLocalizedText()
    {
        if (storeItemCaption != null && selectedIndex >= 0)
        {
            storeItemCaption.StringReference.TableEntryReference = SelectedButton.localizedItemKey;
            storeItemCaption.RefreshString();
        }
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("PLAYER_COINS", coins);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("PLAYER_COINS", coins);
    }
}