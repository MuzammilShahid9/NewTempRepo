using UnityEngine;

public class PliotBubbleManager : MonoBehaviour
{
	private Role followTarget;

	private float roleHeight = 1.45f;

	private void Start()
	{
		if (RoleManager.Instance.roleDictionary.ContainsKey(PlotDialogManager.Instance.currDialogData.roleType))
		{
			followTarget = RoleManager.Instance.roleDictionary[PlotDialogManager.Instance.currDialogData.roleType];
			roleHeight = followTarget.roleHeight;
		}
		base.transform.GetComponent<Canvas>().sortingLayerName = "UI";
		SetPosition();
	}

	private void Update()
	{
		SetPosition();
	}

	public void SetPosition()
	{
		if (followTarget != null)
		{
			Vector3 position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y + roleHeight, followTarget.transform.position.z);
			Vector3 startPosition = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(position);
			float x = base.transform.GetComponent<RectTransform>().sizeDelta.x;
			float y = base.transform.GetComponent<RectTransform>().sizeDelta.y;
			Vector2 sizeDelta = base.transform.parent.GetComponent<RectTransform>().sizeDelta;
			Vector2 vector = CalculatePosition(startPosition, sizeDelta);
			startPosition = new Vector3(vector.x, vector.y, startPosition.z);
			new Vector2(vector.x, vector.y);
			if (startPosition.x + x > sizeDelta.x / 2f)
			{
				startPosition.x = sizeDelta.x / 2f - x;
			}
			else if (startPosition.x < 0f - sizeDelta.x / 2f)
			{
				startPosition.x = 0f - sizeDelta.x / 2f;
			}
			if (startPosition.y + y > sizeDelta.y / 2f)
			{
				startPosition.y = sizeDelta.y / 2f - y;
			}
			else if (startPosition.y < 0f - sizeDelta.y / 2f)
			{
				startPosition.y = 0f - sizeDelta.y / 2f;
			}
			if (CastleSceneUIManager.Instance.taskBtn.IsActive())
			{
				Vector3 localPosition = CastleSceneUIManager.Instance.taskBtn.transform.GetComponent<RectTransform>().localPosition;
				if (startPosition.x < localPosition.x && startPosition.y < localPosition.y)
				{
					if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
					{
						startPosition.y = localPosition.y;
					}
					else
					{
						startPosition.x = localPosition.x;
					}
				}
			}
			if (CastleSceneUIManager.Instance.glodBtn.IsActive())
			{
				Vector3 localPosition2 = CastleSceneUIManager.Instance.glodBtn.transform.GetComponent<RectTransform>().localPosition;
				if (startPosition.x < localPosition2.x && startPosition.y + y > localPosition2.y)
				{
					if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
					{
						startPosition.x = localPosition2.x;
					}
					else
					{
						startPosition.y = localPosition2.y - y;
					}
				}
			}
			if (CastleSceneUIManager.Instance.scrollBtn.IsActive())
			{
				Vector3 localPosition3 = CastleSceneUIManager.Instance.scrollBtn.transform.GetComponent<RectTransform>().localPosition;
				if (startPosition.x + x > localPosition3.x && startPosition.y + y > localPosition3.y)
				{
					if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
					{
						startPosition.y = localPosition3.y - y;
					}
					else
					{
						startPosition.x = localPosition3.x - x;
					}
				}
			}
			if (CastleSceneUIManager.Instance.enterGameBtn.IsActive())
			{
				Vector3 localPosition4 = CastleSceneUIManager.Instance.enterGameBtn.transform.GetComponent<RectTransform>().localPosition;
				if (startPosition.x + x > localPosition4.x && startPosition.y < localPosition4.y)
				{
					if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
					{
						startPosition.y = localPosition4.y;
					}
					else
					{
						startPosition.x = localPosition4.x - x;
					}
				}
			}
			base.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
		}
		else if (RoleManager.Instance.roleDictionary.ContainsKey(PlotDialogManager.Instance.currDialogData.roleType))
		{
			followTarget = RoleManager.Instance.roleDictionary[PlotDialogManager.Instance.currDialogData.roleType];
			roleHeight = followTarget.roleHeight;
		}
	}

	public Vector2 CalculatePosition(Vector3 startPosition, Vector2 ScreenSize)
	{
		float x = startPosition.x;
		float y = startPosition.y;
		float num = 0f;
		float x2 = x / (float)Screen.width * ScreenSize.x - ScreenSize.x / 2f;
		num = y / (float)Screen.height * ScreenSize.y - ScreenSize.y / 2f;
		return new Vector2(x2, num);
	}
}
