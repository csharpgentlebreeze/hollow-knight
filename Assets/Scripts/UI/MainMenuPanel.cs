using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "StartGame":
                StartCoroutine(WaitAndHide("MainMenuPanel", () =>
                {
                    GameManager.Instance.gameState = GameState.Playing;
                    AudioManager.Instance.ClearSoundList();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    UIManager.Instance.ClearPanel();
                }));
                break;
            case "Option":
                StartCoroutine(WaitAndHide("MainMenuPanel", () =>
                {
                    UIManager.Instance.ShowPanel<BasePanel>("OptionMenuPanel");
                }));
                break;
            case "Achievement":
                /*StartCoroutine(WaitAndClose("MainMenuPanel", () =>
                {
                    UIManager.Instance.ShowPanel<BasePanel>("AchievementMenuPanel");
                }));*/
                break;
            case "QuitGame":
                StartCoroutine(WaitAndHide("MainMenuPanel", () =>
                {
                    Application.Quit();
                }));
                break;
        }
    }
}
