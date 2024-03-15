using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentItemUi : MonoBehaviour
{

    [SerializeField] private int itemId;
    [SerializeField] private UILabel talentText;

    public void SetTalentUi(TalentItemInfo talentItemInfo)
    {
        itemId = talentItemInfo.itemId;
        talentText.text = $"{talentItemInfo.itemName}: +{talentItemInfo.Value * 100}%";
    }
}
