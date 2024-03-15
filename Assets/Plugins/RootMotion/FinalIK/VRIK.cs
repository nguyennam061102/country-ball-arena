using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[AddComponentMenu("Scripts/RootMotion.FinalIK/IK/VR IK")]
	public class VRIK : IK
	{
		[Serializable]
		public class References
		{
			public Transform root;

			public Transform pelvis;

			public Transform spine;

			[Tooltip("Optional")]
			public Transform chest;

			[Tooltip("Optional")]
			public Transform neck;

			public Transform head;

			[Tooltip("Optional")]
			public Transform leftShoulder;

			public Transform leftUpperArm;

			public Transform leftForearm;

			public Transform leftHand;

			[Tooltip("Optional")]
			public Transform rightShoulder;

			public Transform rightUpperArm;

			public Transform rightForearm;

			public Transform rightHand;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftThigh;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftCalf;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform leftFoot;

			[Tooltip("Optional")]
			public Transform leftToes;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightThigh;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightCalf;

			[Tooltip("VRIK also supports legless characters.If you do not wish to use legs, leave all leg references empty.")]
			public Transform rightFoot;

			[Tooltip("Optional")]
			public Transform rightToes;

			public bool isFilled
			{
				get
				{
					if (root == null || pelvis == null || spine == null || head == null || leftUpperArm == null || leftForearm == null || leftHand == null || rightUpperArm == null || rightForearm == null || rightHand == null)
					{
						return false;
					}
					if (leftThigh == null && leftCalf == null && leftFoot == null && rightThigh == null && rightCalf == null && rightFoot == null)
					{
						return true;
					}
					if (leftThigh == null || leftCalf == null || leftFoot == null || rightThigh == null || rightCalf == null || rightFoot == null)
					{
						return false;
					}
					return true;
				}
			}

			public bool isEmpty
			{
				get
				{
					if (root != null || pelvis != null || spine != null || chest != null || neck != null || head != null || leftShoulder != null || leftUpperArm != null || leftForearm != null || leftHand != null || rightShoulder != null || rightUpperArm != null || rightForearm != null || rightHand != null || leftThigh != null || leftCalf != null || leftFoot != null || leftToes != null || rightThigh != null || rightCalf != null || rightFoot != null || rightToes != null)
					{
						return false;
					}
					return true;
				}
			}

			public Transform[] GetTransforms()
			{
				return new Transform[22]
				{
					root,
					pelvis,
					spine,
					chest,
					neck,
					head,
					leftShoulder,
					leftUpperArm,
					leftForearm,
					leftHand,
					rightShoulder,
					rightUpperArm,
					rightForearm,
					rightHand,
					leftThigh,
					leftCalf,
					leftFoot,
					leftToes,
					rightThigh,
					rightCalf,
					rightFoot,
					rightToes
				};
			}

			public static bool AutoDetectReferences(Transform root, out References references)
			{
				references = new References();
				Animator componentInChildren = root.GetComponentInChildren<Animator>();
				if (componentInChildren == null || !componentInChildren.isHuman)
				{
					UnityEngine.Debug.LogWarning("VRIK needs a Humanoid Animator to auto-detect biped references. Please assign references manually.");
					return false;
				}
				references.root = root;
				references.pelvis = componentInChildren.GetBoneTransform(HumanBodyBones.Hips);
				references.spine = componentInChildren.GetBoneTransform(HumanBodyBones.Spine);
				references.chest = componentInChildren.GetBoneTransform(HumanBodyBones.Chest);
				references.neck = componentInChildren.GetBoneTransform(HumanBodyBones.Neck);
				references.head = componentInChildren.GetBoneTransform(HumanBodyBones.Head);
				references.leftShoulder = componentInChildren.GetBoneTransform(HumanBodyBones.LeftShoulder);
				references.leftUpperArm = componentInChildren.GetBoneTransform(HumanBodyBones.LeftUpperArm);
				references.leftForearm = componentInChildren.GetBoneTransform(HumanBodyBones.LeftLowerArm);
				references.leftHand = componentInChildren.GetBoneTransform(HumanBodyBones.LeftHand);
				references.rightShoulder = componentInChildren.GetBoneTransform(HumanBodyBones.RightShoulder);
				references.rightUpperArm = componentInChildren.GetBoneTransform(HumanBodyBones.RightUpperArm);
				references.rightForearm = componentInChildren.GetBoneTransform(HumanBodyBones.RightLowerArm);
				references.rightHand = componentInChildren.GetBoneTransform(HumanBodyBones.RightHand);
				references.leftThigh = componentInChildren.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
				references.leftCalf = componentInChildren.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
				references.leftFoot = componentInChildren.GetBoneTransform(HumanBodyBones.LeftFoot);
				references.leftToes = componentInChildren.GetBoneTransform(HumanBodyBones.LeftToes);
				references.rightThigh = componentInChildren.GetBoneTransform(HumanBodyBones.RightUpperLeg);
				references.rightCalf = componentInChildren.GetBoneTransform(HumanBodyBones.RightLowerLeg);
				references.rightFoot = componentInChildren.GetBoneTransform(HumanBodyBones.RightFoot);
				references.rightToes = componentInChildren.GetBoneTransform(HumanBodyBones.RightToes);
				return true;
			}
		}

		[ContextMenuItem("Auto-detect References", "AutoDetectReferences")]
		[Tooltip("Bone mapping. Right-click on the component header and select 'Auto-detect References' of fill in manually if not a Humanoid character. Chest, neck, shoulder and toe bones are optional. VRIK also supports legless characters. If you do not wish to use legs, leave all leg references empty.")]
		public References references = new References();

		[Tooltip("The VRIK solver.")]
		public IKSolverVR solver = new IKSolverVR();

		[ContextMenu("User Manual")]
		protected override void OpenUserManual()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/page16.html");
		}

		[ContextMenu("Scrpt Reference")]
		protected override void OpenScriptReference()
		{
			Application.OpenURL("http://www.root-motion.com/finalikdox/html/class_root_motion_1_1_final_i_k_1_1_v_r_i_k.html");
		}

		[ContextMenu("TUTORIAL VIDEO (STEAMVR SETUP)")]
		private void OpenSetupTutorial()
		{
			Application.OpenURL("https://www.youtube.com/watch?v=6Pfx7lYQiIA&feature=youtu.be");
		}

		[ContextMenu("Auto-detect References")]
		public void AutoDetectReferences()
		{
			References.AutoDetectReferences(base.transform, out references);
		}

		[ContextMenu("Guess Hand Orientations")]
		public void GuessHandOrientations()
		{
			solver.GuessHandOrientations(references, onlyIfZero: false);
		}

		public override IKSolver GetIKSolver()
		{
			return solver;
		}

		protected override void InitiateSolver()
		{
			if (references.isEmpty)
			{
				AutoDetectReferences();
			}
			if (references.isFilled)
			{
				solver.SetToReferences(references);
			}
			base.InitiateSolver();
		}

		protected override void UpdateSolver()
		{
			if (references.root != null && references.root.localScale == Vector3.zero)
			{
				UnityEngine.Debug.LogError("VRIK Root Transform's scale is zero, can not update VRIK. Make sure you have not calibrated the character to a zero scale.", base.transform);
				base.enabled = false;
			}
			else
			{
				base.UpdateSolver();
			}
		}
	}
}
