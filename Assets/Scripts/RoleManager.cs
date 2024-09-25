using System;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class RoleManager : MonoBehaviour
{
	[Serializable]
	public class RoleConfigDataList
	{
		public List<RoleConfigData> data = new List<RoleConfigData>();
	}

	public GameObject roleAlice;

	public GameObject roleJohn;

	public GameObject roleArthur;

	public GameObject roleCat;

	public GameObject roleTina;

	public List<GameObject> roles;

	public Dictionary<RoleType, Role> roleDictionary = new Dictionary<RoleType, Role>();

	public List<RoleConfigData> roleConfig;

	private static RoleManager instance;

	public static RoleManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Role/RoleConfig") as TextAsset;
		if (textAsset != null)
		{
			RoleConfigDataList roleConfigDataList = JsonUtility.FromJson<RoleConfigDataList>(textAsset.text);
			roleConfig = roleConfigDataList.data;
		}
	}

	private void Start()
	{
		UpdateRoleDisplay();
	}

	public void UpdateRoleDisplay()
	{
		GenerateAllUnlockRoles();
		if (!TestConfig.active || !TestConfig.cinemaMode)
		{
			DestoryRole();
			GenerateAndDisAppearRole();
			DealRolePosition();
		}
	}

	public void StopIdle()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.StopIdle();
		}
	}

	public void RoleUnderPlotControl()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.UnderPlotControl();
		}
	}

	public void RoleReleasePlotControl()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.ReleasePlotControl();
		}
	}

	private void DestoryAllRole()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			if (item.Key != 0)
			{
				UnityEngine.Object.Destroy(item.Value.gameObject);
			}
		}
		Role role = new Role();
		role = ((!roleDictionary.ContainsKey(RoleType.Alice)) ? GameObject.Find("Role_Alice").GetComponent<Role>() : roleDictionary[RoleType.Alice]);
		roleDictionary.Clear();
		roleDictionary.Add(RoleType.Alice, role);
	}

	private void DestoryRole()
	{
		List<RoleType> list = new List<RoleType>(roleDictionary.Keys);
		for (int i = 0; i < roleDictionary.Count; i++)
		{
			if (!JudgeRoleExist(list[i]) && list[i] != 0)
			{
				UnityEngine.Object.Destroy(roleDictionary[list[i]].gameObject);
				roleDictionary.Remove(list[i]);
				list = new List<RoleType>(roleDictionary.Keys);
				i--;
			}
		}
	}

	private bool JudgeRoleExist(RoleType roleType)
	{
		TaskConfigData showRoleData = TaskManager.Instance.GetShowRoleData();
		if (showRoleData.ExistRoleID == null || showRoleData.ExistRoleID == "")
		{
			return false;
		}
		string[] array = showRoleData.ExistRoleID.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			if (int.Parse(array[i].Split('.')[0]) == (int)roleType)
			{
				return true;
			}
		}
		return false;
	}

	public void HideAllRoles()
	{
		StopAni();
		StopMove();
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.gameObject.SetActive(false);
		}
	}

	public void ShowAllRoles()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			if (item.Value != null)
			{
				item.Value.gameObject.SetActive(true);
				item.Value.ShowRoleStartIdle();
			}
		}
	}

	public void GenerateAndDisAppearRole()
	{
		TaskConfigData showRoleData = TaskManager.Instance.GetShowRoleData();
		if (showRoleData.ExistRoleID == null)
		{
			return;
		}
		string[] array = showRoleData.ExistRoleID.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "")
			{
				if (int.Parse(array[i].Split('.')[0]) == 1 && !roleDictionary.ContainsKey(RoleType.John))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(roleJohn, base.transform);
					roleDictionary.Add(RoleType.John, gameObject.GetComponent<Role>());
				}
				else if (int.Parse(array[i].Split('.')[0]) == 2 && !roleDictionary.ContainsKey(RoleType.Arthur))
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(roleArthur, base.transform);
					roleDictionary.Add(RoleType.Arthur, gameObject2.GetComponent<Role>());
				}
				else if (int.Parse(array[i].Split('.')[0]) == 3 && !roleDictionary.ContainsKey(RoleType.Cat))
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(roleCat, base.transform);
					roleDictionary.Add(RoleType.Cat, gameObject3.GetComponent<Role>());
				}
				else if (int.Parse(array[i].Split('.')[0]) == 4 && !roleDictionary.ContainsKey(RoleType.Tina))
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(roleTina, base.transform);
					roleDictionary.Add(RoleType.Tina, gameObject4.GetComponent<Role>());
				}
			}
		}
	}

	public void GenerateAllRoles()
	{
		for (int i = 0; i < roles.Count; i++)
		{
			if (!roleDictionary.ContainsKey(roles[i].GetComponent<Role>().roleType))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(roles[i], base.transform);
				roleDictionary.Add(roles[i].GetComponent<Role>().roleType, gameObject.GetComponent<Role>());
			}
		}
	}

	public void GenerateAllUnlockRoles()
	{
		for (int i = 0; i < roles.Count; i++)
		{
			if (!roleDictionary.ContainsKey(roles[i].GetComponent<Role>().roleType) && roles[i].GetComponent<Role>().roleType != 0)
			{
				PositionInfo positionInfo = UserDataManager.Instance.GetService().roleSavePosition[(int)roles[i].GetComponent<Role>().roleType];
				if (positionInfo.x != 0f || positionInfo.y != 0f || positionInfo.z != 0f)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(roles[i], base.transform);
					roleDictionary.Add(roles[i].GetComponent<Role>().roleType, gameObject.GetComponent<Role>());
				}
			}
		}
	}

	private void DealRolePosition()
	{
		TaskConfigData showRoleData = TaskManager.Instance.GetShowRoleData();
		if (showRoleData.GenerateRole != null && showRoleData.GenerateRole != "")
		{
			int num = int.Parse(showRoleData.GenerateRole.Substring(0, 1));
			string text = showRoleData.GenerateRole.Split('(')[1];
			float x = Convert.ToSingle(text.Split(',')[0]);
			float y = Convert.ToSingle(text.Split(',')[1]);
			float z = Convert.ToSingle(text.Split(',')[2].Split(')')[0]);
			switch (num)
			{
			case 1:
				roleDictionary[RoleType.John].transform.position = new Vector3(x, y, z);
				roleDictionary[RoleType.John].transform.rotation = GetLookAtPosition(showRoleData.GenerateRole);
				break;
			case 2:
				roleDictionary[RoleType.Arthur].transform.position = new Vector3(x, y, z);
				roleDictionary[RoleType.Arthur].transform.rotation = GetLookAtPosition(showRoleData.GenerateRole);
				break;
			case 3:
				roleDictionary[RoleType.Cat].transform.position = new Vector3(x, y, z);
				roleDictionary[RoleType.Cat].transform.rotation = GetLookAtPosition(showRoleData.GenerateRole);
				break;
			case 4:
				roleDictionary[RoleType.Tina].transform.position = new Vector3(x, y, z);
				roleDictionary[RoleType.Tina].transform.rotation = GetLookAtPosition(showRoleData.GenerateRole);
				break;
			}
		}
	}

	public Quaternion GetLookAtPosition(string moveString)
	{
		string[] array = moveString.Split('(');
		Quaternion result = default(Quaternion);
		if (array.Length < 3)
		{
			return result;
		}
		string[] array2 = array[2].Split(',');
		result.x = Convert.ToSingle(array2[0]);
		result.y = Convert.ToSingle(array2[1]);
		result.z = Convert.ToSingle(array2[2]);
		result.w = Convert.ToSingle(array2[3].Split(')')[0]);
		return result;
	}

	public void AddRole(RoleType roleType, Role role)
	{
		roleDictionary.Add(roleType, role);
	}

	public Role GetRole(RoleType roleType)
	{
		return Instance.roleDictionary[roleType];
	}

	public Role CreateRoleWithRoleType(RoleType roleType)
	{
		Role result = new Role();
		if (roleType == RoleType.John && !roleDictionary.ContainsKey(RoleType.John))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(roleJohn, base.transform);
			roleDictionary.Add(RoleType.John, gameObject.GetComponent<Role>());
			result = roleDictionary[RoleType.John];
		}
		else if (roleType == RoleType.Arthur && !roleDictionary.ContainsKey(RoleType.Arthur))
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(roleArthur, base.transform);
			roleDictionary.Add(RoleType.Arthur, gameObject2.GetComponent<Role>());
			result = roleDictionary[RoleType.Arthur];
		}
		else if (roleType == RoleType.Cat && !roleDictionary.ContainsKey(RoleType.Cat))
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(roleCat, base.transform);
			roleDictionary.Add(RoleType.Cat, gameObject3.GetComponent<Role>());
			result = roleDictionary[RoleType.Cat];
		}
		else if (roleType == RoleType.Tina && !roleDictionary.ContainsKey(RoleType.Tina))
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(roleTina, base.transform);
			roleDictionary.Add(RoleType.Tina, gameObject4.GetComponent<Role>());
			result = roleDictionary[RoleType.Tina];
		}
		return result;
	}

	public void RoleMove(RoleType roleType, MoveType moveType, Vector3 TargetPosition, Quaternion lookAtPosition, WalkType walkType)
	{
		switch (moveType)
		{
		case MoveType.Move:
			if (!roleDictionary.ContainsKey(roleType))
			{
				CreateRoleWithRoleType(roleType);
			}
			switch (roleType)
			{
			case RoleType.Alice:
				roleDictionary[RoleType.Alice].MoveTo(TargetPosition, lookAtPosition, walkType);
				break;
			case RoleType.John:
				roleDictionary[RoleType.John].MoveTo(TargetPosition, lookAtPosition, walkType);
				break;
			case RoleType.Arthur:
				roleDictionary[RoleType.Arthur].MoveTo(TargetPosition, lookAtPosition, walkType);
				break;
			case RoleType.Cat:
				roleDictionary[RoleType.Cat].MoveTo(TargetPosition, lookAtPosition, walkType);
				break;
			case RoleType.Tina:
				roleDictionary[RoleType.Tina].MoveTo(TargetPosition, lookAtPosition, walkType);
				break;
			}
			break;
		case MoveType.Flash:
			if (roleType == RoleType.Alice && roleDictionary.ContainsKey(RoleType.Alice))
			{
				roleDictionary[RoleType.Alice].FlashTo(TargetPosition, lookAtPosition);
			}
			else if (roleType == RoleType.John && roleDictionary.ContainsKey(RoleType.John))
			{
				roleDictionary[RoleType.John].FlashTo(TargetPosition, lookAtPosition);
			}
			else if (roleType == RoleType.Arthur && roleDictionary.ContainsKey(RoleType.Arthur))
			{
				roleDictionary[RoleType.Arthur].FlashTo(TargetPosition, lookAtPosition);
			}
			else if (roleType == RoleType.Cat && roleDictionary.ContainsKey(RoleType.Cat))
			{
				roleDictionary[RoleType.Cat].FlashTo(TargetPosition, lookAtPosition);
			}
			else if (roleType == RoleType.Tina && roleDictionary.ContainsKey(RoleType.Tina))
			{
				roleDictionary[RoleType.Tina].FlashTo(TargetPosition, lookAtPosition);
			}
			break;
		}
	}

	public void RolePlayAni(RoleType roleType, string AnimName)
	{
		switch (roleType)
		{
		case RoleType.Alice:
			roleDictionary[RoleType.Alice].PlayAnimation(AnimName);
			return;
		case RoleType.John:
			if (roleDictionary.ContainsKey(RoleType.John))
			{
				roleDictionary[RoleType.John].PlayAnimation(AnimName);
				return;
			}
			break;
		}
		if (roleType == RoleType.Arthur && roleDictionary.ContainsKey(RoleType.Arthur))
		{
			roleDictionary[RoleType.Arthur].PlayAnimation(AnimName);
		}
		else if (roleType == RoleType.Cat && roleDictionary.ContainsKey(RoleType.Cat))
		{
			roleDictionary[RoleType.Cat].PlayAnimation(AnimName);
		}
		else if (roleType == RoleType.Tina && roleDictionary.ContainsKey(RoleType.Tina))
		{
			roleDictionary[RoleType.Tina].PlayAnimation(AnimName);
		}
	}

	public void StopMove()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.StopMove();
			item.Value.StopRoleIdleAction();
			item.Value.ClearBubble();
		}
	}

	public void StopAni()
	{
		foreach (KeyValuePair<RoleType, Role> item in roleDictionary)
		{
			item.Value.StopMove();
			item.Value.StopAni();
			item.Value.StopRoleIdleAction();
			item.Value.ClearBubble();
		}
	}

	public List<int> GetUnlockRoleInfo(int taskID)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < roleConfig.Count; i++)
		{
			if (taskID == int.Parse(roleConfig[i].UnlockPlotId.Split(',')[1]) && UserDataManager.Instance.GetService().stage == int.Parse(roleConfig[i].UnlockPlotId.Split(',')[0]))
			{
				list.Add(roleConfig[i].ID);
			}
		}
		return list;
	}
}
