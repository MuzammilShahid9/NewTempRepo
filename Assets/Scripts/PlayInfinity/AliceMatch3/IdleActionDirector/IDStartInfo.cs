using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDStartInfo : MonoBehaviour
	{
		private static IDStartInfo instance;

		public InputField startStageInput;

		public InputField startTaskInput;

		public InputField endStageInput;

		public InputField endTaskInput;

		public Dropdown selectRole;

		public static IDStartInfo Instance
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

		public void Load(IDInfo action)
		{
			if (action == null)
			{
				return;
			}
			int value = 0;
			startStageInput.text = action.startTask.Split('-')[0];
			startTaskInput.text = action.startTask.Split('-')[1];
			endStageInput.text = action.endTask.Split('-')[0];
			endTaskInput.text = action.endTask.Split('-')[1];
			selectRole.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.roleTypeConfig.Count; i++)
			{
				if (CDConfigManager.Instance.roleTypeConfig[i].RoleType != null && CDConfigManager.Instance.roleTypeConfig[i].RoleType != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.roleTypeConfig[i].RoleType;
					selectRole.options.Add(optionData);
					if (CDConfigManager.Instance.roleTypeConfig[i].RoleType == IDInfoManager.Instance.GetCurrIDInfo().roleType.ToString())
					{
						value = i;
					}
				}
			}
			selectRole.captionText.text = IDInfoManager.Instance.GetCurrIDInfo().roleType.ToString();
			selectRole.value = value;
		}

		public void BtnRecClicked()
		{
			IDInfoManager.Instance.GetCurrIDInfo().startTask = startStageInput.text + "-" + startTaskInput.text;
			IDInfoManager.Instance.GetCurrIDInfo().endTask = endStageInput.text + "-" + endTaskInput.text;
			IDInfoManager.Instance.GetCurrIDInfo().roleType = (RoleType)Enum.Parse(typeof(RoleType), selectRole.captionText.text);
		}
	}
}
