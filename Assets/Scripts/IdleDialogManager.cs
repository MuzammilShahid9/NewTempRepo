using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using UnityEngine;

public class IdleDialogManager : MonoBehaviour
{
	private static IdleDialogManager instance;

	private int[] idleCurrStepArray;

	private int[] idleCurrConversationIndexArray;

	private bool[] isIdleStepFinishArray;

	private Coroutine[] roleIdleActionIEnumeratorArray;

	private List<string>[] idleStepFinishConditionArrayList;

	private IdleBubbleConfigData[] idleBubbleConfigDataArray;

	public static IdleDialogManager Instance
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
		idleCurrConversationIndexArray = new int[GeneralConfig.RoleNumber];
		isIdleStepFinishArray = new bool[GeneralConfig.RoleNumber];
		roleIdleActionIEnumeratorArray = new Coroutine[GeneralConfig.RoleNumber];
		idleStepFinishConditionArrayList = new List<string>[GeneralConfig.RoleNumber];
		idleBubbleConfigDataArray = new IdleBubbleConfigData[GeneralConfig.RoleNumber];
	}

	private void Start()
	{
		for (int i = 0; i < GeneralConfig.RoleNumber; i++)
		{
			idleCurrStepArray[i] = 0;
			idleCurrConversationIndexArray[i] = 0;
			isIdleStepFinishArray[i] = true;
			List<string> list = new List<string>();
			idleStepFinishConditionArrayList[i] = list;
		}
	}

	public void StartConversationAction(IDConversationConfig currConversationConfig, int currStep, RoleType roleType)
	{
		if (currConversationConfig.convList.Count > 0)
		{
			idleCurrStepArray[(int)roleType] = currStep;
			isIdleStepFinishArray[(int)roleType] = false;
			idleStepFinishConditionArrayList[(int)roleType].Clear();
			idleCurrConversationIndexArray[(int)roleType] = 0;
			string[] array = currConversationConfig.convList.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				idleStepFinishConditionArrayList[(int)roleType].Add(array[i]);
			}
			RunNextConversation(roleType);
			roleIdleActionIEnumeratorArray[(int)roleType] = StartCoroutine(ProcessRoleMove(roleType));
		}
	}

	public void RunNextConversation(RoleType roleType)
	{
		if (idleCurrConversationIndexArray[(int)roleType] >= idleStepFinishConditionArrayList[(int)roleType].Count)
		{
			StartCoroutine(DelayPlotFinish(roleType));
			return;
		}
		string text = idleStepFinishConditionArrayList[(int)roleType][idleCurrConversationIndexArray[(int)roleType]];
		DebugUtils.Log(DebugType.Other, "Run conversation " + text);
		if (text == "")
		{
			StartCoroutine(DelayPlotFinish(roleType));
			return;
		}
		string[] array = text.Split('|');
		string[] array2 = array[0].Split(',');
		int num = UnityEngine.Random.Range(0, array2.Length);
		string key = array2[num];
		idleBubbleConfigDataArray[(int)roleType] = new IdleBubbleConfigData();
		idleBubbleConfigDataArray[(int)roleType].Key = key;
		idleBubbleConfigDataArray[(int)roleType].roleType = (RoleType)Enum.Parse(typeof(RoleType), array[1]);
		roleIdleActionIEnumeratorArray[(int)roleType] = StartCoroutine(DealDialog(roleType, idleCurrConversationIndexArray[(int)roleType]));
		idleCurrConversationIndexArray[(int)roleType]++;
	}

	public void StopIdle()
	{
		StopAllCoroutines();
		IdleBubbleManager.Instance.StopIdle();
	}

	private IEnumerator DealDialog(RoleType roleType, int currStep)
	{
		yield return null;
		isIdleStepFinishArray[(int)roleType] = false;
		if (idleBubbleConfigDataArray[(int)roleType] == null)
		{
			FinishStep(roleType);
			yield break;
		}
		RoleType roleType2 = roleType;
		int num = currStep;
		IdleBubbleManager.Instance.ShowBubble(idleBubbleConfigDataArray[(int)roleType], currStep);
		AudioManager.Instance.PlayAudioEffect("main_bubble");
	}

	private IEnumerator DelayPlotFinish(RoleType roleType)
	{
		yield return null;
		Debug.Log("IdleDialogFinish");
		IdleManager.Instance.FinishOneCondition(roleType, idleCurrStepArray[(int)roleType]);
	}

	private IEnumerator ProcessRoleMove(RoleType roleType)
	{
		yield return new WaitUntil(() => isIdleStepFinishArray[(int)roleType]);
		FinishStep(roleType);
	}

	public void FinishStep(RoleType roleType)
	{
		RunNextConversation(roleType);
	}
}
