using System.Collections.Generic;
using UnityEngine;

public enum StoreItemType
{
    Item,
    Upgrade,
    Outfit
}

[CreateAssetMenu(menuName = "Store/Item")]
public class StoreItemData : ScriptableObject
{
    [Header("Identity")]
    public string id;

    [Header("UI")]
    public Sprite icon;

    [Header("Localization Keys")]
    public string titleKey;
    public string descriptionKey;
    public string buyTextKey;

    [Header("Type")]
    public StoreItemType type;

    [Header("Upgrade Settings")]
    public List<int> costs;
    public bool repeatLastCost;
    public int maxLevel;
}