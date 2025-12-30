using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeController : MonoBehaviour
{
    [Header("Default Fade Settings")]
    [Tooltip("Default fade duration if not specified")]
    public float defaultDuration = 0.3f;

    public event System.Action OnFadeComplete;


    [Header("Black Overlay Options")]
    [Tooltip("If true, the fade will use a black overlay transition")]
    public bool forceBlack = false;

    private CanvasGroup canvasGroup;        // The main CanvasGroup
    private Coroutine fadeCoroutineNormal;
    private Coroutine fadeCoroutineBlack;

    private CanvasGroup blackOverlay;       // Dynamically created black overlay panel

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (forceBlack)
            SetupBlackOverlay();
    }

    // -----------------------
    // Public API
    // -----------------------

    /// <summary>
    /// Fade in the content
    /// </summary>
    /// <param name="duration">Optional fade duration</param>
    /// <param name="force">Force start alpha</param>
    /// <param name="useBlack">Temporarily use black overlay fade</param>
    public void FadeInDefault() => FadeIn();
    public void FadeIn(float duration = -1f, bool force = false, bool useBlack = false)
    {
        if (duration <= 0f) duration = defaultDuration;

        bool blackActive = forceBlack || useBlack;

        if (blackActive)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            FadeBlackPanel(fadeIn: true, duration);
        }
        else
        {
            if (force)
                canvasGroup.alpha = 0f;

            FadeTo(1f, duration);
        }
    }


    /// <summary>
    /// Fade out the content
    /// </summary>
    /// <param name="duration">Optional fade duration</param>
    /// <param name="force">Force start alpha</param>
    /// <param name="useBlack">Temporarily use black overlay fade</param>
    public void FadeOutDefault() => FadeOut();
    public void FadeOut(float duration = -1f, bool force = false, bool useBlack = false)
    {
        if (duration <= 0f) duration = defaultDuration;

        bool blackActive = forceBlack || useBlack;

        if (blackActive)
        {
            canvasGroup.alpha = 1f; // Ensure the canvas is fully visible for the overlay
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            FadeBlackPanel(fadeIn: false, duration);
        }
        else
        {
            if (force)
                canvasGroup.alpha = 1f;

            FadeTo(0f, duration);
        }
    }


    /// <summary>
    /// Fade CanvasGroup to a target alpha
    /// </summary>
    public void FadeTo(float targetAlpha, float duration)
    {
        if (fadeCoroutineNormal != null)
            StopCoroutine(fadeCoroutineNormal);

        fadeCoroutineNormal = StartCoroutine(FadeRoutine(canvasGroup, targetAlpha, duration));
    }

    // -----------------------
    // Black overlay logic
    // -----------------------

    private void SetupBlackOverlay()
    {
        if (blackOverlay != null) return;

        // Check if a child named BlackOverlay exists
        Transform existing = transform.Find("BlackOverlay");
        if (existing != null)
        {
            blackOverlay = existing.GetComponent<CanvasGroup>();
            if (blackOverlay != null) return;
        }

        // Create new black panel as child
        GameObject go = new GameObject("BlackOverlay", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        go.transform.SetAsLastSibling();

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = Color.black;

        blackOverlay = go.AddComponent<CanvasGroup>();
        blackOverlay.alpha = 0f;
        blackOverlay.interactable = false;
        blackOverlay.blocksRaycasts = false;
    }

    private void FadeBlackPanel(bool fadeIn, float duration)
    {
        if (!blackOverlay)
            SetupBlackOverlay();

        blackOverlay.interactable = true;
        blackOverlay.blocksRaycasts = true;

        float from = fadeIn ? 1f : 0f; // Inverse fade
        float to = fadeIn ? 0f : 1f;

        if (fadeCoroutineBlack != null)
            StopCoroutine(fadeCoroutineBlack);

        fadeCoroutineBlack = StartCoroutine(FadeBlackRoutine(from, to, duration, fadeIn));
    }

    /// <summary>
    /// Coroutine for black overlay fade
    /// </summary>
    private IEnumerator FadeBlackRoutine(float from, float to, float duration, bool fadeIn)
    {
        blackOverlay.alpha = from;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            blackOverlay.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        blackOverlay.alpha = to;

        if (fadeIn)
        {
            // After fade in, normal content fully visible
            blackOverlay.interactable = false;
            blackOverlay.blocksRaycasts = false;
        }
        else
        {
            // After fade out, hide both canvas and overlay
            blackOverlay.alpha = 0f;
            blackOverlay.interactable = false;
            blackOverlay.blocksRaycasts = false;

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        OnFadeComplete?.Invoke();
    }

    // -----------------------
    // Coroutine for normal CanvasGroup fade
    // -----------------------
    private IEnumerator FadeRoutine(CanvasGroup cg, float targetAlpha, float duration)
    {
        float startAlpha = cg.alpha;

        if (Mathf.Abs(startAlpha - targetAlpha) < 0.001f)
            yield break;

        float time = 0f;

        cg.blocksRaycasts = false;
        cg.interactable = false;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        cg.alpha = targetAlpha;
        bool visible = targetAlpha > 0.99f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;

        OnFadeComplete?.Invoke();
    }
}