using UnityEngine;

public class FollowInactiveHand : MonoBehaviour
{
	public BaseCharacter data;

	public GameObject leftHand;

	public GameObject rightHand;

	public Transform offHand;
	
	
	private void Start()
	{
		data = base.transform.root.GetComponent<BaseCharacter>();
	}

	private void Update()
	{
		if (data == null || data.UI) return;
		transform.localPosition = Vector3.zero;
		if (data.movement.aimDirection.x < 0f)
		{
			transform.SetParent(rightHand.transform);
			offHand.transform.SetEulerAnglesZAxis(0);
		}
		else
		{
			transform.SetParent(leftHand.transform);
			offHand.transform.SetEulerAnglesZAxis(180);
		}
	}
}
