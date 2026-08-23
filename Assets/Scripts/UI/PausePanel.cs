using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PausePanel : BasePanel
{
    void Start()
    {
        InputManager.Instance.goOn.performed += (context) =>
        {
            AudioManager.Instance.PlaySound("UI/ui_button_confirm",false);
            Continue();
        };
    }
    protected override void OnClick(string btnName)
    {
            switch (btnName)
            {
                case "Continue":
                    Continue();
                    break;
                case "Option":
                    StartCoroutine(WaitAndHide("PausePanel", () =>
                    {
                        GameManager.Instance.gameState = GameState.UI;
                        UIManager.Instance.ShowPanel<BasePanel>("OptionMenuPanel");
                    }));
                    break;
                case "BackToMainMenu":
                    StartCoroutine(WaitAndHide("PausePanel", () =>
                    {
                        UIManager.Instance.ClosePanel("KnightPanel");
                        GameManager.Instance.gameState = GameState.UI;
                        Time.timeScale = 1.0f;
                        UIManager.Instance.ClearPanel();
                        SceneManager.LoadScene(0);
                    }));
                    break;
            }
    }

    private void Continue()
    {
        StartCoroutine(WaitAndHide("PausePanel", () =>
        {
            GameManager.Instance.gameState = GameState.Playing;
            Cursor.visible = false;
            EventManager.Instance.EventTrigger("Continue");
            AudioManager.Instance.UnPauseBackgroundMusic();
            AudioManager.Instance.UnPauseAllSounds();
            Time.timeScale = 1.0f;
        }));
    }
}
