using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class PanelMenu : MonoBehaviour
{
    private UiController UI => UiController.Instance;

    [SerializeField] private TweenAlpha logoTransition;
    [SerializeField] private GameObject logoAnim, logo, noAdsButton;
    [SerializeField] private float timeIn, timeOut;
    [SerializeField] UILabel lbSurvival, lbDeathMatch, lbSandbox;
    [SerializeField] GameObject effectDeathMatchOn, effectDeathMatchOff;
    [SerializeField] GameObject effectSandboxOn, effectSandboxOff;

    [Header("WARNING")]
    public GameObject shopWarning;
    public GameObject characterWarning;
    public GameObject spinWarning;
    public GameObject missionWarning;

    private void Start()
    {
        //purchase
        //noAdsButton.SetActive(SkygoBridge.instance.CanShowAd != 0 && ApplovinBridge.instance.CanShowAd != 0);
        PlayLogo();
        CheckWarning();
        if (!GameData.ExplainMenu)
        {
            GameData.ExplainMenu = true;
            AgentBananaTalk.Instance.ShowAgentGuide("There are 3 Game Modes: Survival, Death Match and Sandbox.\nFeel free to look around.", "Cool", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }

        lbSurvival.text = "Stage " + GameData.MatchLevel;
        lbDeathMatch.text = GameData.PlayerLevel >= 4 ? "DEATH MATCH" : "Player Level 4 to unlock";
        effectDeathMatchOn.SetActive(GameData.PlayerLevel >= 4);
        effectDeathMatchOff.SetActive(GameData.PlayerLevel < 4);

        lbSandbox.text = GameData.PlayerLevel >= 4 ? "SANDBOX" : "Player Level 4 to unlock";
        effectSandboxOn.SetActive(GameData.PlayerLevel >= 4);
        effectSandboxOff.SetActive(GameData.PlayerLevel < 4);
    }

    void PlayLogo()
    {
        if (!MenuController.Instance.logoPlayed)
        {
            MenuController.Instance.logoPlayed = true;
            StartCoroutine(LogoExecution());
        }
        else
        {
            //logoAnim.SetActive(false);
            logo.SetActive(true);
        }
    }

    void CheckWarning()
    {
        shopWarning.SetActive(GameData.ShopWarning);
        int sumWarning = GameData.MainHandWarning + GameData.OffHandWarning + GameData.SkinWarning;
        characterWarning.SetActive(sumWarning > 0);
        characterWarning.GetComponentInChildren<UILabel>().text = sumWarning + "";

        spinWarning.SetActive(GameData.FreeUpgradeTalent > 0);
        spinWarning.GetComponentInChildren<UILabel>().text = GameData.FreeUpgradeTalent + "";
        int canClaimMissionCount = 0;
        //Debug.Log(DailyMission.Instance.myTodayMissions.Count);
        foreach (var mission in DailyMission.Instance.myTodayMissions)
        {
            if (mission.MissionDone && !mission.MissionClaimed) canClaimMissionCount++;
        }
        missionWarning.SetActive(canClaimMissionCount > 0);
        missionWarning.GetComponentInChildren<UILabel>().text = canClaimMissionCount + "";
    }

    private IEnumerator LogoExecution()
    {
        yield return new WaitForSeconds(timeIn);
        //logoTransition.PlayForward();
        yield return new WaitForSeconds(timeOut);
        //logoTransition.PlayReverse();
        //logoAnim.SetActive(false);
        logo.SetActive(true);
    }

    public void OnSettingButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelMenuSetting);
    }

    public void OnShopButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelShop);
    }

    public void OnCharacterButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.Character);
    }

    public void OnSpinButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelSpin);
    }

    public void OnTalentButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelTalent);
    }

    public void OnDailyMissionButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.DailyMission);
    }

    public void OnNoAdsButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UnityEvent uEvent = new UnityEvent();
        uEvent.AddListener(
            () =>
            {
                //SkygoBridge.instance.CanShowAd = 0;
                //
                //ApplovinBridge.instance.CanShowAd = 0;
                //noAdsButton.SetActive(ApplovinBridge.instance.CanShowAd != 0 && SkygoBridge.instance.CanShowAd != 0);
            });
        //purchase
        //SkygoBridge.instance.PurchaseIAP("skygo_capsule_noads_299", uEvent);
    }

    public void OnSurvivalButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        //UI.GetPanel(PanelName.PanelCharacter).GetComponent<PanelCharacter>().SetTitleType(TitleType.LoadOut);
        GameFollowData.Instance.playingGameMode = GameMode.Survival;
        Loading.Instance.LoadScene("game");
        Destroy(gameObject);
    }

    public void OnDeathMatchButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.PlayerLevel >= 4) UI.GetPanel(PanelName.PanelDeathMatchStart);
    }

    public void OnSandBoxButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.PlayerLevel >= 4)
        {
            GameFollowData.Instance.playingGameMode = GameMode.SandBox;
            Loading.Instance.LoadScene("game");
            Destroy(gameObject);
        }
    }
}