using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class Card : MonoBehaviour
{
    private PanelSelectCard SelectCard;
    private GameController gameController => GameController.Instance;

    public CardInfo cardInfo;

    [TextArea]
    public string cardDescription;

    public UILabel cardName, cardContent;
    public UIButton pickButton, pickOneMoreCardButton;

    private bool pick;
    public GameObject iconCheckOff, iconCheckOn;

    public int cardTier;
    public int cardID;
    public Sprite smallCardIcon;

    public bool isThisCardSelected = false;
    public GameObject vienxanh;

    public void SetPanelCard(PanelSelectCard psc)
    {
        SelectCard = psc;
    }

    private void Start()
    {
        GetComponent<UI2DSprite>().color = new Color32(255, 255, 255, 255);
        cardName.text = ToFormattedText(cardInfo);
        cardContent.text = cardDescription;

        pickButton.gameObject.SetActive(false);
        pickOneMoreCardButton.gameObject.SetActive(false);
        if (vienxanh != null) vienxanh.gameObject.SetActive(false);
    }

    public void ClickToSelectCard()
    {
        Sound.Play(Sound.SoundData.CardSelected);
        if (isThisCardSelected || SelectCard.cardPickedCount >= SelectCard.cardsToPick) return;

        SelectCard.cardPickedCount += 1;
        isThisCardSelected = true;
        if (vienxanh != null) vienxanh.gameObject.SetActive(true);

        iconCheckOn.SetActive(true);
        iconCheckOn.GetComponent<TweenAlpha>().PlayForward();
        iconCheckOn.GetComponent<TweenScale>().PlayForward();
        //pickButton.gameObject.SetActive(false);
        gameController.cardImageList.Add(smallCardIcon);
        gameController.playerCardsPicked.Add(cardID);

        if (SelectCard.cardPickedCount >= SelectCard.cardsToPick)
        {
            foreach (var card in SelectCard.cardList)
            {
                //Debug.Log(card.name);
                if (!card.isThisCardSelected)
                {
                    card.GetComponent<UI2DSprite>().color = new Color32(100, 100, 100, 255);
                }
                card.GetComponent<BoxCollider>().enabled = false;
            }
        }

        DailyMission.Instance.dailyMissionList[1].MissionProgress++;
        if (GameData.TutorialCompleted == 1)
        {
            GameData.TutorialCompleted = 2;
            AgentBananaTalk.Instance.ShowAgentGuide("Now tap CONTINUE to start fight.", "I got it!", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }
        GetComponent<ApplyCardStats>().Pick();
        SelectCard.OnPlayerPickedOneCard();
    }

    public void PickOneMoreCard()
    {
        Sound.Play(Sound.SoundData.CardSelected);
        var onDone = new UnityEvent();
        onDone.AddListener(() =>
        {
            ClickToSelectCard();
            GetComponent<ApplyCardStats>().Pick();
            pickOneMoreCardButton.gameObject.SetActive(false);
            DailyMission.Instance.dailyMissionList[4].MissionProgress++;
            GameEventTrackerProVCL.Instance.OnPlayerWatchVideoToReward("getmorecard");
        });
        onDone.Invoke();
        //reward
        //ApplovinBridge.instance.ShowRewarAdsApplovin(onDone, null);
    }

    private string ToFormattedText(CardInfo value)
    {
        var stringVal = value.ToString();
        var bld = new StringBuilder();

        for (var i = 0; i < stringVal.Length; i++)
        {
            if (char.IsUpper(stringVal[i]))
            {
                bld.Append(" ");
            }

            bld.Append(stringVal[i]);
        }

        return bld.ToString();
    }
}

public enum CardInfo
{
    Bounce,
    Fighter,
    BurstFire,
    Damage,
    Hunter,
    IcyBullet,
    FastHand,
    Bash,
    KeepItCool,
    KeepItHot,
    BloodPower,
    BreakThrough,
    Echo,
    Buff,
    Collide,
    GlassCanon,
    NatureGrow,
    Hp,
    Vampire,
    Immortal,
    Giant,
    Velocity,
    Thirst,
    Push,
    LandMine,
    HailOfFire,
    Heal
}

