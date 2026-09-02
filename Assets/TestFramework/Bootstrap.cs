using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Bootstrap : PersistentMonoSingleton<Bootstrap>
{
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ReEnterMainMenu();
        }
    }
    private void ReEnterMainMenu()
    {
        StartCoroutine(Begin());
    }
    
    IEnumerator Begin()
    {
        yield return new WaitForSeconds(0.2f);
        /*AudioManager.Instance.PlayBackgroundMusic("UI/MainStartPanel_BGM");*/
        UIKit.OpenPanel<MainMenuPanel>();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

