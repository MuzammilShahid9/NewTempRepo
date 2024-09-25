using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class ColorBombExplode
{
	public Vector3 selfPos;

	public List<Vector3> bombList;

	public List<Element> bombElementList;

	public ElementType type;

	public Board board;

	public ColorBombExplode(Vector3 selfPos, Board board, List<Vector3> bombList, List<Element> bombElementList = null, ElementType type = ElementType.None)
	{
		this.bombList = bombList;
		this.type = type;
		this.bombElementList = bombElementList;
		this.board = board;
		this.selfPos = selfPos;
	}
}
