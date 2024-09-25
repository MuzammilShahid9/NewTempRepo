using UnityEngine;

public class CameraHelper : MonoBehaviour
{
	private Camera camera;

	private void Awake()
	{
		camera = GetComponent<Camera>();
	}

	private void Start()
	{
		AdjustScreen();
	}

	public void AdjustScreen()
	{
		int height = Screen.height;
		int width = Screen.width;
		float orthographicSize2 = camera.orthographicSize;
		float orthographicSize = 3.8f * ((float)Screen.height * 1f / (float)Screen.width);
		camera.orthographicSize = orthographicSize;
	}
}
