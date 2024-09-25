using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class CreateOneMoveToBomb
{
	public Board board;

	public int color;

	public ElementRemoveInfo item;

	public Vector3 endPos;

	public float time;

	public CreateOneMoveToBomb(Board board, int color, ElementRemoveInfo item, Vector3 endPos, float time)
	{
		this.board = board;
		this.color = color;
		this.item = item;
		this.endPos = endPos;
		this.time = time;
	}
}
