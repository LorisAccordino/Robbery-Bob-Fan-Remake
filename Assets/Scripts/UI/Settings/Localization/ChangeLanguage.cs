using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;

public class ChangeLanguage : MonoBehaviour
{
    private const string LocalePrefKey = "SelectedLocaleCode";

    private void Start()
    {
        StartCoroutine(LoadSavedLocale());
    }

    public void SetLocale(int localeIndex)
    {
        if (localeIndex < 0 || localeIndex >= LocalizationSettings.AvailableLocales.Locales.Count)
            return;

        Locale locale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        ApplyLocale(locale);
    }

    public void SetLocaleByCode(string code)
    {
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (locale != null)
            ApplyLocale(locale);
        else
            Debug.LogWarning($"Locale with code '{code}' not found.");
    }

    private void ApplyLocale(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;

        PlayerPrefs.SetString(LocalePrefKey, locale.Identifier.Code);
        PlayerPrefs.Save();
    }

    private IEnumerator LoadSavedLocale()
    {
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