using UnityEngine;

public class ShakeOneElement
{
	public Transform target;

	public float strength;

	public float duration;

	public ShakeOneElement(Transform target, float strength, float duration)
	{
		this.target = target;
		this.strength = strength;
		this.duration = duration;
	}
}
