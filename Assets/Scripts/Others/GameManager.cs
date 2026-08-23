using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 管理游戏状态，控制游戏的暂停和继续
/// </summary>
public enum GameState
{
    UI,
    Playing,
    Paused,
}
public class GameManager : MonoSingleton<GameManager>
{
    public GameState gameState;
    private void Start()
    {
        gameState = GameState.UI;
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
        };
    }
}
