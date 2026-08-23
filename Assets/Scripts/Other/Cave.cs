using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : MonoBehaviour
{
    public AudioSource cave_noise;

    public AudioSource cave_wind;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayAudio(cave_noise);
        AudioManager.Instance.PlayAudio(cave_wind);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
