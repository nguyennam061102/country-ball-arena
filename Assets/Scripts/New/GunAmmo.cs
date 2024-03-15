using UnityEngine;
using UnityEngine.UI;

public class GunAmmo : MonoBehaviour
{
	public BaseCharacter data;

	[Header("Settings")]
	public int maxAmmo = 3;

	private int lastMaxAmmo;

	private int currentAmmo;

	public float reloadTime = 1.5f;

	public float reloadTimeMultiplier = 1f;

	public float reloadTimeAdd;

	private Gun gun;

	//public CurveAnimation reloadAnim;

	public Image reloadRing;

	public Image cooldownRing;

	public float reloadCounter;

	private float freeReloadCounter;

	public Populate populate;

	public float ammoReg;

	private float currentRegCounter;

	private void Start()
	{
		gun = GetComponentInParent<Gun>();
		data = GetComponentInParent<BaseCharacter>();

        lastMaxAmmo = maxAmmo;
		currentAmmo = maxAmmo;
        //ReDrawTotalBullets();
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(false);
	}

	private float ReloadTime()
	{
		return (reloadTime + reloadTimeAdd) * reloadTimeMultiplier / (1 + CharacterStats.Instance.TalentCooldown);
	}

	//private void Update()
	//{
	//	if (gun.isReloading)
	//	{
	//		reloadCounter -= TimeHandler.deltaTime;
	//		if (reloadCounter < 0f)
	//		{
	//			ReloadAmmo();
	//		}
	//	}
	//	else if (currentAmmo != maxAmmo)
	//	{
	//		freeReloadCounter += TimeHandler.deltaTime;
	//		if (freeReloadCounter > ReloadTime() /*&& gun.player.data.stats.automaticReload*/)
	//		{
	//			currentAmmo = maxAmmo;
	//			SetActiveBullets();
	//		}
	//		currentRegCounter += ammoReg * TimeHandler.deltaTime * (float)maxAmmo;
	//		if (currentRegCounter > 1f)
	//		{
	//			currentAmmo++;
	//			currentRegCounter = 0f;
	//			SetActiveBullets();
	//		}
	//	}
	//	reloadRing.fillAmount = (ReloadTime() - reloadCounter) / ReloadTime();
	//	reloadRing.gameObject.SetActive(reloadCounter > 0);
	//	if (maxAmmo != lastMaxAmmo)
	//	{
	//		ReDrawTotalBullets();
	//	}
	//	lastMaxAmmo = maxAmmo;
	//}

	public void ReloadAmmo(bool playSound = true)
	{
		//gun.player.data.stats.OnReload(maxAmmo - currentAmmo);
		gun.isReloading = false;
		currentAmmo = maxAmmo;
		// SoundStopReloadInProgress();
		// if (playSound)
		// {
		// 	SoundManager.Instance.Play(soundReloadComplete, base.transform);
		// }
		SetActiveBullets();
	}

	public void Shoot(GameObject projectile)
	{
		//currentAmmo--;
		freeReloadCounter = 0f;
		SetActiveBullets();
		if (currentAmmo <= 0)
		{
			reloadCounter = ReloadTime();
			gun.isReloading = true;
			gun.SoundGun.volume = gun.reloadVolume;
			gun.SoundGun.PlayOneShot(gun.reloadSound);
			//gun.player.data.stats.OnOutOfAmmp(maxAmmo);
		}
	}

    public void OneBulletPerShoot()
    {
        //currentAmmo--;
        freeReloadCounter = 0f;
        SetActiveBullets();
        if (currentAmmo <= 0)
        {
            reloadCounter = ReloadTime();
            gun.isReloading = true;
            gun.SoundGun.volume = gun.reloadVolume;
            gun.SoundGun.PlayOneShot(gun.reloadSound);
        }
    }

	public void ReDrawTotalBullets()
	{
		currentAmmo = maxAmmo;
		for (int num = populate.transform.childCount - 1; num >= 0; num--)
		{
			if (populate.transform.GetChild(num).gameObject.activeSelf)
			{
				UnityEngine.Object.Destroy(populate.transform.GetChild(num).gameObject);
			}
		}
		populate.times = maxAmmo;
		populate.DoPopulate();
		SetActiveBullets(forceTurnOn: true);
	}

	private void SetActiveBullets(bool forceTurnOn = false, bool active = true)
	{
		//for (int i = 1; i < populate.transform.childCount; i++)
		// {
		// 	if ((i <= currentAmmo) | forceTurnOn)
		// 	{
		// 		// if ((populate.transform.GetChild(i).GetComponent<CurveAnimation>().currentState != CurveAnimationUse.In) | forceTurnOn)
		// 		// {
		// 		// 	populate.transform.GetChild(i).GetComponent<CurveAnimation>().PlayIn();
		// 		// }
		// 		Debug.Log("1");
		// 		populate.UpdateChild(i);
		// 	}
		// 	// else if (populate.transform.GetChild(i).GetComponent<CurveAnimation>().currentState != CurveAnimationUse.Out)
		// 	// {
		// 	// 	populate.transform.GetChild(i).GetComponent<CurveAnimation>().PlayOut();
		// 	// }
		// 	else
		// 	{
		// 		Debug.Log("2");
		// 		populate.UpdateChild(i, false);
		// 	}
		// }
		for (int i = 0; i < populate.list.Count; i++)
		{
			if (i <= currentAmmo - 1)
			{
				populate.UpdateChild(i);
			}
			else
			{
				populate.UpdateChild(i, false);
			}
		}
	}

	public void ResetAmmo()
	{
		currentAmmo = maxAmmo;
		SetActiveBullets();
	}
}
