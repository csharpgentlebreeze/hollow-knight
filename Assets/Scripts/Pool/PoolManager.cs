using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    public GameObject parentObject;
    public Queue<GameObject> objectPool = new Queue<GameObject>();

    public PoolData(GameObject obj,GameObject poolParent)
    {
        parentObject = new GameObject(obj.name);
        
        parentObject.transform.SetParent(poolParent.transform);
        objectPool = new Queue<GameObject>();
    }
    
    public GameObject GetObject()
    {
        GameObject obj = objectPool.Dequeue();
        
        obj.SetActive(true);
        
        obj.transform.parent = null;
        
        return obj;
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.transform.SetParent(parentObject.transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}

public class PoolManager : MonoSingleton<PoolManager>
{
    public Dictionary<string, PoolData> pool;
    
    private GameObject poolParent;
    
    public void Get(string path, UnityAction<GameObject> callback)
    {
        if (pool.ContainsKey(path) && pool[path].objectPool.Count > 0)
        {
            callback(pool[path].GetObject());
        }
        else
        {
            ResourceManager.Instance.LoadAsync<GameObject>(path, (o) =>
            {
                o.name = path;
                callback(o);
            });
        } ;
    }
    
    public void Push(string path, GameObject obj)
    {
        if (!pool.ContainsKey(path))
        {
            pool[path] = new PoolData(obj,poolParent);
        }
        pool[path].ReturnObject(obj);
    }
    
    public void ClearPool()
    {
        foreach (var item in pool)
        {
            while (item.Value.objectPool.Count > 0)
            {
                GameObject obj = item.Value.objectPool.Dequeue();
                Destroy(obj);
            }
        }
        poolParent = null;
        pool.Clear();
    }

    protected void Awake()
    {
        base.Awake();
        pool = new Dictionary<string, PoolData>();
        if (poolParent == null)
        {
            poolParent = new GameObject("Pool");
        }
    }
}
