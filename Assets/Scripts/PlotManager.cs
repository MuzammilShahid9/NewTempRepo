using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
	[Serializable]
	public class PlotConfigDataList
	{
		public List<PlotConfigData> data = new List<PlotConfigData>();
	}

	[Serializable]
	public class PlotConfigData
	{
		public string Step;

		public int ID;

		public string IsAuto;

		public string StartCondition;

		public string UnlockItemID;
	}

	private static PlotManager instance;

	private List<PlotConfigData> plotConfig;

	private List<bool> stepFinishCondition = new List<bool>();

	public bool isStepFinished;

	public bool isPlotFinish = true;

	public bool delayTimeFinish;

	public bool isSkip;

	public CDAction currPlotAction;

	public Task currTaskPlot;

	private PlotConfigData currPLotData;

	private int currPlotID;

	private int currTaskID;

	private int currTaskStepID;

	private bool hideAllBtn;

	private List<string> allStep = new List<string>();

	private List<ItemIndex> unlockItemID = new List<ItemIndex>();

	private List<int> unlockRoomID = new List<int>();

	public int currStep;

	public static PlotManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Plot/PlotConfig") as TextAsset;
		if (textAsset != null)
		{
			PlotConfigDataList plotConfigDataList = JsonUtility.FromJson<PlotConfigDataList>(textAsset.text);
			plotConfig = plotConfigDataList.data;
			RemoveSpecialChar();
		}
		isPlotFinish = true;
	}

	private void Start()
	{
		isStepFinished = true;
		isPlotFinish = true;
		delayTimeFinish = true;
		if (UserDataManager.Instance.GetService().firstEnterGame)
		{
			UserDataManager.Instance.GetService().FirstDownloadVersion = Application.version;
			UserDataManager.Instance.GetService().firstEnterGame = false;
			UserDataManager.Instance.Save();
		}
		if ((!TestConfig.active || !TestConfig.cinemaMode) && UserDataManager.Instance.GetService().tutorialProgress < 2)
		{
			StartCoroutine(StartFirstEnterGamePlot());
		}
	}

	private void RemoveSpecialChar()
	{
		for (int i = 0; i < plotConfig.Count; i++)
		{
			if (plotConfig[i].Step != "")
			{
				string[] array = plotConfig[i].Step.Split('\n');
				string text = "";
				for (int j = 0; j < array.Length; j++)
				{
					text += array[j];
				}
				plotConfig[i].Step = text;
			}
		}
	}

	public void DelayStartPlot(int taskID, int taskStep, float delayTime, bool isHideBtn = false)
	{
		isPlotFinish = false;
		hideAllBtn = isHideBtn;
		currTaskID = taskID;
		currTaskStepID = taskStep;
		StartCoroutine(DelayStartPlotIEnumerator(taskID, taskStep, delayTime));
	}

	private IEnumerator DelayStartPlotIEnumerator(int taskID, int taskStep, float delayTime)
	{
		IdleManager.Instance.StopAllIdle();
		yield return new WaitForSeconds(delayTime);
		RoleManager.Instance.RoleUnderPlotControl();
		Task actionWithTaskId = StageManage.Instance.GetActionWithTaskId(taskID, taskStep);
		StartNewPlot(actionWithTaskId);
	}

	public void StartNewPlotWithTaskId(int taskID, int taskStep)
	{
		IdleManager.Instance.StopAllIdle();
		RoleManager.Instance.RoleUnderPlotControl();
		currTaskID = taskID;
		currTaskStepID = taskStep;
		Task actionWithTaskId = StageManage.Instance.GetActionWithTaskId(taskID, taskStep);
		StartNewPlot(actionWithTaskId);
	}

	public void StartPlot(int plotID)
	{
		currPlotID = plotID;
		DebugUtils.Log(DebugType.Plot, "###startPlot " + plotID);
		for (int i = 0; i < plotConfig.Count; i++)
		{
			if (plotConfig[i].ID == plotID)
			{
				currPLotData = plotConfig[i];
			}
		}
		string[] array = currPLotData.Step.Split(';');
		allStep.Clear();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text != "")
			{
				allStep.Add(text.Trim());
			}
		}
		isStepFinished = false;
		if (allStep.Count > 0)
		{
			PlotCameraManager.Instance.LockCamera();
			RoleManager.Instance.RoleUnderPlotControl();
			CastleSceneUIManager.Instance.HideAllBtn();
			DealUnlockItemID();
		}
		StartCoroutine(ProcessPlot());
	}

	public void StartNewPlot(Task currTask)
	{
		isSkip = false;
		StopAllCoroutines();
		currTaskPlot = currTask;
		DealNewUnlockItemID();
		DealNewUnlockRoomID();
		PlotCameraManager.Instance.LockCamera();
		if (!TaskManager.Instance.JudgeAutoPlotInclude(currTaskID))
		{
			CastleSceneUIManager.Instance.HideAllBtn();
		}
		if (hideAllBtn)
		{
			CastleSceneUIManager.Instance.HideAllBtn();
		}
		StartCoroutine(ProcessNewPlot());
	}

	public void DealSingleAction(CDAction currAction)
	{
		StartCoroutine(ProcessSingleAction(currAction));
	}

	private IEnumerator ProcessSingleAction(CDAction currAction)
	{
		isPlotFinish = false;
		currStep = 1;
		StartNewAction(currAction);
		yield return new WaitUntil(() => isStepFinished);
		DealNewPlotFinish();
	}

	public void StartNewAction(CDAction currAction)
	{
		currPlotAction = currAction;
		stepFinishCondition.Clear();
		isStepFinished = false;
		if (currAction.buildConfig.isSet)
		{
			stepFinishCondition.Add(false);
			PlotItemAniManager.Instance.StartItemAnimAction(currAction.buildConfig, currStep);
		}
		else
		{
			if (currAction.camConfig.isSet)
			{
				stepFinishCondition.Add(false);
				PlotCameraManager.Instance.StartCameraAction(currAction.camConfig, currStep);
			}
			if (currAction.roleConfig.isSet)
			{
				stepFinishCondition.Add(false);
				PlotRoleAniManager.Instance.StartRoleAction(currAction.roleConfig, currStep);
			}
			if (currAction.convConfig.isSet)
			{
				stepFinishCondition.Add(false);
				PlotDialogManager.Instance.StartConversationAction(currAction.convConfig, currStep);
			}
			if (currAction.otherConfig.isSet)
			{
				DealOtherAction(currAction);
			}
		}
		if (currAction.audioConfig.isSet)
		{
			DealAudioAction(currAction);
		}
		if (!currAction.buildConfig.isSet && !currAction.camConfig.isSet && !currAction.roleConfig.isSet && !currAction.convConfig.isSet && !currAction.otherConfig.isSet && !currAction.audioConfig.isSet)
		{
			isStepFinished = true;
		}
		else
		{
			StartCoroutine(StartActionWithTime(currAction));
		}
	}

	public bool JudgeNextStepHaveDialog()
	{
		if (currTaskPlot != null && currStep < currTaskPlot.actions.Count)
		{
			CDAction cDAction = currTaskPlot.actions[currStep];
			if (cDAction.convConfig.isSet)
			{
				DebugUtils.Log(DebugType.Other, string.Concat("nextAction ", cDAction, " have dialog"));
				return true;
			}
			DebugUtils.Log(DebugType.Other, string.Concat("nextAction ", cDAction, " have no dialog"));
			return false;
		}
		return false;
	}

	private void DealAudioAction(CDAction targetAction)
	{
		if (targetAction.audioConfig.isMusicSet)
		{
			stepFinishCondition.Add(false);
			MusicConfigData musicConfigData = new MusicConfigData();
			musicConfigData.MusicName = targetAction.audioConfig.musicName.ToString();
			musicConfigData.Loop = (targetAction.audioConfig.isMusicLoop ? 1 : 0);
			musicConfigData.Stop = (targetAction.audioConfig.isMusicStop ? 1 : 0);
			musicConfigData.LoopDelayTime = UnityEngine.Random.Range(targetAction.audioConfig.musicMinTime, targetAction.audioConfig.musicMaxTime);
			musicConfigData.Fade = 0;
			PlotMusicManager.Instance.StartMusic(musicConfigData, currStep);
		}
		else if (targetAction.audioConfig.isEffectSet)
		{
			stepFinishCondition.Add(false);
			EffectConfigData effectConfigData = new EffectConfigData();
			effectConfigData.EffectName = targetAction.audioConfig.effectName.ToString();
			effectConfigData.Loop = (targetAction.audioConfig.isEffectLoop ? 1 : 0);
			effectConfigData.Stop = (targetAction.audioConfig.isEffectStop ? 1 : 0);
			effectConfigData.LoopDelayTime = UnityEngine.Random.Range(targetAction.audioConfig.effectMinTime, targetAction.audioConfig.effectMaxTime);
			effectConfigData.Fade = 0;
			DebugUtils.Log(DebugType.Other, "PlotPlayAudio " + effectConfigData.EffectName);
			PlotMusicManager.Instance.StartEffect(effectConfigData, currStep);
		}
	}

	private void DealOtherAction(CDAction targetAction)
	{
		if (targetAction.otherConfig.isBlackScreen)
		{
			stepFinishCondition.Add(false);
			PlotBlankScreenManager.Instance.StartBlankScreen(currStep);
		}
		else if (targetAction.otherConfig.isCastleRename)
		{
			stepFinishCondition.Add(false);
			PlotRenameManager.Instance.StartRename(0, currStep);
		}
		else if (targetAction.otherConfig.isChapterEnd)
		{
			stepFinishCondition.Add(false);
			PlotFinishStageManager.Instance.StartFinishStage(currStep);
		}
		else if (targetAction.otherConfig.isCatRename)
		{
			stepFinishCondition.Add(false);
			PlotRenameManager.Instance.StartRename(1, currStep);
		}
		else if (targetAction.otherConfig.unlockRoomID != 0)
		{
			stepFinishCondition.Add(false);
			PlotRoomUnlockManager.Instance.StartUnlock(targetAction.otherConfig.unlockRoomID, currStep);
		}
	}

	private bool JudgeIsSelectItem(CDAction currAction)
	{
		bool result = false;
		if (currAction.buildConfig.isSet)
		{
			ItemAnim item = CastleManager.Instance.GetItem(currAction.buildConfig.roomID, currAction.buildConfig.itemID);
			if (item != null)
			{
				result = item.JudgeIsSelectItem();
			}
		}
		return result;
	}

	private IEnumerator StartActionWithTime(CDAction currAction)
	{
		yield return null;
		if (!currAction.convConfig.isSet && !currAction.buildConfig.isSet && currAction.tm > -1f)
		{
			delayTimeFinish = false;
			yield return new WaitForSeconds(currAction.tm);
			if (currPlotAction == currAction)
			{
				delayTimeFinish = true;
				isStepFinished = true;
			}
		}
	}

	public void PlotInsertRoleAction()
	{
		if (currPlotAction.roleConfig.isSet)
		{
			stepFinishCondition.Add(false);
			PlotRoleAniManager.Instance.StartRoleAction(currPlotAction.roleConfig, currStep);
		}
	}

	public PlotConfigData LoadPlot(int plotID)
	{
		currPlotID = plotID;
		DebugUtils.Log(DebugType.Plot, "load lot " + plotID);
		for (int i = 0; i < plotConfig.Count; i++)
		{
			if (plotConfig[i].ID == plotID)
			{
				return plotConfig[i];
			}
		}
		return null;
	}

	public ItemIndex GetPlotUnlockItemInfo(int plotID)
	{
		ItemIndex result = null;
		PlotConfigData plotConfigData = null;
		for (int i = 0; i < plotConfig.Count; i++)
		{
			if (plotConfig[i].ID == plotID)
			{
				plotConfigData = plotConfig[i];
			}
		}
		string[] array = plotConfigData.Step.Split(';');
		List<string> list = new List<string>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text != "")
			{
				list.Add(text);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			string[] array3 = list[k].Split(',');
			for (int l = 0; l < array3.Length; l++)
			{
				if (array3[l] != "" && array3[l].Substring(0, 1) == "I")
				{
					result = PlotItemAniManager.Instance.GetAnimItemInfo(array3[l].Substring(1));
				}
			}
		}
		return result;
	}

	public void DealUnlockItemID()
	{
		unlockItemID.Clear();
		for (int i = 0; i < allStep.Count; i++)
		{
			string[] array = allStep[i].Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != "" && array[j].Substring(0, 1) == "I")
				{
					unlockItemID.Add(PlotItemAniManager.Instance.GetAnimItemInfo(array[j].Substring(1)));
				}
			}
		}
	}

	public void DealNewUnlockItemID()
	{
		unlockItemID.Clear();
		for (int i = 0; i < currTaskPlot.actions.Count; i++)
		{
			if (currTaskPlot.actions[i].buildConfig.isSet)
			{
				ItemIndex itemIndex = new ItemIndex();
				itemIndex.roomIndex = currTaskPlot.actions[i].buildConfig.roomID;
				itemIndex.itemIndex = currTaskPlot.actions[i].buildConfig.itemID;
				unlockItemID.Add(itemIndex);
			}
		}
	}

	public void DealNewUnlockRoomID()
	{
		unlockRoomID.Clear();
		for (int i = 0; i < currTaskPlot.actions.Count; i++)
		{
			if (currTaskPlot.actions[i].otherConfig.isSet && currTaskPlot.actions[i].otherConfig.unlockRoomID != 0)
			{
				unlockRoomID.Add(currTaskPlot.actions[i].otherConfig.unlockRoomID);
			}
		}
	}

	public void DealPlot(int Step)
	{
		if (Step > allStep.Count)
		{
			return;
		}
		stepFinishCondition.Clear();
		isStepFinished = false;
		currStep = Step;
		string[] array = allStep[currStep - 1].Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			DebugUtils.Log(DebugType.Plot, array[i] + " " + currStep);
			if (array[i] == "")
			{
				break;
			}
			if (array[i].Substring(1) == "")
			{
				isStepFinished = true;
				break;
			}
			if (array[i].Substring(0, 1) == "C")
			{
				stepFinishCondition.Add(false);
				PlotCameraManager.Instance.StartCamera(array[i].Substring(1));
			}
			else if (array[i].Substring(0, 1) == "R")
			{
				stepFinishCondition.Add(false);
				PlotRoleMoveManager.Instance.StartRoleMove(array[i].Substring(1));
			}
			else if (array[i].Substring(0, 1) == "T")
			{
				stepFinishCondition.Add(false);
				PlotDialogManager.Instance.StartDialog(array[i].Substring(1));
			}
			else if (array[i].Substring(0, 1) == "D")
			{
				stepFinishCondition.Add(false);
				StartCoroutine(DelayAction(Convert.ToSingle(array[i].Substring(1))));
			}
			else if (array[i].Substring(0, 1) == "A")
			{
				stepFinishCondition.Add(false);
				PlotRoleAniManager.Instance.StartRoleAni(array[i].Substring(1));
			}
			else if (array[i].Substring(0, 1) == "S")
			{
				stepFinishCondition.Add(false);
			}
			else
			{
				if (array[i].Substring(0, 1) == "M" || array[i].Substring(0, 1) == "E")
				{
					continue;
				}
				if (array[i].Substring(0, 1) == "I")
				{
					stepFinishCondition.Add(false);
					PlotItemAniManager.Instance.StartItemAni(array[i].Substring(1));
				}
				else if (array[i].Substring(0, 1) == "L")
				{
					stepFinishCondition.Add(false);
					stepFinishCondition.Add(false);
					string roleMoveType = PlotRoleMoveManager.Instance.GetRoleMoveType(array[i].Substring(2), array[i].Substring(1, 1));
					if (roleMoveType == "F")
					{
						PlotRoleMoveManager.Instance.StartRoleMove(array[i].Substring(2));
						PlotCameraManager.Instance.StartCamera(array[i].Substring(2), true, array[i].Substring(1, 1));
						PlotCameraManager.Instance.LockToRole(array[i].Substring(1, 1));
					}
					else if (roleMoveType == "W")
					{
						PlotCameraManager.Instance.StartCamera(array[i].Substring(2), true, array[i].Substring(1, 1));
						PlotRoleMoveManager.Instance.StartRoleMove(array[i].Substring(2), true);
						PlotCameraManager.Instance.LockToRole(array[i].Substring(1, 1));
					}
				}
				else if (array[i].Substring(0, 1) == "F")
				{
					stepFinishCondition.Add(false);
				}
				else if (array[i].Substring(0, 1) == "N")
				{
					stepFinishCondition.Add(false);
				}
			}
		}
	}

	public void FinishOneCondition(int plotStep = -1)
	{
		if (plotStep != -1 && (plotStep == -1 || plotStep != currStep))
		{
			return;
		}
		DebugUtils.Log(DebugType.Plot, "FinishOneCondition");
		DebugUtils.Log(DebugType.Other, "FinishOneCondition");
		if (isStepFinished)
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

	public void SkipNewStep()
	{
		DebugUtils.Log(DebugType.Plot, "SkipStep" + currStep);
		DebugUtils.Log(DebugType.Other, "SkipStep" + currStep);
		isSkip = true;
		int unSkipStep = GetUnSkipStep();
		DebugUtils.Log(DebugType.Other, "nextStep" + unSkipStep);
		if (unSkipStep != -1)
		{
			DealUnFinishstep();
			if (unSkipStep > 0)
			{
				GetLeftStep(unSkipStep);
			}
			currStep = 0;
			StopAllCoroutines();
			RestoreAllManager();
			RoleManager.Instance.RoleUnderPlotControl();
			StartCoroutine(ProcessNewPlot());
		}
		else
		{
			StopAllCoroutines();
			DealUnFinishstep();
			DealNewPlotFinish();
			currStep = 0;
		}
	}

	public void SendSkipBtnClickAnalytic()
	{
		if (UserDataManager.Instance.GetCoin() <= 5000000)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("IsSkip", isSkip.ToString());
			Analytics.Event("Skip_" + UserDataManager.Instance.GetService().stage.ToString() + "_" + currTaskID, dictionary);
			dictionary.Clear();
		}
	}

	private void RestoreAllManager()
	{
		PlotItemAniManager.Instance.RestortPlotStep();
		PlotCameraManager.Instance.RestortPlotStep();
		PlotRoleAniManager.Instance.RestortPlotStep();
		PlotDialogManager.Instance.RestortPlotStep();
		PlotMusicManager.Instance.RestortPlotStep();
		PlotBlankScreenManager.Instance.RestortPlotStep();
		PlotRenameManager.Instance.RestortPlotStep();
		PlotFinishStageManager.Instance.RestortPlotStep();
	}

	public void SkipStep()
	{
		DebugUtils.Log(DebugType.Plot, "SkipStep" + currStep);
		int unSkipStep = GetUnSkipStep();
		if (unSkipStep != -1)
		{
			DealUnFinishstep();
			if (unSkipStep > 0)
			{
				GetLeftStep(unSkipStep);
			}
			currStep = 0;
			StopAllCoroutines();
			StartCoroutine(ProcessPlot());
		}
		else
		{
			DealUnFinishstep();
			DealPlotFinish();
			currStep = 0;
			StopAllCoroutines();
		}
	}

	private int GetUnSkipStep()
	{
		for (int i = currStep; i < currTaskPlot.actions.Count; i++)
		{
			bool flag = false;
			if (currTaskPlot.actions[i].convConfig.isSet && !JudgeActionIncludeRoleMove(currTaskPlot.actions[i]))
			{
				flag = true;
			}
			if (!flag)
			{
				return i;
			}
		}
		return -1;
	}

	private bool JudgeActionIncludeRoleMove(CDAction action)
	{
		if (action.roleConfig.isSet)
		{
			foreach (RoleType key in action.roleConfig.roles.Keys)
			{
				string[] array = action.roleConfig.roles[key].anim.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].IndexOf("(") > 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	private void GetLeftStep(int step)
	{
		List<CDAction> list = new List<CDAction>();
		for (int i = step; i < currTaskPlot.actions.Count; i++)
		{
			list.Add(currTaskPlot.actions[i]);
		}
		currTaskPlot.actions = list;
	}

	public void CancelAllStep()
	{
		PlotCameraManager.Instance.UnlockToRole();
		PlotCameraManager.Instance.UnlockCamera();
		DealUnFinishstep();
		allStep.Clear();
		StopAllCoroutines();
		currStep = 0;
		isStepFinished = true;
		isPlotFinish = true;
	}

	public void SkipOneStep()
	{
		DebugUtils.Log(DebugType.Plot, "skipStep CurrentStep" + currStep);
		string[] array = allStep[currStep - 1].Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			DebugUtils.Log(DebugType.Plot, array[i]);
			if (array[i].Substring(0, 1) == "C")
			{
				PlotCameraManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "R")
			{
				PlotRoleMoveManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "T")
			{
				PlotDialogManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "D")
			{
				StopAllCoroutines();
			}
			else if (array[i].Substring(0, 1) == "A")
			{
				PlotRoleAniManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "S")
			{
				PlotBlankScreenManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "M")
			{
				PlotMusicManager.Instance.StopStep();
			}
			else if (array[i].Substring(0, 1) == "I")
			{
				PlotItemAniManager.Instance.StopStep();
			}
		}
		isStepFinished = true;
	}

	public void DealUnFinishstep()
	{
		if (currStep >= 1)
		{
			stepFinishCondition.Clear();
			DebugUtils.Log(DebugType.Plot, "skipAllStep CurrentStep" + currStep);
			CDAction cDAction = currTaskPlot.actions[currStep - 1];
			if (cDAction.camConfig.isSet)
			{
				PlotCameraManager.Instance.StopStep();
			}
			if (cDAction.roleConfig.isSet)
			{
				PlotRoleAniManager.Instance.StopStep();
			}
			if (cDAction.convConfig.isSet)
			{
				PlotDialogManager.Instance.StopStep();
			}
			if (cDAction.buildConfig.isSet)
			{
				PlotItemAniManager.Instance.StopStep();
			}
		}
	}

	public void DealPlotFinish()
	{
		currStep = 0;
		isPlotFinish = true;
		TaskManager.Instance.DealPlotFinish(currPlotID);
		PlotCameraManager.Instance.UnlockCamera();
		RoleManager.Instance.RoleReleasePlotControl();
		PlotCameraManager.Instance.UnlockToRole();
		PlotBlankScreenManager.Instance.StopStep();
		UnlockItem();
		UnlockRole();
		DebugUtils.Log(DebugType.Plot, "PlotFinish");
		DealSpecialPlot();
	}

	public void DealNewPlotFinish()
	{
		DebugUtils.Log(DebugType.Other, "newPlotFinish");
		currStep = 0;
		isPlotFinish = true;
		if (TaskManager.Instance.currDoTask != null)
		{
			UserDataManager.Instance.GetService().scrollNum -= TaskManager.Instance.currDoTask.CostScrollNum;
			UserDataManager.Instance.Save();
		}
		if (TaskManager.Instance.JudgeAutoPlotInclude(currTaskID))
		{
			TaskManager.Instance.DealAutoTaskFinish(currTaskID);
		}
		else
		{
			TaskManager.Instance.DealNewPlotFinish(currTaskID, currTaskStepID);
		}
		SendSkipBtnClickAnalytic();
		PlotCameraManager.Instance.UnlockCamera();
		RoleManager.Instance.RoleReleasePlotControl();
		RoleManager.Instance.UpdateRoleDisplay();
		CastleSceneUIManager.Instance.gameObject.SetActive(true);
		CameraControl.Instance.UnlockToRole();
		UnlockItem();
		UnlockRole();
		UnlockRoom();
		if (TestConfig.active && TestConfig.cinemaMode)
		{
			CastleSceneUIManager.Instance.ShowAllBtn();
		}
		else
		{
			DealSpecialPlot();
		}
	}

	private void UnlockRole()
	{
		List<int> unlockRoleInfo = RoleManager.Instance.GetUnlockRoleInfo(currTaskID);
		for (int i = 0; i < unlockRoleInfo.Count; i++)
		{
			if (unlockRoleInfo[i] != -1 && !UserDataManager.Instance.GetService().UnlockRoleIDList.Contains(unlockRoleInfo[i]))
			{
				UserDataManager.Instance.GetService().UnlockRoleIDList.Add(unlockRoleInfo[i]);
			}
		}
	}

	private void UnlockItem()
	{
		if (TestConfig.active && TestConfig.cinemaMode)
		{
			return;
		}
		if (unlockItemID.Count > 0)
		{
			for (int i = 0; i < unlockItemID.Count; i++)
			{
				if (unlockItemID[i] != null)
				{
					ItemAnim item = CastleManager.Instance.GetItem(unlockItemID[i].roomIndex, unlockItemID[i].itemIndex);
					if (item != null && item.selectImage == -1)
					{
						item.selectImage = 0;
					}
					if (item != null)
					{
						UserDataManager.Instance.AddUnlockItemInfo(item.roomID, item.itemID, item.selectImage);
					}
				}
			}
		}
		UserDataManager.Instance.Save();
	}

	private void UnlockRoom()
	{
		if (TestConfig.active && TestConfig.cinemaMode)
		{
			return;
		}
		if (unlockRoomID.Count > 0)
		{
			for (int i = 0; i < unlockRoomID.Count; i++)
			{
				UserDataManager.Instance.AddUnlockRoomInfo(unlockRoomID[i]);
			}
		}
		UserDataManager.Instance.Save();
	}

	public void DealSpecialPlot()
	{
		if (currTaskID == 0 && UserDataManager.Instance.GetService().stage == 1)
		{
			UserDataManager.Instance.GetService().tutorialProgress = 2;
			UserDataManager.Instance.Save();
			SceneTransManager.Instance.ChangeSceneWithEffect(delegate
			{
				SceneTransManager.Instance.TransToSwitch(SceneType.GameScene);
				CastleSceneUIManager.Instance.ShowAllBtn();
			});
		}
		else if (currTaskID == 1 && UserDataManager.Instance.GetService().stage == 1)
		{
			UserDataManager.Instance.GetService().tutorialProgress = 6;
			UserDataManager.Instance.Save();
			CastleSceneUIManager.Instance.ShowUpButton();
			CastleSceneUIManager.Instance.TaskBtnClick();
		}
		else if (currTaskID == GeneralConfig.ChangeItemTutorialStartPlot && UserDataManager.Instance.GetService().stage == 1)
		{
			UserDataManager.Instance.GetService().tutorialProgress = 7;
			UserDataManager.Instance.Save();
			TutorialManager.Instance.ShowTutorial();
		}
		else if (TaskManager.Instance.currAutoTaskID == -1)
		{
			CastleSceneUIManager.Instance.ShowAllBtn();
		}
	}

	private void OnDestroy()
	{
	}

	private IEnumerator StartFirstEnterGamePlot()
	{
		yield return null;
		StartNewPlotWithTaskId(0, -1);
	}

	private IEnumerator ProcessPlot()
	{
		isPlotFinish = false;
		for (int i = 1; i <= allStep.Count; i++)
		{
			DealPlot(i);
			yield return new WaitUntil(() => isStepFinished);
		}
		DealPlotFinish();
	}

	private IEnumerator ProcessNewPlot()
	{
		isPlotFinish = false;
		for (int i = 0; i < currTaskPlot.actions.Count; i++)
		{
			currStep = i + 1;
			StartNewAction(currTaskPlot.actions[i]);
			yield return new WaitUntil(() => isStepFinished && delayTimeFinish);
			CameraControl.Instance.UnlockToRole();
			DebugUtils.Log(DebugType.Other, "PlotStepFinish");
		}
		DealNewPlotFinish();
	}

	private IEnumerator DelayAction(float time)
	{
		yield return new WaitForSeconds(time);
		FinishOneCondition();
	}
}
