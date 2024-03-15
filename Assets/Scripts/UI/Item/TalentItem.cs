using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentItem : MonoBehaviour
{

    [HideInInspector] public TalentItemInfo itemInfo;

    [SerializeField] private int itemId;
    [SerializeField] private UI2DSprite itemIcon;
    [SerializeField] private UILabel itemName;
    [SerializeField] private UILabel itemLevel;

    public void SetItem(TalentItemInfo itemInfo, bool upgrade = false)
    {
        this.itemInfo = itemInfo;
        this.itemId = itemInfo.itemId;
        this.itemIcon.sprite2D = itemInfo.itemIcon;
        this.itemIcon.MakePixelPerfect();
        //this.itemName.text = itemInfo.itemName; [514685]x{itemInfo.numPerShot}[-]
        this.itemName.text = $"[12FF00]+{itemInfo.Value * 100}%[-] {itemInfo.itemName}";
        this.itemLevel.text = itemInfo.Level.ToString();
        transform.localScale = Vector3.one * 0.8f;
        if (!upgrade) return;
        this.itemLevel.GetComponent<UITweener>().ResetToBeginning();
        this.itemLevel.GetComponent<UITweener>().PlayForward();

    }

    public void UpgradeLevel()
    {
        itemInfo.Level++;
        this.itemName.text = $"+{itemInfo.Value * 100}% + {itemInfo.itemName}";
        SetItem(itemInfo, true);
    }
}
