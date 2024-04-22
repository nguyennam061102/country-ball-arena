using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Pathfinding;
using Sirenix.OdinInspector;
using System.Linq;

public class GameController : SingletonMonoBehavior<GameController>
{
    public MovementController MovementController => GetComponent<MovementController>();
    private MapController MapController => MapController.Instance;
    private PlayerManager PlayerManager => PlayerManager.Instance;
    private UiController UI => UiController.Instance;

    public Vector3 screenSize;

    public bool gamePaused, startGame;
    public Text countDownText;

    public List<Sprite> cardImageList;
    public AudioSource inGameMusic, counterAudio;
    public AudioClip countClip, fightClip;

    public Card[] cards;
    public List<int> playerCardsPicked, previousCardsAI1, previousCardsAI2;

    public ShowDamage showDamage;

    [Header("LEVEL INFO")]
    [SerializeField] private Image levelFill;
    [SerializeField] private Text levelText;
    [SerializeField] private Text mapTimerText, totalKillCountText, player1KillsText, player2KillsText, player3KillsText, killTargetText;
    [SerializeField] private Image player1Image, player2Image, player3Image;

    [SerializeField] GameObject groupLevel, groupBoss;
    [SerializeField] Image bossHPFill, bossIcon;
    [SerializeField] Text bossNameText, bossHP;

    bool isSpawnBoss = false;

    public int PlayerKillCount
    {
        get => PlayerPrefs.GetInt("KillCount", 0);
        set => PlayerPrefs.SetInt("KillCount", value);
    }

    public int ThisMatchKillCount;

    public Camera gameplayCamera;
    //MobilePostProcessing mobilePostProcessing;
    public SurvivalMode survivalMode;

    [SerializeField] Text gameEndRoundText;
    [SerializeField] TweenScale endRoundTextScale;
    [SerializeField] InputMovement playerInput;

    [SerializeField] Image[] PlayerPickedCards;
    [SerializeField] GameObject groupPickedCards;

    protected override void Awake()
    {
        gamePaused = true;
        startGame = true;
        GameUtils.CalculateScreenSizeInUnits();
        screenSize = GameUtils.screenSize;
        GameData.RoundLevel = 1;
        PlayerKillCount = 0;
        cardImageList = new List<Sprite>();
        playerCardsPicked = new List<int>();
        previousCardsAI1 = new List<int>();
        previousCardsAI2 = new List<int>();
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].cardID = i;
        }
        GameEventTrackerProVCL.Instance.OnPlayerPlayGameMode(GameFollowData.Instance.playingGameMode);

        survivalMode = new SurvivalMode();
        survivalMode.OnLevelPointChange += SurvivalMode_OnLevelPointChange;
    }

    private void SurvivalMode_OnLevelPointChange(float arg1, float arg2)
    {
        levelFill.fillAmount = arg1 / arg2;
    }

    private void Start()
    {
        //GameData.MatchLevel = 24;

        Loading.Instance.ShowBGLoad(false);
        CoinUi.Instance.currencyParent.SetActive(false);
        GameData.CanWatchRevive = true;
        SetAudio();
        GameData.onMusicChanged += SetAudio;
        SetLevelInfo();
        SetControlSide();
        SetGraphics();
        ThisMatchKillCount = 0;
        MapController.Instance.FirstTimeShowMap();
        PlayerKillScore = new List<int>();
        PlayerKillScore.Add(0);
        PlayerKillScore.Add(0);
        PlayerKillScore.Add(0);

        groupLevel.gameObject.SetActive(true);
        groupBoss.gameObject.SetActive(false);

        if (GameData.TutorialCompleted == 10)
        {
            MovementController.backbutton.SetActive(true);
            MovementController.emojibutton.SetActive(true);
            switch (GameFollowData.Instance.playingGameMode)
            {
                case GameMode.Survival:
                    MovementController.groupSurvival.SetActive(true);
                    MovementController.groupDM.SetActive(false);
                    MovementController.groupSandbox.SetActive(false);
                    groupPickedCards.SetActive(true);
                    break;
                case GameMode.DeathMatch:
                    MovementController.groupSurvival.SetActive(false);
                    MovementController.groupDM.SetActive(true);
                    MovementController.groupSandbox.SetActive(false);

                    totalKillCountText.text = "0/" + PlayerKillCountToWinGame;
                    killTargetText.text = "GET " + PlayerKillCountToWinGame + " KILLS TO WIN";
                    player1Image.sprite = GameFollowData.Instance.skinList[GameData.CurrentSkinId].itemIcon;
                    player2Image.sprite = GameFollowData.Instance.skinList[GameFollowData.Instance.Player2SkinID].itemIcon;
                    player3Image.sprite = GameFollowData.Instance.skinList[GameFollowData.Instance.Player3SkinID].itemIcon;
                    groupPickedCards.SetActive(true);
                    break;
                case GameMode.SandBox:
                    MovementController.groupSurvival.SetActive(false);
                    MovementController.groupDM.SetActive(false);
                    MovementController.groupSandbox.SetActive(true);
                    groupPickedCards.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            MovementController.backbutton.SetActive(false);
            MovementController.emojibutton.SetActive(false);
            MovementController.groupSurvival.SetActive(false);
            MovementController.groupDM.SetActive(false);
            MovementController.groupSandbox.SetActive(false);
            groupPickedCards.SetActive(false);
        }
    }
    float scanDelay = 0.2f;

    private void Update()
    {
        scanDelay -= TimeHandler.deltaTime;
        if (scanDelay <= 0f)
        {
            scanDelay = 1f;
            if (MapController.currentMap.IsThisMapPhysic()) AstarPath.active.Scan();
        }
    }

    float mapTimer = 0f;
    float baseMapTimer = 20f;

    private void FixedUpdate()
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch)) //free for all mode
        {
            if (!gamePaused)
            {
                mapTimer -= TimeHandler.deltaTime;
                if (mapTimer <= baseMapTimer * 0.5f)
                {
                    if (!callSpawnBox && !callingEndGame)
                    {
                        callSpawnBox = true;
                        MapController.SpawnWeaponCache();
                        ShowEndRoundText("Supply Box has arrived!");
                    }
                }
            }
        }
    }

    public void ShowPlayerPickedCards()
    {
        int pickedCount = playerCardsPicked.Count;
        for (int i = 0; i < PlayerPickedCards.Length; i++)
        {
            if (i < pickedCount)
            {
                PlayerPickedCards[i].enabled = true;
                PlayerPickedCards[i].sprite = Instance.cards[playerCardsPicked[i]].smallCardIcon;
            }
            else
            {
                PlayerPickedCards[i].enabled = false;
            }
        }
    }

    public void ApplyPlayerGunStatOnSpawn(Player player)
    {
        if (player.playerID == 0)
        {
            foreach (int i in playerCardsPicked)
            {
                cards[i].GetComponent<ApplyCardStats>().PickCard(player.playerID, false, PickerType.Player, true);
            }
        }
        else if (player.playerID == 1)
        {
            foreach (int i in previousCardsAI1)
            {
                cards[i].GetComponent<ApplyCardStats>().PickCard(player.playerID, false, PickerType.Player, true);
            }
        }
        else if (player.playerID == 2)
        {
            foreach (int i in previousCardsAI2)
            {
                cards[i].GetComponent<ApplyCardStats>().PickCard(player.playerID, false, PickerType.Player, true);
            }
        }
    }

    public void SetGraphics()
    {
        //if (mobilePostProcessing == null) mobilePostProcessing = gameplayCamera.GetComponent<MobilePostProcessing>();
        //mobilePostProcessing.enabled = GameData.GraphicsLevel == 1;
    }

    [SerializeField] GameObject controlLeft, controlRight;
    public void SetControlSide()
    {
        controlLeft.SetActive(GameData.ControlLeft);
        controlRight.SetActive(!GameData.ControlLeft);
    }

    void SetLevelInfo()
    {
        //levelText.text = $"Stage {GameData.MatchLevel}" + " (Round " + GameData.RoundLevel + "/" + TotalRound + ")";
        levelText.text = $"Level {GameData.MatchLevel}";
        levelFill.fillAmount = 0f;
        //levelFill.fillAmount = (float)(GameData.RoundLevel - 1) / TotalRound;
    }

    void SetAudio()
    {
        if (inGameMusic != null)
        {
            inGameMusic.mute = !GameData.Music;
        }
    }
    [SerializeField] PanelEmoji panelEmoji;
    public void OnEmojiButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        panelEmoji.gameObject.SetActive(true);
        panelEmoji.OnInit();
    }
    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UiController.Instance.GetPanel(PanelName.PanelPause);
    }

    public void ShowEndRoundText(string content)
    {
        Debug.Log(content);
        gameEndRoundText.text = content;
        endRoundTextScale.ResetToBeginning();
        endRoundTextScale.PlayForward();
    }

    public void StartRound()
    {
        if (GameData.TutorialCompleted == 2)
        {
            GameData.TutorialCompleted = 3;
            AgentBananaTalk.Instance.ShowPanelButtonIngame(() =>
            {
                AgentBananaTalk.Instance.HidePanel();
                if (startGame) startGame = false;
                StartCoroutine(StartRoundCoro());
            });
        }
        else
        {
            if (startGame) startGame = false;
            StartCoroutine(StartRoundCoro());
        }
    }

    IEnumerator StartRoundCoro()
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival) || GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
        {
            ShowPlayerPickedCards();
            mapTimer = baseMapTimer;
            mapTimerText.text = baseMapTimer + "";
            MovementController.ShowIngameUI(true);
            countDownText.gameObject.SetActive(true);
            counterAudio.PlayOneShot(countClip);
            countDownText.text = "3";
            countDownText.GetComponent<UITweener>().ResetToBeginning();
            countDownText.GetComponent<UITweener>().PlayForward();
            yield return new WaitForSeconds(.5f);
            counterAudio.PlayOneShot(countClip);
            countDownText.text = "2";
            countDownText.GetComponent<UITweener>().ResetToBeginning();
            countDownText.GetComponent<UITweener>().PlayForward();
            AstarPath.active.Scan();
            yield return new WaitForSeconds(.5f);
            counterAudio.PlayOneShot(countClip);
            countDownText.text = "1";
            countDownText.GetComponent<UITweener>().ResetToBeginning();
            countDownText.GetComponent<UITweener>().PlayForward();
            foreach (var player in PlayerManager.players)
            {
                player.Gun.gunAmmo.ResetAmmo();
            }
            yield return new WaitForSeconds(.5f);
            counterAudio.PlayOneShot(fightClip);
            countDownText.text = "Fight";
            countDownText.GetComponent<UITweener>().ResetToBeginning();
            countDownText.GetComponent<UITweener>().PlayForward();
            yield return new WaitForSeconds(.5f);
            countDownText.gameObject.SetActive(false);
            gamePaused = false;
            callOverTime = false;
            callingNextRound = false;
            callSpawnBox = false;
            for (int i = 0; i < PlayerManager.players.Count; i++)
            {
                PlayerManager.SetPlayersPlaying(true, i);
                PlayerManager.SetPlayersSimulated(true, i);
                PlayerManager.SetPlayersKinematic(false, i);
            }
            PlayerManager.SetPlayerRealTargetBasedOnStage(GameData.MatchLevel);
            PlayerManager.EnablePlayersCollider(true);
            PlayerManager.SetPlayerCanTakeDamage(true);
            Invoke("EnableAIPlayer", 0.5f);
            //load quang cao o day
            //SkygoBridge.instance.LoadInterAds();
            //inter
            //ApplovinBridge.instance.LoadInterAds();
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            mapTimer = baseMapTimer;
            mapTimerText.text = baseMapTimer + "";
            MovementController.ShowIngameUI(true);
            foreach (var player in PlayerManager.players)
            {
                player.Gun.gunAmmo.ResetAmmo();
            }
            counterAudio.PlayOneShot(fightClip);
            countDownText.text = "Start Simulation!";
            countDownText.GetComponent<UITweener>().ResetToBeginning();
            countDownText.GetComponent<UITweener>().PlayForward();
            yield return new WaitForSeconds(.5f);
            countDownText.gameObject.SetActive(false);
            gamePaused = false;
            callOverTime = false;
            callingNextRound = false;
            callSpawnBox = false;
            for (int i = 0; i < PlayerManager.players.Count; i++)
            {
                PlayerManager.SetPlayersPlaying(true, i);
                PlayerManager.SetPlayersSimulated(true, i);
                PlayerManager.SetPlayersKinematic(false, i);
            }
            PlayerManager.SetPlayerRealTargetBasedOnStage(GameData.MatchLevel);
            PlayerManager.EnablePlayersCollider(true);
            PlayerManager.SetPlayerCanTakeDamage(true);
            Invoke("EnableAIPlayer", 0.5f);

            if (GameData.ExplainSandbox == 0)
            {
                GameData.ExplainSandbox = 5;
                AgentBananaTalk.Instance.ShowAgentGuide("Hey! Welcome to Sandbox Mode.\nYou can test Card Combo in this game mode." +
                    "\nTap \"Edit Cards\" button at the bottom of screen to start." +
                    "\nTap icon to show Card - Tap Card or Icon again to confirm PICK.", "Okay", () =>
                {
                    AgentBananaTalk.Instance.HidePanel();
                });
            }
        }
    }

    void EnableAIPlayer()
    {
        foreach (Player p in PlayerManager.GetPlayersNotInID(0))
        {
            p.GetComponent<PlayerAI>().EnableMovement(true);
        }
    }

    public void NextRound()
    {
        playerInput.ResetAllButtonsState();
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival))
        {
            if (survivalMode.IsCompleted())
            {
                if (GameData.MatchLevel % 4 == 0)
                {
                    if (isSpawnBoss)
                    {
                        foreach (ProjectileHit ph in FindObjectsOfType<ProjectileHit>()) Destroy(ph.gameObject);
                        PlayerManager.SetPlayersCollider(false);
                        PlayerManager.SetPlayerCanTakeDamage(false);
                        EndGame();
                    }
                    else SpawnBoss();
                }
                else
                {
                    foreach (ProjectileHit ph in FindObjectsOfType<ProjectileHit>()) Destroy(ph.gameObject);
                    PlayerManager.SetPlayersCollider(false);
                    PlayerManager.SetPlayerCanTakeDamage(false);
                    EndGame();
                }              
            }
            else
            {
                StartCoroutine(NextRoundCoro());
            }
            TimeHandler.instance.DoSlowDown();
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
        {
            foreach (ProjectileHit ph in FindObjectsOfType<ProjectileHit>()) Destroy(ph.gameObject);
            PlayerManager.SetPlayersCollider(false);
            PlayerManager.SetPlayerCanTakeDamage(false);

            StartCoroutine(NextRoundCoro());
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            //do nothing
        }
    }

    Player playertobecomeboss;

    void SpawnBoss()
    {
        isSpawnBoss = true;
        groupLevel.gameObject.SetActive(false);
        groupBoss.gameObject.SetActive(false);
        //heal player and free damage + hp regen
        PlayerManager.Instance.GetPlayerWithID(0).healthHandler.FullyRestore();

        //cards[13].GetComponent<ApplyCardStats>().PickCard(0, false, PickerType.Player, false);
        //cards[7].GetComponent<ApplyCardStats>().PickCard(0, false, PickerType.Player, false);

        //reset bot to become boss
        playertobecomeboss = PlayerManager.Instance.GetPlayerWithID(2);
        int bossSkinID = playertobecomeboss.skinHandler.SetBossSkin();
        playertobecomeboss.ResetPlayerStats();
        playertobecomeboss.BossSetHealth();
        playertobecomeboss.holding.GetGunAndSpawn();
        bossNameText.text = "KILL BOSS: " + GameFollowData.Instance.skinList[bossSkinID].itemName;
        bossIcon.sprite = GameFollowData.Instance.skinList[bossSkinID].itemIcon;
        bossIcon.SetNativeSize();
        cards[14].GetComponent<ApplyCardStats>().PickCard(2, false, PickerType.Player, false);
        cards[14].GetComponent<ApplyCardStats>().PickCard(2, false, PickerType.Player, false);
        cards[14].GetComponent<ApplyCardStats>().PickCard(2, false, PickerType.Player, false);
        cards[GameFollowData.Instance.skinList[bossSkinID].signatureCard].GetComponent<ApplyCardStats>().PickCard(2, false, PickerType.Player, false);
        if (GameData.RoundLevel > 4)
        {
            int rndCard = 0;
            for (int i = 4; i < GameData.RoundLevel; i++)
            {
                rndCard = Random.Range(0, cards.Length);
                cards[rndCard].GetComponent<ApplyCardStats>().PickCard(2, false, PickerType.Player, false);
            }
        }
        ShowEndRoundText(string.Format(bossFightTextList[Random.Range(0, bossFightTextList.Length)], GameFollowData.Instance.skinList[bossSkinID].itemName));
        playertobecomeboss.healthHandler.SimpleDieToRespawnAsBoss();

        StartCoroutine(WaitAndSpawnBoss(1f));
    }

    IEnumerator WaitAndSpawnBoss(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerRespawnAtSpawnPosition(2);
        groupLevel.gameObject.SetActive(false);
        groupBoss.gameObject.SetActive(true);
        StartCoroutine(UpHealthBarCoro(playertobecomeboss.maxHealth));
    }

    void ShowBossHP()
    {
        bossHPFill.fillAmount = playertobecomeboss.health / playertobecomeboss.maxHealth;
        bossHP.text = (int)(Mathf.Clamp(playertobecomeboss.health, 0, playertobecomeboss.maxHealth)) + "/" + (int)playertobecomeboss.maxHealth;
    }

    IEnumerator UpHealthBarCoro(float maxHP)
    {
        bossHPFill.fillAmount = 0;
        bossHP.text = "0/" + (int)maxHP;
        float t = 0;
        while (t < 1)
        {
            t += Mathf.Clamp01(TimeHandler.deltaTime / 2.1f);
            bossHPFill.fillAmount = t;
            bossHP.text = (int)(t * maxHP) + "/" + (int)maxHP;
            yield return new WaitForSecondsRealtime(TimeHandler.deltaTime);
        }
        playertobecomeboss.healthHandler.onHealthChange += ShowBossHP;
    }

    IEnumerator NextRoundCoro()
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.Survival))
        {
            DailyMission.Instance.dailyMissionList[3].MissionProgress++;
            ShowEndRoundText(winTextList[Random.Range(0, winTextList.Length)]);
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.DeathMatch))
        {
            ShowEndRoundText(IsPlayerLeading() ? nextWinTextList[Random.Range(0, nextWinTextList.Length)] : nextLoseTextList[Random.Range(0, nextLoseTextList.Length)]);
        }
        else if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            //do nothing
        }
        yield return new WaitForSeconds(1.5f);
        gamePaused = true;
        MapController.currentMap.HideMap();
        GameEventTrackerProVCL.Instance.OnPlayerCompleteRound();
    }

    public void EndGame(bool win = true, float timeShowEndPanel = 3f)
    {
        StartCoroutine(EndGameCoro(win, timeShowEndPanel));
    }

    IEnumerator EndGameCoro(bool win, float timeShowEndPanel)
    {
        playerInput.ResetAllButtonsState();
        if (GameData.CanWatchRevive && !win && GameData.TutorialCompleted == 10)
        {
            yield return new WaitForSeconds(TimeHandler.instance.gameOverTime);
            UI.GetPanel(PanelName.PanelContinue);
        }
        else
        {
            ShowEndRoundText(!win ? loseTextList[Random.Range(0, loseTextList.Length)] : victoryTextList[Random.Range(0, victoryTextList.Length)]);
            yield return new WaitForSeconds(timeShowEndPanel);
            GameEventTrackerProVCL.Instance.OnPlayerCompleteMatch();
            if (win && GameData.TutorialCompleted == 10)
            {
                DailyMission.Instance.dailyMissionList[2].MissionProgress++;
                GameData.MatchLevel++;
            }
            if (GameData.TutorialCompleted == 3)
            {
                GameData.TutorialCompleted = 10;
                GameData.Gold += 4000;
                GameData.Diamond += 10;
                MovementController.ShowIngameUI(false);

                AgentBananaTalk.Instance.ShowGroupReward();
                if (win)
                {
                    AgentBananaTalk.Instance.ShowAgentGuide("Hey, you are good! Here's your reward.\n Use this to buy and upgrade weapons for fight!", "Tutorial Completed", () =>
                    {
                        AgentBananaTalk.Instance.HidePanel();
                        Loading.Instance.LoadScene("menu");
                    });
                }
                else
                {
                    AgentBananaTalk.Instance.ShowAgentGuide("You need to practice more. Take my gift.\n Use this to buy and upgrade weapons for fight!", "Tutorial Completed", () =>
                    {
                        AgentBananaTalk.Instance.HidePanel();
                        Loading.Instance.LoadScene("menu");
                    });
                }

                SkygoBridge.instance.LogEvent("tutorial_completed");
            }
            else
            {
                gamePaused = true;
                CoinUi.Instance.currencyParent.SetActive(true);
                if (win)
                {
                    UI.GetPanel(PanelName.PanelSpin);
                }
                else UI.GetPanel(PanelName.PanelEndgame).GetComponent<PanelEndGame>().win = false;
            }
        }
    }

    //Death Match
    List<int> PlayerKillScore;
    int PlayerKillCountToWinGame
    {
        get
        {
            return 10 + GameData.PlayerLevel;
        }
    }
    bool callingEndGame = false;
    bool callingNextRound = false;
    bool callOverTime = false;
    bool callSpawnBox = false;
    int winnerID = -1;

    public bool IsPlayerWin()
    {
        return winnerID == 0;
    }

    int GetBiggestValue()
    {
        int result = 0;
        foreach (int val in PlayerKillScore)
        {
            if (val > result) result = val;
        }
        return result;
    }

    public bool IsPlayerLeading()
    {
        return PlayerKillScore[0] >= GetBiggestValue();
    }

    public int GetPlayerKillScore(int id)
    {
        return PlayerKillScore[id];
    }

    public void PlayerDies(Player caller, Player killer) // only sandbox and deathmatch
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox))
        {
            PlayerRespawnAtSpawnPosition(caller.playerID);
        }
        else
        {
            if (killer != null && killer != caller)
            {
                if (killer.playerID < PlayerKillScore.Count) PlayerKillScore[killer.playerID]++;
            }

            if (killer != null && killer != caller && PlayerKillScore[killer.playerID] >= PlayerKillCountToWinGame)
            {
                if (!callingEndGame)
                {
                    callingEndGame = true;
                    winnerID = killer.playerID;
                    EndGameDeathMatch();
                }
            }
            else
            {
                if (mapTimer <= 0)
                {
                    if (PlayerManager.IsMaxOneSurvive() || caller.playerID == 0)
                    {
                        if (!callingNextRound)
                        {
                            callingNextRound = true;
                            NextRound();
                        }
                    }
                }
                else MoveToSpawnPosAndRespawn(caller);
            }
            player1KillsText.text = GetPlayerKillScore(0) + "";
            player2KillsText.text = GetPlayerKillScore(1) + "";
            player3KillsText.text = GetPlayerKillScore(2) + "";
        }
    }

    public void PlayerRespawnAtSpawnPosition(int pid)
    {
        Vector3 posToRespawn = MapController.Instance.GetSpawnPoints()[pid].localStartPos;
        PlayerManager.MoveOnePlayerAndRespawn(pid, posToRespawn);
    }

    public void MoveToSpawnPosAndRespawn(Player p)
    {
        Vector3 posToRespawn = MapController.Instance.currentMap.GetRespawnPosition();
        PlayerManager.MoveOnePlayerAndRespawn(p.playerID, posToRespawn);
    }

    void EndGameDeathMatch(float timeShowEndPanel = 3f)
    {
        StartCoroutine(EndGameDMCoro(IsPlayerWin(), timeShowEndPanel));
    }

    IEnumerator EndGameDMCoro(bool playerWin, float timeShowEndPanel)
    {
        PlayerManager.SetPlayersKinematic(true, 0);
        PlayerManager.SetPlayersKinematic(true, 1);
        PlayerManager.SetPlayersKinematic(true, 2);
        PlayerManager.SetPlayersSimulated(false, 0);
        PlayerManager.SetPlayersSimulated(false, 1);
        PlayerManager.SetPlayersSimulated(false, 2);
        PlayerManager.SetPlayersPlaying(false, 0);
        PlayerManager.SetPlayersPlaying(false, 1);
        PlayerManager.SetPlayersPlaying(false, 2);
        PlayerManager.SetPlayerCanTakeDamage(false);

        ShowEndRoundText(playerWin ? loseTextList[Random.Range(0, loseTextList.Length)] : winTextList[Random.Range(0, winTextList.Length)]);
        TimeHandler.instance.DoSlowDown();
        yield return new WaitForSeconds(timeShowEndPanel);
        GameEventTrackerProVCL.Instance.OnPlayerCompleteMatch();
        CoinUi.Instance.currencyParent.SetActive(true);
        UI.GetPanel(PanelName.PanelDeathMatchEnd);
    }

    public int TotalRound
    {
        get
        {
            var tmp = GameData.MatchLevel;
            if (tmp < 2) return 1 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 5) return 2 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 9) return 3 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 14) return 4 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 22) return 5 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 35) return 6 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 56) return 7 + GameEventTrackerProVCL.Instance.AdditionRounds;
            if (tmp < 90) return 8 + GameEventTrackerProVCL.Instance.AdditionRounds;
            return 10 + GameEventTrackerProVCL.Instance.AdditionRounds;
        }
    }

    public float AIDamageMultiplier
    {
        get
        {
            if (TotalRound == 0) return 0.6f;
            if (TotalRound == 1) return 0.7f;
            if (TotalRound == 2) return 0.8f;
            if (TotalRound == 3) return 0.9f;
            if (TotalRound == 4) return 1f;
            if (TotalRound == 5) return 1f;
            if (TotalRound == 6) return 1f;
            if (TotalRound == 7) return 1.05f;
            if (TotalRound == 8) return 1.1f;
            if (TotalRound == 9) return 1.15f;
            return 1.2f;
        }
    }

    private readonly string[] victoryTextList = new[] { "YOU WIN!", "VICTORY!", "MISSION COMPLETED!" };
    private readonly string[] bossFightTextList = new[] { "{0} IS ON THE HUNT!", "{0} COMES TO KILL YOU!" };
    private readonly string[] winTextList = new[] { "Too Easy", "REKT", "Next! Please", "Dominating", "GG EZ", "Go Go Go", "Ha ha ha" };
    private readonly string[] loseTextList = new[] { "You Noob!", "Too Hard for you", "You Failed", "Mission Failed" };
    private readonly string[] nextWinTextList = new[] { "You are leading! Keep it up!", "Good! Keep Pressure!", "Just another easy day", "Push Foward!", "You are winning" };
    private readonly string[] nextLoseTextList = new[] { "Try harder next", "Next Round", "Are they smurfing?", "You can make it! Kill them all!", "Be careful next time" };
}

public class SurvivalMode
{
    float startPoint = 100f;
    float killReward = 100f;
    float assistReward = 40f;

    private float currentLevelPoint = 0;
    public float CurrentLevelPoint
    {
        get
        {
            return currentLevelPoint;
        }
        private set
        {
            currentLevelPoint = value;
            if (OnLevelPointChange != null) OnLevelPointChange(currentLevelPoint, GetLevelTarget());
        }
    }
    public event Action<float, float> OnLevelPointChange;

    public float GetLevelTarget()
    {
        return startPoint * Mathf.Pow(1.1f, GameData.MatchLevel - 1);
    }

    public void EnemyDie(bool kill)
    {
        CurrentLevelPoint += kill ? killReward : assistReward;
    }

    public bool IsCompleted()
    {
        return CurrentLevelPoint >= GetLevelTarget();
    }
}
