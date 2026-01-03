using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUIController : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("Default fade duration (used if no duration is specified)")]
    [SerializeField] private float defaultDuration = 0.3f;
    [SerializeField] private bool disableInteraction = false;

    // Events for external systems
    [Header("Fade Events")]
    public UnityEvent OnFadeInComplete;   // Called after FadeIn finishes
    public UnityEvent OnFadeOutComplete;  // Called after FadeOut finishes

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    // Fade in the UI to fully visible
    public void FadeIn(float duration = -1f) => Fade(true, duration);

    // Fade out the UI to invisible
    public void FadeOut(float duration = -1f) => Fade(false, duration);

    // Internal fade logic
    private void Fade(bool fadeIn, float duration)
    {
        if (duration <= 0f) duration = defaultDuration;

        float from = fadeIn ? 0f : 1f;
        float to = fadeIn ? 1f : 0f;

        StartFade(from, to, duration, fadeIn);
    }

    private void StartFade(float from, float to, float duration, bool isFadeIn)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(canvasGroup, from, to, duration, isFadeIn));
    }

    private IEnumerator FadeRoutine(CanvasGroup cg, float from, float to, float duration, bool isFadeIn)
    {
        float time = 0f;
        cg.alpha = from;
        cg.blocksRaycasts = false;
        if (disableInteraction) cg.interactable = false;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, time / duration);
            yield return null;
        }

        // Ensure final alpha
        cg.alpha = to;

        // Enable/disable interaction based on visibility
        bool visible = to > 0.99f;
        if (disableInteraction) cg.interactable = visible;
        cg.blocksRaycasts = visible;

        // Fire the appropriate event
        if (isFadeIn)
            OnFadeInComplete?.Invoke();
        else
            OnFadeOutComplete?.Invoke();
    }
}