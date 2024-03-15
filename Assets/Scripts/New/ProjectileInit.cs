using UnityEngine;

public class ProjectileInit : MonoBehaviour
{

	internal void OFFLINE_Init(int senderID, int nrOfProj, float dmgM, float randomSeed, bool AI = false)
	{
		PlayerManager.Instance.players[senderID].Gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed);
		if (!AI)
		{
			//PlayerManager.instance.player.Gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed);
			DailyMission.Instance.dailyMissionList[5].MissionProgress++;
		}
		// else
		// {
		// 	PlayerManager.instance.target.Gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed);
		// }
	}

	internal void OFFLINE_Init_SeparateGun(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed)
	{
		GetChildGunWithID(gunID, PlayerManager.Instance.players[senderID].gameObject).BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed);
	}

	private Gun GetChildGunWithID(int id, GameObject player)
	{
        foreach(Gun g in player.GetComponentsInChildren<Gun>())
        {
            if (g.GetGunID() == id) return g;
        }
        return null;
	}

	internal void OFFLINE_Init_noAmmoUse(int senderID, int nrOfProj, float dmgM, float randomSeed, bool AI)
	{
		PlayerManager.Instance.players[senderID].Gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, useAmmo: false);
		// if (!AI)
		// {
		// 	PlayerManager.instance.player.blockTrigger.blockGun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
		// 	//PlayerManager.Instance.player.holding.holdable.GetComponent<Gun>().BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
		// }
		// else
		// {
		// 	PlayerManager.instance.target.blockTrigger.blockGun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
		// 	//PlayerManager.Instance.target.holding.holdable.GetComponent<Gun>().BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
		// }
	}
}
