using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class CollectStandardElement
{
	public Vector3 InitPos;

	public int type;

	public Vector3 EndPos;

	public float delay;

	public Board board;

	public Element element;

	public int layer;

	public CollectStandardElement(Board board, Element element, int type, Vector3 EndPos, float delay, int layer)
	{
		this.type = type;
		InitPos = element.transform.position;
		this.EndPos = EndPos;
		this.delay = delay;
		this.board = board;
		this.element = element;
		this.layer = layer;
	}
}
