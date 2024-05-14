using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterSkinHandler : MonoBehaviour
{
    [SerializeField] private BaseCharacter Data;
    public CharacterSkin[] skinList;
    public CharacterSkin skin;
    public SpriteRenderer headSr;
    public SkeletonAnimation faceAnim;
    public SpriteRenderer eye;

    public Color color;

    public Action onSetSkinAction;

    void SetupSkin()
    {
        headSr.sprite = skin.skinSprite;
        if (Data.AI)
        {
            int index = Random.Range(0, GameFollowData.Instance.eyes.Count);
            eye.sprite = GameFollowData.Instance.eyes[index];
        }else
        {
            eye.sprite = skin.eye;
        }
        if (skin.defaultSkin)
        {
            if (Data.player.playerID == 0)
            {
                color = new Color32(0, 240, 255, 255);
            }
            else if (Data.player.playerID == 1)
            {
                color = new Color32(255, 17, 0, 255);
            }
            else if (Data.player.playerID == 2)
            {
                color = new Color32(0, 137, 3, 255);
            }
        }
        else
        {
            color = skin.useColor ? skin.skinColor : Color.white;
        }
        //headSr.color = color;
        Data.SetLandPartColor();
        Data.blockTrigger.SetBlock();
        //faceAnim.skeletonDataAsset = skin.faceAnim;
        //faceAnim.ClearState();
        //faceAnim.Initialize(true);
        //faceAnim.state.SetAnimation(0, "idle", true);
        //faceAnim.gameObject.SetActive(false);
        onSetSkinAction?.Invoke();
    }

    public void SetCharacterSkin()
    {
        if (!Data.AI)
        {
            skin = skinList[GameData.CurrentSkinId];
        }
        else
        {
            if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival))
            {
                skin = skinList[GetBossSkin()];
            }
            else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
            {
                if (Data.player.playerID == 1) skin = skinList[GameFollowData.Instance.Player2SkinID];
                else if (Data.player.playerID == 2) skin = skinList[GameFollowData.Instance.Player3SkinID];
            }
            else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
            {
                skin = skinList[GetBossSkin()];
            }
        }
        SetupSkin();
    }

    public int SetBossSkin()
    {
        if (Data.AI)
        {
            int val = GetBossSkin();
            skin = skinList[val];
            SetupSkin();
            return val;
        }
        else return -1;
    }

    public void Win()
    {
        //faceAnim.state.SetAnimation(0, "win", true);
    }

    int GetRandomSkin()
    {
        return Random.Range(0, skinList.Length);
    }

    int GetCreepSkin()
    {
        return 0;
    }

    int GetBossSkin()
    {
        return Random.Range(1, skinList.Length);
    }

    public Color ColorForEFX
    {
        get
        {
            if (skin.defaultSkin && (bool)Data)
            {
                if (Data.player.playerID == 0)
                {
                    return new Color32(0, 240, 255, 255);
                }
                else if (Data.player.playerID == 1)
                {
                    return new Color32(255, 17, 0, 255);
                }
                else
                {
                    return new Color32(0, 137, 3, 255);
                }
            }
            else if (SkygoBridge.instance.isForRecording() && SkygoBridge.instance.isForADs())
            {
                if (skin.skinId == 1 || skin.skinId == 6 || skin.skinId == 12)
                {
                    return new Color32(205, 205, 205, 255);
                }
            }
            return skin.skinColor;
        }
    }

    public Color ColorForParts
    {
        get
        {
            if (skin.defaultSkin && (bool)Data)
            {
                if (Data.player.playerID == 0)
                {
                    return new Color32(0, 240, 255, 255);
                }
                else if (Data.player.playerID == 1)
                {
                    return new Color32(255, 17, 0, 255);
                }
                else
                {
                    return new Color32(0, 137, 3, 255);
                }
            }
            return skin.skinColor;
        }
    }
}
