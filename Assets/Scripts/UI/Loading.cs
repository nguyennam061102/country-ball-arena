using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Loading : DontDestroy<Loading>
{
    private UiController UI => UiController.Instance;

    [SerializeField] private GameObject bgLoading, bgAskTutorial;

    [SerializeField] private UI2DSprite fillLoading;
    [SerializeField] private UILabel loadingText, tipText;
    AsyncOperation asyncLoad;
    bool loading = false;
    float progress = 0f;

    protected override void Awake()
    {
        base.Awake();
        //PlayerPrefs.DeleteAll();
        if (GameData.MatchLevel > 1) GameData.TutorialCompleted = 10;
        if (GameData.TutorialCompleted > 0) GameData.TutorialCompleted = 10;     
    }

    private void Start()
    {
        GameData.ExplainMenu = true;
        GameData.ExplainCardStack = true;
        GameData.ExplainSandbox = 5;
        //if (GameData.TutorialCompleted == 10)
        //{
        //    LoadScene("menu");
        //}
        //else //start tutorial
        //{
        //    AskTutorial();
        //}
        if (GameData.TutorialCompleted != 10)
        {
            GameData.TutorialCompleted = 10;
            GameData.Gold += 3000;
            GameData.Diamond += 10;
        }
        LoadScene("menu");
    }

    public void ShowBGLoad(bool flag)
    {
        bgLoading.SetActive(flag);
    }

    private void Update()
    {
        if (loading)
        {
            FillTween();
        }
    }

    public void AskTutorial()
    {
        bgLoading.SetActive(false);
        bgAskTutorial.SetActive(true);
    }

    public void PlayTutorial()
    {
        DailyMission.Instance.FakeSetDailyMission();
        LoadScene("game");
    }

    public void SkipTutorial()
    {
        GameData.TutorialCompleted = 10;
        LoadScene("menu");
    }

    public void LoadScene(string sceneName)
    {
        bgLoading.SetActive(true);
        bgAskTutorial.SetActive(false);

        StartCoroutine(LoadYourAsyncScene(sceneName));
        ChangeTip();
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        loading = true;
        progress = 0f;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void ActiveScene()
    {
        //Debug.Log("hihi");
        asyncLoad.allowSceneActivation = true;
        loading = false;
    }

    void FillTween()
    {
        progress += Time.fixedDeltaTime;
        //Debug.Log(asyncLoad.progress);
        if (asyncLoad.progress < 0.9f)
        {
            progress = Mathf.Clamp(progress, 0f, 0.9f);
        }
        else
        {
            progress = Mathf.Clamp01(progress);
            if (progress >= 1f)
            {
                ActiveScene();
            }
        }
        fillLoading.fillAmount = progress;
        loadingText.text = $"Loading... {progress * 100f:0}%";
    }

    public void ChangeTip()
    {
        tipText.text = alltips[Random.Range(0, alltips.Length)];
    }

    string[] alltips ={
        "Tip: Consider lower your Graphics under Setting menu for better Frame Rate and smoother gameplay",
        "Tip: Remember to upgrade your weapons and off-hand skills",
        "Tip: You get 1 free Spin Wheel every day, don't forget it",
        "Tip: On 3-Players Arena, last-player-standing wins, so prepare your own strategy",
        "Tip: You can get free weapons/upgrades when complete matchs",
        "Tip: 1 Kill = 1 Diamond, simple and just like that",
        "Tip: Daily rewards are good for you, let's play everyday!",
        "Tip: Enemy also have their own cards too, be careful",
        "Tip: New Capsule players joins each Round, smile now?",
        "Tip: Sometimes you stuck in a wall... Dev advice: don't worry, don't panic, just spam JUMP button",
        "Tip: Your Mobile is too fast that you can't even read this loading tip",
        "Tip: Try sandbox mode to test and prepare for real fight",
        "Tip: Free weapons and weapon upgrades are provided on Survival Mode",
        "Tip: Death Match: Kill and respawn, simple",
        "Tip: Remember to give us your review about our game, it's always useful"
    };
}