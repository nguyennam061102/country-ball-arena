using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class PanelDailyReward : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Execute DailyReward Item")]
    void Execute()
    {
        for (int i = 0; i < drItemList.Count; i++)
        {
            drItemList[i].id = i;
            drItemList[i].gameObject.name = $"day_{i + 1}";
            drItemList[i].icon.sprite2D = dailyRewardIconList[i];
            drItemList[i].icon.MakePixelPerfect();
            EditorUtility.SetDirty(drItemList[i]);
        }
    }
#endif

    private GameFollowData GfData => GameFollowData.Instance;
    private UiController UI => UiController.Instance;
    private DailyReward DailyReward => MenuController.Instance.dailyReward;

    public List<DailyRewardItem> drItemList;
    public DrSlotList todayReward;

    public Sprite[] dailyRewardIconList;

    public void OnClaimButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        foreach (var item in todayReward.drSlots)
        {
            if (item != null)
            {
                switch (item.drTypes)
                {
                    case DrType.Gold:
                        GameData.Gold += item.value;
                        break;
                    case DrType.Diamond:
                        GameData.Diamond += item.value;
                        break;
                    case DrType.Talent:
                        UpgradeRandomTalent(1);
                        break;
                    case DrType.Random:
                        RandomItem();
                        break;
                }
            }
        }

        string saveDaily = DateTime.Now.Year + ":" + DateTime.Now.Month + ":" + DateTime.Now.Day + ":" +
                           DailyReward.checkOrder;
        //DailyReward.xxx++;
        GameData.DailyReward = saveDaily;
        GameData.NewDayOpen = false;
        ShowRewardPanel();
    }

    public void OnClaimX2Button()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            foreach (var item in todayReward.drSlots)
            {
                if (item != null)
                {
                    switch (item.drTypes)
                    {
                        case DrType.Gold:
                            GameData.Gold += item.value * 2;
                            break;
                        case DrType.Diamond:
                            GameData.Diamond += item.value * 2;
                            break;
                        case DrType.Talent:
                            UpgradeRandomTalent(2);
                            break;
                        case DrType.Random:
                            RandomItem();
                            break;
                    }
                }
            }

            string saveDaily = DateTime.Now.Year + ":" + DateTime.Now.Month + ":" + DateTime.Now.Day + ":" +
                               DailyReward.checkOrder;
            GameData.DailyReward = saveDaily;
            GameData.NewDayOpen = false;
            ShowRewardPanel();
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("dailyreward");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    private int randomTalent;
    private string randomTalentValue;

    private List<TalentItemInfo> rndTalentList;
    private List<ShopItemInfo> rndShopItemList;

    void UpgradeRandomTalent(int t)
    {
        if (GfData.IsAllTalentMaxLevel)
        {
            GameData.Gold += 600;
            GameData.Diamond += 2;
            randomTalent = 5;
            randomTalentValue = "+600 Gold && +2 Diamond";
            return;
        }

        rndTalentList = GfData.talentItemList.Where(itemInfo => !itemInfo.IsLevelMax).ToList();
        randomTalent = Random.Range(0, rndTalentList.Count);

        GfData.talentItemList[randomTalent].Level += t;
        GameData.TalentUpgradedCount += t;
        randomTalentValue = $"{GfData.talentItemList[randomTalent].itemName} +{t} level";
    }

    private int randomItem;
    private string randomItemValue;
    private bool fullAll, weapon;

    void RandomItem()
    {
        var a = 0;
        var i = 0;
        fullAll = false;
        weapon = false;

        //todo: Full All
        if (GfData.IsAllMainHandGoldUnlock && GfData.IsAllMainHandGoldMaxLevel && GfData.IsAllSkinGoldUnlock)
        {
            fullAll = true;
            GameData.Gold += 5000;
            GameData.Diamond += 20;
            randomItem = 5;
            randomItemValue = "+5000 Gold && +20 Diamond";
        }

        rndShopItemList = new List<ShopItemInfo>();

        //todo: Not Unlock Full

        if (!GfData.IsAllMainHandGoldUnlock && !GfData.IsAllSkinGoldUnlock) //todo: Not Unlock Both
        {
            a = Random.Range(0, 2);
            switch (a)
            {
                case 0:
                    weapon = true;
                    rndShopItemList.AddRange(GfData.mainHandList.Where(itemInfo =>
                        !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold));
                    i = Random.Range(0, rndShopItemList.Count);
                    rndShopItemList[i].IsUnlocked = true;
                    randomItem = i;
                    randomItemValue = $"{rndShopItemList[i].itemName} Unlocked";
                    break;
                case 1:
                    rndShopItemList.AddRange(GfData.skinList.Where(itemInfo =>
                        !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold));
                    i = Random.Range(0, rndShopItemList.Count);
                    rndShopItemList[i].IsUnlocked = true;
                    randomItem = i;
                    randomItemValue = $"{rndShopItemList[i].itemName} Unlocked";
                    break;
            }
        }

        if (!GfData.IsAllMainHandGoldUnlock && GfData.IsAllSkinGoldUnlock) //todo: Skin Unlocked All
        {
            weapon = true;
            rndShopItemList.AddRange(GfData.mainHandList.Where(itemInfo =>
                !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold));
            i = Random.Range(0, rndShopItemList.Count);
            rndShopItemList[i].IsUnlocked = true;
            randomItem = i;
            randomItemValue = $"{rndShopItemList[i].itemName} Unlocked";
        }

        if (GfData.IsAllMainHandGoldUnlock && !GfData.IsAllSkinGoldUnlock) //todo: MainHand Unlocked All
        {
            rndShopItemList.AddRange(GfData.skinList.Where(itemInfo =>
                !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold));
            i = Random.Range(0, rndShopItemList.Count);
            rndShopItemList[i].IsUnlocked = true;
            randomItem = i;
            randomItemValue = $"{rndShopItemList[i].itemName} Unlocked";
        }

        if (GfData.IsAllMainHandGoldUnlock && GfData.IsAllSkinGoldUnlock)
        {
            if (!GfData.IsAllMainHandGoldMaxLevel)
            {
                weapon = true;
                rndShopItemList.AddRange(GfData.mainHandList.Where(itemInfo =>
                    !itemInfo.IsLevelMax && itemInfo.currencyType == CurrencyType.Gold));
                i = Random.Range(0, rndShopItemList.Count);
                rndShopItemList[i].ItemLevel += 1;
                randomItem = i;
                randomItemValue = $"{rndShopItemList[i].itemName} +1 level";
            }
        }
    }

    [Header("REWARD")] [SerializeField] private GameObject rewardPanel;
    [SerializeField] private UI2DSprite rewardIcon;
    [SerializeField] private UILabel rewardText;

    void ShowRewardPanel()
    {
        rewardText.text = "";
        rewardPanel.SetActive(true);
        switch (DailyReward.checkOrder)
        {
            default:
                rewardIcon.sprite2D = dailyRewardIconList[DailyReward.checkOrder];
                rewardIcon.MakePixelPerfect();
                for (int i = 0; i < todayReward.drSlots.Count; i++)
                {
                    var t = todayReward.drSlots[i];
                    if (t == null) continue;
                    //if (i > 0) rewardText.text += " ";
                    rewardText.text += $"+{t.value} {t.drTypes.ToString()} ";
                }

                break;
            case 4:
                rewardIcon.sprite2D = GfData.IsAllTalentMaxLevel
                    ? dailyRewardIconList[randomTalent]
                    : rndTalentList[randomTalent].itemIcon;
                rewardIcon.MakePixelPerfect();
                rewardText.text = randomTalentValue;
                break;
            case 6:
                rewardIcon.sprite2D = fullAll ? dailyRewardIconList[randomItem] : rndShopItemList[randomItem].itemIcon;
                if (!fullAll && weapon)
                {
                    rewardIcon.transform.localScale = new Vector3(2, 2, 2);
                }
                else rewardIcon.transform.localScale = Vector3.one;

                rewardIcon.MakePixelPerfect();
                rewardText.text = randomItemValue;
                break;
        }

        GameEventTrackerProVCL.Instance.OnPlayerGetDayReward(DailyReward.checkOrder);
    }

    public void OnContinueButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        rewardPanel.SetActive(false);
        UI.GetPanel(PanelName.PanelMenu);
    }
}