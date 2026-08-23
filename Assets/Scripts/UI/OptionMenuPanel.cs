using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "Game":
                
                break;
            case "Volume":
                StartCoroutine(WaitAndHide("OptionMenuPanel", () =>
                {
                    UIManager.Instance.ShowPanel<BasePanel>("VolumeMenuPanel");
                }));
                break;
            case "Video":
                
                break;
            case "Keyboard":
                
                break;
            case "Back":
                StartCoroutine(WaitAndHide("OptionMenuPanel", () =>
                {
                    UIManager.Instance.BackToLast();
                }));
                break;
        }        
    }
}
