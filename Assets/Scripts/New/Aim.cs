using UnityEngine;

public class Aim : MonoBehaviour
{
	private PlayerMovement input;

    [SerializeField] private HoldingObject holdingObject;

	private BaseCharacter data;

	private Vector3 aimDirection;

	private void Awake()
	{
		input = GetComponent<PlayerMovement>();
		data = GetComponent<BaseCharacter>();
		//holdingObject = GetComponentInChildren<HoldingObject>();//bay gio keo thang tu inspector
	}

	private void Update()
	{
		if ((double)input.aimDirection.magnitude > 0.2)
		{
			aimDirection = input.aimDirection;
		}
		if (input.moveDirection.magnitude > 0.2f && /*Optionshandler.leftStickAim &&*/ input.aimDirection == Vector3.zero)
		{
			aimDirection = input.moveDirection;
		}
		if ((bool)holdingObject)
		{
			if (aimDirection != Vector3.zero)
			{
				holdingObject.transform.rotation = Quaternion.LookRotation(aimDirection);
			}
			data.movement.aimDirection = aimDirection;
		}
	}
}
