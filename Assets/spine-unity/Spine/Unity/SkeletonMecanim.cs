using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[RequireComponent(typeof(Animator))]
	public class SkeletonMecanim : SkeletonRenderer, ISkeletonAnimation
	{
		[Serializable]
		public class MecanimTranslator
		{
			public enum MixMode
			{
				AlwaysMix,
				MixNext,
				SpineStyle
			}

			protected class ClipInfos
			{
				public bool isInterruptionActive;

				public bool isLastFrameOfInterruption;

				public int clipInfoCount;

				public int nextClipInfoCount;

				public int interruptingClipInfoCount;

				public readonly List<AnimatorClipInfo> clipInfos = new List<AnimatorClipInfo>();

				public readonly List<AnimatorClipInfo> nextClipInfos = new List<AnimatorClipInfo>();

				public readonly List<AnimatorClipInfo> interruptingClipInfos = new List<AnimatorClipInfo>();

				public AnimatorStateInfo stateInfo;

				public AnimatorStateInfo nextStateInfo;

				public AnimatorStateInfo interruptingStateInfo;

				public float interruptingClipTimeAddition;
			}

			private class AnimationClipEqualityComparer : IEqualityComparer<AnimationClip>
			{
				internal static readonly IEqualityComparer<AnimationClip> Instance = new AnimationClipEqualityComparer();

				public bool Equals(AnimationClip x, AnimationClip y)
				{
					return x.GetInstanceID() == y.GetInstanceID();
				}

				public int GetHashCode(AnimationClip o)
				{
					return o.GetInstanceID();
				}
			}

			private class IntEqualityComparer : IEqualityComparer<int>
			{
				internal static readonly IEqualityComparer<int> Instance = new IntEqualityComparer();

				public bool Equals(int x, int y)
				{
					return x == y;
				}

				public int GetHashCode(int o)
				{
					return o;
				}
			}

			public bool autoReset = true;

			public MixMode[] layerMixModes = new MixMode[0];

			public MixBlend[] layerBlendModes = new MixBlend[0];

			private readonly Dictionary<int, Animation> animationTable = new Dictionary<int, Animation>(IntEqualityComparer.Instance);

			private readonly Dictionary<AnimationClip, int> clipNameHashCodeTable = new Dictionary<AnimationClip, int>(AnimationClipEqualityComparer.Instance);

			private readonly List<Animation> previousAnimations = new List<Animation>();

			protected ClipInfos[] layerClipInfos = new ClipInfos[0];

			private Animator animator;

			public Animator Animator
			{
				get
				{
					return animator;
				}
			}

			public void Initialize(Animator animator, SkeletonDataAsset skeletonDataAsset)
			{
				this.animator = animator;
				previousAnimations.Clear();
				animationTable.Clear();
				foreach (Animation animation in skeletonDataAsset.GetSkeletonData(true).Animations)
				{
					animationTable.Add(animation.Name.GetHashCode(), animation);
				}
				clipNameHashCodeTable.Clear();
				ClearClipInfosForLayers();
			}

			public void Apply(Skeleton skeleton)
			{
				if (layerMixModes.Length < animator.layerCount)
				{
					Array.Resize(ref layerMixModes, animator.layerCount);
				}
				InitClipInfosForLayers();
				int i = 0;
				for (int layerCount = animator.layerCount; i < layerCount; i++)
				{
					GetStateUpdatesFromAnimator(i);
				}
				if (autoReset)
				{
					List<Animation> list = previousAnimations;
					int j = 0;
					for (int count = list.Count; j < count; j++)
					{
						list[j].SetKeyedItemsToSetupPose(skeleton);
					}
					list.Clear();
					int k = 0;
					for (int layerCount2 = animator.layerCount; k < layerCount2; k++)
					{
						float num = ((k == 0) ? 1f : animator.GetLayerWeight(k));
						if (num <= 0f)
						{
							continue;
						}
						bool flag = animator.GetNextAnimatorStateInfo(k).fullPathHash != 0;
						bool isInterruptionActive;
						int clipInfoCount;
						int nextClipInfoCount;
						int interruptingClipInfoCount;
						IList<AnimatorClipInfo> clipInfo;
						IList<AnimatorClipInfo> nextClipInfo;
						IList<AnimatorClipInfo> interruptingClipInfo;
						bool shallInterpolateWeightTo;
						GetAnimatorClipInfos(k, out isInterruptionActive, out clipInfoCount, out nextClipInfoCount, out interruptingClipInfoCount, out clipInfo, out nextClipInfo, out interruptingClipInfo, out shallInterpolateWeightTo);
						for (int l = 0; l < clipInfoCount; l++)
						{
							AnimatorClipInfo animatorClipInfo = clipInfo[l];
							if (animatorClipInfo.weight * num != 0f)
							{
								list.Add(GetAnimation(animatorClipInfo.clip));
							}
						}
						if (flag)
						{
							for (int m = 0; m < nextClipInfoCount; m++)
							{
								AnimatorClipInfo animatorClipInfo2 = nextClipInfo[m];
								if (animatorClipInfo2.weight * num != 0f)
								{
									list.Add(GetAnimation(animatorClipInfo2.clip));
								}
							}
						}
						if (!isInterruptionActive)
						{
							continue;
						}
						for (int n = 0; n < interruptingClipInfoCount; n++)
						{
							AnimatorClipInfo animatorClipInfo3 = interruptingClipInfo[n];
							if ((shallInterpolateWeightTo ? ((animatorClipInfo3.weight + 1f) * 0.5f) : animatorClipInfo3.weight) * num != 0f)
							{
								list.Add(GetAnimation(animatorClipInfo3.clip));
							}
						}
					}
				}
				int num2 = 0;
				for (int layerCount3 = animator.layerCount; num2 < layerCount3; num2++)
				{
					float num3 = ((num2 == 0) ? 1f : animator.GetLayerWeight(num2));
					bool isInterruptionActive2;
					AnimatorStateInfo stateInfo;
					AnimatorStateInfo nextStateInfo;
					AnimatorStateInfo interruptingStateInfo;
					float interruptingClipTimeAddition;
					GetAnimatorStateInfos(num2, out isInterruptionActive2, out stateInfo, out nextStateInfo, out interruptingStateInfo, out interruptingClipTimeAddition);
					bool flag2 = nextStateInfo.fullPathHash != 0;
					int clipInfoCount2;
					int nextClipInfoCount2;
					int interruptingClipInfoCount2;
					IList<AnimatorClipInfo> clipInfo2;
					IList<AnimatorClipInfo> nextClipInfo2;
					IList<AnimatorClipInfo> interruptingClipInfo2;
					bool shallInterpolateWeightTo2;
					GetAnimatorClipInfos(num2, out isInterruptionActive2, out clipInfoCount2, out nextClipInfoCount2, out interruptingClipInfoCount2, out clipInfo2, out nextClipInfo2, out interruptingClipInfo2, out shallInterpolateWeightTo2);
					MixMode mixMode = layerMixModes[num2];
					MixBlend blend = ((num2 < layerBlendModes.Length) ? layerBlendModes[num2] : MixBlend.Replace);
					if (mixMode == MixMode.AlwaysMix)
					{
						for (int num4 = 0; num4 < clipInfoCount2; num4++)
						{
							AnimatorClipInfo animatorClipInfo4 = clipInfo2[num4];
							float num5 = animatorClipInfo4.weight * num3;
							if (num5 != 0f)
							{
								GetAnimation(animatorClipInfo4.clip).Apply(skeleton, 0f, AnimationTime(stateInfo.normalizedTime, animatorClipInfo4.clip.length, stateInfo.loop, stateInfo.speed < 0f), stateInfo.loop, null, num5, blend, MixDirection.In);
							}
						}
						if (flag2)
						{
							for (int num6 = 0; num6 < nextClipInfoCount2; num6++)
							{
								AnimatorClipInfo animatorClipInfo5 = nextClipInfo2[num6];
								float num7 = animatorClipInfo5.weight * num3;
								if (num7 != 0f)
								{
									GetAnimation(animatorClipInfo5.clip).Apply(skeleton, 0f, AnimationTime(nextStateInfo.normalizedTime, animatorClipInfo5.clip.length, nextStateInfo.speed < 0f), nextStateInfo.loop, null, num7, blend, MixDirection.In);
								}
							}
						}
						if (!isInterruptionActive2)
						{
							continue;
						}
						for (int num8 = 0; num8 < interruptingClipInfoCount2; num8++)
						{
							AnimatorClipInfo animatorClipInfo6 = interruptingClipInfo2[num8];
							float num9 = (shallInterpolateWeightTo2 ? ((animatorClipInfo6.weight + 1f) * 0.5f) : animatorClipInfo6.weight) * num3;
							if (num9 != 0f)
							{
								GetAnimation(animatorClipInfo6.clip).Apply(skeleton, 0f, AnimationTime(interruptingStateInfo.normalizedTime + interruptingClipTimeAddition, animatorClipInfo6.clip.length, interruptingStateInfo.speed < 0f), interruptingStateInfo.loop, null, num9, blend, MixDirection.In);
							}
						}
						continue;
					}
					int num10;
					for (num10 = 0; num10 < clipInfoCount2; num10++)
					{
						AnimatorClipInfo animatorClipInfo7 = clipInfo2[num10];
						if (animatorClipInfo7.weight * num3 != 0f)
						{
							GetAnimation(animatorClipInfo7.clip).Apply(skeleton, 0f, AnimationTime(stateInfo.normalizedTime, animatorClipInfo7.clip.length, stateInfo.loop, stateInfo.speed < 0f), stateInfo.loop, null, 1f, blend, MixDirection.In);
							break;
						}
					}
					for (; num10 < clipInfoCount2; num10++)
					{
						AnimatorClipInfo animatorClipInfo8 = clipInfo2[num10];
						float num11 = animatorClipInfo8.weight * num3;
						if (num11 != 0f)
						{
							GetAnimation(animatorClipInfo8.clip).Apply(skeleton, 0f, AnimationTime(stateInfo.normalizedTime, animatorClipInfo8.clip.length, stateInfo.loop, stateInfo.speed < 0f), stateInfo.loop, null, num11, blend, MixDirection.In);
						}
					}
					num10 = 0;
					if (flag2)
					{
						if (mixMode == MixMode.SpineStyle)
						{
							for (; num10 < nextClipInfoCount2; num10++)
							{
								AnimatorClipInfo animatorClipInfo9 = nextClipInfo2[num10];
								if (animatorClipInfo9.weight * num3 != 0f)
								{
									GetAnimation(animatorClipInfo9.clip).Apply(skeleton, 0f, AnimationTime(nextStateInfo.normalizedTime, animatorClipInfo9.clip.length, nextStateInfo.speed < 0f), nextStateInfo.loop, null, 1f, blend, MixDirection.In);
									break;
								}
							}
						}
						for (; num10 < nextClipInfoCount2; num10++)
						{
							AnimatorClipInfo animatorClipInfo10 = nextClipInfo2[num10];
							float num12 = animatorClipInfo10.weight * num3;
							if (num12 != 0f)
							{
								GetAnimation(animatorClipInfo10.clip).Apply(skeleton, 0f, AnimationTime(nextStateInfo.normalizedTime, animatorClipInfo10.clip.length, nextStateInfo.speed < 0f), nextStateInfo.loop, null, num12, blend, MixDirection.In);
							}
						}
					}
					num10 = 0;
					if (!isInterruptionActive2)
					{
						continue;
					}
					if (mixMode == MixMode.SpineStyle)
					{
						for (; num10 < interruptingClipInfoCount2; num10++)
						{
							AnimatorClipInfo animatorClipInfo11 = interruptingClipInfo2[num10];
							if ((shallInterpolateWeightTo2 ? ((animatorClipInfo11.weight + 1f) * 0.5f) : animatorClipInfo11.weight) * num3 != 0f)
							{
								GetAnimation(animatorClipInfo11.clip).Apply(skeleton, 0f, AnimationTime(interruptingStateInfo.normalizedTime + interruptingClipTimeAddition, animatorClipInfo11.clip.length, interruptingStateInfo.speed < 0f), interruptingStateInfo.loop, null, 1f, blend, MixDirection.In);
								break;
							}
						}
					}
					for (; num10 < interruptingClipInfoCount2; num10++)
					{
						AnimatorClipInfo animatorClipInfo12 = interruptingClipInfo2[num10];
						float num13 = (shallInterpolateWeightTo2 ? ((animatorClipInfo12.weight + 1f) * 0.5f) : animatorClipInfo12.weight) * num3;
						if (num13 != 0f)
						{
							GetAnimation(animatorClipInfo12.clip).Apply(skeleton, 0f, AnimationTime(interruptingStateInfo.normalizedTime + interruptingClipTimeAddition, animatorClipInfo12.clip.length, interruptingStateInfo.speed < 0f), interruptingStateInfo.loop, null, num13, blend, MixDirection.In);
						}
					}
				}
			}

			private static float AnimationTime(float normalizedTime, float clipLength, bool loop, bool reversed)
			{
				if (reversed)
				{
					normalizedTime = 1f - normalizedTime + (float)(int)normalizedTime + (float)(int)normalizedTime;
				}
				float num = normalizedTime * clipLength;
				if (loop)
				{
					return num;
				}
				if (!(clipLength - num < 71f / (678f * (float)Math.PI)))
				{
					return num;
				}
				return clipLength;
			}

			private static float AnimationTime(float normalizedTime, float clipLength, bool reversed)
			{
				if (reversed)
				{
					normalizedTime = 1f - normalizedTime + (float)(int)normalizedTime + (float)(int)normalizedTime;
				}
				return normalizedTime * clipLength;
			}

			private void InitClipInfosForLayers()
			{
				if (layerClipInfos.Length >= animator.layerCount)
				{
					return;
				}
				Array.Resize(ref layerClipInfos, animator.layerCount);
				int i = 0;
				for (int layerCount = animator.layerCount; i < layerCount; i++)
				{
					if (layerClipInfos[i] == null)
					{
						layerClipInfos[i] = new ClipInfos();
					}
				}
			}

			private void ClearClipInfosForLayers()
			{
				int i = 0;
				for (int num = layerClipInfos.Length; i < num; i++)
				{
					if (layerClipInfos[i] == null)
					{
						layerClipInfos[i] = new ClipInfos();
						continue;
					}
					layerClipInfos[i].isInterruptionActive = false;
					layerClipInfos[i].isLastFrameOfInterruption = false;
					layerClipInfos[i].clipInfos.Clear();
					layerClipInfos[i].nextClipInfos.Clear();
					layerClipInfos[i].interruptingClipInfos.Clear();
				}
			}

			private void GetStateUpdatesFromAnimator(int layer)
			{
				ClipInfos clipInfos = layerClipInfos[layer];
				int currentAnimatorClipInfoCount = animator.GetCurrentAnimatorClipInfoCount(layer);
				int nextAnimatorClipInfoCount = animator.GetNextAnimatorClipInfoCount(layer);
				List<AnimatorClipInfo> clipInfos2 = clipInfos.clipInfos;
				List<AnimatorClipInfo> nextClipInfos = clipInfos.nextClipInfos;
				List<AnimatorClipInfo> interruptingClipInfos = clipInfos.interruptingClipInfos;
				clipInfos.isInterruptionActive = currentAnimatorClipInfoCount == 0 && clipInfos2.Count != 0 && nextAnimatorClipInfoCount == 0 && nextClipInfos.Count != 0;
				if (clipInfos.isInterruptionActive)
				{
					AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(layer);
					clipInfos.isLastFrameOfInterruption = nextAnimatorStateInfo.fullPathHash == 0;
					if (!clipInfos.isLastFrameOfInterruption)
					{
						clipInfos.interruptingClipInfoCount = interruptingClipInfos.Count;
						animator.GetNextAnimatorClipInfo(layer, interruptingClipInfos);
						float normalizedTime = clipInfos.interruptingStateInfo.normalizedTime;
						float normalizedTime2 = nextAnimatorStateInfo.normalizedTime;
						clipInfos.interruptingClipTimeAddition = normalizedTime2 - normalizedTime;
						clipInfos.interruptingStateInfo = nextAnimatorStateInfo;
					}
					return;
				}
				clipInfos.clipInfoCount = currentAnimatorClipInfoCount;
				clipInfos.nextClipInfoCount = nextAnimatorClipInfoCount;
				clipInfos.interruptingClipInfoCount = 0;
				clipInfos.isLastFrameOfInterruption = false;
				if (clipInfos2.Capacity < currentAnimatorClipInfoCount)
				{
					clipInfos2.Capacity = currentAnimatorClipInfoCount;
				}
				if (nextClipInfos.Capacity < nextAnimatorClipInfoCount)
				{
					nextClipInfos.Capacity = nextAnimatorClipInfoCount;
				}
				animator.GetCurrentAnimatorClipInfo(layer, clipInfos2);
				animator.GetNextAnimatorClipInfo(layer, nextClipInfos);
				clipInfos.stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
				clipInfos.nextStateInfo = animator.GetNextAnimatorStateInfo(layer);
			}

			private void GetAnimatorClipInfos(int layer, out bool isInterruptionActive, out int clipInfoCount, out int nextClipInfoCount, out int interruptingClipInfoCount, out IList<AnimatorClipInfo> clipInfo, out IList<AnimatorClipInfo> nextClipInfo, out IList<AnimatorClipInfo> interruptingClipInfo, out bool shallInterpolateWeightTo1)
			{
				ClipInfos clipInfos = layerClipInfos[layer];
				isInterruptionActive = clipInfos.isInterruptionActive;
				clipInfoCount = clipInfos.clipInfoCount;
				nextClipInfoCount = clipInfos.nextClipInfoCount;
				interruptingClipInfoCount = clipInfos.interruptingClipInfoCount;
				clipInfo = clipInfos.clipInfos;
				nextClipInfo = clipInfos.nextClipInfos;
				interruptingClipInfo = (isInterruptionActive ? clipInfos.interruptingClipInfos : null);
				shallInterpolateWeightTo1 = clipInfos.isLastFrameOfInterruption;
			}

			private void GetAnimatorStateInfos(int layer, out bool isInterruptionActive, out AnimatorStateInfo stateInfo, out AnimatorStateInfo nextStateInfo, out AnimatorStateInfo interruptingStateInfo, out float interruptingClipTimeAddition)
			{
				ClipInfos clipInfos = layerClipInfos[layer];
				isInterruptionActive = clipInfos.isInterruptionActive;
				stateInfo = clipInfos.stateInfo;
				nextStateInfo = clipInfos.nextStateInfo;
				interruptingStateInfo = clipInfos.interruptingStateInfo;
				interruptingClipTimeAddition = (clipInfos.isLastFrameOfInterruption ? clipInfos.interruptingClipTimeAddition : 0f);
			}

			private Animation GetAnimation(AnimationClip clip)
			{
				int value;
				if (!clipNameHashCodeTable.TryGetValue(clip, out value))
				{
					value = clip.name.GetHashCode();
					clipNameHashCodeTable.Add(clip, value);
				}
				Animation value2;
				animationTable.TryGetValue(value, out value2);
				return value2;
			}
		}

		[SerializeField]
		protected MecanimTranslator translator;

		public MecanimTranslator Translator
		{
			get
			{
				return translator;
			}
		}

		protected event UpdateBonesDelegate _UpdateLocal;

		protected event UpdateBonesDelegate _UpdateWorld;

		protected event UpdateBonesDelegate _UpdateComplete;

		public event UpdateBonesDelegate UpdateLocal
		{
			add
			{
				_UpdateLocal += value;
			}
			remove
			{
				_UpdateLocal -= value;
			}
		}

		public event UpdateBonesDelegate UpdateWorld
		{
			add
			{
				_UpdateWorld += value;
			}
			remove
			{
				_UpdateWorld -= value;
			}
		}

		public event UpdateBonesDelegate UpdateComplete
		{
			add
			{
				_UpdateComplete += value;
			}
			remove
			{
				_UpdateComplete -= value;
			}
		}

		public override void Initialize(bool overwrite)
		{
			if (valid && !overwrite)
			{
				return;
			}
			base.Initialize(overwrite);
			if (valid)
			{
				if (translator == null)
				{
					translator = new MecanimTranslator();
				}
				translator.Initialize(GetComponent<Animator>(), skeletonDataAsset);
			}
		}

		public void Update()
		{
			if (valid)
			{
				translator.Apply(skeleton);
				if (this._UpdateLocal != null)
				{
					this._UpdateLocal(this);
				}
				skeleton.UpdateWorldTransform();
				if (this._UpdateWorld != null)
				{
					this._UpdateWorld(this);
					skeleton.UpdateWorldTransform();
				}
				if (this._UpdateComplete != null)
				{
					this._UpdateComplete(this);
				}
			}
		}
	}
}
