using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private MenuManager manager;

    private void OnEnable()
    {
        manager.OnScreenOpened += ShowScreen;
        manager.OnScreenClosed += HideScreen;
    }

    private void OnDisable()
    {
        manager.OnScreenOpened -= ShowScreen;
        manager.OnScreenClosed -= HideScreen;
    }

    private void ShowScreen(MenuScreen screen)
    {
        screen.canvasGroup.alpha = 1f;
        screen.canvasGroup.blocksRaycasts = true;
        screen.OnEnter?.Invoke();
    }

    private void HideScreen(MenuScreen screen)
    {
        screen.canvasGroup.alpha = 0f;
        screen.canvasGroup.blocksRaycasts = false;
        screen.OnExit?.Invoke();
    }
}