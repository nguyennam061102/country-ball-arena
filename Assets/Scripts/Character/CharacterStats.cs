using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats Instance;
    private void Awake()
    {
        Instance = this;
    }

    private GameFollowData GfData => GameFollowData.Instance;
    private TalentItemInfo[] TalentList => GfData.talentItemList;

    public float GunLevelDamageMultiplier(int currentlevel)
    {
        return Mathf.Pow(1.1f, currentlevel - 1);
    }

    public float SkinLevelHPMultiplier(int currentLevel)
    {
        return Mathf.Pow(1.08f, currentLevel - 1);
    }

    //todo: TOTAL HEALTH
    public float GetPlayerHealthBasedOnManyThings(CharacterSkin cSkin, int characterLevel, int skinLevel, float talentHealth = 0, float healthBonus = 0)
    {
        if (cSkin == null) return 1;
        return cSkin.health * SkinLevelHPMultiplier(skinLevel) * Mathf.Pow(1.1f, characterLevel - 1) * (1 + talentHealth) + healthBonus;
    }

    //todo: TOTAL DAMAGE
    public float GetPlayerDamageBasedOnManyThings(float baseDamage, int gunLevel)
    {
        return Mathf.Round(baseDamage * GunLevelDamageMultiplier(gunLevel) * (1 + TalentDamage));
    }

    //todo: TAKE DAMAGE
    private float DegreaseDamagePercent(int levelA, int levelB)
    {
        if (levelA > levelB)
        {
            return Mathf.Pow(0.92f, levelB - levelA);
        }
        return 1;
    }

    public float GetDamageTaken(float baseDamage, float talentDefense, float card, int levelA, int levelB)
    {
        return baseDamage * (1 - talentDefense) * (1 - card) * DegreaseDamagePercent(levelA, levelB);
    }

    #region TalentsInfo

    public float TalentHealth => TalentList[0].Value;
    public float TalentDamage => TalentList[1].Value;
    public float TalentSpeed => TalentList[2].Value;
    public float TalentAvoid => TalentList[3].Value;
    public float TalentSkillDamage => TalentList[4].Value;
    public float TalentCooldown => TalentList[5].Value;
    public float TalentCritical => TalentList[6].Value;
    public float TalentGoldBonus => TalentList[7].Value;
    public float TalentDefense => TalentList[8].Value;
    public float TalentExp => TalentList[9].Value;

    #endregion

}
