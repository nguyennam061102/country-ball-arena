using UnityEngine;

public class ToggleStats : MonoBehaviour
{
	public float movementSpeedMultiplier = 1f;

	public float hpMultiplier = 1f;

	private BaseCharacter data;

	private void Start()
	{
		data = GetComponentInParent<BaseCharacter>();
	}

	public void TurnOn()
	{
		data.health *= hpMultiplier;
		data.maxHealth *= hpMultiplier;
		data.stats.movementSpeed *= movementSpeedMultiplier;
		data.stats.ConfigureMassAndSize();
	}

	public void TurnOff()
	{
		data.health /= hpMultiplier;
		data.maxHealth /= hpMultiplier;
		data.stats.movementSpeed /= movementSpeedMultiplier;
		data.stats.ConfigureMassAndSize();
	}
}
