using UnityEngine;

public class SetColorByBlockCD : MonoBehaviour
{
	private Block block;

	public SpriteRenderer spriteRend;

	public Color onCDColor;

	public Color offCDColor;

	public bool isOnCD;

	private void Start()
	{
		block = GetComponentInParent<Block>();
		//spriteRend = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		if (isOnCD != block.IsOnCD())
		{
			isOnCD = block.IsOnCD();
			if (isOnCD)
			{
				spriteRend.color = onCDColor;
			}
			else
			{
				spriteRend.color = offCDColor;
			}
		}
	}
}
