using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StoreItemView : MonoBehaviour
{
    [Header("Common")]
    public Image icon;
    public LocalizeStringEvent title;
    public TMP_Text costText;
    public Button buyButton;
    public GameObject selectedHighlight;

    [Header("Optional Areas")]
    public LocalizeStringEvent description;

    public GameObject quantityArea;
    public TMP_Text quantityText;

    public GameObject upgradeArea;
    public Image upgradeBar;

    public LocalizeStringEvent buyText;

    [Header("Item SO Data")]
    public StoreItemData Data { get; set; }

    // Selection settings
    private bool isSelected = false;
    private Vector2 originalPosition;
    public float selectedOffsetX = -10f;

    // Selection and scrolling
    public RectTransform rectTransform;
    private StoreSelectionController selectionController;


    public void Setup(StoreItemData data, StoreSelectionController controller, System.Action<StoreItemView> onBuy)
    {
        Data = data;
        selectionController = controller;

        buyButton.onClick.AddListener(() => onBuy?.Invoke(this));

        icon.sprite = Data.icon;
        title.StringReference.TableEntryReference = Data.titleKey;
        description.StringReference.TableEntryReference = Data.descriptionKey;
        buyText.StringReference.TableEntryReference = Data.buyTextKey;

        description.gameObject.SetActive(Data.type == StoreItemType.Item);
        quantityArea.SetActive(Data.type == StoreItemType.Item);
        upgradeArea.SetActive(Data.type == StoreItemType.Upgrade);

        if (Data.costs.Count > 0) 
            SetCost(Data.costs[0]);

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null) originalPosition = rectTransform.anchoredPosition;
    }

    public void OnClicked()
    {
        if (selectionController.Select(Data.id))
        {
            selectionController.UpdateSelection(this);
        }
    }

    public void SetSelected(bool value)
    {
        selectedHighlight.SetActive(value);

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition =
                value ? new Vector2(originalPosition.x + selectedOffsetX, originalPosition.y)
                      : originalPosition;
        }
    }

    public void Refresh(StoreState state)
    {
        switch (Data.type)
        {
            case StoreItemType.Item:
                quantityText.text = $"x{state.GetItemQuantity(Data.id)}";
                break;

            case StoreItemType.Upgrade:
                int lvl = state.GetUpgradeLevel(Data.id);
                upgradeBar.fillAmount = (float)lvl / Data.maxLevel;
                break;

            case StoreItemType.Outfit:
                //ownedBadge.SetActive(state.IsOutfitOwned(Data.id));
                buyButton.gameObject.SetActive(!state.IsOutfitOwned(Data.id));
                break;
        }
    }

    public void SetCost(int value)
    {
        costText.text = $"{value}";
    }
}