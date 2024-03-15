using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyMission : DontDestroy<DailyMission>
{
    public List<DailyMissionInfo> dailyMissionList;
    [HideInInspector] public List<DailyMissionInfo> myTodayMissions; //todo: Use For Code

    private string[] dailyMissionId;
    private List<int> tmpList;

    public void FakeSetDailyMission()
    {
        RenewDailyMission();
        GetTodayMissions();
    }

    public void RenewDailyMission()
    {
        dailyMissionId = new string[6];
        tmpList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 6; i++)
        {
            var rnd = Random.Range(0, tmpList.Count);
            dailyMissionId[i] = tmpList[rnd].ToString();
            tmpList.Remove(tmpList[rnd]);
        }
        DailyMissionId = string.Join("-", dailyMissionId);
    }

    public void GetTodayMissions()
    {
        //Debug.Log(DailyMissionId);
        myTodayMissions = new List<DailyMissionInfo>();
        dailyMissionId = DailyMissionId.Split('-');
        for (int i = 0; i < 6; i++)
        {
            myTodayMissions.Add(dailyMissionList[int.Parse(dailyMissionId[i])]);
        }
        //Debug.Log(myTodayMissions.Count);
    }

    public string DailyMissionId
    {
        get => PlayerPrefs.GetString("DailyMissionId", "0-1-2-3");
        set => PlayerPrefs.SetString("DailyMissionId", value);
    }
}

[Serializable]
public class DailyMissionInfo
{
    public DailyMissionType type;
    public Sprite missionIcon;
    public int minValue, maxValue, baseGoldReward;

    public int TargetProgress
    {
        get
        {
            if (!HasTargetProgress)
            {
                HasTargetProgress = true;
                PlayerPrefs.SetInt($"{type}_TargetProgress", Random.Range(minValue, maxValue));
            }
            return PlayerPrefs.GetInt($"{type}_TargetProgress");
        }
    }

    public bool HasTargetProgress
    {
        get => PlayerPrefs.GetInt($"{type}_HasTargetProgress", 0) == 1;
        set => PlayerPrefs.SetInt($"{type}_HasTargetProgress", value ? 1 : 0);
    }

    public int MissionProgress
    {
        get => PlayerPrefs.GetInt($"{type}_Progress", 0);
        set => PlayerPrefs.SetInt($"{type}_Progress", value);
    }

    public int MissionReward => TargetProgress * baseGoldReward;

    public bool MissionDone => MissionProgress >= TargetProgress;

    public bool MissionClaimed
    {
        get => PlayerPrefs.GetInt($"{type}_Claimed", 0) == 1;
        set => PlayerPrefs.SetInt($"{type}_Claimed", value ? 1 : 0);
    }

    public override string ToString()
    {
        switch (type)
        {
            default:
                return "Skygo Game 2021";
            case DailyMissionType.KillEnemy:
                return $"Kill {TargetProgress} enemies";
            case DailyMissionType.PickCard:
                return $"Pick {TargetProgress} cards";
            case DailyMissionType.WinMatch:
                return $"Win {TargetProgress} matches";
            case DailyMissionType.WinRound:
                return $"Win {TargetProgress} rounds";
            case DailyMissionType.WatchRewardedVideo:
                return $"Watch rewarded video {TargetProgress} times";
            case DailyMissionType.ShootBullet:
                return $"Shoot {TargetProgress} bullets";
            case DailyMissionType.DealDamage:
                return $"Deal {TargetProgress} damage";
            case DailyMissionType.UpgradeWeapon:
                return $"Upgrade weapon {TargetProgress} times";
            case DailyMissionType.PurchaseGoldOrDiamond:
                return $"Purchase Gold or Diamond pack {TargetProgress} times";
            case DailyMissionType.TakeDamage:
                return $"Take {TargetProgress} damage";
        }
    }

    public void Reset()
    {
        HasTargetProgress = false;
        MissionProgress = 0;
        MissionClaimed = false;
    }
}

public enum DailyMissionType
{
    KillEnemy,
    PickCard,
    WinMatch,
    WinRound,
    WatchRewardedVideo,
    ShootBullet,
    DealDamage,
    UpgradeWeapon,
    PurchaseGoldOrDiamond,
    TakeDamage
}
