using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelContinue : MonoBehaviour
{
    private PlayerManager PlayerManager => PlayerManager.Instance;
    private GameController GController => GameController.Instance;
    private MovementController MovementController => GameController.Instance.MovementController;

    [SerializeField] private UILabel counterText;
    [SerializeField] private float counter = 5;

    private void OnEnable()
    {
        MovementController.ShowIngameUI(false);
        //MovementController.movementGo.SetActive(false);
        //GC.gamePaused = true;

        for (int i = 0; i < PlayerManager.players.Count; i++)
        {
            PlayerManager.SetPlayersSimulated(false, i);
            PlayerManager.SetPlayersKinematic(true, i);
        }

        PlayerManager.EnablePlayersCollider(false);
        PlayerManager.SetPlayerCanTakeDamage(false);

        counter = 5;
        counterText.text = $"{counter:0}";
        StartCoroutine(Counter());
    }

    IEnumerator Counter()
    {
        yield return new WaitForSeconds(1f);
        while (counter > 1)
        {
            counter--;
            counterText.text = $"{counter:0}";
            yield return new WaitForSeconds(1f);
        }
        //GameData.CanWatchRevive = false;
        //GameController.Instance.EndGame(false);
        //Destroy(gameObject);
        OnNoTksButton();
    }

    public void OnRevivalButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            PlayerManager.GetPlayerWithID(0).healthHandler.CallRevive();
            GameData.CanWatchRevive = false;
            for (int i = 0; i < PlayerManager.players.Count; i++)
            {
                PlayerManager.SetPlayersSimulated(true, i);
                PlayerManager.SetPlayersKinematic(false, i);
            }
            PlayerManager.EnablePlayersCollider(true);
            PlayerManager.SetPlayerCanTakeDamage(true);
            //MovementController.movementGo.SetActive(true);
            MovementController.ShowIngameUI(true);
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToRespawn();
            Destroy(gameObject);
        });
        UnityEvent onFailed = new UnityEvent();
        onFailed.AddListener(() =>
        {
            OnNoTksButton();
        });
        onDone.Invoke();
        //reward
        //bool show = ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, onFailed);
        //if (show) StopAllCoroutines();
    }

    public void OnNoTksButton()
    {
        GameData.CanWatchRevive = false;
        MovementController.ShowIngameUI(true);
        GameController.Instance.EndGame(false, 3f);
        Sound.Play(Sound.SoundData.ButtonClick);
        Destroy(gameObject);
    }
}
