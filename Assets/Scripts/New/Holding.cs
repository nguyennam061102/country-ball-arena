using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Holding : MonoBehaviour
{
    public float force;

    public float drag;

    public Holdable holdable, holdablePrefab;

    private Transform handPos;

    private PlayerVelocity rig;

    //private GeneralInput input;

    private BaseCharacter data;

    //private Player player;

    //private Gun gun;

    private bool hasSpawnedGun;

    private void Awake()
    {
        handPos = GetComponentInChildren<HandPos>().transform;
        data = GetComponent<BaseCharacter>();
    }

    public void SpawnGun()
    {
        if (hasSpawnedGun)
        {
            Destroy(holdable.gameObject);
        }
        hasSpawnedGun = true;
        holdable = Instantiate(holdablePrefab, base.transform.position, Quaternion.identity);
        holdable.GetComponent<Holdable>().holder = data;
        holdable.GetComponent<Gun>().SetGun(data);
        rig = GetComponent<PlayerVelocity>();
        data = GetComponent<BaseCharacter>();
    }

    private void FixedUpdate()
    {
        if (holdable && holdable.rig)
        {
            holdable.rig.AddForce((handPos.transform.position + (Vector3)rig.velocity * 0.04f - holdable.transform.position) * force * holdable.rig.mass, ForceMode2D.Force);
            holdable.rig.AddForce(holdable.rig.velocity * (0f - drag) * holdable.rig.mass, ForceMode2D.Force);
            holdable.rig.transform.rotation = Quaternion.LookRotation(Vector3.forward, handPos.transform.forward);
        }
    }

    public void GetGunAndSpawn()
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival) || GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            if (data == null) data = GetComponent<BaseCharacter>();
            if (data.player.playerID == 0)
            {
                holdablePrefab = data.weaponList[GameData.CurrentMainHandId];
            }
            else
            {
                var maxValue = (int)(GameData.MatchLevel) + 1;
                if (maxValue >= 11) maxValue = 11;
                holdablePrefab = data.weaponList[Random.Range(0, maxValue)];
            }
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
        {
            if (data.player.playerID == 0) holdablePrefab = data.weaponList[GameData.CurrentMainHandId];
            if (data.player.playerID == 1) holdablePrefab = data.weaponList[GameFollowData.Instance.Player2WeaponID];
            else if (data.player.playerID == 2) holdablePrefab = data.weaponList[GameFollowData.Instance.Player3WeaponID];
        }
        SpawnGun();
        GameController.Instance.ApplyPlayerGunStatOnSpawn(data.player);
    }

    public void GetRandomPlayerGunAndSpawn()
    {
        List<int> canGetWeapon = new List<int>();
        for (int i = 1; i < data.weaponList.Length; i++)
        {
            if (data.weaponList[i] != holdablePrefab) canGetWeapon.Add(i);
        }
        int idToTake = canGetWeapon[Random.Range(0, canGetWeapon.Count)];
        holdablePrefab = data.weaponList[idToTake];
        SpawnGun();
        GameController.Instance.ApplyPlayerGunStatOnSpawn(data.player);
    }

    public void SetActive(bool active)
    {
        holdable.gameObject.SetActive(active);
    }
}
