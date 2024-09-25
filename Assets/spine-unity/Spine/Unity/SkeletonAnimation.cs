using UnityEngine;

namespace Spine.Unity
{
	[ExecuteAlways]
	[AddComponentMenu("Spine/SkeletonAnimation")]
	public class SkeletonAnimation : SkeletonRenderer, ISkeletonAnimation, IAnimationStateComponent
	{
		public AnimationState state;

		[SerializeField]
		[SpineAnimation("", "", true, false)]
		private string _animationName;

		public bool loop;

		public float timeScale = 1f;

		public AnimationState AnimationState
		{
			get
			{
				return state;
			}
		}

		public string AnimationName
		{
			get
			{
				if (!valid)
				{
					return _animationName;
				}
				TrackEntry current = state.GetCurrent(0);
				if (current != null)
				{
					return current.Animation.Name;
				}
				return null;
			}
			set
			{
				if (_animationName == value)
				{
					return;
				}
				_animationName = value;
				if (string.IsNullOrEmpty(value))
				{
					state.ClearTrack(0);
					return;
				}
				Animation animation = skeletonDataAsset.GetSkeletonData(false).FindAnimation(value);
				if (animation != null)
				{
					state.SetAnimation(0, animation, loop);
				}
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

		public static SkeletonAnimation AddToGameObject(GameObject gameObject, SkeletonDataAsset skeletonDataAsset)
		{
			return SkeletonRenderer.AddSpineComponent<SkeletonAnimation>(gameObject, skeletonDataAsset);
		}

		public static SkeletonAnimation NewSkeletonAnimationGameObject(SkeletonDataAsset skeletonDataAsset)
		{
			return SkeletonRenderer.NewSpineGameObject<SkeletonAnimation>(skeletonDataAsset);
		}

		public override void ClearState()
		{
			base.ClearState();
			if (state != null)
			{
				state.ClearTracks();
			}
		}

		public override void Initialize(bool overwrite)
		{
			if (valid && !overwrite)
			{
				return;
			}
			base.Initialize(overwrite);
			if (!valid)
			{
				return;
			}
			state = new AnimationState(skeletonDataAsset.GetAnimationStateData());
			if (!string.IsNullOrEmpty(_animationName))
			{
				Animation animation = skeletonDataAsset.GetSkeletonData(false).FindAnimation(_animationName);
				if (animation != null)
				{
					animation.PoseSkeleton(skeleton, 0f);
					skeleton.UpdateWorldTransform();
					state.SetAnimation(0, animation, loop);
				}
			}
		}

		private void Update()
		{
			Update(Time.deltaTime);
		}

		public void Update(float deltaTime)
		{
			if (valid)
			{
				deltaTime *= timeScale;
				skeleton.Update(deltaTime);
				state.Update(deltaTime);
				state.Apply(skeleton);
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
