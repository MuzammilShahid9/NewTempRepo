using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Spine/SkeletonGameObjectsBone")]
	public class SkeletonUtilityBone : MonoBehaviour
	{
		public enum Mode
		{
			Follow,
			Override
		}

		public enum UpdatePhase
		{
			Local,
			World,
			Complete
		}

		public string boneName;

		public Transform parentReference;

		public Mode mode;

		public bool position;

		public bool rotation;

		public bool scale;

		public bool zPosition = true;

		[Range(0f, 1f)]
		public float overrideAlpha = 1f;

		[NonSerialized]
		public SkeletonUtility hierarchy;

		[NonSerialized]
		public Bone bone;

		[NonSerialized]
		public bool transformLerpComplete;

		[NonSerialized]
		public bool valid;

		private Transform cachedTransform;

		private Transform skeletonTransform;

		private bool incompatibleTransformMode;

		public bool IncompatibleTransformMode
		{
			get
			{
				return incompatibleTransformMode;
			}
		}

		public void Reset()
		{
			bone = null;
			cachedTransform = base.transform;
			valid = hierarchy != null && hierarchy.skeletonRenderer != null && hierarchy.skeletonRenderer.valid;
			if (valid)
			{
				skeletonTransform = hierarchy.transform;
				hierarchy.OnReset -= HandleOnReset;
				hierarchy.OnReset += HandleOnReset;
				DoUpdate(UpdatePhase.Local);
			}
		}

		private void OnEnable()
		{
			hierarchy = base.transform.GetComponentInParent<SkeletonUtility>();
			if (!(hierarchy == null))
			{
				hierarchy.RegisterBone(this);
				hierarchy.OnReset += HandleOnReset;
			}
		}

		private void HandleOnReset()
		{
			Reset();
		}

		private void OnDisable()
		{
			if (hierarchy != null)
			{
				hierarchy.OnReset -= HandleOnReset;
				hierarchy.UnregisterBone(this);
			}
		}

		public void DoUpdate(UpdatePhase phase)
		{
			if (!valid)
			{
				Reset();
				return;
			}
			Skeleton skeleton = hierarchy.skeletonRenderer.skeleton;
			if (bone == null)
			{
				if (string.IsNullOrEmpty(boneName))
				{
					return;
				}
				bone = skeleton.FindBone(boneName);
				if (bone == null)
				{
					Debug.LogError("Bone not found: " + boneName, this);
					return;
				}
			}
			Transform transform = cachedTransform;
			float num = Mathf.Sign(skeleton.scaleX * skeleton.scaleY);
			if (mode == Mode.Follow)
			{
				switch (phase)
				{
				case UpdatePhase.Local:
					if (position)
					{
						transform.localPosition = new Vector3(bone.x, bone.y, 0f);
					}
					if (rotation)
					{
						if (bone.data.transformMode.InheritsRotation())
						{
							transform.localRotation = Quaternion.Euler(0f, 0f, bone.rotation);
						}
						else
						{
							Vector3 eulerAngles2 = skeletonTransform.rotation.eulerAngles;
							transform.rotation = Quaternion.Euler(eulerAngles2.x, eulerAngles2.y, eulerAngles2.z + bone.WorldRotationX * num);
						}
					}
					if (scale)
					{
						transform.localScale = new Vector3(bone.scaleX, bone.scaleY, 1f);
						incompatibleTransformMode = BoneTransformModeIncompatible(bone);
					}
					break;
				case UpdatePhase.World:
				case UpdatePhase.Complete:
					if (!bone.appliedValid)
					{
						bone.UpdateAppliedTransform();
					}
					if (position)
					{
						transform.localPosition = new Vector3(bone.ax, bone.ay, 0f);
					}
					if (rotation)
					{
						if (bone.data.transformMode.InheritsRotation())
						{
							transform.localRotation = Quaternion.Euler(0f, 0f, bone.AppliedRotation);
						}
						else
						{
							Vector3 eulerAngles = skeletonTransform.rotation.eulerAngles;
							transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + bone.WorldRotationX * num);
						}
					}
					if (scale)
					{
						transform.localScale = new Vector3(bone.ascaleX, bone.ascaleY, 1f);
						incompatibleTransformMode = BoneTransformModeIncompatible(bone);
					}
					break;
				}
			}
			else
			{
				if (mode != Mode.Override || transformLerpComplete)
				{
					return;
				}
				if (parentReference == null)
				{
					if (position)
					{
						Vector3 localPosition = transform.localPosition;
						bone.x = Mathf.Lerp(bone.x, localPosition.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, localPosition.y, overrideAlpha);
					}
					if (rotation)
					{
						float appliedRotation = Mathf.LerpAngle(bone.Rotation, transform.localRotation.eulerAngles.z, overrideAlpha);
						bone.Rotation = appliedRotation;
						bone.AppliedRotation = appliedRotation;
					}
					if (scale)
					{
						Vector3 localScale = transform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, localScale.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, localScale.y, overrideAlpha);
					}
				}
				else
				{
					if (transformLerpComplete)
					{
						return;
					}
					if (position)
					{
						Vector3 vector = parentReference.InverseTransformPoint(transform.position);
						bone.x = Mathf.Lerp(bone.x, vector.x, overrideAlpha);
						bone.y = Mathf.Lerp(bone.y, vector.y, overrideAlpha);
					}
					if (rotation)
					{
						float appliedRotation2 = Mathf.LerpAngle(bone.Rotation, Quaternion.LookRotation(Vector3.forward, parentReference.InverseTransformDirection(transform.up)).eulerAngles.z, overrideAlpha);
						bone.Rotation = appliedRotation2;
						bone.AppliedRotation = appliedRotation2;
					}
					if (scale)
					{
						Vector3 localScale2 = transform.localScale;
						bone.scaleX = Mathf.Lerp(bone.scaleX, localScale2.x, overrideAlpha);
						bone.scaleY = Mathf.Lerp(bone.scaleY, localScale2.y, overrideAlpha);
					}
					incompatibleTransformMode = BoneTransformModeIncompatible(bone);
				}
				transformLerpComplete = true;
			}
		}

		public static bool BoneTransformModeIncompatible(Bone bone)
		{
			return !bone.data.transformMode.InheritsScale();
		}

		public void AddBoundingBox(string skinName, string slotName, string attachmentName)
		{
			SkeletonUtility.AddBoneRigidbody2D(base.transform.gameObject);
			SkeletonUtility.AddBoundingBoxGameObject(bone.skeleton, skinName, slotName, attachmentName, base.transform);
		}
	}
}
