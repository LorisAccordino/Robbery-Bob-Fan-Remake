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

    [Header("Lifecycle Events")]
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public UnityEvent OnFocus;
    public UnityEvent OnBlur;

    [Header("Custom Events")]
    public UnityEvent OnConfirm;
    public UnityEvent OnCancel;

    [HideInInspector] public bool isVisible;
}