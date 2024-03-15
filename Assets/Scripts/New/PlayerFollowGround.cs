using UnityEngine;

public class PlayerFollowGround : MonoBehaviour
{
	private BaseCharacter data;

	private Vector2 lastPos;

	private Rigidbody2D lastRig;

	private void Start()
	{
		data = GetComponent<BaseCharacter>();
	}

	private void FixedUpdate()
	{
		if (data.standOnRig == null || !data.isGrounded)
		{
			lastPos = Vector2.zero;
			return;
		}
		if (lastPos != Vector2.zero && data.standOnRig == lastRig)
		{
            Debug.Log("alo");
			data.playerVel.transform.position = data.playerVel.position + (data.standOnRig.position - lastPos);
		}
		lastPos = data.standOnRig.position;
		lastRig = data.standOnRig;
	}
}
