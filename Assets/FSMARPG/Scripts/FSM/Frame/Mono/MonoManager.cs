using System;
using UnityEngine.EventSystems;

public class MonoManager : SingletonMono<MonoManager>
{
    private Action UpdateAction;
    private Action LateUpdateAction;
    private Action FixedUpdateAction;



    public void AddUpdateListener(Action action)
    {
        UpdateAction += action;
    }

    public void RemoveUpdateListener(Action action)
    {
        UpdateAction -= action;
    }

    public void AddLateUpdateListener(Action action)
    {
        LateUpdateAction += action;
    }

    public void RemoveLateUpdateListener(Action action)
    {
        LateUpdateAction -= action;
    }

    public void AddFixedUpdateListener(Action action)
    {
        FixedUpdateAction += action;
    }

    public void RemoveFixedUpdateListener(Action action)
    {
        FixedUpdateAction -= action;
    }


    private void Update()
    {
        if (InteractWithUI()) return;
        UpdateAction?.Invoke();
    }

    private void LateUpdate()
    {
        if (InteractWithUI()) return;
        LateUpdateAction?.Invoke();
    }

    private void FixedUpdate()
    {
        if (InteractWithUI()) return;
        FixedUpdateAction?.Invoke();
    }

    bool InteractWithUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
