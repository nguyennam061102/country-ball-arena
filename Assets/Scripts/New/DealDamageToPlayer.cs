using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageToPlayer : MonoBehaviour
{
    public enum TargetPlayer
    {
        Own,
        Other
    }

    [Header("Settings")] 
    
    public float damagePercent = 0.02f;

    public int damageAddCount = 1;

    public bool lethal = true;

    public TargetPlayer targetPlayer;

    private BaseCharacter data;

    BaseCharacter[] target;

    private float RealDamage => (damagePercent * damageAddCount) * data.maxHealth;

    private void Start()
    {
        data = GetComponentInParent<BaseCharacter>();

        if (!data.dealDamageToPlayer)
        {
            data.dealDamageToPlayer = this;
        }
        else
        {
            data.dealDamageToPlayer.damageAddCount += this.damageAddCount;
            Destroy(this.gameObject);
        }
    }

    public void Go()
    {
        if (targetPlayer == TargetPlayer.Other)
        {
            target = PlayerManager.Instance.GetPlayersNotInID(data.player.playerID);
        }
        else if (targetPlayer == TargetPlayer.Own)
        {
            target = PlayerManager.Instance.GetPlayersInID(data.player.playerID);
        }

        foreach (Player pl in target)
        {
            pl.healthHandler.TakeDamage(RealDamage * Vector2.up, RealDamage, null);
        }    
    }
}
