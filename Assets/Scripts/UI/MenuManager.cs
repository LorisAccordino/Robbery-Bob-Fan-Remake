using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainMenu;

    private Stack<CanvasGroup> menuStack = new Stack<CanvasGroup>();

    private void Start()
    {
        OpenMenu(mainMenu);
    }

    public void OpenMenu(CanvasGroup menu)
    {
        if (menuStack.Count > 0)
            SetMenuState(menuStack.Peek(), false);

        SetMenuState(menu, true);
        menuStack.Push(menu);
    }

    public void GoBack()
    {
        if (menuStack.Count <= 1)
            return;

        CanvasGroup current = menuStack.Pop();
        SetMenuState(current, false);

        SetMenuState(menuStack.Peek(), true);
    }

    public void GoToMainMenu()
    {
        while (menuStack.Count > 1)
        {
            CanvasGroup menu = menuStack.Pop();
            SetMenuState(menu, false);
        }

        SetMenuState(menuStack.Peek(), true);
    }

    private void SetMenuState(CanvasGroup menu, bool visible)
    {
        menu.alpha = visible ? 1f : 0f;
        //menu.interactable = visible;
        menu.blocksRaycasts = visible;
    }
}