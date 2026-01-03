using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreMenuManager : MonoBehaviour
{
    public List<StoreItemData> storeItemsData;
    public StoreItemView itemPrefab;
    public Transform content;

    [Header("Scroll")]
    public SmoothScrollToItem smoothScroll;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onItemClicked;
    public UnityEngine.Events.UnityEvent onPurchaseSuccess;
    public UnityEngine.Events.UnityEvent onNotEnoughCoins;
    public UnityEngine.Events.UnityEvent onItemMaxed;

    private StoreState state;
    private CurrencyManager currency;
    private StorePurchaseService purchaseService;
    private StoreSelectionController selectionController;

    void Awake()
    {
        state = new StoreState();
        currency = new CurrencyManager();
        purchaseService = new StorePurchaseService(state, currency);
        selectionController = new StoreSelectionController();

        StoreStatePersistence.Load(state);
    }

    void Start()
    {
        foreach (var data in storeItemsData)
        {
            var view = Instantiate(itemPrefab, content);
            view.Setup(data, selectionController, BuyItem);
            view.Refresh(state);
            view.GetComponent<Button>().onClick.AddListener(view.OnClicked);
        }
    }

    private void BuyItem(StoreItemView view)
    {
        var result = purchaseService.TryPurchase(view.Data);

        switch (result)
        {
            case PurchaseResult.Success:
                view.Refresh(state);
                onPurchaseSuccess?.Invoke();
                break;
            case PurchaseResult.NotEnoughCoins:
                onNotEnoughCoins?.Invoke();
                break;
            case PurchaseResult.Maxed:
                onItemMaxed?.Invoke();
                break;
        }

        StoreStatePersistence.Save(state);
        currency.Save();
    }
}