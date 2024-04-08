using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewUIManager : SingletonMonoBehavior<NewUIManager>
{
    [SerializeField] PanelRename UIRename;
    [SerializeField] Transform tfCanvas;
    public void OpenUI()
    {
        PanelRename newUIRename = Instantiate(UIRename, tfCanvas);
    }
}
