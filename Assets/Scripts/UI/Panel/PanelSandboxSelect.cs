using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PanelSandboxSelect : MonoBehaviour
{
    [SerializeField] GameObject GroupPickCard, GroupCardEdit, viensang;
    [SerializeField] Image[] AllCardsToPick;
    [SerializeField] Image[] PlayerPickedCards;
    [SerializeField] Text cardName, cardDesc, playerCardPickedCount;
    [SerializeField] Image CardImageShow;
    [SerializeField] int showingCardID = -1;
    [SerializeField] CameraFollowPlayer camFollowPlayer;

    [SerializeField] IconFlyToPosition[] iconmover;
    float duration = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < AllCardsToPick.Length; i++)
        {
            AllCardsToPick[i].sprite = GameController.Instance.cards[i].smallCardIcon;
        }
        ShowPlayerPickedCards();
        GroupPickCard.SetActive(false);
        GroupCardEdit.SetActive(true);
        camFollowPlayer.enabled = true;
        foreach (IconFlyToPosition icm in iconmover)
        {
            icm.StopJump();
            icm.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        }
    }

    public void ShowPickCardSandbox()
    {
        GroupPickCard.SetActive(true);
        GroupCardEdit.SetActive(false);
        CardImageShow.gameObject.SetActive(showingCardID != -1);
        GameController.Instance.MovementController.movementGo.SetActive(false);
        camFollowPlayer.StartFollow(PlayerManager.Instance.GetPlayerWithID(0).transform);

        if (GameData.ExplainSandbox == 1)
        {
            GameData.ExplainSandbox = 2;
            AgentBananaTalk.Instance.ShowAgentGuide("Tap small Icon to show Cards Description.", "Next", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }
    }

    public void ClickToShowCard(Transform t)
    {
        int id = t.GetSiblingIndex();
        if (id == showingCardID)
        {
            PickCard(true);
        }
        else ShowCard(id);
        viensang.gameObject.SetActive(true);
        viensang.transform.parent = t;
        viensang.transform.localPosition = Vector3.zero;
    }

    public void ShowCard(int cardID)
    {
        cardName.text = ToFormattedText(GameController.Instance.cards[cardID].cardInfo);
        cardDesc.text = GameController.Instance.cards[cardID].cardDescription;
        CardImageShow.sprite = GameController.Instance.cards[cardID].GetComponent<UI2DSprite>().sprite2D;
        CardImageShow.gameObject.SetActive(true);
        showingCardID = cardID;
        CardImageShow.GetComponent<TweenScale>().ResetToBeginning();
        CardImageShow.GetComponent<TweenScale>().PlayForward();

        if (GameData.ExplainSandbox == 2)
        {
            GameData.ExplainSandbox = 3;
            AgentBananaTalk.Instance.ShowAgentGuide("See that Card on the right of screen?\nTap that CARD or same small ICON again to confirm PICK card.", "Simple", () =>
            {
                AgentBananaTalk.Instance.HidePanel();
            });
        }
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

    void ShowPlayerPickedCards()
    {
        int pickedCount = GameController.Instance.playerCardsPicked.Count;
        for (int i = 0; i < PlayerPickedCards.Length; i++)
        {
            if (i < pickedCount)
            {
                PlayerPickedCards[i].sprite = GameController.Instance.cards[GameController.Instance.playerCardsPicked[i]].smallCardIcon;
                PlayerPickedCards[i].color = new Color32(255, 255, 255, 255);
            }
            else
            {
                PlayerPickedCards[i].color = new Color32(255, 255, 255, 0);
            }
        }
        playerCardPickedCount.text = "Player's Cards (" + GameController.Instance.playerCardsPicked.Count + "/10)";
    }

    public void PickCard(bool fromIcon)
    {
        if (GameController.Instance.playerCardsPicked.Count >= 10) return;
        GameController.Instance.playerCardsPicked.Add(showingCardID);
        GameController.Instance.cards[showingCardID].GetComponent<ApplyCardStats>().PickCard(0, false, PickerType.Player, false);
        GameController.Instance.cardImageList.Add(GameController.Instance.cards[showingCardID].smallCardIcon);

        IconFlyToPosition icm = iconmover[GameController.Instance.playerCardsPicked.Count - 1];
        Vector3 startPos = fromIcon ? AllCardsToPick[showingCardID].transform.position : CardImageShow.transform.position;
        icm.Fly(startPos, PlayerPickedCards[GameController.Instance.playerCardsPicked.Count - 1].transform.position, 100f, 120f, duration, 0f,
            () =>
            {
                ShowPlayerPickedCards();
                icm.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
            });
        icm.GetComponent<TweenRotation>().duration = duration;
        icm.GetComponent<TweenRotation>().to = new Vector3(0, 0, 720);
        icm.GetComponent<TweenRotation>().ResetToBeginning();
        icm.GetComponent<TweenRotation>().PlayForward();

        icm.GetComponent<TweenScale>().duration = duration;
        icm.GetComponent<TweenScale>().from = Vector3.one;
        icm.GetComponent<TweenScale>().to = Vector3.one * 0.625f;
        icm.GetComponent<TweenScale>().ResetToBeginning();
        icm.GetComponent<TweenScale>().PlayForward();
        icm.GetComponent<Image>().sprite = AllCardsToPick[showingCardID].sprite;
        icm.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        //ShowPlayerPickedCards();
        if (GameData.ExplainSandbox == 3)
        {
            GameData.ExplainSandbox = 4;
            AgentBananaTalk.Instance.ShowAgentGuide("Your PICKED cards list is on the left of screen.\nYou only can pick maximum of 10 cards.", "Next", () =>
            {
                AgentBananaTalk.Instance.ShowAgentGuide("Keep PICKING more cards, when you are done, tap \"FIGHT\" button to test cards combo", "I got it!", () =>
                {
                    AgentBananaTalk.Instance.HidePanel();
                });
            });
        }
    }

    public void OnClosePanel()
    {
        GroupPickCard.SetActive(false);
        GroupCardEdit.SetActive(true);
        GameController.Instance.MovementController.movementGo.SetActive(true);
        camFollowPlayer.StopFollow();
        if (GameData.ExplainSandbox == 4)
        {
            GameData.ExplainSandbox = 5;
            AgentBananaTalk.Instance.ShowAgentGuide("You can Edit Cards again to test other combo.\nJust tap \"Remove All Cards\" and select new cards.", "Next", () =>
            {
                AgentBananaTalk.Instance.ShowAgentGuide("That's all for now. Have FUN!", "Thanks", () =>
                {
                    AgentBananaTalk.Instance.HidePanel();
                });
            });
        }
    }

    public void RemoveAllCards()
    {
        for (int i = 0; i < GameController.Instance.playerCardsPicked.Count; i++)
        {
            IconFlyToPosition icm = iconmover[i];
            icm.Fly(PlayerPickedCards[i].transform.position, AllCardsToPick[GameController.Instance.playerCardsPicked[i]].transform.position, 100f, 120f, duration, 0f,
                () =>
                {
                    //ShowPlayerPickedCards();
                    icm.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                });
            icm.GetComponent<TweenRotation>().duration = duration;
            icm.GetComponent<TweenRotation>().to = new Vector3(0, 0, 720);
            icm.GetComponent<TweenRotation>().ResetToBeginning();
            icm.GetComponent<TweenRotation>().PlayForward();

            icm.GetComponent<TweenScale>().duration = duration;
            icm.GetComponent<TweenScale>().from = Vector3.one * 0.625f;
            icm.GetComponent<TweenScale>().to = Vector3.one;
            icm.GetComponent<TweenScale>().ResetToBeginning();
            icm.GetComponent<TweenScale>().PlayForward();

            icm.GetComponent<Image>().sprite = AllCardsToPick[GameController.Instance.playerCardsPicked[i]].sprite;
            icm.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        GameController.Instance.playerCardsPicked.Clear();
        GameController.Instance.cardImageList.Clear();
        Player player = PlayerManager.Instance.GetPlayerWithID(0);
        player.ResetPlayerStats();
        //re create
        player.SetHealthStats();
        player.holding.GetGunAndSpawn();
        GameController.Instance.GetComponent<MovementController>().SetPlayerBlock(player);
        ShowPlayerPickedCards();
    }
}
