using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[DisallowMultipleComponent]
	public class SlotBlendModes : MonoBehaviour
	{
		public struct MaterialTexturePair
		{
			public Texture2D texture2D;

			public Material material;
		}

		internal class MaterialWithRefcount
		{
			public Material materialClone;

			public int refcount = 1;

			public MaterialWithRefcount(Material mat)
			{
				materialClone = mat;
			}
		}

		internal struct SlotMaterialTextureTuple
		{
			public Slot slot;

			public Texture2D texture2D;

			public Material material;

			public SlotMaterialTextureTuple(Slot slot, Material material, Texture2D texture)
			{
				this.slot = slot;
				this.material = material;
				texture2D = texture;
			}
		}

		private static Dictionary<MaterialTexturePair, MaterialWithRefcount> materialTable;

		public Material multiplyMaterialSource;

		public Material screenMaterialSource;

		private Texture2D texture;

		private SlotMaterialTextureTuple[] slotsWithCustomMaterial = new SlotMaterialTextureTuple[0];

		internal static Dictionary<MaterialTexturePair, MaterialWithRefcount> MaterialTable
		{
			get
			{
				if (materialTable == null)
				{
					materialTable = new Dictionary<MaterialTexturePair, MaterialWithRefcount>();
				}
				return materialTable;
			}
		}

		public bool Applied { get; private set; }

		internal static Material GetOrAddMaterialFor(Material materialSource, Texture2D texture)
		{
			if (materialSource == null || texture == null)
			{
				return null;
			}
			Dictionary<MaterialTexturePair, MaterialWithRefcount> dictionary = MaterialTable;
			MaterialTexturePair materialTexturePair = default(MaterialTexturePair);
			materialTexturePair.material = materialSource;
			materialTexturePair.texture2D = texture;
			MaterialTexturePair key = materialTexturePair;
			MaterialWithRefcount value;
			if (!dictionary.TryGetValue(key, out value))
			{
				value = new MaterialWithRefcount(new Material(materialSource));
				Material materialClone = value.materialClone;
				materialClone.name = "(Clone)" + texture.name + "-" + materialSource.name;
				materialClone.mainTexture = texture;
				dictionary[key] = value;
			}
			else
			{
				value.refcount++;
			}
			return value.materialClone;
		}

		internal static MaterialWithRefcount GetExistingMaterialFor(Material materialSource, Texture2D texture)
		{
			if (materialSource == null || texture == null)
			{
				return null;
			}
			Dictionary<MaterialTexturePair, MaterialWithRefcount> dictionary = MaterialTable;
			MaterialTexturePair key = new MaterialTexturePair
			{
				material = materialSource,
				texture2D = texture
			};
			MaterialWithRefcount value;
			if (!dictionary.TryGetValue(key, out value))
			{
				return null;
			}
			return value;
		}

		internal static void RemoveMaterialFromTable(Material materialSource, Texture2D texture)
		{
			Dictionary<MaterialTexturePair, MaterialWithRefcount> dictionary = MaterialTable;
			MaterialTexturePair key = new MaterialTexturePair
			{
				material = materialSource,
				texture2D = texture
			};
			dictionary.Remove(key);
		}

		private void Start()
		{
			if (!Applied)
			{
				Apply();
			}
		}

		private void OnDestroy()
		{
			if (Applied)
			{
				Remove();
			}
		}

		public void Apply()
		{
			GetTexture();
			if (texture == null)
			{
				return;
			}
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			if (component == null)
			{
				return;
			}
			Dictionary<Slot, Material> customSlotMaterials = component.CustomSlotMaterials;
			int num = 0;
			foreach (Slot slot in component.Skeleton.Slots)
			{
				switch (slot.data.blendMode)
				{
				case BlendMode.Multiply:
					if (multiplyMaterialSource != null)
					{
						customSlotMaterials[slot] = GetOrAddMaterialFor(multiplyMaterialSource, texture);
						num++;
					}
					break;
				case BlendMode.Screen:
					if (screenMaterialSource != null)
					{
						customSlotMaterials[slot] = GetOrAddMaterialFor(screenMaterialSource, texture);
						num++;
					}
					break;
				}
			}
			slotsWithCustomMaterial = new SlotMaterialTextureTuple[num];
			int num2 = 0;
			foreach (Slot slot2 in component.Skeleton.Slots)
			{
				switch (slot2.data.blendMode)
				{
				case BlendMode.Multiply:
					if (multiplyMaterialSource != null)
					{
						slotsWithCustomMaterial[num2++] = new SlotMaterialTextureTuple(slot2, multiplyMaterialSource, texture);
					}
					break;
				case BlendMode.Screen:
					if (screenMaterialSource != null)
					{
						slotsWithCustomMaterial[num2++] = new SlotMaterialTextureTuple(slot2, screenMaterialSource, texture);
					}
					break;
				}
			}
			Applied = true;
			component.LateUpdate();
		}

		public void Remove()
		{
			GetTexture();
			if (texture == null)
			{
				return;
			}
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			if (component == null)
			{
				return;
			}
			Dictionary<Slot, Material> customSlotMaterials = component.CustomSlotMaterials;
			SlotMaterialTextureTuple[] array = slotsWithCustomMaterial;
			for (int i = 0; i < array.Length; i++)
			{
				SlotMaterialTextureTuple slotMaterialTextureTuple = array[i];
				Slot slot = slotMaterialTextureTuple.slot;
				Material material = slotMaterialTextureTuple.material;
				Texture2D texture2D = slotMaterialTextureTuple.texture2D;
				MaterialWithRefcount existingMaterialFor = GetExistingMaterialFor(material, texture2D);
				if (--existingMaterialFor.refcount == 0)
				{
					RemoveMaterialFromTable(material, texture2D);
				}
				Material value;
				if (customSlotMaterials.TryGetValue(slot, out value))
				{
					Material material2 = ((existingMaterialFor == null) ? null : existingMaterialFor.materialClone);
					if ((object)value == material2)
					{
						customSlotMaterials.Remove(slot);
					}
				}
			}
			slotsWithCustomMaterial = null;
			Applied = false;
			if (component.valid)
			{
				component.LateUpdate();
			}
		}

		public void GetTexture()
		{
			if (!(texture == null))
			{
				return;
			}
			SkeletonRenderer component = GetComponent<SkeletonRenderer>();
			if (component == null)
			{
				return;
			}
			SkeletonDataAsset skeletonDataAsset = component.skeletonDataAsset;
			if (skeletonDataAsset == null)
			{
				return;
			}
			AtlasAssetBase atlasAssetBase = skeletonDataAsset.atlasAssets[0];
			if (!(atlasAssetBase == null))
			{
				Material primaryMaterial = atlasAssetBase.PrimaryMaterial;
				if (!(primaryMaterial == null))
				{
					texture = primaryMaterial.mainTexture as Texture2D;
				}
			}
		}
	}
}
