using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Role : MonoBehaviour
{
	public enum ArriveStatus
	{
		none,
		arrived,
		arriving,
		overtime
	}

	public enum RoleStatus
	{
		none,
		Idle,
		PlotControl,
		MouseControl
	}

	public RoleType roleType;

	private Animator selfAni;

	public ArriveStatus status;

	public SpriteRenderer shadowSprite;

	public float roleHeight = 1.15f;

	public float roleWalkSpeed = 1.1f;

	public float roleRunSpeed = 2.1f;

	public float roleMoveStopDistance = 0.13f;

	public GameObject magicEffect;

	public GameObject shortMagicEffect;

	public GameObject footGameObject;

	public GameObject liningGameObject;

	public AnimationClip plotIdleAnimation;

	public AnimationClip idleActionAnimation;

	private bool underMoveControl;

	private bool underAnimControl;

	private AnimatorStateInfo animatorInfo;

	private string controlAnimName;

	private Vector3 targetPosition;

	private RoleStatus roleStatus;

	private bool canMove;

	private bool roleRotate;

	private float angleSpeed = 0.1f;

	private float idleActionWaitTime = 5f;

	private float moveRotateSpeed;

	private Quaternion lookAtPosition;

	private bool isIdleActionFinish;

	private bool isStepFinished;

	private bool isDelay;

	private bool moveRotate;

	private BubbleManager bubble;

	private SortingGroup selfSorting;

	private int lastActionID = -1;

	private int plotStep;

	private int idleStep;

	private RoleType underIdleControlRoleType;

	private string[] animList;

	private int _animIndex;

	private List<bool> stepFinishCondition = new List<bool>();

	private NavMeshAgent mr;

	private void Awake()
	{
	}

	private void Start()
	{
		if (roleType == RoleType.Alice && !RoleManager.Instance.roleDictionary.ContainsKey(RoleType.Alice))
		{
			RoleManager.Instance.AddRole(roleType, this);
			PositionInfo positionInfo = UserDataManager.Instance.GetService().roleSavePosition[0];
			base.transform.transform.position = new Vector3(positionInfo.x, positionInfo.y, positionInfo.z);
		}
		selfSorting = base.transform.GetComponent<SortingGroup>();
		if (status == ArriveStatus.none)
		{
			status = ArriveStatus.arrived;
		}
		roleHeight = 1.15f;
		mr = GetComponent<NavMeshAgent>();
		selfAni = GetComponent<Animator>();
		roleStatus = RoleStatus.Idle;
		isIdleActionFinish = true;
		moveRotate = false;
		DealPosition();
		canMove = true;
	}

	public void RunOld(string animData)
	{
		animList = animData.Split(';');
		RunNextAnim();
	}

	public void Run(CDRoleAnim config, int currStep)
	{
		roleStatus = RoleStatus.PlotControl;
		plotStep = currStep;
		if (config.anim != null)
		{
			_animIndex = 0;
			animList = config.anim.ToArray();
			RunNextAnim();
		}
	}

	public void IdleRun(IDRoleAnim config, int currStep, RoleType underIdleControlType)
	{
		roleStatus = RoleStatus.Idle;
		idleStep = currStep;
		underIdleControlRoleType = underIdleControlType;
		if (config.anim != null)
		{
			_animIndex = 0;
			animList = config.anim.ToArray();
			RunNextAnim();
		}
	}

	private void RunNextAnim()
	{
		if (animList == null)
		{
			return;
		}
		if (_animIndex >= animList.Length)
		{
			if (roleStatus == RoleStatus.Idle)
			{
				IdleRoleAniManager.Instance.FinishOneCondition(underIdleControlRoleType, idleStep);
			}
			else if (roleStatus == RoleStatus.PlotControl)
			{
				PlotRoleAniManager.Instance.FinishOneCondition(plotStep);
			}
			return;
		}
		string text = animList[_animIndex];
		DebugUtils.Log(DebugType.Other, "Run Anim " + text);
		int num = text.IndexOf("(");
		_animIndex++;
		if (num > 0)
		{
			if (text[0] == 'W')
			{
				MoveTo(GetTargetPosition(text), GetLookAtPosition(text), WalkType.Walk);
			}
			else if (text[0] == 'F')
			{
				FlashTo(GetTargetPosition(text), GetLookAtPosition(text));
				RunNextAnim();
			}
			else if (text[0] == 'R')
			{
				MoveTo(GetTargetPosition(text), GetLookAtPosition(text), WalkType.Run);
			}
		}
		else
		{
			PlayAnimation(text);
		}
	}

	public Vector3 GetTargetPosition(string moveString)
	{
		string[] array = moveString.Split('(')[1].Split(',');
		Vector3 result = default(Vector3);
		result.x = Convert.ToSingle(array[0]);
		result.y = Convert.ToSingle(array[1]);
		result.z = Convert.ToSingle(array[2].Split(')')[0]);
		return result;
	}

	public Quaternion GetLookAtPosition(string moveString)
	{
		string[] array = moveString.Split('(')[2].Split(',');
		Quaternion result = default(Quaternion);
		result.x = Convert.ToSingle(array[0]);
		result.y = Convert.ToSingle(array[1]);
		result.z = Convert.ToSingle(array[2]);
		result.w = Convert.ToSingle(array[3].Split(')')[0]);
		return result;
	}

	private void DealPosition()
	{
		if (UserDataManager.Instance.GetService().roleSavePosition != null && UserDataManager.Instance.GetService().roleSavePosition.Count > 4)
		{
			PositionInfo positionInfo = UserDataManager.Instance.GetService().roleSavePosition[(int)roleType];
			if (positionInfo.x != 0f || positionInfo.y != 0f || positionInfo.z != 0f)
			{
				mr.enabled = false;
				base.transform.position = new Vector3(positionInfo.x, positionInfo.y, positionInfo.z);
				mr.enabled = true;
			}
		}
		if (UserDataManager.Instance.GetService().roleSaveRotation != null && UserDataManager.Instance.GetService().roleSaveRotation.Count > 4)
		{
			RotationInfo rotationInfo = UserDataManager.Instance.GetService().roleSaveRotation[(int)roleType];
			if (rotationInfo.x != 0f || rotationInfo.y != 0f || rotationInfo.z != 0f || rotationInfo.w != 0f)
			{
				base.transform.rotation = new Quaternion(rotationInfo.x, rotationInfo.y, rotationInfo.z, rotationInfo.w);
			}
		}
	}

	private void Update()
	{
		if (status == ArriveStatus.arriving)
		{
			DealArrive();
			MoveRotate();
		}
		if (underAnimControl || !isIdleActionFinish)
		{
			animatorInfo = selfAni.GetCurrentAnimatorStateInfo(0);
			if (((animatorInfo.normalizedTime >= 1f && JudgeSameAnim(animatorInfo)) || JudgeSpecialAnim()) && !(controlAnimName == "Walk") && !(controlAnimName == "Run") && !(controlAnimName == "Idle"))
			{
				selfAni.SetBool(controlAnimName, false);
				DealSpecialAnim();
				if (controlAnimName != "SitDown" && controlAnimName != "GotoBed")
				{
					controlAnimName = "None";
					selfAni.SetTrigger("Idle");
				}
				if (underAnimControl)
				{
					underAnimControl = false;
					RunNextAnim();
				}
				else if (!isIdleActionFinish)
				{
					DebugUtils.Log(DebugType.Other, "Anim Finish" + controlAnimName);
					controlAnimName = "None";
					FinishOneCondition();
				}
			}
		}
		animatorInfo = selfAni.GetCurrentAnimatorStateInfo(0);
		if (animatorInfo.IsName("Walk") || animatorInfo.IsName("Run"))
		{
			canMove = true;
		}
		if (roleRotate)
		{
			RoleRotate();
		}
		if (roleStatus == RoleStatus.Idle && isIdleActionFinish && !TestConfig.cinemaMode && !TestConfig.idleDirectorMode)
		{
			StartCoroutine(StartIdleAction());
		}
		if (roleStatus == RoleStatus.MouseControl)
		{
			UpdateRolePosition();
		}
		if (TestConfig.active && TestConfig.roleMoveTest)
		{
			JudgeClick();
		}
		CinemaClick();
		DealLayer();
	}

	private void DealSpecialAnim()
	{
		if (controlAnimName == "Magic")
		{
			StartCoroutine(DelayHideMagicEffect());
		}
		else if (controlAnimName == "ShortMagic")
		{
			StartCoroutine(DelayHideShortMagicEffect());
		}
		else if (controlAnimName == "GetUp" && roleType == RoleType.Alice)
		{
			footGameObject.SetActive(false);
			liningGameObject.SetActive(false);
			shadowSprite.gameObject.SetActive(true);
		}
		else if (controlAnimName == "LieDown" || controlAnimName == "GotoBed")
		{
			controlAnimName = "None";
		}
		else if (controlAnimName == "StandUp" && roleType == RoleType.Alice)
		{
			shadowSprite.gameObject.SetActive(true);
		}
	}

	private void JudgeClick()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int num = LayerMask.NameToLayer("Plane");
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5000f, 1 << num) && roleType == RoleType.Alice)
			{
				MoveTo(hitInfo.point, Quaternion.identity, WalkType.Walk);
			}
		}
	}

	private void MoveRotate()
	{
		Vector3 steeringTarget = mr.steeringTarget;
		Vector3 vector = mr.steeringTarget - base.transform.position;
		Vector3 to = new Vector3(vector.x, 0f, vector.z);
		float num = Vector3.Angle(base.transform.forward, to);
		if (!moveRotate)
		{
			moveRotateSpeed = num / 180f * 720f;
			if (num > 90f)
			{
				moveRotateSpeed = num / 180f * 1440f;
			}
			moveRotate = true;
		}
		if (base.transform.forward.x * to.z - base.transform.forward.z * to.x > 0f)
		{
			moveRotateSpeed *= -1f;
		}
		else
		{
			float num2 = base.transform.forward.x * to.z - base.transform.forward.z * to.x;
			float num3 = 0f;
		}
		if (num > 5f)
		{
			base.transform.Rotate(new Vector3(0f, moveRotateSpeed * Time.deltaTime, 0f), Space.Self);
			moveRotate = false;
		}
	}

	private void CinemaClick()
	{
		if (Input.GetMouseButtonDown(0) && PlotManager.Instance.isStepFinished && !EventSystem.current.IsPointerOverGameObject() && (CastleSceneManager.Instance.cinemaMode || CastleSceneManager.Instance.IdleEditorMode))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int num = LayerMask.NameToLayer("Role");
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5000f, 1 << num) && hitInfo.transform == base.transform)
			{
				ClickOnRole();
			}
		}
	}

	public void UpdateRolePosition()
	{
		Vector3 position = new Vector3(0f, 0f, 0f);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		int num = LayerMask.NameToLayer("Plane");
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 5000f, 1 << num))
		{
			position = hitInfo.point + new Vector3(0f, 0.1f, 0f);
		}
		base.transform.position = position;
	}

	private void ClickOnRole()
	{
		if (roleStatus != RoleStatus.MouseControl)
		{
			StopRoleIdleAction();
			selfAni.SetBool(controlAnimName, false);
			selfAni.SetTrigger("Idle");
			roleStatus = RoleStatus.MouseControl;
			mr.enabled = false;
			return;
		}
		roleStatus = RoleStatus.Idle;
		Vector3 position = new Vector3(0f, 0f, 0f);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		int num = LayerMask.NameToLayer("Plane");
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 5000f, 1 << num))
		{
			position = hitInfo.point;
		}
		selfAni.SetBool("Idle", false);
		base.transform.position = position;
		mr.enabled = true;
	}

	public void ShowRoleStartIdle()
	{
		isIdleActionFinish = true;
	}

	public bool JudgeSameAnim(AnimatorStateInfo animatorInfo)
	{
		if (animatorInfo.IsName(controlAnimName))
		{
			return true;
		}
		if (controlAnimName == "SitDown")
		{
			if (animatorInfo.IsName("SitDown") || animatorInfo.IsName("Sit"))
			{
				return true;
			}
			return false;
		}
		if (controlAnimName == "GotoBed")
		{
			if (animatorInfo.IsName("GotoBed") || animatorInfo.IsName("Sleep"))
			{
				return true;
			}
			return false;
		}
		if (controlAnimName == "GetUp")
		{
			if (animatorInfo.IsName("GetUp") || animatorInfo.IsName("Idle"))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool JudgeSpecialAnim()
	{
		if (controlAnimName == "GetUp" && animatorInfo.IsName("Idle"))
		{
			return true;
		}
		return false;
	}

	public void DealLayer()
	{
		float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(base.transform.position).y;
		if (selfSorting != null)
		{
			selfSorting.sortingOrder = 10000 - (int)(y * 10f);
		}
		if (shadowSprite != null)
		{
			shadowSprite.sortingOrder = 10000 - (int)(y * 10f) - 1;
		}
	}

	private bool JudgeNoChangeAnim()
	{
		if (animatorInfo.IsName("LieDown"))
		{
			return true;
		}
		return false;
	}

	public void UnderPlotControl()
	{
		ToggleIdleAnimationToPlot();
		roleStatus = RoleStatus.PlotControl;
		StopAllCoroutines();
		ClearBubble();
		isIdleActionFinish = true;
		isDelay = false;
		if (selfAni == null)
		{
			selfAni = GetComponent<Animator>();
		}
		if (mr == null)
		{
			mr = GetComponent<NavMeshAgent>();
		}
		if (!JudgeNoChangeAnim())
		{
			selfAni.SetBool("Plot", true);
		}
		StartCoroutine(DelaySetPlotFalse());
		if (selfAni.GetBool("Idle"))
		{
			selfAni.SetBool("Idle", false);
		}
		else
		{
			selfAni.SetTrigger("Idle");
		}
		if (controlAnimName != null && controlAnimName != "")
		{
			selfAni.SetBool(controlAnimName, false);
		}
		mr.SetDestination(base.transform.position);
	}

	public void ToggleIdleAnimationToPlot()
	{
		if (plotIdleAnimation != null && selfAni != null)
		{
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(selfAni.runtimeAnimatorController);
			selfAni.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["Idle"] = plotIdleAnimation;
		}
	}

	public void ToggleIdleAnimationToIdle()
	{
		if (idleActionAnimation != null)
		{
			if (animatorInfo.IsName("Idle"))
			{
				StartCoroutine(DelayToggleIdleAnim(1f - (float)((double)animatorInfo.normalizedTime - Math.Floor(animatorInfo.normalizedTime))));
				return;
			}
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(selfAni.runtimeAnimatorController);
			selfAni.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["Idle"] = idleActionAnimation;
		}
	}

	private IEnumerator DelayToggleIdleAnim(float animFinishTime)
	{
		yield return new WaitForSeconds(animFinishTime * animatorInfo.length);
		if (selfAni != null)
		{
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(selfAni.runtimeAnimatorController);
			selfAni.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["Idle"] = idleActionAnimation;
		}
	}

	private IEnumerator DelaySetPlotFalse()
	{
		yield return null;
		selfAni.SetBool("Plot", false);
	}

	public void ReleasePlotControl()
	{
		ToggleIdleAnimationToIdle();
		UpdateMagicEffect();
		roleStatus = RoleStatus.Idle;
	}

	public void UpdateMagicEffect()
	{
		if (magicEffect != null)
		{
			magicEffect.SetActive(false);
		}
		if (shortMagicEffect != null)
		{
			shortMagicEffect.SetActive(false);
		}
	}

	public void RoleRotate()
	{
		if (!roleRotate)
		{
			return;
		}
		Vector3 eulerAngles = lookAtPosition.eulerAngles;
		Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
		float num = 0f;
		if (Math.Abs(eulerAngles.y - eulerAngles2.y) % 360f <= 180f)
		{
			num = (eulerAngles.y - eulerAngles2.y) % 360f / 0.1f;
		}
		else if (Math.Abs(eulerAngles.y - eulerAngles2.y) % 360f > 180f)
		{
			int num2 = ((!(eulerAngles.y - eulerAngles2.y >= 0f)) ? 1 : (-1));
			num = ((eulerAngles.y - eulerAngles2.y) % 360f + (float)(360 * num2)) / 0.1f;
		}
		base.transform.Rotate(new Vector3(0f, num * Time.deltaTime, 0f), Space.Self);
		if (Math.Abs(eulerAngles2.y - eulerAngles.y) % 360f < 10f)
		{
			DebugUtils.Log(DebugType.Other, "LookAttarget");
			roleRotate = false;
			if (underMoveControl)
			{
				underMoveControl = false;
				RunNextAnim();
			}
			else if (!isIdleActionFinish)
			{
				FinishOneCondition();
			}
		}
	}

	public void StopRoleIdleAction()
	{
		StopAllCoroutines();
		if (!animatorInfo.IsName("Sleep") && !animatorInfo.IsName("GotoBed"))
		{
			selfAni.SetBool("Plot", false);
		}
	}

	public void StopIdle()
	{
		StopAllCoroutines();
	}

	public void MoveTo(Vector3 TargetPosition, Quaternion LookAtPosition, WalkType walkType)
	{
		StopRoleIdleAction();
		underMoveControl = true;
		if (selfAni == null)
		{
			selfAni = GetComponent<Animator>();
		}
		if (mr == null)
		{
			mr = GetComponent<NavMeshAgent>();
		}
		if (controlAnimName != "Idle" && controlAnimName != null && controlAnimName != "")
		{
			selfAni.SetBool(controlAnimName, false);
		}
		selfAni.SetTrigger("Idle");
		lookAtPosition = LookAtPosition;
		targetPosition = TargetPosition;
		status = ArriveStatus.arriving;
		StartCoroutine(RoleMove(TargetPosition, walkType));
	}

	public void IdleMoveTo(string positionString)
	{
		Vector3 vector = (targetPosition = GetPositionString(positionString));
		lookAtPosition = GetLookTargetString(positionString);
		status = ArriveStatus.arriving;
		selfAni.SetBool("Plot", false);
		StartCoroutine(RoleMove(vector, WalkType.Walk));
	}

	private Vector3 GetPositionString(string positionString)
	{
		string text = positionString.Split(')')[0];
		float x = Convert.ToSingle(text.Split(',')[0].Substring(1));
		float y = Convert.ToSingle(text.Split(',')[1]);
		float z = Convert.ToSingle(text.Split(',')[2]);
		return new Vector3(x, y, z);
	}

	private Quaternion GetLookTargetString(string positionString)
	{
		string text = positionString.Split('(')[2];
		text = text.Split(')')[0];
		float x = Convert.ToSingle(text.Split(',')[0]);
		float y = Convert.ToSingle(text.Split(',')[1]);
		float z = Convert.ToSingle(text.Split(',')[2]);
		float w = Convert.ToSingle(text.Split(',')[3]);
		return new Quaternion(x, y, z, w);
	}

	public void FlashTo(Vector3 TargetPosition, Quaternion LookAtPosition)
	{
		mr.enabled = false;
		StopRoleIdleAction();
		base.transform.position = TargetPosition;
		targetPosition = TargetPosition;
		mr.enabled = true;
		mr.SetDestination(TargetPosition);
		status = ArriveStatus.arrived;
		base.transform.rotation = LookAtPosition;
	}

	public void PlayAnimation(string AnimName)
	{
		StopRoleIdleAction();
		DebugUtils.Log(DebugType.Other, "PlotPlayAnimName" + AnimName);
		selfAni.SetBool("Plot", false);
		if (controlAnimName != null && controlAnimName != "")
		{
			selfAni.SetBool(controlAnimName, false);
		}
		selfAni.SetTrigger("Idle");
		canMove = false;
		underAnimControl = true;
		StartCoroutine(DelayStartPlayAnim(AnimName));
	}

	private IEnumerator DelayStartPlayAnim(string AnimName)
	{
		yield return new WaitForSeconds(0.2f);
		selfAni.SetBool("Idle", false);
		controlAnimName = AnimName;
		selfAni.SetBool(AnimName, true);
		DealSpecialAnimStart(AnimName);
	}

	private void DealSpecialAnimStart(string AnimName)
	{
		switch (AnimName)
		{
		case "Magic":
			if (magicEffect != null)
			{
				magicEffect.SetActive(true);
			}
			break;
		case "ShortMagic":
			if (shortMagicEffect != null)
			{
				shortMagicEffect.SetActive(true);
			}
			break;
		case "LieDown":
			SortingLayerManager.Instance.ChangePurpleBedSortingLayer();
			footGameObject.SetActive(true);
			liningGameObject.SetActive(true);
			shadowSprite.gameObject.SetActive(false);
			break;
		case "SitDown":
			shadowSprite.gameObject.SetActive(false);
			break;
		case "GetUp":
			SortingLayerManager.Instance.RecoverPurpleBedSortingLayer();
			break;
		}
	}

	private IEnumerator DelayHideMagicEffect()
	{
		yield return new WaitForSeconds(0.5f);
		if (magicEffect != null)
		{
			magicEffect.SetActive(false);
		}
	}

	private IEnumerator DelayHideShortMagicEffect()
	{
		yield return new WaitForSeconds(1f);
		if (shortMagicEffect != null)
		{
			shortMagicEffect.SetActive(false);
		}
	}

	public void IdlePlayAnimation(string AnimName)
	{
		DebugUtils.Log(DebugType.Other, "IdlePlayAnim" + AnimName);
		canMove = false;
		selfAni.SetBool("Plot", false);
		if (controlAnimName != null && controlAnimName != "")
		{
			selfAni.SetBool(controlAnimName, false);
		}
		if (selfAni.GetBool("Idle"))
		{
			StartCoroutine(ChangeIdleFalse(AnimName));
			return;
		}
		selfAni.SetTrigger("Idle");
		StartCoroutine(ChangeIdleFalse(AnimName));
	}

	private IEnumerator ChangeIdleFalse(string AnimName)
	{
		yield return new WaitForSeconds(0.2f);
		selfAni.SetBool("Idle", false);
		controlAnimName = AnimName;
		selfAni.SetBool(AnimName, true);
	}

	private void DealArrive()
	{
		if (JudgeArrive())
		{
			DebugUtils.Log(DebugType.Other, "Arrived");
			selfAni.SetBool("Walk", false);
			mr.SetDestination(base.transform.position);
			if (roleType == RoleType.Cat || roleType == RoleType.John)
			{
				selfAni.SetBool("Run", false);
			}
			selfAni.SetTrigger("Idle");
			roleRotate = true;
			status = ArriveStatus.arrived;
		}
	}

	private bool JudgeArrive()
	{
		if (Mathf.Abs(base.transform.position.x - targetPosition.x) < roleMoveStopDistance && Mathf.Abs(base.transform.position.z - targetPosition.z) < roleMoveStopDistance)
		{
			return true;
		}
		return false;
	}

	public void StopMove()
	{
		selfAni.SetBool("Walk", false);
		selfAni.SetBool("Run", false);
		selfAni.SetTrigger("Idle");
		underMoveControl = false;
		isDelay = false;
		roleRotate = false;
		if (mr.isOnNavMesh)
		{
			mr.SetDestination(base.transform.position);
		}
	}

	public void StopAni()
	{
		if (controlAnimName != null)
		{
			selfAni.SetBool(controlAnimName, false);
		}
		underAnimControl = false;
		isDelay = false;
		if (selfAni != null)
		{
			selfAni.SetTrigger("Idle");
		}
	}

	public void ShowBubble(string content)
	{
		CastleSceneUIManager.Instance.ShowBubble(this, content);
	}

	public void SetBubble(BubbleManager targetBubble)
	{
		if (bubble != null)
		{
			bubble.Hide();
		}
		bubble = targetBubble;
	}

	public void ClearBubble()
	{
		if (bubble != null)
		{
			bubble.DestoryBubble();
		}
		bubble = null;
	}

	public void FinishStep()
	{
		if (!isIdleActionFinish)
		{
			FinishOneCondition();
		}
	}

	public void FinishOneCondition()
	{
		DebugUtils.Log(DebugType.Other, "IdleActionFinishOneCondition");
		if (isIdleActionFinish)
		{
			return;
		}
		for (int i = 0; i < stepFinishCondition.Count; i++)
		{
			if (!stepFinishCondition[i])
			{
				stepFinishCondition[i] = true;
				break;
			}
		}
		ChangeStepFinished();
	}

	private void ChangeStepFinished()
	{
		bool flag = true;
		for (int i = 0; i < stepFinishCondition.Count; i++)
		{
			if (!stepFinishCondition[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			isStepFinished = true;
		}
		else
		{
			isStepFinished = false;
		}
	}

	private string GetRandomTask()
	{
		string[] array = IdleActionManager.Instance.GetUnlockStepId(roleType).Split(',');
		List<int> list = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array.Length > 2)
			{
				if (array[i] != "" && int.Parse(array[i].Split('.')[0]) != lastActionID)
				{
					list.Add(int.Parse(array[i].Split('.')[0]));
				}
			}
			else if (array[i] != "")
			{
				list.Add(int.Parse(array[i].Split('.')[0]));
			}
		}
		if (list.Count < 1)
		{
			return "";
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		int stepID = (lastActionID = list[index]);
		return IdleActionManager.Instance.GetActionStepById(stepID);
	}

	private string GetRandomWalk(int walkType)
	{
		string[] array = IdleActionManager.Instance.GetWalkPosition(walkType).Split(';');
		if (array.Length < 1)
		{
			return "";
		}
		int num = UnityEngine.Random.Range(0, array.Length - 1);
		return array[num];
	}

	private string GetRandomBubble(int bubbleType)
	{
		string[] array = IdleActionManager.Instance.GetBubbleText(bubbleType).Split(';');
		if (array.Length < 1)
		{
			return "";
		}
		int num = UnityEngine.Random.Range(0, array.Length - 1);
		return LanguageConfig.GetString(array[num]);
	}

	private string GetRandomAnim(string animType)
	{
		string[] array = IdleActionManager.Instance.GetAnimName(animType).Split(';');
		if (array.Length < 1)
		{
			return "";
		}
		int num = UnityEngine.Random.Range(0, array.Length - 1);
		return array[num];
	}

	private IEnumerator ChangeMoveAnim(WalkType walkType)
	{
		yield return new WaitForSeconds(0.1f);
	}

	private IEnumerator RoleMove(Vector3 TargetPosition, WalkType walkType)
	{
		yield return null;
		selfAni.SetBool("Idle", false);
		if (!JudgeArrive())
		{
			switch (walkType)
			{
			case WalkType.Walk:
				selfAni.SetBool("Walk", true);
				controlAnimName = "Walk";
				mr.speed = roleWalkSpeed;
				break;
			case WalkType.Run:
				selfAni.SetBool("Run", true);
				controlAnimName = "Run";
				mr.speed = roleRunSpeed;
				break;
			}
			mr.SetDestination(TargetPosition);
		}
	}

	public void FinishIdleAction()
	{
		isIdleActionFinish = true;
	}

	private IEnumerator StartIdleAction()
	{
		DebugUtils.Log(DebugType.Other, "StartIdleActionIEnumerator");
		isIdleActionFinish = false;
		yield return new WaitForSeconds(idleActionWaitTime);
		if (roleStatus == RoleStatus.Idle && !isIdleActionFinish && PlotManager.Instance.isPlotFinish)
		{
			IdleManager.Instance.StartIdleAction(roleType);
		}
		else
		{
			isIdleActionFinish = true;
		}
	}

	private IEnumerator StartStepAction(string stepAction)
	{
		yield return null;
		string[] actionStep = stepAction.Split(';');
		for (int i = 0; i < actionStep.Length; i++)
		{
			if (actionStep[i] != "")
			{
				StartCoroutine(DealActionStep(actionStep[i]));
				yield return new WaitUntil(() => isStepFinished);
			}
		}
		isIdleActionFinish = true;
	}

	private IEnumerator StartDealy(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		isDelay = false;
		if (!isIdleActionFinish)
		{
			FinishOneCondition();
		}
	}

	private IEnumerator DealActionStep(string Step)
	{
		stepFinishCondition.Clear();
		isStepFinished = false;
		string[] stepArray = Step.Split(',');
		for (int j = 0; j < stepArray.Length && !(stepArray[j] == ""); j++)
		{
			stepFinishCondition.Add(false);
		}
		for (int i = 0; i < stepArray.Length && !(stepArray[i] == ""); i++)
		{
			if (stepArray[i].Substring(0, 1) == "A")
			{
				string randomAnim = GetRandomAnim(stepArray[i].Substring(1));
				IdlePlayAnimation(randomAnim);
			}
			else if (stepArray[i].Substring(0, 1) == "B")
			{
				string randomBubble = GetRandomBubble(int.Parse(stepArray[i].Substring(1)));
				ShowBubble(randomBubble);
				AudioManager.Instance.PlayAudioEffect("main_bubble");
			}
			else if (stepArray[i].Substring(0, 1) == "W")
			{
				string randomWalk = GetRandomWalk(int.Parse(stepArray[i].Substring(1)));
				IdleMoveTo(randomWalk);
			}
			else if (stepArray[i].Substring(0, 1) == "D")
			{
				isDelay = true;
				StartCoroutine(StartDealy(Convert.ToSingle(stepArray[i].Substring(1))));
			}
			yield return new WaitUntil(() => !isDelay);
		}
	}

	private void OnDisable()
	{
		UserDataManager.Instance.GetService().roleSavePosition[(int)roleType].x = base.transform.position.x;
		UserDataManager.Instance.GetService().roleSavePosition[(int)roleType].y = base.transform.position.y;
		UserDataManager.Instance.GetService().roleSavePosition[(int)roleType].z = base.transform.position.z;
		UserDataManager.Instance.GetService().roleSaveRotation[(int)roleType].x = base.transform.rotation.x;
		UserDataManager.Instance.GetService().roleSaveRotation[(int)roleType].y = base.transform.rotation.y;
		UserDataManager.Instance.GetService().roleSaveRotation[(int)roleType].z = base.transform.rotation.z;
		UserDataManager.Instance.GetService().roleSaveRotation[(int)roleType].w = base.transform.rotation.w;
		UserDataManager.Instance.Save();
	}
}
