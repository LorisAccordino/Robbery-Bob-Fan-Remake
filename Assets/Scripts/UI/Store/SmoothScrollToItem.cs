using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmoothScrollToItem : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("Settings")]
    public float scrollSpeed = 5f;          // Higher = faster convergence
    public float stopThresholdWorld = 1f;   // Distance in world units to consider "aligned"
    public bool lockScrollDuringAnimation = true;

    private Coroutine scrollRoutine;

    /// <summary>
    /// Scroll adaptively to keep target near viewport center
    /// </summary>
    public void ScrollTo(RectTransform target)
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(ScrollToTargetCenter(target));
    }

    private IEnumerator ScrollToTargetCenter(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        if (lockScrollDuringAnimation)
            scrollRect.enabled = false;

        RectTransform viewport = scrollRect.viewport;

        while (true)
        {
            // World Y positions
            float targetWorldY = target.position.y;
            float viewportWorldY = viewport.position.y;

            // Delta between target and viewport center
            float deltaWorld = targetWorldY - viewportWorldY;

            // Stop if target close enough to center
            if (Mathf.Abs(deltaWorld) <= stopThresholdWorld)
                break;

            // Current normalized position
            float currentNormalized = scrollRect.verticalNormalizedPosition;

            // Convert world delta to normalized movement
            // Now inverted logic: 
            // target above center -> scroll up (normalized increases)
            // target below center -> scroll down (normalized decreases)
            float moveAmount = deltaWorld / (content.rect.height - viewport.rect.height);

            // Apply speed factor
            float nextNormalized = currentNormalized + moveAmount * scrollSpeed * Time.deltaTime;

            // Clamp to [0,1]
            nextNormalized = Mathf.Clamp01(nextNormalized);

            scrollRect.verticalNormalizedPosition = nextNormalized;

            // Stop if we hit the top or bottom
            if (nextNormalized <= 0f || nextNormalized >= 1f)
                break;

            yield return null;
        }

        // Snap exactly
        scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition; // ensures final clamp

        if (lockScrollDuringAnimation)
            scrollRect.enabled = true;

        scrollRoutine = null;
    }
}