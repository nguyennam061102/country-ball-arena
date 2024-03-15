using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : BaseCharacter
{
    public int playerID;
    public int outOfBoundCount;

    private void Start()
    {
        SetHealthStats();
        //GetComponent<CharacterStatModifiers>().ConfigureMassAndSize();
        //Invoke("SetMassAndSize", 0.1f);
        StartCoroutine(SetMassAndSize());
    }

    IEnumerator SetMassAndSize()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        GetComponent<CharacterStatModifiers>().ConfigureMassAndSize();
    }

    public void BossSetHealth()
    {
        level = GameData.PlayerLevel;
        if (Stats != null) maxHealth = Stats.GetPlayerHealthBasedOnManyThings(skinHandler.skin, level, level + 4);
    }

    public void SetHealthStats()
    {
        if (!AI) // Player
        {
            level = GameData.PlayerLevel;
            if (Stats != null) maxHealth = Stats.GetPlayerHealthBasedOnManyThings(skinHandler.skin, level, GameFollowData.Instance.skinList[GameData.CurrentSkinId].ItemLevel, Stats.TalentHealth);
            health = maxHealth;
        }
        else // AI
        {
            if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival) || GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
            {
                level = Random.Range(GameData.PlayerLevel - 1, GameData.PlayerLevel + 1) + (int)(Mathf.Pow(1.1f, GameData.MatchLevel));
            }
            else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
            {
                level = Random.Range(GameData.PlayerLevel - 1, GameData.PlayerLevel + 1) + (int)(GameData.PlayerDeathMatchWinCount / 3);
            }

            if (Stats != null) maxHealth = Stats.GetPlayerHealthBasedOnManyThings(skinHandler.skin, GameData.PlayerLevel - 1, level);
            health = maxHealth;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    public void AssignPlayerID(int ID)
    {
        playerID = ID;
    }

    public void ResetPlayerStats()
    {
        holding.holdable.GetComponent<Gun>().ResetStats();
        stats.ResetStats();
        block.ResetStats();
    }
}
