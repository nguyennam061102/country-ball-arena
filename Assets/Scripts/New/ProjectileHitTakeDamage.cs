using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHitTakeDamage : RayHitEffect
{
    private bool done;
    public float damagePercent = 0.02f;
    public int damageAddCount = 1;
    public override HasToReturn DoHitEffect(HitInfo hit)
    {
        if (done)
        {
            //return HasToReturn.canContinue;
        }

        if (hit.collider.CompareTag("Player"))
        {
            float RealDamage = (damagePercent * damageAddCount) * GetComponentInParent<SpawnedAttack>().spawner.maxHealth;
            GetComponentInParent<SpawnedAttack>().spawner.healthHandler.TakeDamage(RealDamage * Vector2.up, RealDamage, null);
        }
        done = true;
        return HasToReturn.hasToReturn;
    }
}
