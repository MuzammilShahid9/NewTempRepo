using System;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class Camera3DManager : MonoBehaviour
{
	private static float PanSpeedX = 30f;

	private static float PanSpeedY = 16f;

	private static readonly float ZoomSpeedTouch = 0.01f;

	private static readonly float ZoomSpeedMouse = 0.5f;

	public static readonly float[] BoundsX = new float[2] { -100f, 5f };

	public static readonly float[] BoundsZ = new float[2] { -180f, -4f };

	public static readonly float[] ZoomBounds = new float[2] { 10f, 85f };

	public Camera cam;

	public Vector3 camStartPosition;

	private bool panActive;

	private bool delayMoveX;

	private bool delayMoveY;

	private Vector3 lastPanPosition;

	private int panFingerId;

	private bool zoomActive;

	private Vector2[] lastZoomPositions;

	private float maxPositionX;

	private float minPositionX;

	private float maxPositionY;

	private float minPositionY;

	private float maxPositionZ;

	private float minPositionZ;

	private Vector3 sceneStartPos;

	private Vector3 offset = new Vector3(-1.92f, -10.3f, -1.92f);

	private Vector3 offset2 = new Vector3(17.4f, 10.2f, -17.7f);

	private float dragTM;

	private Vector3 dragStartPosition;

	private Vector3 dragEndPosition;

	private Vector3 dragDis;

	private float dragSpeedX;

	private float dragSpeedY;

	private float dragSpeedZ;

	private float slowDown;

	private float perSlowDown = 0.01f;

	private bool xControlMove;

	private bool yControlMove;

	private bool zControlMove;

	private bool controlScale;

	private bool tutorialControlScale;

	private Vector3 targetPos;

	private float underControlSpeedX;

	private float underControlSpeedY;

	private float underControlSpeedZ;

	private float controlMoveTime;

	private float controlScaleIndex;

	private float controlScaleTime;

	private float controlScaleSpeed;

	private float lastCameraSize;

	private bool underBuildControl;

	private bool underTutorialControl;

	public bool isUnderPlot;

	public bool isUnderCinemaControl;

	private void Awake()
	{
		camStartPosition = base.transform.position;
		lastCameraSize = GeneralConfig.StartCameraSize;
		Input.multiTouchEnabled = true;
		cam = GetComponent<Camera>();
		cam.orthographicSize = GeneralConfig.StartCameraSize;
		DealCameraMoveLimit();
		cam.fieldOfView = 60f;
		AdjustSpeed();
	}

	private void AdjustSpeed()
	{
		PanSpeedX = cam.orthographicSize / 8f * 30f;
		PanSpeedY = cam.orthographicSize / 8f * 16f;
		perSlowDown = cam.orthographicSize / 8f * GeneralConfig.CameraSlowDownSpeed;
	}

	private void CalculateLimit()
	{
		sceneStartPos = GameObject.Find("3DScene").transform.position;
		maxPositionX = sceneStartPos.x + offset2.x + offset.x + GeneralConfig.MaxMoveDistanceX * Mathf.Sin((float)Math.PI / 4f);
		minPositionX = sceneStartPos.x + offset2.x + offset.x + GeneralConfig.MinMoveDistanceX * Mathf.Sin((float)Math.PI / 4f);
		maxPositionY = sceneStartPos.y + offset2.y + offset.y + GeneralConfig.MaxMoveDistanceY / Mathf.Sin((float)Math.PI / 3f);
		minPositionY = sceneStartPos.y + offset2.y + offset.y + GeneralConfig.MinMoveDistanceY / Mathf.Sin((float)Math.PI / 3f);
		maxPositionZ = sceneStartPos.z + offset2.z + offset.z + GeneralConfig.MaxMoveDistanceX * Mathf.Sin((float)Math.PI / 4f);
		minPositionZ = sceneStartPos.z + offset2.z + offset.z + GeneralConfig.MinMoveDistanceX * Mathf.Sin((float)Math.PI / 4f);
	}

	private void Update()
	{
		if (cam.orthographicSize != lastCameraSize)
		{
			CameraControl.Instance.SetAllCameraSize(cam.orthographicSize);
			lastCameraSize = cam.orthographicSize;
		}
		if (xControlMove || yControlMove || zControlMove)
		{
			UpdateControlMove();
		}
		if (controlScale)
		{
			UpdateControlScale();
			return;
		}
		if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
		{
			HandleTouch();
		}
		else
		{
			HandleMouse();
			HandleDelayMove();
		}
		HandleDelayMoveBack();
		if (!underTutorialControl)
		{
			HandleScaleBack();
		}
	}

	private void HandleTouch()
	{
		if (!PlotManager.Instance.isPlotFinish || EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		switch (Input.touchCount)
		{
		case 1:
			if (!CameraControl.Instance.JudgeClickOnUI())
			{
				zoomActive = false;
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began)
				{
					lastPanPosition = touch.position;
					panFingerId = touch.fingerId;
					panActive = true;
					delayMoveX = false;
					delayMoveY = false;
					slowDown = 0f;
					dragTM = 0f;
					dragStartPosition = touch.position;
				}
				else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
				{
					PanCamera(touch.position);
				}
				if (panActive)
				{
					dragTM += Time.deltaTime;
				}
			}
			break;
		case 2:
			if (!CameraControl.Instance.JudgeClickOnUI())
			{
				panActive = false;
				Vector2[] array = new Vector2[2]
				{
					Input.GetTouch(0).position,
					Input.GetTouch(1).position
				};
				if (!zoomActive)
				{
					lastZoomPositions = array;
					zoomActive = true;
					break;
				}
				float num = Vector2.Distance(array[0], array[1]);
				float num2 = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
				float num3 = num - num2;
				ZoomCamera(num3, ZoomSpeedTouch);
				lastZoomPositions = array;
			}
			break;
		default:
			if (panActive)
			{
				delayMoveX = true;
				delayMoveY = true;
				slowDown = 0f;
				dragDis = cam.ScreenToViewportPoint(dragStartPosition - lastPanPosition);
				dragSpeedX = dragDis.x / dragTM * 1f * cam.orthographicSize / 8f;
				dragSpeedY = dragDis.y / dragTM * 1f * cam.orthographicSize / 8f;
				DealDragSpeed();
			}
			HandleDelayMove();
			panActive = false;
			zoomActive = false;
			break;
		}
	}

	private void HandleMouse()
	{
		if (PlotManager.Instance.isPlotFinish)
		{
			if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				panActive = true;
				delayMoveX = false;
				delayMoveY = false;
				slowDown = 0f;
				dragTM = 0f;
				dragStartPosition = Input.mousePosition;
				lastPanPosition = Input.mousePosition;
			}
			else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				dragEndPosition = Input.mousePosition;
				delayMoveX = true;
				delayMoveY = true;
				panActive = false;
				dragDis = cam.ScreenToViewportPoint(dragStartPosition - dragEndPosition);
				dragSpeedX = dragDis.x / dragTM * 1f * cam.orthographicSize / 8f;
				dragSpeedY = dragDis.y / dragTM * 1f * cam.orthographicSize / 8f;
				DealDragSpeed();
			}
			else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				PanCamera(Input.mousePosition);
			}
			if (panActive)
			{
				dragTM += Time.deltaTime;
			}
			float axis = Input.GetAxis("Mouse ScrollWheel");
			zoomActive = true;
			ZoomCamera(axis, ZoomSpeedMouse);
			zoomActive = false;
		}
	}

	private void DealDragSpeed()
	{
		if (dragSpeedX > GeneralConfig.DragSpeed)
		{
			dragSpeedX = GeneralConfig.DragSpeed;
		}
		if (dragSpeedX < 0f - GeneralConfig.DragSpeed)
		{
			dragSpeedX = 0f - GeneralConfig.DragSpeed;
		}
		if (dragSpeedY > GeneralConfig.DragSpeed / PanSpeedX * PanSpeedY)
		{
			dragSpeedY = GeneralConfig.DragSpeed / PanSpeedX * PanSpeedY;
		}
		if (dragSpeedY < 0f - GeneralConfig.DragSpeed / PanSpeedX * PanSpeedY)
		{
			dragSpeedY = 0f - GeneralConfig.DragSpeed / PanSpeedX * PanSpeedY;
		}
	}

	private void PanCamera(Vector3 newPanPosition)
	{
		if (panActive)
		{
			Vector3 vector = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
			float x = base.transform.position.x + vector.x * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX;
			float y = base.transform.position.y + vector.y / Mathf.Sin((float)Math.PI / 3f) * PanSpeedY;
			float z = base.transform.position.z + vector.x * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX;
			if (!CameraMoveLimitManager.Instance.JudgeOutOfLimit())
			{
				base.transform.position = new Vector3(x, y, z);
			}
			lastPanPosition = newPanPosition;
		}
	}

	private void ZoomCamera(float offset, float speed)
	{
		if (!zoomActive || offset == 0f || EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (cam.orthographic)
		{
			cam.orthographicSize -= offset * speed;
			cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
			if (cam.orthographicSize > GeneralConfig.MaxCameraSize)
			{
				cam.orthographicSize = GeneralConfig.MaxCameraSize;
			}
			if (cam.orthographicSize < GeneralConfig.MinCameraSize)
			{
				cam.orthographicSize = GeneralConfig.MinCameraSize;
			}
			lastCameraSize = cam.orthographicSize;
			AdjustSpeed();
			DealCameraMoveLimit();
		}
		else
		{
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - offset * speed, ZoomBounds[0], ZoomBounds[1]);
		}
	}

	private void DealCameraMoveLimit()
	{
		GeneralConfig.MaxMoveDistanceX = GeneralConfig.FixedMaxMoveDistanceX + (8f - cam.orthographicSize) / 6f * 10.7f;
		GeneralConfig.MinMoveDistanceX = GeneralConfig.FixedMinMoveDistanceX - (8f - cam.orthographicSize) / 6f * 10.7f;
		GeneralConfig.MaxMoveDistanceY = GeneralConfig.FixedMaxMoveDistanceY + (8f - cam.orthographicSize) / 6f * 5.98f;
		GeneralConfig.MinMoveDistanceY = GeneralConfig.FixedMinMoveDistanceY - (8f - cam.orthographicSize) / 6f * 5.98f;
		CalculateLimit();
	}

	public void RestoreDelayMove()
	{
		delayMoveX = false;
		delayMoveY = false;
		dragSpeedX = 0f;
		dragSpeedY = 0f;
		dragStartPosition = Input.mousePosition;
		dragEndPosition = Input.mousePosition;
	}

	private void HandleDelayMove()
	{
		if ((!delayMoveX && !delayMoveY) || panActive || isUnderPlot || isUnderCinemaControl)
		{
			return;
		}
		float x = base.transform.position.x;
		float y = base.transform.position.y;
		float z = base.transform.position.z;
		if (dragSpeedX != 0f)
		{
			slowDown = perSlowDown / GeneralConfig.DragSpeed * Mathf.Abs(dragSpeedX);
		}
		if (delayMoveX)
		{
			if (dragTM > 0f)
			{
				if (dragDis.x > 0f && dragSpeedX > 0f)
				{
					dragSpeedX -= slowDown;
				}
				else if (dragDis.x >= 0f && dragSpeedX <= 0f)
				{
					dragSpeedX = 0f;
				}
				if (dragDis.x < 0f && dragSpeedX < 0f)
				{
					dragSpeedX += slowDown;
				}
				else if (dragDis.x <= 0f && dragSpeedX >= 0f)
				{
					dragSpeedX = 0f;
				}
				if (dragSpeedX != 0f)
				{
					x = base.transform.position.x + dragSpeedX * Mathf.Sin((float)Math.PI / 4f);
					z = base.transform.position.z + dragSpeedX * Mathf.Sin((float)Math.PI / 4f);
					if (CameraMoveLimitManager.Instance.JudgeOutOfLimit())
					{
						delayMoveX = false;
					}
				}
				else
				{
					delayMoveX = false;
				}
			}
			else
			{
				delayMoveX = false;
			}
		}
		if (delayMoveY)
		{
			if (dragTM > 0f)
			{
				float num = dragSpeedY / (dragSpeedX / perSlowDown);
				if (dragSpeedY != 0f)
				{
					num = perSlowDown / GeneralConfig.DragSpeed * Mathf.Abs(dragSpeedY);
				}
				if (dragDis.y > 0f && dragSpeedY > 0f)
				{
					dragSpeedY -= num;
				}
				else if (dragDis.y >= 0f && dragSpeedY <= 0f)
				{
					dragSpeedY = 0f;
				}
				if (dragDis.y < 0f && dragSpeedY < 0f)
				{
					dragSpeedY += num;
				}
				else if (dragDis.y <= 0f && dragSpeedY >= 0f)
				{
					dragSpeedY = 0f;
				}
				if (dragSpeedY != 0f)
				{
					y = base.transform.position.y + dragSpeedY / Mathf.Sin((float)Math.PI / 3f);
					if (CameraMoveLimitManager.Instance.JudgeOutOfLimit())
					{
						delayMoveY = false;
					}
				}
				else
				{
					delayMoveY = false;
				}
			}
			else
			{
				delayMoveY = false;
			}
		}
		if (!CameraMoveLimitManager.Instance.JudgeOutOfLimit())
		{
			base.transform.position = new Vector3(x, y, z);
		}
	}

	private void HandleDelayMoveBack()
	{
		if (CameraMoveLimitManager.Instance.GetRightLimitPosition() < 200f && !panActive && !delayMoveX)
		{
			base.transform.position = new Vector3(base.transform.position.x - 0.01f * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX, base.transform.position.y, base.transform.position.z - 0.01f * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX);
		}
		if (CameraMoveLimitManager.Instance.GetLeftLimitPosition() > -200f && !panActive && !delayMoveX)
		{
			base.transform.position = new Vector3(base.transform.position.x + 0.01f * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX, base.transform.position.y, base.transform.position.z + 0.01f * Mathf.Sin((float)Math.PI / 4f) * PanSpeedX);
		}
		if (CameraMoveLimitManager.Instance.GetUpLimitPosition() < 100f && !panActive && !delayMoveY)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.01f / Mathf.Sin((float)Math.PI / 3f) * PanSpeedY, base.transform.position.z);
		}
		if (CameraMoveLimitManager.Instance.GetDownLimitPosition() > -100f && !panActive && !delayMoveY)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.01f / Mathf.Sin((float)Math.PI / 3f) * PanSpeedY, base.transform.position.z);
		}
	}

	private void HandleScaleBack()
	{
		if ((bool)cam)
		{
			if ((double)cam.orthographicSize > (double)GeneralConfig.MaxCameraSize - 0.5 && !zoomActive)
			{
				cam.orthographicSize -= 0.1f;
			}
			if ((double)cam.orthographicSize < (double)GeneralConfig.MinCameraSize + 0.5 && !zoomActive)
			{
				cam.orthographicSize += 0.1f;
			}
		}
		lastCameraSize = cam.orthographicSize;
		DealCameraMoveLimit();
	}

	private void ClampToBounds()
	{
		Vector3 position = base.transform.position;
		position.x = Mathf.Clamp(base.transform.position.x, BoundsX[0], BoundsX[1]);
		position.z = Mathf.Clamp(base.transform.position.z, BoundsZ[0], BoundsZ[1]);
		base.transform.position = position;
	}

	public void MoveTo(Vector3 targetPosition, float moveTime, float scale, bool underControl = false)
	{
		underBuildControl = underControl;
		xControlMove = true;
		yControlMove = true;
		zControlMove = true;
		controlScale = false;
		controlMoveTime = moveTime;
		controlScaleIndex = scale;
		controlScaleTime = moveTime;
		if (moveTime == 0f)
		{
			base.transform.position = targetPosition;
			underControlSpeedX = 0f;
			underControlSpeedY = 0f;
			underControlSpeedZ = 0f;
		}
		else
		{
			targetPos = targetPosition;
			if (targetPos.x - base.transform.position.x != 0f)
			{
				underControlSpeedX = (targetPos.x - base.transform.position.x) / controlMoveTime;
				if ((double)Mathf.Abs(underControlSpeedX) < 0.01)
				{
					underControlSpeedX = 0f;
				}
			}
			else
			{
				underControlSpeedX = 0f;
			}
			if (targetPos.y - base.transform.position.y != 0f)
			{
				underControlSpeedY = (targetPos.y - base.transform.position.y) / controlMoveTime;
				if ((double)Mathf.Abs(underControlSpeedY) < 0.01)
				{
					underControlSpeedY = 0f;
				}
			}
			else
			{
				underControlSpeedY = 0f;
			}
			if (targetPos.z - base.transform.position.z != 0f)
			{
				underControlSpeedZ = (targetPos.z - base.transform.position.z) / controlMoveTime;
				if ((double)Mathf.Abs(underControlSpeedZ) < 0.01)
				{
					underControlSpeedZ = 0f;
				}
			}
			else
			{
				underControlSpeedZ = 0f;
			}
		}
		if (controlScaleTime == 0f)
		{
			controlScaleSpeed = controlScaleIndex - cam.orthographicSize;
			cam.orthographicSize = controlScaleIndex;
			DealCameraMoveLimit();
		}
		else
		{
			controlScaleSpeed = (controlScaleIndex - cam.orthographicSize) / controlScaleTime;
		}
		if ((double)Mathf.Abs(controlScaleSpeed) < 0.01)
		{
			controlScaleSpeed = 0f;
		}
	}

	public void ScaleTo(float scaleIndex, float scaleTime)
	{
		underTutorialControl = true;
		controlScaleIndex = scaleIndex;
		controlScaleTime = scaleTime;
		controlScale = true;
		if (controlScaleTime == 0f)
		{
			controlScaleSpeed = controlScaleIndex - cam.orthographicSize;
			cam.orthographicSize = controlScaleIndex;
			DealCameraMoveLimit();
		}
		else
		{
			controlScaleSpeed = (controlScaleIndex - cam.orthographicSize) / controlScaleTime;
		}
	}

	public void FinishTutorialScale()
	{
		underTutorialControl = false;
	}

	private void UpdateControlMove()
	{
		if (underControlSpeedX > 0f)
		{
			if (base.transform.position.x < targetPos.x)
			{
				float x = ((base.transform.position.x + underControlSpeedX * Time.deltaTime < targetPos.x) ? (base.transform.position.x + underControlSpeedX * Time.deltaTime) : targetPos.x);
				base.transform.position = new Vector3(x, base.transform.position.y, base.transform.position.z);
			}
			else
			{
				xControlMove = false;
			}
		}
		else if (underControlSpeedX < 0f)
		{
			if (base.transform.position.x > targetPos.x)
			{
				float x2 = ((base.transform.position.x + underControlSpeedX * Time.deltaTime > targetPos.x) ? (base.transform.position.x + underControlSpeedX * Time.deltaTime) : targetPos.x);
				base.transform.position = new Vector3(x2, base.transform.position.y, base.transform.position.z);
			}
			else
			{
				xControlMove = false;
			}
		}
		else if (underControlSpeedX == 0f)
		{
			xControlMove = false;
		}
		if (underControlSpeedY > 0f)
		{
			if (base.transform.position.y < targetPos.y)
			{
				float y = ((base.transform.position.y + underControlSpeedY * Time.deltaTime < targetPos.y) ? (base.transform.position.y + underControlSpeedY * Time.deltaTime) : targetPos.y);
				base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
			}
			else
			{
				yControlMove = false;
			}
		}
		else if (underControlSpeedY < 0f)
		{
			if (base.transform.position.y > targetPos.y)
			{
				float y2 = ((base.transform.position.y + underControlSpeedY * Time.deltaTime > targetPos.y) ? (base.transform.position.y + underControlSpeedY * Time.deltaTime) : targetPos.y);
				base.transform.position = new Vector3(base.transform.position.x, y2, base.transform.position.z);
			}
			else
			{
				yControlMove = false;
			}
		}
		else if (underControlSpeedY == 0f)
		{
			yControlMove = false;
		}
		if (underControlSpeedZ > 0f)
		{
			if (base.transform.position.z < targetPos.z)
			{
				float z = ((base.transform.position.z + underControlSpeedZ * Time.deltaTime < targetPos.z) ? (base.transform.position.z + underControlSpeedZ * Time.deltaTime) : targetPos.z);
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, z);
			}
			else
			{
				zControlMove = false;
			}
		}
		else if (underControlSpeedZ < 0f)
		{
			if (base.transform.position.z > targetPos.z)
			{
				float z2 = ((base.transform.position.z + underControlSpeedZ * Time.deltaTime > targetPos.z) ? (base.transform.position.z + underControlSpeedZ * Time.deltaTime) : targetPos.z);
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, z2);
			}
			else
			{
				zControlMove = false;
			}
		}
		else if (underControlSpeedZ == 0f)
		{
			zControlMove = false;
		}
		if (!xControlMove && !yControlMove && !zControlMove)
		{
			controlScale = true;
			if (underBuildControl)
			{
				BuildManager.Instance.MoveFinish();
				underBuildControl = false;
			}
		}
	}

	private void UpdateControlScale()
	{
		if (cam.orthographicSize > controlScaleIndex && (double)Mathf.Abs(cam.orthographicSize - controlScaleIndex) > 0.01)
		{
			if (controlScaleSpeed < 0f)
			{
				cam.orthographicSize += controlScaleSpeed * Time.deltaTime;
				return;
			}
			PlotCameraManager.Instance.FinishStep();
			controlScale = false;
		}
		else if (cam.orthographicSize < controlScaleIndex && (double)Mathf.Abs(cam.orthographicSize - controlScaleIndex) > 0.01)
		{
			if (controlScaleSpeed > 0f)
			{
				cam.orthographicSize += controlScaleSpeed * Time.deltaTime;
				return;
			}
			PlotCameraManager.Instance.FinishStep();
			controlScale = false;
		}
		else
		{
			PlotCameraManager.Instance.FinishStep();
			controlScale = false;
		}
	}

	public void UnderCinemaControl()
	{
		isUnderCinemaControl = true;
	}

	public void ReleaseCinemaControl()
	{
		isUnderCinemaControl = false;
		delayMoveX = false;
		delayMoveY = false;
	}

	public void StopMove()
	{
		underControlSpeedX = 0f;
		underControlSpeedY = 0f;
		controlScaleSpeed = 0f;
		controlScale = false;
		xControlMove = false;
		yControlMove = false;
		zControlMove = false;
	}

	private void OnDisable()
	{
		if (UserDataManager.Instance.GetService().cameraSavePositioin == null)
		{
			UserDataManager.Instance.GetService().cameraSavePositioin = new PositionInfo();
		}
		UserDataManager.Instance.GetService().cameraSavePositioin.x = base.transform.position.x;
		UserDataManager.Instance.GetService().cameraSavePositioin.y = base.transform.position.y;
		UserDataManager.Instance.GetService().cameraSavePositioin.z = base.transform.position.z;
		UserDataManager.Instance.Save();
	}
}
