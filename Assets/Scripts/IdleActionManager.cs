using System;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class IdleActionManager : MonoBehaviour
{
	[Serializable]
	public class ActionConfigDataList
	{
		public List<ActionConfigData> data = new List<ActionConfigData>();
	}

	[Serializable]
	public class ActionConfigData
	{
		public int ID;

		public string StartTaskID;

		public string TaskID;

		public int RoleType;

		public string StepAction;
	}

	[Serializable]
	public class StepConfigDataList
	{
		public List<StepConfigData> data = new List<StepConfigData>();
	}

	[Serializable]
	public class StepConfigData
	{
		public int ID;

		public string ActionStep;
	}

	[Serializable]
	public class WalkConfigDataList
	{
		public List<WalkConfigData> data = new List<WalkConfigData>();
	}

	[Serializable]
	public class WalkConfigData
	{
		public int Type;

		public string Position;
	}

	[Serializable]
	public class BubbleConfigDataList
	{
		public List<BubbleConfigData> data = new List<BubbleConfigData>();
	}

	[Serializable]
	public class BubbleConfigData
	{
		public int Type;

		public string Text;
	}

	[Serializable]
	public class AnimConfigDataList
	{
		public List<AnimConfigData> data = new List<AnimConfigData>();
	}

	[Serializable]
	public class AnimConfigData
	{
		public int ID;

		public string John;

		public string Alice;

		public string Arthur;

		public string Tina;

		public string Cat;
	}

	private static IdleActionManager instance;

	private List<ActionConfigData> actionConfig;

	private List<StepConfigData> stepConfig;

	private List<WalkConfigData> walkConfig;

	private List<BubbleConfigData> bubbleConfig;

	private List<AnimConfigData> animConfig;

	public static IdleActionManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/IdleAction/ActionConfig") as TextAsset;
		if (textAsset != null)
		{
			ActionConfigDataList actionConfigDataList = JsonUtility.FromJson<ActionConfigDataList>(textAsset.text);
			actionConfig = actionConfigDataList.data;
		}
		textAsset = Resources.Load("Config/IdleAction/StepConfig") as TextAsset;
		if (textAsset != null)
		{
			StepConfigDataList stepConfigDataList = JsonUtility.FromJson<StepConfigDataList>(textAsset.text);
			stepConfig = stepConfigDataList.data;
		}
		textAsset = Resources.Load("Config/IdleAction/WalkConfig") as TextAsset;
		if (textAsset != null)
		{
			WalkConfigDataList walkConfigDataList = JsonUtility.FromJson<WalkConfigDataList>(textAsset.text);
			walkConfig = walkConfigDataList.data;
		}
		textAsset = Resources.Load("Config/IdleAction/BubbleConfig") as TextAsset;
		if (textAsset != null)
		{
			BubbleConfigDataList bubbleConfigDataList = JsonUtility.FromJson<BubbleConfigDataList>(textAsset.text);
			bubbleConfig = bubbleConfigDataList.data;
		}
		textAsset = Resources.Load("Config/IdleAction/AnimConfig") as TextAsset;
		if (textAsset != null)
		{
			AnimConfigDataList animConfigDataList = JsonUtility.FromJson<AnimConfigDataList>(textAsset.text);
			animConfig = animConfigDataList.data;
		}
		RemoveSpecialChar();
	}

	public string GetUnlockStepId(RoleType roleType)
	{
		string text = "";
		for (int i = 0; i < actionConfig.Count; i++)
		{
			if (JudgeRightTask(actionConfig[i]) && actionConfig[i].RoleType == (int)roleType)
			{
				text = text + actionConfig[i].StepAction + ",";
			}
		}
		return text;
	}

	private bool JudgeRightTask(ActionConfigData action)
	{
		int num = int.Parse(action.StartTaskID.Split('-')[0]);
		int num2 = int.Parse(action.StartTaskID.Split('-')[1]);
		int num3 = int.Parse(action.TaskID.Split('-')[0]);
		int num4 = int.Parse(action.TaskID.Split('-')[1]);
		if (num == num3)
		{
			if (num2 <= UserDataManager.Instance.GetService().LastFinishTaskID && num4 >= UserDataManager.Instance.GetService().LastFinishTaskID && UserDataManager.Instance.GetService().LastFinishTaskStage == num)
			{
				return true;
			}
			return false;
		}
		if (num2 <= UserDataManager.Instance.GetService().LastFinishTaskID && UserDataManager.Instance.GetService().LastFinishTaskStage == num)
		{
			return true;
		}
		if (num4 >= UserDataManager.Instance.GetService().LastFinishTaskID && UserDataManager.Instance.GetService().LastFinishTaskStage == num3)
		{
			return true;
		}
		return false;
	}

	public string GetWalkPosition(int walkType)
	{
		string text = "";
		for (int i = 0; i < walkConfig.Count; i++)
		{
			if (walkConfig[i].Type == walkType)
			{
				text = text + walkConfig[i].Position + ";";
			}
		}
		return text;
	}

	public string GetBubbleText(int bubbleType)
	{
		string text = "";
		for (int i = 0; i < bubbleConfig.Count; i++)
		{
			if (bubbleConfig[i].Type == bubbleType)
			{
				text = text + bubbleConfig[i].Text + ";";
			}
		}
		return text;
	}

	public string GetAnimName(string animType)
	{
		string result = "";
		AnimConfigData animConfigData = null;
		int num = int.Parse(animType.Substring(1));
		for (int i = 0; i < animConfig.Count; i++)
		{
			if (animConfig[i].ID == num)
			{
				animConfigData = animConfig[i];
			}
		}
		if (animConfigData.Alice != "" && animType.Substring(0, 1) == "A")
		{
			result = animConfigData.Alice;
		}
		if (animConfigData.John != "" && animType.Substring(0, 1) == "B")
		{
			result = animConfigData.John;
		}
		if (animConfigData.Arthur != "" && animType.Substring(0, 1) == "K")
		{
			result = animConfigData.Arthur;
		}
		if (animConfigData.Tina != "" && animType.Substring(0, 1) == "M")
		{
			result = animConfigData.Tina;
		}
		if (animConfigData.Cat != "" && animType.Substring(0, 1) == "C")
		{
			result = animConfigData.Cat;
		}
		return result;
	}

	public string GetActionStepById(int stepID)
	{
		for (int i = 0; i < stepConfig.Count; i++)
		{
			if (stepConfig[i].ID == stepID)
			{
				return stepConfig[i].ActionStep;
			}
		}
		return "";
	}

	private void RemoveSpecialChar()
	{
		for (int i = 0; i < stepConfig.Count; i++)
		{
			if (stepConfig[i].ActionStep != "")
			{
				string[] array = stepConfig[i].ActionStep.Split('\n');
				string text = "";
				for (int j = 0; j < array.Length; j++)
				{
					text += array[j];
				}
				stepConfig[i].ActionStep = text;
			}
		}
	}
}
