using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDInfoManager : MonoBehaviour
	{
		private static IDInfoManager instance;

		public GameObject chapterText;

		public GameObject chapterTextPanel;

		public VerticalLayoutGroup textContent;

		private List<Text> _chapterTextList = new List<Text>();

		public List<IDInfo> _idleActionConfig = new List<IDInfo>();

		public int currentAction;

		private string _path;

		private XmlDocument _xml;

		private XmlNode _plotRoot;

		private XmlNodeList _xmlTaskNodes;

		public static IDInfoManager Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			_xml = new XmlDocument();
		}

		private void Start()
		{
			_path = Application.dataPath + "/Resources/Config/IdleAction/";
			if (!Directory.Exists(_path))
			{
				return;
			}
			FileInfo[] files = new DirectoryInfo(_path).GetFiles("*", SearchOption.AllDirectories);
			DebugUtils.Log(DebugType.Other, files.Length);
			for (int i = 0; i < files.Length; i++)
			{
				if (!files[i].Name.EndsWith(".meta") && files[i].Name.Contains("IdleActionConfig"))
				{
					LoadIdleActionConfig();
				}
			}
		}

		public IDInfo GetCurrIDInfo()
		{
			return _idleActionConfig[currentAction];
		}

		private void ResetColor()
		{
			foreach (Text chapterText in _chapterTextList)
			{
				chapterText.color = Color.black;
			}
		}

		private void SetTextColor(int idx, Color color)
		{
			ResetColor();
			_chapterTextList[idx].color = color;
		}

		public void BtnPrevChapterClicked()
		{
			if (currentAction > 0)
			{
				ResetColor();
				currentAction--;
				SetTextColor(currentAction, Color.red);
				IDActionManager.Instance.LoadAction(_idleActionConfig[currentAction]);
			}
		}

		public void BtnNextChapterClicked()
		{
			if (currentAction < _chapterTextList.Count - 1)
			{
				ResetColor();
				currentAction++;
				SetTextColor(currentAction, Color.red);
				IDActionManager.Instance.LoadAction(_idleActionConfig[currentAction]);
			}
		}

		public void BtnRunClick()
		{
			IdleManager.Instance.StartIdleActionWithIDInfo(_idleActionConfig[currentAction]);
		}

		public void BtnAddPrevTaskClicked()
		{
			if (_idleActionConfig.Count == 0)
			{
				InsertTask(0);
			}
			else
			{
				InsertTask(currentAction);
			}
		}

		public void BtnAddNextTaskClicked()
		{
			if (_idleActionConfig.Count == 0)
			{
				InsertTask(0);
			}
			else
			{
				InsertTask(currentAction + 1);
			}
		}

		public void BtnDelTaskClicked()
		{
			if (_idleActionConfig.Count > 0)
			{
				int taskTextIdx = GetTaskTextIdx(currentAction, 0);
				DelText(GetTaskTextIdx(currentAction, 0));
				if (taskTextIdx > 0)
				{
					SetTextColor(taskTextIdx - 1, Color.red);
					_idleActionConfig.RemoveAt(currentAction);
					currentAction--;
				}
				else if (taskTextIdx == 0 && _idleActionConfig.Count > 0)
				{
					SetTextColor(taskTextIdx, Color.red);
					_idleActionConfig.RemoveAt(taskTextIdx);
				}
			}
			RefreshTask();
		}

		private void DelText(int idx)
		{
			UnityEngine.Object.Destroy(_chapterTextList[idx].gameObject);
			_chapterTextList.RemoveAt(idx);
		}

		public void InsertTask(int taskIdx)
		{
			IDInfo iDInfo = new IDInfo();
			iDInfo.startTask = "0-0";
			iDInfo.endTask = "0-0";
			iDInfo.roleType = RoleType.Alice;
			iDInfo.actions = new List<IDAction>();
			if (_idleActionConfig.Count > 0 && taskIdx < _idleActionConfig.Count)
			{
				_idleActionConfig.Insert(taskIdx, iDInfo);
				currentAction = taskIdx;
			}
			if (_idleActionConfig.Count > 0 && taskIdx == _idleActionConfig.Count)
			{
				_idleActionConfig.Add(iDInfo);
				currentAction = taskIdx;
			}
			else if (_idleActionConfig.Count == 0)
			{
				_idleActionConfig.Add(iDInfo);
				currentAction = 0;
			}
			int taskTextIdx = GetTaskTextIdx(taskIdx, 0);
			AddText("Action " + taskIdx + "   " + iDInfo.startTask + "~~" + iDInfo.endTask + " " + iDInfo.roleType);
			SetTextColor(taskTextIdx, Color.red);
			currentAction = taskIdx;
			IDActionManager.Instance.LoadAction(_idleActionConfig[currentAction]);
			RefreshTask();
		}

		public void RefreshTask()
		{
			for (int i = 0; i < _idleActionConfig.Count; i++)
			{
				_chapterTextList[GetTaskTextIdx(i, 0)].text = "Action " + (i + 1) + "   " + _idleActionConfig[i].startTask + "~~" + _idleActionConfig[i].endTask + " " + _idleActionConfig[i].roleType;
			}
			StartCoroutine(resizeContent());
		}

		private IEnumerator resizeContent()
		{
			yield return null;
			textContent.GetComponent<RectTransform>().sizeDelta = new Vector2(textContent.GetComponent<RectTransform>().sizeDelta.x, textContent.preferredHeight);
		}

		private void InsertText(string str, int idx)
		{
			GameObject obj = UnityEngine.Object.Instantiate(chapterText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(chapterTextPanel.transform);
			obj.transform.SetSiblingIndex(idx);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			if (idx >= _chapterTextList.Count)
			{
				_chapterTextList.Add(component);
			}
			else
			{
				_chapterTextList.Insert(idx, component);
			}
		}

		public int GetTaskTextIdx(int task, int subTask)
		{
			if (task == 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < task; i++)
			{
				num++;
			}
			return num;
		}

		public void BtnSaveActionClicked()
		{
			SaveTask(_path);
		}

		public void SaveTask(string path)
		{
			_plotRoot.RemoveAll();
			int num = 0;
			foreach (IDInfo item in _idleActionConfig)
			{
				XmlElement xmlElement = _xml.CreateElement("Action" + (num + 1));
				xmlElement.SetAttribute("StartTask", item.startTask.ToString());
				xmlElement.SetAttribute("EndTask", item.endTask.ToString());
				xmlElement.SetAttribute("RoleType", item.roleType.ToString());
				_plotRoot.AppendChild(xmlElement);
				IDActionManager.Instance.SaveAction(item, xmlElement);
				num++;
			}
			IdleActionDirector.Instance.ShowText("Save Success!");
			DebugUtils.Log(DebugType.Other, "save to " + path);
			_xml.Save(path);
			LoadIdleActionConfig(currentAction);
		}

		private void LoadIdleActionConfig(int index = 0)
		{
			_path = Application.dataPath + "/Resources/Config/IdleAction/IdleActionConfig.xml";
			_xml.Load(_path);
			_plotRoot = _xml.SelectSingleNode("IdleActionConfig");
			_xmlTaskNodes = _plotRoot.ChildNodes;
			ClearText();
			_idleActionConfig.Clear();
			int num = 0;
			foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
			{
				IDInfo iDInfo = new IDInfo();
				iDInfo.actionElem = xmlTaskNode;
				iDInfo.startTask = iDInfo.actionElem.GetAttribute("StartTask");
				iDInfo.endTask = iDInfo.actionElem.GetAttribute("EndTask");
				iDInfo.roleType = (RoleType)Enum.Parse(typeof(RoleType), iDInfo.actionElem.GetAttribute("RoleType"));
				AddText("Action " + (num + 1) + "   " + iDInfo.startTask + "~~" + iDInfo.endTask + " " + iDInfo.roleType);
				foreach (XmlElement childNode in xmlTaskNode.ChildNodes)
				{
					iDInfo.actions.Add(IdleConfigManager.Instance.GetActionDataByXML(childNode));
				}
				_idleActionConfig.Add(iDInfo);
				num++;
			}
			currentAction = index;
			SetTextColor(currentAction, Color.red);
			IDActionManager.Instance.LoadAction(_idleActionConfig[currentAction]);
			StartCoroutine(resizeContent());
		}

		private void ClearText()
		{
			_chapterTextList.Clear();
			for (int i = 0; i < chapterTextPanel.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(chapterTextPanel.transform.GetChild(i).gameObject);
			}
		}

		private void AddText(string str)
		{
			GameObject obj = UnityEngine.Object.Instantiate(chapterText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(chapterTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_chapterTextList.Add(component);
		}

		public XmlDocument GetXML()
		{
			return _xml;
		}
	}
}
