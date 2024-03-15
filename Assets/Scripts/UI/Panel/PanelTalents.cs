using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PanelTalents : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    private TalentItemInfo[] tDataList => GameFollowData.Instance.talentItemList;
    [SerializeField] private TalentItem talentItemPrefab;
    [SerializeField] private List<TalentItem> tList;
    [SerializeField] private UIGrid gridParent;
    [SerializeField] private Transform talentSelect;
    [SerializeField] private GameObject upgradeButton, watchButton;
    [SerializeField] private UILabel priceToUpgradeText, upgradeLeft;
    //[SerializeField] private GameObject fxOn, fxOff;
    [SerializeField] UI2DSprite sprButton;
    private TalentItemInfo currentItem;
    private int priceToUpgrade
    {
        get
        {
            return 1 + (int)(GameData.TalentUpgradedCount / 3f);
        }
    }

    private void OnEnable()
    {
        ShowTalents();
        CheckButton();
        GameData.OnTalentUpgradeCountChanged += CheckButton;
    }

    void ShowTalents()
    {
        tList = new List<TalentItem>();
        for (int j = 0; j < 10; j++)
        {
            var talentItem = Instantiate(talentItemPrefab, gridParent.transform);
            talentItem.SetItem(tDataList[j]);
            tList.Add(talentItem);
        }
        gridParent.Reposition();
        SetPriceToUpgrade();
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        //var panelCharacter = UI.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>();
        //panelCharacter.SetTitleType(panelCharacter.IsPanelCharacter ? TitleType.Character : TitleType.LoadOut);
        UI.GetPanel(PanelName.PanelMenu);
    }

    private void SetPriceToUpgrade()
    {
        priceToUpgradeText.text = priceToUpgrade.ToString("0");
    }

    private int i, value, previousValue;
    public int t = 20;
    private bool isUpgrading;

    public void OnUpgradeButton()
    {
        if (isUpgrading) return;
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.FreeUpgradeTalent > 0)
        {
            if (GameData.Diamond >= priceToUpgrade)
            {
                GameData.Diamond -= priceToUpgrade;
                StartCoroutine(UpgradeRandomTalent());
                GameData.TalentUpgradedCount++;
                SetPriceToUpgrade();
            }
            else CoinUi.Instance.Alert(false);
        }
    }

    public void OnWatchFreeUpgradeButton()
    {
        if (isUpgrading) return;
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            StartCoroutine(UpgradeRandomTalent());
            GameData.TalentUpgradedCount++;
            SetPriceToUpgrade();
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    IEnumerator UpgradeRandomTalent()
    {
        GameData.FreeUpgradeTalent--;
        GameEventTrackerProVCL.Instance.OnPlayerUpgradeTalent();
        i = t;
        talentSelect.gameObject.SetActive(true);
        isUpgrading = true;
        while (i > 0)
        {
            value = Random.Range(0, tList.Count);
            while (value == previousValue || tList[value].itemInfo.IsLevelMax)
            {
                value = Random.Range(0, tList.Count);
            }
            previousValue = value;
            talentSelect.localPosition = new Vector3(tList[value].transform.localPosition.x, tList[value].transform.localPosition.y + gridParent.transform.localPosition.y);
            i--;
            yield return new WaitForSeconds(0.1f);
        }

        tList[value].UpgradeLevel();
        Sound.Play(Sound.SoundData.Upgrade);
        talentSelect.GetComponent<UITweener>().ResetToBeginning();
        talentSelect.GetComponent<UITweener>().PlayForward();
        yield return new WaitForSeconds(2f);
        talentSelect.gameObject.SetActive(false);
        isUpgrading = false;
    }

    void CheckButton()
    {
        //bool watch = GameData.TalentUpgradedCount > 0 && GameData.TalentUpgradedCount % 10 == 0 && GameData.FreeUpgradeTalent > 0;
        //watchButton.SetActive(watch);
        //upgradeButton.SetActive(!watch);
        //if (!watch)
        //{
        //    sprButton.color = GameData.FreeUpgradeTalent > 0 ? new Color32(0, 150, 220, 255) : new Color32(128, 132, 134, 255);
        //}
        sprButton.color = GameData.FreeUpgradeTalent > 0 ? new Color32(0, 150, 220, 255) : new Color32(128, 132, 134, 255);
        //upgradeButton.SetActive(true);
        upgradeLeft.text = GameData.FreeUpgradeTalent > 0 ? $"Upgrades Left: {GameData.FreeUpgradeTalent}" : $"Reach level {GameData.PlayerLevel + 1} to get upgrades";
    }
}
