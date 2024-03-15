using UnityEngine;

namespace RootMotion.FinalIK
{
	public class IKJBoneParams : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float weight = 1f;
	}
}
