using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//GDPR
//using AppLovinMax.Internal.API;

public class PanelSetting : MonoBehaviour
{

    [SerializeField] private UILabel soundText, musicText, controlText, graphicsText;
    [SerializeField] private UI2DSprite sprSound, sprMusic, sprGraphics;

    private void OnEnable()
    {
        soundText.text = GameData.Sound ? "On" : "Off";
        musicText.text = GameData.Music ? "On" : "Off";
        controlText.text = GameData.ControlLeft ? "Left" : "Right";
        graphicsText.text = GameData.GraphicsLevel == 0 ? "Low" : "High";
        SetButtonColor();
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UiController.Instance.GetPanel(PanelName.PanelMenu);
        CheckConfigStatus();
    }

    public void OnSoundButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        GameData.Sound = !GameData.Sound;
        soundText.text = GameData.Sound ? "On" : "Off";
        SetButtonColor();
    }

    public void OnMusicButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        GameData.Music = !GameData.Music;
        musicText.text = GameData.Music ? "On" : "Off";
        SetButtonColor();
    }

    public void OnControlButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        GameData.ControlLeft = !GameData.ControlLeft;
        controlText.text = GameData.ControlLeft ? "Left" : "Right";
    }

    public void OnGraphicsButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.GraphicsLevel == 1) GameData.GraphicsLevel = 0;
        else if(GameData.GraphicsLevel == 0) GameData.GraphicsLevel = 1;
        graphicsText.text = GameData.GraphicsLevel == 0 ? "Low" : "High";
        SetButtonColor();
    }

    public void OnRestorePurchase()
    {
        //purchase
        //SkygoBridge.instance.RestorePurchase();
        //LoadAndShowCmpFlow();
        //print("Restore purchase click");
    }

    void SetButtonColor()
    {
        sprSound.color = GameData.Sound ? new Color32(50, 184, 81, 255) : new Color32(128, 132, 134, 255);
        sprMusic.color = GameData.Music ? new Color32(50, 184, 81, 255) : new Color32(128, 132, 134, 255);
        sprGraphics.color = GameData.GraphicsLevel == 0 ? new Color32(50, 184, 81, 255) : new Color32(240, 154, 41, 255);
    }

    //GDPR
    public void OnCornfirmAds()
    {
        LoadAndShowCmpFlow();
    }
    private void LoadAndShowCmpFlow()
    {
        //var cmpService = MaxSdk.CmpService;

        //cmpService.ShowCmpForExistingUser(error =>
        //{
        //    if (null == error)
        //    {
        //        // The CMP alert was shown successfully.
        //    }
        //});
    }
    private void CheckConfigStatus()
    {
        //if(MaxSdk.GetSdkConfiguration().AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Authorized || MaxSdk.GetSdkConfiguration().AppTrackingStatus == MaxSdkBase.AppTrackingStatus.Restricted)
        //{
        //    SkygoBridge.instance.ConfirmAds = 1;
        //    ApplovinBridge.instance.LoadAds();
        //}
        //else
        //{
        //    SkygoBridge.instance.ConfirmAds = 0;
        //}
    }
}
