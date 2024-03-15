using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private PanelCharacter panelChar;
    private ShopItemInfo itemInfo;

    public int itemId, itemLevel;
    [SerializeField] private UI2DSprite icon;
    [SerializeField] private string itemName, content;
    [SerializeField] UILabel lbName, lbLevel;
    public GameObject selectItem;

    public void SetItemInfo(ShopItemInfo itemInfo, PanelCharacter panel, bool canScale)
    {
        panelChar = panel;
        this.itemId = itemInfo.itemId;
        this.itemLevel = itemInfo.ItemLevel;
        this.icon.sprite2D = itemInfo.itemIcon;
        this.icon.MakePixelPerfect();
        if (canScale) this.icon.transform.localScale = Vector3.one * 270f / 350f;
        this.itemName = itemInfo.itemName;
        this.content = itemInfo.value == 0 ? "" : itemInfo.value.ToString();
        lbName.text = itemInfo.itemName;
        lbLevel.text = "Level " + itemInfo.ItemLevel;
        this.itemInfo = itemInfo;
        switch (itemInfo.shopItemType)
        { 
            case ShopItemType.MainHand:
                selectItem.SetActive(itemId == GameData.CurrentMainHandId);
                break;
            case ShopItemType.OffHand:
                selectItem.SetActive(itemId == GameData.CurrentOffHandId);
                break;
            case ShopItemType.Skin:
                selectItem.SetActive(itemId == GameData.CurrentSkinId);
                break;
            default:
                break;
        }      
    }

    public void OnSelectButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        //show stat
        panelChar.panelInventoryData.gameObject.SetActive(true);
        panelChar.panelInventoryData.SetItemInfo(this.itemInfo, this);
    }

    public void CheckItemAgain()
    {
        lbLevel.text = "Level " + itemInfo.ItemLevel;
        switch (itemInfo.shopItemType)
        {
            case ShopItemType.MainHand:
                selectItem.SetActive(itemId == GameData.CurrentMainHandId);
                break;
            case ShopItemType.OffHand:
                selectItem.SetActive(itemId == GameData.CurrentOffHandId);
                break;
            case ShopItemType.Skin:
                selectItem.SetActive(itemId == GameData.CurrentSkinId);
                break;
            default:
                break;
        }
    }
}
