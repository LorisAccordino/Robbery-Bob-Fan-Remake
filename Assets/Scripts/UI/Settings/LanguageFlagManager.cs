using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class LanguageFlagManager : MonoBehaviour
{
    [System.Serializable]
    public class LanguageFlag
    {
        public string localeCode;   // "en", "it", "zh-Hans", etc.
        public Sprite flagSprite;
    }

    public Image flagImage; // UI Image where the flag is shown
    public List<LanguageFlag> flags;

    private Dictionary<string, Sprite> flagMap;

    void Awake()
    {
        flagMap = new Dictionary<string, Sprite>();
        foreach (var f in flags)
        {
            flagMap[f.localeCode] = f.flagSprite;
        }
    }

    void Start()
    {
        UpdateFlag();
    }

    public void UpdateFlag()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;

        if (flagMap.TryGetValue(localeCode, out Sprite sprite))
        {
            flagImage.sprite = sprite;
        }
    }
}