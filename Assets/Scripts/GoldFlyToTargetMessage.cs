using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public struct GoldFlyToTargetMessage
{
	public Vector3 startPos;

	public Vector3 endPos;

	public float time;

	public Board board;

	public int goldNum;

	public Transform parent;

	public GoldFlyToTargetMessage(Vector3 startPos, Vector3 endPos, float time, int goldNum, Board board, Transform parent)
	{
		this.startPos = startPos;
		this.endPos = endPos;
		this.time = time;
		this.board = board;
		this.goldNum = goldNum;
		this.parent = parent;
	}
}
