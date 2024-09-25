using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDActionConfig : MonoBehaviour
	{
		public InputField actionTime;

		public List<GameObject> panels = new List<GameObject>();

		public List<Toggle> panelActive = new List<Toggle>();

		private static IDActionConfig instance;

		public static IDActionConfig Instance
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

		public void OnTimeValueChange()
		{
			DebugUtils.Log(DebugType.Other, "value " + actionTime.text);
			if (actionTime.text == "" || actionTime.text.Substring(0, 1) == "-")
			{
				IDStepManager.Instance.currentAction.tm = -1f;
			}
			else
			{
				IDStepManager.Instance.currentAction.tm = float.Parse(actionTime.text);
			}
		}

		public void SelectBtnClicked(int idx)
		{
			if (IDStepManager.Instance.GetCurrentAction() != null)
			{
				CloseAll();
				if (idx != 4)
				{
					panels[idx].SetActive(true);
				}
				switch (idx)
				{
				case 0:
					IDStartInfo.Instance.Load(IDInfoManager.Instance.GetCurrIDInfo());
					break;
				case 1:
					IDRoleAnimation.Instance.Load(IDStepManager.Instance.GetCurrentAction());
					break;
				case 2:
					IDConversation.Instance.Load(IDStepManager.Instance.GetCurrentAction());
					break;
				case 3:
					IDDelay.Instance.Load(IDStepManager.Instance.GetCurrentAction());
					break;
				case 5:
					IDAudio.Instance.Load(IDStepManager.Instance.GetCurrentAction());
					break;
				}
			}
		}

		public void OnCameraPanelActiveValueChange()
		{
			IDAction currentAction = IDStepManager.Instance.currentAction;
		}

		public void OnRolePanelActiveValueChange()
		{
			if (IDStepManager.Instance.currentAction != null)
			{
				IDStepManager.Instance.currentAction.roleConfig.isSet = panelActive[1].isOn;
			}
		}

		public void OnConvPanelActiveValueChange()
		{
			if (IDStepManager.Instance.currentAction != null)
			{
				IDStepManager.Instance.currentAction.convConfig.isSet = panelActive[2].isOn;
			}
		}

		public void OnBuildPanelActiveValueChange()
		{
			if (IDStepManager.Instance.currentAction != null)
			{
				IDStepManager.Instance.currentAction.delayConfig.isSet = panelActive[3].isOn;
			}
		}

		public void OnOtherPanelActiveValueChange()
		{
			if (IDStepManager.Instance.currentAction != null)
			{
				IDStepManager.Instance.currentAction.otherConfig.isSet = false;
			}
		}

		public void OnAudioPanelActiveValueChange()
		{
			if (IDStepManager.Instance.currentAction != null)
			{
				IDStepManager.Instance.currentAction.audioConfig.isSet = panelActive[5].isOn;
			}
		}
	}
}
