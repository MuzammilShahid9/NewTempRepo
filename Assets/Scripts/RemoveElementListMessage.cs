using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;

public struct RemoveElementListMessage
{
	public List<ElementRemoveInfo> list;

	public float delayTime;

	public Board board;

	public Action action;

	public float delayStartTime;

	public ElementType removeFromType;

	public RemoveElementListMessage(List<ElementRemoveInfo> list, float delayTime, Board board, float delayStartTime = 0f, Action action = null, ElementType removeFromType = ElementType.None)
	{
		this.list = list;
		this.delayTime = delayTime;
		this.board = board;
		this.action = action;
		this.delayStartTime = delayStartTime;
		this.removeFromType = removeFromType;
	}
}
