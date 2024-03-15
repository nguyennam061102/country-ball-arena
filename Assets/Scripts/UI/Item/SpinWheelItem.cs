using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpinWheelItem : MonoBehaviour
{
    private GameFollowData GfData => GameFollowData.Instance;
    private PanelSpinWheel Spin;

    public enum SlotInfo
    {
        Gold,
        Diamond,
        Skin,
        Gun
    }

    public SlotInfo slotInfo;
    public int id;
    public int prizeSeed;
    public Transform prizePosition;
    public int prizeAmount;

    public UI2DSprite prizeIcon;
    public UILabel prizeValue;
    public List<ShopItemInfo> tmpList;

    private void OnEnable()
    {
        tmpList = new List<ShopItemInfo>();
    }

    public void SetItem(int id, int prizeSeed, int prizeAmount, Sprite prizeIcon, PanelSpinWheel panelsw)
    {
        Spin = panelsw;
        this.id = id;
        this.prizeSeed = prizeSeed;
        this.prizeAmount = prizeAmount;

        this.prizeIcon.sprite2D = prizeIcon;
        this.prizeIcon.MakePixelPerfect();
        this.prizeValue.text = $"+{this.prizeAmount}";
    }

    public void TakePrize()
    {
        switch (slotInfo)
        {
            case SlotInfo.Gold:
                GameData.Gold += this.prizeAmount;
                Spin.ShowRewardPanel(prizeIcon.sprite2D, $"+{prizeAmount} Gold");
                break;
            case SlotInfo.Diamond:
                GameData.Diamond += this.prizeAmount;
                Spin.ShowRewardPanel(prizeIcon.sprite2D, $"+{prizeAmount} Diamond");
                break;
            case SlotInfo.Skin:
                TakeRandomSkin();
                break;
            case SlotInfo.Gun:
                TakeRandomGun();
                break;
        }
    }

    void TakeRandomSkin()
    {
        var r = -1;
        if (GfData.IsAllSkinGoldUnlock)
        {
            GameData.Gold += 500;
            Spin.ShowRewardPanel(Spin.prizeList[6].prizeIcon, "+500", true);
        }
        else
        {
            tmpList = GfData.skinList
                .Where(itemInfo => !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold).ToList();
            r = Random.Range(0, tmpList.Count);
            tmpList[r].IsUnlocked = true;
            Spin.ShowRewardPanel(tmpList[r].itemIcon, "Unlocked");
        }
    }

    void TakeRandomGun()
    {
        var r = -1;
        tmpList = new List<ShopItemInfo>();
        if (GfData.IsAllMainHandGoldUnlock)
        {
            if (GfData.IsAllMainHandGoldMaxLevel)
            {
                GameData.Gold += 500;
                Spin.ShowRewardPanel(Spin.prizeList[6].prizeIcon, "+500");
            }
            else
            {
                tmpList.AddRange(GfData.mainHandList.Where(itemInfo =>
                    !itemInfo.IsLevelMax && itemInfo.currencyType == CurrencyType.Gold));
                r = Random.Range(0, tmpList.Count);
                tmpList[r].ItemLevel += 1;
                Spin.ShowRewardPanel(tmpList[r].itemIcon, "+1 level");
            }
        }
        else
        {
            tmpList.AddRange(GfData.mainHandList.Where(itemInfo =>
                !itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold));
            r = Random.Range(0, tmpList.Count);
            tmpList[r].IsUnlocked = true;
            Spin.ShowRewardPanel(tmpList[r].itemIcon, "Unlocked");
        }
    }
}