using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelEmoji : MonoBehaviour
{
    [SerializeField] List<EmojiItem> emojiItem;
    [SerializeField] List<Vector3> pos;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] Transform emojiPos;
    bool isOpen;
    // Update is called once per frame
    void Awake()
    {
        //verticalLayoutGroup.enabled = false;
        for (int i = 0; i < emojiItem.Count; i++)
        {
            pos.Add(emojiItem[i].transform.localPosition);
            emojiItem[i].transform.position = emojiPos.transform.position;
            emojiItem[i].Emoji.sprite = FakeOnlController.Instance.Emoji[i];                       
        }
        this.gameObject.SetActive(false);
    }
    public void OnInit()
    {
        if(!isOpen)
        {
            for (int i = 0; i < emojiItem.Count; i++)
            {
                emojiItem[i].transform.DOLocalMove(pos[i], .5f);
            }
            isOpen = true;
        }
        else
        {
            for (int i = 0; i < emojiItem.Count; i++)
            {
                emojiItem[i].transform.DOMove(emojiPos.position, .5f);
            }
            isOpen = false;
        }
    }
    public void SetEmoji(int id)
    {
        PlayerAssigner.instance.mainPlayer.SetEmoji(id);
        for (int i = 0; i < emojiItem.Count; i++)
        {
            emojiItem[i].transform.DOMove(emojiPos.position, .5f);
        }
        isOpen = false;
        //this.gameObject.SetActive(false);
    }

}
