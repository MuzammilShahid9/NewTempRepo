using System;
using System.Collections.Generic;
using System.Xml;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class IdleConfigManager : MonoBehaviour
{
	private static IdleConfigManager instance;

	private XmlDocument _xml;

	private string _plotPath;

	private XmlNode _plotRoot;

	private XmlNodeList _xmlTaskNodes;

	public List<IDInfo> taskList = new List<IDInfo>();

	public static IdleConfigManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
        PlayerPrefs.DeleteAll();
		_xml = new XmlDocument();
        Debug.Log("Application.dataPath " + Application.dataPath);
		_plotPath = Application.dataPath + "/Resources/Config/IdleAction/IdleActionConfig.xml";
		LoadIdleActionConfigInfo();
	}

	private void LoadIdleActionConfigInfo()
	{
		taskList.Clear();
		LoadIdleActionConfig();
	}

	private void LoadIdleActionConfig()
	{
		TextAsset textAsset = (TextAsset)Resources.Load("Config/IdleAction/IdleActionConfig");
        Debug.Log("textAsset " + textAsset.text);
		_xml.LoadXml(textAsset.text);
		_plotRoot = _xml.SelectSingleNode("IdleActionConfig");
        Debug.Log("_plotRoot " + _plotRoot.ChildNodes.ToString());
        _xmlTaskNodes = _plotRoot.ChildNodes;
		int num = 0;
		foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
		{
			IDInfo iDInfo = new IDInfo();
			iDInfo.actionElem = xmlTaskNode;
			iDInfo.startTask = xmlTaskNode.GetAttribute("StartTask");
			iDInfo.endTask = xmlTaskNode.GetAttribute("EndTask");
			iDInfo.roleType = (RoleType)Enum.Parse(typeof(RoleType), xmlTaskNode.GetAttribute("RoleType"));
			iDInfo.actions = new List<IDAction>();
			foreach (XmlElement childNode in xmlTaskNode.ChildNodes)
			{
				iDInfo.actions.Add(GetActionDataByXML(childNode));
			}
			taskList.Add(iDInfo);
			num++;
		}
	}

	public IDAction GetActionDataByXML(XmlElement actionElem)
	{
        Debug.Log(IDStepManager.Instance);
        IDAction iDAction = IDStepManager.Instance.CreateAction();
		iDAction.actionElem = actionElem;
		iDAction.tm = float.Parse(actionElem.GetAttribute("Time"));
		iDAction.roleConfig = new IDRoleConfig();
		iDAction.roleConfig.roles = new Dictionary<RoleType, IDRoleAnim>();
		if (actionElem.GetElementsByTagName("Role").Count > 0)
		{
			iDAction.roleConfig.roleElem = (XmlElement)actionElem.GetElementsByTagName("Role")[0];
			foreach (XmlElement childNode in iDAction.roleConfig.roleElem.ChildNodes)
			{
				string[] array = childNode.GetAttribute("Anim").Split(';');
				IDRoleAnim iDRoleAnim = new IDRoleAnim
				{
					anim = new List<string>()
				};
				string[] array2 = array;
				foreach (string item in array2)
				{
					iDRoleAnim.anim.Add(item);
				}
				iDAction.roleConfig.roles.Add((RoleType)Enum.Parse(typeof(RoleType), childNode.Name), iDRoleAnim);
			}
			iDAction.roleConfig.isSet = true;
		}
		iDAction.convConfig = new IDConversationConfig();
		iDAction.convConfig.convList = new List<string>();
		if (actionElem.GetElementsByTagName("Conversation").Count > 0)
		{
			iDAction.convConfig.convElem = (XmlElement)actionElem.GetElementsByTagName("Conversation")[0];
			string[] array2 = iDAction.convConfig.convElem.GetAttribute("Content").Split(';');
			foreach (string item2 in array2)
			{
				iDAction.convConfig.convList.Add(item2);
			}
			iDAction.convConfig.isSet = true;
		}
		iDAction.buildConfig = new IDBuildConfig();
		if (actionElem.GetElementsByTagName("Build").Count > 0)
		{
			iDAction.buildConfig.buildElem = (XmlElement)actionElem.GetElementsByTagName("Build")[0];
			string[] array3 = iDAction.buildConfig.buildElem.GetAttribute("Data").Split(';');
			iDAction.buildConfig.roomID = int.Parse(array3[0]);
			iDAction.buildConfig.itemID = int.Parse(array3[1]);
			if (array3.Length > 2)
			{
				iDAction.buildConfig.stageID = int.Parse(array3[2]);
			}
			iDAction.buildConfig.isSet = true;
		}
		iDAction.otherConfig = new IDOtherConfig();
		if (actionElem.GetElementsByTagName("Other").Count > 0)
		{
			iDAction.otherConfig.otherElem = (XmlElement)actionElem.GetElementsByTagName("Other")[0];
			if (iDAction.otherConfig.otherElem.GetAttribute("IsChapterEnd") != "")
			{
				iDAction.otherConfig.isChapterEnd = bool.Parse(iDAction.otherConfig.otherElem.GetAttribute("IsChapterEnd"));
			}
			else
			{
				iDAction.otherConfig.isChapterEnd = false;
			}
			if (iDAction.otherConfig.otherElem.GetAttribute("IsCatRename") != "")
			{
				iDAction.otherConfig.isCatRename = bool.Parse(iDAction.otherConfig.otherElem.GetAttribute("IsCatRename"));
			}
			else
			{
				iDAction.otherConfig.isCatRename = false;
			}
			if (iDAction.otherConfig.otherElem.GetAttribute("IsCastleRename") != "")
			{
				iDAction.otherConfig.isCastleRename = bool.Parse(iDAction.otherConfig.otherElem.GetAttribute("IsCastleRename"));
			}
			else
			{
				iDAction.otherConfig.isCastleRename = false;
			}
			if (iDAction.otherConfig.otherElem.GetAttribute("IsBlackScreen") != "")
			{
				iDAction.otherConfig.isBlackScreen = bool.Parse(iDAction.otherConfig.otherElem.GetAttribute("IsBlackScreen"));
			}
			else
			{
				iDAction.otherConfig.isBlackScreen = false;
			}
			iDAction.otherConfig.isSet = true;
		}
		iDAction.audioConfig = new IDAudioConfig();
		if (actionElem.GetElementsByTagName("Audio").Count > 0)
		{
			iDAction.audioConfig.audioElem = (XmlElement)actionElem.GetElementsByTagName("Audio")[0];
			if (iDAction.audioConfig.audioElem.GetAttribute("IsMusicSet") != "")
			{
				iDAction.audioConfig.isMusicSet = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsMusicSet"));
				iDAction.audioConfig.musicName = iDAction.audioConfig.audioElem.GetAttribute("MusicName");
				iDAction.audioConfig.isMusicLoop = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsMusicLoop"));
				iDAction.audioConfig.isMusicStop = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsMusicStop"));
				iDAction.audioConfig.musicMinTime = float.Parse(iDAction.audioConfig.audioElem.GetAttribute("MusicMinTime"));
				iDAction.audioConfig.musicMaxTime = float.Parse(iDAction.audioConfig.audioElem.GetAttribute("MusicMaxTime"));
			}
			if (iDAction.audioConfig.audioElem.GetAttribute("IsEffectSet") != "")
			{
				iDAction.audioConfig.isEffectSet = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsEffectSet"));
				iDAction.audioConfig.effectName = iDAction.audioConfig.audioElem.GetAttribute("EffectName");
				iDAction.audioConfig.isEffectLoop = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsEffectLoop"));
				iDAction.audioConfig.isEffectStop = bool.Parse(iDAction.audioConfig.audioElem.GetAttribute("IsEffectStop"));
				iDAction.audioConfig.effectMinTime = float.Parse(iDAction.audioConfig.audioElem.GetAttribute("EffectMinTime"));
				iDAction.audioConfig.effectMaxTime = float.Parse(iDAction.audioConfig.audioElem.GetAttribute("EffectMaxTime"));
			}
			iDAction.audioConfig.isSet = true;
		}
		iDAction.delayConfig = new IDDelayConfig();
		if (actionElem.GetElementsByTagName("Delay").Count > 0)
		{
			iDAction.delayConfig.delayElem = (XmlElement)actionElem.GetElementsByTagName("Delay")[0];
			iDAction.delayConfig.delayTime = float.Parse(iDAction.delayConfig.delayElem.GetAttribute("DelayTime"));
			iDAction.delayConfig.isSet = true;
		}
		return iDAction;
	}

	public List<IDInfo> GetUnlockIdleAction(RoleType roleType)
	{
		List<IDInfo> list = new List<IDInfo>();
		for (int i = 0; i < taskList.Count; i++)
		{
			if (JudgeRightTask(taskList[i]) && taskList[i].roleType == roleType)
			{
				list.Add(taskList[i]);
			}
		}
		return list;
	}

	private bool JudgeRightTask(IDInfo action)
	{
		int num = int.Parse(action.startTask.Split('-')[0]);
		int num2 = int.Parse(action.startTask.Split('-')[1]);
		int num3 = int.Parse(action.endTask.Split('-')[0]);
		int num4 = int.Parse(action.endTask.Split('-')[1]);
		int lastFinishTaskID = UserDataManager.Instance.GetService().LastFinishTaskID;
		int lastFinishTaskStage = UserDataManager.Instance.GetService().LastFinishTaskStage;
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
		if (UserDataManager.Instance.GetService().LastFinishTaskStage > num && UserDataManager.Instance.GetService().LastFinishTaskStage < num3)
		{
			return true;
		}
		return false;
	}
}
