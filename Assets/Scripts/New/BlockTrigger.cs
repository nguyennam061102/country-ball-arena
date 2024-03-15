using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BlockTrigger : MonoBehaviour
{
    public enum BlockTriggerType
    {
        Default,
        None,
        ShieldCharge,
        Echo,
        Empower
    }

    public UnityEvent triggerEvent;

    public UnityEvent triggerEventEarly;

    public bool delayOtherActions;

    public UnityEvent triggerFirstBlockThatDelaysOthers;

    public UnityEvent triggerSuperFirstBlock;

    public UnityEvent successfulBlockEvent;

    public UnityEvent blockRechargeEvent;

    private BlockEffect[] effects;

    public float cooldown;

    private float lastTriggerTime = -5f;

    public BlockTriggerType blackListedType = BlockTriggerType.None;

    public float cooldownSuccess;

    private float lastTriggerTimeSuccessful = -5f;

    private Block componentInParent;

    public BaseCharacter data;

    [Header("OFF_HAND")]
    public GameObject block;
    public GameObject boomBard;
    public GameObject ring;
    public GameObject iceStorm;
    public GameObject medic;

    [Space(20)] public Gun blockGun;

    private void OnEnable()
    {
        effects = GetComponents<BlockEffect>();
        componentInParent = GetComponentInParent<Block>();
        componentInParent.SuperFirstBlockAction =
            (Action<BlockTriggerType>)Delegate.Combine(componentInParent.SuperFirstBlockAction,
                new Action<BlockTriggerType>(DoSuperFirstBlock));
        componentInParent.FirstBlockActionThatDelaysOthers = (Action<BlockTriggerType>)Delegate.Combine(
            componentInParent.FirstBlockActionThatDelaysOthers,
            new Action<BlockTriggerType>(DoFirstBlockThatDelaysOthers));
        componentInParent.BlockAction = (Action<BlockTriggerType>)Delegate.Combine(componentInParent.BlockAction,
            new Action<BlockTriggerType>(DoBlock));
        componentInParent.BlockActionEarly =
            (Action<BlockTriggerType>)Delegate.Combine(componentInParent.BlockActionEarly,
                new Action<BlockTriggerType>(DoBlockEarly));
        componentInParent.BlockProjectileAction = (Action<GameObject, Vector3, Vector3>)Delegate.Combine(
            componentInParent.BlockProjectileAction, new Action<GameObject, Vector3, Vector3>(DoBlockedProjectile));
        componentInParent.BlockRechargeAction =
            (Action)Delegate.Combine(componentInParent.BlockRechargeAction, new Action(DoBlockRecharge));
        if (delayOtherActions)
        {
            GetComponentInParent<Block>().delayOtherActions = true;
        }

        //CheckBlockCooldown();
    }

    public void SetBlock()
    {
        if (data == null) return;
        if (data.player.playerID == 0)
        {
            ActiveBlock(GameData.CurrentOffHandId);
        }
        else if (data.player.playerID == 1)
        {
            ActiveBlock(GameData.CurrentOffHandAi1Id);
        }
        else if (data.player.playerID == 2)
        {
            ActiveBlock(GameData.CurrentOffHandAi2Id);
        }
    }

    void ActiveBlock(int id)
    {
        block.SetActive(false);
        boomBard.SetActive(false);
        iceStorm.SetActive(false);
        ring.SetActive(false);
        medic.SetActive(false);
        switch (id)
        {
            case 0:
                componentInParent.cooldown = 12 / (1 + data.Stats.TalentCooldown);
                block.SetActive(true);
                break;
            case 1:
                componentInParent.cooldown = 16 / (1 + data.Stats.TalentCooldown);
                boomBard.SetActive(true);
                blockGun = boomBard.GetComponentInChildren<Gun>();
                break;
            case 2:
                componentInParent.cooldown = 20 / (1 + data.Stats.TalentCooldown);
                iceStorm.SetActive(true);
                break;
            case 3:
                componentInParent.cooldown = 25 / (1 + data.Stats.TalentCooldown);
                ring.SetActive(true);
                blockGun = ring.GetComponentInChildren<Gun>();
                break;
            case 4:
                componentInParent.cooldown = 14 / (1 + data.Stats.TalentCooldown);
                medic.SetActive(true);
                break;
        }

        if (blockGun != null) blockGun.SetGun(data);
    }

    private void OnDestroy()
    {
        Block componentInParent = GetComponentInParent<Block>();
        if (componentInParent == null) return;
        componentInParent.SuperFirstBlockAction =
            (Action<BlockTriggerType>)Delegate.Remove(componentInParent.SuperFirstBlockAction,
                new Action<BlockTriggerType>(DoSuperFirstBlock));
        componentInParent.FirstBlockActionThatDelaysOthers = (Action<BlockTriggerType>)Delegate.Remove(
            componentInParent.FirstBlockActionThatDelaysOthers,
            new Action<BlockTriggerType>(DoFirstBlockThatDelaysOthers));
        componentInParent.BlockAction = (Action<BlockTriggerType>)Delegate.Remove(componentInParent.BlockAction,
            new Action<BlockTriggerType>(DoBlock));
        componentInParent.BlockActionEarly =
            (Action<BlockTriggerType>)Delegate.Remove(componentInParent.BlockActionEarly,
                new Action<BlockTriggerType>(DoBlockEarly));
        componentInParent.BlockProjectileAction = (Action<GameObject, Vector3, Vector3>)Delegate.Remove(
            componentInParent.BlockProjectileAction, new Action<GameObject, Vector3, Vector3>(DoBlockedProjectile));
        componentInParent.BlockRechargeAction =
            (Action)Delegate.Remove(componentInParent.BlockRechargeAction, new Action(DoBlockRecharge));
    }

    public void DoSuperFirstBlock(BlockTriggerType triggerType)
    {
        if (triggerType != blackListedType && !(lastTriggerTime + cooldown > Time.time))
        {
            lastTriggerTime = Time.time;
            triggerSuperFirstBlock.Invoke();
        }
    }

    public void DoFirstBlockThatDelaysOthers(BlockTriggerType triggerType)
    {
        if (triggerType != blackListedType && !(lastTriggerTime + cooldown > Time.time))
        {
            lastTriggerTime = Time.time;
            triggerFirstBlockThatDelaysOthers.Invoke();
        }
    }

    public void DoBlockEarly(BlockTriggerType triggerType)
    {
        if (triggerType != blackListedType && !(lastTriggerTime + cooldown > Time.time))
        {
            lastTriggerTime = Time.time;
            triggerEventEarly.Invoke();
        }
    }

    public void DoBlock(BlockTriggerType triggerType)
    {
        if (triggerType != blackListedType && !(lastTriggerTime + cooldown > Time.time))
        {
            lastTriggerTime = Time.time;
            triggerEvent.Invoke();
        }
    }

    public void DoBlock()
    {
        if (!componentInParent.active) return;
        triggerEvent.Invoke();
        componentInParent.LHD_DoBlock();
        switch (GameData.CurrentOffHandId)
        {
            case 0:
                Sound.Play(Sound.SoundData.OffHandBlock);
                break;
            case 1:
                Sound.Play(Sound.SoundData.OffHandBombard);
                break;
            case 2:
                Sound.Play(Sound.SoundData.OffHandIceStorm);
                break;
            case 3:
                Sound.Play(Sound.SoundData.OffHandRing);
                break;
            case 4:
                Sound.Play(Sound.SoundData.OffHandMedic);
                break;
        }
    }

    public void DoBlockedProjectile(GameObject projectile, Vector3 forward, Vector3 hitPos)
    {
        if (!(lastTriggerTimeSuccessful + cooldownSuccess > Time.time))
        {
            lastTriggerTimeSuccessful = Time.time;
            successfulBlockEvent.Invoke();
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].DoBlockedProjectile(projectile, forward, hitPos);
            }
        }
    }

    public void DoBlockRecharge()
    {
        blockRechargeEvent.Invoke();
    }
}