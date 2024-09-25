namespace Spine
{
	public class MeshAttachment : VertexAttachment, IHasRendererObject
	{
		internal float regionOffsetX;

		internal float regionOffsetY;

		internal float regionWidth;

		internal float regionHeight;

		internal float regionOriginalWidth;

		internal float regionOriginalHeight;

		private MeshAttachment parentMesh;

		internal float[] uvs;

		internal float[] regionUVs;

		internal int[] triangles;

		internal float r = 1f;

		internal float g = 1f;

		internal float b = 1f;

		internal float a = 1f;

		internal int hulllength;

		internal bool inheritDeform;

		public int HullLength
		{
			get
			{
				return hulllength;
			}
			set
			{
				hulllength = value;
			}
		}

		public float[] RegionUVs
		{
			get
			{
				return regionUVs;
			}
			set
			{
				regionUVs = value;
			}
		}

		public float[] UVs
		{
			get
			{
				return uvs;
			}
			set
			{
				uvs = value;
			}
		}

		public int[] Triangles
		{
			get
			{
				return triangles;
			}
			set
			{
				triangles = value;
			}
		}

		public float R
		{
			get
			{
				return r;
			}
			set
			{
				r = value;
			}
		}

		public float G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public float B
		{
			get
			{
				return b;
			}
			set
			{
				b = value;
			}
		}

		public float A
		{
			get
			{
				return a;
			}
			set
			{
				a = value;
			}
		}

		public string Path { get; set; }

		public object RendererObject { get; set; }

		public float RegionU { get; set; }

		public float RegionV { get; set; }

		public float RegionU2 { get; set; }

		public float RegionV2 { get; set; }

		public bool RegionRotate { get; set; }

		public float RegionOffsetX
		{
			get
			{
				return regionOffsetX;
			}
			set
			{
				regionOffsetX = value;
			}
		}

		public float RegionOffsetY
		{
			get
			{
				return regionOffsetY;
			}
			set
			{
				regionOffsetY = value;
			}
		}

		public float RegionWidth
		{
			get
			{
				return regionWidth;
			}
			set
			{
				regionWidth = value;
			}
		}

		public float RegionHeight
		{
			get
			{
				return regionHeight;
			}
			set
			{
				regionHeight = value;
			}
		}

		public float RegionOriginalWidth
		{
			get
			{
				return regionOriginalWidth;
			}
			set
			{
				regionOriginalWidth = value;
			}
		}

		public float RegionOriginalHeight
		{
			get
			{
				return regionOriginalHeight;
			}
			set
			{
				regionOriginalHeight = value;
			}
		}

		public bool InheritDeform
		{
			get
			{
				return inheritDeform;
			}
			set
			{
				inheritDeform = value;
			}
		}

		public MeshAttachment ParentMesh
		{
			get
			{
				return parentMesh;
			}
			set
			{
				parentMesh = value;
				if (value != null)
				{
					bones = value.bones;
					vertices = value.vertices;
					worldVerticesLength = value.worldVerticesLength;
					regionUVs = value.regionUVs;
					triangles = value.triangles;
					HullLength = value.HullLength;
					Edges = value.Edges;
					Width = value.Width;
					Height = value.Height;
				}
			}
		}

		public int[] Edges { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		public MeshAttachment(string name)
			: base(name)
		{
		}

		public void UpdateUVs()
		{
			float[] array = regionUVs;
			if (uvs == null || uvs.Length != array.Length)
			{
				uvs = new float[array.Length];
			}
			float[] array2 = uvs;
			if (RegionRotate)
			{
				float num = regionWidth / (RegionV2 - RegionV);
				float num2 = regionHeight / (RegionU2 - RegionU);
				float num3 = RegionU - (RegionOriginalHeight - RegionOffsetY - RegionHeight) / num2;
				float num4 = RegionV - (RegionOriginalWidth - RegionOffsetX - RegionWidth) / num;
				float num5 = RegionOriginalHeight / num2;
				float num6 = RegionOriginalWidth / num;
				int i = 0;
				for (int num7 = array2.Length; i < num7; i += 2)
				{
					array2[i] = num3 + array[i + 1] * num5;
					array2[i + 1] = num4 + num6 - array[i] * num6;
				}
			}
			else
			{
				float num8 = regionWidth / (RegionU2 - RegionU);
				float num9 = regionHeight / (RegionV2 - RegionV);
				float num10 = RegionU - RegionOffsetX / num8;
				float num11 = RegionV - (RegionOriginalHeight - RegionOffsetY - RegionHeight) / num9;
				float num12 = RegionOriginalWidth / num8;
				float num13 = RegionOriginalHeight / num9;
				int j = 0;
				for (int num14 = array2.Length; j < num14; j += 2)
				{
					array2[j] = num10 + array[j] * num12;
					array2[j + 1] = num11 + array[j + 1] * num13;
				}
			}
		}

		public override bool ApplyDeform(VertexAttachment sourceAttachment)
		{
			if (this != sourceAttachment)
			{
				if (inheritDeform)
				{
					return parentMesh == sourceAttachment;
				}
				return false;
			}
			return true;
		}
	}
}
