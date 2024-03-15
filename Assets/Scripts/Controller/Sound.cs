using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Sound
{
    public enum SoundData
    {
        Jump,
        OffHandBlock,
        OffHandBombard,
        OffHandIceStorm,
        OffHandRing,
        OffHandMedic,
        ButtonClick,
        CardSelected,
        DiamondPurchase,
        GoldPurchase,
        Upgrade,
    }

    public static void Play(SoundData data)
    {
        if (!GameData.Sound) return;
        SoundManager.Instance.PlayOneShot(Clip(data));
    }

    private static AudioClip Clip(SoundData data)
    {
        return (from sound in SoundManager.Instance.soundList where sound.data == data select sound.clip).FirstOrDefault();
    }
}

