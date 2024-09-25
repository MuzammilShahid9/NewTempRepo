using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	private static TaskManager instance;

	public TaskConfigData currDoTask;

	public float taskProgress;

	public TaskPanelManager taskPanelManager;

	[HideInInspector]
	public List<TaskConfigData> showTaskList = new List<TaskConfigData>();

	private List<TaskConfigData> autoTaskConfig = new List<TaskConfigData>();

	private List<TaskConfigData> taskConfig;

	private int stageAllStep;

	private float stageCurrStep;

	public int currAutoTaskID;

	public Dictionary<int, TaskConfigData> currStageTask = new Dictionary<int, TaskConfigData>();

	public List<int> finishTaskIDList = new List<int>();

	private List<int> showTaskIDList = new List<int>();

	private List<int> unfinishTaskIDList = new List<int>();

	public static TaskManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		if (TestConfig.active && TestConfig.startTask > 0)
		{
			UserDataManager.Instance.GetService().stage = TestConfig.startStage;
		}
		if (TestConfig.active)
		{
			UserDataManager.Instance.GetService().scrollNum = TestConfig.scrollNum;
		}
		stageAllStep = 0;
		UserDataManager.Instance.Save();
		LoadStageTask();
		if (TestConfig.active && TestConfig.startTask > 0)
		{
			UserDataManager.Instance.GetService().finishTaskString = "";
			AddStartCondition(TestConfig.startTask);
		}
		string finishTaskString = UserDataManager.Instance.GetService().finishTaskString;
		UserDataManager.Instance.Save();
	}

	private void DealOldTaskData()
	{
		new List<ItemUnlockInfo>();
		for (int i = 1; i < UserDataManager.Instance.GetService().stage; i++)
		{
			List<ItemIndex> itemWithStageId = StageManage.Instance.GetItemWithStageId(i);
			for (int j = 0; j < itemWithStageId.Count; j++)
			{
				int itemUnlockInfo = UserDataManager.Instance.GetItemUnlockInfo(itemWithStageId[j].roomIndex + 1, itemWithStageId[j].itemIndex + 1);
				if (itemUnlockInfo == -1)
				{
					UserDataManager.Instance.AddUnlockItemInfo(itemWithStageId[j].roomIndex + 1, itemWithStageId[j].itemIndex + 1, 0);
				}
				else
				{
					UserDataManager.Instance.AddUnlockItemInfo(itemWithStageId[j].roomIndex + 1, itemWithStageId[j].itemIndex + 1, itemUnlockInfo);
				}
			}
		}
		string[] array = UserDataManager.Instance.GetService().finishTaskString.Split(';');
		for (int k = 0; k < array.Length; k++)
		{
			if (!(array[k] != ""))
			{
				continue;
			}
			List<ItemIndex> unlockItemID = StageManage.Instance.GetUnlockItemID(UserDataManager.Instance.GetService().stage, int.Parse(array[k]));
			for (int l = 0; l < unlockItemID.Count; l++)
			{
				int itemUnlockInfo2 = UserDataManager.Instance.GetItemUnlockInfo(unlockItemID[l].roomIndex + 1, unlockItemID[l].itemIndex + 1);
				if (itemUnlockInfo2 == -1)
				{
					UserDataManager.Instance.AddUnlockItemInfo(unlockItemID[l].roomIndex + 1, unlockItemID[l].itemIndex + 1, 0);
				}
				else
				{
					UserDataManager.Instance.AddUnlockItemInfo(unlockItemID[l].roomIndex + 1, unlockItemID[l].itemIndex + 1, itemUnlockInfo2);
				}
			}
		}
		UserDataManager.Instance.Save();
	}

	private void LoadStageTask()
	{
		if (UserDataManager.Instance.GetService().stage <= GeneralConfig.MaxStage)
		{
			TaskConfigDataList taskConfigDataList = JsonUtility.FromJson<TaskConfigDataList>((Resources.Load("Config/Task/chapter" + UserDataManager.Instance.GetStage()) as TextAsset).text);
			taskConfig = taskConfigDataList.data;
			currStageTask.Clear();
			autoTaskConfig.Clear();
			stageAllStep = 0;
			for (int i = 0; i < taskConfig.Count; i++)
			{
				if (taskConfig[i].Stage == UserDataManager.Instance.GetStage())
				{
					currStageTask.Add(taskConfig[i].ID, taskConfig[i]);
					if (taskConfig[i].IsAuto == 0)
					{
						stageAllStep += taskConfig[i].FinishConditionNum;
					}
				}
			}
			for (int j = 0; j < taskConfig.Count; j++)
			{
				if (taskConfig[j].IsAuto == 1)
				{
					autoTaskConfig.Add(taskConfig[j]);
				}
			}
		}
		else
		{
			taskConfig = null;
			stageAllStep = 0;
			currStageTask.Clear();
			autoTaskConfig.Clear();
		}
	}

	private int GetTaskStageWithId(int taskID)
	{
		int result = -1;
		for (int i = 0; i < taskConfig.Count; i++)
		{
			if (taskConfig[i].ID == taskID)
			{
				return taskConfig[i].Stage;
			}
		}
		return result;
	}

	private void AddStartCondition(int index)
	{
		string startCondition = currStageTask[index].StartCondition;
		if (startCondition == null || !(startCondition != ""))
		{
			return;
		}
		string[] array = startCondition.Split(',');
		foreach (string text in array)
		{
			if (!JudgeIsHaveTask(text.Split('.')[0]))
			{
				UserData service = UserDataManager.Instance.GetService();
				service.finishTaskString = service.finishTaskString + text.Split('.')[0] + ";";
				AddStartCondition(int.Parse(text.Split('.')[0]));
			}
		}
	}

	private bool JudgeIsHaveTask(string taskIDstring)
	{
		string[] array = UserDataManager.Instance.GetService().finishTaskString.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == taskIDstring)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		DealOldTaskData();
		DealTaskData();
	}

	public TaskConfigData GetShowRoleData()
	{
		List<TaskConfigData> data = JsonUtility.FromJson<TaskConfigDataList>((Resources.Load("Config/Task/chapter" + UserDataManager.Instance.GetService().LastFinishTaskStage) as TextAsset).text).data;
		TaskConfigData result = new TaskConfigData();
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].ID == UserDataManager.Instance.GetService().LastFinishTaskID)
			{
				result = data[i];
			}
		}
		return result;
	}

	private void DealTaskData()
	{
		finishTaskIDList.Clear();
		unfinishTaskIDList.Clear();
		string[] array = UserDataManager.Instance.GetFinishTaskString().Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "" && int.Parse(array[i]) > UserDataManager.Instance.GetService().LastFinishTaskID)
			{
				UserDataManager.Instance.GetService().LastFinishTaskID = int.Parse(array[i]);
				UserDataManager.Instance.GetService().LastFinishTaskStage = UserDataManager.Instance.GetService().stage;
				UserDataManager.Instance.Save();
			}
			if (array[i] != "" && currStageTask.ContainsKey(int.Parse(array[i])))
			{
				finishTaskIDList.Add(int.Parse(array[i]));
			}
		}
		foreach (int key in currStageTask.Keys)
		{
			if (!JudgeTaskFinish(key))
			{
				unfinishTaskIDList.Add(key);
			}
		}
		DealShowTaskData();
	}

	public bool JudgeTaskFinish(int tempID)
	{
		string[] array = UserDataManager.Instance.GetFinishTaskString().Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "" && int.Parse(array[i]) == tempID)
			{
				return true;
			}
		}
		return false;
	}

	private void DealShowTaskData()
	{
		showTaskList.Clear();
		for (int i = 0; i < unfinishTaskIDList.Count; i++)
		{
			if (JudgeTaskStartCondition(unfinishTaskIDList[i]) && currStageTask[unfinishTaskIDList[i]].IsAuto != 1)
			{
				showTaskList.Add(currStageTask[unfinishTaskIDList[i]]);
			}
			else if (JudgeTaskStartCondition(unfinishTaskIDList[i]) && currStageTask[unfinishTaskIDList[i]].IsAuto == 1)
			{
				PlotManager.Instance.DelayStartPlot(unfinishTaskIDList[i], -1, 0.2f, true);
			}
		}
		CalculateProgress();
	}

	private void DealWrongTask()
	{
		string[] array = UserDataManager.Instance.GetCurrDoTaskString().Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "" && JudgeIsWrongTask(int.Parse(array[i].Split(',')[0])))
			{
				TaskConfigData taskWithID = GetTaskWithID(int.Parse(array[i].Split(',')[0]));
				if (taskWithID != null)
				{
					DealTaskStatu(taskWithID, taskWithID.FinishConditionNum);
				}
			}
		}
	}

	private TaskConfigData GetTaskWithID(int taskID)
	{
		TaskConfigData result = null;
		if (currStageTask.ContainsKey(taskID))
		{
			return currStageTask[taskID];
		}
		return result;
	}

	private bool JudgeIsWrongTask(int taskID)
	{
		if (currStageTask.ContainsKey(taskID) && currStageTask[taskID].FinishConditionNum < 2)
		{
			return true;
		}
		return false;
	}

	private void CalculateProgress()
	{
		stageCurrStep = 0f;
		for (int i = 0; i < finishTaskIDList.Count; i++)
		{
			foreach (int key in currStageTask.Keys)
			{
				if (key == finishTaskIDList[i])
				{
					stageCurrStep += currStageTask[key].FinishConditionNum;
				}
			}
		}
		string[] array = UserDataManager.Instance.GetCurrDoTaskString().Split(';');
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] != "")
			{
				stageCurrStep += int.Parse(array[j].Split(',')[1]);
			}
		}
		if (stageAllStep > 0)
		{
			taskProgress = stageCurrStep / (float)stageAllStep * 1f;
		}
		else
		{
			taskProgress = 0f;
		}
		if (taskProgress < 0f)
		{
			taskProgress = 0f;
		}
	}

	private bool JudgeTaskStartCondition(int ID)
	{
		if (currStageTask[ID].StartCondition == "")
		{
			return true;
		}
		string[] array = currStageTask[ID].StartCondition.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < finishTaskIDList.Count; j++)
			{
				if (array[i] == "" || int.Parse(array[i].Split('.')[0]) == finishTaskIDList[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public void DealPlotFinish(int plotID)
	{
		currDoTask = null;
		for (int i = 0; i < showTaskList.Count; i++)
		{
			string[] array = showTaskList[i].PlotID.Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				if (int.Parse(array[j].Split('.')[0]) == plotID)
				{
					DealTaskStatu(showTaskList[i], j + 1);
				}
			}
		}
	}

	public void DealNewPlotFinish(int taskID, int taskStepID)
	{
		currDoTask = null;
		for (int i = 0; i < showTaskList.Count; i++)
		{
			if (showTaskList[i].ID == taskID)
			{
				DealTaskStatu(showTaskList[i], taskStepID);
			}
		}
	}

	public void DealAutoTaskFinish(int taskID)
	{
		for (int i = 0; i < taskConfig.Count; i++)
		{
			if (taskConfig[i].ID == taskID)
			{
				DealTaskStatu(taskConfig[i], -1);
			}
		}
	}

	public void DealTaskStatu(TaskConfigData task, int step)
	{
		if (step >= 0 && step < task.FinishConditionNum - 1)
		{
			string[] array = UserDataManager.Instance.GetCurrDoTaskString().Split(';');
			string text = "";
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != "" && int.Parse(array[i].Split(',')[0]) == task.ID)
				{
					flag = true;
					array[i] = task.ID.ToString() + "," + step;
				}
			}
			if (flag)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != "")
					{
						text = text + array[j] + ";";
					}
				}
				UserDataManager.Instance.SetCurrDoTaskString(text);
			}
			else
			{
				string currDoTaskString = UserDataManager.Instance.GetCurrDoTaskString() + task.ID.ToString() + "," + step + ";";
				UserDataManager.Instance.SetCurrDoTaskString(currDoTaskString);
			}
		}
		else if (step >= task.FinishConditionNum - 1 || step < 0)
		{
			string text2 = UserDataManager.Instance.GetFinishTaskString() + task.ID + ";";
			UserDataManager.Instance.SetFinishTaskString(text2.ToString());
			string[] array2 = UserDataManager.Instance.GetCurrDoTaskString().Split(';');
			string text3 = "";
			bool flag2 = false;
			for (int k = 0; k < array2.Length; k++)
			{
				if (array2[k] != "" && int.Parse(array2[k].Split(',')[0]) == task.ID)
				{
					flag2 = true;
					array2[k] = "";
				}
			}
			if (flag2)
			{
				for (int l = 0; l < array2.Length; l++)
				{
					if (array2[l] != "")
					{
						text3 = text3 + array2[l] + ";";
					}
				}
				UserDataManager.Instance.SetCurrDoTaskString(text3);
			}
			if (!JudgeAutoPlotIncludeTask(task.ID))
			{
				UserDataManager.Instance.GetService().LastFinishTaskID = task.ID;
				UserDataManager.Instance.GetService().LastFinishTaskStage = UserDataManager.Instance.GetService().stage;
				UserDataManager.Instance.Save();
				UpdateStageStatus();
			}
			else
			{
				PlotManager.Instance.DelayStartPlot(currAutoTaskID, -1, 0.1f);
			}
		}
		CalculateProgress();
	}

	public void UpdateStageStatus()
	{
		if (JudgeStageFinish() && UserDataManager.Instance.GetService().stage == 0)
		{
			DealStageTask();
			return;
		}
		DealTaskData();
		CalculateProgress();
	}

	public void DealStageTask()
	{
		UserDataManager.Instance.SetCurrDoTaskString("");
		UserDataManager.Instance.SetFinishTaskString("");
		UserDataManager.Instance.IncressStage();
		UserDataManager.Instance.Save();
		StageManage.Instance.LoadChapterInfo();
		LoadStageTask();
		stageCurrStep = 0f;
		UpdateStageStatus();
	}

	private bool JudgeStageFinish()
	{
		bool flag = true;
		foreach (int key in currStageTask.Keys)
		{
			if (!JudgeTaskFinish(key))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	public bool JudgeAutoPlotInclude(int taskID)
	{
		currAutoTaskID = -1;
		for (int i = 0; i < autoTaskConfig.Count; i++)
		{
			if (taskID == autoTaskConfig[i].ID)
			{
				return true;
			}
		}
		return false;
	}

	public bool JudgeAutoPlotIncludeTask(int finishTaskID)
	{
		for (int i = 0; i < autoTaskConfig.Count; i++)
		{
			string[] array = autoTaskConfig[i].StartCondition.Split(',');
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != "" && int.Parse(array[j].Split('.')[0]) == finishTaskID && JudgeAutoTaskStart(autoTaskConfig[i]))
				{
					currAutoTaskID = autoTaskConfig[i].ID;
					return true;
				}
			}
		}
		currAutoTaskID = -1;
		return false;
	}

	public bool JudgeAutoTaskStart(TaskConfigData plotData)
	{
		string[] array = plotData.StartCondition.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			if (!JudgeTaskFinish(int.Parse(array[i].Split('.')[0])))
			{
				return false;
			}
		}
		return true;
	}

	public void StartTask(TaskConfigData task)
	{
		currDoTask = task;
	}
}
