using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class CollectSkillPower
{
	public Vector3 startPos;

	public Vector3 endPos;

	public float flyTime;

	public float speed;

	public ElementType type;

	public Transform parent;

	public CollectSkillPower(Vector3 startPos, Vector3 endPos, float flyTime)
	{
		this.startPos = startPos;
		this.endPos = endPos;
		this.flyTime = flyTime;
	}

	public CollectSkillPower(Vector3 startPos, Vector3 endPos, float flyTime, float speed, ElementType type, Transform parent)
	{
		this.startPos = startPos;
		this.endPos = endPos;
		this.flyTime = flyTime;
		this.speed = speed;
		this.type = type;
		this.parent = parent;
	}
}
