using System;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
	public bool triggerOnEveryShot = true;

	public UnityEvent triggerEvent;

	private BaseCharacter data;

	private void Start()
	{
		data = GetComponentInParent<BaseCharacter>();
		
		Gun gun = data.Gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Combine(gun.ShootPojectileAction, new Action<GameObject>(Shoot));
		data.Gun.AddAttackAction(Attack);
	}

	private void OnDestroy()
	{
		Gun gun = data.Gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Remove(gun.ShootPojectileAction, new Action<GameObject>(Shoot));
		data.Gun.RemoveAttackAction(Attack);
	}

	public void Attack()
	{
		if (!triggerOnEveryShot)
		{
			triggerEvent.Invoke();
		}
	}

	public void Shoot(GameObject projectile)
	{
		if (triggerOnEveryShot)
		{
			triggerEvent.Invoke();
		}
	}
}
