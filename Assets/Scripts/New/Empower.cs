﻿//using Sonigon;
using System;
using UnityEngine;

public class Empower : MonoBehaviour
{
    //public SoundEvent soundEmpowerSpawn;

    public GameObject addObjectToBullet;

    public float dmgMultiplier = 2f;

    public float speedMultiplier = 2f;

    public Color empowerColor;

    private BaseCharacter data;

    private ParticleSystem[] parts;

    private Transform particleTransform;

    private bool empowered;

    private bool isOn;

    HealthHandler healthHandler;
    Gun gun;
    Block componentInParent;

    private void Start()
    {
        particleTransform = base.transform.GetChild(0);
        data = GetComponentInParent<BaseCharacter>();
        parts = GetComponentsInChildren<ParticleSystem>();
        healthHandler = GetComponentInParent<Player>().healthHandler;
        healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(ResetEmpower));
        gun = data.holding.holdable.GetComponent<Gun>();
        gun.ShootPojectileAction = (Action<GameObject>)Delegate.Combine(gun.ShootPojectileAction, new Action<GameObject>(Attack));
        componentInParent = GetComponentInParent<Block>();
        componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(Block));
    }

    private void OnDestroy()
    {
        if (healthHandler != null) healthHandler.reviveAction = (Action)Delegate.Remove(healthHandler.reviveAction, new Action(ResetEmpower));
        if (gun != null) gun.ShootPojectileAction = (Action<GameObject>)Delegate.Remove(gun.ShootPojectileAction, new Action<GameObject>(Attack));
        if (componentInParent != null) componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(Block));
    }

    private void ResetEmpower()
    {
        empowered = false;
    }

    public void Block(BlockTrigger.BlockTriggerType trigger)
    {
        if (trigger != BlockTrigger.BlockTriggerType.Echo && trigger != BlockTrigger.BlockTriggerType.Empower && trigger != BlockTrigger.BlockTriggerType.ShieldCharge)
        {
            empowered = true;
        }
    }

    public void Attack(GameObject projectile)
    {
        SpawnedAttack component = projectile.GetComponent<SpawnedAttack>();
        if (!component)
        {
            return;
        }
        if (empowered)
        {
            ProjectileHit component2 = projectile.GetComponent<ProjectileHit>();
            MoveTransform component3 = projectile.GetComponent<MoveTransform>();
            component.SetColor(empowerColor);
            component2.damage *= dmgMultiplier;
            component3.localForce *= speedMultiplier;
            if ((bool)addObjectToBullet)
            {
                UnityEngine.Object.Instantiate(addObjectToBullet, projectile.transform.position, projectile.transform.rotation, projectile.transform);
            }
        }
        empowered = false;
    }

    private void Update()
    {
        if (empowered)
        {
            particleTransform.transform.position = data.holding.holdable.GetComponent<Gun>().shootPosition.position;
            particleTransform.transform.rotation = data.holding.holdable.GetComponent<Gun>().shootPosition.localRotation;
            if (!isOn)
            {
                //SoundManager.Instance.PlayAtPosition(soundEmpowerSpawn, SoundManager.Instance.GetTransform(), base.transform);
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i].Play();
                }
                isOn = true;
            }
        }
        else if (isOn)
        {
            for (int j = 0; j < parts.Length; j++)
            {
                parts[j].Stop();
            }
            isOn = false;
        }
    }
}
