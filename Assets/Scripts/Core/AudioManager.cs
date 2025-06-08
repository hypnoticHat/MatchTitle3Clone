using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.time = 0f;
        musicSource.Play();
    }
    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.UnPause();
    }

    public float GetPlaybackTime()
    {
        return musicSource.time;
    }
}

