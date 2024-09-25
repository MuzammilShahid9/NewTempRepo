using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public struct ButterFlyToTargetMessage
{
	public Transform element;

	public Vector3 endPos;

	public float time;

	public Board board;

	public int flag;

	public ButterFlyToTargetMessage(Transform element, Vector3 endPos, float time, Board board, int flag)
	{
		this.element = element;
		this.endPos = endPos;
		this.time = time;
		this.board = board;
		this.flag = flag;
	}
}
