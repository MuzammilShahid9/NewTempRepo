using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class BombInfo
{
	public ElementType type;

	public Vector2 posLeftUP;

	public Vector2 posRightDown;

	public int bombColOrRow;

	public BombInfo(ElementType type, Vector2 posLeftUP, Vector2 posRightDown)
	{
		this.type = type;
		this.posLeftUP = posLeftUP;
		this.posRightDown = posRightDown;
	}

	public BombInfo(ElementType type, int bombColOrRow)
	{
		this.type = type;
		this.bombColOrRow = bombColOrRow;
	}
}
