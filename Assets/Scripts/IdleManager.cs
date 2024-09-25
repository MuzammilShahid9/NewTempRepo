using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
	private static IdleManager instance;

	private IDInfo[] roleLastIdleActionArray;

	private Coroutine[] roleIdleActionIEnumeratorArray;

	private bool[] isIdleFinishArray;

	private bool[] isIdleStepFinishArray;

	private int[] idleCurrStepArray;

	private List<bool>[] idleStepFinishConditionArrayList;

	public static IdleManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		roleLastIdleActionArray = new IDInfo[GeneralConfig.RoleNumber];
		roleIdleActionIEnumeratorArray = new Coroutine[GeneralConfig.RoleNumber];
		isIdleFinishArray = new bool[GeneralConfig.RoleNumber];
		isIdleStepFinishArray = new bool[GeneralConfig.RoleNumber];
		idleCurrStepArray = new int[GeneralConfig.RoleNumber];
		idleStepFinishConditionArrayList = new List<bool>[GeneralConfig.RoleNumber];
	}

	private void Start()
	{
		for (int i = 0; i < GeneralConfig.RoleNumber; i++)
		{
			idleCurrStepArray[i] = 0;
			isIdleFinishArray[i] = true;
			isIdleStepFinishArray[i] = true;
			List<bool> list = new List<bool>();
			idleStepFinishConditionArrayList[i] = list;
		}
	}

	public void StartIdleAction(RoleType roleType)
	{
		List<IDInfo> unlockIdleAction = IdleConfigManager.Instance.GetUnlockIdleAction(roleType);
		List<IDInfo> list = new List<IDInfo>();
		for (int i = 0; i < unlockIdleAction.Count; i++)
		{
			if (unlockIdleAction.Count > 2)
			{
				if (unlockIdleAction[i] != roleLastIdleActionArray[(int)roleType])
				{
					list.Add(unlockIdleAction[i]);
				}
			}
			else
			{
				list.Add(unlockIdleAction[i]);
			}
		}
		if (list.Count >= 1)
		{
			int index = Random.Range(0, list.Count);
			roleLastIdleActionArray[(int)roleType] = list[index];
			if (roleIdleActionIEnumeratorArray[(int)roleType] != null)
			{
				StopCoroutine(roleIdleActionIEnumeratorArray[(int)roleType]);
			}
			roleIdleActionIEnumeratorArray[(int)roleType] = StartCoroutine(StartRoleIdleAction(list[index], roleType));
		}
	}

	public void StartIdleActionWithIDInfo(IDInfo tempIDInfo)
	{
		if (roleIdleActionIEnumeratorArray[(int)tempIDInfo.roleType] != null)
		{
			StopCoroutine(roleIdleActionIEnumeratorArray[(int)tempIDInfo.roleType]);
		}
		roleIdleActionIEnumeratorArray[(int)tempIDInfo.roleType] = StartCoroutine(StartRoleIdleAction(tempIDInfo, tempIDInfo.roleType));
	}

	public void StartIdleActionWithActionInfo(IDActionManager.Action tempIDInfo, RoleType roleType)
	{
		StartNewAction(tempIDInfo.actions, roleType);
	}

	private IEnumerator StartRoleIdleAction(IDInfo idleAction, RoleType roleType)
	{
		isIdleFinishArray[(int)roleType] = false;
		for (int i = 0; i < idleAction.actions.Count; i++)
		{
			idleCurrStepArray[(int)roleType] = i + 1;
			StartNewAction(idleAction.actions[i], roleType);
			yield return new WaitUntil(() => isIdleStepFinishArray[(int)roleType]);
			DebugUtils.Log(DebugType.Other, "IdleStepFinish");
		}
		if (RoleManager.Instance.roleDictionary.ContainsKey(roleType))
		{
			RoleManager.Instance.roleDictionary[roleType].FinishIdleAction();
		}
	}

	public void StartNewAction(IDAction currAction, RoleType roleType)
	{
		idleStepFinishConditionArrayList[(int)roleType].Clear();
		isIdleStepFinishArray[(int)roleType] = false;
		if (currAction.audioConfig.isSet)
		{
			DealAudioAction(currAction, roleType);
		}
		if (currAction.roleConfig.isSet)
		{
			idleStepFinishConditionArrayList[(int)roleType].Add(false);
			IdleRoleAniManager.Instance.StartRoleAction(currAction.roleConfig, idleCurrStepArray[(int)roleType], roleType);
		}
		if (currAction.convConfig.isSet)
		{
			idleStepFinishConditionArrayList[(int)roleType].Add(false);
			IdleDialogManager.Instance.StartConversationAction(currAction.convConfig, idleCurrStepArray[(int)roleType], roleType);
		}
		if (currAction.delayConfig.isSet)
		{
			idleStepFinishConditionArrayList[(int)roleType].Add(false);
			StartCoroutine(WaitIdleFinish(currAction.delayConfig, idleCurrStepArray[(int)roleType], roleType));
		}
	}

	public void StopAllIdle()
	{
		StopAllCoroutines();
		IdleRoleAniManager.Instance.StopIdle();
		IdleDialogManager.Instance.StopIdle();
		IdleMusicManager.Instance.StopIdle();
	}

	private IEnumerator WaitIdleFinish(IDDelayConfig currConversationConfig, int currStep, RoleType roleType)
	{
		yield return new WaitForSeconds(currConversationConfig.delayTime);
		Debug.Log("IdleDelayFinish");
		FinishOneCondition(roleType, currStep);
	}

	private void DealAudioAction(IDAction targetAction, RoleType roleType)
	{
		if (targetAction.audioConfig.isMusicSet)
		{
			idleStepFinishConditionArrayList[(int)roleType].Add(false);
			MusicConfigData musicConfigData = new MusicConfigData();
			musicConfigData.MusicName = targetAction.audioConfig.musicName.ToString();
			musicConfigData.Loop = (targetAction.audioConfig.isMusicLoop ? 1 : 0);
			musicConfigData.Stop = (targetAction.audioConfig.isMusicStop ? 1 : 0);
			musicConfigData.LoopDelayTime = Random.Range(targetAction.audioConfig.musicMinTime, targetAction.audioConfig.musicMaxTime);
			musicConfigData.Fade = 0;
			IdleMusicManager.Instance.StartMusic(musicConfigData, idleCurrStepArray[(int)roleType], roleType);
		}
		else if (targetAction.audioConfig.isEffectSet)
		{
			idleStepFinishConditionArrayList[(int)roleType].Add(false);
			EffectConfigData effectConfigData = new EffectConfigData();
			effectConfigData.EffectName = targetAction.audioConfig.effectName.ToString();
			effectConfigData.Loop = (targetAction.audioConfig.isEffectLoop ? 1 : 0);
			effectConfigData.Stop = (targetAction.audioConfig.isEffectStop ? 1 : 0);
			effectConfigData.LoopDelayTime = Random.Range(targetAction.audioConfig.effectMinTime, targetAction.audioConfig.effectMaxTime);
			effectConfigData.Fade = 0;
			DebugUtils.Log(DebugType.Other, "PlotPlayAudio " + effectConfigData.EffectName);
			IdleMusicManager.Instance.StartEffect(effectConfigData, idleCurrStepArray[(int)roleType], roleType);
		}
	}

	public void FinishOneCondition(RoleType roleType, int plotStep)
	{
		if (plotStep != idleCurrStepArray[(int)roleType])
		{
			return;
		}
		DebugUtils.Log(DebugType.Plot, "FinishOneCondition");
		if (isIdleStepFinishArray[(int)roleType])
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			StartIdleAction(RoleType.Alice);
		}
	}
}
