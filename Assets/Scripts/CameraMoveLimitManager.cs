using UnityEngine;

public class CameraMoveLimitManager : MonoBehaviour
{
	private static CameraMoveLimitManager instance;

	public GameObject leftLimit;

	public GameObject rightLimit;

	public GameObject upLimit;

	public GameObject downLimit;

	[HideInInspector]
	public float leftLimitNumber;

	[HideInInspector]
	public float rightLimitNumber;

	[HideInInspector]
	public float upLimitNumber;

	[HideInInspector]
	public float downLimitNumber;

	public static CameraMoveLimitManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public bool JudgeOutOfLimit()
	{
		leftLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(leftLimit.transform.position).x;
		rightLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(rightLimit.transform.position).x;
		upLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(upLimit.transform.position).y;
		downLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(downLimit.transform.position).y;
		if (leftLimitNumber > 0f)
		{
			return true;
		}
		if (rightLimitNumber < 0f)
		{
			return true;
		}
		if (upLimitNumber < 0f)
		{
			return true;
		}
		if (downLimitNumber > 0f)
		{
			return true;
		}
		return false;
	}

	public float GetLeftLimitPosition()
	{
		leftLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(leftLimit.transform.position).x;
		return leftLimitNumber;
	}

	public float GetRightLimitPosition()
	{
		rightLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(rightLimit.transform.position).x;
		return rightLimitNumber;
	}

	public float GetUpLimitPosition()
	{
		upLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(upLimit.transform.position).y;
		return upLimitNumber;
	}

	public float GetDownLimitPosition()
	{
		downLimitNumber = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(downLimit.transform.position).y;
		return downLimitNumber;
	}
}
