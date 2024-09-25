using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class SkeletonPartsRenderer : MonoBehaviour
	{
		private MeshGenerator meshGenerator;

		private MeshRenderer meshRenderer;

		private MeshFilter meshFilter;

		private MeshRendererBuffers buffers;

		private SkeletonRendererInstruction currentInstructions = new SkeletonRendererInstruction();

		public MeshGenerator MeshGenerator
		{
			get
			{
				LazyIntialize();
				return meshGenerator;
			}
		}

		public MeshRenderer MeshRenderer
		{
			get
			{
				LazyIntialize();
				return meshRenderer;
			}
		}

		public MeshFilter MeshFilter
		{
			get
			{
				LazyIntialize();
				return meshFilter;
			}
		}

		private void LazyIntialize()
		{
			if (buffers == null)
			{
				buffers = new MeshRendererBuffers();
				buffers.Initialize();
				if (meshGenerator == null)
				{
					meshGenerator = new MeshGenerator();
					meshFilter = GetComponent<MeshFilter>();
					meshRenderer = GetComponent<MeshRenderer>();
					currentInstructions.Clear();
				}
			}
		}

		public void ClearMesh()
		{
			LazyIntialize();
			meshFilter.sharedMesh = null;
		}

		public void RenderParts(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh)
		{
			LazyIntialize();
			MeshRendererBuffers.SmartMesh nextMesh = buffers.GetNextMesh();
			currentInstructions.SetWithSubset(instructions, startSubmesh, endSubmesh);
			bool flag = SkeletonRendererInstruction.GeometryNotEqual(currentInstructions, nextMesh.instructionUsed);
			SubmeshInstruction[] items = currentInstructions.submeshInstructions.Items;
			meshGenerator.Begin();
			if (currentInstructions.hasActiveClipping)
			{
				for (int i = 0; i < currentInstructions.submeshInstructions.Count; i++)
				{
					meshGenerator.AddSubmesh(items[i], flag);
				}
			}
			else
			{
				meshGenerator.BuildMeshWithArrays(currentInstructions, flag);
			}
			buffers.UpdateSharedMaterials(currentInstructions.submeshInstructions);
			Mesh mesh = nextMesh.mesh;
			if (meshGenerator.VertexCount <= 0)
			{
				flag = false;
				mesh.Clear();
			}
			else
			{
				meshGenerator.FillVertexData(mesh);
				if (flag)
				{
					meshGenerator.FillTriangles(mesh);
					meshRenderer.sharedMaterials = buffers.GetUpdatedSharedMaterialsArray();
				}
				else if (buffers.MaterialsChangedInLastUpdate())
				{
					meshRenderer.sharedMaterials = buffers.GetUpdatedSharedMaterialsArray();
				}
				meshGenerator.FillLateVertexData(mesh);
			}
			meshFilter.sharedMesh = mesh;
			nextMesh.instructionUsed.Set(currentInstructions);
		}

		public void SetPropertyBlock(MaterialPropertyBlock block)
		{
			LazyIntialize();
			meshRenderer.SetPropertyBlock(block);
		}

		public static SkeletonPartsRenderer NewPartsRendererGameObject(Transform parent, string name, int sortingOrder = 0)
		{
			GameObject obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
			obj.transform.SetParent(parent, false);
			SkeletonPartsRenderer skeletonPartsRenderer = obj.AddComponent<SkeletonPartsRenderer>();
			skeletonPartsRenderer.MeshRenderer.sortingOrder = sortingOrder;
			return skeletonPartsRenderer;
		}
	}
}
