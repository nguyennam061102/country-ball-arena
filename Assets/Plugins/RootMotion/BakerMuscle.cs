using System;
using UnityEngine;

namespace RootMotion
{
	[Serializable]
	public class BakerMuscle
	{
		public AnimationCurve curve;

		private int muscleIndex = -1;

		private string propertyName;

		public BakerMuscle(int muscleIndex)
		{
			this.muscleIndex = muscleIndex;
			propertyName = MuscleNameToPropertyName(HumanTrait.MuscleName[muscleIndex]);
			Reset();
		}

		private string MuscleNameToPropertyName(string n)
		{
			if (n == "Left Index 1 Stretched")
			{
				return "LeftHand.Index.1 Stretched";
			}
			if (n == "Left Index 2 Stretched")
			{
				return "LeftHand.Index.2 Stretched";
			}
			if (n == "Left Index 3 Stretched")
			{
				return "LeftHand.Index.3 Stretched";
			}
			if (n == "Left Middle 1 Stretched")
			{
				return "LeftHand.Middle.1 Stretched";
			}
			if (n == "Left Middle 2 Stretched")
			{
				return "LeftHand.Middle.2 Stretched";
			}
			if (n == "Left Middle 3 Stretched")
			{
				return "LeftHand.Middle.3 Stretched";
			}
			if (n == "Left Ring 1 Stretched")
			{
				return "LeftHand.Ring.1 Stretched";
			}
			if (n == "Left Ring 2 Stretched")
			{
				return "LeftHand.Ring.2 Stretched";
			}
			if (n == "Left Ring 3 Stretched")
			{
				return "LeftHand.Ring.3 Stretched";
			}
			if (n == "Left Little 1 Stretched")
			{
				return "LeftHand.Little.1 Stretched";
			}
			if (n == "Left Little 2 Stretched")
			{
				return "LeftHand.Little.2 Stretched";
			}
			if (n == "Left Little 3 Stretched")
			{
				return "LeftHand.Little.3 Stretched";
			}
			if (n == "Left Thumb 1 Stretched")
			{
				return "LeftHand.Thumb.1 Stretched";
			}
			if (n == "Left Thumb 2 Stretched")
			{
				return "LeftHand.Thumb.2 Stretched";
			}
			if (n == "Left Thumb 3 Stretched")
			{
				return "LeftHand.Thumb.3 Stretched";
			}
			if (n == "Left Index Spread")
			{
				return "LeftHand.Index.Spread";
			}
			if (n == "Left Middle Spread")
			{
				return "LeftHand.Middle.Spread";
			}
			if (n == "Left Ring Spread")
			{
				return "LeftHand.Ring.Spread";
			}
			if (n == "Left Little Spread")
			{
				return "LeftHand.Little.Spread";
			}
			if (n == "Left Thumb Spread")
			{
				return "LeftHand.Thumb.Spread";
			}
			if (n == "Right Index 1 Stretched")
			{
				return "RightHand.Index.1 Stretched";
			}
			if (n == "Right Index 2 Stretched")
			{
				return "RightHand.Index.2 Stretched";
			}
			if (n == "Right Index 3 Stretched")
			{
				return "RightHand.Index.3 Stretched";
			}
			if (n == "Right Middle 1 Stretched")
			{
				return "RightHand.Middle.1 Stretched";
			}
			if (n == "Right Middle 2 Stretched")
			{
				return "RightHand.Middle.2 Stretched";
			}
			if (n == "Right Middle 3 Stretched")
			{
				return "RightHand.Middle.3 Stretched";
			}
			if (n == "Right Ring 1 Stretched")
			{
				return "RightHand.Ring.1 Stretched";
			}
			if (n == "Right Ring 2 Stretched")
			{
				return "RightHand.Ring.2 Stretched";
			}
			if (n == "Right Ring 3 Stretched")
			{
				return "RightHand.Ring.3 Stretched";
			}
			if (n == "Right Little 1 Stretched")
			{
				return "RightHand.Little.1 Stretched";
			}
			if (n == "Right Little 2 Stretched")
			{
				return "RightHand.Little.2 Stretched";
			}
			if (n == "Right Little 3 Stretched")
			{
				return "RightHand.Little.3 Stretched";
			}
			if (n == "Right Thumb 1 Stretched")
			{
				return "RightHand.Thumb.1 Stretched";
			}
			if (n == "Right Thumb 2 Stretched")
			{
				return "RightHand.Thumb.2 Stretched";
			}
			if (n == "Right Thumb 3 Stretched")
			{
				return "RightHand.Thumb.3 Stretched";
			}
			if (n == "Right Index Spread")
			{
				return "RightHand.Index.Spread";
			}
			if (n == "Right Middle Spread")
			{
				return "RightHand.Middle.Spread";
			}
			if (n == "Right Ring Spread")
			{
				return "RightHand.Ring.Spread";
			}
			if (n == "Right Little Spread")
			{
				return "RightHand.Little.Spread";
			}
			if (n == "Right Thumb Spread")
			{
				return "RightHand.Thumb.Spread";
			}
			return n;
		}

		public void MultiplyLength(AnimationCurve curve, float mlp)
		{
			Keyframe[] keys = curve.keys;
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].time *= mlp;
			}
			curve.keys = keys;
		}

		public void SetCurves(ref AnimationClip clip, float maxError, float lengthMlp)
		{
			MultiplyLength(curve, lengthMlp);
			BakerUtilities.ReduceKeyframes(curve, maxError);
			clip.SetCurve(string.Empty, typeof(Animator), propertyName, curve);
		}

		public void Reset()
		{
			curve = new AnimationCurve();
		}

		public void SetKeyframe(float time, float[] muscles)
		{
			curve.AddKey(time, muscles[muscleIndex]);
		}

		public void SetLoopFrame(float time)
		{
			BakerUtilities.SetLoopFrame(time, curve);
		}
	}
}
