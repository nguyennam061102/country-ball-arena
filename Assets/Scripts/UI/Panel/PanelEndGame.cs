using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PanelEndGame : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    private CharacterStats Stats => CharacterStats.Instance;
    private PlayerManager playerManager => PlayerManager.Instance;
    private MovementController Mc => gameController.MovementController;
    private GameController gameController => GameController.Instance;
    private GameFollowData GfData => GameFollowData.Instance;

    //public bool win;
    public int killCount;
    public int matchLevel;
    public bool win;

    [SerializeField] private GameObject levelComplete, levelFail;

    [SerializeField] private float currentExp, targetExp, expReward, tmpExp;
    [SerializeField] private UILabel expRewardText, expProgress, levelText, goldReward, diamondReward, stage;
    [SerializeField] private UI2DSprite fillExp;
    private bool updateExp;

    [SerializeField] private UIButton watchButton;
    [SerializeField] private UILabel x2Text;
    private int tmp;

    [Header("UPGRADE")]
    [SerializeField] private GroupPlayerShow pShow;
    //[SerializeField] private SpriteRenderer mainHand, offHand;
    //[SerializeField] private Sprite[] mainHandList, offHandList;

    [Header("WEAPON")]
    [SerializeField] private UI2DSprite weaponIcon;
    [SerializeField] private UI2DSprite weaponFill;
    [SerializeField] private UIButton freeUpgradeButton, watchUpgradeButton;
    [SerializeField] private UILabel weaponText, weaponPercent;

    [SerializeField] private UILabel labelPlayMore, lbNextLevel;

    private List<ShopItemInfo> tmpList;

    private int Rate
    {
        get => PlayerPrefs.GetInt("RateGame", 0);
        set => PlayerPrefs.SetInt("RateGame", value);
    }

    [SerializeField] private GameObject panelRate;

    private int WeaponEndGameId
    {
        get => PlayerPrefs.GetInt("WeaponEndGameId", -1);
        set => PlayerPrefs.SetInt("WeaponEndGameId", value);
    }

    private float WeaponFillAmount
    {
        get => PlayerPrefs.GetFloat("WeaponFillAmount", 0);
        set => PlayerPrefs.SetFloat("WeaponFillAmount", value);
    }

    private bool unlockWeapon;

    private void OnEnable()
    {
        if (GameData.RateType) return;
        Rate++;
        if (Rate % 3 == 0) panelRate.SetActive(true);
    }

    private void Start()
    {
        gameController.gameplayCamera.enabled = false;
        levelComplete.SetActive(win);
        levelFail.SetActive(!win);
        
        //SkygoBridge.instance.ShowInterstitial(null);

        //DailyMission.Instance.dailyMissionList[2].MissionProgress++;
        killCount = gameController.PlayerKillCount;
        matchLevel = GameData.MatchLevel;

        //if (Mc != null) Mc.movementGo.SetActive(false);
        Mc.ShowIngameUI(false);
        GetInfo();
        GetAnim();
        GetWeaponUpgrade();
        if (win) stage.text = $"Stage {GameData.MatchLevel - 1 }";
        else stage.text = $"Stage {GameData.MatchLevel}";

        lbNextLevel.text = win ? "Next Level" : "Replay";
    }

    void GetAnim()
    {
        pShow.ShowPlayer("", GameData.CurrentSkinId, GameData.CurrentMainHandId, GameData.CurrentOffHandId);
        //pShow.skeleton.SetSkin(GameData.CurrentSkinId.ToString());
        //pShow.skeleton.SetSlotsToSetupPose();
        //pShow.AnimationState.Apply(pShow.Skeleton);
        //var rnd = Random.Range(0, 2);
        //switch (rnd)
        //{
        //    case 0:
        //        pShow.state.SetAnimation(0, "idle 1", true);
        //        pShow.timeScale = Random.Range(0.8f, 1.2f);
        //        break;
        //    case 1:
        //        pShow.state.SetAnimation(0, "idle 2", true);
        //        pShow.timeScale = Random.Range(0.8f, 1.2f);
        //        break;
        //}
        //mainHand.sprite = mainHandList[GameData.CurrentMainHandId];
        //mainHand.transform.localScale = new Vector3(1.5f, 1.5f);
        //mainHand.transform.SetEulerAnglesZAxis(45f);
        //offHand.sprite = offHandList[GameData.CurrentOffHandId];
        //offHand.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //offHand.transform.SetEulerAnglesZAxis(180);
        //offHand.transform.localRotation = Quaternion.Euler(0, 0, 180f);
    }

    void GetInfo()
    {
        expReward = GetExp();
        expRewardText.text = $"EXP: +{expReward}";

        levelText.text = GameData.PlayerLevel.ToString();
        currentExp = GameData.CurrentExp;
        targetExp = GetTargetExp;
        fillExp.fillAmount = currentExp / targetExp;
        expProgress.text = $"{currentExp}/{targetExp}";

        goldReward.text = $"+{(int)GetGold()}";
        diamondReward.text = "+" + gameController.ThisMatchKillCount + " (" + gameController.ThisMatchKillCount + " kills)";

        GameData.Gold += (int)GetGold();
        GameData.Diamond += gameController.ThisMatchKillCount;
        tmpExp = currentExp + expReward;

        tmp = Random.Range(2, 4);
        x2Text.text = $"+{(int)GetGold() * tmp}";

        updateExp = true;
    }
    private void Update()
    {
        if (GameData.PlayerLevel == 80)
        {
            fillExp.fillAmount = 1;
            return;
        }
        currentExp = Mathf.Round(currentExp);
        expProgress.text = $"{currentExp}/{targetExp}";

        if (!updateExp) return;

        currentExp += (targetExp / 2) * Time.fixedDeltaTime;
        fillExp.fillAmount += 0.5f * Time.fixedDeltaTime;
        if (currentExp >= targetExp)
        {
            updateExp = false;
            GameData.PlayerLevel++;
            GameData.FreeUpgradeTalent += 3;
            GameData.Diamond += 5;
            GameEventTrackerProVCL.Instance.OnPlayerLevelUp();
            levelText.text = GameData.PlayerLevel.ToString();
            tmpExp -= targetExp;
            currentExp = 0;
            fillExp.fillAmount = 0;
            targetExp = GetTargetExp;
            updateExp = true;
        }

        if (currentExp >= tmpExp)
        {
            currentExp = tmpExp;
            GameData.CurrentExp = currentExp;
            updateExp = false;
        }
    }

    void GetWeaponUpgrade()
    {
        tmpList = new List<ShopItemInfo>();
        bool wpnorupgrd = false;

        if (GfData.IsAllMainHandGoldUnlock)
        {
            wpnorupgrd = false;
            foreach (var itemInfo in GfData.mainHandList)
            {
                if (itemInfo.IsLevelMax || itemInfo.currencyType == CurrencyType.Diamond) continue;
                tmpList.Add(itemInfo);
            }

            if (WeaponEndGameId == -1 || tmpList.Count < WeaponEndGameId + 1) WeaponEndGameId = Random.Range(0, tmpList.Count);

            weaponIcon.sprite2D = tmpList[WeaponEndGameId].itemIcon;
            //weaponIcon.MakePixelPerfect();
            weaponFill.sprite2D = tmpList[WeaponEndGameId].itemIcon;
            weaponFill.MakePixelPerfect();

            var priceToUpgrade = Mathf.Round(tmpList[WeaponEndGameId].basePriceToUpgrade * Mathf.Pow(1.08f, tmpList[WeaponEndGameId].ItemLevel));
            WeaponFillAmount += (int)GetGold() * 0.8f / priceToUpgrade;
            weaponFill.fillAmount = WeaponFillAmount;
            weaponPercent.text = $"{(int)(WeaponFillAmount * 100)}" + "%";
        }
        else
        {
            wpnorupgrd = true;
            foreach (var itemInfo in GfData.mainHandList)
            {
                if (!itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold) tmpList.Add(itemInfo);
            }
            if (WeaponEndGameId == -1 || tmpList.Count < WeaponEndGameId + 1) WeaponEndGameId = Random.Range(0, tmpList.Count);

            weaponIcon.sprite2D = tmpList[WeaponEndGameId].itemIcon;
            //weaponIcon.MakePixelPerfect();
            weaponFill.sprite2D = tmpList[WeaponEndGameId].itemIcon;
            //weaponFill.MakePixelPerfect();

            WeaponFillAmount += (int)GetGold() * 0.8f / tmpList[WeaponEndGameId].price;
            weaponFill.fillAmount = WeaponFillAmount;
            weaponPercent.text = $"{(int)(WeaponFillAmount * 100)}" + "%";

            if (weaponFill.fillAmount < 1)
            {
                freeUpgradeButton.gameObject.SetActive(false);
                watchUpgradeButton.gameObject.SetActive(false);
                return;
            }
            unlockWeapon = true;
        }

        bool isFreeForYou = Random.Range(0f, 1f) < 0.5f;
        freeUpgradeButton.gameObject.SetActive(WeaponFillAmount >= 1 && isFreeForYou);
        freeUpgradeButton.GetComponentInChildren<UILabel>().text = wpnorupgrd ? "Free Weapon" : "Free Upgrade";
        watchUpgradeButton.gameObject.SetActive(WeaponFillAmount >= 1 && !isFreeForYou);
        watchUpgradeButton.GetComponentInChildren<UILabel>().text = wpnorupgrd ? "Watch Video\nFree Weapon" : "Watch Video\nFree Upgrade";

        labelPlayMore.text = weaponFill.fillAmount >= 1 ? "" : "Play more to get this";
        weaponText.text = weaponFill.fillAmount >= 1 ? "Just For You!" : "What is this!";
        weaponPercent.gameObject.SetActive(weaponFill.fillAmount < 1f);
    }

    public void OnFreeUpgradeWeaponButton()
    {
        if (unlockWeapon)
        {
            tmpList[WeaponEndGameId].IsUnlocked = true;
            freeUpgradeButton.enabled = false;
            freeUpgradeButton.GetComponentInChildren<UILabel>().text = "Unlocked";
        }
        else
        {
            tmpList[WeaponEndGameId].ItemLevel++;
            freeUpgradeButton.enabled = false;
            freeUpgradeButton.GetComponentInChildren<UILabel>().text = "Upgraded";
        }
    }

    public void OnWatchFreeUpgradeWeaponButton()
    {
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            if (unlockWeapon)
            {
                tmpList[WeaponEndGameId].IsUnlocked = true;
                watchUpgradeButton.enabled = false;
                watchUpgradeButton.GetComponentInChildren<UILabel>().text = "Unlocked";
            }
            else
            {
                tmpList[WeaponEndGameId].ItemLevel++;
                watchUpgradeButton.enabled = false;
                watchUpgradeButton.GetComponentInChildren<UILabel>().text = "Upgraded";
            }
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("freeweaponsurvival");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    private float GetBaseExp()
    {
        if (killCount > 0) return 25 * matchLevel + 100 * killCount;
        return 100;
    }

    private float GetExp()
    {
        return Mathf.Round(GetBaseExp() * Mathf.Pow(1.09f, matchLevel - 1) * (1 + Stats.TalentExp));
    }

    private float GetTargetExp => (float)Math.Round(60 * Mathf.Pow(GameData.PlayerLevel + 1, 2.8f) - 60);

    private float GetBaseGold()
    {
        if (killCount > 0) return 250 * matchLevel + 1000 * killCount;
        return 300;
    }

    private float GetGold()
    {
        return Mathf.Round(GetBaseGold() * Mathf.Pow(1.1f, matchLevel - 1) * (1 + Stats.TalentGoldBonus)) * GameEventTrackerProVCL.Instance.CoinMultipleAmount;
    }

    public void OnWatchX2Button()
    {
        var onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            GameData.Gold += (int)GetGold() * tmp;
            watchButton.gameObject.SetActive(false);
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("endgamesurvival");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    public void OnUpgradeButton()
    {
        //ne ne
        //MenuController.Instance.showCharacterPanel = true;
        GameFollowData.Instance.IsToUpgradeCharacter = true;
        
        Loading.Instance.LoadScene("menu");
        Destroy(gameObject);
    }

    public void OnHomeButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onCloseInterEvent = new UnityEvent();
        onCloseInterEvent.AddListener(() =>
        {
            GameFollowData.Instance.IsChangeSceneFromGameplay = true;
            
            if (weaponFill.fillAmount >= 1)
            {
                WeaponEndGameId = -1;
                WeaponFillAmount = 0;
            }
            Loading.Instance.LoadScene("menu");
            Destroy(gameObject);
        });
        onCloseInterEvent.Invoke();
        //inter
        //bool flag = ApplovinBridge.instance.ShowInterAdsApplovin(onCloseInterEvent);
        //if (!flag) onCloseInterEvent.Invoke();
    }

    public void OnContinueButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onCloseInterEvent = new UnityEvent();
        onCloseInterEvent.AddListener(() =>
        {
            if (weaponFill.fillAmount >= 1)
            {
                WeaponEndGameId = -1;
                WeaponFillAmount = 0;
            }
            Loading.Instance.LoadScene("game");
            Destroy(gameObject);
        });
        onCloseInterEvent.Invoke();
        //inter
        //bool flag = ApplovinBridge.instance.ShowInterAdsApplovin(onCloseInterEvent);
        //if (!flag) onCloseInterEvent.Invoke();
    }
}
