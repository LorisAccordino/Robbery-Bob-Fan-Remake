using UnityEngine;
using UnityEngine.Localization.Components;
using System.Collections.Generic;

[RequireComponent(typeof(LocalizeStringEvent))]
public class DynamicLocalizedString : MonoBehaviour
{
    [System.Serializable]
    public class WeightedEntry
    {
        public string key;      // Localization table key
        public float weight = 1; // Random choice weight
    }

    public List<WeightedEntry> entries = new List<WeightedEntry>();
    private LocalizeStringEvent localizeEvent;

    private void Awake()
    {
        localizeEvent = GetComponent<LocalizeStringEvent>();
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (entries.Count == 0) return;

        string selectedKey = GetRandomKey();
        if (localizeEvent != null)
            localizeEvent.StringReference.TableEntryReference = selectedKey;
    }

    private string GetRandomKey()
    {
        float totalWeight = 0f;
        foreach (var e in entries) totalWeight += e.weight;

        float r = Random.Range(0f, totalWeight);
        float sum = 0f;
        foreach (var e in entries)
        {
            sum += e.weight;
            if (r <= sum) return e.key;
        }
        return entries[0].key;
    }
}