using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;

public struct RemoveElementListToBombMessage
{
	public List<ElementRemoveInfo> list;

	public float delayTime;

	public Board board;

	public int color;

	public int count;

	public RemoveElementListToBombMessage(List<ElementRemoveInfo> list, float delayTime, Board board, int color, int count)
	{
		this.list = list;
		this.board = board;
		this.delayTime = delayTime;
		this.color = color;
		this.count = count;
	}
}
