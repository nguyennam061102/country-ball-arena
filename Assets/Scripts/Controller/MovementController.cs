using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public GameObject ingameUI;
    public GameObject movementGo;
    [Header("BLOCK")]
    public Sprite[] blockList;
    public Image blockImage, loadBlock;
    public Image blockImage2, loadBlock2;
    public Text loadBlockSeconds;
    public Text loadBlockSeconds2;
    Block block;
    public GameObject groupSurvival, groupDM, groupSandbox, backbutton, emojibutton;

    private void Start()
    {
        blockImage.sprite = blockList[GameData.CurrentOffHandId];
        blockImage.SetNativeSize();
        blockImage2.sprite = blockList[GameData.CurrentOffHandId];
        blockImage2.SetNativeSize();
    }

    public void ShowIngameUI(bool flag)
    {
        ingameUI.SetActive(flag);
    }

    public void SetPlayerBlock(Player p)
    {
        block = p.GetComponent<Block>();
    }

    private void Update()
    {
        if (block != null)
        {
            if(block.counter < block.Cooldown())
            {
                loadBlock.fillAmount = 1 - block.counter / block.Cooldown();
                loadBlock2.fillAmount = 1 - block.counter / block.Cooldown();
                float remainVal = block.Cooldown() - block.counter;
                loadBlockSeconds.text = remainVal.ToString("0.0");
                loadBlockSeconds2.text = remainVal.ToString("0.0");
            }
            else
            {
                loadBlock.fillAmount = 0f;
                loadBlock2.fillAmount = 0f;
                loadBlockSeconds.text = "";
                loadBlockSeconds2.text = "";
            }
        }
    }
}
