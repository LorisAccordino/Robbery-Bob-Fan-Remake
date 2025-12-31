using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmoothScrollToItem : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;
    public RectTransform content;

    [Header("Scroll Settings")]
    public float scrollDuration = 0.25f;

    private Coroutine scrollRoutine;

    public void ScrollTo(RectTransform target)
    {
        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(ScrollCoroutine(target));
    }

    private IEnumerator ScrollCoroutine(RectTransform target)
    {
        // Force layout update
        Canvas.ForceUpdateCanvases();

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        // Item position relative to content
        Vector2 localPos = content.InverseTransformPoint(target.position);

        // Item center
        float itemCenterY = -localPos.y;

        // Target position normalized (0 = top, 1 = bottom)
        float targetNormalized =
            Mathf.Clamp01(
                (itemCenterY - viewportHeight * 0.5f) /
                (contentHeight - viewportHeight)
            );

        float start = scrollRect.verticalNormalizedPosition;
        float time = 0f;

        while (time < scrollDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / scrollDuration);
            scrollRect.verticalNormalizedPosition =
                Mathf.Lerp(start, 1f - targetNormalized, t);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 1f - targetNormalized;
    }
}