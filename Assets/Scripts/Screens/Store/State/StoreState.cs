using System.Collections.Generic;

public class StoreState
{
    private Dictionary<string, int> itemQuantities = new();
    private Dictionary<string, int> upgradeLevels = new();
    private HashSet<string> ownedOutfits = new();

    public int GetItemQuantity(string id) =>
        itemQuantities.TryGetValue(id, out var v) ? v : 0;

    public int GetUpgradeLevel(string id) =>
        upgradeLevels.TryGetValue(id, out var v) ? v : 0;

    public bool IsOutfitOwned(string id) =>
        ownedOutfits.Contains(id);

    public void AddItem(string id, int amount = 1)
    {
        itemQuantities[id] = GetItemQuantity(id) + amount;
    }

    public void LevelUp(string id)
    {
        upgradeLevels[id] = GetUpgradeLevel(id) + 1;
    }

    public void AddOutfit(string id)
    {
        ownedOutfits.Add(id);
    }

    // Interal access for the save system
    public Dictionary<string, int> ItemQuantities => itemQuantities;
    public Dictionary<string, int> UpgradeLevels => upgradeLevels;
    public HashSet<string> OwnedOutfits => ownedOutfits;
}