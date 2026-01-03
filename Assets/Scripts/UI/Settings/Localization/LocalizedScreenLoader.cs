using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Manages a loading screen for localized UI.
/// Shows the loading overlay and hides the UI until localization elements have updated.
/// Supports minimum loading time and option to wait for all elements or only visible/active ones.
/// </summary>
public class LocalizedScreenLoader : MonoBehaviour
{
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup loadingOverlay; // The loading screen overlay
    [SerializeField] private CanvasGroup uiRoot;         // The main UI root

    [Header("Timing settings")]
    [Tooltip("Minimum time (seconds) the loading screen will remain visible")]
    [SerializeField] private float minLoadingTime = 1f;
    [Tooltip("Maximum time (seconds) the loading screen will remain visible")]
    [SerializeField] private float timeout = 5f;

    [Tooltip("If true, waits for all Localize*Events in the scene; if false, only waits for active/visible elements")]
    [SerializeField] private bool waitAllElements = false;

    [Header("References")]
    public MenuManager menuManager;

    // Cache the fade controller for efficiency
    private FadeUIController loadingFadeController;

    private void Awake()
    {
        if (loadingOverlay)
        {
            loadingFadeController = loadingOverlay.GetComponent<FadeUIController>();
            if (loadingFadeController == null)
                loadingFadeController = loadingOverlay.gameObject.AddComponent<FadeUIController>();
        }
    }

    private void OnEnable()
    {
        // Listen for global locale changes
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        // Start initial localization routine on enable
        StartCoroutine(StartRoutine());
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        // Restart the loading routine when language changes
        StartCoroutine(StartRoutine());
    }

    private IEnumerator StartRoutine()
    {
        // Hide the UI and show the loading overlay
        ShowUI(false);
        StartCoroutine(ShowLoading(true));

        float startTime = Time.time;

        // Collect Localize*Events to wait for
        List<LocalizeStringEvent> stringEvents = new List<LocalizeStringEvent>();
        List<LocalizeSpriteEvent> spriteEvents = new List<LocalizeSpriteEvent>();

        foreach (var e in FindObjectsOfType<LocalizeStringEvent>(true))
            if (waitAllElements || e.gameObject.activeInHierarchy)
                stringEvents.Add(e);

        foreach (var e in FindObjectsOfType<LocalizeSpriteEvent>(true))
            if (waitAllElements || e.gameObject.activeInHierarchy)
                spriteEvents.Add(e);

        int pendingUpdates = stringEvents.Count + spriteEvents.Count;

        if (pendingUpdates > 0)
        {
            void SafeOnUpdated()
            {
                try
                {
                    pendingUpdates--;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error in localization update: " + ex);
                    pendingUpdates--;
                }
            }

            foreach (var e in stringEvents)
                e.OnUpdateString.AddListener(_ => SafeOnUpdated());

            foreach (var e in spriteEvents)
                e.OnUpdateAsset.AddListener(_ => SafeOnUpdated());

            while (pendingUpdates > 0)
            {
                // If elapsed time is greater than timeout, stop waiting
                if (Time.time - startTime > timeout)
                {
                    Debug.LogWarning("Localization loading timeout reached. Continuing anyway.");
                    break;
                }
                yield return null;
            }

            // Cleanup listener
            foreach (var e in stringEvents)
                e.OnUpdateString.RemoveAllListeners();

            foreach (var e in spriteEvents)
                e.OnUpdateAsset.RemoveAllListeners();
        }

        // Ensure minimum loading time
        float elapsed = Time.time - startTime;
        if (elapsed < minLoadingTime)
            yield return new WaitForSeconds(minLoadingTime - elapsed);

        // Go to main menu, after language changed
        menuManager.GoToMainMenu();

        // Wait for fade out
        yield return StartCoroutine(ShowLoading(false));

        // Now show UI
        ShowUI(true);
    }

    /// <summary>
    /// Shows or hides the loading overlay
    /// </summary>
    private IEnumerator ShowLoading(bool show)
    {
        if (loadingFadeController)
        {
            bool fadeFinished = false;
            System.Action callback = () => fadeFinished = true;

            //loadingFadeController.OnFadeComplete += callback;

            if (show)
                loadingFadeController.FadeIn();
            else
                loadingFadeController.FadeOut();

            // Wait until fade finished
            while (!fadeFinished)
                yield return null;

            //loadingFadeController.OnFadeComplete -= callback;
        }
        else
        {
            // Fallback
            loadingOverlay.alpha = show ? 1f : 0f;
            loadingOverlay.interactable = show;
            loadingOverlay.blocksRaycasts = show;
        }
    }

    /// <summary>
    /// Shows or hides the main UI root
    /// </summary>
    private void ShowUI(bool show)
    {
        if (!uiRoot) return;

        // Set alpha settings
        uiRoot.alpha = show ? 1f : 0f;
        uiRoot.interactable = show;
        uiRoot.blocksRaycasts = show;
    }
}