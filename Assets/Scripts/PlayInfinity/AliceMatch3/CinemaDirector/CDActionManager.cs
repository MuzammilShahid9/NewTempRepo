using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDActionManager : MonoBehaviour
	{
		private static CDActionManager instance;

		public Button btnPrevAction;

		public Button btnNextAction;

		public Button btnAddPrevAction;

		public Button btnAddNextAction;

		public Button btnDelAction;

		public Button btnRunAction;

		public GameObject actionText;

		public GameObject actionTextPanel;

		public GameObject actionPreviewText;

		public GameObject actionPreviewTextPanel;

		private PlotRoleMoveManager.RoleMoveConfigData _currRoleMoveData;

		private PlotCameraManager.CameraConfigData _currCameraMoveData;

		public PlotDialogManager.DialogConfigData _currDialogData;

		private PlotRoleAniManager.RoleAniConfigData _currRoleAniData;

		public Role currentRole;

		private List<Text> _actionTextList = new List<Text>();

		public int currentActionIdx = -1;

		public CDAction currentAction;

		public static CDActionManager Instance
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
		}

		public void Load(Task task)
		{
			ClearText();
			if (task.actions.Count > 0)
			{
				for (int i = 0; i < task.actions.Count; i++)
				{
					AddText("Action " + (i + 1));
				}
				currentActionIdx = 0;
				currentAction = CDTaskManager.Instance.GetCurrentTask().actions[0];
				SetTextColor(0, Color.red);
				if (currentAction != null)
				{
					LoadConfigs(currentAction);
				}
			}
			else
			{
				currentActionIdx = -1;
				currentAction = null;
				CDActionConfig.Instance.UnselectAllToggle();
			}
			UpdateActionInfo();
		}

		public void LoadAction(CDAction action)
		{
			if (action != null)
			{
				LoadConfigs(action);
			}
		}

		public void LoadConfigs(CDAction action)
		{
			CDActionConfig.Instance.actionTime.text = action.tm.ToString();
			CDActionConfig.Instance.panelActive[0].isOn = action.camConfig.isSet;
			CDActionConfig.Instance.panelActive[1].isOn = action.roleConfig.isSet;
			CDActionConfig.Instance.panelActive[2].isOn = action.convConfig.isSet;
			CDActionConfig.Instance.panelActive[3].isOn = action.buildConfig.isSet;
			CDActionConfig.Instance.panelActive[4].isOn = action.otherConfig.isSet;
			CDActionConfig.Instance.panelActive[5].isOn = action.audioConfig.isSet;
			CDActionConfig.Instance.CloseAll();
		}

		private void AddText(string str)
		{
			GameObject obj = Object.Instantiate(actionText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(actionTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_actionTextList.Add(component);
		}

		private void InsertText(string str, int idx)
		{
			GameObject obj = Object.Instantiate(actionText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(actionTextPanel.transform);
			obj.transform.SetSiblingIndex(idx);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			if (idx >= _actionTextList.Count)
			{
				_actionTextList.Add(component);
			}
			else
			{
				_actionTextList.Insert(idx, component);
			}
		}

		private void DelText(int line)
		{
			Object.Destroy(_actionTextList[line].gameObject);
			_actionTextList.RemoveAt(line);
		}

		private void ClearText()
		{
			_actionTextList.Clear();
			for (int i = 0; i < actionTextPanel.transform.childCount; i++)
			{
				Object.Destroy(actionTextPanel.transform.GetChild(i).gameObject);
			}
		}

		private void SetTextColor(int idx, Color color)
		{
			ResetColor();
			_actionTextList[idx].color = color;
		}

		private void ResetColor()
		{
			foreach (Text actionText in _actionTextList)
			{
				actionText.color = Color.black;
			}
		}

		private void AddPreviewText(string str)
		{
			GameObject obj = Object.Instantiate(actionPreviewText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(actionPreviewTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
		}

		private void ClearPreviewText()
		{
			for (int i = 0; i < actionPreviewTextPanel.transform.childCount; i++)
			{
				Object.Destroy(actionPreviewTextPanel.transform.GetChild(i).gameObject);
			}
		}

		public void BtnRunActionClicked()
		{
			CastleSceneManager.Instance.HideUI();
			CinemaDirector.Instance.gameObject.SetActive(false);
			PlotManager.Instance.DealSingleAction(currentAction);
		}

		public void NextActionClicked()
		{
			DebugUtils.Log(DebugType.Other, "currentActionIdx " + currentActionIdx);
			DebugUtils.Log(DebugType.Other, "_actionTextList.Count " + _actionTextList.Count);
			if (currentActionIdx < _actionTextList.Count - 1)
			{
				SavePanelSetStaus();
				currentActionIdx++;
				currentAction = CDTaskManager.Instance.GetCurrentTask().actions[currentActionIdx];
				LoadAction(currentAction);
				ResetColor();
				_actionTextList[currentActionIdx].color = Color.red;
				UpdateActionInfo();
			}
		}

		public void PrevActionClicked()
		{
			DebugUtils.Log(DebugType.Other, "currentActionIdx " + currentActionIdx);
			DebugUtils.Log(DebugType.Other, "_actionTextList.Count " + _actionTextList.Count);
			if (currentActionIdx > 0)
			{
				SavePanelSetStaus();
				currentActionIdx--;
				currentAction = CDTaskManager.Instance.GetCurrentTask().actions[currentActionIdx];
				LoadAction(currentAction);
				ResetColor();
				_actionTextList[currentActionIdx].color = Color.red;
				UpdateActionInfo();
			}
		}

		public void SavePanelSetStaus()
		{
			currentAction.camConfig.isSet = CDActionConfig.Instance.panelActive[0].isOn;
			currentAction.roleConfig.isSet = CDActionConfig.Instance.panelActive[1].isOn;
			currentAction.convConfig.isSet = CDActionConfig.Instance.panelActive[2].isOn;
			currentAction.buildConfig.isSet = CDActionConfig.Instance.panelActive[3].isOn;
			currentAction.otherConfig.isSet = CDActionConfig.Instance.panelActive[4].isOn;
			currentAction.audioConfig.isSet = CDActionConfig.Instance.panelActive[5].isOn;
		}

		public void BtnClearActionClicked()
		{
			ClearText();
			CDTaskManager.Instance.GetCurrentTask().actions.Clear();
			currentActionIdx = -1;
		}

		public CDAction CreateAction()
		{
			CDAction cDAction = new CDAction();
			cDAction.tm = -1f;
			cDAction.camConfig = new CDCameraConfig();
			cDAction.camConfig.isSet = false;
			cDAction.camConfig.tm = 0.5f;
			cDAction.roleConfig = new CDRoleConfig();
			cDAction.roleConfig.isSet = false;
			cDAction.roleConfig.roles = new Dictionary<RoleType, CDRoleAnim>();
			cDAction.convConfig = new CDConversationConfig();
			cDAction.convConfig.isSet = false;
			cDAction.convConfig.convList = new List<string>();
			cDAction.buildConfig = new CDBuildConfig();
			cDAction.buildConfig.isSet = false;
			cDAction.otherConfig = new CDOtherConfig();
			cDAction.otherConfig.isSet = false;
			cDAction.audioConfig = new CDAudioConfig();
			cDAction.audioConfig.isSet = false;
			return cDAction;
		}

		public void BtnAddNextActionClicked()
		{
			if (CDTaskManager.Instance.GetCurrentTask().actions.Count == 0)
			{
				InsertAction(0);
			}
			else
			{
				InsertAction(currentActionIdx + 1);
			}
			UpdateActionInfo();
		}

		public void BtnAddPrevActionClicked()
		{
			if (CDTaskManager.Instance.GetCurrentTask().actions.Count == 0)
			{
				InsertAction(0);
			}
			else
			{
				InsertAction(currentActionIdx);
			}
		}

		public void InsertAction(int index)
		{
			CDAction item = (currentAction = CreateAction());
			if (CDTaskManager.Instance.GetCurrentTask().actions.Count > 0 && index < CDTaskManager.Instance.GetCurrentTask().actions.Count)
			{
				CDTaskManager.Instance.GetCurrentTask().actions.Insert(index, item);
				currentActionIdx = index;
			}
			if (CDTaskManager.Instance.GetCurrentTask().actions.Count > 0 && index == CDTaskManager.Instance.GetCurrentTask().actions.Count)
			{
				CDTaskManager.Instance.GetCurrentTask().actions.Add(item);
				currentActionIdx = index;
			}
			else if (CDTaskManager.Instance.GetCurrentTask().actions.Count == 0)
			{
				CDTaskManager.Instance.GetCurrentTask().actions.Add(item);
				currentActionIdx = 0;
			}
			InsertText("Action " + (index + 1), index + 1);
			SetTextColor(index, Color.red);
			RefreshAction();
			LoadConfigs(currentAction);
		}

		public void BtnDelActionClicked()
		{
			if (CDTaskManager.Instance.GetCurrentTask().actions.Count <= 0)
			{
				return;
			}
			DelText(currentActionIdx);
			if (currentActionIdx > 0)
			{
				SetTextColor(currentActionIdx - 1, Color.red);
				CDTaskManager.Instance.GetCurrentTask().actions.RemoveAt(currentActionIdx);
				currentActionIdx--;
				currentAction = CDTaskManager.Instance.GetCurrentTask().actions[currentActionIdx];
			}
			else if (currentActionIdx == 0 && CDTaskManager.Instance.GetCurrentTask().actions.Count > 0)
			{
				CDTaskManager.Instance.GetCurrentTask().actions.RemoveAt(currentActionIdx);
				if (CDTaskManager.Instance.GetCurrentTask().actions.Count > 0)
				{
					SetTextColor(currentActionIdx, Color.red);
					currentAction = CDTaskManager.Instance.GetCurrentTask().actions[currentActionIdx];
				}
			}
			RefreshAction();
			LoadConfigs(currentAction);
		}

		public void SaveAction(Task task, XmlElement taskElem)
		{
			if (task.actions.Count == 0)
			{
				return;
			}
			for (int i = 0; i < task.actions.Count; i++)
			{
				CDAction cDAction = task.actions[i];
				XmlElement xmlElement = CDTaskManager.Instance.GetXML().CreateElement("Action" + (i + 1));
				xmlElement.SetAttribute("Time", cDAction.tm.ToString());
				taskElem.AppendChild(xmlElement);
				if (cDAction.camConfig.isSet)
				{
					XmlElement xmlElement2 = CDTaskManager.Instance.GetXML().CreateElement("Camera");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("Pos", cDAction.camConfig.pos.ToString());
					xmlElement2.SetAttribute("Size", cDAction.camConfig.size.ToString());
					xmlElement2.SetAttribute("Time", cDAction.camConfig.tm.ToString());
					xmlElement2.SetAttribute("IsFollow", cDAction.camConfig.isFollow.ToString());
					xmlElement2.SetAttribute("FollowTarget", cDAction.camConfig.followRole.ToString());
				}
				if (cDAction.roleConfig.isSet && cDAction.roleConfig.roles.Count > 0)
				{
					XmlElement xmlElement3 = null;
					bool flag = false;
					foreach (RoleType key in cDAction.roleConfig.roles.Keys)
					{
						if (cDAction.roleConfig.roles[key].anim != null && cDAction.roleConfig.roles[key].anim.Count > 0)
						{
							xmlElement3 = CDTaskManager.Instance.GetXML().CreateElement("Role");
							xmlElement.AppendChild(xmlElement3);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						foreach (RoleType key2 in cDAction.roleConfig.roles.Keys)
						{
							if (cDAction.roleConfig.roles[key2].anim == null || cDAction.roleConfig.roles[key2].anim.Count <= 0)
							{
								continue;
							}
							if (!flag)
							{
								xmlElement3 = CDTaskManager.Instance.GetXML().CreateElement("Role");
								xmlElement.AppendChild(xmlElement3);
								flag = true;
							}
							XmlElement xmlElement4 = CDTaskManager.Instance.GetXML().CreateElement(key2.ToString());
							xmlElement3.AppendChild(xmlElement4);
							string text = "";
							foreach (string item in cDAction.roleConfig.roles[key2].anim)
							{
								text = text + item + ";";
							}
							xmlElement4.SetAttribute("Anim", text.Substring(0, text.Length - 1));
						}
					}
				}
				if (cDAction.convConfig.isSet && cDAction.convConfig.convList.Count > 0)
				{
					XmlElement xmlElement5 = CDTaskManager.Instance.GetXML().CreateElement("Conversation");
					xmlElement.AppendChild(xmlElement5);
					string text2 = "";
					foreach (string conv in cDAction.convConfig.convList)
					{
						text2 = text2 + conv + ";";
					}
					if (text2 != "")
					{
						xmlElement5.SetAttribute("Content", text2.Substring(0, text2.Length - 1));
					}
				}
				if (cDAction.buildConfig.isSet)
				{
					XmlElement xmlElement6 = CDTaskManager.Instance.GetXML().CreateElement("Build");
					xmlElement.AppendChild(xmlElement6);
					string text3 = "";
					text3 = text3 + cDAction.buildConfig.roomID + ";";
					text3 = text3 + cDAction.buildConfig.itemID + ";";
					text3 = text3 + cDAction.buildConfig.stageID + ";";
					text3 += cDAction.buildConfig.delayTime;
					xmlElement6.SetAttribute("Data", text3);
				}
				if (cDAction.otherConfig.isSet)
				{
					XmlElement xmlElement7 = CDTaskManager.Instance.GetXML().CreateElement("Other");
					xmlElement.AppendChild(xmlElement7);
					if (cDAction.otherConfig.isChapterEnd)
					{
						xmlElement7.SetAttribute("IsChapterEnd", cDAction.otherConfig.isChapterEnd.ToString());
					}
					if (cDAction.otherConfig.isCatRename)
					{
						xmlElement7.SetAttribute("IsCatRename", cDAction.otherConfig.isCatRename.ToString());
					}
					if (cDAction.otherConfig.isCastleRename)
					{
						xmlElement7.SetAttribute("IsCastleRename", cDAction.otherConfig.isCastleRename.ToString());
					}
					if (cDAction.otherConfig.isBlackScreen)
					{
						xmlElement7.SetAttribute("IsBlackScreen", cDAction.otherConfig.isBlackScreen.ToString());
					}
					if (cDAction.otherConfig.unlockRoomID != 0)
					{
						xmlElement7.SetAttribute("UnlockRoomID", cDAction.otherConfig.unlockRoomID.ToString());
					}
				}
				if (cDAction.audioConfig.isSet)
				{
					XmlElement xmlElement8 = CDTaskManager.Instance.GetXML().CreateElement("Audio");
					xmlElement.AppendChild(xmlElement8);
					if (cDAction.audioConfig.isMusicSet)
					{
						xmlElement8.SetAttribute("IsMusicSet", cDAction.audioConfig.isMusicSet.ToString());
						xmlElement8.SetAttribute("MusicName", cDAction.audioConfig.musicName.ToString());
						xmlElement8.SetAttribute("IsMusicLoop", cDAction.audioConfig.isMusicLoop.ToString());
						xmlElement8.SetAttribute("IsMusicStop", cDAction.audioConfig.isMusicStop.ToString());
						xmlElement8.SetAttribute("MusicMinTime", cDAction.audioConfig.musicMinTime.ToString());
						xmlElement8.SetAttribute("MusicMaxTime", cDAction.audioConfig.musicMaxTime.ToString());
					}
					if (cDAction.audioConfig.isEffectSet)
					{
						xmlElement8.SetAttribute("IsEffectSet", cDAction.audioConfig.isEffectSet.ToString());
						xmlElement8.SetAttribute("EffectName", cDAction.audioConfig.effectName.ToString());
						xmlElement8.SetAttribute("IsEffectLoop", cDAction.audioConfig.isEffectLoop.ToString());
						xmlElement8.SetAttribute("IsEffectStop", cDAction.audioConfig.isEffectStop.ToString());
						xmlElement8.SetAttribute("EffectMinTime", cDAction.audioConfig.effectMinTime.ToString());
						xmlElement8.SetAttribute("EffectMaxTime", cDAction.audioConfig.effectMaxTime.ToString());
					}
				}
			}
			UpdateActionInfo();
		}

		public void RecCamPos(Vector3 pos, float size, float tm)
		{
			currentAction.camConfig.pos = pos;
			currentAction.camConfig.size = size;
			currentAction.camConfig.tm = tm;
		}

		public CDAction GetCurrentAction()
		{
			return currentAction;
		}

		public void RefreshAction()
		{
			for (int i = 0; i < CDTaskManager.Instance.GetCurrentTask().actions.Count; i++)
			{
				_actionTextList[i].text = "Action " + (i + 1);
			}
		}

		public void UpdateActionInfo()
		{
			ClearPreviewText();
			if (currentAction == null)
			{
				return;
			}
			if (currentAction.camConfig.isSet)
			{
				if (currentAction.camConfig.isFollow)
				{
					string str = "Camera: Follow(" + currentAction.camConfig.followRole.ToString() + ")";
					AddPreviewText(str);
				}
				else
				{
					string str2 = "Camera: Pos" + currentAction.camConfig.pos.ToString() + " Size(" + currentAction.camConfig.size + ") Time(" + currentAction.camConfig.tm + ")";
					AddPreviewText(str2);
				}
			}
			if (currentAction.roleConfig.isSet)
			{
				bool flag = false;
				foreach (RoleType key in currentAction.roleConfig.roles.Keys)
				{
					if (currentAction.roleConfig.roles[key].anim != null && currentAction.roleConfig.roles[key].anim.Count > 0)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					foreach (RoleType key2 in currentAction.roleConfig.roles.Keys)
					{
						DebugUtils.Log(DebugType.Other, "role " + key2);
						if (currentAction.roleConfig.roles[key2].anim.Count <= 0)
						{
							continue;
						}
						string text = "";
						foreach (string item in currentAction.roleConfig.roles[key2].anim)
						{
							text += item;
						}
						string str3 = key2.ToString() + ":" + text;
						AddPreviewText(str3);
					}
				}
			}
			if (currentAction.convConfig.isSet)
			{
				foreach (string conv in currentAction.convConfig.convList)
				{
					string str4 = "Conversation: " + conv;
					AddPreviewText(str4);
				}
			}
			if (currentAction.buildConfig.isSet)
			{
				string str5 = "Build: Room(" + currentAction.buildConfig.roomID + ") Item(" + currentAction.buildConfig.itemID + ")Stage(" + currentAction.buildConfig.stageID + ")DelayTime(" + currentAction.buildConfig.delayTime + ")";
				AddPreviewText(str5);
			}
			if (currentAction.otherConfig.isSet)
			{
				string text2 = "";
				if (currentAction.otherConfig.isChapterEnd)
				{
					text2 += "IsChapterEnd = True;";
				}
				if (currentAction.otherConfig.isCatRename)
				{
					text2 += "IsCatRename = True;";
				}
				if (currentAction.otherConfig.isCastleRename)
				{
					text2 += "IsCastleRename = True;";
				}
				if (currentAction.otherConfig.isBlackScreen)
				{
					text2 += "IsBlackScreen = True;";
				}
				if (currentAction.otherConfig.unlockRoomID != 0)
				{
					text2 = text2 + "UnlockRoomID = " + currentAction.otherConfig.unlockRoomID + ";";
				}
				AddPreviewText(text2);
			}
			if (currentAction.audioConfig.isSet)
			{
				string text3 = "";
				if (currentAction.audioConfig.isMusicSet)
				{
					text3 += "IsMusicSet = True;";
					text3 = text3 + "IsMusicLoop = " + currentAction.audioConfig.isMusicLoop + ";";
					text3 = text3 + "IsMusicStop = " + currentAction.audioConfig.isMusicStop + ";\n";
					text3 = text3 + "MusicName = " + currentAction.audioConfig.musicName.ToString() + ";";
					text3 = text3 + "MusicLimitTime = " + currentAction.audioConfig.musicMinTime + "~" + currentAction.audioConfig.musicMaxTime;
				}
				if (currentAction.audioConfig.isEffectSet)
				{
					text3 += "IsEffectSet = True;";
					text3 = text3 + "IsEffectLoop = " + currentAction.audioConfig.isEffectLoop + ";";
					text3 = text3 + "IsEffectStop = " + currentAction.audioConfig.isEffectStop + ";\n";
					text3 = text3 + "EffectName = " + currentAction.audioConfig.effectName.ToString() + ";";
					text3 = text3 + "EffectLimitTime = " + currentAction.audioConfig.effectMinTime + "~" + currentAction.audioConfig.effectMaxTime;
				}
				AddPreviewText(text3);
			}
		}

		public void Reset()
		{
			ClearText();
			ClearPreviewText();
			currentActionIdx = -1;
			currentAction = null;
		}
	}
}
