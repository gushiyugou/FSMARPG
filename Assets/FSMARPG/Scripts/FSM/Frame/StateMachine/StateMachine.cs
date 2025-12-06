using System;
using System.Collections.Generic;


public interface IStateMachineOwner { }
public class StateMachine
{
    private IStateMachineOwner owner;

    private Dictionary<Type, StateBase> stateDictionary = new Dictionary<Type, StateBase>();

    public  Type CurrentStateType { get => currentState.GetType(); }
    public bool IsHasState { get => currentState != null; }
    private StateBase currentState;
    public StateBase CurrentState { get => currentState; }
    /// <summary>
    /// 状态机初始化
    /// </summary>
    /// <param name="owner">状态机的宿主</param>
    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <typeparam name="T">要切换的状态类型</typeparam>
    /// <param name="reCurrent">是否刷新状态</param>
    /// <returns></returns>
    public bool ChangeState<T>(bool reCurrent = false) where T :StateBase,new()
    {
        //PlayerStateBase.GetPerviousState(currentState);
        //当前有状态同时当前状态与要切换的状态类型一致且不需要刷新则直接返回
        if (IsHasState && CurrentStateType == typeof(T) && !reCurrent) return false;


        //当前有状态则先退出当前状态
        if (currentState != null)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        currentState = GetState<T>();
        currentState.Enter();
        MonoManager.Instance.AddUpdateListener(currentState.Update);
        MonoManager.Instance.AddLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.AddFixedUpdateListener(currentState.FixedUpdate);
        
        return true;
    }


    private StateBase GetState<T>() where T : StateBase, new()
    {
        Type type = typeof(T);
        //判断是否存在该状态类
        if(!stateDictionary.TryGetValue(type, out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDictionary.Add(type, state);
        }
        return state;
    }

    
    
    /// <summary>
    /// 结束状态
    /// </summary>
    public void Stop()
    {
        currentState.Exit();
        MonoManager.Instance.RemoveUpdateListener(currentState.Update);
        MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        currentState = null;

        foreach(var item in stateDictionary.Values)
        {
            item.UnInit();
        }
        stateDictionary.Clear();
    }
}
