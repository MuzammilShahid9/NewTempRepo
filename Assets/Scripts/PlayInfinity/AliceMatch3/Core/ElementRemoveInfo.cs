namespace PlayInfinity.AliceMatch3.Core
{
	public class ElementRemoveInfo
	{
		public Cell cell;

		public bool force;

		public bool showAnim = true;

		public bool grassFlag;

		public float delay = 0.1f;

		public BombInfo bombInfo;

		public ElementType ChangeToBomb = ElementType.None;

		public ElementType RemoveFrom;

		public bool isCollect;

		public ElementRemoveInfo(Cell cell, bool force = false, bool showAnim = true, bool grassFlag = false, float delay = 0.2f, BombInfo bombInfo = null, ElementType ChangeToBomb = ElementType.None, ElementType RemoveFrom = ElementType.None, bool isCollect = true)
		{
			this.cell = cell;
			this.force = force;
			this.showAnim = showAnim;
			this.grassFlag = grassFlag;
			this.delay = delay;
			this.bombInfo = bombInfo;
			this.ChangeToBomb = ChangeToBomb;
			this.RemoveFrom = RemoveFrom;
			this.isCollect = isCollect;
		}
	}
}
