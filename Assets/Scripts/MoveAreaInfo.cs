using PlayInfinity.AliceMatch3.Core;

public class MoveAreaInfo
{
	public Board board;

	public int oldAreaIndex;

	public int newAreaIndex;

	public MoveAreaInfo(Board board, int oldAreaIndex, int newAreaIndex)
	{
		this.board = board;
		this.oldAreaIndex = oldAreaIndex;
		this.newAreaIndex = newAreaIndex;
	}

	public MoveAreaInfo(Board board, int newAreaIndex)
	{
		this.board = board;
		this.newAreaIndex = newAreaIndex;
		oldAreaIndex = 9999;
	}
}
