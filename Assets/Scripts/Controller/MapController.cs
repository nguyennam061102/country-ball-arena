using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapController : SingletonMonoBehavior<MapController>
{
    private GameController gameController => GameController.Instance;
    private PlayerManager playerManager => PlayerManager.Instance;
    [SerializeField] private MapTweener[] mapPrefabList;
    [SerializeField] private MapTweener mapTutorial;
    [HideInInspector] public MapTweener currentMap;

    [SerializeField] private ParticleColorChanger FGColor;
    [SerializeField] SpriteRenderer bgMap;
    [SerializeField] Sprite[] allBGs;
    [SerializeField] GameObject deathMatchBox;

    private int CurrentMapId
    {
        get => PlayerPrefs.GetInt("CurrentMapId", 0);
        set => PlayerPrefs.SetInt("CurrentMapId", value);
    }

    public void FirstTimeShowMap()
    {
        //if (!FGColor.isInit) FGColor.Init();
        //if (!BGColor.isInit) BGColor.Init();

        bgMap.transform.localScale = Vector3.one * 2f;
        bgMap.sprite = allBGs[Random.Range(0, allBGs.Length)];
        bgMap.color = new Color32(180, 180, 180, 255);

        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            SpawnMap(true);
        }
        else
        {
            SpawnMap(false);
            UiController.Instance.GetPanel(PanelName.PanelSelectCard).GetComponent<PanelSelectCard>().StartShowCard(8, 3);
        }
        //todo: Create Player Here
        FakeOnlController.Instance.GetRandomeName();
        PlayerAssigner.instance.Create(false);
        PlayerAssigner.instance.Create(true);
        PlayerAssigner.instance.Create(true);
        PlayerAssigner.instance.Create(true);
        gameController.GetComponent<MovementController>().SetPlayerBlock(playerManager.GetPlayerWithID(0));
    }

    public void SpawnMap(bool showImmediately = false)
    {
        if (GameData.TutorialCompleted == 0)
        {
            currentMap = Instantiate(mapTutorial, Vector3.zero, Quaternion.identity);
        }
        else
        {
            //currentMap = Instantiate(mapPrefabList[16], Vector3.zero, Quaternion.identity);
            currentMap = Instantiate(mapPrefabList[GetRandomMapId()], Vector3.zero, Quaternion.identity);
        }
        if (showImmediately) currentMap.ShowMap();
        pointsToSpawn = currentMap.GetComponentsInChildren<SpawnPoint>();
        //FGColor.SetParticleAndColor();
        //BGColor.SetParticleAndColor();   

        if ((bool)playerManager)
        {
            if (!gameController.startGame)
            {
                Player[] players = playerManager.GetPlayersNotInID(0);
                foreach (Player player in players)
                {
                    player.GetComponent<PlayerAI>().EnableMovement(false);
                    player.healthHandler.Revive();
                    player.SetHealthStats();
                    player.holding.GetGunAndSpawn();

                    //if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival)) //survival thi moi random
                    //{
                    //    player.skinHandler.SetCharacterSkin();
                    //}

                    if (player.playerID == 1)
                    {
                        GameData.CurrentOffHandAi1Id = Random.Range(0, 5);
                    }
                    else if (player.playerID == 2)
                    {
                        GameData.CurrentOffHandAi2Id = Random.Range(0, 5);
                    }
                    //add card every wave
                    int rndCard = Random.Range(0, gameController.cards.Length);
                    gameController.cards[rndCard].GetComponent<ApplyCardStats>().PickCard(player.playerID, false, PickerType.Player, false);

                    if (player.playerID == 1) gameController.previousCardsAI1.Add(rndCard);
                    else if (player.playerID == 2) gameController.previousCardsAI2.Add(rndCard);
                }

                playerManager.GetPlayerWithID(0).holding.GetGunAndSpawn();
                playerManager.CallMovePlayers();
            }
        }
    }

    List<int> listIDToRandom;

    int GetRandomMapId()
    {
        listIDToRandom = new List<int>();
        for (int i = 0; i < mapPrefabList.Length; i++)
        {
            if (i != CurrentMapId)
            {
                listIDToRandom.Add(i);
            }
        }
        CurrentMapId = listIDToRandom[Random.Range(0, listIDToRandom.Count)];
        return CurrentMapId;
    }

    [SerializeField] SpawnPoint[] pointsToSpawn;

    public SpawnPoint[] GetSpawnPoints()
    {
        if (pointsToSpawn == null) pointsToSpawn = currentMap.GetComponentsInChildren<SpawnPoint>();
        return pointsToSpawn;
    }

    public void SpawnWeaponCache()
    {
        Vector3 pos = currentMap.GetRespawnPosition();
        pos.y = gameController.screenSize.y * 1.25f;
        Rigidbody2D cb = Instantiate(deathMatchBox, pos, Quaternion.identity).GetComponent<Rigidbody2D>();
        cb.transform.parent = this.currentMap.transform;
        cb.transform.localScale = Vector3.one * 0.8f;
        cb.AddTorque(Random.Range(-100f, 100f));
        cb.AddForce(Vector2.down * Random.Range(0, 20f), ForceMode2D.Impulse);
        currentMap.AddPhysicBlock(cb);
    }
}