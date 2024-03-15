using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitWall, hitCharacter;

    public void PlaySound(bool wall = true)
    {
        audioSource.PlayOneShot(wall ? hitWall : hitCharacter);
    }
}
