using System;

namespace Spine
{
	public class AttachmentTimeline : Timeline, ISlotTimeline
	{
		internal int slotIndex;

		internal float[] frames;

		internal string[] attachmentNames;

		public int PropertyId
		{
			get
			{
				return 67108864 + slotIndex;
			}
		}

		public int FrameCount
		{
			get
			{
				return frames.Length;
			}
		}

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("index must be >= 0.");
				}
				slotIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public string[] AttachmentNames
		{
			get
			{
				return attachmentNames;
			}
			set
			{
				attachmentNames = value;
			}
		}

		public AttachmentTimeline(int frameCount)
		{
			frames = new float[frameCount];
			attachmentNames = new string[frameCount];
		}

		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			frames[frameIndex] = time;
			attachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixBlend blend, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (direction == MixDirection.Out && blend == MixBlend.Setup)
			{
				string attachmentName = slot.data.attachmentName;
				slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
				return;
			}
			float[] array = frames;
			if (time < array[0])
			{
				if (blend == MixBlend.Setup || blend == MixBlend.First)
				{
					string attachmentName = slot.data.attachmentName;
					slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
				}
			}
			else
			{
				int num = ((!(time >= array[array.Length - 1])) ? (Animation.BinarySearch(array, time) - 1) : (array.Length - 1));
				string attachmentName = attachmentNames[num];
				slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
			}
		}
	}
}
