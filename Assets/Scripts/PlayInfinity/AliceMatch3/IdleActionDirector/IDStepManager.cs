using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDStepManager : MonoBehaviour
	{
		private static IDStepManager instance;

		public GameObject actionText;

		public GameObject actionTextPanel;

		public GameObject actionPreviewText;

		public GameObject actionPreviewTextPanel;

		public int currentActionIdx = -1;

		public IDAction currentAction;

		private List<Text> _actionTextList = new List<Text>();

		public static IDStepManager Instance
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

		public void Load(IDActionManager.Action task)
		{
			ClearText();
			currentActionIdx = 0;
			currentAction = task.actions;
			if (currentAction != null)
			{
				LoadConfigs(currentAction);
			}
		}

		public void LoadConfigs(IDAction action)
		{
			currentAction = action;
			IDActionConfig.Instance.actionTime.text = action.tm.ToString();
			IDActionConfig.Instance.panelActive[0].isOn = true;
			IDActionConfig.Instance.panelActive[1].isOn = action.roleConfig.isSet;
			IDActionConfig.Instance.panelActive[2].isOn = action.convConfig.isSet;
			IDActionConfig.Instance.panelActive[3].isOn = action.delayConfig.isSet;
			IDActionConfig.Instance.panelActive[4].isOn = false;
			IDActionConfig.Instance.panelActive[5].isOn = action.audioConfig.isSet;
			IDActionConfig.Instance.CloseAll();
			UpdateActionInfo();
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

		private void ClearText()
		{
			_actionTextList.Clear();
			for (int i = 0; i < actionTextPanel.transform.childCount; i++)
			{
				Object.Destroy(actionTextPanel.transform.GetChild(i).gameObject);
			}
		}

		private void ClearPreviewText()
		{
			for (int i = 0; i < actionPreviewTextPanel.transform.childCount; i++)
			{
				Object.Destroy(actionPreviewTextPanel.transform.GetChild(i).gameObject);
			}
		}

		public void UpdateActionInfo()
		{
			ClearPreviewText();
			if (currentAction == null)
			{
				return;
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
						string str = key2.ToString() + ":" + text;
						AddPreviewText(str);
					}
				}
			}
			if (currentAction.convConfig.isSet)
			{
				foreach (string conv in currentAction.convConfig.convList)
				{
					string str2 = "Conversation: " + conv;
					AddPreviewText(str2);
				}
			}
			if (currentAction.buildConfig.isSet)
			{
				string str3 = "Build: Room(" + currentAction.buildConfig.roomID + ") Item(" + currentAction.buildConfig.itemID + ")Stage(" + currentAction.buildConfig.stageID + ")";
				AddPreviewText(str3);
			}
			if (currentAction.otherConfig.isSet)
			{
				string text2 = "";
				if (currentAction.otherConfig.isChapterEnd)
				{
					text2 += "IsChapterEnd = True";
				}
				if (currentAction.otherConfig.isCatRename)
				{
					text2 += "IsCatRename = True";
				}
				if (currentAction.otherConfig.isCastleRename)
				{
					text2 += "IsCastleRename = True";
				}
				if (currentAction.otherConfig.isBlackScreen)
				{
					text2 += "IsBlackScreen = True";
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
					text3 = text3 + "MusicName = " + currentAction.audioConfig.musicName + ";";
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
			if (currentAction.delayConfig.isSet)
			{
				string text4 = "";
				text4 = text4 + "Delay = " + currentAction.delayConfig.delayTime + "s;";
				AddPreviewText(text4);
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

		public IDAction GetCurrentAction()
		{
			return currentAction;
		}

		public IDAction CreateAction()
		{
			IDAction iDAction = new IDAction();
			iDAction.tm = -1f;
			iDAction.roleConfig = new IDRoleConfig();
			iDAction.roleConfig.isSet = false;
			iDAction.roleConfig.roles = new Dictionary<RoleType, IDRoleAnim>();
			iDAction.convConfig = new IDConversationConfig();
			iDAction.convConfig.isSet = false;
			iDAction.convConfig.convList = new List<string>();
			iDAction.buildConfig = new IDBuildConfig();
			iDAction.buildConfig.isSet = false;
			iDAction.otherConfig = new IDOtherConfig();
			iDAction.otherConfig.isSet = false;
			iDAction.delayConfig = new IDDelayConfig();
			iDAction.delayConfig.isSet = false;
			iDAction.audioConfig = new IDAudioConfig();
			iDAction.audioConfig.isSet = false;
			return iDAction;
		}
	}
}
