using UnityEngine;
using UnityEngine.Events;

public class StatsAfterDealingDamage : MonoBehaviour
{
	public float duration = 3f;

	public float movementSpeedMultiplier = 1f;
	
	public float movementSpeedAddMultiplier = 0.15f;

	public float jumpMultiplier = 1f;

	public float hpMultiplier = 1f;
	
	public float hpAddMultiplier = 0;

	public UnityEvent startEvent;

	public UnityEvent endEvent;

	private bool isOn;

	private BaseCharacter data;

	private void Start()
	{
		data = GetComponentInParent<BaseCharacter>();

		if (!data.statsAfterDealingDamage)
		{
			data.statsAfterDealingDamage = this;
		}
		else
		{
			data.statsAfterDealingDamage.movementSpeedMultiplier += this.movementSpeedAddMultiplier;
			data.statsAfterDealingDamage.hpMultiplier += hpAddMultiplier;
			Destroy(this.gameObject);
		}
	}

	private void Update()
	{
		bool flag = data.stats.sinceDealtDamage < duration;
		if (isOn != flag)
		{
			isOn = flag;
			Vector3 localScale = base.transform.localScale;
			if (isOn)
			{
				data.health *= hpMultiplier;
				data.maxHealth *= hpMultiplier;
				data.stats.movementSpeed *= movementSpeedMultiplier;
				data.stats.jump *= jumpMultiplier;
				data.stats.ConfigureMassAndSize();
				startEvent.Invoke();
			}
			else
			{
				data.health /= hpMultiplier;
				data.maxHealth /= hpMultiplier;
				data.stats.movementSpeed /= movementSpeedMultiplier;
				data.stats.jump /= jumpMultiplier;
				data.stats.ConfigureMassAndSize();
				endEvent.Invoke();
			}
		}
	}

	public void Interupt()
	{
		// if (isOn)
		// {
		// 	data.health /= hpMultiplier;
		// 	data.maxHealth /= hpMultiplier;
		// 	data.stats.movementSpeed /= movementSpeedMultiplier;
		// 	data.stats.jump /= jumpMultiplier;
		// 	data.stats.ConfigureMassAndSize();
		// 	endEvent.Invoke();
		// 	isOn = false;
		// }
	}
}
