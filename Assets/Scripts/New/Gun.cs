using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : Weapon
{

    public ShootType shootType;
    private GameFollowData GfData => GameFollowData.Instance;
    private CharacterStats Stats => CharacterStats.Instance;

    [Header("Settings")] public float recoil;

    public float bodyRecoil;

    public float shake;

    public bool forceSpecificShake;

    public ProjectilesToSpawn[] projectiles;

    private Rigidbody2D rig;

    public Transform shootPosition;

    //public Player player;

    public BaseCharacter data;

    [HideInInspector] public bool isReloading;

    [Header("Multiply")]

    public float damage = 1f;

    //public float reloadTime = 1f;

    public float reloadTimeAdd;

    public float reloadTimeMultiplier = 1f;

    public float recoilMuiltiplier = 1f;

    public float knockback = 1f;

    public float attackSpeed = 1f;

    public float projectileSpeed = 1f;

    public float projectielSimulatonSpeed = 1f;

    public float gravity = 1f;

    public float damageAfterDistanceMultiplier = 1f;

    public float bulletDamageMultiplier = 1f;

    public float multiplySpread = 1f;

    public float shakeM = 1f;

    [Header("Add")] public int ammo;

    public float ammoReg;

    public float size;

    public float overheatMultiplier;

    public float timeToReachFullMovementMultiplier;

    public int numberOfProjectiles;

    public int bursts;

    public int reflects;

    public int smartBounce;

    public int bulletPortal;

    public int randomBounces;

    public float timeBetweenBullets;

    public float projectileSize;

    public float speedMOnBounce = 1f;

    public float dmgMOnBounce = 1f;

    public float drag;

    public float dragMinSpeed = 1f;

    public float spread;

    public float evenSpread;

    public float percentageDamage;

    public float cos;

    public float slow;

    [Header("Charge Multiply")] public float chargeDamageMultiplier = 1f;

    [Header("(1 + Charge * x) Multiply")] public float chargeSpreadTo;

    public float chargeEvenSpreadTo;

    public float chargeSpeedTo;

    public float chargeRecoilTo;

    [Header("(1 + Charge * x) Add")] public float chargeNumberOfProjectilesTo;

    [Header("Special")]

    public float destroyBulletAfter;

    public float destroyBulletAfterMultiplier = 1f;

    public float forceSpecificAttackSpeed;

    public bool lockGunToDefault;

    public bool unblockable;

    public bool ignoreWalls;

    public bool degreaseSizeByTime;

    //[HideInInspector]
    public float currentCharge;

    public bool useCharge;

    public bool dontAllowAutoFire;

    public ObjectsToSpawn[] objectsToSpawn;

    public Color projectileColor = Color.black;

    public bool waveMovement;

    public bool teleport;

    public bool spawnSkelletonSquare;

    public float explodeNearEnemyRange;

    public float explodeNearEnemyDamage;

    public float hitMovementMultiplier = 1f;

    private Action attackAction;

    [HideInInspector] public bool isProjectileGun;

    [HideInInspector] public float defaultCooldown = 1f;

    [HideInInspector] public int attackID = -1;

    public float attackSpeedMultiplier = 1f;

    private int gunID = -1;

    public GunAmmo gunAmmo;

    private Vector3 spawnPos;

    public Action<GameObject> ShootPojectileAction;

    private float spreadOfLastBullet;

    private SpawnedAttack spawnedAttack;

    [HideInInspector] internal Vector3 forceShootDir;

    public ParticleSystem muzzleFx;

    public bool card;

    private bool AI
    {
        get
        {
            if (holdable != null && holdable.holder != null) return holdable.holder.AI;
            return false;
        }
    }

    private float usedCooldown
    {
        get
        {
            if (!lockGunToDefault)
            {
                return attackSpeed;
            }

            return defaultCooldown;
        }
    }

    internal float GetRangeCompensation(float distance)
    {
        return Mathf.Pow(distance, 2f) * 0.015f / projectileSpeed;
    }

    public AudioSource SoundGun => GetComponent<AudioSource>();
    public AudioClip shotSound, reloadSound;
    public float shotVolume = 1f;
    public float reloadVolume = 1f;

    internal void ResetStats()
    {
        isReloading = false;
        damage = 1f;
        //reloadTime = 1f;
        reloadTimeAdd = 0f;
        recoilMuiltiplier = 1f;
        gunAmmo.reloadTimeMultiplier = 1f;
        gunAmmo.reloadTimeAdd = 0f;
        knockback = 1f;
        attackSpeed = 0.3f;
        projectileSpeed = 1f;
        projectielSimulatonSpeed = 1f;
        gravity = 1f;
        damageAfterDistanceMultiplier = 1f;
        bulletDamageMultiplier = 1f;
        multiplySpread = 1f;
        shakeM = 1f;
        ammo = 0;
        ammoReg = 0f;
        size = 0f;
        overheatMultiplier = 0f;
        timeToReachFullMovementMultiplier = 0f;
        numberOfProjectiles = 1;
        bursts = 0;
        reflects = 0;
        smartBounce = 0;
        bulletPortal = 0;
        randomBounces = 0;
        timeBetweenBullets = 0f;
        projectileSize = 0f;
        speedMOnBounce = 1f;
        dmgMOnBounce = 1f;
        drag = 0f;
        dragMinSpeed = 1f;
        spread = 0f;
        evenSpread = 0f;
        percentageDamage = 0f;
        cos = 0f;
        slow = 0f;
        chargeNumberOfProjectilesTo = 0f;
        destroyBulletAfter = 0f;
        forceSpecificAttackSpeed = 0f;
        lockGunToDefault = false;
        unblockable = false;
        ignoreWalls = false;
        currentCharge = 0f;
        useCharge = false;
        waveMovement = false;
        teleport = false;
        spawnSkelletonSquare = false;
        explodeNearEnemyRange = 0f;
        explodeNearEnemyDamage = 0f;
        hitMovementMultiplier = 1f;
        isProjectileGun = false;
        defaultCooldown = 1f;
        attackSpeedMultiplier = 1f;
        objectsToSpawn = new ObjectsToSpawn[0];
        GetComponentInChildren<GunAmmo>().maxAmmo = 3;
        GetComponentInChildren<GunAmmo>().ReDrawTotalBullets();
        projectileColor = Color.black;
    }

    private void Awake()
    {
        ShootPos componentInChildren = GetComponentInChildren<ShootPos>();
        if ((bool)componentInChildren)
        {
            shootPosition = componentInChildren.transform;
        }
        else
        {
            shootPosition = base.transform;
        }
    }

    public void SetGun(BaseCharacter baseCharacter)
    {
        //hihi
        data = baseCharacter;
        //haha
        gunAmmo = GetComponentInChildren<GunAmmo>();
        Gun[] componentsInChildren = base.transform.root.GetComponentsInChildren<Gun>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (componentsInChildren[i] == this)
            {
                gunID = i;
            }
        }

        holdable = GetComponent<Holdable>();
        defaultCooldown = usedCooldown;
        rig = GetComponent<Rigidbody2D>();
        int ilevel = 0;
        if ((bool)data && Stats && !card && !gunBlock)
        {
            if (!data.AI)
            {
                ilevel = GfData.mainHandList[GameData.CurrentMainHandId].ItemLevel;
            }
            else
            {
                if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival) || GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
                {
                    if (GameData.MatchLevel < 3 && GameData.PlayerLevel == 1) ilevel = 1;
                    else
                    {
                        ilevel = (int)(Mathf.Pow(1.08f, GameData.MatchLevel));
                    }
                }
                else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
                {
                    ilevel = GfData.mainHandList[GameData.CurrentMainHandId].ItemLevel + (int)(GameData.PlayerDeathMatchWinCount / 3);
                }
            }
            if (ilevel <= 0) ilevel = 0;

            damage = Stats.GetPlayerDamageBasedOnManyThings(damage, ilevel);
            gunAmmo.maxAmmo += (int)(ilevel / 5);
        }
        if (gunBlock)
        {
            if (data.player.playerID == 0)
            {
                damage = Stats.GetPlayerDamageBasedOnManyThings(damage, GfData.offHandList[GameData.CurrentOffHandId].ItemLevel);
            }
            else
            {
                if (GameData.RoundLevel < 3 && GameData.PlayerLevel == 1) damage = Stats.GetPlayerDamageBasedOnManyThings(damage, 1) * GameController.Instance.AIDamageMultiplier;
                else damage = Stats.GetPlayerDamageBasedOnManyThings(damage, Random.Range(GameData.RoundLevel - 3, GameData.RoundLevel - 1)) * GameController.Instance.AIDamageMultiplier;
            }
        }
    }

    private void Start()
    {
        //gunAmmo = GetComponentInChildren<GunAmmo>();
        //Gun[] componentsInChildren = base.transform.root.GetComponentsInChildren<Gun>();
        //for (int i = 0; i < componentsInChildren.Length; i++)
        //{
        //    if (componentsInChildren[i] == this)
        //    {
        //        gunID = i;
        //    }
        //}
        //if (!data)
        //{
        //    data = GetComponentInParent<BaseCharacter>();
        //}

        //if (!data)
        //{
        //    ProjectileHit componentInParent = GetComponentInParent<ProjectileHit>();
        //    if ((bool) componentInParent)
        //    {
        //        data = componentInParent.ownPlayer;
        //    }
        //}

        //if (!data)
        //{
        //    SpawnedAttack component = base.transform.root.GetComponent<SpawnedAttack>();
        //    if ((bool) component)
        //    {
        //        data = component.spawner;
        //    }
        //}

        //holdable = GetComponent<Holdable>();
        //defaultCooldown = usedCooldown;
        //rig = GetComponent<Rigidbody2D>();
        //if ((bool) data && Stats && !card && !gunBlock)
        //{
        //    if (!data.AI)
        //    {
        //        damage = Stats.Damage(damage, GfData.mainHandList[GameData.CurrentMainHandId].ItemLevel);
        //    }
        //    else
        //    {
        //        if (GameData.RoundLevel < 3 && GameData.PlayerLevel == 1) damage = Stats.Damage(damage, 1);
        //        else damage = Stats.Damage(damage, Random.Range(GameData.RoundLevel - 3, GameData.RoundLevel - 1));
        //    }
        //}
        //if (gunBlock)
        //{
        //    if (data.player.playerID == 0)
        //    {
        //        damage = Stats.Damage(damage, GfData.offHandList[GameData.CurrentOffHandId].ItemLevel);
        //    }
        //    else
        //    {
        //        if (GameData.RoundLevel < 3 && GameData.PlayerLevel == 1) damage = Stats.Damage(damage, 1) * GameController.Instance.AIDamageMultiplier;
        //        else damage = Stats.Damage(damage, Random.Range(GameData.RoundLevel - 3, GameData.RoundLevel - 1)) * GameController.Instance.AIDamageMultiplier;
        //    }
        //}
    }

    //public bool isMine;
    public bool gunBlock;

    private void Update()
    {
        if ((bool)holdable && (bool)holdable.holder && (bool)holdable.holder)
        {
            data = holdable.holder;
        }
        sinceAttack += TimeHandler.deltaTime * attackSpeedMultiplier;
        // if (!GameManager.instance.battleOngoing || (player != null && (!player.data.isPlaying || player.data.dead)))
        // {
        // 	soundGun.StopAutoPlayTail();
        // }
        //isMine = CheckIsMine();
    }

    // private void OnDestroy()
    // {
    //     //soundGun.StopAutoPlayTail();
    // }

    public bool IsReady(float readuIn = 0f)
    {
        return sinceAttack + readuIn * attackSpeedMultiplier > usedCooldown;
    }

    public float ReadyAmount()
    {
        return sinceAttack / usedCooldown;
    }

    public override bool Attack(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f, bool useAmmo = true)
    {
        if (sinceAttack < usedCooldown && !forceAttack)
        {
            return false;
        }

        if (isReloading && !forceAttack)
        {
            return false;
        }

        sinceAttack = 0f;
        int attacks = Mathf.Clamp(Mathf.RoundToInt(0.5f * charge / attackSpeed), 1, 10);
        if (lockGunToDefault)
        {
            attacks = 1;
        }

        StartCoroutine(DoAttacks(charge, forceAttack, damageM, attacks, recoilM, useAmmo));
        return true;
    }

    private IEnumerator DoAttacks(float charge, bool forceAttack = false, float damageM = 1f, int attacks = 1,
        float recoilM = 1f, bool useAmmo = true)
    {
        for (int i = 0; i < attacks; i++)
        {
            DoAttack(charge, forceAttack, damageM, recoilM, useAmmo);
            yield return new WaitForSeconds(0.3f / (float)attacks);
        }
    }

    private void DoAttack(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f,
        bool useAmmo = true)
    {
        //float num = 1f * (1f + charge * chargeRecoilTo) * recoilM;
        //if ((bool)rig) rig.AddForce(rig.mass * recoil * Mathf.Clamp( /*usedCooldown*/ 0.3f, 0f, 1f) * -base.transform.up, ForceMode2D.Impulse);
        //data.playerVel.AddForce(-base.transform.up * bodyRecoil, ForceMode2D.Impulse);
        //bool flag = (bool) holdable;
        attackAction?.Invoke();
        StartCoroutine(FireBurst(charge, forceAttack, damageM, recoilM, useAmmo));
    }

    private bool CheckIsMine()
    {
        bool result = true;
        if ((bool)holdable && (bool)holdable.holder)
        {
            result = !holdable.holder.AI;
        }
        else
        {
            BaseCharacter componentInParent = GetComponentInParent<BaseCharacter>();
            if ((bool)componentInParent)
            {
                result = !componentInParent.AI;
            }
        }
        return result;
    }

    private IEnumerator FireBurst(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f,
        bool useAmmo = true)
    {
        //CameraShake.Instance.ImShakingBroooooo(transform.up * 1.25f);
        int currentNumberOfProjectiles = lockGunToDefault ? 1 : (numberOfProjectiles + Mathf.RoundToInt(chargeNumberOfProjectilesTo * charge));
        if (!lockGunToDefault)
        {
            //do nothing
        }

        if (timeBetweenBullets == 0f)
        {
            //CameraShake.Instance.ImShakingBroooooo(base.transform.up * shake);


            if ((bool)SoundGun) SoundGun.volume = shotVolume;
            if ((bool)SoundGun) SoundGun.PlayOneShot(shotSound);
        }

        for (int ii = 0; ii < Mathf.Clamp(bursts, 1, 100); ii++)
        {
            for (int i = 0; i < projectiles.Length; i++)
            {
                for (int j = 0; j < currentNumberOfProjectiles; j++)
                {
                    spawnPos = base.transform.position;
                    if ((bool)data)
                    {
                        if ((bool)holdable)
                        {
                            spawnPos = shootPosition.position;
                        }
                    }
                    GameObject gameObject = Instantiate(projectiles[i].objectToSpawn.gameObject, spawnPos, getShootRotation(j, currentNumberOfProjectiles, charge));
                    if ((bool)holdable)
                    {
                        if (useAmmo)
                        {
                            muzzleFx.Play();
                            gameObject.GetComponent<ProjectileInit>().OFFLINE_Init(holdable.holder.player.playerID, currentNumberOfProjectiles, damageM, UnityEngine.Random.Range(0f, 1f), AI);
                        }
                        else
                        {
                            gameObject.GetComponent<ProjectileInit>().OFFLINE_Init_noAmmoUse(holdable.holder.player.playerID, currentNumberOfProjectiles, damageM, UnityEngine.Random.Range(0f, 1f), AI);
                        }
                    }
                    else
                    {
                        gameObject.GetComponent<ProjectileInit>().OFFLINE_Init_SeparateGun(GetComponentInParent<Player>().playerID, gunID, currentNumberOfProjectiles, damageM, UnityEngine.Random.Range(0f, 1f));
                    }

                    if (timeBetweenBullets != 0f)
                    {
                        //CameraShake.Instance.ImShakingBroooooo(base.transform.up * shake);
                        SoundGun.volume = shotVolume;
                        SoundGun.PlayOneShot(shotSound);
                    }
                }
            }
            if (rig) rig.AddForce(rig.mass * recoil * Mathf.Clamp(0.3f, 0f, 1f) * -base.transform.up, ForceMode2D.Impulse);
            data.playerVel.AddForce(-base.transform.up * bodyRecoil, ForceMode2D.Impulse);
            if (useAmmo && (bool)gunAmmo)
            {
                gunAmmo.OneBulletPerShoot();
            }
            yield return new WaitForSeconds(timeBetweenBullets);
        }
    }

    public void BulletInit(GameObject bullet, int usedNumberOfProjectiles, float damageM, float randomSeed, bool useAmmo = true)
    {
        spawnedAttack = bullet.GetComponent<SpawnedAttack>();
        if (!spawnedAttack) spawnedAttack = bullet.AddComponent<SpawnedAttack>();
        if (!bullet.GetComponentInChildren<DontChangeMe>()) ApplyProjectileStats(bullet, usedNumberOfProjectiles, damageM, randomSeed);
        ApplyPlayerStuff(bullet);
        if (ShootPojectileAction != null)
        {
            ShootPojectileAction(bullet);
        }

        //if (useAmmo && (bool)gunAmmo)
        //{
        //    gunAmmo.Shoot(bullet);
        //}
    }

    private Quaternion getShootRotation(int bulletID, int numOfProj, float charge)
    {
        Vector3 forward = shootPosition.forward;
        if (forceShootDir != Vector3.zero)
        {
            forward = forceShootDir;
        }

        float d = multiplySpread * Mathf.Clamp(1f + charge * chargeSpreadTo, 0f, float.PositiveInfinity);
        float num = UnityEngine.Random.Range(0f - spread, spread);
        num /= (1f + projectileSpeed * 0.5f) * 0.5f;
        forward += Vector3.Cross(forward, Vector3.forward) * num * d;
        return Quaternion.LookRotation(lockGunToDefault ? shootPosition.forward : forward);
    }

    private void ApplyPlayerStuff(GameObject obj)
    {
        ProjectileHit component = obj.GetComponent<ProjectileHit>();
        component.ownWeapon = base.gameObject;
        if ((bool)data)
        {
            component.ownPlayer = data;
        }
        spawnedAttack.spawner = data;
        spawnedAttack.attackID = attackID;
    }

    private void ApplyProjectileStats(GameObject obj, int numOfProj = 1, float damageM = 1f, float randomSeed = 0f)
    {
        ProjectileHit component = obj.GetComponent<ProjectileHit>();
        component.dealDamageMultiplierr *= bulletDamageMultiplier;
        //component.damage *= damage * damageM;
        component.damage = damage * damageM; //todo: LHD Modify
        component.percentageDamage = percentageDamage;
        component.stun = component.damage / 150f;
        component.force *= knockback;
        component.movementSlow = slow;
        //component.hasControl = CheckIsMine();
        component.hasControl = true;
        component.projectileColor = projectileColor;
        component.unblockable = unblockable;
        RayCastTrail component2 = obj.GetComponent<RayCastTrail>();
        if (ignoreWalls)
        {
            component2.mask = component2.ignoreWallsMask;
        }

        if ((bool)component2)
        {
            component2.extraSize += size;
        }

        if ((bool)data)
        {
            //PlayerSkin teamColor = component.team = PlayerSkinBank.GetPlayerSkinColors(player.playerID);
            //obj.GetComponent<RayCastTrail>().teamID = player.playerID;
            //SetTeamColor.TeamColorThis(obj, teamColor);
        }

        List<ObjectsToSpawn> list = new List<ObjectsToSpawn>();
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            list.Add(objectsToSpawn[i]);
            if (!objectsToSpawn[i].AddToProjectile || (/*(bool)objectsToSpawn[i].AddToProjectile.gameObject.GetComponent<StopRecursion>() &&*/ isProjectileGun))
            {
                continue;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate(objectsToSpawn[i].AddToProjectile, component.transform.position, component.transform.rotation, component.transform);
            gameObject.transform.localScale *= 1f * (1f - objectsToSpawn[i].scaleFromDamage) + component.damage / 55f * objectsToSpawn[i].scaleFromDamage;
            if (objectsToSpawn[i].scaleStacks)
            {
                gameObject.transform.localScale *= 1f + (float)objectsToSpawn[i].stacks * objectsToSpawn[i].scaleStackM;
            }

            if (!objectsToSpawn[i].removeScriptsFromProjectileObject)
            {
                continue;
            }

            MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>();
            for (int j = 0; j < componentsInChildren.Length; j++)
            {
                if (componentsInChildren[j].GetType().ToString() != "SoundImplementation.SoundUnityEventPlayer")
                {
                    UnityEngine.Object.Destroy(componentsInChildren[j]);
                }
                //UnityEngine.Debug.Log(componentsInChildren[j].GetType().ToString());
            }
        }

        component.objectsToSpawn = list.ToArray();
        if (reflects > 0)
        {
            RayHitReflect rayHitReflect = obj.gameObject.AddComponent<RayHitReflect>();
            rayHitReflect.reflects = reflects;
            rayHitReflect.speedM = speedMOnBounce;
            rayHitReflect.dmgM = dmgMOnBounce;
        }

        if (!forceSpecificShake)
        {
            float num = component.damage / 100f * ((1f + usedCooldown) / 2f) / ((1f + (float)numOfProj) / 2f) * 2f;
            float num2 = Mathf.Clamp((0.2f + component.damage * (((float)numberOfProjectiles + 2f) / 2f) / 100f * ((1f + usedCooldown) / 2f)) * 1f, 0f, 3f);
            component.shake = num * shakeM;
            shake = num2;
        }

        MoveTransform component3 = obj.GetComponent<MoveTransform>();
        component3.localForce *= projectileSpeed;
        component3.simulationSpeed *= projectielSimulatonSpeed;
        component3.gravity *= gravity;
        component3.worldForce *= gravity;
        component3.drag = drag;
        component3.drag = Mathf.Clamp(component3.drag, 0f, 45f);
        component3.velocitySpread = Mathf.Clamp(spread * 50f, 0f, 50f);
        component3.dragMinSpeed = dragMinSpeed;
        component3.localForce *= Mathf.Lerp(1f - component3.velocitySpread * 0.01f,
            1f + component3.velocitySpread * 0.01f, randomSeed);
        component3.selectedSpread = 0f;
        if (damageAfterDistanceMultiplier != 1f)
        {
            //obj.AddComponent<ChangeDamageMultiplierAfterDistanceTravelled>().muiltiplier = damageAfterDistanceMultiplier;
        }

        if (cos > 0f)
        {
            //obj.gameObject.AddComponent<Cos>().multiplier = cos;
        }

        if (destroyBulletAfter != 0f)
        {
            obj.GetComponent<RemoveAfterSeconds>().seconds = destroyBulletAfter * destroyBulletAfterMultiplier; //todo: LHD Modify
            obj.GetComponent<SetScaleFromSizeAndExtraSize>().destroyBulletAfterMultiplier = destroyBulletAfterMultiplier;
        }

        if ((bool)spawnedAttack && projectileColor != Color.black)
        {
            spawnedAttack.SetColor(projectileColor);
        }

        if (degreaseSizeByTime)
        {
            obj.gameObject.AddComponent<DegreaseSizeByTime>();
        }
    }

    public void AddAttackAction(Action action)
    {
        attackAction = (Action)Delegate.Combine(attackAction, action);
    }

    internal void RemoveAttackAction(Action action)
    {
        attackAction = (Action)Delegate.Remove(attackAction, action);
    }

    public int GetGunID()
    {
        return this.gunID;
    }
}

public enum ShootType
{
    None,
    Straight,
    Curve
}