using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelInventory : MonoBehaviour
{
    private GameFollowData gfData => GameFollowData.Instance;
    private CharacterStats Stats => CharacterStats.Instance;

    public enum CharacterInfo
    {
        MainHand,
        OffHand,
        Skin
    }

    public CharacterInfo characterInfo;

    [Header("===Item Info===")]
    [SerializeField] private int itemId;
    [SerializeField] private int itemLevel;
    [SerializeField] private float itemPrice;
    [SerializeField] private UI2DSprite icon, eye, curentEye;
    [SerializeField] private GameObject changeEye, desc;
    [SerializeField] private UILabel itemName, itemLevelText, damageText, magSizeText, lbItemDesc;
    private ShopItemInfo sItemInfo;
    private InventoryItem inventoryItem;

    [Space]
    [SerializeField] private UI2DSprite equipButton;
    [SerializeField] private UI2DSprite upgradeButton;

    [SerializeField] TweenScale twScale;
    [SerializeField] TweenAlpha twAlpha;

    [SerializeField] PanelCharacter pchar;
    public int count;

    private void OnEnable()
    {
        twScale.ResetToBeginning();
        twScale.PlayForward();
        twAlpha.ResetToBeginning();
        twAlpha.PlayForward();
        
    }

    public void SetItemInfo(ShopItemInfo itemInfo, InventoryItem inventoryItem)
    {
        this.itemId = itemInfo.itemId;
        this.icon.sprite2D = itemInfo.itemIcon;
        this.icon.MakePixelPerfect();
        this.itemName.text = itemInfo.itemName;
        this.itemLevelText.text = $"Level: {itemInfo.ItemLevel}";
        lbItemDesc.text = itemInfo.itemShortDesc;

        if (itemInfo.shopItemType == ShopItemType.MainHand) characterInfo = CharacterInfo.MainHand;
        else if (itemInfo.shopItemType == ShopItemType.OffHand) characterInfo = CharacterInfo.OffHand;
        else if (itemInfo.shopItemType == ShopItemType.Skin) characterInfo = CharacterInfo.Skin;

        switch (characterInfo)
        {
            case CharacterInfo.MainHand:
                this.changeEye.SetActive(false);
                this.desc.SetActive(true);
                this.eye.gameObject.SetActive(false);
                if (itemInfo.damage > 0)
                {
                    int upDMG = (int)(itemInfo.damage * (Stats.GunLevelDamageMultiplier(itemInfo.ItemLevel + 1) - Stats.GunLevelDamageMultiplier(itemInfo.ItemLevel)));
                    this.damageText.text = itemInfo.numPerShot > 1
                        ? $"[514685]DMG: {(int)(itemInfo.damage * Stats.GunLevelDamageMultiplier(itemInfo.ItemLevel))}[-][12FF00](+{upDMG})[-][514685]x{itemInfo.numPerShot}[-]"
                        : $"[514685]DMG: {(int)(itemInfo.damage * Stats.GunLevelDamageMultiplier(itemInfo.ItemLevel))}[-][12FF00](+{upDMG})[-]";
                }
                else this.damageText.text = "";
                this.magSizeText.text = "[514685]ROF: " + itemInfo.rof + " sec";
                break;
            case CharacterInfo.OffHand:
                this.changeEye.SetActive(false);
                this.desc.SetActive(true);
                this.eye.gameObject.SetActive(false);
                this.damageText.text = "[514685]" + itemInfo.function;
                this.magSizeText.text = $"[514685]CD: {itemInfo.coolDown} sec";
                break;
            case CharacterInfo.Skin:
                int upHP = (int)(itemInfo.health * (Stats.SkinLevelHPMultiplier(itemInfo.ItemLevel + 1) - Stats.SkinLevelHPMultiplier(itemInfo.ItemLevel)));
                this.damageText.text = $"[514685]HP: {(int)(itemInfo.health * Stats.SkinLevelHPMultiplier(itemInfo.ItemLevel))}[-][12FF00](+{upHP})[-]";
                this.magSizeText.text = $"[514685]SPD: {itemInfo.moveSpeed}";
                this.icon.width = Mathf.RoundToInt(this.icon.width * 2f);
                this.icon.height = Mathf.RoundToInt(this.icon.height * 2f);
                this.changeEye.SetActive(true);
                this.desc.SetActive(false);
                this.eye.gameObject.SetActive(true);
                this.eye.sprite2D = itemInfo.connectedSkin.eye;
                this.eye.MakePixelPerfect();
                this.curentEye.sprite2D = itemInfo.connectedSkin.eye;
                this.curentEye.MakePixelPerfect();
                count = GameFollowData.Instance.eyes.IndexOf(itemInfo.connectedSkin.eye);
                break;
        }

        this.itemLevel = itemInfo.ItemLevel;
        this.itemPrice = itemInfo.price;
        this.sItemInfo = itemInfo;
        this.inventoryItem = inventoryItem;

        if (itemInfo.IsLevelMax || !itemInfo.canUpgrade)
        {
            upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            upgradeButton.gameObject.SetActive(true);
            upgradeButton.GetComponentInChildren<UILabel>().text = $"{PriceToUpgrade()}";
        }

        switch (characterInfo)
        {
            case CharacterInfo.MainHand:
                equipButton.GetComponentInChildren<UILabel>().text = this.itemId == GameData.CurrentMainHandId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentMainHandId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            case CharacterInfo.OffHand:
                equipButton.GetComponentInChildren<UILabel>().text = this.itemId == GameData.CurrentOffHandId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentOffHandId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            case CharacterInfo.Skin:
                equipButton.GetComponentInChildren<UILabel>().text = this.itemId == GameData.CurrentSkinId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentSkinId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void OnChageEyeLeft()
    {
        if (this.count + 1 < GameFollowData.Instance.eyes.Count)
        {
            this.count += 1;
            GameFollowData.Instance.skinList[itemId].connectedSkin.eye = GameFollowData.Instance.eyes[count];
            this.eye.sprite2D = GameFollowData.Instance.eyes[count];
            this.eye.MakePixelPerfect();
            this.curentEye.sprite2D = GameFollowData.Instance.eyes[count];
            this.curentEye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.sprite2D = GameFollowData.Instance.eyes[count];
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().SetCharacterAnim();
        }
        else
        {
            this.count = 0;
            GameFollowData.Instance.skinList[itemId].connectedSkin.eye = GameFollowData.Instance.eyes[count];
            this.eye.sprite2D = GameFollowData.Instance.eyes[count];
            this.eye.MakePixelPerfect();
            this.curentEye.sprite2D = GameFollowData.Instance.eyes[count];
            this.curentEye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.sprite2D = GameFollowData.Instance.eyes[count];
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().SetCharacterAnim();
        }
    }
    public void OnChageEyeRight()
    {
        if (this.count > 0)
        {
            this.count -= 1;
            GameFollowData.Instance.skinList[itemId].connectedSkin.eye = GameFollowData.Instance.eyes[count];
            this.eye.sprite2D = GameFollowData.Instance.eyes[count];
            this.eye.MakePixelPerfect();
            this.curentEye.sprite2D = GameFollowData.Instance.eyes[count];
            this.curentEye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.sprite2D = GameFollowData.Instance.eyes[count];
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().SetCharacterAnim();
        }
        else
        {
            this.count = GameFollowData.Instance.eyes.Count - 1;
            GameFollowData.Instance.skinList[itemId].connectedSkin.eye = GameFollowData.Instance.eyes[count];
            this.eye.sprite2D = GameFollowData.Instance.eyes[count];
            this.eye.MakePixelPerfect();
            this.curentEye.sprite2D = GameFollowData.Instance.eyes[count];
            this.curentEye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.sprite2D = GameFollowData.Instance.eyes[count];
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.MakePixelPerfect();
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.width * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height = Mathf.RoundToInt(UiController.Instance.currentPanel.GetComponent<PanelCharacter>().ItemList[itemId].Eye.height * 0.5f);
            UiController.Instance.currentPanel.GetComponent<PanelCharacter>().SetCharacterAnim();
        }
    }
    public void OnEquipButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);

        switch (characterInfo)
        {
            case CharacterInfo.MainHand:
                GameData.CurrentMainHandId = itemId;
                equipButton.GetComponentInChildren<UILabel>().text = itemId == GameData.CurrentMainHandId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentMainHandId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            case CharacterInfo.OffHand:
                GameData.CurrentOffHandId = itemId;
                equipButton.GetComponentInChildren<UILabel>().text = itemId == GameData.CurrentOffHandId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentOffHandId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            case CharacterInfo.Skin:
                GameData.CurrentSkinId = itemId;
                equipButton.GetComponentInChildren<UILabel>().text = itemId == GameData.CurrentSkinId ? "Equipped" : "Equip";
                equipButton.GetComponent<UI2DSprite>().color = this.itemId == GameData.CurrentSkinId ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        pchar.ReshowAfterUpgradeOrSelect();
    }

    public void OnUpgradeButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.Gold >= PriceToUpgrade())
        {
            Sound.Play(Sound.SoundData.Upgrade);
            GameData.Gold -= (int)PriceToUpgrade();
            sItemInfo.ItemLevel++;
            GameEventTrackerProVCL.Instance.OnPlayerUpgrade(sItemInfo.itemName);
            SetItemInfo(sItemInfo, inventoryItem);
            DailyMission.Instance.dailyMissionList[7].MissionProgress++;
        }
        else
        {
            CoinUi.Instance.Alert();
        }
        pchar.ReshowAfterUpgradeOrSelect();
    }

    private float PriceToUpgrade()
    {
        return Mathf.Round(sItemInfo.basePriceToUpgrade * Mathf.Pow(1.08f, itemLevel));
    }

    public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }
}