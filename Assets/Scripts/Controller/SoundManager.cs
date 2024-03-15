using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : DontDestroy<SoundManager>
{
    [Serializable]
    public class SoundSource
    {
        public Sound.SoundData data;
        public AudioClip clip;
    }

    public SoundSource[] soundList;
    private AudioSource AudioSource => GetComponent<AudioSource>();

    public void PlayOneShot(AudioClip clip)
    {
        AudioSource.PlayOneShot(clip);
    }
}
