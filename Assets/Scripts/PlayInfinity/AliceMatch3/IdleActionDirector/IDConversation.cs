using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDConversation : MonoBehaviour
	{
		private static IDConversation instance;

		public InputField textIDInput;

		public Dropdown selectRole;

		public Button btnAdd;

		public Button btnDel;

		public GameObject textPanel;

		public GameObject textPrefab;

		public List<Text> _textList = new List<Text>();

		public static IDConversation Instance
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

		public void Load(IDAction action)
		{
			if (action == null)
			{
				return;
			}
			ClearText();
			int value = 0;
			foreach (string conv in action.convConfig.convList)
			{
				AddText(conv);
			}
			selectRole.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.roleTypeConfig.Count; i++)
			{
				if (CDConfigManager.Instance.roleTypeConfig[i].RoleType != null && CDConfigManager.Instance.roleTypeConfig[i].RoleType != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.roleTypeConfig[i].RoleType;
					if (CDConfigManager.Instance.roleTypeConfig[i].RoleType == IDInfoManager.Instance.GetCurrIDInfo().roleType.ToString())
					{
						value = i;
					}
					selectRole.options.Add(optionData);
				}
			}
			selectRole.captionText.text = IDInfoManager.Instance.GetCurrIDInfo().roleType.ToString();
			selectRole.value = value;
			textIDInput.text = "idle_bubble";
		}

		public void BtnAddClicked()
		{
			if (textIDInput.text != "")
			{
				string text = "";
				text = text + textIDInput.text + "|";
				text += selectRole.captionText.text;
				AddText(text);
				IDStepManager.Instance.GetCurrentAction().convConfig.convList.Add(text);
			}
			IDStepManager.Instance.UpdateActionInfo();
		}

		public void BtnAddBehindClicked()
		{
			if (textIDInput.text != "")
			{
				if (IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count < 1)
				{
					string text = "";
					text = text + textIDInput.text + "|";
					text += selectRole.captionText.text;
					AddText(text);
					IDStepManager.Instance.GetCurrentAction().convConfig.convList.Add(text);
				}
				else
				{
					DelText();
					string[] array = IDStepManager.Instance.GetCurrentAction().convConfig.convList[IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count - 1].Split('|');
					string text2 = array[0];
					text2 = text2 + "," + textIDInput.text + "|";
					text2 += array[1];
					IDStepManager.Instance.GetCurrentAction().convConfig.convList[IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count - 1] = text2;
					AddText(text2);
				}
			}
			IDStepManager.Instance.UpdateActionInfo();
		}

		public void BtnDelBehindClicked()
		{
			if (IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count < 1)
			{
				return;
			}
			string[] array = IDStepManager.Instance.GetCurrentAction().convConfig.convList[IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count - 1].Split('|');
			string text = array[0];
			string[] array2 = text.Split(',');
			if (array2.Length >= 2)
			{
				string text2 = "";
				for (int i = 0; i < array2.Length - 1; i++)
				{
					text2 += array2[i];
				}
				text = text2 + "|";
				text += array[1];
				IDStepManager.Instance.GetCurrentAction().convConfig.convList[IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count - 1] = text;
				DelText();
				AddText(text);
				IDStepManager.Instance.UpdateActionInfo();
			}
		}

		public void BtnDelClicked()
		{
			DelText();
			IDStepManager.Instance.GetCurrentAction().convConfig.convList.RemoveAt(IDStepManager.Instance.GetCurrentAction().convConfig.convList.Count - 1);
			IDStepManager.Instance.UpdateActionInfo();
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

		private void AddText(string str)
		{
			GameObject obj = Object.Instantiate(textPrefab);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(textPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_textList.Add(component);
		}
	}
}
