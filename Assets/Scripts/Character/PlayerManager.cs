using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private MapController MapController => MapController.Instance;

    public LayerMask canSeePlayerMask;

    public List<Player> players = new List<Player>();

    public static PlayerManager Instance;

    private bool playersShouldBeActive;

    public AnimationCurve playerMoveCurve;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsAllAIDie()
    {
        foreach (Player p in players)
        {
            if (p.AI && !p.dead) return false;
        }
        return true;
    }

    public bool IsMaxOneSurvive()
    {
        int surviveCount = 0;
        foreach (Player p in players)
        {
            if (!p.dead) surviveCount++;
        }
        return surviveCount <= 1;
    }

    public Player GetClosestPlayer(Vector2 refPos, Vector2 forward)
    {
        Player result = null;
        float num = float.PositiveInfinity;
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].dead && CanSeePlayer(refPos, players[i]).canSee)
            {
                float num2 = Vector2.Distance(refPos, players[i].playerVel.position);
                num2 += Vector2.Angle(forward, players[i].playerVel.position - refPos);
                if (num2 < num)
                {
                    num = num2;
                    result = players[i];
                }
            }
        }
        return result;
    }

    public Player GetClosestPlayerToAim(Vector3 position, int caller, bool needVision = false)
    {
        float num = float.MaxValue;
        Player[] playersInTeam = GetPlayersNotInID(caller, true);
        Player result = null;
        for (int i = 0; i < playersInTeam.Length; i++)
        {
            if (!players[i].dead)
            {
                float num2 = Vector2.Distance(position, playersInTeam[i].transform.position);
                if ((!needVision || CanSeePlayer(position, playersInTeam[i]).canSee) && num2 < num)
                {
                    num = num2;
                    result = playersInTeam[i];
                }
            }
        }
        return result;
    }

    public Player GetRandomPlayerToAim(int caller, bool needVision = false)
    {
        Player[] playersInTeam = GetPlayersNotInID(caller, true);
        Player result = null;
        if (playersInTeam.Length != 0)
        {
            result = playersInTeam[UnityEngine.Random.Range(0, playersInTeam.Length)];
        }
        return result;
    }

    private readonly CanSeeInfo canSeeInfo = new CanSeeInfo();
    public CanSeeInfo CanSeePlayer(Vector2 from, BaseCharacter player)
    {
        canSeeInfo.canSee = true;
        canSeeInfo.distance = float.PositiveInfinity;
        if (!player)
        {
            canSeeInfo.canSee = false;
            return canSeeInfo;
        }
        RaycastHit2D[] array = Physics2D.RaycastAll(from, (player.playerVel.position - from).normalized, Vector2.Distance(from, player.playerVel.position), canSeePlayerMask);
        for (int i = 0; i < array.Length; i++)
        {
            if ((bool)array[i].transform && !array[i].transform.root.GetComponent<SpawnedAttack>() && !array[i].transform.root.GetComponent<BaseCharacter>() && array[i].distance < canSeeInfo.distance)
            {
                canSeeInfo.canSee = false;
                canSeeInfo.hitPoint = array[i].point;
                canSeeInfo.distance = array[i].distance;
            }
        }
        return canSeeInfo;
    }

    internal Player GetPlayerWithID(int playerID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID == playerID)
            {
                return players[i];
            }
        }
        return null;
    }

    public static void RegisterPlayer(Player player)
    {
        Instance.players.Add(player);
        if (Instance.playersShouldBeActive)
        {
            player.isPlaying = true;
        }
    }

    public Player[] GetPlayersInID(int playerID)
    {
        List<Player> list = new List<Player>();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID == playerID)
            {
                list.Add(players[i]);
            }
        }
        return list.ToArray();
    }

    public Player[] GetPlayersNotInID(int playerID, bool needAlive = false)
    {
        List<Player> list = new List<Player>();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID != playerID)
            {
                if (!needAlive)
                {
                    list.Add(players[i]);
                }
                else if (!players[i].dead)
                {
                    list.Add(players[i]);
                }
            }
        }
        return list.ToArray();
    }

    public void CallMovePlayers()
    {
        MovePlayers(MapController.Instance.GetSpawnPoints());
    }

    private void MovePlayers(SpawnPoint[] spawnPoints)
    {
        EnablePlayersCollider(false);
        for (int i = 0; i < players.Count; i++)
        {
            StartCoroutine(Move(players[i].playerVel, spawnPoints[i].localStartPos));
        }
    }

    public void MoveOnePlayerAndRespawn(int id, Vector3 destination)
    {
        //StartCoroutine(BlinkMove(GetPlayerWithID(id).playerVel, destination, true));
        BlinkMove(GetPlayerWithID(id).playerVel, destination, true);
    }

    public void SetPlayersPlaying(bool playing, int playerID)
    {
        playersShouldBeActive = playing;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID == playerID) players[i].isPlaying = playing;
        }
    }

    public void SetPlayersSimulated(bool simulated, int playerID)
    {
        playersShouldBeActive = simulated;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID == playerID) players[i].playerVel.simulated = simulated;
        }
    }

    public void SetPlayersKinematic(bool kinematic, int playerID)
    {
        playersShouldBeActive = kinematic;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerID == playerID) players[i].playerVel.isKinematic = kinematic;
        }
    }

    public void SetPlayersCollider(bool enable)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].mainCol.enabled = enable;
        }
    }

    public void BlinkMove(PlayerVelocity player, Vector3 targetPos, bool reviveRightAfterMove = false)
    {
        player.GetComponent<Player>().isPlaying = false;
        player.simulated = false;
        player.isKinematic = true;
        player.GetComponent<BaseCharacter>().mainCol.enabled = false;
        player.GetComponent<BaseCharacter>().transform.position = targetPos;
        if (reviveRightAfterMove)
        {
            player.simulated = true;
            player.isKinematic = false;
            player.GetComponent<Player>().isPlaying = true;
            player.GetComponent<HealthHandler>().canTakeDamage = true;
            player.GetComponent<BaseCharacter>().mainCol.enabled = true;
            player.GetComponent<Player>().healthHandler.CallRevive();
        }
    }

    public IEnumerator Move(PlayerVelocity player, Vector3 targetPos, bool reviveRightAfterMove = false)
    {
        player.GetComponent<Player>().isPlaying = false;
        player.simulated = false;
        player.isKinematic = true;
        player.GetComponent<BaseCharacter>().mainCol.enabled = false;
        Vector3 targetStartPos = player.transform.position;
        PlayerCollision col = player.GetComponent<PlayerCollision>();
        player.GetComponent<BaseCharacter>().MovePlayer(targetPos);
        if (reviveRightAfterMove)
        {
            yield return new WaitForSecondsRealtime(player.GetComponent<BaseCharacter>().TweenPosition.duration * 1.1f);
            player.simulated = true;
            player.isKinematic = false;
            player.GetComponent<Player>().isPlaying = true;
            player.GetComponent<HealthHandler>().canTakeDamage = true;
            player.GetComponent<BaseCharacter>().mainCol.enabled = true;
            player.GetComponent<Player>().healthHandler.CallRevive();
        }
    }

    public void SetPlayerRealTargetBasedOnStage(int stage)
    {
        //1-10: 3
        //11-20: 2
        //21-30: 1
        //31-40: 0.5
        //41-50: 0
        float dValue = 3f;
        if (stage <= 10)
        {
            dValue = 3f;
        }
        else if (stage <= 20)
        {
            dValue = 2f;
        }
        else if (stage <= 30)
        {
            dValue = 1f;
        }
        else if (stage <= 40)
        {
            dValue = 0.5f;
        }
        else
        {
            dValue = 0f;
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTargetDistance(dValue);
        }
    }

    public void EnablePlayersCollider(bool flag)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].EnableCollider(flag);
        }
    }

    public void SetPlayerCanTakeDamage(bool flag)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<HealthHandler>().canTakeDamage = flag;
        }
    }
}
