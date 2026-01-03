using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MenuScreen
{
    [Header("Identity")]
    public string id;

    [Header("UI")]
    public CanvasGroup canvasGroup;

    [Header("Behaviour")]
    public bool blockBack = false;
    public bool clearStackOnOpen = false;

    [Header("Events")]
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
}