using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	//[Header("Sounds")]
	//public SoundEvent soundDamage;

	public AudioSource audioSource;

	[Header("Settings")]
	public float slow;

	public float silence;

	public bool fastSlow;

	public float stun;

	public float force = 2000f;

	public float objectForceMultiplier = 1f;

	public bool forceIgnoreMass;

	public float damage = 25f;

	//public Color dmgColor = Color.black;

	public float range = 2f;

	public float radius;

	public float flyingFor;

	public bool auto = true;

	public bool ignoreTeam;

	public bool ignoreWalls;

	public bool staticRangeMultiplier;

	public bool scaleSlow = true;

	public bool scaleSilence = true;

	public bool scaleDmg = true;

	public bool scaleRadius = true;

	public bool scaleStun = true;

	public bool scaleForce = true;

	public float immunity;

	private SpawnedAttack spawned;

	public bool locallySimulated;

	public Action<Damagable> DealDamageAction;

	public Action<Damagable> DealHealAction;

	public Action<Damagable, float> HitTargetAction;

	public Action<BaseCharacter, float> hitPlayerAction;

	public bool gunBlock;

	private void Start()
	{
		spawned = GetComponent<SpawnedAttack>();
		if (!gunBlock) damage = spawned.spawner.Gun.damage / 2;
		else damage = spawned.spawner.Stats.GetPlayerDamageBasedOnManyThings(damage, GameFollowData.Instance.offHandList[GameData.CurrentOffHandId].ItemLevel);		
		if (auto) Explode();
	}

	private void DoExplosionEffects(Collider2D hitCol, float rangeMultiplier, float distance)
	{
		float num = scaleDmg ? base.transform.localScale.x : 1f;
		float d = scaleForce ? base.transform.localScale.x : 1f;
		float num2 = scaleSlow ? base.transform.localScale.x : 1f;
		float num3 = scaleSilence ? base.transform.localScale.x : 1f;
		float num4 = scaleStun ? ((1f + base.transform.localScale.x) * 0.5f) : 1f;
		Damagable componentInParent = hitCol.gameObject.GetComponentInParent<Damagable>();

		BaseCharacter characterData = null;
		if ((bool)componentInParent)
		{
			characterData = hitCol.gameObject.GetComponentInParent<BaseCharacter>();
			if (characterData.player.playerID == spawned.spawner.player.playerID) return;
			if ((bool)spawned)
			{
				BaseCharacter spawner = spawned.spawner;
			}
			hitPlayerAction?.Invoke(characterData, rangeMultiplier);
			if (damage < 0f)
			{
				if ((bool)characterData)
				{
					characterData.healthHandler.Heal(0f - damage);
				}
				if (DealHealAction != null)
				{
					DealHealAction(componentInParent);
				}
			}
			else if (damage > 0f)
			{
				// if (soundDamage != null && characterData != null)
				// {
				// 	SoundManager.Instance.Play(soundDamage, characterData.transform);
				// }
				audioSource.Play();
				Vector2 vector = ((Vector2)hitCol.bounds.ClosestPoint(base.transform.position) - (Vector2)base.transform.position).normalized;
				if (vector == Vector2.zero)
				{
					vector = Vector2.up;
				}
				//if (spawned.IsMine())
				if (componentInParent != null)
				{
					//Debug.Log(componentInParent.name, componentInParent);
					componentInParent.CallTakeDamage(num * damage * rangeMultiplier * vector, damage, base.transform.position, null, spawned.spawner);
				}
				if (DealDamageAction != null)
				{
					DealDamageAction(componentInParent);
				}
			}
		}
		if ((bool)characterData)
		{
			if (HitTargetAction != null)
			{
				HitTargetAction(componentInParent, distance);
			}
			if (force != 0f)
			{
				if (locallySimulated)
				{
					characterData.healthHandler.TakeForce(((Vector2)hitCol.bounds.ClosestPoint(base.transform.position) - (Vector2)base.transform.position).normalized * rangeMultiplier * force * d, ForceMode2D.Impulse, forceIgnoreMass);
				}
				else if (spawned.IsMine())
				{
					characterData.healthHandler.CallTakeForce(((Vector2)hitCol.bounds.ClosestPoint(base.transform.position) - (Vector2)base.transform.position).normalized * rangeMultiplier * force * d, ForceMode2D.Impulse, forceIgnoreMass, ignoreBlock: false, flyingFor * rangeMultiplier);
				}
			}
			if (stun > 0f)
			{
				//characterData.stunHandler.AddStun(stun * num4);
			}
		}
		else if ((bool)hitCol.attachedRigidbody)
		{
			hitCol.attachedRigidbody.AddForce(((Vector2)hitCol.bounds.ClosestPoint(base.transform.position) - (Vector2)base.transform.position).normalized * rangeMultiplier * force * d, ForceMode2D.Impulse);
		}
	}

	public void Explode()
	{
		float num = scaleRadius ? base.transform.localScale.x : 1f;
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, range * num);
		radius = range * num;
		//Debug.Log($"num: {num} + radius: {radius}");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.layer != 19)
			{
				Damagable componentInParent = array[i].gameObject.GetComponentInParent<Damagable>();
				float num2 = Vector2.Distance(base.transform.position, array[i].bounds.ClosestPoint(base.transform.position));
				float value = 1f - num2 / (range * num);
				if (staticRangeMultiplier)
				{
					value = 1f;
				}
				value = Mathf.Clamp(value, 0f, 1f);
				//NetworkPhysicsObject component = array[i].GetComponent<NetworkPhysicsObject>();
				// if ((bool)component && component.photonView.IsMine)
				// {
				// 	float d = scaleForce ? base.transform.localScale.x : 1f;
				// 	component.BulletPush((Vector2)(component.transform.position - base.transform.position).normalized * objectForceMultiplier * 1f * value * force * d, Vector2.zero, null);
				// }
				if (((bool)componentInParent || (bool)array[i].attachedRigidbody) && (!ignoreTeam || !spawned || !(spawned.spawner.gameObject == array[i].transform.gameObject)))
				{
					DoExplosionEffects(array[i], value, num2);
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
