using UnityEngine;

public class ProjectileHitEmpower : RayHitEffect
{
	//todo: Add to E_Empower when want to DoBlock when projectile hit something
	private bool done;

	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (done)
		{
			//return HasToReturn.canContinue;
		}

		if (hit.collider.CompareTag("Player"))
		{
			GetComponentInParent<SpawnedAttack>().spawner.block.DoBlockAtPosition(firstBlock: true, dontSetCD: true, BlockTrigger.BlockTriggerType.Empower, hit.point - (Vector2)base.transform.forward * 0.05f, onlyBlockEffects: true);
		}
		done = true;
		return HasToReturn.hasToReturn;
	}
}
