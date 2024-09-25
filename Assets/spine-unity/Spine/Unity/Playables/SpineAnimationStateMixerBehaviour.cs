using UnityEngine;
using UnityEngine.Playables;

namespace Spine.Unity.Playables
{
	public class SpineAnimationStateMixerBehaviour : PlayableBehaviour
	{
		private float[] lastInputWeights;

		public int trackIndex;

		private AnimationState dummyAnimationState;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			SkeletonAnimation skeletonAnimation = playerData as SkeletonAnimation;
			if (skeletonAnimation == null)
			{
				return;
			}
			Skeleton skeleton = skeletonAnimation.Skeleton;
			AnimationState animationState = skeletonAnimation.AnimationState;
			if (!Application.isPlaying)
			{
				PreviewEditModePose(playable, skeletonAnimation);
				return;
			}
			int inputCount = playable.GetInputCount();
			if (lastInputWeights == null || lastInputWeights.Length < inputCount)
			{
				lastInputWeights = new float[inputCount];
				for (int i = 0; i < inputCount; i++)
				{
					lastInputWeights[i] = 0f;
				}
			}
			float[] array = lastInputWeights;
			for (int j = 0; j < inputCount; j++)
			{
				float num = array[j];
				float inputWeight = playable.GetInputWeight(j);
				bool num2 = inputWeight > num;
				array[j] = inputWeight;
				if (!num2)
				{
					continue;
				}
				SpineAnimationStateBehaviour behaviour = ((ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(j)).GetBehaviour();
				if (behaviour.animationReference == null)
				{
					float mixDuration = (behaviour.customDuration ? behaviour.mixDuration : animationState.Data.DefaultMix);
					animationState.SetEmptyAnimation(trackIndex, mixDuration);
				}
				else if (behaviour.animationReference.Animation != null)
				{
					TrackEntry trackEntry = animationState.SetAnimation(trackIndex, behaviour.animationReference.Animation, behaviour.loop);
					trackEntry.EventThreshold = behaviour.eventThreshold;
					trackEntry.DrawOrderThreshold = behaviour.drawOrderThreshold;
					trackEntry.AttachmentThreshold = behaviour.attachmentThreshold;
					if (behaviour.customDuration)
					{
						trackEntry.MixDuration = behaviour.mixDuration;
					}
				}
				skeletonAnimation.Update(0f);
				skeletonAnimation.LateUpdate();
			}
		}

		public void PreviewEditModePose(Playable playable, SkeletonAnimation spineComponent)
		{
			if (Application.isPlaying || spineComponent == null)
			{
				return;
			}
			int inputCount = playable.GetInputCount();
			int num = -1;
			for (int i = 0; i < inputCount; i++)
			{
				if (playable.GetInputWeight(i) >= 1f)
				{
					num = i;
				}
			}
			if (num == -1)
			{
				return;
			}
			ScriptPlayable<SpineAnimationStateBehaviour> playable2 = (ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(num);
			SpineAnimationStateBehaviour behaviour = playable2.GetBehaviour();
			Skeleton skeleton = spineComponent.Skeleton;
			if (behaviour.animationReference != null && spineComponent.SkeletonDataAsset.GetSkeletonData(true) != behaviour.animationReference.SkeletonDataAsset.GetSkeletonData(true))
			{
				Debug.LogWarningFormat("SpineAnimationStateMixerBehaviour tried to apply an animation for the wrong skeleton. Expected {0}. Was {1}", spineComponent.SkeletonDataAsset, behaviour.animationReference.SkeletonDataAsset);
			}
			Animation animation = null;
			float trackTime = 0f;
			bool loop = false;
			if (num != 0 && inputCount > 1)
			{
				ScriptPlayable<SpineAnimationStateBehaviour> playable3 = (ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(num - 1);
				SpineAnimationStateBehaviour behaviour2 = playable3.GetBehaviour();
				animation = ((behaviour2.animationReference != null) ? behaviour2.animationReference.Animation : null);
				trackTime = (float)playable3.GetTime();
				loop = behaviour2.loop;
			}
			Animation animation2 = ((behaviour.animationReference != null) ? behaviour.animationReference.Animation : null);
			float num2 = (float)playable2.GetTime();
			float num3 = behaviour.mixDuration;
			if (!behaviour.customDuration && animation != null && animation2 != null)
			{
				num3 = spineComponent.AnimationState.Data.GetMix(animation, animation2);
			}
			if (animation != null && num3 > 0f && num2 < num3)
			{
				dummyAnimationState = dummyAnimationState ?? new AnimationState(spineComponent.skeletonDataAsset.GetAnimationStateData());
				TrackEntry trackEntry = dummyAnimationState.GetCurrent(0);
				TrackEntry trackEntry2 = ((trackEntry != null) ? trackEntry.mixingFrom : null);
				if (trackEntry == null || trackEntry.animation != animation2 || trackEntry2 == null || trackEntry2.animation != animation)
				{
					dummyAnimationState.ClearTracks();
					trackEntry2 = dummyAnimationState.SetAnimation(0, animation, loop);
					trackEntry2.AllowImmediateQueue();
					if (animation2 != null)
					{
						trackEntry = dummyAnimationState.SetAnimation(0, animation2, behaviour.loop);
					}
				}
				trackEntry2.trackTime = trackTime;
				if (trackEntry != null)
				{
					trackEntry.trackTime = num2;
					trackEntry.mixTime = num2;
				}
				skeleton.SetToSetupPose();
				dummyAnimationState.Update(0f);
				dummyAnimationState.Apply(skeleton);
			}
			else
			{
				skeleton.SetToSetupPose();
				if (animation2 != null)
				{
					animation2.PoseSkeleton(skeleton, num2, behaviour.loop);
				}
			}
		}
	}
}
