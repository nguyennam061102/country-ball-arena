using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GameFollowData : MonoBehaviour
{
    public ShopItemInfo[] mainHandList;
    public ShopItemInfo[] offHandList;
    public ShopItemInfo[] skinList;
    public ShopItemInfo[] diamondList;
    public ShopItemInfo[] goldList;

    [Header("===TALENTS===")] public TalentItemInfo[] talentItemList;
    [Header("===PLAYING GAME MODE===")] public GameMode playingGameMode;
    public int Player2SkinID, Player2WeaponID, Player2OffhandID, Player3SkinID, Player3WeaponID, Player3OffhandID;
    public bool IsChangeSceneFromGameplay = false;
    public bool IsToUpgradeCharacter = false;

    public void UnlockItem(ShopItemInfo itemInfo)
    {
        itemInfo.IsUnlocked = true;
    }

    public bool isPlaying = true;

#if UNITY_EDITOR
    [ContextMenu("EXECUTE ITEM ID")]
    void ExecuteItemId()
    {
        for (int i = 0; i < mainHandList.Length; i++)
        {
            mainHandList[i].itemId = i;
            EditorUtility.SetDirty(mainHandList[i].itemIcon);
        }

        for (int i = 0; i < offHandList.Length; i++)
        {
            offHandList[i].itemId = i;
            EditorUtility.SetDirty(offHandList[i].itemIcon);
        }

        for (int i = 0; i < skinList.Length; i++)
        {
            skinList[i].itemId = i;
            skinList[i].useAnim = true;
            EditorUtility.SetDirty(skinList[i].itemIcon);
        }

        for (int i = 0; i < diamondList.Length; i++)
        {
            diamondList[i].itemId = i;
            EditorUtility.SetDirty(diamondList[i].itemIcon);
        }

        for (int i = 0; i < goldList.Length; i++)
        {
            goldList[i].itemId = i;
            EditorUtility.SetDirty(goldList[i].itemIcon);
        }

        for (int i = 0; i < talentItemList.Length; i++)
        {
            talentItemList[i].itemId = i;
            EditorUtility.SetDirty(talentItemList[i].itemIcon);
        }
    }
#endif


    private static GameFollowData mInstance;

    public static GameFollowData Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameFollowData.Instance;
            }

            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
        }
        else Destroy(this.gameObject);
        foreach(ShopItemInfo skin in skinList)
        {
            skin.IsUnlocked = true;
        }
    }

    //public bool IsAllMainHandGoldUnlock => mainHandList.All(itemInfo => itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold);
    public bool IsAllMainHandGoldUnlock
    {
        get
        {
            foreach (var itemInfo in mainHandList)
            {
                if (!itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold) return false;
                //break;
            }
            return true;
        }
    }

    public bool IsAllMainHandGoldMaxLevel
    {
        get
        {
            foreach (var itemInfo in mainHandList)
            {
                if (!itemInfo.IsLevelMax && itemInfo.currencyType == CurrencyType.Gold) return false;
                //break;
            }
            return true;
        }
    }

    public bool IsAllSkinGoldUnlock
    {
        get
        {
            foreach (var itemInfo in skinList)
            {
                if (!itemInfo.IsUnlocked && itemInfo.currencyType == CurrencyType.Gold) return false;
                break;
            }
            return true;
        }
    }
    public bool IsAllTalentMaxLevel => talentItemList.All(itemInfo => itemInfo.IsLevelMax);
}

[Serializable]
public class ShopItemInfo
{
    public string itemName;
    public int itemId;
    public ShopItemType shopItemType;
    public CurrencyType currencyType;
    public Sprite itemIcon;
    public float price, basePriceToUpgrade, value;
    public string sku;
    public bool currency;
    public bool canUpgrade;
    public bool useAnim;
    [TextArea]
    public string itemShortDesc;

    [Header("MAIN HAND")]
    [SerializeField] Gun connectedGun;
    public float damage => connectedGun.damage;
    public float numPerShot => connectedGun.numberOfProjectiles;
    public float rof => connectedGun.attackSpeed;

    [Header("OFF HAND")]
    public string function;
    public float coolDown;

    [Header("SKIN")]
    [SerializeField] CharacterSkin connectedSkin;
    public float health => connectedSkin.health;
    public float moveSpeed => connectedSkin.speed;
    public int signatureCard;

    public bool IsUnlocked
    {
        get
        {
            if (currency) return false;
            return PlayerPrefs.GetInt($"{itemName}_{itemId}_unlocked", itemId == 0 ? 1 : 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt($"{itemName}_{itemId}_unlocked", value ? 1 : 0);
            switch (shopItemType)
            {
                case ShopItemType.MainHand:
                    GameData.MainHandWarning++;
                    break;
                case ShopItemType.OffHand:
                    GameData.OffHandWarning++;
                    break;
                case ShopItemType.Skin:
                    //GameData.SkinWarning++;
                    break;
            }
        }
    }

    public int ItemLevel
    {
        get => PlayerPrefs.GetInt($"{itemName}_{itemId}_level", 1);
        set => PlayerPrefs.SetInt($"{itemName}_{itemId}_level", value);
    }

    public bool IsLevelMax => ItemLevel == 80;
}

public enum ShopItemType
{
    Gold,
    Diamond,
    MainHand,
    OffHand,
    Skin
}

public enum CurrencyType
{
    Gold,
    Diamond
}

[Serializable]
public class TalentItemInfo
{
    public string itemName;
    public int itemId;
    public Sprite itemIcon;
    public float baseValue;

    public int Level
    {
        get => PlayerPrefs.GetInt($"talent_{itemName}_lv", 0);
        set => PlayerPrefs.SetInt($"talent_{itemName}_lv", value);
    }

    public bool IsLevelMax => Level == 100;

    public float Value => PlayerPrefs.GetFloat($"talent_{itemName}_val", Level * baseValue) / 100;
}

public enum GameMode
{
    Survival,
    DeathMatch,
    SandBox
}