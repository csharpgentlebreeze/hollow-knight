using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Mono的管理器
public class MonoManager : MonoSingleton<MonoManager>
{
    public MonoController monoController;

    protected void Awake()
    {
        base.Awake();
        GameObject obj = new GameObject("MonoController");
        monoController = obj.AddComponent<MonoController>();
    }
    
    public void AddUpdateListener(UnityAction action)
    {
        monoController.AddUpdateListener(action);
    }
    
    public void RemoveUpdateListener(UnityAction action)
    {
        monoController.RemoveUpdateListener(action);
    }

    public void AddUpdateListener(UnityAction<EventArgs> action)
    {
        monoController.AddUpdateListener(action);
    }
    
    public void RemoveUpdateListener(UnityAction<EventArgs> action)
    {
        monoController.RemoveUpdateListener(action);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return monoController.StartCoroutine(routine);
    }
}
