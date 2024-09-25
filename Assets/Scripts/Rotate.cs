using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float speed = 3f;

	private void Update()
	{
		base.transform.RotateAround(Vector3.forward, speed * Time.deltaTime);
	}
}
