using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventTrackerProVCL : DontDestroy<GameEventTrackerProVCL>
{
    public float CoinMultipleAmount;
    public float DamageMultipleAmount;
    public int AdditionRounds
    {
        get
        {
            return PlayerPrefs.GetInt("AdditionalRound", 0);
        }
        set
        {
            PlayerPrefs.SetInt("AdditionalRound", value);
        }
    }
    public bool AllowSelectCardAds = true;
    public float CameraShakeMultiplier = 1f;
    public int NumPlayToShowAds
    {
        get
        {
            return PlayerPrefs.GetInt("NumPlayToShowAds", 0);
        }
        set
        {
            PlayerPrefs.SetInt("NumPlayToShowAds", value);
        }
    }

    //Thêm số round cần để hiện quảng cáo
    public void GetNumPlayToShowAds()
    {
        string val = SkygoBridge.instance.GetConfig("num_play_show_ads");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                NumPlayToShowAds = int.Parse(val);
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Mức độ rung của camera
    public void GetDamageCameraShakeAmount()
    {
        string val = SkygoBridge.instance.GetConfig("camera_shake_multiplier");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                CameraShakeMultiplier = float.Parse(val) / 100f;
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Thêm số round cần để chơi
    public void GetAdditionalRoundToPlay()
    {
        string val = SkygoBridge.instance.GetConfig("additional_rounds");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                AdditionRounds = int.Parse(val);
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Sát thương quái gây ra cho nhân vật chính
    public void GetAllowSelectCardAds()
    {
        string val = SkygoBridge.instance.GetConfig("allow_select_card_ads");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                AllowSelectCardAds = int.Parse(val) == 1;
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Sát thương quái gây ra cho nhân vật chính
    public void GetDamageMultipleAmount()
    {
        string val = SkygoBridge.instance.GetConfig("bot_damage_multiplier");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                DamageMultipleAmount = float.Parse(val) / 100f;
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Số vàng nhận được thực tế sẽ nhân với CoinMultipleAmount(), ví dụ nhận đc 500 vàng với giá trị này là 0.5 thì họ chỉ nhận 250 vàng
    public void GetCoinMultipleAmount()
    {
        string val = SkygoBridge.instance.GetConfig("coin_multiple_amount");
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                CoinMultipleAmount = float.Parse(val) / 100f;
            }
            catch (Exception e)
            {
                //nope!
            }
        }
    }

    //Gọi khi có nhân vật chết - 0 là mình chết 1 là địch chết
    public void OnCharacterDie(int side)
    {
        if (side == 0) SkygoBridge.instance.LogEvent("player_die");
    }

    //Gọi khi người chơi mua đồ 0: súng, 1: off-hand, 2: skin
    public void OnPlayerPurchase(int id)
    {
        if (id == 0)
        {
            SkygoBridge.instance.LogEvent("player_purchase_gun");
        }
        else if (id == 1)
        {
            SkygoBridge.instance.LogEvent("player_purchase_offhand");
        }
        else if (id == 2)
        {
            SkygoBridge.instance.LogEvent("player_purchase_skin");
        }
    }

    //Gọi khi người chơi hồi sinh vì xem HẾT VIDEO QUẢNG CÁO
    public void OnPlayerWatchVideoToRespawn()
    {
        SkygoBridge.instance.LogEvent("player_watch_video_to_respawn");
    }

    //Gọi khi người chơi nâng talent
    public void OnPlayerUpgradeTalent()
    {
        SkygoBridge.instance.LogEvent("player_upgrade_talent");
    }

    //Gọi khi người chơi thắng round đó
    public void OnPlayerCompleteRound()
    {
        SkygoBridge.instance.LogEvent("player_complete_round");
    }

    //Gọi khi người chơi hoàn thành game
    public void OnPlayerCompleteMatch()
    {
        SkygoBridge.instance.LogEvent("player_complete_match");
    }

    //Gọi khi người chơi lên level
    public void OnPlayerLevelUp()
    {
        SkygoBridge.instance.LogEvent("player_level_up");
    }

    //Gọi khi người chơi nâng cấp 1 cái gì đó - string là tên vũ khí / tên skin / tên off-hand
    public void OnPlayerUpgrade(string thingtoupgrade)
    {
        SkygoBridge.instance.LogEvent("player_upgrade_" + thingtoupgrade);
    }

    //Gọi khi người chơi nhận thưởng của ngày thứ x
    public void OnPlayerGetDayReward(int day)
    {
        SkygoBridge.instance.LogEvent("player_get_reward_day_" + day);
    }

    //Gọi khi người chơi xem video
    public void OnPlayerWatchVideoToReward(string pString)
    {
        SkygoBridge.instance.LogEvent("player_watch_video_" + pString);
    }

    //Gọi khi người chơi chơi 1 mode nào đó
    public void OnPlayerPlayGameMode(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Survival:
                SkygoBridge.instance.LogEvent("player_play_survival");
                break;
            case GameMode.DeathMatch:
                SkygoBridge.instance.LogEvent("player_play_deathmatch");
                break;
            case GameMode.SandBox:
                SkygoBridge.instance.LogEvent("player_play_sandbox");
                break;
        }
    }
}
