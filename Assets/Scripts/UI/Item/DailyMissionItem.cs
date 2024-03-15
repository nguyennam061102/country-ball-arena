using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyMissionItem : MonoBehaviour
{
    [SerializeField] private UI2DSprite missionIcon;
    [SerializeField] private UILabel missionName, missionReward, buttonText;
    [SerializeField] private UI2DSprite sprClaim;
    [SerializeField] private BoxCollider claimButton;
    private DailyMissionInfo info;

    public void SetItem(DailyMissionInfo info)
    {
        this.info = info;
        this.missionIcon.sprite2D = info.missionIcon;
        this.missionIcon.MakePixelPerfect();
        this.missionName.text = info.ToString();
        this.missionReward.text = $"+{info.MissionReward}";
        if (info.MissionClaimed)
        {
            buttonText.text = "Completed";
            sprClaim.color = new Color32(240, 154, 41, 255);
            return;
        }

        claimButton.enabled = info.MissionDone;
        sprClaim.color = info.MissionDone ? new Color32(50, 184, 81, 255) : new Color32(128, 132, 134, 255);
        buttonText.text = info.MissionDone ? "Claim" : $"{info.MissionProgress}/{info.TargetProgress}";
    }

    public void OnClaimButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (!info.MissionClaimed)
        {
            info.MissionClaimed = true;
            GameData.Gold += info.MissionReward;
            SetItem(info);
        }
    }
}
