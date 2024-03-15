using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : SingletonMonoBehavior<MenuController>
{
    [HideInInspector] public bool logoPlayed, showCharacterPanel;
    public DailyReward dailyReward;

    private void Start()
    {
        
        Loading.Instance.ShowBGLoad(false);
        //SkygoBridge.instance.SetCrosspromotionPosition(1);
        CoinUi.Instance.currencyParent.SetActive(true);
        CoinUi.Instance.UpdateGoldAndText();
        if (dailyReward.CheckDaily())
        {
            //todo: Renew Daily Mission
            foreach (var item in DailyMission.Instance.dailyMissionList)
            {
                item.Reset();
            }
            DailyMission.Instance.RenewDailyMission();
            DailyMission.Instance.GetTodayMissions();
        }
        else
        {
            DailyMission.Instance.GetTodayMissions();
            if (GameFollowData.Instance.IsChangeSceneFromGameplay)
            {
                GameFollowData.Instance.IsChangeSceneFromGameplay = false;
                switch (GameFollowData.Instance.playingGameMode)
                {
                    case GameMode.Survival:
                        //UiController.Instance.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.LoadOut);
                        UiController.Instance.GetPanel(PanelName.PanelMenu);
                        break;
                    case GameMode.DeathMatch:
                        UiController.Instance.GetPanel(PanelName.PanelDeathMatchStart);
                        break;
                    case GameMode.SandBox:
                        UiController.Instance.GetPanel(PanelName.PanelMenu);
                        break;
                }
            }
            else if (GameFollowData.Instance.IsToUpgradeCharacter)
            {
                GameFollowData.Instance.IsToUpgradeCharacter = false;
                UiController.Instance.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.Character);
            }
            else UiController.Instance.GetPanel(PanelName.PanelMenu);         
        }

    }
}
