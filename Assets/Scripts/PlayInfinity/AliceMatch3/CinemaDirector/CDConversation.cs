using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDConversation : MonoBehaviour
	{
		private static CDConversation instance;

		public Button btnAdd;

		public Button btnDel;

		public ToggleGroup toggleGroup;

		public Toggle leftToggle;

		public Toggle rightToggle;

		public Dropdown selectRole;

		public Dropdown selectExpression;

		public InputField textIDInput;

		public InputField nameIDInput;

		public GameObject textPanel;

		public GameObject textPrefab;

		public List<Text> _textList = new List<Text>();

		public static CDConversation Instance
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
			leftToggle.isOn = true;
			rightToggle.isOn = false;
			nameIDInput.text = "Name";
		}

		public void Load(CDAction action)
		{
			ClearText();
			foreach (string conv in action.convConfig.convList)
			{
				AddText(conv);
			}
			selectRole.ClearOptions();
			selectExpression.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.roleTypeConfig.Count; i++)
			{
				if (CDConfigManager.Instance.roleTypeConfig[i].RoleType != null && CDConfigManager.Instance.roleTypeConfig[i].RoleType != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.roleTypeConfig[i].RoleType;
					selectRole.options.Add(optionData);
				}
			}
			selectRole.captionText.text = selectRole.options[0].text;
			selectRole.value = 0;
			for (int j = 0; j < CDConfigManager.Instance.roleImageConfig.Count; j++)
			{
				if (CDConfigManager.Instance.roleImageConfig[j].Alice != null && CDConfigManager.Instance.roleImageConfig[j].Alice != "")
				{
					Dropdown.OptionData optionData2 = new Dropdown.OptionData();
					optionData2.text = CDConfigManager.Instance.roleImageConfig[j].Alice;
					selectExpression.options.Add(optionData2);
				}
			}
			selectExpression.captionText.text = selectExpression.options[0].text;
			selectExpression.value = 0;
		}

		public void BtnAddClicked()
		{
			if (textIDInput.text != "" && nameIDInput.text != "")
			{
				string text = "";
				text = text + textIDInput.text + "|";
				text = text + nameIDInput.text + "|";
				text = text + selectRole.captionText.text + "|";
				text = text + selectExpression.captionText.text + "|";
				text = ((!leftToggle.isOn) ? (text + "right") : (text + "Left"));
				AddText(text);
				CDActionManager.Instance.GetCurrentAction().convConfig.convList.Add(text);
			}
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void BtnDelClicked()
		{
			DelText();
			CDActionManager.Instance.GetCurrentAction().convConfig.convList.RemoveAt(CDActionManager.Instance.GetCurrentAction().convConfig.convList.Count - 1);
			CDActionManager.Instance.UpdateActionInfo();
		}

		private void AddText(string str)
		{
			GameObject obj = Object.Instantiate(textPrefab);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(textPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_textList.Add(component);
		}

		private void DelText()
		{
			if (_textList.Count > 0)
			{
				Object.Destroy(_textList[_textList.Count - 1].gameObject);
				_textList.RemoveAt(_textList.Count - 1);
			}
		}

		private void ClearText()
		{
			_textList.Clear();
			for (int i = 0; i < textPanel.transform.childCount; i++)
			{
				Object.Destroy(textPanel.transform.GetChild(i).gameObject);
			}
		}

		public void SelectRoleDropdownValueChanged(Dropdown change)
		{
			DebugUtils.Log(DebugType.Other, "New Value : " + change.value);
			selectExpression.ClearOptions();
			leftToggle.isOn = false;
			rightToggle.isOn = true;
			if (change.value == 0)
			{
				for (int i = 0; i < CDConfigManager.Instance.roleImageConfig.Count; i++)
				{
					if (CDConfigManager.Instance.roleImageConfig[i].Alice != null && CDConfigManager.Instance.roleImageConfig[i].Alice != "")
					{
						Dropdown.OptionData optionData = new Dropdown.OptionData();
						optionData.text = CDConfigManager.Instance.roleImageConfig[i].Alice;
						selectExpression.options.Add(optionData);
					}
				}
				leftToggle.isOn = true;
				rightToggle.isOn = false;
			}
			else if (change.value == 1)
			{
				for (int j = 0; j < CDConfigManager.Instance.roleImageConfig.Count; j++)
				{
					if (CDConfigManager.Instance.roleImageConfig[j].John != null && CDConfigManager.Instance.roleImageConfig[j].John != "")
					{
						Dropdown.OptionData optionData2 = new Dropdown.OptionData();
						optionData2.text = CDConfigManager.Instance.roleImageConfig[j].John;
						selectExpression.options.Add(optionData2);
					}
				}
			}
			else if (change.value == 2)
			{
				for (int k = 0; k < CDConfigManager.Instance.roleImageConfig.Count; k++)
				{
					if (CDConfigManager.Instance.roleImageConfig[k].Arthur != null && CDConfigManager.Instance.roleImageConfig[k].Arthur != "")
					{
						Dropdown.OptionData optionData3 = new Dropdown.OptionData();
						optionData3.text = CDConfigManager.Instance.roleImageConfig[k].Arthur;
						selectExpression.options.Add(optionData3);
					}
				}
			}
			else if (change.value == 4)
			{
				for (int l = 0; l < CDConfigManager.Instance.roleImageConfig.Count; l++)
				{
					if (CDConfigManager.Instance.roleImageConfig[l].Tina != null && CDConfigManager.Instance.roleImageConfig[l].Tina != "")
					{
						Dropdown.OptionData optionData4 = new Dropdown.OptionData();
						optionData4.text = CDConfigManager.Instance.roleImageConfig[l].Tina;
						selectExpression.options.Add(optionData4);
					}
				}
			}
			if (selectExpression.options.Count > 0)
			{
				selectExpression.value = 0;
				selectExpression.captionText.text = selectExpression.options[0].text;
			}
		}

		public void SelectExpressionDropdownValueChanged(Dropdown change)
		{
		}
	}
}
