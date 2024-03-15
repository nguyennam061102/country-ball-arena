using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopItem : MonoBehaviour
{
    private PanelShop panelShop;

    [SerializeField] private int itemId, itemLevel;
    [SerializeField] private UI2DSprite icon, iconCurrency, sprButton;
    [SerializeField] private UILabel itemName, content, price;
    [SerializeField] private SkeletonAnimation anim;
    [SerializeField] private Sprite iconGold, iconDiamond;
    private ShopItemInfo itemInfo;

    //[SerializeField] private ParticleSystem fxNormal, fxOwned, fxGold, fxDiamond;

    public void SetItemInfo(ShopItemInfo itemInfo, PanelShop pShop)
    {
        panelShop = pShop;
        this.itemId = itemInfo.itemId;
        this.itemLevel = itemInfo.ItemLevel;
        if (!itemInfo.useAnim)
        {
            this.icon.sprite2D = itemInfo.itemIcon;
            this.icon.MakePixelPerfect();
            if (panelShop.shopInfo == PanelShop.ShopInfo.MainHand)
            {
                this.icon.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
                this.icon.GetComponent<UIWidget>().width = 320;
            }
        }
        this.itemName.text = itemInfo.itemName;
        this.content.text = itemInfo.value == 0 ? itemInfo.itemShortDesc : "+" + itemInfo.value.ToString();
        this.price.text = !itemInfo.IsUnlocked ? itemInfo.currency ? $"${itemInfo.price}" : itemInfo.price.ToString() : "Owned";
        //simple change
        //if (!itemInfo.IsUnlocked)
        //{
        //    if (itemInfo.currency)
        //    {
        //        this.price.text = $"${itemInfo.price}";
        //    }
        //    else
        //    {
        //        this.price.text = itemInfo.price.ToString();
        //    }
        //}
        //else
        //{
        //    this.price.text = "Owned";
        //}

        this.iconCurrency.sprite2D = itemInfo.currencyType == CurrencyType.Gold ? iconGold : iconDiamond;

        if (itemInfo.IsUnlocked || itemInfo.currency)
        {
            iconCurrency.gameObject.SetActive(false);
            this.price.transform.localPosition = Vector3.zero;
            if (panelShop.shopInfo == PanelShop.ShopInfo.Gold) sprButton.color = new Color32(240, 154, 41, 255);
            if (panelShop.shopInfo == PanelShop.ShopInfo.Diamond) sprButton.color = new Color32(240, 154, 41, 255);
        }

        this.itemInfo = itemInfo;
        //lockGo.SetActive(!isUnlocked);
        sprButton.color = itemInfo.IsUnlocked ? new Color32(240, 154, 41, 255) : new Color32(50, 184, 81, 255);

        //if ((bool)fxNormal) fxNormal.gameObject.SetActive(!itemInfo.IsUnlocked);
        //if ((bool)fxOwned) fxOwned.gameObject.SetActive(itemInfo.IsUnlocked);
    }

    private void Start()
    {
        if (!itemInfo.useAnim) return;
        anim.skeleton.SetSkin(itemId.ToString());
        anim.skeleton.SetSlotsToSetupPose();
        anim.AnimationState.Apply(anim.Skeleton);

        var tmp = Random.Range(0, 3);
        switch (tmp)
        {
            case 0:
                anim.state.SetAnimation(0, "idle 1", true);
                anim.timeScale = Random.Range(0.8f, 1.2f);
                break;
            case 1:
                anim.state.SetAnimation(0, "idle 2", true);
                anim.timeScale = Random.Range(0.8f, 1.2f);
                break;
            case 2:
                anim.state.SetAnimation(0, "idle 3", true);
                anim.timeScale = Random.Range(0.8f, 1.2f);
                break;
        }
    }

    public void Unlock()
    {
        //this.price.text = "Owned";
        SetItemInfo(itemInfo, panelShop);
    }

    public void OnSelectButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (itemInfo.IsUnlocked) return;
        panelShop.ShowConfirmUnlock(itemInfo, this);
    }
}