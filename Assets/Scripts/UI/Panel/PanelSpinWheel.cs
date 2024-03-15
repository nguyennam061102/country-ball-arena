using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PanelSpinWheel : MonoBehaviour
{
    [SerializeField] private Transform Wheel;
    Transform PrizePosition;
    float spinSpeed;
    float baseSpeed = 600f;
    int phase = 0;
    [HideInInspector] public bool spinning = false;
    Vector3 toPointerDirection = Vector3.up;
    float maxSeed = 100;

    public List<SpinWheelItem> sList;
    public List<PrizeInfo> prizeList;

    [SerializeField] private GameObject watchToSpinButton, freeSpinButton, nextTimeButton, continueButton;
    [SerializeField] UILabel lbFreeSpin, lbWatVideoToSpin;

    private void Start()
    {
        GameController.Instance.gameplayCamera.enabled = false;
        GameController.Instance.MovementController.ShowIngameUI(false);

        for (int i = 0; i < sList.Count; i++)
        {
            sList[i].transform.SetEulerAnglesZAxis(i * -36f);
            sList[i].SetItem(i, prizeList[i].prizeSeed, prizeList[i].prizeAmount, prizeList[i].prizeIcon, this);
        }

        freeSpinButton.SetActive(true);
        watchToSpinButton.SetActive(false);
        nextTimeButton.SetActive(false);
        continueButton.SetActive(false);
        lbFreeSpin.text = "Free Spin";
        lbWatVideoToSpin.text = "Spin Again";
    }

    private void StartSpin()
    {
        if(spinning) return;
        continueButton.SetActive(false);
        var r = Random.Range(0f, maxSeed);
        //var r = 1; //todo: Test
        var prizeId = -1;
        if (r >= 25) prizeId = Random.Range(0, 2);
        if (r < 25 && r >= 15) prizeId = Random.Range(2, 4);
        if (r < 15 && r >= 5) prizeId = Random.Range(4, 6);
        if (r < 5 && r >= 4) prizeId = Random.Range(6, 8);
        if (r < 4 && r >= 1) prizeId = Random.Range(8, 10);
        if (prizeId < 0 || prizeId >= 10) prizeId = 0;
        StartCoroutine(SpinCoro(prizeId));
    }

    public void OnButtonFreeSpin()
    {
        if(spinning) return;
        Sound.Play(Sound.SoundData.ButtonClick);
        lbFreeSpin.text = "Spinning";
        StartSpin();
        GameData.FreeSpin = false;
    }

    public void OnButtonWatchToSpin()
    {
        if(spinning) return;
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            lbWatVideoToSpin.text = "Spinning";
            StartSpin();
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("freespin");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
        //if (Application.isEditor) StartSpin();
    }

    IEnumerator SpinCoro(int prizeId)
    {
        spinning = true;
        PrizePosition = sList[prizeId].prizePosition;
        baseSpeed = Random.Range(300f, 400f) * 1.5f;
        spinSpeed = baseSpeed;
        float totalAngle = Random.Range(4, 6) * 360f;
        float maxTotalAnglePhase2 = 2f; //avoid division by zero
        phase = 0;
        
        while (phase < 2 && spinning)
        {
            float value = spinSpeed * Time.fixedUnscaledDeltaTime;
            Wheel.Rotate(new Vector3(0, 0, value));
            totalAngle -= value;
            if (phase == 0)
            {
                if (totalAngle <= 0)
                {
                    phase = 1;
                    Vector3 cTarget = (PrizePosition.position - Wheel.position);
                    float angle = Mathf.Abs(Vector3.Angle(cTarget, toPointerDirection));
                    float crossZ = Vector3.Cross(cTarget, toPointerDirection).z;
                    if (angle < 0) angle += 360;
                    if (crossZ < 0) angle = 360 - angle;
                    if (angle < 180) angle += 360;
                    totalAngle = angle + Random.Range(-5, 5);
                    maxTotalAnglePhase2 = totalAngle;
                }
            }
            else
            {
                spinSpeed = baseSpeed * totalAngle / maxTotalAnglePhase2;
                if (spinSpeed < 2f || totalAngle <= 0)
                {
                    phase = 2;
                }
            }
            yield return new WaitForFixedUpdate();
        }
        TakePrize(prizeId);
        spinning = false;
        if (freeSpinButton.activeSelf)
        {
            freeSpinButton.SetActive(false);
            watchToSpinButton.SetActive(true);
            nextTimeButton.SetActive(false);
            continueButton.SetActive(true);
        }     
        else if (watchToSpinButton.activeSelf)
        {
            freeSpinButton.SetActive(false);
            watchToSpinButton.SetActive(false);
            nextTimeButton.SetActive(true);
            continueButton.SetActive(false);
        }
    }

    void TakePrize(int prizeId)
    {
        sList[prizeId].TakePrize();
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UiController.Instance.GetPanel(PanelName.PanelEndgame).GetComponent<PanelEndGame>().win = true;
    }

    [Header("REWARD")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private UI2DSprite rewardIcon;
    [SerializeField] private UILabel rewardText;

    public void ShowRewardPanel(Sprite rewardIcon, string rewardText, bool skin = false)
    {
        rewardPanel.SetActive(true);
        this.rewardIcon.sprite2D = rewardIcon;
        this.rewardIcon.MakePixelPerfect();
        this.rewardIcon.transform.localScale = skin ? new Vector3(2, 2, 2) : Vector3.one;
        this.rewardText.text = rewardText;
    }
    
    public void OnContinueButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        rewardPanel.SetActive(false);
    }
}

[Serializable]
public class PrizeInfo
{
    public int prizeSeed;
    public int prizeAmount;
    public Sprite prizeIcon;
}
