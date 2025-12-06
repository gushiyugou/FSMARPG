using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootStepController : MonoBehaviour
{
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void Play()
    {
        if(audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }


    public void Stop()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void Pause()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ChangeAudioClip(AudioClip clip)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = clip;
        }
    }
}
