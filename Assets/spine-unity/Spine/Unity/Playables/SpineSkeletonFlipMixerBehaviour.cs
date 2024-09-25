using UnityEngine;
using UnityEngine.Playables;

namespace Spine.Unity.Playables
{
	public class SpineSkeletonFlipMixerBehaviour : PlayableBehaviour
	{
		private float originalScaleX;

		private float originalScaleY;

		private float baseScaleX;

		private float baseScaleY;

		private SpinePlayableHandleBase playableHandle;

		private bool m_FirstFrameHappened;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			playableHandle = playerData as SpinePlayableHandleBase;
			if (playableHandle == null)
			{
				return;
			}
			Skeleton skeleton = playableHandle.Skeleton;
			if (!m_FirstFrameHappened)
			{
				originalScaleX = skeleton.ScaleX;
				originalScaleY = skeleton.ScaleY;
				baseScaleX = Mathf.Abs(originalScaleX);
				baseScaleY = Mathf.Abs(originalScaleY);
				m_FirstFrameHappened = true;
			}
			int inputCount = playable.GetInputCount();
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				SpineSkeletonFlipBehaviour behaviour = ((ScriptPlayable<SpineSkeletonFlipBehaviour>)playable.GetInput(i)).GetBehaviour();
				num += inputWeight;
				if (inputWeight > num2)
				{
					SetSkeletonScaleFromFlip(skeleton, behaviour.flipX, behaviour.flipY);
					num2 = inputWeight;
				}
				if (!Mathf.Approximately(inputWeight, 0f))
				{
					num3++;
				}
			}
			if (num3 != 1 && 1f - num > num2)
			{
				skeleton.scaleX = originalScaleX;
				skeleton.scaleY = originalScaleY;
			}
		}

		public void SetSkeletonScaleFromFlip(Skeleton skeleton, bool flipX, bool flipY)
		{
			skeleton.scaleX = (flipX ? (0f - baseScaleX) : baseScaleX);
			skeleton.scaleY = (flipY ? (0f - baseScaleY) : baseScaleY);
		}

		public override void OnGraphStop(Playable playable)
		{
			m_FirstFrameHappened = false;
			if (!(playableHandle == null))
			{
				Skeleton skeleton = playableHandle.Skeleton;
				skeleton.scaleX = originalScaleX;
				skeleton.scaleY = originalScaleY;
			}
		}
	}
}
