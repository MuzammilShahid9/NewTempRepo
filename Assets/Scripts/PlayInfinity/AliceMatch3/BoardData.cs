using System;

namespace PlayInfinity.AliceMatch3.Editor
{
	[Serializable]
	public class BoardData
	{
		public int width;

		public int height;

		public Area[] areaList;

		public Path[] pathList;

		public Path[] transporterPathList;

		public Path[] catPathList;

		public Trans[] transList;

		public Creater[] createrList;

		public int[] map;
	}
}
