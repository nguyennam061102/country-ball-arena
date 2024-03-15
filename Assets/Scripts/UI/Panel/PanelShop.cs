using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelShop : MonoBehaviour
{
    private GameFollowData gfData => GameFollowData.Instance;
    private UiController UI => UiController.Instance;

    [SerializeField] private Transform parent;

    public enum ShopInfo
    {
        MainHand,
        OffHand,
        Skin,
        Diamond,
        Gold
    }

    public ShopInfo shopInfo;

    [Header("===Shop System===")]
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private List<ShopItem> shopItemList;
    [Space(20)]
    [SerializeField] private UIButton[] shopButtons;
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private UIScrollView uiScrollView;
    [SerializeField] private UIGrid grid, staticGrid;

    private void OnEnable()
    {
        GameData.ShopWarning = false;
        ShowShop(ShopInfo.MainHand);
        SetButtonSprite(0);
    }

    public void OnMainHandButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        ShowShop(ShopInfo.MainHand);
        SetButtonSprite(0);
    }

    public void OnOffHandButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        ShowShop(ShopInfo.OffHand);
        SetButtonSprite(1);
    }

    public void OnSkinButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        ShowShop(ShopInfo.Skin);
        SetButtonSprite(2);
    }

    public void OnDiamondButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        ShowShop(ShopInfo.Diamond);
        SetButtonSprite(3);
    }

    public void OnGoldButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        ShowShop(ShopInfo.Gold);
        SetButtonSprite(4);
    }

    void SetButtonSprite(int i)
    {
        for (int j = 0; j < shopButtons.Length; j++)
        {
            shopButtons[j].transform.GetChild(0).GetComponent<UILabel>().effectStyle = i == j ? UILabel.Effect.Shape3D : UILabel.Effect.None;
            shopButtons[j].transform.GetChild(0).GetComponent<UILabel>().effectDistance = new Vector2(1f, 5f);
            shopButtons[j].normalSprite2D = i == j ? @on : off;
            shopButtons[j].GetComponent<UI2DSprite>().MakePixelPerfect();
        }
    }

    private void ShowShop(ShopInfo shopInfo)
    {
        if (shopItemList != null)
        {
            foreach (ShopItem si in shopItemList) Destroy(si.gameObject);
        }
        shopItemList = new List<ShopItem>();
        this.shopInfo = shopInfo;
        switch (shopInfo)
        {
            case ShopInfo.MainHand:
                foreach (var itemInfo in gfData.mainHandList)
                {
                    var shopItem = Instantiate(shopItemPrefab, grid.transform);
                    shopItem.SetItemInfo(itemInfo, this);
                    shopItemList.Add(shopItem);
                    //shopItem.GetComponent<UITweener>().delay = tmp;
                    //shopItem.GetComponent<UITweener>().PlayForward();
                }

                break;
            case ShopInfo.OffHand:
                foreach (var itemInfo in gfData.offHandList)
                {
                    var shopItem = Instantiate(shopItemPrefab, staticGrid.transform);
                    shopItem.SetItemInfo(itemInfo, this);
                    shopItemList.Add(shopItem);
                    //shopItem.GetComponent<UITweener>().delay = tmp;
                    //shopItem.GetComponent<UITweener>().PlayForward();
                }

                break;
            case ShopInfo.Skin:
                foreach (var itemInfo in gfData.skinList)
                {
                    var shopItem = Instantiate(shopItemPrefab, grid.transform);
                    shopItem.SetItemInfo(itemInfo, this);
                    shopItemList.Add(shopItem);
                    //shopItem.GetComponent<UITweener>().delay = tmp;
                    //shopItem.GetComponent<UITweener>().PlayForward();
                }

                break;
            case ShopInfo.Diamond:
                foreach (var itemInfo in gfData.diamondList)
                {
                    var shopItem = Instantiate(shopItemPrefab, staticGrid.transform);
                    shopItem.SetItemInfo(itemInfo, this);
                    shopItemList.Add(shopItem);
                    //shopItem.GetComponent<UITweener>().delay = tmp;
                    //shopItem.GetComponent<UITweener>().PlayForward();
                }

                break;
            case ShopInfo.Gold:
                foreach (var itemInfo in gfData.goldList)
                {
                    var shopItem = Instantiate(shopItemPrefab, staticGrid.transform);
                    shopItem.SetItemInfo(itemInfo, this);
                    shopItemList.Add(shopItem);
                    //shopItem.GetComponent<UITweener>().delay = tmp;
                    //shopItem.GetComponent<UITweener>().PlayForward();
                }
                break;
        }
        Invoke("GridAndScrollReposition", 0.05f);
    }

    void GridAndScrollReposition()
    {
        uiScrollView.ResetPosition();
        grid.Reposition();
        grid.GetComponent<TweenScale>().ResetToBeginning();
        grid.GetComponent<TweenScale>().PlayForward();
        staticGrid.Reposition();
        staticGrid.GetComponent<TweenScale>().ResetToBeginning();
        staticGrid.GetComponent<TweenScale>().PlayForward();
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (UI.lastPanelName.Equals(PanelName.PanelInventory))
        {
            //switch (UI.panelInventoryType)
            //{
            //    case 0:
            //        UI.GetPanel(PanelName.PanelInventory).GetComponent<PanelInventory>().ShowInventory(PanelInventory.CharacterInfo.MainHand);
            //        break;
            //    case 1:
            //        UI.GetPanel(PanelName.PanelInventory).GetComponent<PanelInventory>().ShowInventory(PanelInventory.CharacterInfo.OffHand);
            //        break;
            //    case 2:
            //        UI.GetPanel(PanelName.PanelInventory).GetComponent<PanelInventory>().ShowInventory(PanelInventory.CharacterInfo.Skin);
            //        break;
            //    default:
            //        break;
            //}
        }
        else if (UI.lastPanelName.Equals(PanelName.PanelCharacter))
        {
            switch (UI.panelCharacterTitle)
            {
                case 0:
                    UI.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.Character);
                    break;
                case 1:
                    UI.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.LoadOut);
                    break;
            }
        }
        else if (UI.lastPanelName.Equals(PanelName.PanelShop)
            || UI.lastPanelName.Equals(PanelName.PanelReady)
            || UI.lastPanelName.Equals(PanelName.PanelSelectCard)
            || UI.lastPanelName.Equals(PanelName.PanelPause)
            || UI.lastPanelName.Equals(PanelName.PanelEndgame)
            || UI.lastPanelName.Equals(PanelName.PanelContinue))
        {
            UI.GetPanel(PanelName.PanelMenu);
        }
        else
        {
            UI.GetPanel(UI.lastPanelName);
        }
    }

    public void ShowConfirmUnlock(ShopItemInfo shopItemInfo, ShopItem shopItem)
    {
        if (this.shopInfo == ShopInfo.Diamond)
        {
            var onDone = new UnityEvent();
            onDone.AddListener(() =>
            {
                GameData.Diamond += (int)shopItemInfo.value;
                DailyMission.Instance.dailyMissionList[8].MissionProgress++;
                Sound.Play(Sound.SoundData.GoldPurchase);
            });
            onDone.Invoke();
            //purchase
            //SkygoBridge.instance.PurchaseIAP(shopItemInfo.sku, onDone);
            return;
        }
        if (this.shopInfo == ShopInfo.Gold)
        {
            var onDone = new UnityEvent();
            onDone.AddListener(() =>
            {
                GameData.Gold += (int)shopItemInfo.value;
                DailyMission.Instance.dailyMissionList[8].MissionProgress++;
                Sound.Play(Sound.SoundData.DiamondPurchase);
            });
            onDone.Invoke();
            //purchase
            //SkygoBridge.instance.PurchaseIAP(shopItemInfo.sku, onDone);
            return;
        }

        if (CheckEnoughCoin(shopItemInfo))
        {
            if (shopItemInfo.currencyType == CurrencyType.Gold) GameData.Gold -= (int)shopItemInfo.price;
            else GameData.Diamond -= (int)shopItemInfo.price;
            gfData.UnlockItem(shopItemInfo);
            shopItem.Unlock();

            switch (shopInfo)
            {
                case ShopInfo.MainHand:
                    GameEventTrackerProVCL.Instance.OnPlayerPurchase(0);
                    break;
                case ShopInfo.OffHand:
                    GameEventTrackerProVCL.Instance.OnPlayerPurchase(1);
                    break;
                case ShopInfo.Skin:
                    GameEventTrackerProVCL.Instance.OnPlayerPurchase(2);
                    break;
            }
        }
    }

    public bool CheckEnoughCoin(ShopItemInfo itemInfo)
    {
        if (itemInfo.currencyType == CurrencyType.Gold)
        {
            if (GameData.Gold < itemInfo.price)
            {
                CoinUi.Instance.Alert();
                OnGoldButton();
            }
            return GameData.Gold >= itemInfo.price;
        }
        if (GameData.Diamond < itemInfo.price)
        {
            CoinUi.Instance.Alert(false);
            OnDiamondButton();
        }
        return GameData.Diamond >= itemInfo.price;
    }
}