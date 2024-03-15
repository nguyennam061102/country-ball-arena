using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyReward : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    
    int checkCoin;
    public int checkOrder;
    public List<DrSlotList> drList;

    public int xxx
    {
        get => PlayerPrefs.GetInt("xxx", 0);
        set
        {
            if (value >= 7) value = 0;
            PlayerPrefs.SetInt("xxx", value);
        }
    }

    private void OnEnable()
    {
        //Debug.Log(GameData.IsFirstTimePlay);
        //if (GameData.IsFirstTimePlay)
        //{
        //    UI.GetPanel(PanelName.PanelRename);
        //}
        //else
        //{
        //    CheckDaily();
        //}
    }

    // private void Start()
    // {
    //     Test();
    // }

    // void Test()
    // {
    //     checkOrder = xxx;
    //     if (checkOrder >= 7) checkOrder = 0;
    //     Debug.Log($"today: {checkOrder}");
    //     ShowDailyReward();
    // }

    public bool CheckDaily()
    {
        DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        string[] splitted = GameData.DailyReward.Split(':');
        int year = int.Parse(splitted[0]);
        int month = int.Parse(splitted[1]);
        int day = int.Parse(splitted[2]);
        int rOrder = int.Parse(splitted[3]);
        DateTime prevRewardDay = new DateTime(year, month, day);
        if (prevRewardDay < today)
        {
            checkOrder = ++rOrder;
            if (checkOrder >= 7) checkOrder = 0;
            if (GameData.IsFirstTimePlay)
            {
                UI.GetPanel(PanelName.PanelRename);
            }
            else
            {
                ShowDailyReward();
            }
            GameData.FreeSpin = true;
            GameData.NewDayOpen = true;
            return true;
        }
        return false;
    }

    public void ShowDailyReward()
    {
        var panelDr = UI.GetPanel(PanelName.PanelDailyReward).GetComponent<PanelDailyReward>();
        for (int i = 0; i < panelDr.drItemList.Count; i++)
        {
            panelDr.drItemList[i].SetItem(drList[i], checkOrder, panelDr);
        }
    }
}

[Serializable]
public class DrSlotList
{
    public List<DrSlot> drSlots;
}

[Serializable]
public class DrSlot
{
    public DrType drTypes;
    public int value;
}

[Serializable]
public enum DrType
{
    Gold,
    Diamond,
    Talent, 
    Random
}