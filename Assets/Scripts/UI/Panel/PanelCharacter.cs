using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PanelCharacter : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    private PanelInventory PanelInventory => UI.GetPanel(PanelName.PanelInventory).GetComponent<PanelInventory>();
    private GameFollowData GfData => GameFollowData.Instance;
    private CharacterStats Stats => CharacterStats.Instance;

    [Header("TITLE")]
    [SerializeField] private TitleType titleType;
    [SerializeField] private GameObject characterTitle;
    [SerializeField] private GameObject loadOutTitle;
    [SerializeField] private GameObject startButton;

    [Header("PLAYER LEVEL")]
    [SerializeField] private float currentExp;
    [SerializeField] private float targetExp;
    [SerializeField] private UI2DSprite fillExpBar;
    [SerializeField] private UILabel level, expProgress, namePlayer;

    [Header("CHARACTER")]
    [SerializeField] private SkeletonAnimation characterAnim;
    [SerializeField] private SpriteRenderer offHand;
    [SerializeField] private SpriteRenderer eye;
    [SerializeField] private Sprite[] offHandList;
    [SerializeField] GameObject[] mainHand;
    [SerializeField] private GameObject mainHandWaring, offHandWarning, skinWarning;

    [Space]
    [SerializeField] private CharacterSkin[] skinList;
    [SerializeField] private UILabel hpText, damageText, matchLevelText;

    public bool IsPanelCharacter
    {
        get => PlayerPrefs.GetInt("IsPanelCharacter", 1) == 1;
        set => PlayerPrefs.SetInt("IsPanelCharacter", value ? 1 : 0);
    }

    private void OnEnable()
    {
        GetLevelInfo();
        CheckWarning();
        GetCharacterInfo();
        OnMainHandButton();
    }

    void CheckWarning()
    {
        mainHandWaring.SetActive(GameData.MainHandWarning > 0);
        mainHandWaring.GetComponentInChildren<UILabel>().text = GameData.MainHandWarning + "";
        offHandWarning.SetActive(GameData.OffHandWarning > 0);
        offHandWarning.GetComponentInChildren<UILabel>().text = GameData.OffHandWarning + "";
        skinWarning.SetActive(GameData.SkinWarning > 0);
        skinWarning.GetComponentInChildren<UILabel>().text = GameData.SkinWarning + "";
    }

    private void Start()
    {
        SetCharacterAnim();
        characterTitle.SetActive(titleType == TitleType.Character);
        loadOutTitle.SetActive(titleType == TitleType.LoadOut);
        startButton.SetActive(titleType == TitleType.LoadOut);
        matchLevelText.text = $"Stage {GameData.MatchLevel}";
    }

    public void ReshowAfterUpgradeOrSelect()
    {
        SetCharacterAnim();
        GetCharacterInfo();
        foreach (InventoryItem ii in itemList) ii.CheckItemAgain();
    }

    public void SetTitleType(TitleType type)
    {
        titleType = type;
        IsPanelCharacter = type == TitleType.Character;
        if (type.Equals(TitleType.Character)) UI.panelCharacterTitle = 0;
        else if (type.Equals(TitleType.LoadOut)) UI.panelCharacterTitle = 1;
    }

    public void SetCharacterAnim()
    {
        characterAnim.skeleton.SetSkin(GameFollowData.Instance.skinList[ GameData.CurrentSkinId].connectedSkin.skinName);
        characterAnim.Skeleton.SetSlotsToSetupPose();
        characterAnim.AnimationState.Apply(characterAnim.Skeleton);
        var tmp = Random.Range(0, 2);
        switch (tmp)
        {
            case 0:
                characterAnim.state.SetAnimation(0, "0. Idle", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
            case 1:
                characterAnim.state.SetAnimation(0, "0. Idle", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
        }

        //mainHand.sprite = mainHandList[GameData.CurrentMainHandId];
        //mainHand.transform.localScale = new Vector3(1.5f, 1.5f);
        //mainHand.transform.SetEulerAnglesZAxis(45f);
        foreach (GameObject go in mainHand) go.SetActive(false);
        mainHand[GameData.CurrentMainHandId].gameObject.SetActive(true);
        eye.sprite = GameFollowData.Instance.skinList[GameData.CurrentSkinId].connectedSkin.eye;
        offHand.sprite = offHandList[GameData.CurrentOffHandId];
        //offHand.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        offHand.transform.SetEulerAnglesZAxis(20);
        offHand.transform.localRotation = Quaternion.Euler(0, 0, 20f);
    }

    private void GetLevelInfo()
    {
        if (GameData.PlayerLevel == 80)
        {
            fillExpBar.fillAmount = 1;
            return;
        }

        currentExp = GameData.CurrentExp;
        targetExp = TargetExp;

        fillExpBar.fillAmount = currentExp / targetExp;
        level.text = GameData.PlayerLevel.ToString();
        expProgress.text = $"{currentExp:0}/{targetExp:0}";
    }

    private void GetCharacterInfo()
    {
        namePlayer.text = GameData.PlayerName;
        hpText.text = $"{Mathf.Round(Stats.GetPlayerHealthBasedOnManyThings(skinList[GameData.CurrentSkinId], GameData.PlayerLevel, GfData.skinList[GameData.CurrentSkinId].ItemLevel))}";
        damageText.text = $"{Mathf.Round(Stats.GetPlayerDamageBasedOnManyThings(GfData.mainHandList[GameData.CurrentMainHandId].damage, GfData.mainHandList[GameData.CurrentMainHandId].ItemLevel))}";
    }
    public void OnRenameButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelRename);
    }
    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelMenu);
    }

    [SerializeField] private UIGrid inventoryGrid;
    [SerializeField] private InventoryItem inventoryItemPrefab;
    private List<InventoryItem> itemList;
    [SerializeField] UI2DSprite[] buttonSpr;
    [SerializeField] private Sprite on, off;

    void ClearItemListAndCreateNew()
    {
        if (itemList != null) foreach (InventoryItem ii in itemList) Destroy(ii.gameObject);
        itemList = new List<InventoryItem>();
    }
    public void OnSubmitName()
    {
        GameData.PlayerName = inputName.label.text;
    }
    public void OnMainHandButton()
    {
        ClearItemListAndCreateNew();
        if (GameData.MainHandWarning > 0) GameData.MainHandWarning = 0;
        CheckWarning();
        foreach (var info in GameFollowData.Instance.mainHandList)
        {
            if (info.IsUnlocked)
            {
                var item = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
                itemList.Add(item);
                item.SetItemInfo(info, this, true);
                item.transform.localScale = Vector3.one * 0.9f;
            }
        }
        Invoke("RepositionGrid", 0.05f);
        SetButton(0);
        //inventoryGrid.GetComponent<TweenScale>().ResetToBeginning();
        //inventoryGrid.GetComponent<TweenScale>().PlayForward();
    }

    public void OnOffHandButton()
    {
        ClearItemListAndCreateNew();
        if (GameData.OffHandWarning > 0) GameData.OffHandWarning = 0;
        CheckWarning();
        foreach (var info in GameFollowData.Instance.offHandList)
        {
            if (info.IsUnlocked)
            {
                var item = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
                itemList.Add(item);
                item.SetItemInfo(info, this, false);
                item.transform.localScale = Vector3.one * 0.9f;
            }
        }
        Invoke("RepositionGrid", 0.05f);
        SetButton(1);
        //inventoryGrid.GetComponent<TweenScale>().ResetToBeginning();
        //inventoryGrid.GetComponent<TweenScale>().PlayForward();
    }

    public void OnSkinButton()
    {
        ClearItemListAndCreateNew();
        if (GameData.SkinWarning > 0) GameData.SkinWarning = 0;
        CheckWarning();
        foreach (var info in GameFollowData.Instance.skinList)
        {
            if (info.IsUnlocked)
            {
                var item = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
                itemList.Add(item);
                item.SetItemInfo(info, this, false);
                item.transform.localScale = Vector3.one * 0.9f;
            }
        }
        Invoke("RepositionGrid", 0.05f);
        SetButton(2);
        //inventoryGrid.GetComponent<TweenScale>().ResetToBeginning();
        //inventoryGrid.GetComponent<TweenScale>().PlayForward();
    }

    void SetButton(int id)
    {
        for (int i = 0; i < buttonSpr.Length; i++)
        {
            buttonSpr[i].sprite2D = i == id ? on : off;
            buttonSpr[i].MakePixelPerfect();
        }
    }
    [SerializeField]
    private UIScrollView UIScrollView;
    void RepositionGrid()
    {
        UIScrollView.ResetPosition();
        inventoryGrid.Reposition();
        inventoryGrid.GetComponent<TweenScale>().ResetToBeginning();
        inventoryGrid.GetComponent<TweenScale>().PlayForward();
        //grid.Reposition();
        //grid.GetComponent<TweenScale>().ResetToBeginning();
        //grid.GetComponent<TweenScale>().PlayForward();
    }

    private float TargetExp => (float)Math.Round(60 * Mathf.Pow(GameData.PlayerLevel + 1, 2.8f) - 60);

    public List<InventoryItem> ItemList { get => itemList; set => itemList = value; }

    public void OnPlayButton()
    {
        GameFollowData.Instance.playingGameMode = GameMode.Survival;
        Loading.Instance.LoadScene("game");
        Destroy(gameObject);
    }

    public PanelInventory panelInventoryData;
    public UIInput inputName;
}

public enum TitleType
{
    Character,
    LoadOut
}
