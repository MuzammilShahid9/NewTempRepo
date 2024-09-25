using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDTaskManager : MonoBehaviour
	{
		private static CDTaskManager instance;

		public GameObject plotText;

		public GameObject plotTextPanel;

		public VerticalLayoutGroup textContent;

		public Button btnLoadTask;

		public Button btnSaveTask;

		public Button btnAddPrevTask;

		public Button btnAddNextTask;

		public Button btnDelTask;

		public Button btnRunTask;

		public Button btnPrevTask;

		public Button btnNextTask;

		public int currentTask;

		public int currentSubTask;

		public int taskTotal;

		public List<Task> taskList;

		private PlotManager.PlotConfigData _currPLotData;

		private PlotRoleMoveManager.RoleMoveConfigData _currRoleMoveData;

		private PlotCameraManager.CameraConfigData _currCameraMoveData;

		private List<Text> _taskTextList = new List<Text>();

		private string _plotPath;

		private XmlNode _plotRoot;

		private XmlNodeList _xmlTaskNodes;

		private XmlDocument _xml;

		public static CDTaskManager Instance
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
			taskList = new List<Task>();
		}

		private void Start()
		{
			currentTask = 0;
			currentSubTask = 0;
		}

		private void OnEnable()
		{
			StartCoroutine(resizeContent());
		}

		public void CreateChapter(int chapter, string xmlFile)
		{
			Reset();
			CDActionManager.Instance.Reset();
			_xml = new XmlDocument();
			_xml.AppendChild(_xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			_xml.AppendChild(_xml.CreateElement("Chapter" + chapter));
			_plotRoot = _xml.SelectSingleNode("Chapter" + chapter);
			_xml.Save(xmlFile);
		}

		public void LoadChapter(string xmlFile, int chapterNum, int taskID, int subTaskID)
		{
			ClearText();
			taskList.Clear();
			CDActionManager.Instance.Reset();
			_xml.Load(xmlFile);
			_plotRoot = _xml.SelectSingleNode("Chapter" + (chapterNum + 1));
			_xmlTaskNodes = _plotRoot.ChildNodes;
			taskTotal = _plotRoot.ChildNodes.Count;
			if (taskID >= _plotRoot.ChildNodes.Count)
			{
				CinemaDirector.Instance.ShowText("No such task");
				return;
			}
			int num = 0;
			foreach (XmlElement xmlTaskNode in _xmlTaskNodes)
			{
				Task task = new Task();
				task.taskElem = xmlTaskNode;
				AddText("Task " + (num + 1));
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
							task2.actions.Add(StageManage.Instance.LoadAction(childNode));
						}
						task.subTaskList.Add(task2);
						AddText("    SubTask " + (num + 1) + "-" + (num2 + 1));
						num2++;
					}
				}
				else
				{
					task.actions = new List<CDAction>();
					foreach (XmlElement childNode2 in xmlTaskNode.ChildNodes)
					{
						task.actions.Add(StageManage.Instance.LoadAction(childNode2));
					}
				}
				taskList.Add(task);
				num++;
			}
			if (taskList[0].subTaskList == null)
			{
				SetTextColor(taskID, Color.red);
			}
			else
			{
				SetTextColor(taskID + 1, Color.red);
			}
			currentTask = taskID;
			if (taskList[taskID].subTaskList == null)
			{
				CDActionManager.Instance.Load(taskList[taskID]);
			}
			else
			{
				CDActionManager.Instance.Load(taskList[taskID].subTaskList[0]);
				currentSubTask = 0;
			}
			StartCoroutine(resizeContent());
		}

		public void BtnAddPrevTaskClicked()
		{
			if (taskList.Count == 0)
			{
				InsertTask(0);
			}
			else
			{
				InsertTask(currentTask);
			}
		}

		public void BtnAddNextTaskClicked()
		{
			if (taskList.Count == 0)
			{
				InsertTask(0);
			}
			else
			{
				InsertTask(currentTask + 1);
			}
		}

		public void InsertTask(int taskIdx)
		{
			Task task = new Task();
			task.actions = new List<CDAction>();
			if (taskList.Count > 0 && taskIdx < taskList.Count)
			{
				taskList.Insert(taskIdx, task);
				currentTask = taskIdx;
			}
			if (taskList.Count > 0 && taskIdx == taskList.Count)
			{
				taskList.Add(task);
				currentTask = taskIdx;
			}
			else if (taskList.Count == 0)
			{
				taskList.Add(task);
				currentTask = 0;
			}
			int taskTextIdx = GetTaskTextIdx(taskIdx, 0);
			InsertText("Task " + taskIdx, taskTextIdx);
			SetTextColor(taskTextIdx, Color.red);
			currentTask = taskIdx;
			currentSubTask = 0;
			taskTotal++;
			CDActionManager.Instance.Load(task);
			RefreshTask();
		}

		private IEnumerator resizeContent()
		{
			yield return null;
			textContent.GetComponent<RectTransform>().sizeDelta = new Vector2(textContent.GetComponent<RectTransform>().sizeDelta.x, textContent.preferredHeight);
		}

		public void BtnAddSubTaskClicked()
		{
			if (taskList.Count == 0)
			{
				CinemaDirector.Instance.ShowText("Subtask must add to task");
				return;
			}
			if (taskList[currentTask].subTaskList == null && taskList[currentTask].actions.Count > 0)
			{
				CinemaDirector.Instance.ShowText("Can not add SubTask to non empty task");
				return;
			}
			Task task = new Task();
			task.actions = new List<CDAction>();
			if (taskList[currentTask].subTaskList == null)
			{
				taskList[currentTask].subTaskList = new List<Task>();
			}
			else
			{
				currentSubTask++;
			}
			taskList[currentTask].subTaskList.Add(task);
			GetTaskTextIdx(currentTask, currentSubTask);
			InsertText("    SubTask " + (currentTask + 1) + "-" + (currentSubTask + 1).ToString(), GetTaskTextIdx(currentTask, currentSubTask));
			SetTextColor(GetTaskTextIdx(currentTask, currentSubTask), Color.red);
			CDActionManager.Instance.Load(task);
			StartCoroutine(resizeContent());
		}

		public int GetTaskTextIdx(int task, int subTask)
		{
			if (task == 0 && taskList[task].subTaskList != null)
			{
				return subTask + 1;
			}
			if (task == 0 && taskList[task].subTaskList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < task; i++)
			{
				num++;
				if (taskList[i].subTaskList != null)
				{
					num += taskList[i].subTaskList.Count;
				}
			}
			if (taskList[task].subTaskList == null)
			{
				return num;
			}
			return num + subTask + 1;
		}

		public void RefreshTask()
		{
			for (int i = 0; i < taskList.Count; i++)
			{
				if (taskList[i].subTaskList != null)
				{
					GetTaskTextIdx(i, 0);
					_taskTextList[GetTaskTextIdx(i, 0) - 1].text = "Task " + (i + 1);
					for (int j = 0; j < taskList[i].subTaskList.Count; j++)
					{
						_taskTextList[GetTaskTextIdx(i, j)].text = "    SubTask " + (i + 1) + "-" + (j + 1);
					}
				}
				else
				{
					_taskTextList[GetTaskTextIdx(i, 0)].text = "Task " + (i + 1);
				}
			}
			StartCoroutine(resizeContent());
		}

		public void BtnPrevTaskClicked()
		{
			CDActionConfig.Instance.CloseAll();
			if (taskList[currentTask].subTaskList == null)
			{
				if (currentTask > 0)
				{
					currentTask--;
					int taskTextIdx = GetTaskTextIdx(currentTask, currentSubTask);
					if (taskList[currentTask].subTaskList != null)
					{
						CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
					}
					else
					{
						CDActionManager.Instance.Load(taskList[currentTask]);
					}
					SetTextColor(taskTextIdx, Color.red);
				}
			}
			else if (currentSubTask > 0)
			{
				currentSubTask--;
				int taskTextIdx2 = GetTaskTextIdx(currentTask, currentSubTask);
				SetTextColor(taskTextIdx2, Color.red);
				CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
			}
			else if (currentSubTask == 0 && currentTask > 0)
			{
				currentTask--;
				currentSubTask = 0;
				if (taskList[currentTask].subTaskList != null)
				{
					CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
					int taskTextIdx3 = GetTaskTextIdx(currentTask, currentSubTask);
					SetTextColor(taskTextIdx3, Color.red);
				}
				else
				{
					int taskTextIdx4 = GetTaskTextIdx(currentTask, currentSubTask);
					SetTextColor(taskTextIdx4, Color.red);
					CDActionManager.Instance.Load(taskList[currentTask]);
				}
			}
		}

		public void BtnNextTaskClicked()
		{
			CDActionConfig.Instance.CloseAll();
			if (taskList[currentTask].subTaskList == null)
			{
				if (currentTask < taskTotal - 1)
				{
					currentTask++;
					int taskTextIdx = GetTaskTextIdx(currentTask, currentSubTask);
					if (taskList[currentTask].subTaskList != null)
					{
						CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
					}
					else
					{
						CDActionManager.Instance.Load(taskList[currentTask]);
					}
					SetTextColor(taskTextIdx, Color.red);
				}
			}
			else if (currentSubTask < taskList[currentTask].subTaskList.Count - 1)
			{
				currentSubTask++;
				int taskTextIdx2 = GetTaskTextIdx(currentTask, currentSubTask);
				SetTextColor(taskTextIdx2, Color.red);
				CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
			}
			else if (currentTask < taskTotal - 1)
			{
				currentTask++;
				currentSubTask = 0;
				if (taskList[currentTask].subTaskList != null)
				{
					CDActionManager.Instance.Load(taskList[currentTask].subTaskList[currentSubTask]);
					int taskTextIdx3 = GetTaskTextIdx(currentTask, currentSubTask);
					SetTextColor(taskTextIdx3, Color.red);
				}
				else
				{
					int taskTextIdx4 = GetTaskTextIdx(currentTask, currentSubTask);
					SetTextColor(taskTextIdx4, Color.red);
					CDActionManager.Instance.Load(taskList[currentTask]);
				}
			}
			DebugUtils.Log(DebugType.Other, "current task " + currentTask);
		}

		public Task GetCurrentTask()
		{
			if (taskList[currentTask].subTaskList == null)
			{
				return taskList[currentTask];
			}
			return taskList[currentTask].subTaskList[currentSubTask];
		}

		public void BtnDelTaskClicked()
		{
			if (taskList.Count > 0)
			{
				if (taskList[currentTask].subTaskList != null)
				{
					taskList[currentTask].subTaskList.RemoveAt(currentSubTask);
					int taskTextIdx = GetTaskTextIdx(currentTask, currentSubTask);
					DelText(GetTaskTextIdx(currentTask, currentSubTask));
					currentSubTask--;
					if (currentSubTask < 0 && taskList[currentTask].subTaskList.Count == 0)
					{
						currentSubTask = 0;
						DelText(GetTaskTextIdx(currentTask, currentSubTask) - 1);
						taskList.RemoveAt(currentTask);
						if (currentTask == taskList.Count && taskList.Count > 0)
						{
							currentTask--;
						}
						taskTotal--;
					}
					else if (currentSubTask < 0 && taskList[currentTask].subTaskList.Count > 0)
					{
						currentSubTask = 0;
					}
					if (taskList.Count > 0)
					{
						taskTextIdx = GetTaskTextIdx(currentTask, currentSubTask);
						SetTextColor(taskTextIdx, Color.red);
					}
				}
				else
				{
					int taskTextIdx2 = GetTaskTextIdx(currentTask, currentSubTask);
					DelText(GetTaskTextIdx(currentTask, currentSubTask));
					if (taskTextIdx2 > 0)
					{
						SetTextColor(taskTextIdx2 - 1, Color.red);
						taskList.RemoveAt(currentTask);
						currentTask--;
					}
					else if (taskTextIdx2 == 0 && taskList.Count > 0)
					{
						SetTextColor(taskTextIdx2, Color.red);
						taskList.RemoveAt(taskTextIdx2);
					}
					taskTotal--;
				}
			}
			RefreshTask();
		}

		public void BtnRunTaskClicked()
		{
			CastleSceneManager.Instance.HideUI();
			CinemaDirector.Instance.gameObject.SetActive(false);
			PlotManager.Instance.StartNewPlot(GetCurrentTask());
		}

		private void AddText(string str)
		{
			GameObject obj = Object.Instantiate(plotText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(plotTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_taskTextList.Add(component);
		}

		private void InsertText(string str, int idx)
		{
			GameObject obj = Object.Instantiate(plotText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(plotTextPanel.transform);
			obj.transform.SetSiblingIndex(idx);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			if (idx >= _taskTextList.Count)
			{
				_taskTextList.Add(component);
			}
			else
			{
				_taskTextList.Insert(idx, component);
			}
		}

		private void DelText(int idx)
		{
			Object.Destroy(_taskTextList[idx].gameObject);
			_taskTextList.RemoveAt(idx);
		}

		private void ClearText()
		{
			_taskTextList.Clear();
			for (int i = 0; i < plotTextPanel.transform.childCount; i++)
			{
				Object.Destroy(plotTextPanel.transform.GetChild(i).gameObject);
			}
		}

		private void ResetColor()
		{
			foreach (Text taskText in _taskTextList)
			{
				taskText.color = Color.black;
			}
		}

		private void SetTextColor(int idx, Color color)
		{
			if (_taskTextList.Count != 0)
			{
				ResetColor();
				_taskTextList[idx].color = color;
			}
		}

		public void SaveTask(string path)
		{
			_plotRoot.RemoveAll();
			int num = 0;
			foreach (Task task in taskList)
			{
				XmlElement xmlElement = _xml.CreateElement("Task" + (num + 1));
				_plotRoot.AppendChild(xmlElement);
				if (task.subTaskList != null)
				{
					foreach (Task subTask in task.subTaskList)
					{
						XmlElement xmlElement2 = _xml.CreateElement("SubTask");
						xmlElement.AppendChild(xmlElement2);
						CDActionManager.Instance.SaveAction(subTask, xmlElement2);
					}
				}
				else
				{
					CDActionManager.Instance.SaveAction(task, xmlElement);
				}
				num++;
			}
			CinemaDirector.Instance.ShowText("Save Success!");
			DebugUtils.Log(DebugType.Other, "save to " + path);
			_xml.Save(path);
		}

		public void Reset()
		{
			ClearText();
			currentTask = 0;
			currentSubTask = 0;
			taskList.Clear();
		}

		public XmlDocument GetXML()
		{
			return _xml;
		}

		public void SaveXML()
		{
			_xml.Save(_plotPath);
		}
	}
}
