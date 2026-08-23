using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

/// <summary>
/// 个体音效控制器,负责个体3D立体声效
/// </summary>
public class AudioController : MonoBehaviour
{
    private AudioMixer audioMixer;
    private List<AudioSource> soundList = new List<AudioSource>();
    
    private IEnumerator PlaySound(AudioSource sound,UnityAction<AudioSource> callBack)
    {
        foreach (AudioSource s in soundList)
        {
            if (s == sound)
            {
                sound.volume = Mathf.Log(sound.volume);
            }
            else
            {
                sound.volume = sound.volume;
            }
        }
        sound.Play();
        soundList.Add(sound);
        if(callBack != null)callBack(sound);
        if (sound.loop == false)
        {
            yield return new WaitForSecondsRealtime(sound.clip.length);
            StopSound(sound);
        }
    }

    private IEnumerator PlaySound(AudioClip clip,bool isLoop,float volume, UnityAction<AudioSource> callBack)
    {
        AudioSource sound = gameObject.AddComponent<AudioSource>();
        sound.spatialBlend = 1.0f;
        sound.rolloffMode = AudioRolloffMode.Linear;
        sound.minDistance = 1f;
        sound.maxDistance = 20f;
        sound.volume = volume;
        sound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];
        sound.clip = clip;
        sound.loop = isLoop;
        sound.Play();
        soundList.Add(sound);
        if(callBack != null)callBack(sound);
        if (!isLoop)
        {
            yield return new WaitForSeconds(clip.length);
            StopSound(sound);
        }
    }
    
    private IEnumerator PlaySound(AudioClip clip,float volume, UnityAction<AudioSource> onComplete)
    {
        AudioSource sound = gameObject.AddComponent<AudioSource>();
        sound.spatialBlend = 1.0f;
        sound.rolloffMode = AudioRolloffMode.Linear;
        sound.minDistance = 1f;
        sound.maxDistance = 20f;
        sound.volume = volume;
        sound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Sound")[0];
        sound.clip = clip;
        sound.loop = false;
        sound.Play();
        soundList.Add(sound);
        yield return new WaitForSeconds(clip.length);
        StopSound(sound);
        if(onComplete != null)onComplete(sound);
    }

    protected void Awake()
    {
        audioMixer = ResourceManager.Instance.Load<AudioMixer>("AudioMixer/GameAudio");
    }

    private void Update()
    {
        
    }

    public void PlayAudio(AudioSource sound,UnityAction<AudioSource> callBack = null)
    {
        StartCoroutine(PlaySound(sound,callBack));
    }
    
    public void PlaySound(string soundName,bool isLoop,float volume = 1, UnityAction<AudioSource> callBack = null)
    {
        ResourceManager.Instance.LoadAsync<AudioClip>("AudioClip/" + soundName, (clip) =>
        {
            StartCoroutine(PlaySound(clip,isLoop,volume,callBack));
        });
    }
    
    public void PlaySoundWithComplete(string soundName,float volume = 1, UnityAction<AudioSource> onComplete = null)
    {
        ResourceManager.Instance.LoadAsync<AudioClip>("AudioClip/" + soundName, (clip) =>
        {
            StartCoroutine(PlaySound(clip,volume,onComplete));
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
