using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAssigner : MonoBehaviour
{
    public static PlayerAssigner instance;
    public GameObject playerPrefab;
    int maxPlayers = 4;
    public List<BaseCharacter> players = new List<BaseCharacter>(4);
    public BaseCharacter mainPlayer;
    private int playerIDToSet = -1;

    private void Awake()
    {
        instance = this;
    }

    public void Create(bool isAi)
    {
        if (players.Count < maxPlayers)
        {
            playerIDToSet = PlayerManager.Instance.players.Count;
            Vector3 position = MapController.Instance.GetSpawnPoints()[playerIDToSet].localStartPos;
            BaseCharacter component = Instantiate(playerPrefab, position, Quaternion.identity).GetComponent<BaseCharacter>();

            players.Add(component);
            RegisterPlayer(component, playerIDToSet);

            if (isAi)
            {
                component.nameAI = FakeOnlController.Instance.NameRandom();
                component.GetComponent<BaseCharacter>().SetAI();
                component.holding.GetGunAndSpawn();
                if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival) || GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
                {
                    if (playerIDToSet == 1)
                    {
                        GameData.CurrentOffHandAi1Id = Random.Range(0, 5);
                    }
                    else if (playerIDToSet == 2)
                    {
                        GameData.CurrentOffHandAi2Id = Random.Range(0, 5);
                    }
                }
                else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
                {
                    GameData.CurrentOffHandAi1Id = GameFollowData.Instance.Player2OffhandID;
                    GameData.CurrentOffHandAi1Id = GameFollowData.Instance.Player3OffhandID;
                }
            }
            else
            {
                component.nameAI = GameData.PlayerName;
                mainPlayer = component;
                component.holding.holdablePrefab = component.weaponList[GameData.CurrentMainHandId];
                component.holding.SpawnGun();
            }           
            component.skinHandler.SetCharacterSkin();
            component.playerVel.simulated = false;
            component.playerVel.isKinematic = true;
        }
    }

    private void RegisterPlayer(BaseCharacter player, int playerID)
    {
        PlayerManager.RegisterPlayer(player.GetComponent<Player>());
        player.player.AssignPlayerID(playerID);
    }
}