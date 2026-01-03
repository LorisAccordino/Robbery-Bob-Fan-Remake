using UnityEngine;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.Events;

/// <summary>
/// Watches Localize*Events and fires an event when all have updated.
/// </summary>
public class LocalizationUpdateWatcher : MonoBehaviour
{
    public bool waitAllElements = false;
    public float minDuration = 0.15f;
    public float timeout = 5f;

    public UnityEvent OnLocalizationStarted;
    public UnityEvent OnLocalizationReady;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        OnLocaleChanged(LocalizationSettings.SelectedLocale);
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        OnLocalizationStarted?.Invoke();
        StartWatching();
    }

    public void StartWatching() => StartCoroutine(WatchRoutine());

    private IEnumerator WatchRoutine()
    {
        List<LocalizeStringEvent> stringEvents = new List<LocalizeStringEvent>();
        List<LocalizeSpriteEvent> spriteEvents = new List<LocalizeSpriteEvent>();

        foreach (var e in FindObjectsByType<LocalizeStringEvent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (waitAllElements || e.gameObject.activeInHierarchy)
                stringEvents.Add(e);

        foreach (var e in FindObjectsByType<LocalizeSpriteEvent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (waitAllElements || e.gameObject.activeInHierarchy)
                spriteEvents.Add(e);

        int pending = stringEvents.Count + spriteEvents.Count;
        if (pending == 0)
        {
            yield return new WaitForSeconds(minDuration);
            OnLocalizationReady?.Invoke();
            yield break;
        }

        void SafeDecrement() => pending--;

        foreach (var e in stringEvents)
            e.OnUpdateString.AddListener(_ => SafeDecrement());
        foreach (var e in spriteEvents)
            e.OnUpdateAsset.AddListener(_ => SafeDecrement());

        float startTime = Time.time;
        while (pending > 0 && Time.time - startTime < timeout)
            yield return null;

        // Ensure waiting min duration
        float elapsed = Time.time - startTime;
        if (elapsed < minDuration)
            yield return new WaitForSeconds(minDuration - elapsed);

        // Cleanup
        foreach (var e in stringEvents) e.OnUpdateString.RemoveAllListeners();
        foreach (var e in spriteEvents) e.OnUpdateAsset.RemoveAllListeners();

        if (pending > 0) Debug.LogWarning("Localization timeout reached.");

        OnLocalizationReady?.Invoke();
    }
}