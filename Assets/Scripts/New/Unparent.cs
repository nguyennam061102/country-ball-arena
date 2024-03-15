using System.Collections;
using UnityEngine;

public class Unparent : MonoBehaviour
{
	public Transform parent;

	public bool follow;

	public float destroyDelay;

	private bool done;

	private void Start()
	{
		parent = base.transform.root;
		//parent = GetComponentInParent<BaseCharacter>().transform;
	}

	//private void LateUpdate()
	private void FixedUpdate()
	{
		if (!done)
		{
			if (transform.root != null)
			{
				transform.SetParent(null, true);
			}
			if (follow && (bool)parent)
			{
				transform.position = parent.transform.position;
			}
			if (!parent)
			{
				StartCoroutine(DelayRemove());
				done = true;
			}
		}
	}

	private IEnumerator DelayRemove()
	{
		yield return new WaitForSeconds(destroyDelay);
		Destroy(gameObject);
	}
}
