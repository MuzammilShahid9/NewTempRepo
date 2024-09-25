using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class PlotCameraManager : MonoBehaviour
{
	[Serializable]
	public class CameraConfigDataList
	{
		public List<CameraConfigData> data = new List<CameraConfigData>();
	}

	[Serializable]
	public class CameraConfigData
	{
		public string Key;

		public int MoveOrFlash;

		public string Position;

		public float MoveTime;

		public float ScaleTime;
	}

	public class CameraAction
	{
		public Vector3 targetPosition;

		public float tm;

		public float targetScale;
	}

	private static PlotCameraManager instance;

	public bool isStepFinished;

	private List<CameraConfigData> cameraConfig;

	private CameraConfigData currCameraData;

	private bool isLockToRole;

	private string lockActor;

	private int currStep;

	private int plotStep;

	public static PlotCameraManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		isStepFinished = true;
		CameraConfigDataList cameraConfigDataList = JsonUtility.FromJson<CameraConfigDataList>((Resources.Load("Config/Plot/CameraConfig") as TextAsset).text);
		cameraConfig = cameraConfigDataList.data;
	}

	public void StartCamera(string cameraID, bool lockToRole = false, string lockToActor = "")
	{
		isLockToRole = lockToRole;
		lockActor = lockToActor;
		currCameraData = null;
		for (int i = 0; i < cameraConfig.Count; i++)
		{
			if (cameraConfig[i].Key == cameraID)
			{
				currCameraData = cameraConfig[i];
			}
		}
		DealCamera(1);
	}

	public void StartCameraAction(CDCameraConfig currCameraConfig, int currStep)
	{
		plotStep = currStep;
		isStepFinished = false;
		RunCameraAction(currCameraConfig);
	}

	public CameraConfigData GetCameraMove(string cameraID, bool lockToRole = false, string lockToActor = "")
	{
		isLockToRole = lockToRole;
		lockActor = lockToActor;
		currCameraData = null;
		for (int i = 0; i < cameraConfig.Count; i++)
		{
			if (cameraConfig[i].Key == cameraID)
			{
				return cameraConfig[i];
			}
		}
		return null;
	}

	public void RunCameraActionFromString(string animData)
	{
		if (animData[0] != 'F')
		{
			string[] array = animData.Split(';');
			Vector3 vec3ByString = MathTool.GetVec3ByString(array[0]);
			CameraControl.Instance.GoTo2DPosition(vec3ByString, float.Parse(array[1]), float.Parse(array[2]));
		}
	}

	public void RunCameraAction(CDCameraConfig config)
	{
		if (config.isFollow)
		{
			CameraControl.Instance.LockToRole(config.followRole);
			StartCoroutine(DelayFinishStep());
		}
		else
		{
			CameraControl.Instance.GoTo2DPosition(config.pos, config.size, config.tm);
		}
	}

	private IEnumerator DelayFinishStep()
	{
		yield return null;
		FinishStep();
	}

	public void DealCamera(int Step)
	{
		isStepFinished = false;
		currStep = Step;
		if (currCameraData == null)
		{
			FinishStep();
			return;
		}
		float x = 0f;
		float y = 0f;
		float z = 0f;
		float scale = 3f;
		if (currCameraData.Position != "")
		{
			if (isLockToRole)
			{
				Vector3 lockCameraPosition = GetLockCameraPosition(lockActor);
				x = lockCameraPosition.x;
				y = lockCameraPosition.y;
				z = lockCameraPosition.z;
				scale = Convert.ToSingle(currCameraData.Position);
			}
			else
			{
				string[] array = currCameraData.Position.Split(',');
				x = Convert.ToSingle(array[0].Substring(1));
				y = Convert.ToSingle(array[1]);
				z = Convert.ToSingle(array[2].Split(')')[0]);
				scale = Convert.ToSingle(currCameraData.Position.Split(';')[1]);
			}
		}
		CameraControl.Instance.MoveTo2DPosition(new Vector3(x, y, z), currCameraData.MoveTime, scale, currCameraData.ScaleTime);
	}

	public Vector3 GetLockCameraPosition(string Actor)
	{
		return new Vector3(1f, 1f, 1f);
	}

	public void BuildMoveTo(Vector3 itemPosition, float moveTime)
	{
		CameraControl.Instance.BuildControlMoveToPosition(itemPosition, moveTime);
	}

	public void LockCamera()
	{
		CameraControl.Instance.LockCamera();
	}

	public void UnlockCamera()
	{
		CameraControl.Instance.UnlockCamra();
	}

	public void LockToRole(string Actor)
	{
		StartCoroutine(DelayLockToRole(Actor));
	}

	private IEnumerator DelayLockToRole(string Actor)
	{
		yield return new WaitUntil(() => isStepFinished);
		CameraControl.Instance.LockToAlice(Actor);
	}

	public void UnlockToRole()
	{
		CameraControl.Instance.UnlockToAlice();
	}

	public void FinishStep()
	{
		if (!isStepFinished)
		{
			DebugUtils.Log(DebugType.Plot, "PlotCameraFinish");
			isStepFinished = true;
			UnlockToRole();
			PlotManager.Instance.FinishOneCondition(plotStep);
		}
	}

	public void RestortPlotStep()
	{
		plotStep = -2;
	}

	public void StopStep()
	{
		isStepFinished = true;
		CameraControl.Instance.StopMove();
	}
}
