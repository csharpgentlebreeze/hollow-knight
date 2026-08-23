using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{
    
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> action;

    public EventInfo(UnityAction<T> action)
    {
        this.action += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction action;

    public EventInfo(UnityAction action)
    {
        this.action += action;
    }
}

public class EventManager : MonoSingleton<EventManager>
{
    public Dictionary<string,IEventInfo> eventDict = new Dictionary<string,IEventInfo>();

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDict.ContainsKey(name))
        {
            (eventDict[name] as EventInfo<T>).action += action;
        }
        else
        {
            eventDict.Add(name, new EventInfo<T>(action));
        }
    }
    
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDict.ContainsKey(name))
        {
            (eventDict[name] as EventInfo).action += action;
        }
        else
        {
            eventDict.Add(name, new EventInfo(action));
        }
    }

    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDict.ContainsKey(name))
        {
            (eventDict[name] as EventInfo<T>).action -= action;
        }
    }
    
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDict.ContainsKey(name))
        {
            (eventDict[name] as EventInfo).action -= action;
        }
    }
    
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDict.ContainsKey(name) && (eventDict[name] as EventInfo<T>).action != null)
        {
            (eventDict[name] as EventInfo<T>).action.Invoke(info);
        }
    }
    
    public void EventTrigger(string name)
    {
        if (eventDict.ContainsKey(name) && (eventDict[name] as EventInfo).action != null)
        {
            (eventDict[name] as EventInfo).action.Invoke();
        }
    }
    
    public void Clear()
    {
        eventDict.Clear();
    }
}
