using System;
using UnityEngine;

public class StunPlayer : MonoBehaviour
{
    public enum TargetPlayer
    {
        OtherPlayer,
        Self
    }

    public TargetPlayer targetPlayer;

    public float time = 0.5f;

    private Player target;

    public SpawnedAttack spawnedAttack;

    private void Start()
    {
        spawnedAttack = GetComponent<SpawnedAttack>();
        time = spawnedAttack.spawner.stats.stunTime;
    }

    public void Go()
    {
        if (!target)
        {
            target = GetComponentInParent<Player>();
            if (targetPlayer == TargetPlayer.OtherPlayer)
            {
                //target = PlayerManager.Instance.GetOtherPlayer(target);
            }
        }

        if ((bool) target) target.stunHandler.AddStun(time);
    }
}