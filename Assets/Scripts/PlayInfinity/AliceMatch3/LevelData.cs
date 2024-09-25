using System;

namespace PlayInfinity.AliceMatch3.Editor
{
	[Serializable]
	public class LevelData
	{
		public int level;

		public int move;

		public int hard;

		public int[] probabilityList;

		public int[] skillProbabilityList;

		public int[] targetList;

		public int[] targetListByCollect;

		public BoardData[] boardData;
	}
}
