using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelRename : MonoBehaviour
{
    private UiController UI => UiController.Instance;
    public UIInput Input;
    public GameObject ButtonBack;
    // Start is called before the first frame update
    void Start()
    {
        if(GameData.IsFirstTimePlay)
        {
            ButtonBack.SetActive(false);
            Input.label.text = "Player";
        }
        else
        {
            ButtonBack.SetActive(true);
        }
    }

    [Button]
    public void OnRename()
    {
        GameData.PlayerName = Input.label.text;
        Sound.Play(Sound.SoundData.ButtonClick);
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
    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UI.GetPanel(PanelName.PanelCharacter);
    }
}
