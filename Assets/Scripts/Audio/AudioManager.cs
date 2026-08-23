using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

/// <summary>
/// 全局音效管理器
/// </summary>
public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource backgroundMusic;
    private AudioMixer audioMixer;
    
    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    
    private IEnumerator PlaySound(AudioSource sound,UnityAction<AudioSource> callBack)
    {
        sound.Play();
        soundList.Add(sound);
        if(callBack != null)callBack(sound);
        if (sound.loop == false)
        {
            yield return new WaitForSecondsRealtime(sound.clip.length);
            sound.Stop();
        }
    }

    private IEnumerator PlaySound(AudioClip clip,bool isLoop,float volume,UnityAction<AudioSource> callBack)
    {
        AudioSource sound = soundObj.AddComponent<AudioSource>();
        foreach (AudioSource s in soundList)
        {
            if (s.clip == clip)
            {
                sound.volume = Mathf.Log(volume);
            }
            else
            {
                sound.volume = volume;
            }
        }
        sound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];
        sound.clip = clip;
        sound.loop = isLoop;
        sound.Play();
        soundList.Add(sound);
        if(callBack != null)callBack(sound);
        if (!isLoop)
        {
            yield return new WaitForSecondsRealtime(clip.length);
            StopSound(sound);
        }
    }
    
    private IEnumerator PlaySound(AudioClip clip,float volume, UnityAction<AudioSource> onComplete)
    {
        AudioSource sound = soundObj.AddComponent<AudioSource>();
        foreach (AudioSource s in soundList)
        {
            if (s.clip == clip)
            {
                sound.volume = Mathf.Log(volume);
            }
            else
            {
                sound.volume = volume;
            }
        }
        sound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];
        sound.clip = clip;
        sound.loop = false;
        sound.Play();
        soundList.Add(sound);
        yield return new WaitForSecondsRealtime(clip.length);
        StopSound(sound);
        if(onComplete != null)onComplete(sound);
    }

    protected void Awake()
    {
        base.Awake();
        audioMixer = ResourceManager.Instance.Load<AudioMixer>("AudioMixer/GameAudio");
        if (soundObj == null)
        {
            soundObj = new GameObject("Sound");
            // 防止场景切换时被销毁
            DontDestroyOnLoad(soundObj);
            // 将 soundObj 设为 AudioManager 的子对象，方便管理
            soundObj.transform.SetParent(this.transform);
        }
    }

    private void Update()
    {
        
    }

    public void SetGlobalVolume(float volume)
    {
        float normalizedVolume = volume / 10f;
        if (normalizedVolume < 0.1f)
        {
            audioMixer.SetFloat("Global", -80f);
        }
        else
        {
            audioMixer.SetFloat("Global",Mathf.Log10(normalizedVolume) * 20);
        }
        
    }
    
    public void PlayBackgroundMusic(string musicName)
    {
        if (backgroundMusic == null)
        {
            GameObject go = new GameObject("BackgroundMusic");
            backgroundMusic = go.AddComponent<AudioSource>();
            backgroundMusic.volume = 1f;
            backgroundMusic.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            
            ResourceManager.Instance.LoadAsync<AudioClip>("AudioClip/" + musicName, (clip) =>
            {
                backgroundMusic.clip = clip;
                backgroundMusic.loop = true;
                backgroundMusic.Play();
            });
        }
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        if (backgroundMusic == null) return;
        float normalizedVolume = volume / 10f;
        if (normalizedVolume < 0.1f)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            audioMixer.SetFloat("Music",Mathf.Log10(normalizedVolume) * 20);
        }
    }

    public void PauseBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        backgroundMusic.Pause();
    }
    public void UnPauseBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        backgroundMusic.UnPause();
    }
    
    public void StopBackgroundMusic()
    {
        if (backgroundMusic == null) return;
        backgroundMusic.Stop();
    }
    
    public void PlaySound(string soundName,bool isLoop, float volume = 1f,UnityAction<AudioSource> callBack = null)
    {
        // 尝试同步加载已存在的音频，避免在立即切换场景时异步回调来不及触发
        AudioClip clip = ResourceManager.Instance.Load<AudioClip>("AudioClip/" + soundName);
        if (clip != null)
        {
            StartCoroutine(PlaySound(clip,isLoop,volume,callBack));
            return;
        }
        // 回退到异步加载
        ResourceManager.Instance.LoadAsync<AudioClip>("AudioClip/" + soundName, (loadedClip) =>
        {
            StartCoroutine(PlaySound(loadedClip,isLoop,volume,callBack));
        });
    }
    
    public void PlayAudio(AudioSource sound,UnityAction<AudioSource> callBack = null)
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (soundList[i] == sound)
            {
                soundList[i].Play();
                return;
            }
        }
        StartCoroutine(PlaySound(sound,callBack));
    }

    public void PlaySoundWithComplete(string soundName,float volume = 1, UnityAction<AudioSource> onComplete = null)
    {
        AudioClip clip = ResourceManager.Instance.Load<AudioClip>("AudioClip/" + soundName);
        if (clip != null)
        {
            StartCoroutine(PlaySound(clip,volume,onComplete));
            return;
        }
        ResourceManager.Instance.LoadAsync<AudioClip>("AudioClip/" + soundName, (loadedClip) =>
        {
            StartCoroutine(PlaySound(loadedClip,volume,onComplete));
        });
    }

    public void SetSoundVolume(float volume)
    {
        float normalizedVolume = volume / 10f;
        if (normalizedVolume < 0.1f)
        {
            audioMixer.SetFloat("Sound", -80f);
        }
        else
        {
            audioMixer.SetFloat("Sound",Mathf.Log10(normalizedVolume) * 20);
        }
    }

    public void PauseAllSounds()
    {
        foreach (AudioSource sound in soundList)
        {
            if (sound != null)
            {
                sound.Pause();
            }
        }
    }

    public void PauseSound(AudioSource audio)
    {
        foreach (AudioSource sound in soundList)
        {
            if (sound == audio)
            {
                sound.Pause();
            }
        }
    }

    public void UnPauseAllSounds()
    {
        foreach (AudioSource sound in soundList)
        {
            if (sound != null)
            {
                sound.UnPause();
            }
        }
    }
    
    public void UnPauseSound(AudioSource audio)
    {
        foreach (AudioSource sound in soundList)
        {
            if (sound == audio)
            {
                sound.UnPause();
            }
        }
    }
    
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            Destroy(source);
        }
    }

    public void ClearSoundList()
    {
        soundList.Clear();
    }
}
