using System;
using System.Collections.Generic;
using System.Xml;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class StageManage : MonoBehaviour
{
	[Serializable]
	public class StageConfigDataList
	{
		public List<StageConfigData> data = new List<StageConfigData>();
	}

	[Serializable]
	public class StageConfigData
	{
		public string Name;

		public int ID;

		public string Award;
	}

	private static StageManage instance;

	public int currentChapter;

	public int currentTask;

	public int currentSubTask;

	public List<Task> taskList = new List<Task>();

	private List<StageConfigData> stageConfig;

	private XmlDocument _xml;

	private string _plotPath;

	private XmlNode _plotRoot;

	private XmlNodeList _xmlTaskNodes;

	public static StageManage Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		StageConfigDataList stageConfigDataList = JsonUtility.FromJson<StageConfigDataList>((Resources.Load("Config/Task/StageConfig") as TextAsset).text);
		stageConfig = stageConfigDataList.data;
	}

	private void Start()
	{
		if (TestConfig.active && TestConfig.startTask > 0)
		{
			UserDataManager.Instance.GetService().stage = TestConfig.startStage;
		}
		_xml = new XmlDocument();
		_plotPath = Application.dataPath + "/Resources/Config/Plot/Chapter" + UserDataManager.Instance.GetService().stage + ".xml";
		LoadChapterInfo();
	}

	public void LoadChapterInfo()
	{
		taskList.Clear();
		LoadChapter(UserDataManager.Instance.GetService().stage);
	}

	public void LoadChapter(int chapter)
	{
		if (UserDataManager.Instance.GetService().stage > GeneralConfig.MaxStage)
		{
			return;
		}
		TextAsset textAsset = (TextAsset)Resources.Load("Config/Plot/Chapter" + UserDataManager.Instance.GetService().stage);
		_xml.LoadXml(textAsset.text);
		_plotRoot = _xml.SelectSingleNode("Chapter" + chapter);
		_xmlTaskNodes = _plotRoot.ChildNodes;
		int num = 0;
		foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
		{
			Task task = new Task();
			task.taskElem = xmlTaskNode;
			XmlNodeList elementsByTagName = xmlTaskNode.GetElementsByTagName("SubTask");
			if (elementsByTagName.Count > 0)
			{
				task.subTaskList = new List<Task>();
				int num2 = 0;
				foreach (XmlElement item in elementsByTagName)
				{
					Task task2 = new Task();
					task2.actions = new List<CDAction>();
					task2.taskElem = item;
					foreach (XmlElement childNode in item.ChildNodes)
					{
						task2.actions.Add(LoadAction(childNode));
					}
					task.subTaskList.Add(task2);
					num2++;
				}
			}
			else
			{
				task.actions = new List<CDAction>();
				foreach (XmlElement childNode2 in xmlTaskNode.ChildNodes)
				{
					task.actions.Add(LoadAction(childNode2));
				}
			}
			taskList.Add(task);
			num++;
		}
	}

	public List<ItemIndex> GetUnlockItemID(int stageID, int taskID)
	{
		List<ItemIndex> list = new List<ItemIndex>();
		ItemIndex itemIndex = null;
		Task task = null;
		task = ((stageID != 1) ? GetActionWithStageAndTaskId(stageID, taskID - 1) : GetActionWithStageAndTaskId(stageID, taskID));
		if (task.subTaskList != null)
		{
			for (int i = 0; i < task.subTaskList.Count; i++)
			{
				Task task2 = task.subTaskList[i];
				for (int j = 0; j < task2.actions.Count; j++)
				{
					if (task2.actions[j].buildConfig.isSet)
					{
						itemIndex = new ItemIndex();
						itemIndex.roomIndex = task2.actions[j].buildConfig.roomID;
						itemIndex.itemIndex = task2.actions[j].buildConfig.itemID;
						list.Add(itemIndex);
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < task.actions.Count; k++)
			{
				if (task.actions[k].buildConfig.isSet)
				{
					itemIndex = new ItemIndex();
					itemIndex.roomIndex = task.actions[k].buildConfig.roomID;
					itemIndex.itemIndex = task.actions[k].buildConfig.itemID;
					list.Add(itemIndex);
				}
			}
		}
		return list;
	}

	public Task GetActionWithStageAndTaskId(int stageID, int taskID)
	{
		_xml = new XmlDocument();
		Task result = null;
		TextAsset textAsset = (TextAsset)Resources.Load("Config/Plot/Chapter" + UserDataManager.Instance.GetService().stage);
		_xml.LoadXml(textAsset.text);
		_plotRoot = _xml.SelectSingleNode("Chapter" + stageID);
		_xmlTaskNodes = _plotRoot.ChildNodes;
		int num = 0;
		foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
		{
			if (num == taskID)
			{
				result = new Task();
				result.taskElem = xmlTaskNode;
				XmlNodeList elementsByTagName = xmlTaskNode.GetElementsByTagName("SubTask");
				if (elementsByTagName.Count > 0)
				{
					result.subTaskList = new List<Task>();
					int num2 = 0;
					foreach (XmlElement item in elementsByTagName)
					{
						Task task = new Task();
						task.actions = new List<CDAction>();
						task.taskElem = item;
						foreach (XmlElement childNode in item.ChildNodes)
						{
							task.actions.Add(LoadAction(childNode));
						}
						result.subTaskList.Add(task);
						num2++;
					}
				}
				else
				{
					result.actions = new List<CDAction>();
					foreach (XmlElement childNode2 in xmlTaskNode.ChildNodes)
					{
						result.actions.Add(LoadAction(childNode2));
					}
				}
				return result;
			}
			num++;
		}
		return result;
	}

	public List<ItemIndex> GetItemWithStageId(int stageID)
	{
		_xml = new XmlDocument();
		List<Task> list = new List<Task>();
		TextAsset textAsset = (TextAsset)Resources.Load("Config/Plot/Chapter" + stageID);
		_xml.LoadXml(textAsset.text);
		_plotRoot = _xml.SelectSingleNode("Chapter" + stageID);
		_xmlTaskNodes = _plotRoot.ChildNodes;
		foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
		{
			Task task = new Task();
			task.taskElem = xmlTaskNode;
			XmlNodeList elementsByTagName = xmlTaskNode.GetElementsByTagName("SubTask");
			if (elementsByTagName.Count > 0)
			{
				task.subTaskList = new List<Task>();
				int num = 0;
				foreach (XmlElement item in elementsByTagName)
				{
					Task task2 = new Task();
					task2.actions = new List<CDAction>();
					task2.taskElem = item;
					foreach (XmlElement childNode in item.ChildNodes)
					{
						task2.actions.Add(LoadAction(childNode));
					}
					task.subTaskList.Add(task2);
					num++;
				}
			}
			else
			{
				task.actions = new List<CDAction>();
				foreach (XmlElement childNode2 in xmlTaskNode.ChildNodes)
				{
					task.actions.Add(LoadAction(childNode2));
				}
			}
			list.Add(task);
		}
		List<ItemIndex> list2 = new List<ItemIndex>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].subTaskList == null)
			{
				for (int j = 0; j < list[i].actions.Count; j++)
				{
					if (list[i].actions[j].buildConfig.isSet)
					{
						ItemIndex itemIndex = new ItemIndex();
						itemIndex.roomIndex = list[i].actions[j].buildConfig.roomID;
						itemIndex.itemIndex = list[i].actions[j].buildConfig.itemID;
						list2.Add(itemIndex);
					}
				}
				continue;
			}
			for (int k = 0; k < list[i].subTaskList.Count; k++)
			{
				Task task3 = list[i].subTaskList[k];
				for (int l = 0; l < task3.actions.Count; l++)
				{
					if (task3.actions[l].buildConfig.isSet)
					{
						ItemIndex itemIndex2 = new ItemIndex();
						itemIndex2.roomIndex = task3.actions[l].buildConfig.roomID;
						itemIndex2.itemIndex = task3.actions[l].buildConfig.itemID;
						list2.Add(itemIndex2);
					}
				}
			}
		}
		return list2;
	}

	public Task GetActionWithTaskId(int taskId, int subTaskId = -1)
	{
		Task task = null;
		task = ((UserDataManager.Instance.GetService().stage == 1) ? taskList[taskId] : taskList[taskId - 1]);
		if (subTaskId != -1)
		{
			task = task.subTaskList[subTaskId];
		}
		return task;
	}

	public CDAction LoadAction(XmlElement actionElem)
	{
		CDAction cDAction = CDActionManager.Instance.CreateAction();
		cDAction.actionElem = actionElem;
		cDAction.tm = float.Parse(actionElem.GetAttribute("Time"));
		cDAction.camConfig = new CDCameraConfig();
		if (actionElem.GetElementsByTagName("Camera").Count > 0)
		{
			cDAction.camConfig.camElem = (XmlElement)actionElem.GetElementsByTagName("Camera")[0];
			cDAction.camConfig.pos = MathTool.GetVec3ByString(cDAction.camConfig.camElem.GetAttribute("Pos"));
			cDAction.camConfig.size = float.Parse(cDAction.camConfig.camElem.GetAttribute("Size"));
			cDAction.camConfig.tm = float.Parse(cDAction.camConfig.camElem.GetAttribute("Time"));
			cDAction.camConfig.isFollow = bool.Parse(cDAction.camConfig.camElem.GetAttribute("IsFollow"));
			cDAction.camConfig.followRole = (RoleType)Enum.Parse(typeof(RoleType), cDAction.camConfig.camElem.GetAttribute("FollowTarget"));
			cDAction.camConfig.isSet = true;
		}
		cDAction.roleConfig = new CDRoleConfig();
		cDAction.roleConfig.roles = new Dictionary<RoleType, CDRoleAnim>();
		if (actionElem.GetElementsByTagName("Role").Count > 0)
		{
			cDAction.roleConfig.roleElem = (XmlElement)actionElem.GetElementsByTagName("Role")[0];
			foreach (XmlElement childNode in cDAction.roleConfig.roleElem.ChildNodes)
			{
				string[] array = childNode.GetAttribute("Anim").Split(';');
				CDRoleAnim cDRoleAnim = new CDRoleAnim
				{
					anim = new List<string>()
				};
				string[] array2 = array;
				foreach (string item in array2)
				{
					cDRoleAnim.anim.Add(item);
				}
				cDAction.roleConfig.roles.Add((RoleType)Enum.Parse(typeof(RoleType), childNode.Name), cDRoleAnim);
			}
			cDAction.roleConfig.isSet = true;
		}
		cDAction.convConfig = new CDConversationConfig();
		cDAction.convConfig.convList = new List<string>();
		if (actionElem.GetElementsByTagName("Conversation").Count > 0)
		{
			cDAction.convConfig.convElem = (XmlElement)actionElem.GetElementsByTagName("Conversation")[0];
			string[] array2 = cDAction.convConfig.convElem.GetAttribute("Content").Split(';');
			foreach (string item2 in array2)
			{
				cDAction.convConfig.convList.Add(item2);
			}
			cDAction.convConfig.isSet = true;
		}
		cDAction.buildConfig = new CDBuildConfig();
		if (actionElem.GetElementsByTagName("Build").Count > 0)
		{
			cDAction.buildConfig.buildElem = (XmlElement)actionElem.GetElementsByTagName("Build")[0];
			string[] array3 = cDAction.buildConfig.buildElem.GetAttribute("Data").Split(';');
			cDAction.buildConfig.roomID = int.Parse(array3[0]);
			cDAction.buildConfig.itemID = int.Parse(array3[1]);
			if (array3.Length > 2)
			{
				cDAction.buildConfig.stageID = int.Parse(array3[2]);
			}
			if (array3.Length > 3)
			{
				cDAction.buildConfig.delayTime = float.Parse(array3[3]);
			}
			else
			{
				cDAction.buildConfig.delayTime = 0f;
			}
			cDAction.buildConfig.isSet = true;
		}
		cDAction.otherConfig = new CDOtherConfig();
		if (actionElem.GetElementsByTagName("Other").Count > 0)
		{
			cDAction.otherConfig.otherElem = (XmlElement)actionElem.GetElementsByTagName("Other")[0];
			if (cDAction.otherConfig.otherElem.GetAttribute("IsChapterEnd") != "")
			{
				cDAction.otherConfig.isChapterEnd = bool.Parse(cDAction.otherConfig.otherElem.GetAttribute("IsChapterEnd"));
			}
			else
			{
				cDAction.otherConfig.isChapterEnd = false;
			}
			if (cDAction.otherConfig.otherElem.GetAttribute("IsCatRename") != "")
			{
				cDAction.otherConfig.isCatRename = bool.Parse(cDAction.otherConfig.otherElem.GetAttribute("IsCatRename"));
			}
			else
			{
				cDAction.otherConfig.isCatRename = false;
			}
			if (cDAction.otherConfig.otherElem.GetAttribute("IsCastleRename") != "")
			{
				cDAction.otherConfig.isCastleRename = bool.Parse(cDAction.otherConfig.otherElem.GetAttribute("IsCastleRename"));
			}
			else
			{
				cDAction.otherConfig.isCastleRename = false;
			}
			if (cDAction.otherConfig.otherElem.GetAttribute("IsBlackScreen") != "")
			{
				cDAction.otherConfig.isBlackScreen = bool.Parse(cDAction.otherConfig.otherElem.GetAttribute("IsBlackScreen"));
			}
			else
			{
				cDAction.otherConfig.isBlackScreen = false;
			}
			if (cDAction.otherConfig.otherElem.GetAttribute("UnlockRoomID") != "")
			{
				cDAction.otherConfig.unlockRoomID = int.Parse(cDAction.otherConfig.otherElem.GetAttribute("UnlockRoomID"));
			}
			else
			{
				cDAction.otherConfig.unlockRoomID = 0;
			}
			cDAction.otherConfig.isSet = true;
		}
		cDAction.audioConfig = new CDAudioConfig();
		if (actionElem.GetElementsByTagName("Audio").Count > 0)
		{
			cDAction.audioConfig.audioElem = (XmlElement)actionElem.GetElementsByTagName("Audio")[0];
			if (cDAction.audioConfig.audioElem.GetAttribute("IsMusicSet") != "")
			{
				cDAction.audioConfig.isMusicSet = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsMusicSet"));
				cDAction.audioConfig.musicName = (MusicClip)Enum.Parse(typeof(MusicClip), cDAction.audioConfig.audioElem.GetAttribute("MusicName"));
				cDAction.audioConfig.isMusicLoop = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsMusicLoop"));
				cDAction.audioConfig.isMusicStop = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsMusicStop"));
				cDAction.audioConfig.musicMinTime = float.Parse(cDAction.audioConfig.audioElem.GetAttribute("MusicMinTime"));
				cDAction.audioConfig.musicMaxTime = float.Parse(cDAction.audioConfig.audioElem.GetAttribute("MusicMaxTime"));
			}
			if (cDAction.audioConfig.audioElem.GetAttribute("IsEffectSet") != "")
			{
				cDAction.audioConfig.isEffectSet = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsEffectSet"));
				cDAction.audioConfig.effectName = (EffectClip)Enum.Parse(typeof(EffectClip), cDAction.audioConfig.audioElem.GetAttribute("EffectName"));
				cDAction.audioConfig.isEffectLoop = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsEffectLoop"));
				cDAction.audioConfig.isEffectStop = bool.Parse(cDAction.audioConfig.audioElem.GetAttribute("IsEffectStop"));
				cDAction.audioConfig.effectMinTime = float.Parse(cDAction.audioConfig.audioElem.GetAttribute("EffectMinTime"));
				cDAction.audioConfig.effectMaxTime = float.Parse(cDAction.audioConfig.audioElem.GetAttribute("EffectMaxTime"));
			}
			cDAction.audioConfig.isSet = true;
		}
		return cDAction;
	}

	public string GetStageAward(int stage)
	{
		for (int i = 0; i < stageConfig.Count; i++)
		{
			if (stageConfig[i].ID == stage)
			{
				return stageConfig[i].Award;
			}
		}
		return "";
	}

	public string GetStageName(int stage)
	{
		for (int i = 0; i < stageConfig.Count; i++)
		{
			if (stageConfig[i].ID == stage)
			{
				return stageConfig[i].Name;
			}
		}
		return "";
	}

	public void ShowStageFinish()
	{
		EntranceRenovatedDlg.Instance.Enter();
	}
}
