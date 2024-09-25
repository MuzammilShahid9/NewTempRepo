using UnityEngine;

public class GrassCreate
{
	public Vector3 position;

	public Transform parent;

	public GrassCreate(Vector3 position, Transform parent)
	{
		this.position = position;
		this.parent = parent;
	}
}
