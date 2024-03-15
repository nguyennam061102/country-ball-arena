using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUi : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    
    [Header("GOLD & DIAMOND")] 
    [SerializeField] private UILabel goldText;
    [SerializeField] private UILabel diamondText;
    [SerializeField] private TweenScale goldTs, diamondTs;
    public GameObject currencyParent;
    public float currentGold, currentDiamond, totalGold, totalDiamond;
    
    private static CoinUi mInstance;

    public static CoinUi Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = CoinUi.Instance;
            }

            return mInstance;
        }
    }

    float speed = 0.08f;
    int updateGoldAmount = 0;
    int updateDiamondAmount = 0;

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }
    
    private void Start()
    {
        UpdateGoldAndText();
        GameData.OnGoldChanged += UpdateGold;
        GameData.OnDiamondChanged += UpdateDiamond;
    }

    private void Update()
    {
        if (goldText != null) goldText.text = currentGold.ToString("0");
        if (updateGold)
        {
            if (addGold)
            {
                currentGold += updateGoldAmount;
                if (currentGold >= totalGold)
                {
                    currentGold = totalGold;
                    updateGold = false;
                }
            }
            else
            {
                currentGold -= updateGoldAmount;
                if (currentGold <= totalGold)
                {
                    currentGold = totalGold;
                    updateGold = false;
                }
            }
        }
        
        if (diamondText != null) diamondText.text = currentDiamond.ToString("0");
        if (updateDiamond)
        {
            if (addDiamond)
            {
                currentDiamond += updateDiamondAmount;
                if (currentDiamond >= totalDiamond)
                {
                    currentDiamond = totalDiamond;
                    updateDiamond = false;
                }
            }
            else
            {
                currentDiamond -= updateDiamondAmount;
                if (currentDiamond <= totalDiamond)
                {
                    currentDiamond = totalDiamond;
                    updateDiamond = false;
                }
            }
        }
    }

    public void UpdateGoldAndText()
    {
        currentGold = GameData.Gold;
        currentDiamond = GameData.Diamond;
    }

    public bool updateGold, updateDiamond, addGold, addDiamond;
    void UpdateGold(int goldAmount, bool add)
    {
        addGold = add;
        totalGold = goldAmount;
        updateGoldAmount = (int)(Mathf.Abs(totalGold - currentGold) * speed);
        if (updateGoldAmount < 1) updateGoldAmount = 1;
        updateGold = true;
    }

    void UpdateDiamond(int diamondAmount, bool add)
    {
        addDiamond = add;
        totalDiamond = diamondAmount;
        updateDiamondAmount = (int)(Mathf.Abs(totalDiamond - currentDiamond) * speed);
        if (updateDiamondAmount < 1) updateDiamondAmount = 1;
        updateDiamond = true;
    }

    public void Alert(bool gold = true)
    {
        StartCoroutine(AlertCoro(gold ? goldTs : diamondTs));
    }

    private IEnumerator AlertCoro(UITweener ts)
    {
        ts.PlayForward();
        yield return new WaitForSeconds(ts.duration);
        ts.PlayReverse();
    }

    public void OnGemButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelShop).GetComponent<PanelShop>().OnDiamondButton();
    }

    public void OnGoldButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelShop).GetComponent<PanelShop>().OnGoldButton();
    }
}
