using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;




public class GameManager : PersistentMonoSingleton<GameManager>
{
    private IRunTimeDataModel data;
    public void OnSingletonInit()
    {
        
    }
    
    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        data = GameArchitecture.Interface.GetModel<IRunTimeDataModel>();
        data.GameStatus.Register((state) =>
        {
            if (state == GameState.Menu)
            {
                data.WantoEsc.UnRegister(Paused);
                Time.timeScale = 1.0f;
                Cursor.visible = true;
            }
            else if (state == GameState.Playing)
            {
                data.WantoEsc.Register(Paused);
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                
            }
            else if (state == GameState.Paused)
            {
                data.WantoEsc.UnRegister(Paused);
                Time.timeScale = 0.0f;
                Cursor.visible = true;
            }
        });
        /*gameState = GameState.UI;
        InputManager.Instance.pause.performed += (context) =>
        {
            gameState = GameState.Paused;
            Cursor.visible = true;
            EventManager.Instance.EventTrigger("Paused");
            AudioManager.Instance.PauseBackgroundMusic();
            AudioManager.Instance.PauseAllSounds();
            AudioManager.Instance.PlaySound("UI/ui_button_confirm",false);
            Time.timeScale = 0.0f;
            UIManager.Instance.ShowPanel<BasePanel>("PausePanel");
        };*/
    }

    private void Update()
    {
        
    }

    private void Paused(bool value)
    {
        if(value) UIKit.OpenPanel<PausePanel>();
    }
}
