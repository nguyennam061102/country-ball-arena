using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelReady : MonoBehaviour
{
    private MovementController Mc => GameController.Instance.MovementController;
    private MapController Map => MapController.Instance;

    private void OnEnable()
    {
        //if (Mc != null) Mc.movementGo.SetActive(false);
    }

    public void OnStartButton()
    {
        //Sound.Play(Sound.SoundData.ButtonClick);
        //Destroy(gameObject);
        //Map.FirstTimeShowMap();
    }
}
