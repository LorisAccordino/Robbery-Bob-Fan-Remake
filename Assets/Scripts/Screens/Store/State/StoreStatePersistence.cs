using UnityEngine;
using System.Collections.Generic;

public static class StoreStatePersistence
{
    public static void Save(StoreState state)
    {
        SaveDictionary(GameKeys.ITEM_KEY, state.ItemQuantities);
        SaveDictionary(GameKeys.UPGRADE_KEY, state.UpgradeLevels);
        SaveHashSet(GameKeys.OUTFIT_KEY, state.OwnedOutfits);

        PlayerPrefs.Save();
    }

    public static void Load(StoreState state)
    {
        LoadDictionary(GameKeys.ITEM_KEY, state.ItemQuantities);
        LoadDictionary(GameKeys.UPGRADE_KEY, state.UpgradeLevels);
        LoadHashSet(GameKeys.OUTFIT_KEY, state.OwnedOutfits);
    }
    

    private static void SaveDictionary(string key, Dictionary<string, int> dict)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(new SerializationWrapper(dict)));
    }

    private static void LoadDictionary(string key, Dictionary<string, int> dict)
    {
        if (!PlayerPrefs.HasKey(key)) return;

        dict.Clear();
        var wrapper = JsonUtility.FromJson<SerializationWrapper>(
            PlayerPrefs.GetString(key)
        );

        foreach (var pair in wrapper.ToDictionary())
            dict[pair.Key] = pair.Value;
    }

    private static void SaveHashSet(string key, HashSet<string> set)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(new List<string>(set)));
    }

    private static void LoadHashSet(string key, HashSet<string> set)
    {
        if (!PlayerPrefs.HasKey(key)) return;

        set.Clear();
        var list = JsonUtility.FromJson<List<string>>(
            PlayerPrefs.GetString(key)
        );

        foreach (var id in list)
            set.Add(id);
    }
}


[System.Serializable]
public class SerializationWrapper
{
    public List<string> keys = new();
    public List<int> values = new();

    public SerializationWrapper(Dictionary<string, int> dict)
    {
        foreach (var kvp in dict)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public Dictionary<string, int> ToDictionary()
    {
        var dict = new Dictionary<string, int>();
        for (int i = 0; i < keys.Count; i++)
            dict[keys[i]] = values[i];
        return dict;
    }
}
