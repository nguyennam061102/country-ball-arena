using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthHandler : Damagable
{
    private CharacterStats Stats => CharacterStats.Instance;

    private GameController gameController => GameController.Instance;

    //public float currentHp, maxHealth;
    public bool canTakeDamage = true;

    [Header("Settings")] public SpriteRenderer hpSprite;

    public DeathEffect deathEffect;

    public float regeneration;

    private BaseCharacter data;

    //private CodeAnimation anim;

    //private Player player;

    private CharacterStatModifiers stats;

    public ParticleSystem healPart, statusBlockPart, iceStormPart;

    private DamageOverTime dot;

    private Vector3 startHealthSpriteScale;

    public float flyingFor;

    private float lastDamaged;

    public Action delayedReviveAction;

    public Action reviveAction;

    public Action onHealthChange;

    [HideInInspector] public bool DestroyOnDeath;

    public bool isRespawning;

    public DeathEffect deathEffectPhoenix;

    public DeathEffect reviveEffect;

    public DeathEffect impactEffect;

    public float vampireMultiplier;

    private float respawnTimeCurrent;

    private float respawnTime = 2.53f;

    private void Awake()
    {
        dot = GetComponent<DamageOverTime>();
        data = GetComponent<BaseCharacter>();
        //anim = GetComponentInChildren<CodeAnimation>();
        //player = GetComponent<Player>();
        stats = GetComponent<CharacterStatModifiers>();
        iceStormPart.transform.parent = null;
        iceStormPart.transform.position = Vector3.zero;
    }

    // private void Start()
    // {
    //     //if (Stats != null) data.maxHealth = Stats.Health(skinHandler.characterSkin, 0);
    //     DestroyOnDeath = true;
    // }

    private void Start()
    {
        statusBlockPart.startColor = data.skinHandler.ColorForEFX;
    }

    private void Update()
    {
        flyingFor -= TimeHandler.deltaTime;
        if (regeneration > 0f)
        {
            Heal(regeneration * TimeHandler.deltaTime);
        }
    }

    public void FullyRestore()
    {
        Heal(data.maxHealth);
    }

    public void Heal(float healAmount)
    {
        //Debug.Log("Call Heal");
        if (healAmount != 0f && data.health != data.maxHealth && !IsCharacterDead())
        {
            //Debug.Log($"Character Heal: {healAmount}");
            //SoundManager.Instance.Play(soundHeal, base.transform);
            data.health += healAmount;
            data.health = Mathf.Clamp(data.health, float.NegativeInfinity, data.maxHealth);
            healPart.Emit((int)Mathf.Clamp(healAmount * 0.2f, 1f, 10f));
            onHealthChange?.Invoke();
        }
    }

    public void CallTakeForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse, bool forceIgnoreMass = false,
        bool ignoreBlock = false, float setFlying = 0f)
    {
        if (data.isPlaying && (!data.block.IsBlocking() || ignoreBlock))
        {
            TakeForce(force, (ForceMode2D)forceMode, forceIgnoreMass, ignoreBlock, setFlying);
        }
    }

    public void TakeForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Impulse, bool forceIgnoreMass = false,
        bool ignoreBlock = false, float setFlying = 0f)
    {
        if (!data.isPlaying || !data.playerVel.simulated)
        {
            return;
        }

        bool flag = data.block.IsBlocking();
        if (flag && !ignoreBlock)
        {
            return;
        }

        if (!flag && setFlying > flyingFor && setFlying > 0.25f)
        {
            flyingFor = setFlying;
            //SoundManager.Instance.Play(soundBounce, base.transform);
        }
        //Debug.Log("day ne?");
        data.playerVel.AddForce(force, forceMode);
        if (force.y > 500f)
        {
            force.y = 500f;
        }

        if (forceIgnoreMass)
        {
            force *= data.playerVel.mass / 100f;
        }

        if (force.y > 0f)
        {
            if (!forceIgnoreMass)
            {
                force.y /= data.playerVel.mass / 100f;
            }

            if (forceMode == ForceMode2D.Force)
            {
                force *= 0.003f;
            }

            data.sinceGrounded -= force.y * 0.005f;
        }

        data.sinceGrounded = Mathf.Clamp(data.sinceGrounded, -0.5f, 100f);
    }

    public virtual void TakeDamage(Vector2 damage, float dmg, BaseCharacter damagingPlayer, bool showEFX = true)
    {
        if (!canTakeDamage) return;
        if (Avoid()) return;

        if (showEFX) Instantiate(impactEffect, base.transform.position, base.transform.rotation).PlayDeath(damage, data.skinHandler.ColorForEFX);

        float totalDamage = GetRealDamageTaken(dmg, damagingPlayer);

        if (!data.AI) totalDamage *= GameEventTrackerProVCL.Instance.DamageMultipleAmount;

        if (Stats.TalentCritical > 0)
        {
            var rnd = Random.Range(0f, 1f);
            if (rnd <= Stats.TalentCritical)
            {
                totalDamage *= 2;
                gameController.showDamage.OnTakeDamage(totalDamage, transform.position, true);
            }
            else
            {
                gameController.showDamage.OnTakeDamage(totalDamage, transform.position, false);
            }
        }
        else
        {
            gameController.showDamage.OnTakeDamage(totalDamage, transform.position, false);
        }

        data.health -= totalDamage;
        onHealthChange?.Invoke();

        if (damagingPlayer != null && damagingPlayer != data)
        {
            if (!damagingPlayer.AI)
            {
                DailyMission.Instance.dailyMissionList[6].MissionProgress += (int)totalDamage;
            }
            else
            {
                DailyMission.Instance.dailyMissionList[9].MissionProgress += (int)totalDamage;
            }
            if (damagingPlayer)
            {
                damagingPlayer.stats.DealtDamage(damage, damagingPlayer != null && damagingPlayer == data, data);
            }
            if (damagingPlayer.healthHandler.vampireMultiplier > 0)
            {
                damagingPlayer.healthHandler.Heal(totalDamage * damagingPlayer.healthHandler.vampireMultiplier);
            }
        }

        if (IsCharacterDead())
        {
            if (data.stats.remainingRespawns > 0 && data.player.playerID == 0)
            {
                DiePhoenix(damage);
            }
            else
            {
                data.health = 0;
                Die(damage, damagingPlayer);
            }
        }
    }

    private bool Avoid()
    {
        if (Stats == null || data.AI) return false;
        var tmp = Random.Range(0f, 1f);
        return tmp <= Stats.TalentAvoid;
    }

    private float GetRealDamageTaken(float baseDamage, BaseCharacter damagingPlayer)
    {
        if (Stats == null || damagingPlayer == null) return baseDamage;
        return Stats.GetDamageTaken(baseDamage, data.AI ? 0 : Stats.TalentDefense,
            0 /*todo: Replace Value When You Have Card Defense*/, damagingPlayer.level, data.level);
    }

    public bool IsCharacterDead()
    {
        return data.health <= 0;
    }

    public void SimpleDieToRespawnAsBoss()
    {
        data.health = 0;
        data.dead = true;
        gameObject.SetActive(false);
        data.holding.SetActive(false);
    }

    private void Die(Vector2 deathDirection, BaseCharacter damagingPlayer)
    {
        if (data.isPlaying && !data.dead)
        {
            //TimeHandler.instance.DoSlowDown();
            //SoundManager.Instance.Play(soundDie, base.transform);
            data.dead = true;
            if (!DestroyOnDeath)
            {
                Instantiate(deathEffect, base.transform.position, base.transform.rotation).PlayDeath(deathDirection, data.skinHandler.ColorForEFX);
                gameObject.SetActive(false);
                data.holding.SetActive(false);

                CameraShake.Instance.ImShakingBroooooo(deathDirection.normalized * 3f);

                //survival mode
                if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival))
                {
                    if (data.AI)
                    {
                        if (damagingPlayer != null && !damagingPlayer.AI)
                        {
                            gameController.PlayerKillCount++;
                            gameController.ThisMatchKillCount++;
                            DailyMission.Instance.dailyMissionList[0].MissionProgress++;                          
                        }
                        if(damagingPlayer != null) gameController.survivalMode.EnemyDie(!damagingPlayer.AI);
                        else gameController.survivalMode.EnemyDie(false);

                        if (PlayerManager.Instance.IsAllAIDie())
                        {
                            gameController.NextRound();
                        }
                    }
                    else
                    {
                        gameController.EndGame(false);
                        GameEventTrackerProVCL.Instance.OnCharacterDie(data.player.playerID);
                    }
                }
                else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch) || GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox)) //Death Match & Sandbox
                {
                    if (damagingPlayer != null) gameController.PlayerDies(data.player, damagingPlayer.player);
                    else
                    {
                        gameController.PlayerDies(data.player, null);
                    }
                }
            }
            else
            {
                Destroy(base.transform.root.gameObject);
            }
        }
    }

    public void DiePhoenix(Vector2 deathDirection)
    {
        if (data.isPlaying && !data.dead)
        {
            TimeHandler.instance.DoSlowDown();
            data.stats.remainingRespawns--;
            isRespawning = true;
            if (!DestroyOnDeath)
            {
                Instantiate(deathEffectPhoenix, base.transform.position, base.transform.rotation).PlayDeath(deathDirection, data.skinHandler.ColorForEFX, data.player.playerID);
                base.gameObject.SetActive(value: false);
                data.holding.SetActive(false);

                CameraShake.Instance.ImShakingBroooooo(deathDirection.normalized * 3f);
            }
        }
    }

    private IEnumerator DelayReviveAction()
    {
        yield return new WaitForSecondsRealtime(1f);
        delayedReviveAction?.Invoke();
        canTakeDamage = true;
    }

    public void CallRevive()
    {
        Instantiate(reviveEffect, transform.position, transform.rotation).Revive(data.player.playerID, data.skinHandler.ColorForEFX);
    }

    public void Revive(bool isFullRevive = true)
    {
        canTakeDamage = false;
        reviveAction?.Invoke();
        flyingFor = 0f;
        if (isFullRevive)
        {
            data.stats.remainingRespawns = data.stats.respawns;
        }
        data.healthHandler.isRespawning = false;
        data.health = data.maxHealth;
        onHealthChange?.Invoke();
        data.playerVel.velocity = Vector2.zero;
        data.playerVel.angularVelocity = 0f;
        data.stunTime = 0f;
        data.block.ResetCD(false);
        data.GetComponent<PlayerCollision>().IgnoreWallForFrames(5);
        gameObject.SetActive(value: true);
        data.holding.SetActive(true);
        StartCoroutine(DelayReviveAction());
        data.dead = false;
        data.stunHandler.StopStun();
        GetComponent<CharacterStatModifiers>().slow = 0f;
        GetComponent<CharacterStatModifiers>().slowSlow = 0f;
        GetComponent<CharacterStatModifiers>().fastSlow = 0f;
        GetComponent<Block>().sinceBlock = float.PositiveInfinity;
        //GetComponent<Gravity>().OutForceField();
    }
    #region OffHand

    public void Block()
    {
        statusBlockPart.Play();
        StartCoroutine(BlockCoro());
    }

    private IEnumerator BlockCoro()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.5f);
        canTakeDamage = true;
    }

    public void IceStorm()
    {
        iceStormPart.Play();
        foreach (var player in PlayerManager.Instance.GetPlayersNotInID(data.player.playerID))
        {
            player.IceStorm();
        }
    }

    public void Medic()
    {
        healPart.Play();
        data.health += data.maxHealth * (0.1f + ((GameFollowData.Instance.offHandList[4].ItemLevel - 1) * 0.01f));
        if (data.health >= data.maxHealth) data.health = data.maxHealth;
    }

    #endregion

    public override void CallTakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition,
        GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true)
    {
        TakeDamage(damage, damageAmount, damagingPlayer);
    }

    public override void TakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition,
        GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true,
        bool ignoreBlock = false)
    {
    }

    public override void TakeDamage(Vector2 damage, float damageAmount, Vector2 damagePosition, Color dmgColor,
        GameObject damagingWeapon = null, BaseCharacter damagingPlayer = null, bool lethal = true,
        bool ignoreBlock = false)
    {
    }

    private void OnDestroy()
    {
        Destroy(this.iceStormPart);
    }
}