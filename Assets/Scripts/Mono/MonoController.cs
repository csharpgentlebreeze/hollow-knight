using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Mono的代理

public class MyEventArgs<T> : EventArgs
{
    public T info;

    public MyEventArgs(T info)
    {
        this.info = info;
    }
}
public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    private event UnityAction<EventArgs> updateEventWithArgs; 
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent();
        }
    }
    
    public void AddUpdateListener(UnityAction action)
    {
        updateEvent += action;
    }
    
    public void RemoveUpdateListener(UnityAction action)
    {
        updateEvent -= action;
    }

    public void AddUpdateListener(UnityAction<EventArgs> action)
    {
        updateEventWithArgs += action;
    }

    public void RemoveUpdateListener(UnityAction<EventArgs> action)
    {
        updateEventWithArgs -= action;
    }
}
