using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class VolumeMenuPanel : BasePanel
{
    public void SetGlobalVolume()
    {
        float value = GetControl<Slider>("GlobalVolume").value;
        GetControl<TextMeshProUGUI>("GlobalVolumeValue").text = value.ToString();
        AudioManager.Instance.SetGlobalVolume(value);
        AudioManager.Instance.PlaySound("UI/ui_option_click",false);
    }

    public void SetMusicVolume()
    {
        float value = GetControl<Slider>("MusicVolume").value;
        GetControl<TextMeshProUGUI>("MusicVolumeValue").text = value.ToString();
        AudioManager.Instance.SetBackgroundMusicVolume(value);
        AudioManager.Instance.PlaySound("UI/ui_option_click",false);
    }

    public void SetSoundVolume()
    {
        float value = GetControl<Slider>("SoundVolume").value;
        GetControl<TextMeshProUGUI>("SoundVolumeValue").text = value.ToString();
        AudioManager.Instance.SetSoundVolume(value);
        AudioManager.Instance.PlaySound("UI/ui_option_click",false);
    }
    
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "BackToDefault":
                GetControl<Slider>("GlobalVolume").value = 10;
                GetControl<TextMeshProUGUI>("GlobalVolumeValue").text = "10";
                AudioManager.Instance.SetGlobalVolume(10);
                GetControl<Slider>("MusicVolume").value = 10;
                GetControl<TextMeshProUGUI>("MusicVolumeValue").text = "10";
                AudioManager.Instance.SetBackgroundMusicVolume(10);
                GetControl<Slider>("SoundVolume").value = 10;
                GetControl<TextMeshProUGUI>("SoundVolumeValue").text = "10";
                AudioManager.Instance.SetSoundVolume(10);
                break;
            case "Back":
                StartCoroutine(WaitAndHide("VolumeMenuPanel", () =>
                {
                    UIManager.Instance.BackToLast();
                }));
                break;
        }        
    }
}
