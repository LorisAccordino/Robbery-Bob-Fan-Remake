using UnityEngine;

public enum PurchaseResult
{
    Success,
    NotEnoughCoins,
    Maxed
}

public class StorePurchaseService
{
    private StoreState state;
    private CurrencyManager currency;

    public StorePurchaseService(StoreState state, CurrencyManager currency)
    {
        this.state = state;
        this.currency = currency;
    }

    public PurchaseResult TryPurchase(StoreItemData data)
    {
        switch (data.type)
        {
            case StoreItemType.Item:
                return BuyItem(data);

            case StoreItemType.Upgrade:
                return BuyUpgrade(data);

            case StoreItemType.Outfit:
                return BuyOutfit(data);
        }

        return PurchaseResult.NotEnoughCoins;
    }

    private PurchaseResult BuyItem(StoreItemData data)
    {
        int cost = data.costs[0];
        if (!currency.CanAfford(cost)) return PurchaseResult.NotEnoughCoins;

        currency.Spend(cost);
        state.AddItem(data.id);
        return PurchaseResult.Success;
    }

    private PurchaseResult BuyUpgrade(StoreItemData data)
    {
        int level = state.GetUpgradeLevel(data.id);
        if (level >= data.maxLevel) return PurchaseResult.Maxed;

        int cost = data.costs[Mathf.Min(level, data.costs.Count - 1)];
        if (!currency.CanAfford(cost)) return PurchaseResult.NotEnoughCoins;

        currency.Spend(cost);
        state.LevelUp(data.id);
        return PurchaseResult.Success;
    }

    private PurchaseResult BuyOutfit(StoreItemData data)
    {
        if (state.IsOutfitOwned(data.id)) return PurchaseResult.Maxed;

        int cost = data.costs[0];
        if (!currency.CanAfford(cost)) return PurchaseResult.NotEnoughCoins;

        currency.Spend(cost);
        state.AddOutfit(data.id);
        return PurchaseResult.Success;
    }
}