using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentSlot : MonoBehaviour
{
    private BaseCharacter Data => GetComponentInParent<BaseCharacter>();
    public List<SpriteRenderer> spriteList;
    private Color color;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            spriteList.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
        
        CheckColor();
        Data.skinHandler.onSetSkinAction += CheckColor;
    }

    private void CheckColor()
    {
        foreach (var sr in spriteList)
        {
            sr.color = Data.skinHandler.ColorForParts;
        }
    }
}
