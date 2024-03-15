using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardItem : MonoBehaviour
{
    public int id;
    [SerializeField] private UILabel lbDay, lbValue;
    public UI2DSprite icon;
    public GameObject rewarded, today;

    public void SetItem(DrSlotList drSlotList, int today, PanelDailyReward panelDailyReward)
    {
        if (id == today) panelDailyReward.todayReward = drSlotList;
        lbDay.text = id == today ? "Today" : $"Day {id + 1}";
        for (var i = 0; i < drSlotList.drSlots.Count; i++)
        {
            var t = drSlotList.drSlots[i];
            if (t == null) continue;
            if (i > 0) lbValue.text += "\n";
            lbValue.text += $"+{t.value} {t.drTypes.ToString()}";
        }

        this.today.SetActive(id == today);
        this.rewarded.SetActive(id < today);
    }
}
