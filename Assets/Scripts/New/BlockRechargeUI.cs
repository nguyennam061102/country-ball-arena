using UnityEngine;
using UnityEngine.UI;

public class BlockRechargeUI : MonoBehaviour
{
    private Block block;
    private BaseCharacter data;
    public Image img;
    public SpriteRenderer offHand;
    public Sprite[] offHandList;

    private void Start()
    {
        img = GetComponentInChildren<Image>();
        block = GetComponentInParent<Block>();
        data = GetComponentInParent<BaseCharacter>();

        if ((bool)data)
        {
            if (data.player.playerID == 0)
            {
                SetOffHandImage(GameData.CurrentOffHandId);
            }
            else if (data.player.playerID == 1)
            {
                SetOffHandImage(GameData.CurrentOffHandAi1Id);
            }
            else if (data.player.playerID == 2)
            {
                SetOffHandImage(GameData.CurrentOffHandAi2Id);
            }
            else if (data.player.playerID == 3)
            {
                SetOffHandImage(GameData.CurrentOffHandAi3Id);
            }
        }
    }

    void SetOffHandImage(int id)
    {
        offHand.sprite = offHandList[id];
    }

    private void Update()
    {
        img.fillAmount = block.counter / block.Cooldown();
        img.gameObject.SetActive(!(img.fillAmount >= 1));
    }
}
