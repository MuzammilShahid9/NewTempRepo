using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class PlotRoleAniManager : MonoBehaviour
{
	[Serializable]
	public class RoleAniConfigDataList
	{
		public List<RoleAniConfigData> data = new List<RoleAniConfigData>();
	}

	[Serializable]
	public class RoleAniConfigData
	{
		public int ID;

		public string John;

		public string Alice;

		public string Arthur;

		public string Tina;

		public string Cat;
	}

	private static PlotRoleAniManager instance;

	private List<RoleAniConfigData> roleAniConfig;

	private RoleAniConfigData currRoleAniData;

	private string currRoleAniString;

	private int plotStep;

	private List<bool> stepFinishCondition = new List<bool>();

	private bool isStepFinished;

	public static PlotRoleAniManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Plot/RoleAniConfig") as TextAsset;
		if (textAsset != null)
		{
			RoleAniConfigDataList roleAniConfigDataList = JsonUtility.FromJson<RoleAniConfigDataList>(textAsset.text);
			roleAniConfig = roleAniConfigDataList.data;
		}
	}

	public void StartRoleAni(string roleAniIDString)
	{
		currRoleAniString = roleAniIDString;
		string text = roleAniIDString.Substring(1);
		int num = -1;
		if (text != "")
		{
			num = int.Parse(roleAniIDString.Substring(1));
		}
		if (num == -1)
		{
			FinishStep();
			return;
		}
		for (int i = 0; i < roleAniConfig.Count; i++)
		{
			if (roleAniConfig[i].ID == num)
			{
				currRoleAniData = roleAniConfig[i];
			}
		}
		isStepFinished = false;
		StartCoroutine(ProcessRoleMove());
		DealRoleMove(1);
	}

	public void StartRoleAction(CDRoleConfig currRoleConfig, int currStep)
	{
		StopAllCoroutines();
		plotStep = currStep;
		isStepFinished = false;
		StartCoroutine(ProcessRoleMove());
		stepFinishCondition.Clear();
		foreach (RoleType key in currRoleConfig.roles.Keys)
		{
			if (RoleManager.Instance.roleDictionary.ContainsKey(key))
			{
				stepFinishCondition.Add(false);
				RoleManager.Instance.roleDictionary[key].Run(currRoleConfig.roles[key], plotStep);
			}
			else
			{
				Role role = RoleManager.Instance.CreateRoleWithRoleType(key);
				stepFinishCondition.Add(false);
				StartCoroutine(RoleProcessActioin(role, currRoleConfig.roles[key]));
			}
		}
	}

	private IEnumerator RoleProcessActioin(Role role, CDRoleAnim roleAnim)
	{
		yield return null;
		role.UnderPlotControl();
		role.Run(roleAnim, plotStep);
	}

	public RoleAniConfigData GetRoleAni(string roleAniIDString)
	{
		currRoleAniString = roleAniIDString;
		string text = roleAniIDString.Substring(1);
		int num = -1;
		if (text != "")
		{
			num = int.Parse(roleAniIDString.Substring(1));
		}
		for (int i = 0; i < roleAniConfig.Count; i++)
		{
			if (roleAniConfig[i].ID == num)
			{
				return roleAniConfig[i];
			}
		}
		return null;
	}

	public void DealRoleMove(int step)
	{
		stepFinishCondition.Clear();
		isStepFinished = false;
		if (currRoleAniData.Alice != "" && currRoleAniString.Substring(0, 1) == "A")
		{
			stepFinishCondition.Add(false);
			RoleManager.Instance.RolePlayAni(RoleType.Alice, currRoleAniData.Alice);
		}
	}

	public void FinishOneCondition(int currStep)
	{
		if (currStep != plotStep)
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

	public void FinishStep()
	{
		PlotManager.Instance.FinishOneCondition(plotStep);
	}

	public void RestortPlotStep()
	{
		plotStep = -2;
	}

	public void StopStep()
	{
		isStepFinished = true;
		RoleManager.Instance.StopAni();
	}

	private IEnumerator ProcessRoleMove()
	{
		yield return new WaitUntil(() => isStepFinished);
		FinishStep();
	}
}
