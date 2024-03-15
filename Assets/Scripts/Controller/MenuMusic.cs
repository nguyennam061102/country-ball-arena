using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenuMusic : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] clips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.Play();
        SetAudio();
        GameData.onMusicChanged += SetAudio;
    }

    void SetAudio()
    {
        if (audioSource != null)
        {
            audioSource.mute = !GameData.Music;
        }
    }
}
