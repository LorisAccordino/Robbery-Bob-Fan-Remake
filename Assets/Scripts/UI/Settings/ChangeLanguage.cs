using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class ChangeLanguage : MonoBehaviour
{
    // PlayerPrefs key used to save the selected locale
    private const string LocalePrefKey = "SelectedLocaleCode";

    private void Start()
    {
        // Load saved locale on startup
        StartCoroutine(LoadSavedLocale());
    }

    /// <summary>
    /// Sets the locale using its index in the Available Locales list
    /// </summary>
    public void SetLocale(int localeIndex)
    {
        if (localeIndex < 0 || localeIndex >= LocalizationSettings.AvailableLocales.Locales.Count)
            return;

        Locale locale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        SetLocaleInternal(locale);
    }

    /// <summary>
    /// Sets the locale using its ISO code (e.g. "en", "it", "zh-Hans")
    /// </summary>
    public void SetLocaleByCode(string code)
    {
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (locale != null)
            SetLocaleInternal(locale);
        else
            Debug.LogWarning($"Locale with code '{code}' not found.");
    }

    /// <summary>
    /// Applies the locale and saves it to PlayerPrefs
    /// </summary>
    private void SetLocaleInternal(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;

        // Save locale code (e.g. "en", "it", "zh-Hans")
        PlayerPrefs.SetString(LocalePrefKey, locale.Identifier.Code);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the saved locale on game startup
    /// </summary>
    private IEnumerator LoadSavedLocale()
    {
        // Wait for Localization system to be ready
        yield return LocalizationSettings.InitializationOperation;

        if (PlayerPrefs.HasKey(LocalePrefKey))
        {
            string savedCode = PlayerPrefs.GetString(LocalePrefKey);
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(savedCode);

            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
            else
            {
                Debug.LogWarning($"Saved locale '{savedCode}' not found. Using default locale.");
            }
        }
    }
}