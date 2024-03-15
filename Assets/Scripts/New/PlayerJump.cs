using System;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
	private BaseCharacter data;

	public float upForce;

	private CharacterStatModifiers stats;

	public ParticleSystem[] jumpPart;

	public float sideForce = 1f;

	public Action JumpAction;

	private void Start()
	{
		stats = GetComponent<CharacterStatModifiers>();
		data = GetComponent<BaseCharacter>();
		SetJumPartColor();
		data.skinHandler.onSetSkinAction += SetJumPartColor;
	}

	void SetJumPartColor()
	{
		foreach (var fx in jumpPart)
		{
			var mainModule = fx.main;
			mainModule.startColor = data.skinHandler.ColorForEFX;
		}
	}

	private void Update()
	{		
		if (data.movement.controlledElseWhere) return;
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}
	}

	public void Jump(bool forceJump = false, float multiplier = 1f)
	{
		if (!forceJump && (data.sinceJump < 0.1f || (data.currentJumps <= 0 && data.sinceWallGrab > 0.05f))) return;
		Sound.Play(Sound.SoundData.Jump);	
		Vector3 a = Vector3.up;
		Vector3 vector = data.groundPos;
        JumpAction?.Invoke();
        bool flag = false;
		if (data.sinceWallGrab < 0.1f && !data.isGrounded)
		{
			a = Vector2.up * 0.8f + data.wallNormal * 0.4f;
			vector = data.wallPos;
			data.currentJumps = data.jumps;
            //Debug.Log("tu nhien vao day");
			flag = true;
		}
		else
		{
			if (data.sinceGrounded > 0.05f)
			{
				vector = base.transform.position;
            }
		}
		if (data.playerVel.velocity.y < 0f)
		{
			data.playerVel.velocity = new Vector2(data.playerVel.velocity.x, 0f);
		}
		data.sinceGrounded = 0f;
		data.sinceJump = 0f;
		data.isGrounded = false;
		data.isWallGrab = false;
		data.currentJumps--;
        //Debug.Log("co tru ko");
		data.playerVel.AddForce(a * multiplier * 0.01f * data.stats.jump * data.playerVel.mass * (1f - stats.slow) * upForce, ForceMode2D.Impulse);
		if (!flag)
		{
			data.playerVel.AddForce(Vector2.right * multiplier * sideForce * 0.01f * data.stats.jump * data.playerVel.mass * (1f - stats.slow) * data.playerVel.velocity.x, ForceMode2D.Impulse);
		}
		for (int i = 0; i < jumpPart.Length; i++)
		{
			jumpPart[i].transform.position = new Vector3(vector.x, vector.y, 5f) - a * 0f;
			jumpPart[i].transform.rotation = Quaternion.LookRotation(data.playerVel.velocity);
			jumpPart[i].Play();
		}
	}
}
