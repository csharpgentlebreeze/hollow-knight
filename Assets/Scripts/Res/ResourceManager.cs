using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//异步加载
//委托和Lambda表达式
//协程
//泛型

public class ResourceManager : MonoSingleton<ResourceManager>
{
    public T Load<T>(string path) where T : Object
    {
        T res = Resources.Load<T>(path);

        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }
    
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(ReallyLoadAsync(path, callback));
    }
    
    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callback) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(path);
        while (!request.isDone)
        {
            EventManager.Instance.EventTrigger("LoadProgress", request.progress);
            yield return request.progress;
        }
        yield return request;
        T res = request.asset as T;
        if (res is GameObject)
        {
            callback(Instantiate(res));
        }
        else
        {
            callback(res);
        }
    }
}
