using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AG.Tool.Singleton;

public class GameEventManager : SingletonNonMono<GameEventManager>
{
    private interface IEventHelp{}

    #region 事件相关

    private class EventHelp : IEventHelp
    {
        private event Action _action;

        public EventHelp(Action action)
        {
            _action = action;
        }

        public void AddCall(Action action)
        {
            _action += action;
        }

        public void Call()
        {
            _action?.Invoke();
        }

        public void RemoveCall(Action action)
        {
            _action -= action;
        }
        
        public void CleanEvent()
        {
            _action = null;
        }
    }
    private class EventHelp<T> : IEventHelp
    {
        private event Action<T> _action;

        public EventHelp(Action<T> action)
        {
            _action = action;
        }

        public void AddCall(Action<T> action)
        {
            _action += action;
        }

        public void Call(T value)
        {
            _action?.Invoke(value);
        }

        public void RemoveCall(Action<T> action)
        {
            _action -= action;
        }
        
        public void CleanEvent()
        {
            _action = null;
        }
    }
    private class EventHelp<T1,T2> : IEventHelp
    {
        private event Action<T1,T2> _action;

        public EventHelp(Action<T1,T2> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1,T2> action)
        {
            _action += action;
        }

        public void Call(T1 value1,T2 value2)
        {
            _action?.Invoke(value1,value2);
        }

        public void RemoveCall(Action<T1,T2> action)
        {
            _action -= action;
        }

        public void CleanEvent()
        {
            _action = null;
        }
    }
    private class EventHelp<T1,T2,T3,T4,T5> : IEventHelp
    {
        private event Action<T1,T2,T3,T4,T5> _action;

        public EventHelp(Action<T1,T2,T3,T4,T5> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1,T2,T3,T4,T5> action)
        {
            _action += action;
        }

        public void Call(T1 value1,T2 value2,T3 value3,T4 value4,T5 value5)
        {
            _action?.Invoke(value1,value2,value3,value4,value5);
        }

        public void RemoveCall(Action<T1,T2,T3,T4,T5> action)
        {
            _action -= action;
        }

        public void CleanEvent()
        {
            _action = null;
        }
    }
    
    private class EventHelp<T1,T2,T3,T4> : IEventHelp
    {
        private event Action<T1,T2,T3,T4> _action;

        public EventHelp(Action<T1,T2,T3,T4> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1,T2,T3,T4> action)
        {
            _action += action;
        }

        public void Call(T1 value1,T2 value2,T3 value3,T4 value4)
        {
            _action?.Invoke(value1,value2,value3,value4);
        }

        public void RemoveCall(Action<T1,T2,T3,T4> action)
        {
            _action -= action;
        }

        public void CleanEvent()
        {
            _action = null;
        }
    }
    
    private class EventHelp<T1,T2,T3> : IEventHelp
    {
        private event Action<T1,T2,T3> _action;

        public EventHelp(Action<T1,T2,T3> action)
        {
            _action = action;
        }

        public void AddCall(Action<T1,T2,T3> action)
        {
            _action += action;
        }

        public void Call(T1 value1,T2 value2,T3 value3)
        {
            _action?.Invoke(value1,value2,value3);
        }

        public void RemoveCall(Action<T1,T2,T3> action)
        {
            _action -= action;
        }

        public void CleanEvent()
        {
            _action = null;
        }
    }

    #endregion

    #region 事件管理器监听逻辑

    private Dictionary<string, IEventHelp> _eventCenter = new Dictionary<string, IEventHelp>();


    #region 添加事件监听
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="eventName">事件名字</param>
    /// <param name="action">回调函数</param>
    public void AddEventListening(string eventName, Action action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
            (e as EventHelp)?.AddCall(action);
        else
            _eventCenter.Add(eventName, new EventHelp(action));
    }
    
    public void AddEventListening<T>(string eventName, Action<T> action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
        {
            (e as EventHelp<T>)?.AddCall(action);
            Debug.Log($"事件{eventName}添加成功");
        }
        else
            _eventCenter.Add(eventName,new EventHelp<T>(action));
        

    }
    
    public void AddEventListening<T1,T2>(string eventName, Action<T1,T2> action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
            (e as EventHelp<T1,T2>)?.AddCall(action);
        else
            _eventCenter.Add(eventName,new EventHelp<T1,T2>(action));
        
    }
    
    public void AddEventListening<T1,T2,T3>(string eventName, Action<T1,T2,T3> action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
            (e as EventHelp<T1,T2,T3>)?.AddCall(action);
        else
            _eventCenter.Add(eventName,new EventHelp<T1,T2,T3>(action));
        
    }
    
    public void AddEventListening<T1,T2,T3,T4>(string eventName, Action<T1,T2,T3,T4> action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
            (e as EventHelp<T1,T2,T3,T4>)?.AddCall(action);
        else
            _eventCenter.Add(eventName,new EventHelp<T1,T2,T3,T4>(action));
        
    }
    
    public void AddEventListening<T1,T2,T3,T4,T5>(string eventName, Action<T1,T2,T3,T4,T5> action)
    {
        if (_eventCenter.TryGetValue(eventName, out IEventHelp e))
            (e as EventHelp<T1,T2,T3,T4,T5>)?.AddCall(action);
        else
            _eventCenter.Add(eventName,new EventHelp<T1,T2,T3,T4,T5>(action));
        
    }

    #endregion
    
    #region 通知事件

    public void CallEvent(string eventName)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp)?.Call();
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    public void CallEvent<T>(string eventName,T value)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp<T>)?.Call(value);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    
    
    public void CallEvent<T1,T2>(string eventName, T1 value1,T2 value2)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp<T1,T2>)?.Call(value1, value2);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    public void CallEvent<T1,T2,T3>(string eventName, T1 value1,T2 value2,T3 value3)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp<T1,T2,T3>)?.Call(value1, value2, value3);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    public void CallEvent<T1,T2,T3,T4>(string eventName, T1 value1,T2 value2,T3 value3,T4 value4)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp<T1,T2,T3,T4>)?.Call(value1, value2, value3, value4);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    public void CallEvent<T1,T2,T3,T4,T5>(string eventName, T1 value1,T2 value2,T3 value3,T4 value4,T5 value5)
    {
        if(_eventCenter.TryGetValue(eventName,out IEventHelp e))
            (e as EventHelp<T1,T2,T3,T4,T5>)?.Call(value1, value2, value3, value4, value5);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，请注册事件后通知执行");
    }
    
    #endregion
    
    #region 移除事件监听

    public void RemoveEventListening(string eventName,Action action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }
    
    public void RemoveEventListening<T>(string eventName,Action<T> action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp<T>)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }
    
    public void RemoveEventListening<T1,T2>(string eventName,Action<T1,T2> action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp<T1,T2>)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }
    
    public void RemoveEventListening<T1,T2,T3>(string eventName,Action<T1,T2,T3> action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp<T1,T2,T3>)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }
    
    public void RemoveEventListening<T1,T2,T3,T4>(string eventName,Action<T1,T2,T3,T4> action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp<T1,T2,T3,T4>)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }
    
    public void RemoveEventListening<T1,T2,T3,T4,T5>(string eventName,Action<T1,T2,T3,T4,T5> action)
    {
        if (_eventCenter.TryGetValue(eventName,out var e))
            (e as EventHelp<T1,T2,T3,T4,T5>)?.RemoveCall(action);
        else
            Debug.LogWarning($"事件==>{eventName}<==不存在，移除失败");
    }


    public void CleanEvent(string eventName)
    {
        if (_eventCenter.TryGetValue(eventName, out var e))
            (e as EventHelp).CleanEvent();
        _eventCenter.Remove(eventName);
    }
    #endregion
    

    #endregion
}
