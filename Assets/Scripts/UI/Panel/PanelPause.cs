using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPause : MonoBehaviour
{
    private GameController gameController => GameController.Instance;
    private MovementController Mc => GameController.Instance.MovementController;
    private UiController UI => UiController.Instance;

    [SerializeField] private UILabel soundText, musicText, controlText, graphicsText;
    [SerializeField] private UI2DSprite sprSound, sprMusic, sprGraphics;
    [SerializeField] private UI2DSprite cardPrefab;
    [SerializeField] private UIGrid cardParent;

    private void OnEnable()
    {
        //if (Mc != null) Mc.movementGo.SetActive(false);
        Mc.ShowIngameUI(false);
        //UI.bgParticle.SetActive(false);
        soundText.text = GameData.Sound ? "On" : "Off";
        musicText.text = GameData.Music ? "On" : "Off";
        controlText.text = GameData.ControlLeft ? "Left" : "Right";
        graphicsText.text = GameData.GraphicsLevel == 0 ? "Low" : "High";
        gameController.gamePaused = true;
        SetButtonColor();

        GetCardUi();
    }

    private void GetCardUi()
    {
        foreach (var item in gameController.cardImageList)
        {
            var card = Instantiate(cardPrefab, cardParent.transform);
            card.sprite2D = item;
            card.width = 128;
            card.height = 128;
            card.gameObject.SetActive(true);
        }
    }

    public void OnBackButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        UiController.Instance.GetPanel(PanelName.PanelMenu);
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

    public void OnRetreatButton()
    {
        GameData.ShopWarning = true;
        Sound.Play(Sound.SoundData.ButtonClick);
        Destroy(gameObject);
        Loading.Instance.LoadScene("menu");
    }

    public void OnGraphicsButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        if (GameData.GraphicsLevel == 1) GameData.GraphicsLevel = 0;
        else if (GameData.GraphicsLevel == 0) GameData.GraphicsLevel = 1;
        graphicsText.text = GameData.GraphicsLevel == 0 ? "Low" : "High";
        SetButtonColor();
    }

    public void OnResumeButton()
    {
        Sound.Play(Sound.SoundData.ButtonClick);
        //if (Mc != null) Mc.movementGo.SetActive(true);
        Mc.ShowIngameUI(true);
        //UI.bgParticle.SetActive(false);
        gameController.SetControlSide();
        gameController.SetGraphics();
        gameController.gamePaused = false;
        Destroy(gameObject);
    }

    void SetButtonColor()
    {
        sprSound.color = GameData.Sound ? new Color32(50, 184, 81, 255) : new Color32(128, 132, 134, 255);
        sprMusic.color = GameData.Sound ? new Color32(50, 184, 81, 255) : new Color32(128, 132, 134, 255);
        sprGraphics.color = GameData.GraphicsLevel == 0 ? new Color32(50, 184, 81, 255) : new Color32(240, 154, 41, 255);
    }

}
