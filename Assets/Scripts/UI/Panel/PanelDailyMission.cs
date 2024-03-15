using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelDailyMission : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    private DailyMission Mission => DailyMission.Instance;
    //private string[] tmpString;
    
    [SerializeField] private DailyMissionItem prefab;
    [SerializeField] private UIGrid parent;
    [SerializeField] private GameObject watchRenewButton;

    private void Start()
    {
        GetItem();
    }

    private void GetItem()
    {
        int claimCount = 0;
        foreach (var info in Mission.myTodayMissions)
        {
            var item = Instantiate(prefab, parent.transform);
            item.SetItem(info);
            if (info.MissionClaimed) claimCount++;
        }
        watchRenewButton.SetActive(claimCount >= Mission.myTodayMissions.Count);
    }

    public void OnWatchRenewButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            Mission.RenewDailyMission();
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("dailymissionrenew");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }
    
    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelMenu);
    }
}
