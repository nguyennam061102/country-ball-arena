using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    /// <summary>
    /// 0: chưa xong
    /// 1: đã xong
    /// </summary>
    public static int TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0);
        set
        {
            PlayerPrefs.SetInt("TutorialCompleted", value);
        }
    }

    public static bool ExplainCardStack
    {
        get => PlayerPrefs.GetInt("ExplainCardStack", 0) == 1;
        set => PlayerPrefs.SetInt("ExplainCardStack", value ? 1 : 0);
    }

    public static bool ExplainMenu
    {
        get => PlayerPrefs.GetInt("ExplainMenu", 0) == 1;
        set => PlayerPrefs.SetInt("ExplainMenu", value ? 1 : 0);
    }

    public static int ExplainSandbox
    {
        get => PlayerPrefs.GetInt("ExplainSandbox", 0);
        set => PlayerPrefs.SetInt("ExplainSandbox", value);
    }

    public static int Diamond
    {
        get => PlayerPrefs.GetInt("Diamond", 5);
        set
        {
            OnDiamondChanged?.Invoke(value, Diamond < value);
            PlayerPrefs.SetInt("Diamond", value);
        }
    }

    public static Action<int, bool> OnDiamondChanged;

    public static int Gold
    {
        get => PlayerPrefs.GetInt("Gold", 1000);
        set
        {
            OnGoldChanged?.Invoke(value, Gold < value);
            PlayerPrefs.SetInt("Gold", value);
        }
    }

    public static Action<int, bool> OnGoldChanged;
    
    public static bool Sound
    {
        get => PlayerPrefs.GetInt("Sound", 1) == 1;
        set => PlayerPrefs.SetInt("Sound", value ? 1 : 0);
    }
    
    public static bool Music
    {
        get => PlayerPrefs.GetInt("Music", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);
            onMusicChanged?.Invoke();
        }
    }

    public static int GraphicsLevel
    {
        get => PlayerPrefs.GetInt("GraphicsLevel", 0);
        set => PlayerPrefs.SetInt("GraphicsLevel", value);
    }

    public static Action onMusicChanged;
    
    public static bool RateType
    {
        get => PlayerPrefs.GetInt("RateType", 0) == 1;
        set => PlayerPrefs.SetInt("RateType", value ? 1 : 0);
    }
    
    public static bool ControlLeft
    {
        get => PlayerPrefs.GetInt("ControlLeft", 1) == 1;
        set => PlayerPrefs.SetInt("ControlLeft", value ? 1 : 0);
    }
    
    public static bool NewDayOpen
    {
        get => PlayerPrefs.GetInt("NewDayOpen", 1) == 1;
        set => PlayerPrefs.SetInt("NewDayOpen", value ? 1 : 0);
    }
    
    public static bool FreeSpin
    {
        get => PlayerPrefs.GetInt("FreeSpin", 1) == 1;
        set => PlayerPrefs.SetInt("FreeSpin", value ? 1 : 0);
    }

    public static string DailyReward
    {
        get => PlayerPrefs.GetString("DailyReward", "2017:12:20:-1");
        set => PlayerPrefs.SetString("DailyReward", value);
    }

    public static int FreeUpgradeTalent
    {
        get => PlayerPrefs.GetInt("TalentFreeUpgrade", 3);
        set => PlayerPrefs.SetInt("TalentFreeUpgrade", value);
    }

    public static int TalentUpgradedCount
    {
        get => PlayerPrefs.GetInt("TalentUpgradedCount", 0);
        set
        {
            PlayerPrefs.SetInt("TalentUpgradedCount", value);
            OnTalentUpgradeCountChanged?.Invoke();
        }
    }
    

    public static Action OnTalentUpgradeCountChanged;

    #region CHARACTER_INFO

    public static int PlayerLevel
    {
        get => PlayerPrefs.GetInt("PlayerLevel", 1);
        set => PlayerPrefs.SetInt("PlayerLevel", value);
    }

    public static int RoundLevel
    {
        get => PlayerPrefs.GetInt("RoundLevel", 1);
        set => PlayerPrefs.SetInt("RoundLevel", value);
    }

    public static int MatchLevel
    {
        get => PlayerPrefs.GetInt("MatchLevel", 1);
        set => PlayerPrefs.SetInt("MatchLevel", value);
    }

    public static float CurrentExp
    {
        get => PlayerPrefs.GetFloat("CurrentExp", 0);
        set => PlayerPrefs.SetFloat("CurrentExp", value);
    }
    
    public static bool CanWatchRevive
    {
        get => PlayerPrefs.GetInt("CanWatchRevive", 1) == 1;
        set => PlayerPrefs.SetInt("CanWatchRevive", value ? 1 : 0);
    }
    
    public static bool ShopWarning
    {
        get => PlayerPrefs.GetInt("ShopWarning", 1) == 1;
        set => PlayerPrefs.SetInt("ShopWarning", value ? 1 : 0);
    }
    
    public static int MainHandWarning
    {
        get => PlayerPrefs.GetInt("MainHandWarning", 1);
        set => PlayerPrefs.SetInt("MainHandWarning", value);
    }
    
    public static int OffHandWarning
    {
        get => PlayerPrefs.GetInt("OffHandWarning", 1);
        set => PlayerPrefs.SetInt("OffHandWarning", value);
    }
    
    public static int SkinWarning
    {
        get => PlayerPrefs.GetInt("SkinWarning", 1);
        set => PlayerPrefs.SetInt("SkinWarning", value);
    }

    public static bool TalentWarning => FreeUpgradeTalent > 0 && Diamond >= TalentUpgradedCount + 1;

    public static int PlayerTotalKillDeathMatch
    {
        get => PlayerPrefs.GetInt("PlayerTotalKillDeathMatch", 0);
        set => PlayerPrefs.SetInt("PlayerTotalKillDeathMatch", value);
    }

    public static int PlayerBestKillDeathMatch
    {
        get => PlayerPrefs.GetInt("PlayerBestKillDeathMatch", 0);
        set => PlayerPrefs.SetInt("PlayerBestKillDeathMatch", value);
    }

    public static int PlayerDeathMatchWinCount
    {
        get => PlayerPrefs.GetInt("PlayerDeathMatchWinCount", 0);
        set => PlayerPrefs.SetInt("PlayerDeathMatchWinCount", value);
    }
    public static string PlayerName
    {
        get => PlayerPrefs.GetString("PlayerName", "Player");
        set => PlayerPrefs.SetString("PlayerName", value);
    }
    public static int PlayerEye
    {
        get => PlayerPrefs.GetInt("PlayerEye", 0);
        set => PlayerPrefs.SetInt("PlayerEye", value);
    }
    #endregion

    #region INVENTORY

    public static int CurrentMainHandId
    {
        get => PlayerPrefs.GetInt("CurrentMainHandId", 0);
        set => PlayerPrefs.SetInt("CurrentMainHandId", value);
    }

    /// <summary>
    /// 0 - Block
    /// 1 - Bombard
    /// 2 - Ice Storm
    /// 3 - Ring
    /// 4 - Medic
    /// </summary>
    public static int CurrentOffHandId
    {
        get => PlayerPrefs.GetInt("CurrentOffHandId", 0);
        set => PlayerPrefs.SetInt("CurrentOffHandId", value);
    }

    public static int CurrentSkinId
    {
        get => PlayerPrefs.GetInt("CurrentSkinId", 0);
        set => PlayerPrefs.SetInt("CurrentSkinId", value);
    }

    public static int CurrentOffHandAi1Id = 0;  
    public static int CurrentOffHandAi2Id = 0;
    public static int CurrentOffHandAi3Id = 0;
    #endregion

    public static bool IsFirstTimePlay
    {
        get => PlayerPrefs.GetInt("IsFirstTimePlay", 1) == 1;
        set => PlayerPrefs.SetInt("IsFirstTimePlay", value ? 1 : 0);
    }
}
