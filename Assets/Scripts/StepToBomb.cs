using UnityEngine;

public class StepToBomb
{
	public Vector3 one;

	public Vector3 two;

	public float time;

	public Transform parent;

	public StepToBomb(Vector3 one, Vector3 two, float time, Transform parent)
	{
		this.one = one;
		this.two = two;
		this.time = time;
		this.parent = parent;
	}
}
