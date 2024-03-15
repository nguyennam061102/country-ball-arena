using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour
{
	public Holdable holdable;

	[HideInInspector]
	public float sinceAttack = 10f;

	private void Start()
	{
		holdable = GetComponent<Holdable>();
	}

	private void Update()
	{
	}

	public abstract bool Attack(float charge, bool forceAttack = false, float damageM = 1f, float recoilMultiplier = 1f, bool useAmmo = true);
}

[Serializable]
public class ProjectilesToSpawn
{
	public GameObject objectToSpawn;

	public int numberOfSpawns = 1;
}