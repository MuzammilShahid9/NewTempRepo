using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotRoleMoveManager : MonoBehaviour
{
	[Serializable]
	public class RoleMoveConfigDataList
	{
		public List<RoleMoveConfigData> data = new List<RoleMoveConfigData>();
	}

	[Serializable]
	public class RoleMoveConfigData
	{
		public string Key;

		public string Alice;

		public string John;

		public string Arthur;

		public string Cat;

		public string Tina;
	}

	private static PlotRoleMoveManager instance;

	public List<RoleMoveConfigData> roleMoveConfig;

	public RoleMoveConfigData currRoleMoveData;

	private List<bool> stepFinishCondition = new List<bool>();

	private bool isStepFinished;

	private bool isLockToRole;

	public static PlotRoleMoveManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Plot/RoleMoveConfig") as TextAsset;
		if (textAsset != null)
		{
			RoleMoveConfigDataList roleMoveConfigDataList = JsonUtility.FromJson<RoleMoveConfigDataList>(textAsset.text);
			roleMoveConfig = roleMoveConfigDataList.data;
		}
	}

	public void StartRoleMove(string roleMoveID, bool lockToRole = false)
	{
		isLockToRole = lockToRole;
		for (int i = 0; i < roleMoveConfig.Count; i++)
		{
			if (roleMoveConfig[i].Key == roleMoveID)
			{
				currRoleMoveData = roleMoveConfig[i];
			}
		}
		isStepFinished = false;
		StartCoroutine(ProcessRoleMove());
		StartCoroutine(DealRoleMove());
	}

	public RoleMoveConfigData GetRoleMove(string roleMoveID, bool lockToRole = false)
	{
		isLockToRole = lockToRole;
		for (int i = 0; i < roleMoveConfig.Count; i++)
		{
			if (roleMoveConfig[i].Key == roleMoveID)
			{
				return roleMoveConfig[i];
			}
		}
		return null;
	}

	public string GetRoleMoveType(string roleMoveID, string roleType)
	{
		RoleMoveConfigData roleMoveConfigData = new RoleMoveConfigData();
		string result = "";
		for (int i = 0; i < roleMoveConfig.Count; i++)
		{
			if (roleMoveConfig[i].Key == roleMoveID)
			{
				roleMoveConfigData = roleMoveConfig[i];
			}
		}
		switch (roleType)
		{
		case "A":
			result = roleMoveConfigData.Alice.Substring(0, 1);
			break;
		case "B":
			result = roleMoveConfigData.John.Substring(0, 1);
			break;
		case "C":
			result = roleMoveConfigData.Arthur.Substring(0, 1);
			break;
		case "D":
			result = roleMoveConfigData.Tina.Substring(0, 1);
			break;
		case "E":
			result = roleMoveConfigData.Cat.Substring(0, 1);
			break;
		}
		return result;
	}

	private IEnumerator DealRoleMove()
	{
		if (isLockToRole)
		{
			yield return new WaitUntil(() => PlotCameraManager.Instance.isStepFinished);
		}
		stepFinishCondition.Clear();
		isStepFinished = false;
		if (currRoleMoveData == null || (currRoleMoveData.Alice == "" && currRoleMoveData.John == "" && currRoleMoveData.Arthur == "" && currRoleMoveData.Cat == "" && currRoleMoveData.Tina == ""))
		{
			FinishStep();
			yield break;
		}
		if (currRoleMoveData.Alice != "")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RoleMove(RoleType.Alice, GetMoveType(currRoleMoveData.Alice), GetTargetPosition(currRoleMoveData.Alice), GetLookAtPosition(currRoleMoveData.Alice), GetWalkType(currRoleMoveData.Alice));
		}
		if (currRoleMoveData.John != "")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RoleMove(RoleType.John, GetMoveType(currRoleMoveData.John), GetTargetPosition(currRoleMoveData.John), GetLookAtPosition(currRoleMoveData.John), GetWalkType(currRoleMoveData.John));
		}
		if (currRoleMoveData.Arthur != "")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RoleMove(RoleType.Arthur, GetMoveType(currRoleMoveData.Arthur), GetTargetPosition(currRoleMoveData.Arthur), GetLookAtPosition(currRoleMoveData.Arthur), GetWalkType(currRoleMoveData.Arthur));
		}
		if (currRoleMoveData.Cat != "")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RoleMove(RoleType.Cat, GetMoveType(currRoleMoveData.Cat), GetTargetPosition(currRoleMoveData.Cat), GetLookAtPosition(currRoleMoveData.Cat), GetWalkType(currRoleMoveData.Cat));
		}
		if (currRoleMoveData.Tina != "")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RoleMove(RoleType.Tina, GetMoveType(currRoleMoveData.Tina), GetTargetPosition(currRoleMoveData.Tina), GetLookAtPosition(currRoleMoveData.Tina), GetWalkType(currRoleMoveData.Tina));
		}
		yield return null;
	}

	public void FinishOneCondition()
	{
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

	public void FinishStep()
	{
		PlotManager.Instance.FinishOneCondition();
		CameraControl.Instance.UnlockToAlice();
	}

	public void StopStep()
	{
		RoleManager.Instance.StopMove();
		StopAllCoroutines();
	}

	public MoveType GetMoveType(string moveString)
	{
		if (moveString.Substring(0, 1) == "R")
		{
			return MoveType.Move;
		}
		if (moveString.Substring(0, 1) == "F")
		{
			return MoveType.Flash;
		}
		if (moveString.Substring(0, 1) == "W")
		{
			return MoveType.Move;
		}
		return MoveType.none;
	}

	public WalkType GetWalkType(string moveString)
	{
		if (moveString.Substring(0, 1) == "R")
		{
			return WalkType.Run;
		}
		if (moveString.Substring(0, 1) == "W")
		{
			return WalkType.Walk;
		}
		return WalkType.none;
	}

	private IEnumerator ProcessRoleMove()
	{
		yield return new WaitUntil(() => isStepFinished);
		FinishStep();
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
}
