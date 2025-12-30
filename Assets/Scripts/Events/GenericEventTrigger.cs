using UnityEngine;
using UnityEngine.Events;

public class GenericEventTrigger : MonoBehaviour
{
    [Header("Unity Events")]
    public UnityEvent OnStart;
    public UnityEvent OnEnableEvent;
    public UnityEvent OnDisableEvent;

    private void Start()
    {
        OnStart?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    private void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }
}