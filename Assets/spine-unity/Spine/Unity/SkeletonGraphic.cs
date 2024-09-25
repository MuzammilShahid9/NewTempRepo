using UnityEngine;
using UnityEngine.UI;

namespace Spine.Unity
{
	[ExecuteAlways]
	[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Spine/SkeletonGraphic (Unity UI Canvas)")]
	public class SkeletonGraphic : MaskableGraphic, ISkeletonComponent, IAnimationStateComponent, ISkeletonAnimation, IHasSkeletonDataAsset
	{
		public SkeletonDataAsset skeletonDataAsset;

		[SpineSkin("", "skeletonDataAsset", true, false, false)]
		public string initialSkinName = "default";

		public bool initialFlipX;

		public bool initialFlipY;

		[SpineAnimation("", "skeletonDataAsset", true, false)]
		public string startingAnimation;

		public bool startingLoop;

		public float timeScale = 1f;

		public bool freeze;

		public bool unscaledTime;

		private Texture overrideTexture;

		protected Skeleton skeleton;

		protected AnimationState state;

		[SerializeField]
		protected MeshGenerator meshGenerator = new MeshGenerator();

		private DoubleBuffered<MeshRendererBuffers.SmartMesh> meshBuffers;

		private SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();

		public SkeletonDataAsset SkeletonDataAsset
		{
			get
			{
				return skeletonDataAsset;
			}
		}

		public Texture OverrideTexture
		{
			get
			{
				return overrideTexture;
			}
			set
			{
				overrideTexture = value;
				base.canvasRenderer.SetTexture(mainTexture);
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (overrideTexture != null)
				{
					return overrideTexture;
				}
				if (!(skeletonDataAsset == null))
				{
					return skeletonDataAsset.atlasAssets[0].PrimaryMaterial.mainTexture;
				}
				return null;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				return skeleton;
			}
			internal set
			{
				skeleton = value;
			}
		}

		public SkeletonData SkeletonData
		{
			get
			{
				if (skeleton != null)
				{
					return skeleton.data;
				}
				return null;
			}
		}

		public bool IsValid
		{
			get
			{
				return skeleton != null;
			}
		}

		public AnimationState AnimationState
		{
			get
			{
				return state;
			}
		}

		public MeshGenerator MeshGenerator
		{
			get
			{
				return meshGenerator;
			}
		}

		public event UpdateBonesDelegate UpdateLocal;

		public event UpdateBonesDelegate UpdateWorld;

		public event UpdateBonesDelegate UpdateComplete;

		public event MeshGeneratorDelegate OnPostProcessVertices;

		public static SkeletonGraphic NewSkeletonGraphicGameObject(SkeletonDataAsset skeletonDataAsset, Transform parent, Material material)
		{
			SkeletonGraphic skeletonGraphic = AddSkeletonGraphicComponent(new GameObject("New Spine GameObject"), skeletonDataAsset, material);
			if (parent != null)
			{
				skeletonGraphic.transform.SetParent(parent, false);
			}
			return skeletonGraphic;
		}

		public static SkeletonGraphic AddSkeletonGraphicComponent(GameObject gameObject, SkeletonDataAsset skeletonDataAsset, Material material)
		{
			SkeletonGraphic skeletonGraphic = gameObject.AddComponent<SkeletonGraphic>();
			if (skeletonDataAsset != null)
			{
				skeletonGraphic.material = material;
				skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
				skeletonGraphic.Initialize(false);
			}
			return skeletonGraphic;
		}

		protected override void Awake()
		{
			base.Awake();
			if (!IsValid)
			{
				Initialize(false);
				Rebuild(CanvasUpdate.PreRender);
			}
		}

		public override void Rebuild(CanvasUpdate update)
		{
			base.Rebuild(update);
			if (!base.canvasRenderer.cull && update == CanvasUpdate.PreRender)
			{
				UpdateMesh();
			}
		}

		public virtual void Update()
		{
			if (!freeze)
			{
				Update(unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
			}
		}

		public virtual void Update(float deltaTime)
		{
			if (IsValid)
			{
				deltaTime *= timeScale;
				skeleton.Update(deltaTime);
				state.Update(deltaTime);
				state.Apply(skeleton);
				if (this.UpdateLocal != null)
				{
					this.UpdateLocal(this);
				}
				skeleton.UpdateWorldTransform();
				if (this.UpdateWorld != null)
				{
					this.UpdateWorld(this);
					skeleton.UpdateWorldTransform();
				}
				if (this.UpdateComplete != null)
				{
					this.UpdateComplete(this);
				}
			}
		}

		public void LateUpdate()
		{
			if (!freeze)
			{
				UpdateMesh();
			}
		}

		public Mesh GetLastMesh()
		{
			return meshBuffers.GetCurrent().mesh;
		}

		public void Clear()
		{
			skeleton = null;
			base.canvasRenderer.Clear();
		}

		public void Initialize(bool overwrite)
		{
			if ((IsValid && !overwrite) || skeletonDataAsset == null)
			{
				return;
			}
			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(false);
			if (skeletonData == null || skeletonDataAsset.atlasAssets.Length == 0 || skeletonDataAsset.atlasAssets[0].MaterialCount <= 0)
			{
				return;
			}
			state = new AnimationState(skeletonDataAsset.GetAnimationStateData());
			if (state == null)
			{
				Clear();
				return;
			}
			skeleton = new Skeleton(skeletonData)
			{
				scaleX = ((!initialFlipX) ? 1 : (-1)),
				scaleY = ((!initialFlipY) ? 1 : (-1))
			};
			meshBuffers = new DoubleBuffered<MeshRendererBuffers.SmartMesh>();
			base.canvasRenderer.SetTexture(mainTexture);
			if (!string.IsNullOrEmpty(initialSkinName))
			{
				skeleton.SetSkin(initialSkinName);
			}
			if (!string.IsNullOrEmpty(startingAnimation))
			{
				Animation animation = skeletonDataAsset.GetSkeletonData(false).FindAnimation(startingAnimation);
				if (animation != null)
				{
					animation.PoseSkeleton(skeleton, 0f);
					skeleton.UpdateWorldTransform();
					state.SetAnimation(0, animation, startingLoop);
				}
				else
				{
					startingAnimation = string.Empty;
				}
			}
		}

		public void UpdateMesh()
		{
			if (IsValid)
			{
				skeleton.SetColor(color);
				MeshRendererBuffers.SmartMesh next = meshBuffers.GetNext();
				SkeletonRendererInstruction skeletonRendererInstruction = currentInstructions;
				MeshGenerator.GenerateSingleSubmeshInstruction(skeletonRendererInstruction, skeleton, material);
				bool flag = SkeletonRendererInstruction.GeometryNotEqual(skeletonRendererInstruction, next.instructionUsed);
				meshGenerator.Begin();
				if (skeletonRendererInstruction.hasActiveClipping)
				{
					meshGenerator.AddSubmesh(skeletonRendererInstruction.submeshInstructions.Items[0], flag);
				}
				else
				{
					meshGenerator.BuildMeshWithArrays(skeletonRendererInstruction, flag);
				}
				if (base.canvas != null)
				{
					meshGenerator.ScaleVertexData(base.canvas.referencePixelsPerUnit);
				}
				if (this.OnPostProcessVertices != null)
				{
					this.OnPostProcessVertices(meshGenerator.Buffers);
				}
				Mesh mesh = next.mesh;
				meshGenerator.FillVertexData(mesh);
				if (flag)
				{
					meshGenerator.FillTrianglesSingle(mesh);
				}
				meshGenerator.FillLateVertexData(mesh);
				base.canvasRenderer.SetMesh(mesh);
				next.instructionUsed.Set(skeletonRendererInstruction);
			}
		}
	}
}
