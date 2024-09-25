using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class BeeHitOne
{
	public Vector3 startPos;

	public ElementType type;

	public Transform parent;

	public BeeHitOne(Vector3 startPos, ElementType type, Transform parent)
	{
		this.startPos = startPos;
		this.type = type;
		this.parent = parent;
	}
}
