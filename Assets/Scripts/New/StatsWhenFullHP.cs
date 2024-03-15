//using Sonigon;
using UnityEngine;

public class StatsWhenFullHP : MonoBehaviour
{
	public bool playSound;

	//public SoundEvent soundPristineGrow;

	//public SoundEvent soundPristineShrink;

	//public float healthMultiplier = 1f;

	//public float sizeMultiplier = 1f;

	public float healthThreshold = 0.9f;

	private BaseCharacter data;

	private bool isOn, active, flag;

	private void Start()
	{
		data = GetComponentInParent<BaseCharacter>();
		data.stats.OnDamageMultiplierAction += Active;
	}

	private void Update()
	{
		flag = data.health / data.maxHealth >= healthThreshold;
		if (active)
		{
			active = false;
			isOn = !this.flag;
		}
		if (flag == isOn)
		{
			return;
		}
		isOn = flag;
		if (isOn)
		{
			if (playSound)
			{
				//SoundManager.Instance.PlayAtPosition(soundPristineGrow, SoundManager.Instance.GetTransform(), base.transform);
			}
			// data.health *= healthMultiplier;
			// data.maxHealth *= healthMultiplier;
			// data.stats.sizeMultiplier *= sizeMultiplier;
			// data.stats.ConfigureMassAndSize();
			data.Gun.damage *= data.stats.damageMultiplier;
		}
		else
		{
			if (playSound)
			{
				//SoundManager.Instance.PlayAtPosition(soundPristineShrink, SoundManager.Instance.GetTransform(), base.transform);
			}
			// data.health /= healthMultiplier;
			// data.maxHealth /= healthMultiplier;
			// data.stats.sizeMultiplier /= sizeMultiplier;
			// data.stats.ConfigureMassAndSize();
			data.Gun.damage /= data.stats.damageMultiplier;
		}
	}

	void Active()
	{
		active = true;
	}
}
