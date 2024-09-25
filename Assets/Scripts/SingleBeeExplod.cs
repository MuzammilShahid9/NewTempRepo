using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class SingleBeeExplod
{
	public Vector3 EffectPos;

	public Transform parent;

	public Vector3 localPostion;

	public Vector2 rAc;

	public Vector3 endPos;

	public float checkTime;

	public float flyTime;

	public Board board;

	public ElementType type;

	public float acc;

	public float initSpeed;

	public SingleBeeExplod(Vector3 EffectPos, Transform parent, Vector3 localPostion, Vector2 rAc, Vector3 endPos, float checkTime, float flyTime, float acc, float initSpeed, Board board, ElementType type)
	{
		this.EffectPos = EffectPos;
		this.parent = parent;
		this.localPostion = localPostion;
		this.rAc = rAc;
		this.endPos = endPos;
		this.checkTime = checkTime;
		this.flyTime = flyTime;
		this.board = board;
		this.type = type;
		this.acc = acc;
		this.initSpeed = initSpeed;
	}
}
