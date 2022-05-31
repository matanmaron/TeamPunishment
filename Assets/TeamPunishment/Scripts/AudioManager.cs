using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource Music;
    [SerializeField] AudioSource SFX1;

    public static AudioManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Music.Play();
        Music.Pause();
    }

    public void PlayMusic()
    {
        Debug.Log("[AudioManager] - PlayMusic");
        Music.UnPause();
    }

    public void StopMusic()
    {
        Debug.Log("[AudioManager] - StopMusic");
        Music.Pause();
    }
}
