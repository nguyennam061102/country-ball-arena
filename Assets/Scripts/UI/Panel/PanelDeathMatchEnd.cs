using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelDeathMatchEnd : MonoBehaviour
{
    [SerializeField] GroupPlayerShow[] playersShow;
    int goldEarn = 0;
    int diamondEarn = 0;
    [SerializeField] UILabel lbGold, lbDamond, lbNextLevel;
    [SerializeField] GameObject winO, loseO;

    [SerializeField] private float currentExp, targetExp, expReward, tmpExp;
    [SerializeField] private UILabel expRewardText, expProgress, levelText;
    [SerializeField] private UI2DSprite fillExp;
    private bool updateExp;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance.IsPlayerWin()) GameData.PlayerDeathMatchWinCount++;

        winO.SetActive(GameController.Instance.IsPlayerWin());
        loseO.SetActive(!GameController.Instance.IsPlayerWin());
        int playerKillScore = GameController.Instance.GetPlayerKillScore(0);

        playersShow[0].ShowPlayer(playerKillScore.ToString() + " Kills",
            GameData.CurrentSkinId, GameData.CurrentMainHandId, GameData.CurrentOffHandId);
        playersShow[1].ShowPlayer(GameController.Instance.GetPlayerKillScore(1).ToString() + " Kills",
            GameFollowData.Instance.Player2SkinID, GameFollowData.Instance.Player2WeaponID, GameFollowData.Instance.Player2OffhandID);
        playersShow[2].ShowPlayer(GameController.Instance.GetPlayerKillScore(2).ToString() + " Kills",
            GameFollowData.Instance.Player3SkinID, GameFollowData.Instance.Player3WeaponID, GameFollowData.Instance.Player3OffhandID);

        goldEarn = (int)(Mathf.Round(playerKillScore * 400 * Mathf.Pow(1.1f, GameData.MatchLevel - 1) * (1 + CharacterStats.Instance.TalentGoldBonus)) * GameEventTrackerProVCL.Instance.CoinMultipleAmount);
        diamondEarn = playerKillScore;
        lbGold.text = "+" + goldEarn;
        lbDamond.text = "+" + diamondEarn;

        GameData.Gold += goldEarn;
        GameData.Diamond += diamondEarn;

        GameData.PlayerTotalKillDeathMatch += playerKillScore;
        if (playerKillScore > GameData.PlayerBestKillDeathMatch) GameData.PlayerBestKillDeathMatch = playerKillScore;

        GameController.Instance.gameplayCamera.enabled = false;
        //GameController.Instance.MovementController.movementGo.SetActive(false);
        GameController.Instance.MovementController.ShowIngameUI(false);

        expReward = GetExp();
        expRewardText.text = $"EXP: +{expReward}";
        levelText.text = GameData.PlayerLevel.ToString();
        currentExp = GameData.CurrentExp;
        targetExp = GetTargetExp;
        fillExp.fillAmount = currentExp / targetExp;
        expProgress.text = $"{currentExp}/{targetExp}";
        tmpExp = currentExp + expReward;
        updateExp = true;

        lbNextLevel.text = "Play Again";
        
        //SkygoBridge.instance.ShowInterstitial(null);
    }

    private void Update()
    {
        if (GameData.PlayerLevel == 80)
        {
            fillExp.fillAmount = 1;
            return;
        }
        currentExp = Mathf.Round(currentExp);
        expProgress.text = $"{currentExp}/{targetExp}";

        if (!updateExp) return;

        currentExp += (targetExp / 2) * Time.fixedDeltaTime;
        fillExp.fillAmount += 0.5f * Time.fixedDeltaTime;

        if (currentExp >= targetExp)
        {
            updateExp = false;
            GameData.PlayerLevel++;
            GameData.FreeUpgradeTalent += 3;
            GameData.Diamond += 5;
            GameEventTrackerProVCL.Instance.OnPlayerLevelUp();
            levelText.text = GameData.PlayerLevel.ToString();
            tmpExp -= targetExp;
            currentExp = 0;
            fillExp.fillAmount = 0;
            targetExp = GetTargetExp;
            updateExp = true;
        }

        if (currentExp >= tmpExp)
        {
            currentExp = tmpExp;
            GameData.CurrentExp = currentExp;
            updateExp = false;
        }
    }

    private float GetTargetExp => (float)Mathf.Round(60 * Mathf.Pow(GameData.PlayerLevel + 1, 2.8f) - 60);
    private float GetBaseExp()
    {
        if (GameController.Instance.GetPlayerKillScore(0) > 0) return 60 * GameController.Instance.GetPlayerKillScore(0);
        return 40;
    }

    private float GetExp()
    {
        return Mathf.Round(GetBaseExp() * (1 + CharacterStats.Instance.TalentExp));
    }

    void Onx2Button()
    {
        UnityEvent onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            GameData.Gold += goldEarn;
            GameData.Diamond += diamondEarn;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("endgamedeathmatch");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    public void OnContinueButton()
    {        
        UnityEvent onCloseInterEvent = new UnityEvent();
        onCloseInterEvent.AddListener(() =>
        {
            GameFollowData.Instance.IsChangeSceneFromGameplay = true;
            Loading.Instance.LoadScene("menu");
            Destroy(gameObject);
        });
        onCloseInterEvent.Invoke();
        //inter
        //bool flag = ApplovinBridge.instance.ShowInterAdsApplovin(onCloseInterEvent);
        //if (!flag) onCloseInterEvent.Invoke();
    }

    public void OnHomeButton()
    {
        UnityEvent onCloseInterEvent = new UnityEvent();
        onCloseInterEvent.AddListener(() =>
        {
            Loading.Instance.LoadScene("menu");
            Destroy(gameObject);
        });
        onCloseInterEvent.Invoke();
        //inter
        //bool flag = ApplovinBridge.instance.ShowInterAdsApplovin(onCloseInterEvent);
        //if (!flag) onCloseInterEvent.Invoke();
    }
}
