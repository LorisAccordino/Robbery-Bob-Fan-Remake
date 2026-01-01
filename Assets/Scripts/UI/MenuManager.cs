using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private MenuScreen mainMenu;
    [SerializeField] private List<MenuScreen> screens;

    private Stack<MenuScreen> menuStack = new Stack<MenuScreen>();
    private Dictionary<string, MenuScreen> screenLookup;

    private void Awake()
    {
        screenLookup = new Dictionary<string, MenuScreen>();

        if (mainMenu != null && !string.IsNullOrEmpty(mainMenu.id))
        {
            screenLookup.Add(mainMenu.id, mainMenu);
            SetScreenState(mainMenu, false, instant: true);
        }

        foreach (var screen in screens)
        {
            if (!screenLookup.ContainsKey(screen.id))
                screenLookup.Add(screen.id, screen);

            SetScreenState(screen, false, instant: true);
        }
    }

    private void Start()
    {
        Open(mainMenu.id);
    }

    // ==========================
    // PUBLIC API
    // ==========================

    public void Open(string screenId)
    {
        if (!screenLookup.TryGetValue(screenId, out MenuScreen next))
        {
            Debug.LogWarning($"MenuScreen '{screenId}' not found.");
            return;
        }

        if (menuStack.Count > 0)
        {
            MenuScreen current = menuStack.Peek();
            current.OnBlur?.Invoke();
            SetScreenState(current, false);
        }

        if (next.clearStackOnOpen)
            ClearStack();

        menuStack.Push(next);
        SetScreenState(next, true);
        next.OnEnter?.Invoke();
        next.OnFocus?.Invoke();
    }

    public void GoBack()
    {
        if (menuStack.Count <= 1)
            return;

        MenuScreen current = menuStack.Peek();
        if (current.blockBack)
            return;

        current = menuStack.Pop();
        current.OnExit?.Invoke();
        SetScreenState(current, false);

        MenuScreen previous = menuStack.Peek();
        SetScreenState(previous, true);
        previous.OnFocus?.Invoke();
    }

    public void GoToMainMenu()
    {
        ClearStack();
        Open(mainMenu.id);
    }

    public void Confirm()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().OnConfirm?.Invoke();
    }

    public void Cancel()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().OnCancel?.Invoke();
    }

    // ==========================
    // INTERNAL
    // ==========================

    private void ClearStack()
    {
        while (menuStack.Count > 0)
        {
            MenuScreen screen = menuStack.Pop();
            screen.OnExit?.Invoke();
            SetScreenState(screen, false);
        }
    }

    private void SetScreenState(MenuScreen screen, bool visible, bool instant = false)
    {
        screen.isVisible = visible;

        CanvasGroup cg = screen.canvasGroup;
        cg.alpha = visible ? 1f : 0f;
        cg.blocksRaycasts = visible;
        //cg.interactable = visible;
    }
}