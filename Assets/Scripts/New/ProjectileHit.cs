using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileHit : RayHit
{
    private CameraShake camShake => CameraShake.Instance;
    
    [HideInInspector] public bool canPushBox = true;

    public float force;

    public float damage;

    public float stun;

    public float percentageDamage;

    public float movementSlow;

    public float shake;

    public ObjectsToSpawn[] objectsToSpawn;

    //public PlayerSkin team;

    public BaseCharacter ownPlayer;

    [HideInInspector] public GameObject ownWeapon;

    public AnimationCurve effectOverTimeCurve;

    //[HideInInspector]
    public List<RayHitEffect> effects;

    private List<HealthHandler> playersHit = new List<HealthHandler>();

    public Color projectileColor = Color.black;

    private Action hitAction;

    private Action<HitInfo> hitActionWithData;

    [HideInInspector] public bool unblockable;

    [HideInInspector] public bool fullSelfDamage;

    public UnityEvent deathEvent;

    public bool destroyOnBlock;

    public float holdPlayerFor = 0.5f;

    public string bulletImmunity = "";

    private SpawnedAttack spawnedAttack;

    [HideInInspector] public float sinceReflect = 10f;

    [HideInInspector] public float dealDamageMultiplierr = 1f;

    internal bool hasControl;

    // [HideInInspector]
    // public PhotonView view;

    private MoveTransform move;

    [HideInInspector] public bool bulletCanDealDeamage = true;

    [HideInInspector] public bool isAllowedToSpawnObjects = true;

    public bool sendCollisions = true;

    public Dictionary<string, Action> customActions = new Dictionary<string, Action>();

    public Dictionary<string, Action<Vector2, Vector2>> customActionsV2V2 =
        new Dictionary<string, Action<Vector2, Vector2>>();

    //public Player player;
    [SerializeField] private GameObject hitBlockFx, hitCharacterFx;
    [SerializeField] private BulletHitSound soundHitPrefab;

    private void Start()
    {
        move = GetComponent<MoveTransform>();
        //view = GetComponent<PhotonView>();
        
        //todo: LHD modify
        effects.AddRange(GetComponentsInChildren<RayHitEffect>());
        effects.Sort((RayHitEffect p1, RayHitEffect p2) => p2.priority.CompareTo(p1.priority));
        //effects.Add(GetComponentInChildren<RayHitReflect>());
        
        spawnedAttack = GetComponent<SpawnedAttack>();
        if ((bool) spawnedAttack && !ownPlayer)
        {
            ownPlayer = spawnedAttack.spawner;
        }

        if ((bool) ownPlayer && !fullSelfDamage)
        {
            StartCoroutine(HoldPlayer(ownPlayer.GetComponent<HealthHandler>()));
        }

        //damage *= base.transform.localScale.x; todo: LHD modify
        if (spawnedAttack.spawner != null) force *= 0.3f * (1 + spawnedAttack.spawner.stats.forceMultiplier);
    }

    public void ResortHitEffects()
    {
        //effects.Sort((RayHitEffect p1, RayHitEffect p2) => p2.priority.CompareTo(p1.priority));
    }

    private IEnumerator HoldPlayer(HealthHandler player)
    {
        if ((bool) player)
        {
            playersHit.Add(player);
        }

        yield return new WaitForSeconds(holdPlayerFor);
        if (playersHit.Contains(player))
        {
            playersHit.Remove(player);
        }
    }

    private void Update()
    {
        sinceReflect += TimeHandler.deltaTime;
    }

    public void AddPlayerToHeld(HealthHandler health)
    {
        StartCoroutine(HoldPlayer(health));
    }

    public void RemoveOwnPlayerFromPlayersHit()
    {
        if ((bool) ownPlayer && playersHit.Contains(ownPlayer.GetComponent<HealthHandler>()))
        {
            playersHit.Remove(ownPlayer.GetComponent<HealthHandler>());
        }
    }

    public override void Hit(HitInfo hit, bool forceCall = false)
    {
//        Debug.Log($"Hit: {hit.collider.name}");
        int num = -1;
        if ((bool)hit.transform)
        {
        	var component = hit.transform.root.GetComponent<BaseCharacter>();
        	 if ((bool)component)
        	 {
        	 	num = component.ViewID;
        	 }
        }
        int num2 = -1;
        if (num == -1)
        {
        	// Collider2D[] componentsInChildren = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>();
        	// for (int i = 0; i < componentsInChildren.Length; i++)
        	// {
        	// 	if (componentsInChildren[i] == hit.collider)
        	// 	{
        	// 		num2 = i;
        	// 	}
        	// }
        }
        HealthHandler healthHandler = null;
        if ((bool)hit.transform)
        {
        	healthHandler = hit.transform.GetComponent<HealthHandler>();
        }
        bool flag = false;
        if ((bool)healthHandler)
        {
        	if (playersHit.Contains(healthHandler))
        	{
        		return;
        	}
        	if (/*player.IsMine && */healthHandler.GetComponent<Block>().IsBlocking())
        	{
        		flag = true;
        	}
        	StartCoroutine(HoldPlayer(healthHandler));
        }
            
        DoHit(hit, move.velocity, num, num2, flag);

        // if (player.IsMine | forceCall)
        //  if (forceCall)
        //  {
        //      if (sendCollisions)
        //      {
        //          // view.RPC("RPCA_DoHit", RpcTarget.All, hit.point, hit.normal, (Vector2) move.velocity, num, num2,
        //          //     flag);
        //      }
        //      else
        //      {
        //          DoHit(hit, move.velocity, -1, -1, flag);
        //      }
        //  }
    }

    //void DoHit(Vector2 hitPoint, Vector2 hitNormal, Vector2 vel, bool wasBlocked = false)
    public float t = 0.005f;
    void DoHit(HitInfo hit, Vector2 vel, int viewID = -1, int colliderID = -1, bool wasBlocked = false)
    {
        //camShake.ImShakingBroooooo(-vel);
        //todo: LHD Modify
        if (hit.collider.CompareTag("Block") && hitBlockFx != null && !(bool) GetComponentInChildren<RayHitDrill>())
        {
            var hitSound = Instantiate(soundHitPrefab, Vector3.zero, Quaternion.identity);
            hitSound.PlaySound();
            var hitFx = Instantiate(hitBlockFx, hit.point, Quaternion.identity);
            hitFx.transform.up = hit.normal;
            hitFx.GetComponentInChildren<ParticleSystem>().Play();
        }

        if (hit.collider.CompareTag("Player") && hitCharacterFx != null)
        {
            var hitSound = Instantiate(soundHitPrefab, Vector3.zero, Quaternion.identity);
            hitSound.PlaySound(false);
            var rotation = Quaternion.LookRotation(hit.normal + Vector2.right * t);
            // var hitFx = Instantiate(hitCharacterFx, hit.point, rotation);
            // var mainColor = hitFx.GetComponentInChildren<ParticleSystem>().main;
            // mainColor.startColor = hit.collider.GetComponent<BaseCharacter>().skinHandler.ColorForSetup;
        }
        
        
        HitInfo hitInfo = new HitInfo();
        if ((bool) move)
        {
            move.velocity = vel;
        }

        hitInfo = hit;
        //hitInfo.point = hit.point;
        //hitInfo.normal = hit.normal;
        //hitInfo.collider = null;

        // if (player.AI)
        // {
        //     hitInfo.collider = player.GetComponentInChildren<Collider2D>();
        //     hit.transform = player.transform;
        // }

        HealthHandler healthHandler = null;
        if ((bool) hitInfo.transform)
        {
            healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
        }
        
        if (isAllowedToSpawnObjects)
        {
            base.transform.position = hitInfo.point;
        }
        
        //todo: new
        if ((bool)hitInfo.collider)
        {
            ProjectileHitSurface component = hitInfo.collider.GetComponent<ProjectileHitSurface>();
            if ((bool)component && component.HitSurface(hitInfo, base.gameObject) == ProjectileHitSurface.HasToStop.HasToStop)
            {
                return;
            }
        }
        if ((bool)healthHandler)
        {
            Block component2 = healthHandler.GetComponent<Block>();
            if (wasBlocked)
            {
                component2.DoBlock(base.gameObject, base.transform.forward, hitInfo.point);
                if (destroyOnBlock)
                {
                    DestroyMe();
                }
                sinceReflect = 0f;
                return;
            }
            CharacterStatModifiers component3 = healthHandler.GetComponent<CharacterStatModifiers>();
            if (movementSlow != 0f && !wasBlocked)
            {
                component3.RPCA_AddSlow(movementSlow); //todo: Slow
            }
        }
        //

        float num = 1f;
        PlayerVelocity playerVelocity = null;
        if ((bool) hitInfo.transform)
        {
            playerVelocity = hitInfo.transform.GetComponentInParent<PlayerVelocity>();
            //if ((bool) playerVelocity) Debug.Log("has player velocity");
        }
        
        //todo: Take Damage
        if ((bool)hitInfo.collider)
        {
            Damagable componentInParent = hitInfo.collider.GetComponentInParent<Damagable>();
            if ((bool)componentInParent)
            {
                if ((bool)healthHandler && percentageDamage != 0f)
                {
                    damage += healthHandler.GetComponent<BaseCharacter>().maxHealth * percentageDamage;
                }
                //if (hasControl)
                {
                    // if (bulletImmunity != "" && (bool)healthHandler)
                    // {
                    //     healthHandler.GetComponent<PlayerImmunity>().IsImune(0.1f, (bulletCanDealDeamage ? damage : 1f) * dealDamageMultiplierr, bulletImmunity);
                    // }
                    if ((bool)componentInParent.GetComponent<Damagable>())
                    {
                        componentInParent.CallTakeDamage(base.transform.forward * damage * dealDamageMultiplierr, damage, hitInfo.point, ownWeapon, ownPlayer);
                    }
                    else
                    {
                        componentInParent.CallTakeDamage(base.transform.forward * (bulletCanDealDeamage ? damage : 1f) * dealDamageMultiplierr, damage, hitInfo.point, ownWeapon, ownPlayer);
                    }
                }
            }
        }
        //
        
        if ((bool) playerVelocity)
        {
            //Debug.Log("velocity");
            float num2 = 1f;
            float d = Mathf.Clamp(playerVelocity.mass / 100f * num2, 0f, 1f) * num2;
            float d2 = 1f;
            playerVelocity.AddForce(-playerVelocity.velocity * 0.1f * playerVelocity.mass, ForceMode2D.Impulse);
            if ((bool) healthHandler)
            {
                num *= 3f;
                if (hasControl)
                {
                    healthHandler.CallTakeForce(base.transform.forward * d2 * d * force);
                }
            }
        }
        
        if (isAllowedToSpawnObjects && !wasBlocked)
        {
            //CameraShake.Instance.ImShakingBroooooo(base.transform.forward * num * shake);
            //DynamicParticles.instance.PlayBulletHit(damage, base.transform, hitInfo, projectileColor); todo: Bullet Hit Fx
            for (int i = 0; i < objectsToSpawn.Length; i++)
            {
                ObjectsToSpawn.SpawnObject(base.transform, hitInfo, objectsToSpawn[i], healthHandler, damage, spawnedAttack, wasBlocked);
            }
        
            base.transform.position = hitInfo.point + hitInfo.normal * 0.01f;
        }
        
        // if ((bool)hitInfo.transform) //todo: Bullet Push
        // {
        //     NetworkPhysicsObject component4 = hitInfo.transform.GetComponent<NetworkPhysicsObject>();
        //     if ((bool)component4 && canPushBox)
        //     {
        //         component4.BulletPush(base.transform.forward * (force * 0.5f + damage * 100f), hitInfo.transform.InverseTransformPoint(hitInfo.point), spawnedAttack.spawner.data);
        //     }
        // }
        
        bool flag = false;
        if (effects != null && effects.Count != 0)
        {
            for (int j = 0; j < effects.Count; j++)
            {
                HasToReturn num3 = effects[j].DoHitEffect(hitInfo);

                //Debug.Log($"num3: {num3.ToString()}");
                
                if (num3 == HasToReturn.canContinue)
                {
                    flag = true;
                    //return;
                }

                if (num3 == HasToReturn.hasToReturn)
                {
                    flag = false;
                }

                if (num3 == HasToReturn.hasToReturnNow)
                {
                    return;
                }
            }
        }

        //Debug.Log($"flag: {flag}");

        
        if (!flag)
        {
            hitAction?.Invoke();
            hitActionWithData?.Invoke(hitInfo);
            deathEvent.Invoke();
            DestroyMe();
        }
    }

    // [PunRPC]
    // public void RPCA_DoHit(Vector2 hitPoint, Vector2 hitNormal, Vector2 vel, int viewID = -1, int colliderID = -1,
    //     bool wasBlocked = false)
    // {
    //     Debug.Log("RPCA_DoHit");
    //     HitInfo hitInfo = new HitInfo();
    //     if ((bool) move)
    //     {
    //         move.velocity = vel;
    //     }
    //
    //     hitInfo.point = hitPoint;
    //     hitInfo.normal = hitNormal;
    //     hitInfo.collider = null;
    //     // if (viewID != -1)
    //     // {
    //     //     // PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
    //     //     // hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
    //     //     // hitInfo.transform = photonView.transform;
    //     // }
    //     // else if (colliderID != -1)
    //     // {
    //     //     //hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
    //     //     hitInfo.transform = hitInfo.collider.transform;
    //     // }
    //
    //     HealthHandler healthHandler = null;
    //     if ((bool) hitInfo.transform)
    //     {
    //         healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
    //     }
    //
    //     if (isAllowedToSpawnObjects)
    //     {
    //         base.transform.position = hitInfo.point;
    //     }
    //
    //     if ((bool) hitInfo.collider)
    //     {
    //         ProjectileHitSurface component = hitInfo.collider.GetComponent<ProjectileHitSurface>();
    //         if ((bool) component && component.HitSurface(hitInfo, base.gameObject) ==
    //             ProjectileHitSurface.HasToStop.HasToStop)
    //         {
    //             return;
    //         }
    //     }
    //
    //     if ((bool) healthHandler)
    //     {
    //         Block component2 = healthHandler.GetComponent<Block>();
    //         if (wasBlocked)
    //         {
    //             component2.DoBlock(base.gameObject, base.transform.forward, hitInfo.point);
    //             if (destroyOnBlock)
    //             {
    //                 DestroyMe();
    //             }
    //
    //             sinceReflect = 0f;
    //             return;
    //         }
    //
    //         CharacterStatModifiers component3 = healthHandler.GetComponent<CharacterStatModifiers>();
    //         if (movementSlow != 0f && !wasBlocked)
    //         {
    //             //component3.RPCA_AddSlow(movementSlow);
    //         }
    //     }
    //
    //     float num = 1f;
    //     PlayerVelocity playerVelocity = null;
    //     if ((bool) hitInfo.transform)
    //     {
    //         playerVelocity = hitInfo.transform.GetComponentInParent<PlayerVelocity>();
    //     }
    //
    //     if ((bool) hitInfo.collider)
    //     {
    //         Damagable componentInParent = hitInfo.collider.GetComponentInParent<Damagable>();
    //         if ((bool) componentInParent)
    //         {
    //             // if ((bool) healthHandler && percentageDamage != 0f)
    //             // {
    //             //     damage += healthHandler.GetComponent<BaseCharacter>().maxHealth * percentageDamage;
    //             // }
    //
    //             if (hasControl)
    //             {
    //                 if (bulletImmunity != "" && (bool) healthHandler)
    //                 {
    //                     //healthHandler.GetComponent<PlayerImmunity>().IsImune(0.1f, (bulletCanDealDeamage ? damage : 1f) * dealDamageMultiplierr, bulletImmunity);
    //                 }
    //                 // if ((bool)componentInParent.GetComponent<DamagableEvent>())
    //                 // {
    //                 // 	componentInParent.CallTakeDamage(base.transform.forward * damage * dealDamageMultiplierr, hitInfo.point, ownWeapon, ownPlayer);
    //                 // }
    //                 else
    //                 {
    //                     componentInParent.CallTakeDamage(
    //                         base.transform.forward * (bulletCanDealDeamage ? damage : 1f) * dealDamageMultiplierr,
    //                         damage, hitInfo.point, ownWeapon, ownPlayer);
    //                     Debug.Log("call here");
    //                 }
    //             }
    //         }
    //         Debug.Log("call here");
    //     }
    //
    //     if ((bool) playerVelocity)
    //     {
    //         float num2 = 1f;
    //         float d = Mathf.Clamp(playerVelocity.mass / 100f * num2, 0f, 1f) * num2;
    //         float d2 = 1f;
    //         playerVelocity.AddForce(-playerVelocity.velocity * 0.1f * playerVelocity.mass, ForceMode2D.Impulse);
    //         if ((bool) healthHandler)
    //         {
    //             num *= 3f;
    //             if (hasControl)
    //             {
    //                 //healthHandler.CallTakeForce(base.transform.forward * d2 * d * force);
    //             }
    //         }
    //         Debug.Log("call here");
    //     }
    //
    //     // if (isAllowedToSpawnObjects && !wasBlocked)
    //     // {
    //     //     GamefeelManager.GameFeel(base.transform.forward * num * shake);
    //     //     //DynamicParticles.instance.PlayBulletHit(damage, base.transform, hitInfo, projectileColor);
    //     //     for (int i = 0; i < objectsToSpawn.Length; i++)
    //     //     {
    //     //         ObjectsToSpawn.SpawnObject(base.transform, hitInfo, objectsToSpawn[i], healthHandler, damage,
    //     //             wasBlocked);
    //     //     }
    //     //
    //     //     base.transform.position = hitInfo.point + hitInfo.normal * 0.01f;
    //     //     Debug.Log("call here");
    //     // }
    //
    //     // if ((bool)hitInfo.transform)
    //     // {
    //     // 	NetworkPhysicsObject component4 = hitInfo.transform.GetComponent<NetworkPhysicsObject>();
    //     // 	if ((bool)component4 && canPushBox)
    //     // 	{
    //     // 		component4.BulletPush(base.transform.forward * (force * 0.5f + damage * 100f), hitInfo.transform.InverseTransformPoint(hitInfo.point), spawnedAttack.spawner.data);
    //     // 	}
    //     // }
    //     bool flag = false;
    //     if (effects != null && effects.Count != 0)
    //     {
    //         for (int j = 0; j < effects.Count; j++)
    //         {
    //             HasToReturn num3 = effects[j].DoHitEffect(hitInfo);
    //             if (num3 == HasToReturn.hasToReturn)
    //             {
    //                 flag = true;
    //             }
    //
    //             if (num3 == HasToReturn.hasToReturnNow)
    //             {
    //                 return;
    //             }
    //         }
    //     }
    //
    //     if (!flag)
    //     {
    //         if (hitAction != null)
    //         {
    //             hitAction();
    //         }
    //
    //         if (hitActionWithData != null)
    //         {
    //             hitActionWithData(hitInfo);
    //         }
    //
    //         deathEvent.Invoke();
    //         DestroyMe();
    //     }
    // }

    private void DestroyMe()
    {
        // if ((bool)view)
        // {
        // 	if (view.IsMine)
        // 	{
        // 		PhotonNetwork.Destroy(base.gameObject);
        // 	}
        // }
        // else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    // [PunRPC]
    // public void RPCA_CallCustomAction(string actionKey)
    // {
    // 	customActions[actionKey]();
    // }
    //
    // [PunRPC]
    // public void RPCA_CallCustomActionV2V2(string actionKey, Vector2 v1, Vector2 v2)
    // {
    // 	customActionsV2V2[actionKey](v1, v2);
    // }

    public void AddHitAction(Action action)
    {
        hitAction = (Action) Delegate.Combine(hitAction, action);
    }

    public void AddHitActionWithData(Action<HitInfo> action)
    {
        hitActionWithData = (Action<HitInfo>) Delegate.Combine(hitActionWithData, action);
    }
}