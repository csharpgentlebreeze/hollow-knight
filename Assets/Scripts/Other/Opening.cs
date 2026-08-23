using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Opening : MonoBehaviour
{
    public VideoPlayer prologue;
    public VideoPlayer intro;
    
    private VideoPlayer nowVideo;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener("Continue", () =>
        {
            if (nowVideo != null)
            {
                nowVideo.Play();
            }
        });
        EventManager.Instance.AddEventListener("Paused", () =>
        {
            if (nowVideo != null)
            {
                nowVideo.Pause();
            }
        });
        Cursor.visible = false;
        AudioManager.Instance.PlaySound("Opening/short_piano_for_opening_text", false);
        prologue.loopPointReached += (v) =>
        {
            v.Stop();
            intro.Play();
            nowVideo = intro;
        };
        intro.loopPointReached += (v) =>
        {
            v.Stop();
            nowVideo = null;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        };
    }

    public void Play()
    {
        prologue.Play();
        nowVideo = prologue;
    }
}
