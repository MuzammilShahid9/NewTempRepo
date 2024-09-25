using System;

namespace Spine
{
	public class IkConstraint : IConstraint, IUpdatable
	{
		internal IkConstraintData data;

		internal ExposedList<Bone> bones = new ExposedList<Bone>();

		internal Bone target;

		internal int bendDirection;

		internal bool compress;

		internal bool stretch;

		internal float mix = 1f;

		public int Order
		{
			get
			{
				return data.order;
			}
		}

		public ExposedList<Bone> Bones
		{
			get
			{
				return bones;
			}
		}

		public Bone Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public float Mix
		{
			get
			{
				return mix;
			}
			set
			{
				mix = value;
			}
		}

		public int BendDirection
		{
			get
			{
				return bendDirection;
			}
			set
			{
				bendDirection = value;
			}
		}

		public bool Compress
		{
			get
			{
				return compress;
			}
			set
			{
				compress = value;
			}
		}

		public bool Stretch
		{
			get
			{
				return stretch;
			}
			set
			{
				stretch = value;
			}
		}

		public IkConstraintData Data
		{
			get
			{
				return data;
			}
		}

		public IkConstraint(IkConstraintData data, Skeleton skeleton)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}
			this.data = data;
			mix = data.mix;
			bendDirection = data.bendDirection;
			compress = data.compress;
			stretch = data.stretch;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData bone in data.bones)
			{
				bones.Add(skeleton.FindBone(bone.name));
			}
			target = skeleton.FindBone(data.target.name);
		}

		public IkConstraint(IkConstraint constraint, Skeleton skeleton)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException("constraint cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			data = constraint.data;
			bones = new ExposedList<Bone>(constraint.Bones.Count);
			foreach (Bone bone in constraint.Bones)
			{
				bones.Add(skeleton.Bones.Items[bone.data.index]);
			}
			target = skeleton.Bones.Items[constraint.target.data.index];
			mix = constraint.mix;
			bendDirection = constraint.bendDirection;
			compress = constraint.compress;
			stretch = constraint.stretch;
		}

		public void Apply()
		{
			Update();
		}

		public void Update()
		{
			Bone bone = target;
			ExposedList<Bone> exposedList = bones;
			switch (exposedList.Count)
			{
			case 1:
				Apply(exposedList.Items[0], bone.worldX, bone.worldY, compress, stretch, data.uniform, mix);
				break;
			case 2:
				Apply(exposedList.Items[0], exposedList.Items[1], bone.worldX, bone.worldY, bendDirection, stretch, mix);
				break;
			}
		}

		public override string ToString()
		{
			return data.name;
		}

		public static void Apply(Bone bone, float targetX, float targetY, bool compress, bool stretch, bool uniform, float alpha)
		{
			if (!bone.appliedValid)
			{
				bone.UpdateAppliedTransform();
			}
			Bone parent = bone.parent;
			float num = 1f / (parent.a * parent.d - parent.b * parent.c);
			float num2 = targetX - parent.worldX;
			float num3 = targetY - parent.worldY;
			float num4 = (num2 * parent.d - num3 * parent.b) * num - bone.ax;
			float num5 = (num3 * parent.a - num2 * parent.c) * num - bone.ay;
			float num6 = (float)Math.Atan2(num5, num4) * (180f / (float)Math.PI) - bone.ashearX - bone.arotation;
			if (bone.ascaleX < 0f)
			{
				num6 += 180f;
			}
			if (num6 > 180f)
			{
				num6 -= 360f;
			}
			else if (num6 < -180f)
			{
				num6 += 360f;
			}
			float num7 = bone.ascaleX;
			float num8 = bone.ascaleY;
			if (compress || stretch)
			{
				float num9 = bone.data.length * num7;
				float num10 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
				if ((compress && num10 < num9) || (stretch && num10 > num9 && num9 > 0.0001f))
				{
					float num11 = (num10 / num9 - 1f) * alpha + 1f;
					num7 *= num11;
					if (uniform)
					{
						num8 *= num11;
					}
				}
			}
			bone.UpdateWorldTransform(bone.ax, bone.ay, bone.arotation + num6 * alpha, num7, num8, bone.ashearX, bone.ashearY);
		}

		public static void Apply(Bone parent, Bone child, float targetX, float targetY, int bendDir, bool stretch, float alpha)
		{
			if (alpha == 0f)
			{
				child.UpdateWorldTransform();
				return;
			}
			if (!parent.appliedValid)
			{
				parent.UpdateAppliedTransform();
			}
			if (!child.appliedValid)
			{
				child.UpdateAppliedTransform();
			}
			float ax = parent.ax;
			float ay = parent.ay;
			float num = parent.ascaleX;
			float num2 = num;
			float num3 = parent.ascaleY;
			float num4 = child.ascaleX;
			int num5;
			int num6;
			if (num < 0f)
			{
				num = 0f - num;
				num5 = 180;
				num6 = -1;
			}
			else
			{
				num5 = 0;
				num6 = 1;
			}
			if (num3 < 0f)
			{
				num3 = 0f - num3;
				num6 = -num6;
			}
			int num7;
			if (num4 < 0f)
			{
				num4 = 0f - num4;
				num7 = 180;
			}
			else
			{
				num7 = 0;
			}
			float ax2 = child.ax;
			float a = parent.a;
			float b = parent.b;
			float c = parent.c;
			float d = parent.d;
			bool num8 = Math.Abs(num - num3) <= 0.0001f;
			float num9;
			float num10;
			float num11;
			if (!num8)
			{
				num9 = 0f;
				num10 = a * ax2 + parent.worldX;
				num11 = c * ax2 + parent.worldY;
			}
			else
			{
				num9 = child.ay;
				num10 = a * ax2 + b * num9 + parent.worldX;
				num11 = c * ax2 + d * num9 + parent.worldY;
			}
			Bone parent2 = parent.parent;
			a = parent2.a;
			b = parent2.b;
			c = parent2.c;
			d = parent2.d;
			float num12 = 1f / (a * d - b * c);
			float num13 = targetX - parent2.worldX;
			float num14 = targetY - parent2.worldY;
			float num15 = (num13 * d - num14 * b) * num12 - ax;
			float num16 = (num14 * a - num13 * c) * num12 - ay;
			float num17 = num15 * num15 + num16 * num16;
			num13 = num10 - parent2.worldX;
			num14 = num11 - parent2.worldY;
			float num18 = (num13 * d - num14 * b) * num12 - ax;
			float num19 = (num14 * a - num13 * c) * num12 - ay;
			float num20 = (float)Math.Sqrt(num18 * num18 + num19 * num19);
			float num21 = child.data.length * num4;
			float num24;
			float num23;
			if (num8)
			{
				num21 *= num;
				float num22 = (num17 - num20 * num20 - num21 * num21) / (2f * num20 * num21);
				if (num22 < -1f)
				{
					num22 = -1f;
				}
				else if (num22 > 1f)
				{
					num22 = 1f;
					if (stretch && num20 + num21 > 0.0001f)
					{
						num2 *= ((float)Math.Sqrt(num17) / (num20 + num21) - 1f) * alpha + 1f;
					}
				}
				num23 = (float)Math.Acos(num22) * (float)bendDir;
				a = num20 + num21 * num22;
				b = num21 * (float)Math.Sin(num23);
				num24 = (float)Math.Atan2(num16 * a - num15 * b, num15 * a + num16 * b);
			}
			else
			{
				a = num * num21;
				b = num3 * num21;
				float num25 = a * a;
				float num26 = b * b;
				float num27 = (float)Math.Atan2(num16, num15);
				c = num26 * num20 * num20 + num25 * num17 - num25 * num26;
				float num28 = -2f * num26 * num20;
				float num29 = num26 - num25;
				d = num28 * num28 - 4f * num29 * c;
				if (d >= 0f)
				{
					float num30 = (float)Math.Sqrt(d);
					if (num28 < 0f)
					{
						num30 = 0f - num30;
					}
					num30 = (0f - (num28 + num30)) / 2f;
					float num31 = num30 / num29;
					float num32 = c / num30;
					float num33 = ((Math.Abs(num31) < Math.Abs(num32)) ? num31 : num32);
					if (num33 * num33 <= num17)
					{
						num14 = (float)Math.Sqrt(num17 - num33 * num33) * (float)bendDir;
						num24 = num27 - (float)Math.Atan2(num14, num33);
						num23 = (float)Math.Atan2(num14 / num3, (num33 - num20) / num);
						goto IL_04e6;
					}
				}
				float num34 = (float)Math.PI;
				float num35 = num20 - a;
				float num36 = num35 * num35;
				float num37 = 0f;
				float num38 = 0f;
				float num39 = num20 + a;
				float num40 = num39 * num39;
				float num41 = 0f;
				c = (0f - a) * num20 / (num25 - num26);
				if (c >= -1f && c <= 1f)
				{
					c = (float)Math.Acos(c);
					num13 = a * (float)Math.Cos(c) + num20;
					num14 = b * (float)Math.Sin(c);
					d = num13 * num13 + num14 * num14;
					if (d < num36)
					{
						num34 = c;
						num36 = d;
						num35 = num13;
						num37 = num14;
					}
					if (d > num40)
					{
						num38 = c;
						num40 = d;
						num39 = num13;
						num41 = num14;
					}
				}
				if (num17 <= (num36 + num40) / 2f)
				{
					num24 = num27 - (float)Math.Atan2(num37 * (float)bendDir, num35);
					num23 = num34 * (float)bendDir;
				}
				else
				{
					num24 = num27 - (float)Math.Atan2(num41 * (float)bendDir, num39);
					num23 = num38 * (float)bendDir;
				}
			}
			goto IL_04e6;
			IL_04e6:
			float num42 = (float)Math.Atan2(num9, ax2) * (float)num6;
			float arotation = parent.arotation;
			num24 = (num24 - num42) * (180f / (float)Math.PI) + (float)num5 - arotation;
			if (num24 > 180f)
			{
				num24 -= 360f;
			}
			else if (num24 < -180f)
			{
				num24 += 360f;
			}
			parent.UpdateWorldTransform(ax, ay, arotation + num24 * alpha, num2, parent.ascaleY, 0f, 0f);
			arotation = child.arotation;
			num23 = ((num23 + num42) * (180f / (float)Math.PI) - child.ashearX) * (float)num6 + (float)num7 - arotation;
			if (num23 > 180f)
			{
				num23 -= 360f;
			}
			else if (num23 < -180f)
			{
				num23 += 360f;
			}
			child.UpdateWorldTransform(ax2, num9, arotation + num23 * alpha, child.ascaleX, child.ascaleY, child.ashearX, child.ashearY);
		}
	}
}
