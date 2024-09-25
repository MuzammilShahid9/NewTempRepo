using UnityEngine;

public class CloudCreate
{
	public Vector3 position;

	public Transform parent;

	public CloudCreate(Vector3 position, Transform parent)
	{
		this.position = position;
		this.parent = parent;
	}
}
