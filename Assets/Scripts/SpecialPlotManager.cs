using System;
using System.Collections.Generic;
using System.Xml;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class SpecialPlotManager : MonoBehaviour
{
	private static SpecialPlotManager instance;

	private XmlDocument _xml;

	private XmlNode _plotRoot;

	private XmlNodeList _xmlTaskNodes;

	public List<Task> taskList = new List<Task>();

	public static SpecialPlotManager Instance
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

	public void GetSpecialPlotData()
	{
		_xml = new XmlDocument();
		TextAsset textAsset = (TextAsset)Resources.Load("Config/Plot/MagicPlot");
		_xml.LoadXml(textAsset.text);
		_plotRoot = _xml.SelectSingleNode("MagicPlot");
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

	public CDAction LoadAction(XmlElement actionElem)
	{
		CDAction cDAction = CDActionManager.Instance.CreateAction();
		cDAction.actionElem = actionElem;
		DebugUtils.Log(DebugType.Other, "time" + actionElem.GetAttribute("Time"));
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
}
