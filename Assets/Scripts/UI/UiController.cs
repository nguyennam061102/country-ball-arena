using System;
using System.Collections.Generic;
using UnityEngine;

public class UiController : DontDestroy<UiController>
{
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Transform UIParent;
    [HideInInspector] public GameObject currentPanel;
    public PanelName lastPanelName;
    public int panelInventoryType;
    public int panelCharacterTitle;
    public PanelName currentPanelName;

    public GameObject GetPanel(PanelName pName)
    {
        if (currentPanel != null)
        {
            lastPanelName = currentPanelName;
            Destroy(currentPanel);
        }
        foreach (var panel in panels)
        {
            if (panel.name.Contains(pName.ToString()))
            {
                currentPanel = Instantiate(panel, UIParent);
                currentPanelName = pName;
                return currentPanel;
            }
        }
        return null;
    }
}

public enum PanelName
{
    PanelMenu = 0,
    PanelMenuSetting = 1,
    PanelShop = 2,
    PanelCharacter = 3,
    PanelInventory = 4,
    PanelSpin = 5,
    PanelTalent = 6,
    PanelReady = 7,
    PanelSelectCard = 8,
    PanelPause = 9,
    PanelEndgame = 10,
    PanelDailyReward = 11,
    DailyMission = 12,
    PanelContinue = 13,
    PanelDeathMatchEnd = 14,
    PanelDeathMatchStart = 15,
    PanelRename = 16
}
