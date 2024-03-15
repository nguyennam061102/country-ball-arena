using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using Game;

public class SkygoBridge : MonoBehaviour
{
    public static SkygoBridge instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    public int CanShowAd
    {
        get
        {
            return PlayerPrefs.GetInt("CanShowAd", 1);
        }
        set
        {
            PlayerPrefs.SetInt("CanShowAd", value);
        }
    }
    public float TimeShowAOA
    {
        get
        {
            return PlayerPrefs.GetFloat("TimeShowAOA", 10f);
        }
        set
        {
            PlayerPrefs.SetFloat("TimeShowAOA", value);
        }
    }
    public float ConfirmAds
    {
        get
        {
            return PlayerPrefs.GetFloat("CornFirmAds", 1f);
        }
        set
        {
            PlayerPrefs.SetFloat("CornFirmAds", value);
        }
    }
    float lastTimeShowAd = 0;
    public float TimeShowAds
    {
        get
        {
            return PlayerPrefs.GetFloat("TimeShowAds", 45f);
        }
        set
        {
            PlayerPrefs.SetFloat("TimeShowAds", value);
        }
    }
    public bool isForRecording()
    {
        return false;
    }

    public bool isForADs()
    {
        return false;
    }
    //[SerializeField] AdsManager ads;
    //[SerializeField] AnalyticsManager analytics;
    //[SerializeField] InAppPurchaseManager iap;
    //[SerializeField] PromotionManager promo;
    //[SerializeField] GDPRScript gdpr;

    //public PromotionManager Crosspromotion
    //{
    //    get
    //    {
    //        return this.promo;
    //    }
    //}

    private void Start()
    {
        Application.targetFrameRate = 60;
        //gdpr.CallGDPR();
    }

//    #region ADS
//    public void ShowBanner()
//    {
//        if (CanShowAd == 1)
//        {
//            ads.ShowBanner();
//        }
//    }

//    public void HideBanner()
//    {
//        ads.HideBanner();
//    }
//    int NumPlay = 0;
//    public bool ShowInterstitial(UnityEvent onClose)
//    {
//        NumPlay++;
//        if (GameEventTrackerProVCL.Instance.NumPlayToShowAds != 0 && NumPlay % GameEventTrackerProVCL.Instance.NumPlayToShowAds != 0) return false;
//#if UNITY_EDITOR
//        Debug.Log("Time: " + Time.time);
//#endif
//        if (CanShowAd == 1)
//        {
//            if (Time.time - lastTimeShowAd >= TimeShowAds)
//            {
//                bool flag = ads.ShowInterstial(onClose);
//                if (flag) lastTimeShowAd = Time.time;
//                return flag;
//            }
//            else return false;
//        }
//        else return false;
//    }

//    public bool ShowRewarded(UnityEvent onCompleted, UnityEvent onFailed)
//    {
//        //PlayerPersistentData.instance.RewardedLevelPlayToday++;
//        return ads.ShowRewarded(onCompleted, onFailed);
//    }

//    public bool ShowRewardedInterstitial(UnityEvent onSuccess, UnityEvent onFailed)
//    {
//        return true;
//    }
//    public void LoadInterAds()
//    {
//        if (Time.time < 30f) return;
//        bool callLoad = false;
//        if (GameEventTrackerProVCL.Instance.NumPlayToShowAds == 0 || GameEventTrackerProVCL.Instance.NumPlayToShowAds == 1) callLoad = true;
//        else if (NumPlay % GameEventTrackerProVCL.Instance.NumPlayToShowAds == (GameEventTrackerProVCL.Instance.NumPlayToShowAds - 1)) callLoad = true;
//        if (callLoad)
//        {
//            Debug.Log("Load inter ads here");
//            ads.LoadInterstitial();
//        }
//    }
//    #endregion

    #region IAP
    public void PurchaseIAP(string sku, UnityEvent onSuccess)
    {
        //iap.BuyProduct(sku, onSuccess);
    }

    public void RestorePurchase()
    {
        //iap.RestorePurchases();
    }
    #endregion

    #region Analytics
    public void LogEvent(string eventName)
    {
        //analytics.LogGameEvent(eventName);
    }
    #endregion

    #region Config
    public string GetConfig(string cfgName)
    {
        //return analytics.GetConfigData(cfgName);
        return "";
    }
    #endregion

    #region Social
    public void RateGame()
    {
        Application.OpenURL("market://details?id=com.skygo.roguelite.capsule");
    }

    public void ShareGame()
    {

    }
    #endregion

    #region Promotion
    //public void SetCrosspromotionPosition(int positionID)
    //{
    //    //promo.SetPosition((CrossPromotionPosition)positionID);
    //}

    //public void ShowPromotion()
    //{
    //    //promo.ShowPromotion();
    //}

    //public void HidePromotion()
    //{
    //    //promo.HidePromotion();
    //}
    #endregion
}
