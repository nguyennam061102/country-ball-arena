using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PanelSelectCard : MonoBehaviour
{
    private MovementController Mc => GameController.Instance.MovementController;
    private MapController Map => MapController.Instance;
    private UiController UI => UiController.Instance;
    private GameController gameController => GameController.Instance;

    private List<int> tmp;
    [SerializeField] private UIGrid ThreeCardsParent, EightCardsParents;
    public List<Card> cardList;

    public UIButton continueButton;
    public int cardPickedCount;
    List<int> cardIdsToShow;
    public int cardsToPick = 0;
    [SerializeField] UILabel lbNumPickCard;

    private void Awake()
    {
        Mc.ShowIngameUI(false);
        gameController.gameplayCamera.enabled = false;
        cardPickedCount = 0;
    }

    private void Start()
    {
        if (GameData.TutorialCompleted == 0)
        {
            ShowFixed3Card();
            GameData.TutorialCompleted = 1;
            AgentBananaTalk.Instance.ShowAgentGuide("Welcome to CAPSULE! Recruit!\nI'm Agent Banana. I will guide you.\n\nBefore start each Round, you have to pick a Card.\nEach Card has its own function, choose carefully.", "Interesting", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }
        //else
        //{
        //    show cards here
        //    ShowRandomCard();
        //}
        if (!GameData.ExplainCardStack && gameController.playerCardsPicked.Count >= 1)
        {
            GameData.ExplainCardStack = true;
            AgentBananaTalk.Instance.ShowAgentGuide("New Card can stack with Cards from previous Rounds.\nChoose Card that fits your playstyle.", "Great", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }
        continueButton.gameObject.SetActive(false);
    }

    public void StartShowCard(int numCards, int cardsToPick)
    {
        this.cardsToPick = cardsToPick;
        cardIdsToShow = new List<int>();
        if (numCards == 3)
        {
            if (GameData.PlayerLevel < 3)
            {
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(GetCardWithTier(1));
            }
            else if (GameData.PlayerLevel < 6)
            {
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(Random.Range(-1f, 1f) < -0.25f ? GetCardWithTier(2) : GetCardWithTier(1));
            }
            else if (GameData.PlayerLevel < 9)
            {
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(Random.Range(-1f, 1f) < -0.25f ? GetCardWithTier(2) : GetCardWithTier(1));
                cardIdsToShow.Add(Random.Range(-1f, 1f) < -0.25f ? GetCardWithTier(2) : GetCardWithTier(1));
            }
            else
            {
                cardIdsToShow.Add(GetCardWithTier(1));
                cardIdsToShow.Add(Random.Range(-1f, 1f) < -0.25f ? GetCardWithTier(2) : GetCardWithTier(1));
                cardIdsToShow.Add(Random.Range(-1f, 1f) < -0.15f ? GetCardWithTier(3) : GetCardWithTier(2));
            }
            lbNumPickCard.text = "Pick 1 more card";
            Show3Cards();
        }
        else if (numCards == 8)
        {
            cardIdsToShow.Add(GetCardWithTier(1));
            cardIdsToShow.Add(GetCardWithTier(1));
            cardIdsToShow.Add(GetCardWithTier(1));
            cardIdsToShow.Add(GetCardWithTier(2));
            cardIdsToShow.Add(GetCardWithTier(2));
            cardIdsToShow.Add(GetCardWithTier(2));
            cardIdsToShow.Add(GetCardWithTier(3));
            cardIdsToShow.Add(GetCardWithTier(3));
            lbNumPickCard.text = string.Format("Pick {0} cards to build your COMBO ({1}/{2})", cardsToPick, cardPickedCount, cardsToPick);
            Show8Cards();
        }
        else
        {
            for (int i = 0; i < numCards; i++)
            {
                cardIdsToShow.Add(GetCardWithTier(1));
            }
            Show8Cards();
        }

    }

    void ShowFixed3Card()
    {
        cardIdsToShow = new List<int> { 14, 7, 5 };
        GameUtils.Shuffle(cardIdsToShow);
        Show3Cards();
    }

    void Show3Cards()
    {
        GameUtils.Shuffle(cardIdsToShow);
        for (int i = 0; i < cardIdsToShow.Count; i++)
        {
            var card = Instantiate(GameController.Instance.cards[cardIdsToShow[i]], ThreeCardsParent.transform);
            card.SetPanelCard(this);
            cardList.Add(card);
        }
        ThreeCardsParent.Reposition();
    }

    void Show8Cards()
    {
        GameUtils.Shuffle(cardIdsToShow);
        for (int i = 0; i < cardIdsToShow.Count; i++)
        {
            var card = Instantiate(GameController.Instance.cards[cardIdsToShow[i]], EightCardsParents.transform);
            card.transform.localScale = Vector3.one * 0.6f;
            card.SetPanelCard(this);
            cardList.Add(card);
        }
        EightCardsParents.Reposition();
    }

    int GetCardWithTier(int tier)
    {
        List<int> sameTierCard = new List<int>();
        for (int i = 0; i < GameController.Instance.cards.Length; i++)
        {
            if (GameController.Instance.cards[i].cardTier == tier)
            {
                if (cardIdsToShow == null) sameTierCard.Add(i);
                else
                {
                    if (!cardIdsToShow.Contains(i)) sameTierCard.Add(i);
                }
            }
        }
        return sameTierCard[Random.Range(0, sameTierCard.Count)];
    }

    public void OnContinueButton()
    {
        UnityEvent onCloseInterEvent = new UnityEvent();
        onCloseInterEvent.AddListener(() =>
        {
            gameController.gameplayCamera.enabled = true;
            if (gameController.startGame) Map.currentMap.ShowMap();
            else GameController.Instance.StartRound();
            Destroy(gameObject);
        });
        onCloseInterEvent.Invoke();

        //inter
        bool canshowad = GameEventTrackerProVCL.Instance.AllowSelectCardAds && gameController.playerCardsPicked.Count > 1 /*&& (SkygoBridge.instance.CanShowAd == 1 && ApplovinBridge.instance.CanShowAd == 1)*/;
        //inter
        //if (canshowad)
        //{
        //    //inter
        //    //bool flag = ApplovinBridge.instance.ShowInterAdsApplovin(onCloseInterEvent);
        //    //if (!flag) onCloseInterEvent.Invoke();
        //}
        //else onCloseInterEvent.Invoke();
    }

    public void OnPlayerPickedOneCard()
    {       
        if (cardsToPick == 1)
        {
            lbNumPickCard.text = "Pick 1 more card";
        }
        else
        {
            lbNumPickCard.text = string.Format("Pick {0} cards to build your COMBO ({1}/{2})", cardsToPick, cardPickedCount, cardsToPick);
        }

        if (cardPickedCount >= cardsToPick) StartCoroutine(cContinue());
    }

    IEnumerator cContinue()
    {
        yield return new WaitForSecondsRealtime(0.75f);
        OnContinueButton();
    }
}
