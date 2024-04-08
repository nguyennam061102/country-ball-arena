using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelRename : MonoBehaviour
{
    private UiController UI => UiController.Instance;

    [SerializeField] private UIGrid grid;
    [SerializeField] private List<PlayerItem> players;
    [SerializeField] private PlayerItem playerMain;
    [SerializeField] private int playerEye;
    [SerializeField] PlayerItem playerItem;
    [SerializeField] UIScrollView scroll;
    public UIInput InputName;
    public GameObject ButtonBack;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        if (GameData.IsFirstTimePlay)
        {
            ButtonBack.SetActive(false);
            InputName.label.text = "Player";
        }
        else
        {
            ButtonBack.SetActive(true);
        }
        playerEye = GameData.PlayerEye;
        index = GameFollowData.Instance.skinList.Count() % 2 == 0 ? GameFollowData.Instance.skinList.Count() / 2 - 1 : (GameFollowData.Instance.skinList.Count() - 1) / 2;
        for(int i = 0; i < GameFollowData.Instance.skinList.Count(); i++)
        {
            PlayerItem item = Instantiate(playerItem, grid.transform);
            item.transform.localScale = Vector3.one * 0.7f;
            if(i == GameData.CurrentSkinId)
            {
                item.SetItem(GameFollowData.Instance.skinList[index]);
            }
            else if (i == index)
            {
                item.SetItem(GameFollowData.Instance.skinList[GameData.CurrentSkinId]);
            }
            else
            {
                item.SetItem(GameFollowData.Instance.skinList[i]);
            }
            players.Add(item);
        }
        playerMain = players[index];
        playerMain.transform.localScale = Vector3.one;
        playerMain.SetEye(GameFollowData.Instance.eyes[playerEye]);
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (scroll.transform.localPosition.x > 210)
            {
                ScrollLeft(Mathf.RoundToInt((scroll.transform.localPosition.x - 210) / 400));
            }
            else if (scroll.transform.localPosition.x < 210)
            {
                ScrollRight(Mathf.RoundToInt((210 - scroll.transform.localPosition.x) / 400));
            }
        }
    }
    public void ScrollRight(int count)
    {
        for(int i = 0; i < count; i++)
        {
          
            PlayerItem firstItem = grid.transform.GetChild(0).GetComponent<PlayerItem>();
            firstItem.transform.SetAsLastSibling();
            players.Remove(firstItem);
            players.Add(firstItem);
            grid.Reposition();
        }

        playerMain.transform.localScale = Vector3.one * 0.7f;
        playerMain = players[index];
        playerMain.transform.localScale = Vector3.one;
        scroll.panel.clipOffset = new Vector2(0, 0);
        scroll.transform.localPosition = new Vector3(210, 50, 0);
    }
    public void ScrollLeft(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int indexs = grid.transform.childCount - 1;
            PlayerItem firstItem = grid.transform.GetChild(indexs).GetComponent<PlayerItem>();
            firstItem.transform.SetAsFirstSibling();
            players.Remove(firstItem);
            players.Insert(0,firstItem);
            grid.Reposition();
        }

        playerMain.transform.localScale = Vector3.one * 0.7f;
        playerMain = players[index];
        playerMain.transform.localScale = Vector3.one;
        scroll.panel.clipOffset = new Vector2(0, 0);
        scroll.transform.localPosition = new Vector3(210, 50, 0);
    }
    public void OnRename()
    {
        GameData.PlayerName = InputName.label.text;
        Sound.Play(Sound.SoundData.ButtonClick);
        GameData.CurrentSkinId = playerMain.id;
        if (GameData.IsFirstTimePlay)
        {
            MenuController.Instance.dailyReward.ShowDailyReward();
            GameData.IsFirstTimePlay = false;
        }
        else
        {
            UI.GetPanel(PanelName.PanelCharacter);
        }
    }
    public void OnChageEyeLeft()
    {
        if ( GameData.PlayerEye + 1 < GameFollowData.Instance.eyes.Count)
        {
            playerMain.SetEye(GameFollowData.Instance.eyes[GameData.PlayerEye + 1]);
            GameData.PlayerEye += 1;
        }
        else
        {
            GameData.PlayerEye = 0;
        }
    }
    public void OnChageEyeRight()
    {
        if (GameData.PlayerEye - 1 > 0)
        {
            playerMain.SetEye(GameFollowData.Instance.eyes[GameData.PlayerEye - 1]);
            GameData.PlayerEye -= 1;
        }
        else
        {
            GameData.PlayerEye = GameFollowData.Instance.eyes.Count - 1;
        }
    }
    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelCharacter);
    }

}
