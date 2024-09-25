using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDActionConfig : MonoBehaviour
	{
		public InputField actionTime;

		public List<GameObject> panels = new List<GameObject>();

		public List<Toggle> panelActive = new List<Toggle>();

		private static CDActionConfig instance;

		public static CDActionConfig Instance
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
			CloseAll();
		}

		public void Init()
		{
			actionTime.text = "-1";
		}

		public void CloseAll()
		{
			foreach (GameObject panel in panels)
			{
				panel.SetActive(false);
			}
		}

		public void UnselectAllToggle()
		{
			foreach (Toggle item in panelActive)
			{
				item.isOn = false;
			}
		}

		public void SelectBtnClicked(int idx)
		{
			if (CDActionManager.Instance.GetCurrentAction() != null)
			{
				CloseAll();
				panels[idx].SetActive(true);
				switch (idx)
				{
				case 0:
					CDCamera.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				case 1:
					CDRoleAnimation.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				case 2:
					CDConversation.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				case 3:
					CDBuild.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				case 4:
					CDOther.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				case 5:
					CDAudio.Instance.Load(CDActionManager.Instance.GetCurrentAction());
					break;
				}
			}
		}

		public void OnTimeValueChange()
		{
			DebugUtils.Log(DebugType.Other, "value " + actionTime.text);
			if (actionTime.text == "" || actionTime.text.Substring(0, 1) == "-")
			{
				CDActionManager.Instance.currentAction.tm = -1f;
			}
			else
			{
				CDActionManager.Instance.currentAction.tm = float.Parse(actionTime.text);
			}
		}

		public void OnCameraPanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.camConfig.isSet = panelActive[0].isOn;
			}
		}

		public void OnRolePanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.roleConfig.isSet = panelActive[1].isOn;
			}
		}

		public void OnConvPanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.convConfig.isSet = panelActive[2].isOn;
			}
		}

		public void OnBuildPanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.buildConfig.isSet = panelActive[3].isOn;
			}
		}

		public void OnOtherPanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.otherConfig.isSet = panelActive[4].isOn;
			}
		}

		public void OnAudioPanelActiveValueChange()
		{
			if (CDActionManager.Instance.currentAction != null)
			{
				CDActionManager.Instance.currentAction.audioConfig.isSet = panelActive[5].isOn;
			}
		}
	}
}
