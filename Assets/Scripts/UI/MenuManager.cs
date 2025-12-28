using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;

    private Stack<GameObject> menuStack = new Stack<GameObject>();

    void Start()
    {
        OpenMenu(mainMenu);
    }

    public void OpenMenu(GameObject menu)
    {
        if (menuStack.Count > 0)
            menuStack.Peek().SetActive(false);

        menu.SetActive(true);
        menuStack.Push(menu);
    }

    public void GoBack()
    {
        if (menuStack.Count <= 1)
            return;

        GameObject current = menuStack.Pop();
        current.SetActive(false);

        menuStack.Peek().SetActive(true);
    }

    public void GoToMainMenu()
    {
        while (menuStack.Count > 1)
        {
            GameObject menu = menuStack.Pop();
            menu.SetActive(false);
        }

        menuStack.Peek().SetActive(true);
    }
}