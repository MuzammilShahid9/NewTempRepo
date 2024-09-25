using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDCamera : MonoBehaviour
	{
		private static CDCamera instance;

		public Text cameraPos;

		public Text cameraSize;

		public Toggle followToggle;

		public Dropdown followTarget;

		public InputField tmInput;

		public Button btnSave;

		public Button btnDelete;

		public static CDCamera Instance
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

		public void Load(CDAction action)
		{
			LoadRoleType();
			if (action != null)
			{
				tmInput.text = action.camConfig.tm.ToString();
				cameraPos.text = action.camConfig.pos.ToString();
				cameraSize.text = action.camConfig.size.ToString();
				followToggle.isOn = action.camConfig.isFollow;
				followTarget.value = (int)action.camConfig.followRole;
				followTarget.captionText.text = followTarget.options[followTarget.value].text;
			}
		}

		private void LoadRoleType()
		{
			followTarget.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.roleTypeConfig.Count; i++)
			{
				if (CDConfigManager.Instance.roleTypeConfig[i].RoleType != null && CDConfigManager.Instance.roleTypeConfig[i].RoleType != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.roleTypeConfig[i].RoleType;
					followTarget.options.Add(optionData);
				}
			}
		}

		public void BtnRecClicked()
		{
			cameraPos.text = CameraControl.Instance.camera3D.cam.transform.position.ToString();
			cameraSize.text = CameraControl.Instance.camera3D.cam.orthographicSize.ToString("F2");
			CDActionManager.Instance.currentAction.camConfig.pos = CameraControl.Instance.camera3D.cam.transform.position;
			CDActionManager.Instance.currentAction.camConfig.size = CameraControl.Instance.camera3D.cam.orthographicSize;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnTimeValueChanged()
		{
			DebugUtils.Log(DebugType.Other, "update camera time " + tmInput.text);
			CDActionManager.Instance.currentAction.camConfig.tm = float.Parse(tmInput.text);
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnIsFollowValueChanged()
		{
			CDActionManager.Instance.currentAction.camConfig.isFollow = followToggle.isOn;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnFollowTargetChanged(Dropdown change)
		{
			DebugUtils.Log(DebugType.Other, "update camera time " + tmInput.text);
			CDActionManager.Instance.currentAction.camConfig.followRole = (RoleType)Enum.Parse(typeof(RoleType), followTarget.options[change.value].text);
			CDActionManager.Instance.UpdateActionInfo();
		}
	}
}
