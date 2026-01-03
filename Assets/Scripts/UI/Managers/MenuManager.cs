using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuScreen mainMenu;
    [SerializeField] private List<MenuScreen> screens;

    private Stack<MenuScreen> menuStack = new();
    private Dictionary<string, MenuScreen> screenLookup;

    public event System.Action<MenuScreen> OnScreenOpened;
    public event System.Action<MenuScreen> OnScreenClosed;

    private void Awake()
    {
        screenLookup = new Dictionary<string, MenuScreen>();
        if (mainMenu != null) screenLookup[mainMenu.id] = mainMenu;
        foreach (var s in screens)
            if (!screenLookup.ContainsKey(s.id)) screenLookup[s.id] = s;
    }

    private void Start() => Open(mainMenu.id);

    public void Open(string screenId)
    {
        if (!screenLookup.TryGetValue(screenId, out MenuScreen next))
        {
            Debug.LogWarning($"MenuScreen '{screenId}' not found.");
            return;
        }

        MenuScreen current = menuStack.Count > 0 ? menuStack.Peek() : null;
        if (current != null)
            OnScreenClosed?.Invoke(current);

        if (next.clearStackOnOpen)
            ClearStack();

        menuStack.Push(next);
        OnScreenOpened?.Invoke(next);
    }

    public void GoToMainMenu()
    {
        // Clear the current stack
        ClearStack();

        // Open the main menu
        if (mainMenu != null)
        {
            menuStack.Push(mainMenu);
            OnScreenOpened?.Invoke(mainMenu);
        }
    }

    public void GoBack()
    {
        if (menuStack.Count <= 1) return;

        MenuScreen current = menuStack.Pop();
        if (current.blockBack)
        {
            menuStack.Push(current);
            return;
        }

        OnScreenClosed?.Invoke(current);
        OnScreenOpened?.Invoke(menuStack.Peek());
    }

    private void ClearStack()
    {
        while (menuStack.Count > 0)
        {
            var s = menuStack.Pop();
            OnScreenClosed?.Invoke(s);
        }
    }
}