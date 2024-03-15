using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelRate : MonoBehaviour
{
    [SerializeField] GameObject groupYesReward, groupNoReward;
    string rValue = "";
    int rInt = 0;

    private void OnEnable()
    {
        rValue = SkygoBridge.instance.GetConfig("game_rate_type");
        if (String.IsNullOrEmpty(rValue)) rValue = "0";
        rInt = int.Parse(rValue);
        groupYesReward.SetActive(rInt != 0);
        groupNoReward.SetActive(rInt == 0);
    }

    public void OnRateButton()
    {
        SkygoBridge.instance.RateGame();       
        if (rInt != 0)
        {
            GameData.Gold += 15000;
            GameData.Diamond += 30;
        }
        GameData.RateType = true;
        gameObject.SetActive(false);
    }

    public void OnRateLaterButton()
    {
        gameObject.SetActive(false);
    }
}
