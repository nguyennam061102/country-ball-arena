using System;
using UnityEngine;

public class RayHitReflect : RayHitEffect
{
	private MoveTransform move;

	private ProjectileHit projHit;

	public int reflects = 1;

	public float speedM = 1f;

	public float dmgM = 1f;

	[HideInInspector]
	public float timeOfBounce = -10000f;

	public Action<HitInfo> reflectAction;

	public float t = 0.1f;

	private void Start()
	{
		move = GetComponent<MoveTransform>();
		projHit = GetComponent<ProjectileHit>();
	}

	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		// if ((bool)hit.transform && (bool)hit.transform.GetComponent<Player>())
		// {
		// 	Debug.Log("1");
		// 	reflects -= 10;
		// }
		// if (reflects <= 0)
		// {
		// 	Debug.Log("2");
		// 	return HasToReturn.canContinue;
		// }
		if (reflects > 0)
		{
			if (reflectAction != null)
			{
				reflectAction(hit);
			}
			
			move.velocity = Vector2.Reflect(move.velocity, hit.normal);
			//Debug.LogError(move.velocity, this);
			move.velocity *= speedM;
			projHit.damage *= dmgM;
			projHit.shake *= dmgM;
			if (dmgM > 1f)
			{
				float num = dmgM - 1f;
				num *= 0.6f;
				dmgM = num + 1f;
				ScaleTrailFromDamage componentInChildren = GetComponentInChildren<ScaleTrailFromDamage>();
				if ((bool)componentInChildren)
				{
					componentInChildren.Rescale();
				}
			}
			timeOfBounce = Time.time;
			base.transform.position = (Vector3)hit.point + move.velocity.normalized * t;
			reflects--;
			return HasToReturn.canContinue;
		}
		return HasToReturn.hasToReturn;
	}
}
