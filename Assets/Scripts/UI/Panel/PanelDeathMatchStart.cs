using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDeathMatchStart : MonoBehaviour
{
    [SerializeField] GroupPlayerShow[] playersShow;
    [SerializeField] UILabel lbTotal, lbBest;

    // Start is called before the first frame update
    void Start()
    {
        lbTotal.text = "Total Kills: " + GameData.PlayerTotalKillDeathMatch;
        lbBest.text = "Best Kills: " + GameData.PlayerBestKillDeathMatch;
        SelectAndShowRandomEnemies();
    }

    void SelectAndShowRandomEnemies()
    {
        playersShow[0].ShowPlayer("You", GameData.CurrentSkinId, GameData.CurrentMainHandId, GameData.CurrentOffHandId);

        int maxWeaponID = 3 + GameData.PlayerDeathMatchWinCount;
        if (maxWeaponID > GameFollowData.Instance.mainHandList.Length) maxWeaponID = GameFollowData.Instance.mainHandList.Length;

        GameFollowData.Instance.Player2SkinID = Random.Range(1, GameFollowData.Instance.skinList.Length);
        GameFollowData.Instance.Player2WeaponID = Random.Range(0, maxWeaponID);
        GameFollowData.Instance.Player2OffhandID = Random.Range(0, GameFollowData.Instance.offHandList.Length);

        playersShow[1].ShowPlayer(GameFollowData.Instance.skinList[GameFollowData.Instance.Player2SkinID].itemName,
            GameFollowData.Instance.Player2SkinID, GameFollowData.Instance.Player2WeaponID, GameFollowData.Instance.Player2OffhandID);

        GameFollowData.Instance.Player3SkinID = Random.Range(1, GameFollowData.Instance.skinList.Length);
        GameFollowData.Instance.Player3WeaponID = Random.Range(0, maxWeaponID);
        GameFollowData.Instance.Player3OffhandID = Random.Range(0, GameFollowData.Instance.offHandList.Length);

        playersShow[2].ShowPlayer(GameFollowData.Instance.skinList[GameFollowData.Instance.Player3SkinID].itemName,
            GameFollowData.Instance.Player3SkinID, GameFollowData.Instance.Player3WeaponID, GameFollowData.Instance.Player3OffhandID);
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UiController.Instance.GetPanel(PanelName.PanelMenu);
    }

    public void OnPlayButton()
    {
        GameFollowData.Instance.playingGameMode = GameMode.DeathMatch;
        Loading.Instance.LoadScene("game");
        Destroy(gameObject);
    }
}
