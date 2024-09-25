using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class CollectVaseElement
{
	public Transform vase;

	public Vector3 EndPos;

	public float delay;

	public float para;

	public Board board;

	public CollectVaseElement(Board board, Transform vase, Vector3 EndPos, float delay, float para)
	{
		this.EndPos = EndPos;
		this.delay = delay;
		this.vase = vase;
		this.para = para;
		this.board = board;
	}
}
