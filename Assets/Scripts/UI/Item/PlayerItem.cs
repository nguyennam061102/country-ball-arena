using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] public int id;
    [SerializeField] private UI2DSprite image;
    [SerializeField] private UI2DSprite eye;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetItem(ShopItemInfo shopItemInfo)
    {
        id = shopItemInfo.itemId;
        image.sprite2D = shopItemInfo.itemIcon;
        eye.sprite2D = shopItemInfo.eye;
        eye.MakePixelPerfect();
    }
    public void SetEye(Sprite eyeSprite)
    {
        eye.sprite2D = eyeSprite;
        eye.MakePixelPerfect();
    }
}
