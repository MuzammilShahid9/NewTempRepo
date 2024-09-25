using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[ExecuteAlways]
	public class SkeletonRendererCustomMaterials : MonoBehaviour
	{
		[Serializable]
		public struct SlotMaterialOverride : IEquatable<SlotMaterialOverride>
		{
			public bool overrideDisabled;

			[SpineSlot("", "", false, true, false)]
			public string slotName;

			public Material material;

			public bool Equals(SlotMaterialOverride other)
			{
				if (overrideDisabled == other.overrideDisabled && slotName == other.slotName)
				{
					return material == other.material;
				}
				return false;
			}
		}

		[Serializable]
		public struct AtlasMaterialOverride : IEquatable<AtlasMaterialOverride>
		{
			public bool overrideDisabled;

			public Material originalMaterial;

			public Material replacementMaterial;

			public bool Equals(AtlasMaterialOverride other)
			{
				if (overrideDisabled == other.overrideDisabled && originalMaterial == other.originalMaterial)
				{
					return replacementMaterial == other.replacementMaterial;
				}
				return false;
			}
		}

		public SkeletonRenderer skeletonRenderer;

		[SerializeField]
		protected List<SlotMaterialOverride> customSlotMaterials = new List<SlotMaterialOverride>();

		[SerializeField]
		protected List<AtlasMaterialOverride> customMaterialOverrides = new List<AtlasMaterialOverride>();

		private void SetCustomSlotMaterials()
		{
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!slotMaterialOverride.overrideDisabled && !string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					skeletonRenderer.CustomSlotMaterials[key] = slotMaterialOverride.material;
				}
			}
		}

		private void RemoveCustomSlotMaterials()
		{
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					Material value;
					if (skeletonRenderer.CustomSlotMaterials.TryGetValue(key, out value) && !(value != slotMaterialOverride.material))
					{
						skeletonRenderer.CustomSlotMaterials.Remove(key);
					}
				}
			}
		}

		private void SetCustomMaterialOverrides()
		{
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (!atlasMaterialOverride.overrideDisabled)
				{
					skeletonRenderer.CustomMaterialOverride[atlasMaterialOverride.originalMaterial] = atlasMaterialOverride.replacementMaterial;
				}
			}
		}

		private void RemoveCustomMaterialOverrides()
		{
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				Material value;
				if (skeletonRenderer.CustomMaterialOverride.TryGetValue(atlasMaterialOverride.originalMaterial, out value) && !(value != atlasMaterialOverride.replacementMaterial))
				{
					skeletonRenderer.CustomMaterialOverride.Remove(atlasMaterialOverride.originalMaterial);
				}
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			skeletonRenderer.Initialize(false);
			SetCustomMaterialOverrides();
			SetCustomSlotMaterials();
		}

		private void OnDisable()
		{
			if (skeletonRenderer == null)
			{
				Debug.LogError("skeletonRenderer == null");
				return;
			}
			RemoveCustomMaterialOverrides();
			RemoveCustomSlotMaterials();
		}
	}
}
