using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenesManager : MonoSingleton<ScenesManager>
{
    
    //同步加载
    public void LoadScene(string sceneName, UnityAction callback)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        callback();
    }
    
    //异步加载
    public void LoadSceneAsync(string sceneName, UnityAction callback)
    {
        MonoManager.Instance.StartCoroutine(ReallyLoadSceneAsync(sceneName, callback));
    }

    private IEnumerator ReallyLoadSceneAsync(string sceneName, UnityAction callback)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            EventManager.Instance.EventTrigger("LoadSceneProgress", operation.progress);
            yield return operation.progress;
        }
        yield return operation;
        callback();
    }
}
