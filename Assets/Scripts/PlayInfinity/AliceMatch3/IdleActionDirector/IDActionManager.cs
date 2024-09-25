using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDActionManager : MonoBehaviour
	{
		public class Action
		{
			public IDAction actions;

			public XmlElement taskElem;
		}

		private static IDActionManager instance;

		public GameObject plotText;

		public GameObject plotTextPanel;

		public int currentStep;

		public List<Action> actionList;

		public VerticalLayoutGroup textContent;

		private int totalStep;

		private IDInfo currAction;

		private List<Text> _taskTextList = new List<Text>();

		public static IDActionManager Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			actionList = new List<Action>();
		}

		private void Start()
		{
			currentStep = 0;
		}

		public void LoadAction(IDInfo tempAction)
		{
			ClearText();
			actionList.Clear();
			currAction = tempAction;
			for (int i = 0; i < currAction.actions.Count; i++)
			{
				Action action = new Action();
				action.taskElem = currAction.actions[i].actionElem;
				action.actions = currAction.actions[i];
				AddText("Step " + (i + 1));
				actionList.Add(action);
			}
			totalStep = currAction.actions.Count;
			currentStep = 0;
			SetTextColor(currentStep, Color.red);
			if (actionList.Count > 0)
			{
				IDStepManager.Instance.Load(actionList[currentStep]);
			}
			else
			{
				Action action2 = new Action();
				action2.actions = IDStepManager.Instance.CreateAction();
				IDStepManager.Instance.Load(action2);
			}
			StartCoroutine(resizeContent());
		}

		private void ClearText()
		{
			_taskTextList.Clear();
			for (int i = 0; i < plotTextPanel.transform.childCount; i++)
			{
				Object.Destroy(plotTextPanel.transform.GetChild(i).gameObject);
			}
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

		public void BtnPrevTaskClicked()
		{
			if (currentStep > 0)
			{
				currentStep--;
				SetTextColor(currentStep, Color.red);
				IDStepManager.Instance.Load(actionList[currentStep]);
			}
		}

		public void BtnNextTaskClicked()
		{
			if (currentStep < actionList.Count - 1)
			{
				currentStep++;
				SetTextColor(currentStep, Color.red);
				IDStepManager.Instance.Load(actionList[currentStep]);
			}
		}

		public void BtnRunClick()
		{
			IdleManager.Instance.StartIdleActionWithActionInfo(actionList[currentStep], currAction.roleType);
		}

		public void BtnAddPrevTaskClicked()
		{
			if (actionList.Count == 0)
			{
				InsertAction(0);
			}
			else
			{
				InsertAction(currentStep);
			}
		}

		public void BtnAddNextTaskClicked()
		{
			if (actionList.Count == 0)
			{
				InsertAction(0);
			}
			else
			{
				InsertAction(currentStep + 1);
			}
		}

		public void InsertAction(int index)
		{
			IDAction iDAction = IDStepManager.Instance.CreateAction();
			if (IDInfoManager.Instance.GetCurrIDInfo().actions.Count > 0 && index < IDInfoManager.Instance.GetCurrIDInfo().actions.Count)
			{
				IDInfoManager.Instance.GetCurrIDInfo().actions.Insert(index, iDAction);
				Action action = new Action();
				action.taskElem = iDAction.actionElem;
				action.actions = iDAction;
				actionList.Insert(index, action);
				currentStep = index;
			}
			if (IDInfoManager.Instance.GetCurrIDInfo().actions.Count > 0 && index == IDInfoManager.Instance.GetCurrIDInfo().actions.Count)
			{
				IDInfoManager.Instance.GetCurrIDInfo().actions.Add(iDAction);
				Action action2 = new Action();
				action2.taskElem = iDAction.actionElem;
				action2.actions = iDAction;
				actionList.Add(action2);
				currentStep = index;
			}
			else if (IDInfoManager.Instance.GetCurrIDInfo().actions.Count == 0)
			{
				IDInfoManager.Instance.GetCurrIDInfo().actions.Add(iDAction);
				Action action3 = new Action();
				action3.taskElem = iDAction.actionElem;
				action3.actions = iDAction;
				actionList.Add(action3);
				currentStep = 0;
			}
			InsertText("Step " + (index + 1), index + 1);
			SetTextColor(index, Color.red);
			RefreshAction();
			IDStepManager.Instance.LoadConfigs(iDAction);
		}

		public void BtnDelActionClicked()
		{
			IDAction action = null;
			if (IDInfoManager.Instance.GetCurrIDInfo().actions.Count <= 0)
			{
				return;
			}
			DelText(currentStep);
			if (currentStep > 0)
			{
				SetTextColor(currentStep - 1, Color.red);
				IDInfoManager.Instance.GetCurrIDInfo().actions.RemoveAt(currentStep);
				currentStep--;
				action = IDInfoManager.Instance.GetCurrIDInfo().actions[currentStep];
			}
			else if (currentStep == 0 && IDInfoManager.Instance.GetCurrIDInfo().actions.Count > 0)
			{
				IDInfoManager.Instance.GetCurrIDInfo().actions.RemoveAt(currentStep);
				if (IDInfoManager.Instance.GetCurrIDInfo().actions.Count > 0)
				{
					SetTextColor(currentStep, Color.red);
					action = IDInfoManager.Instance.GetCurrIDInfo().actions[currentStep];
				}
			}
			RefreshAction();
			IDStepManager.Instance.LoadConfigs(action);
		}

		private void DelText(int line)
		{
			Object.Destroy(_taskTextList[line].gameObject);
			_taskTextList.RemoveAt(line);
		}

		public void RefreshAction()
		{
			for (int i = 0; i < IDInfoManager.Instance.GetCurrIDInfo().actions.Count; i++)
			{
				_taskTextList[i].text = "Step " + (i + 1);
			}
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

		public Action GetCurrentTask()
		{
			return actionList[currentStep];
		}

		private void SetTextColor(int idx, Color color)
		{
			if (_taskTextList.Count != 0)
			{
				ResetColor();
				_taskTextList[idx].color = color;
			}
		}

		private void ResetColor()
		{
			foreach (Text taskText in _taskTextList)
			{
				taskText.color = Color.black;
			}
		}

		private IEnumerator resizeContent()
		{
			yield return null;
			textContent.GetComponent<RectTransform>().sizeDelta = new Vector2(textContent.GetComponent<RectTransform>().sizeDelta.x, textContent.preferredHeight);
		}

		public void SaveAction(IDInfo task, XmlElement taskElem)
		{
			if (task.actions.Count == 0)
			{
				return;
			}
			for (int i = 0; i < task.actions.Count; i++)
			{
				IDAction iDAction = task.actions[i];
				XmlElement xmlElement = IDInfoManager.Instance.GetXML().CreateElement("Step" + (i + 1));
				xmlElement.SetAttribute("Time", iDAction.tm.ToString());
				taskElem.AppendChild(xmlElement);
				if (iDAction.roleConfig.isSet && iDAction.roleConfig.roles.Count > 0)
				{
					XmlElement xmlElement2 = null;
					bool flag = false;
					foreach (RoleType key in iDAction.roleConfig.roles.Keys)
					{
						if (iDAction.roleConfig.roles[key].anim != null && iDAction.roleConfig.roles[key].anim.Count > 0)
						{
							xmlElement2 = IDInfoManager.Instance.GetXML().CreateElement("Role");
							xmlElement.AppendChild(xmlElement2);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						foreach (RoleType key2 in iDAction.roleConfig.roles.Keys)
						{
							if (iDAction.roleConfig.roles[key2].anim == null || iDAction.roleConfig.roles[key2].anim.Count <= 0)
							{
								continue;
							}
							if (!flag)
							{
								xmlElement2 = IDInfoManager.Instance.GetXML().CreateElement("Role");
								xmlElement.AppendChild(xmlElement2);
								flag = true;
							}
							XmlElement xmlElement3 = IDInfoManager.Instance.GetXML().CreateElement(key2.ToString());
							xmlElement2.AppendChild(xmlElement3);
							string text = "";
							foreach (string item in iDAction.roleConfig.roles[key2].anim)
							{
								text = text + item + ";";
							}
							xmlElement3.SetAttribute("Anim", text.Substring(0, text.Length - 1));
						}
					}
				}
				if (iDAction.convConfig.isSet && iDAction.convConfig.convList.Count > 0)
				{
					XmlElement xmlElement4 = IDInfoManager.Instance.GetXML().CreateElement("Conversation");
					xmlElement.AppendChild(xmlElement4);
					string text2 = "";
					foreach (string conv in iDAction.convConfig.convList)
					{
						text2 = text2 + conv + ";";
					}
					if (text2 != "")
					{
						xmlElement4.SetAttribute("Content", text2.Substring(0, text2.Length - 1));
					}
				}
				if (iDAction.buildConfig.isSet)
				{
					XmlElement xmlElement5 = IDInfoManager.Instance.GetXML().CreateElement("Build");
					xmlElement.AppendChild(xmlElement5);
					string text3 = "";
					text3 = text3 + iDAction.buildConfig.roomID + ";";
					text3 = text3 + iDAction.buildConfig.itemID + ";";
					text3 += iDAction.buildConfig.stageID;
					xmlElement5.SetAttribute("Data", text3);
				}
				if (iDAction.otherConfig.isSet)
				{
					XmlElement xmlElement6 = IDInfoManager.Instance.GetXML().CreateElement("Other");
					xmlElement.AppendChild(xmlElement6);
					if (iDAction.otherConfig.isChapterEnd)
					{
						xmlElement6.SetAttribute("IsChapterEnd", iDAction.otherConfig.isChapterEnd.ToString());
					}
					if (iDAction.otherConfig.isCatRename)
					{
						xmlElement6.SetAttribute("IsCatRename", iDAction.otherConfig.isCatRename.ToString());
					}
					if (iDAction.otherConfig.isCastleRename)
					{
						xmlElement6.SetAttribute("IsCastleRename", iDAction.otherConfig.isCastleRename.ToString());
					}
					if (iDAction.otherConfig.isBlackScreen)
					{
						xmlElement6.SetAttribute("IsBlackScreen", iDAction.otherConfig.isBlackScreen.ToString());
					}
				}
				if (iDAction.audioConfig.isSet)
				{
					XmlElement xmlElement7 = IDInfoManager.Instance.GetXML().CreateElement("Audio");
					xmlElement.AppendChild(xmlElement7);
					if (iDAction.audioConfig.isMusicSet)
					{
						xmlElement7.SetAttribute("IsMusicSet", iDAction.audioConfig.isMusicSet.ToString());
						xmlElement7.SetAttribute("MusicName", iDAction.audioConfig.musicName.ToString());
						xmlElement7.SetAttribute("IsMusicLoop", iDAction.audioConfig.isMusicLoop.ToString());
						xmlElement7.SetAttribute("IsMusicStop", iDAction.audioConfig.isMusicStop.ToString());
						xmlElement7.SetAttribute("MusicMinTime", iDAction.audioConfig.musicMinTime.ToString());
						xmlElement7.SetAttribute("MusicMaxTime", iDAction.audioConfig.musicMaxTime.ToString());
					}
					if (iDAction.audioConfig.isEffectSet)
					{
						xmlElement7.SetAttribute("IsEffectSet", iDAction.audioConfig.isEffectSet.ToString());
						xmlElement7.SetAttribute("EffectName", iDAction.audioConfig.effectName.ToString());
						xmlElement7.SetAttribute("IsEffectLoop", iDAction.audioConfig.isEffectLoop.ToString());
						xmlElement7.SetAttribute("IsEffectStop", iDAction.audioConfig.isEffectStop.ToString());
						xmlElement7.SetAttribute("EffectMinTime", iDAction.audioConfig.effectMinTime.ToString());
						xmlElement7.SetAttribute("EffectMaxTime", iDAction.audioConfig.effectMaxTime.ToString());
					}
				}
				if (iDAction.delayConfig.isSet)
				{
					XmlElement xmlElement8 = IDInfoManager.Instance.GetXML().CreateElement("Delay");
					xmlElement.AppendChild(xmlElement8);
					xmlElement8.SetAttribute("DelayTime", iDAction.delayConfig.delayTime.ToString());
				}
			}
			IDStepManager.Instance.UpdateActionInfo();
		}
	}
}
