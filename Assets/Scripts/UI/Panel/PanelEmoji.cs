using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEmoji : MonoBehaviour
{
   

    // Update is called once per frame
    public void OnInit()
    {
        this.gameObject.SetActive(true);
    }
    public void SetEmoji(int id)
    {
        PlayerAssigner.instance.mainPlayer.SetEmoji(id);
        this.gameObject.SetActive(false);
    }
}
