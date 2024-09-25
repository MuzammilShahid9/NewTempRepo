using UnityEngine;

public class SwitchElement
{
	public Vector3 one;

	public Vector3 two;

	public Transform parent;

	public SwitchElement(Vector3 one, Vector3 two, Transform parent)
	{
		this.one = one;
		this.two = two;
		this.parent = parent;
	}
}
