using UnityEngine;

public class BombExplode
{
	public Vector3 position;

	public Transform parent;

	public BombExplode(Vector3 position, Transform parent)
	{
		this.position = position;
		this.parent = parent;
	}
}
