using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class PlotDialogManager : MonoBehaviour
{
	[Serializable]
	public class DialogConfigDataList
	{
		public List<DialogConfigData> data = new List<DialogConfigData>();
	}

	[Serializable]
	public class DialogConfigData
	{
		public string Key;

		public int LeftOrRight;

		public RoleType roleType;

		public string RoleName;

		public string Image;
	}

	private static PlotDialogManager instance;

	private List<DialogConfigData> dialogConfig;

	public DialogConfigData currDialogData;

	private int plotStep;

	private bool isStepFinished;

	private string[] conversationList;

	private int _conversationIndex;

	public static PlotDialogManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Plot/DialogConfig") as TextAsset;
		if (textAsset != null)
		{
			DialogConfigDataList dialogConfigDataList = JsonUtility.FromJson<DialogConfigDataList>(textAsset.text);
			dialogConfig = dialogConfigDataList.data;
		}
	}

	public void StartDialog(string dialogID)
	{
		for (int i = 0; i < dialogConfig.Count; i++)
		{
			if (dialogConfig[i].Key == dialogID)
			{
				currDialogData = dialogConfig[i];
			}
		}
		isStepFinished = false;
		StartCoroutine(ProcessDialog());
	}

	public void StartConversationAction(CDConversationConfig currConversationConfig, int currStep)
	{
		if (currConversationConfig.convList.Count > 0)
		{
			plotStep = currStep;
			_conversationIndex = 0;
			conversationList = currConversationConfig.convList.ToArray();
			RunNextConversation();
		}
	}

	public bool JudgeNextHaveDialog()
	{
		if (_conversationIndex < conversationList.Length)
		{
			return true;
		}
		return false;
	}

	public void RunNextConversation()
	{
		if (_conversationIndex >= conversationList.Length)
		{
			StartCoroutine(DelayPlotFinish());
			return;
		}
		string text = conversationList[_conversationIndex];
		DebugUtils.Log(DebugType.Other, "Run conversation " + text);
		if (text == "")
		{
			StartCoroutine(DelayPlotFinish());
			return;
		}
		string[] array = text.Split('|');
		currDialogData = new DialogConfigData();
		currDialogData.Key = array[0].Substring(1);
		if (array[2] == "Alice")
		{
			currDialogData.roleType = RoleType.Alice;
		}
		else if (array[2] == "John")
		{
			currDialogData.roleType = RoleType.John;
		}
		else if (array[2] == "Arthur")
		{
			currDialogData.roleType = RoleType.Arthur;
		}
		else if (array[2] == "Luna")
		{
			currDialogData.roleType = RoleType.Cat;
		}
		else if (array[2] == "Tina")
		{
			currDialogData.roleType = RoleType.Tina;
		}
		currDialogData.RoleName = LanguageConfig.GetString(array[1]);
		currDialogData.Image = array[3];
		if (array[4] == "Left")
		{
			currDialogData.LeftOrRight = 0;
		}
		else
		{
			currDialogData.LeftOrRight = 1;
		}
		StartCoroutine(DealDialog());
		_conversationIndex++;
	}

	private IEnumerator DelayPlotFinish()
	{
		yield return null;
		PlotManager.Instance.FinishOneCondition(plotStep);
	}

	public void RestortPlotStep()
	{
		plotStep = -2;
	}

	public DialogConfigData GetConversation(string dialogID)
	{
		for (int i = 0; i < dialogConfig.Count; i++)
		{
			if (dialogConfig[i].Key == dialogID)
			{
				return dialogConfig[i];
			}
		}
		return null;
	}

	private IEnumerator DealDialog()
	{
		yield return null;
		isStepFinished = false;
		if (currDialogData == null)
		{
			FinishStep();
			yield break;
		}
		Dialog temoDialog = new Dialog
		{
			content = currDialogData.Key
		};
		if (temoDialog.content == "")
		{
			FinishStep();
			yield break;
		}
		temoDialog.leftOrRight = currDialogData.LeftOrRight;
		temoDialog.roleName = currDialogData.RoleName;
		temoDialog.Image = currDialogData.Image;
		if (!CastleSceneUIManager.Instance.isPlotMaskShow)
		{
			CastleSceneUIManager.Instance.ShowMask();
			yield return new WaitForSeconds(0.2f);
		}
		DialogUIManager.Instance.ShowDialog(temoDialog);
		CastleSceneUIManager.Instance.ShowPlotBubble();
		AudioManager.Instance.PlayAudioEffect("main_bubble");
	}

	public void FinishStep()
	{
		CastleSceneUIManager.Instance.ClearBubble();
		RunNextConversation();
	}

	public void StopStep()
	{
		conversationList = null;
		currDialogData = null;
		StopAllCoroutines();
		CastleSceneUIManager.Instance.ClearBubble();
		DialogUIManager.Instance.ForceHide();
	}

	private IEnumerator ProcessDialog()
	{
		yield return new WaitUntil(() => isStepFinished);
		FinishStep();
	}
}
