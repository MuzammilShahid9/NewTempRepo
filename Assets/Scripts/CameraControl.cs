using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
	private static CameraControl instance;

	public Camera3DManager camera3D;

	public GameObject landPivot;

	public Camera defaultCamera;

	public Vector3 cameraStartPosition = new Vector3(78.5f, 66.1f, -102f);

	private Vector3 camera2DTargetPosition;

	private Vector3 camera3DTargetPosition;

	private Vector3 camera2DStartPosition;

	private Vector3 camera3DStartPosition;

	private Vector3 positionOffset;

	private Vector3 roleStartPos;

	private Vector3 landScreenPivotPos;

	private bool lockToAlice;

	private bool lockToRole;

	private Role targetRole;

	private bool isMove;

	public static CameraControl Instance
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

	private void Start()
	{
		landScreenPivotPos = landPivot.transform.position;
		landPivot.transform.parent.localRotation = Quaternion.Euler(new Vector3(30f, -45f, 0f));
		landPivot.transform.parent.position = new Vector3(33.95f, 13.2f, -37.8f);
		if (UserDataManager.Instance.GetService().tutorialProgress <= 2)
		{
			camera3D.transform.position = cameraStartPosition;
			return;
		}
		PositionInfo cameraSavePositioin = UserDataManager.Instance.GetService().cameraSavePositioin;
		camera3D.transform.position = new Vector3(cameraSavePositioin.x, cameraSavePositioin.y, cameraSavePositioin.z);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			LockToAlice("A");
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			UnlockToAlice();
			UnlockToRole();
			PlotManager.Instance.isPlotFinish = true;
		}
		if (lockToRole)
		{
			positionOffset = targetRole.transform.position - roleStartPos;
			float num = (positionOffset.z + positionOffset.x) * Mathf.Cos((float)Math.PI / 4f);
			float num2 = (positionOffset.z - positionOffset.x) * Mathf.Cos((float)Math.PI / 4f) * Mathf.Cos((float)Math.PI / 3f);
			float num3 = num * Mathf.Sin((float)Math.PI / 4f);
			float num4 = num2 / Mathf.Sin((float)Math.PI / 3f);
			Vector3 position = camera3D.transform.position;
			camera3D.transform.position = new Vector3(position.x + num3, position.y + num4, position.z + num3);
			roleStartPos = targetRole.transform.position;
		}
	}

	public void UnlockToRole()
	{
		lockToRole = false;
	}

	public void SetAllCameraSize(float size)
	{
		camera3D.GetComponent<Camera>().orthographicSize = size;
	}

	public void TutorialScaleCamera(float scaleIndex, float scaleTime)
	{
		camera3D.ScaleTo(scaleIndex, scaleTime);
	}

	public void FinishTutorialScale()
	{
		camera3D.FinishTutorialScale();
	}

	public void RestortCameraDealyMove()
	{
		camera3D.RestoreDelayMove();
	}

	public void MoveTo2DPosition(Vector3 targetPos, float moveTime, float scale, float sacleTime)
	{
		camera3D.MoveTo(targetPos, moveTime, scale);
	}

	public void GoTo2DPosition(Vector3 targetPos, float scale, float tm)
	{
		camera3D.MoveTo(targetPos, tm, scale);
	}

	public void BuildControlMoveToPosition(Vector3 targetPos, float moveTime)
	{
		Vector3 vector = targetPos - landScreenPivotPos;
		camera3DTargetPosition = camera3D.camStartPosition + vector;
		camera2DTargetPosition = new Vector3(targetPos.x, targetPos.y, targetPos.z);
		Vector3 camera2DTargetPosition2 = camera2DTargetPosition;
		Vector3 camera2DStartPosition2 = camera2DStartPosition;
		Vector3 camera2DTargetPosition3 = camera2DTargetPosition;
		Vector3 camera2DStartPosition3 = camera2DStartPosition;
		camera3D.MoveTo(camera3DTargetPosition, moveTime, camera3D.cam.orthographicSize, true);
	}

	public void LockCamera()
	{
		camera3D.isUnderPlot = true;
	}

	public void UnlockCamra()
	{
		camera3D.isUnderPlot = false;
	}

	public void StopMove()
	{
		camera3D.StopMove();
	}

	public void LockToAlice(string actor)
	{
	}

	public void LockToRole(RoleType followRole)
	{
		switch (followRole)
		{
		case RoleType.Alice:
			targetRole = RoleManager.Instance.roleDictionary[RoleType.Alice];
			break;
		case RoleType.John:
			targetRole = RoleManager.Instance.roleDictionary[RoleType.John];
			break;
		case RoleType.Arthur:
			targetRole = RoleManager.Instance.roleDictionary[RoleType.Arthur];
			break;
		case RoleType.Cat:
			targetRole = RoleManager.Instance.roleDictionary[RoleType.Cat];
			break;
		case RoleType.Tina:
			targetRole = RoleManager.Instance.roleDictionary[RoleType.Tina];
			break;
		}
		roleStartPos = targetRole.transform.position;
		lockToRole = true;
	}

	public void UnlockToAlice()
	{
		lockToAlice = false;
	}

	public bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = screenPosition;
		GraphicRaycaster component = canvas.gameObject.GetComponent<GraphicRaycaster>();
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(pointerEventData, list);
		return list.Count > 0;
	}

	public bool JudgeClickOnUI()
	{
		if (Input.touchCount < 1)
		{
			return false;
		}
		if (DialogManagerTemp.Instance != null && IsPointerOverUIObject(DialogManagerTemp.Instance.GetComponent<Canvas>(), Input.GetTouch(0).position))
		{
			return true;
		}
		if (CastleSceneUIManager.Instance != null && IsPointerOverUIObject(CastleSceneUIManager.Instance.GetComponent<Canvas>(), Input.GetTouch(0).position))
		{
			return true;
		}
		if (TutorialManager.Instance != null && TutorialManager.Instance.transform.childCount > 0)
		{
			Transform child = TutorialManager.Instance.transform.GetChild(0);
			if (IsPointerOverUIObject(child.GetComponent<Canvas>(), Input.GetTouch(0).position))
			{
				return true;
			}
		}
		return false;
	}
}
