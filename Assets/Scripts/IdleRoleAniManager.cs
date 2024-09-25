using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using UnityEngine;

public class IdleRoleAniManager : MonoBehaviour
{
	private static IdleRoleAniManager instance;

	private int[] idleCurrStepArray;

	private bool[] isIdleStepFinishArray;

	private Coroutine[] roleIdleActionIEnumeratorArray;

	private List<bool>[] idleStepFinishConditionArrayList;

	public static IdleRoleAniManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		idleCurrStepArray = new int[GeneralConfig.RoleNumber];
		isIdleStepFinishArray = new bool[GeneralConfig.RoleNumber];
		roleIdleActionIEnumeratorArray = new Coroutine[GeneralConfig.RoleNumber];
		idleStepFinishConditionArrayList = new List<bool>[GeneralConfig.RoleNumber];
	}

	private void Start()
	{
		for (int i = 0; i < GeneralConfig.RoleNumber; i++)
		{
			idleCurrStepArray[i] = 0;
			isIdleStepFinishArray[i] = true;
			List<bool> list = new List<bool>();
			idleStepFinishConditionArrayList[i] = list;
		}
	}

	public void StartRoleAction(IDRoleConfig currRoleConfig, int currStep, RoleType roleType)
	{
		idleCurrStepArray[(int)roleType] = currStep;
		isIdleStepFinishArray[(int)roleType] = false;
		idleStepFinishConditionArrayList[(int)roleType].Clear();
		foreach (RoleType key in currRoleConfig.roles.Keys)
		{
			if (RoleManager.Instance.roleDictionary.ContainsKey(key))
			{
				idleStepFinishConditionArrayList[(int)roleType].Add(false);
				RoleManager.Instance.roleDictionary[key].IdleRun(currRoleConfig.roles[key], currStep, roleType);
			}
		}
		if (roleIdleActionIEnumeratorArray[(int)roleType] != null)
		{
			StopCoroutine(roleIdleActionIEnumeratorArray[(int)roleType]);
		}
		roleIdleActionIEnumeratorArray[(int)roleType] = StartCoroutine(ProcessRoleMove(roleType));
	}

	public void StopIdle()
	{
		StopAllCoroutines();
		RoleManager.Instance.StopIdle();
	}

	private IEnumerator ProcessRoleMove(RoleType roleType)
	{
		yield return new WaitUntil(() => isIdleStepFinishArray[(int)roleType]);
		FinishStep(roleType);
	}

	public void FinishStep(RoleType roleType)
	{
		IdleManager.Instance.FinishOneCondition(roleType, idleCurrStepArray[(int)roleType]);
	}

	public void FinishOneCondition(RoleType roleType, int step)
	{
		if (idleCurrStepArray[(int)roleType] != step)
		{
			return;
		}
		for (int i = 0; i < idleStepFinishConditionArrayList[(int)roleType].Count; i++)
		{
			if (!idleStepFinishConditionArrayList[(int)roleType][i])
			{
				idleStepFinishConditionArrayList[(int)roleType][i] = true;
				break;
			}
		}
		ChangeStepFinished(roleType);
	}

	private void ChangeStepFinished(RoleType roleType)
	{
		bool flag = true;
		for (int i = 0; i < idleStepFinishConditionArrayList[(int)roleType].Count; i++)
		{
			if (!idleStepFinishConditionArrayList[(int)roleType][i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			isIdleStepFinishArray[(int)roleType] = true;
		}
		else
		{
			isIdleStepFinishArray[(int)roleType] = false;
		}
	}
}
