using UnityEngine;
using UnityEngine.EventSystems;

public class Camera2DManager : MonoBehaviour
{
	private static float PanSpeedX = 30f;

	private static float PanSpeedY = 16f;

	private static readonly float ZoomSpeedTouch = 0.01f;

	private static readonly float ZoomSpeedMouse = 0.5f;

	public static readonly float[] BoundsX = new float[2] { -100f, 5f };

	public static readonly float[] BoundsZ = new float[2] { -180f, -4f };

	public static readonly float[] ZoomBounds = new float[2] { 10f, 85f };

	public Camera cam;

	private bool panActive;

	private bool delayMoveX;

	private bool delayMoveY;

	private Vector3 lastPanPosition;

	private int panFingerId;

	private bool zoomActive;

	private Vector2[] lastZoomPositions;

	private Vector3 mapStartPos;

	private float dragTM;

	private Vector3 dragStartPosition;

	private Vector3 dragEndPosition;

	private Vector3 dragDis;

	private float dragSpeedX;

	private float dragSpeedY;

	private float slowDown;

	private float perSlowDown = 0.01f;

	private bool xControlMove;

	private bool yControlMove;

	private bool controlScale;

	private Vector3 targetPos;

	private float underControlSpeedX;

	private float underControlSpeedY;

	private float controlMoveTime;

	private float controlScaleIndex;

	private float controlScaleTime;

	private float controlScaleSpeed;

	private float lastCameraSize;

	private bool underBuildControl;

	public bool isUnderPlot;

	private void Awake()
	{
		lastCameraSize = GeneralConfig.StartCameraSize;
		cam = GetComponent<Camera>();
		cam.orthographicSize = GeneralConfig.StartCameraSize;
		DealCameraMoveLimit();
		mapStartPos = GameObject.Find("Land").transform.position;
		cam.fieldOfView = 60f;
		AdjustSpeed();
	}

	private void AdjustSpeed()
	{
		PanSpeedX = cam.orthographicSize / 8f * 30f;
		PanSpeedY = cam.orthographicSize / 8f * 16f;
		perSlowDown = cam.orthographicSize / 8f * GeneralConfig.CameraSlowDownSpeed;
	}

	private void Update()
	{
		if (GlobalVariables.UnderChangeItemControl)
		{
			return;
		}
		if (cam.orthographicSize != lastCameraSize)
		{
			CameraControl.Instance.SetAllCameraSize(cam.orthographicSize);
			lastCameraSize = cam.orthographicSize;
		}
		if (xControlMove || yControlMove)
		{
			UpdateControlMove();
			return;
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
		HandleScaleBack();
	}

	private bool JudgeClickOnUI()
	{
		if (GameObject.Find("Dialogs") != null && CameraControl.Instance.IsPointerOverUIObject(GameObject.Find("Dialogs").GetComponent<Canvas>(), Input.GetTouch(0).position))
		{
			return true;
		}
		if (GameObject.Find("CastleSceneMainUI") != null && CameraControl.Instance.IsPointerOverUIObject(GameObject.Find("CastleSceneMainUI").GetComponent<Canvas>(), Input.GetTouch(0).position))
		{
			return true;
		}
		if (GameObject.Find("TutorialManager(Clone)") != null && GameObject.Find("TutorialManager(Clone)").transform.childCount > 0)
		{
			Transform child = GameObject.Find("TutorialManager(Clone)").transform.GetChild(0);
			if (CameraControl.Instance.IsPointerOverUIObject(child.GetComponent<Canvas>(), Input.GetTouch(0).position))
			{
				return true;
			}
		}
		return false;
	}

	private void HandleTouch()
	{
		if (PlotManager.Instance.currStep > 0 || EventSystem.current.IsPointerOverGameObject() || GlobalVariables.UnderChangeItemControl)
		{
			return;
		}
		switch (Input.touchCount)
		{
		case 1:
			if (!JudgeClickOnUI())
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
			if (!JudgeClickOnUI())
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
				float offset = num - num2;
				ZoomCamera(offset, ZoomSpeedTouch);
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
		if (PlotManager.Instance.currStep <= 0 && !GlobalVariables.UnderChangeItemControl)
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
			float num = base.transform.position.x + vector.x * PanSpeedX;
			float num2 = base.transform.position.y + vector.y * PanSpeedY;
			float z = base.transform.position.z;
			if (num >= GeneralConfig.MaxMoveDistanceX + mapStartPos.x)
			{
				num = GeneralConfig.MaxMoveDistanceX + mapStartPos.x;
			}
			if (num <= GeneralConfig.MinMoveDistanceX + mapStartPos.x)
			{
				num = GeneralConfig.MinMoveDistanceX + mapStartPos.x;
			}
			if (num2 >= GeneralConfig.MaxMoveDistanceY + mapStartPos.y)
			{
				num2 = GeneralConfig.MaxMoveDistanceY + mapStartPos.y;
			}
			if (num2 <= GeneralConfig.MinMoveDistanceY + mapStartPos.y)
			{
				num2 = GeneralConfig.MinMoveDistanceY + mapStartPos.y;
			}
			base.transform.position = new Vector3(num, num2, z);
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
	}

	private void HandleDelayMove()
	{
		if ((!delayMoveX && !delayMoveY) || panActive || isUnderPlot)
		{
			return;
		}
		float num = base.transform.position.x;
		float num2 = base.transform.position.y;
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
					num = base.transform.position.x + dragSpeedX;
					if (num >= GeneralConfig.MaxMoveDistanceX + mapStartPos.x)
					{
						num = GeneralConfig.MaxMoveDistanceX + mapStartPos.x;
						delayMoveX = false;
					}
					if (num <= GeneralConfig.MinMoveDistanceX + mapStartPos.x)
					{
						num = GeneralConfig.MinMoveDistanceX + mapStartPos.x;
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
				float num3 = dragSpeedY / (dragSpeedX / perSlowDown);
				if (dragSpeedY != 0f)
				{
					num3 = perSlowDown / GeneralConfig.DragSpeed * Mathf.Abs(dragSpeedY);
				}
				if (dragDis.y > 0f && dragSpeedY > 0f)
				{
					dragSpeedY -= num3;
				}
				else if (dragDis.y >= 0f && dragSpeedY <= 0f)
				{
					dragSpeedY = 0f;
				}
				if (dragDis.y < 0f && dragSpeedY < 0f)
				{
					dragSpeedY += num3;
				}
				else if (dragDis.y <= 0f && dragSpeedY >= 0f)
				{
					dragSpeedY = 0f;
				}
				if (dragSpeedY != 0f)
				{
					num2 = base.transform.position.y + dragSpeedY;
					if (num2 >= GeneralConfig.MaxMoveDistanceY + mapStartPos.y)
					{
						num2 = GeneralConfig.MaxMoveDistanceY + mapStartPos.y;
						delayMoveY = false;
					}
					if (num2 <= GeneralConfig.MinMoveDistanceY + mapStartPos.y)
					{
						num2 = GeneralConfig.MinMoveDistanceY + mapStartPos.y;
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
		base.transform.position = new Vector3(num, num2, base.transform.position.z);
	}

	private void HandleDelayMoveBack()
	{
		if (base.transform.position.x >= GeneralConfig.MaxMoveDistanceX + mapStartPos.x - 2f && !panActive && !delayMoveX)
		{
			base.transform.position = new Vector3(base.transform.position.x - 0.01f * PanSpeedX, base.transform.position.y, base.transform.position.z);
		}
		if (base.transform.position.x <= GeneralConfig.MinMoveDistanceX + mapStartPos.x + 2f && !panActive && !delayMoveX)
		{
			base.transform.position = new Vector3(base.transform.position.x + 0.01f * PanSpeedX, base.transform.position.y, base.transform.position.z);
		}
		if (base.transform.position.y >= GeneralConfig.MaxMoveDistanceY + mapStartPos.y - 2f && !panActive && !delayMoveY)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - 0.01f * PanSpeedY, base.transform.position.z);
		}
		if (base.transform.position.y <= GeneralConfig.MinMoveDistanceY + mapStartPos.y + 2f && !panActive && !delayMoveY)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.01f * PanSpeedY, base.transform.position.z);
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

	public void MoveTo(Vector3 targetPosition, float moveTime, float scale, float scaleTime, bool underControl = false)
	{
		underBuildControl = underControl;
		xControlMove = true;
		yControlMove = true;
		controlMoveTime = moveTime;
		controlScaleIndex = scale;
		controlScaleTime = scaleTime;
		if (moveTime == 0f)
		{
			base.transform.position = targetPosition;
			underControlSpeedX = 0f;
			underControlSpeedY = 0f;
		}
		else
		{
			targetPos = targetPosition;
			if (targetPos.x - base.transform.position.x != 0f)
			{
				underControlSpeedX = (targetPos.x - base.transform.position.x) / controlMoveTime;
			}
			else
			{
				underControlSpeedX = 0f;
			}
			if (targetPos.y - base.transform.position.y != 0f)
			{
				underControlSpeedY = (targetPos.y - base.transform.position.y) / controlMoveTime;
			}
			else
			{
				underControlSpeedY = 0f;
			}
		}
		if (scaleTime > 0f)
		{
			controlScaleSpeed = (controlScaleIndex - cam.orthographicSize) / scaleTime;
		}
		else
		{
			controlScaleSpeed = 0f;
		}
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
		if (!xControlMove && !yControlMove)
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
		if (cam.orthographicSize > controlScaleIndex)
		{
			if (controlScaleSpeed < 0f)
			{
				cam.orthographicSize += controlScaleSpeed * Time.deltaTime;
			}
			else
			{
				controlScale = false;
			}
		}
		else if (cam.orthographicSize < controlScaleIndex)
		{
			if (controlScaleSpeed > 0f)
			{
				cam.orthographicSize += controlScaleSpeed * Time.deltaTime;
			}
			else
			{
				controlScale = false;
			}
		}
		else
		{
			controlScale = false;
		}
	}

	public void StopMove()
	{
		underControlSpeedX = 0f;
		underControlSpeedY = 0f;
		controlScaleSpeed = 0f;
		controlScale = false;
		xControlMove = false;
		yControlMove = false;
	}
}
