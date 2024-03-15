//using Sonigon;

using System.Collections;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    // [Header("Sounds")]
    // public SoundEvent soundPhoenixActivate;
    //
    // public SoundEvent soundPhoenixChargeLoop;
    //
    // public SoundEvent soundPhoenixRespawn;
    //
    // private SoundParameterIntensity soundParameterChargeLoopIntensity = new SoundParameterIntensity(0f, UpdateMode.Continuous);

    //[Header("Settings")]
    // public float forceMulti = 1f;
    //
    // public float minScale = 0.9f;
    //
    // public float maxScale = 1.1f;
    //
    // public float minDrag = 0.9f;
    //
    // public float maxDrag = 1.1f;
    //
    // public float minForce = 0.9f;
    //
    // public float maxForce = 1.1f;
    //
    // public float spread = 0.5f;

    //private Rigidbody2D[] rigs;

    //private Color baseColor;

    private ParticleSystem[] parts;

    public ParticleSystem partToColor;

    public ParticleSystem[] partsToColor;

    private float respawnTimeCurrent;

    private float respawnTime = 2.3f;

    public void PlayDeath(Vector2 vel, Color color, int playerIDToRevive = -1)
    {
        if (vel.magnitude < 30f)
        {
            vel = vel.normalized * 30f;
        }

        vel *= 1f;
        parts = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < parts.Length; i++)
        {
            ParticleSystem.MainModule main = parts[i].main;
            if (parts[i].name.Contains("ROT"))
            {
                parts[i].transform.rotation = Quaternion.LookRotation(vel);
            }
        }

        for (int j = 0; j < partsToColor.Length; j++)
        {
            var mainModule = partsToColor[j].main;
            mainModule.startColor = color;
        }

        if (playerIDToRevive != -1)
        {
            StartCoroutine(RespawnPlayer(playerIDToRevive));
        }
    }

    public void Revive(int playerID, Color color) //watch video revive
    {
        for (int j = 0; j < partsToColor.Length; j++)
        {
            var mainModule = partsToColor[j].main;
            mainModule.startColor = color;
        }
        StartCoroutine(RespawnPlayer(playerID));
    }

    private IEnumerator RespawnPlayer(int playerIDToRevive = -1)
    {
        //Debug.Log($"ID: {playerIDToRevive}");
        while (respawnTimeCurrent < respawnTime)
        {
            respawnTimeCurrent += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        PlayerManager.Instance.players[playerIDToRevive].healthHandler.Revive(isFullRevive: false);
    }
}