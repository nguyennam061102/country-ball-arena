using TMPro;
using UnityEngine;

public class PlayerWobblePosition : MonoBehaviour
{
	private Vector3 physicsPos;

	public float drag = 15f;

	public float spring = 1000f;

	public float multiplier = 1f;

	public float prediction;

	private Vector3 velocity;

	//private Player player;
	public BaseCharacter data;
	public TextMeshProUGUI textName; 

	private void Start()
	{
		physicsPos = base.transform.position;
		//player = GetComponentInParent<Player>();
		data = GetComponentInParent<BaseCharacter>();
		textName.text = data.nameAI;
	}

	private void Update()
	{
		//float num = Mathf.Clamp(TimeHandler.deltaTime, 0f, 0.03f);
		var num = 0.03f;
		Vector3 a = data.transform.position;
		if (prediction > 0f)
		{
			a += (Vector3)data.playerVel.velocity * prediction;
		}
		velocity += (a - physicsPos) * num * spring;
		velocity -= velocity * drag * num;
		physicsPos += num * multiplier * velocity;
		base.transform.position = physicsPos;
	}
}