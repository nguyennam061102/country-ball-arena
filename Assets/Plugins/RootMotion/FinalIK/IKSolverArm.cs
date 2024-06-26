using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverArm : IKSolver
	{
		[Range(0f, 1f)]
		public float IKRotationWeight = 1f;

		public Quaternion IKRotation = Quaternion.identity;

		public Point chest = new Point();

		public Point shoulder = new Point();

		public Point upperArm = new Point();

		public Point forearm = new Point();

		public Point hand = new Point();

		public bool isLeft;

		public IKSolverVR.Arm arm = new IKSolverVR.Arm();

		private Vector3[] positions = new Vector3[6];

		private Quaternion[] rotations = new Quaternion[6];

		public override bool IsValid(ref string message)
		{
			if (chest.transform == null || shoulder.transform == null || upperArm.transform == null || forearm.transform == null || hand.transform == null)
			{
				message = "Please assign all bone slots of the Arm IK solver.";
				return false;
			}
			UnityEngine.Object[] objects = new Transform[5]
			{
				chest.transform,
				shoulder.transform,
				upperArm.transform,
				forearm.transform,
				hand.transform
			};
			Transform transform = (Transform)Hierarchy.ContainsDuplicate(objects);
			if (transform != null)
			{
				message = transform.name + " is represented multiple times in the ArmIK.";
				return false;
			}
			return true;
		}

		public bool SetChain(Transform chest, Transform shoulder, Transform upperArm, Transform forearm, Transform hand, Transform root)
		{
			this.chest.transform = chest;
			this.shoulder.transform = shoulder;
			this.upperArm.transform = upperArm;
			this.forearm.transform = forearm;
			this.hand.transform = hand;
			Initiate(root);
			return base.initiated;
		}

		public override Point[] GetPoints()
		{
			return new Point[5]
			{
				chest,
				shoulder,
				upperArm,
				forearm,
				hand
			};
		}

		public override Point GetPoint(Transform transform)
		{
			if (chest.transform == transform)
			{
				return chest;
			}
			if (shoulder.transform == transform)
			{
				return shoulder;
			}
			if (upperArm.transform == transform)
			{
				return upperArm;
			}
			if (forearm.transform == transform)
			{
				return forearm;
			}
			if (hand.transform == transform)
			{
				return hand;
			}
			return null;
		}

		public override void StoreDefaultLocalState()
		{
			shoulder.StoreDefaultLocalState();
			upperArm.StoreDefaultLocalState();
			forearm.StoreDefaultLocalState();
			hand.StoreDefaultLocalState();
		}

		public override void FixTransforms()
		{
			if (base.initiated)
			{
				shoulder.FixTransform();
				upperArm.FixTransform();
				forearm.FixTransform();
				hand.FixTransform();
			}
		}

		protected override void OnInitiate()
		{
			IKPosition = hand.transform.position;
			IKRotation = hand.transform.rotation;
			Read();
		}

		protected override void OnUpdate()
		{
			Read();
			Solve();
			Write();
		}

		private void Solve()
		{
			arm.PreSolve();
			arm.ApplyOffsets(1f);
			arm.Solve(isLeft);
			arm.ResetOffsets();
		}

		private void Read()
		{
			arm.IKPosition = IKPosition;
			arm.positionWeight = IKPositionWeight;
			arm.IKRotation = IKRotation;
			arm.rotationWeight = IKRotationWeight;
			positions[0] = root.position;
			positions[1] = chest.transform.position;
			positions[2] = shoulder.transform.position;
			positions[3] = upperArm.transform.position;
			positions[4] = forearm.transform.position;
			positions[5] = hand.transform.position;
			rotations[0] = root.rotation;
			rotations[1] = chest.transform.rotation;
			rotations[2] = shoulder.transform.rotation;
			rotations[3] = upperArm.transform.rotation;
			rotations[4] = forearm.transform.rotation;
			rotations[5] = hand.transform.rotation;
			arm.Read(positions, rotations, hasChest: false, hasNeck: false, hasShoulders: true, hasToes: false, hasLegs: false, 1, 2);
		}

		private void Write()
		{
			arm.Write(ref positions, ref rotations);
			shoulder.transform.rotation = rotations[2];
			upperArm.transform.rotation = rotations[3];
			forearm.transform.rotation = rotations[4];
			hand.transform.rotation = rotations[5];
			forearm.transform.position = positions[4];
			hand.transform.position = positions[5];
		}
	}
}
