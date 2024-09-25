using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDRoleAnimation : MonoBehaviour
	{
		private static CDRoleAnimation instance;

		public Dropdown animTye;

		public Text charPosition;

		public Text charRotation;

		public Button btnRecord;

		public Dropdown selectRole;

		public Dropdown selectAnim;

		public int currentAnim;

		public RoleType currentRole;

		public GameObject animText;

		public GameObject animTextPanel;

		private List<Text> _animTextList = new List<Text>();

		public static CDRoleAnimation Instance
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
			RoleManager.Instance.GenerateAllRoles();
			currentRole = RoleType.Alice;
		}

		public void Init()
		{
			charPosition.text = "(0,0,0)";
			charRotation.text = "(0,0,0)";
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftArrow) && RoleManager.Instance.roleDictionary.ContainsKey(currentRole))
			{
				RoleManager.Instance.roleDictionary[currentRole].transform.Rotate(new Vector3(0f, 1f, 0f), Space.Self);
			}
			if (Input.GetKey(KeyCode.RightArrow) && RoleManager.Instance.roleDictionary.ContainsKey(currentRole))
			{
				RoleManager.Instance.roleDictionary[currentRole].transform.Rotate(new Vector3(0f, -1f, 0f), Space.Self);
			}
		}

		public void Load(CDAction action)
		{
			LoadRoleType();
			currentRole = RoleType.Alice;
			SwitchRole(currentRole);
		}

		public void SwitchRole(RoleType role)
		{
			CDAction currentAction = CDActionManager.Instance.currentAction;
			if (!currentAction.roleConfig.roles.ContainsKey(role))
			{
				AddRole(role);
			}
			LoadRoleAnimation(role);
			ClearText();
			if (currentAction.roleConfig.roles[role].anim.Count <= 0)
			{
				return;
			}
			foreach (string item in currentAction.roleConfig.roles[role].anim)
			{
				AddText(item);
			}
		}

		public void AddRole(RoleType role)
		{
			CDRoleAnim cDRoleAnim = new CDRoleAnim();
			cDRoleAnim.anim = new List<string>();
			CDActionManager.Instance.currentAction.roleConfig.roles.Add(role, cDRoleAnim);
		}

		private void LoadRoleType()
		{
			selectRole.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.roleTypeConfig.Count; i++)
			{
				if (CDConfigManager.Instance.roleTypeConfig[i].RoleType != null && CDConfigManager.Instance.roleTypeConfig[i].RoleType != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.roleTypeConfig[i].RoleType;
					selectRole.options.Add(optionData);
				}
			}
			selectRole.value = 0;
			selectRole.captionText.text = selectRole.options[0].text;
		}

		public void LoadRoleAnimation(RoleType role)
		{
			selectAnim.ClearOptions();
			switch (role)
			{
			case RoleType.Alice:
			{
				for (int j = 0; j < CDConfigManager.Instance.roleAnimConfig.Count; j++)
				{
					if (CDConfigManager.Instance.roleAnimConfig[j].Alice != null && CDConfigManager.Instance.roleAnimConfig[j].Alice != "")
					{
						Dropdown.OptionData optionData2 = new Dropdown.OptionData();
						optionData2.text = CDConfigManager.Instance.roleAnimConfig[j].Alice;
						selectAnim.options.Add(optionData2);
					}
				}
				break;
			}
			case RoleType.John:
			{
				for (int l = 0; l < CDConfigManager.Instance.roleAnimConfig.Count; l++)
				{
					if (CDConfigManager.Instance.roleAnimConfig[l].John != null && CDConfigManager.Instance.roleAnimConfig[l].John != "")
					{
						Dropdown.OptionData optionData4 = new Dropdown.OptionData();
						optionData4.text = CDConfigManager.Instance.roleAnimConfig[l].John;
						selectAnim.options.Add(optionData4);
					}
				}
				break;
			}
			case RoleType.Arthur:
			{
				for (int m = 0; m < CDConfigManager.Instance.roleAnimConfig.Count; m++)
				{
					if (CDConfigManager.Instance.roleAnimConfig[m].Arthur != null && CDConfigManager.Instance.roleAnimConfig[m].Arthur != "")
					{
						Dropdown.OptionData optionData5 = new Dropdown.OptionData();
						optionData5.text = CDConfigManager.Instance.roleAnimConfig[m].Arthur;
						selectAnim.options.Add(optionData5);
					}
				}
				break;
			}
			case RoleType.Cat:
			{
				for (int k = 0; k < CDConfigManager.Instance.roleAnimConfig.Count; k++)
				{
					if (CDConfigManager.Instance.roleAnimConfig[k].Cat != null && CDConfigManager.Instance.roleAnimConfig[k].Cat != "")
					{
						Dropdown.OptionData optionData3 = new Dropdown.OptionData();
						optionData3.text = CDConfigManager.Instance.roleAnimConfig[k].Cat;
						selectAnim.options.Add(optionData3);
					}
				}
				break;
			}
			case RoleType.Tina:
			{
				for (int i = 0; i < CDConfigManager.Instance.roleAnimConfig.Count; i++)
				{
					if (CDConfigManager.Instance.roleAnimConfig[i].Tina != null && CDConfigManager.Instance.roleAnimConfig[i].Tina != "")
					{
						Dropdown.OptionData optionData = new Dropdown.OptionData();
						optionData.text = CDConfigManager.Instance.roleAnimConfig[i].Tina;
						selectAnim.options.Add(optionData);
					}
				}
				break;
			}
			}
			selectAnim.value = 0;
			selectAnim.captionText.text = selectAnim.options[0].text;
		}

		public void BtnAddClicked()
		{
			if (!CDActionManager.Instance.currentAction.roleConfig.roles.ContainsKey(currentRole))
			{
				AddRole(currentRole);
			}
			if (CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim == null)
			{
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim = new List<string>();
			}
			if (selectAnim.captionText.text == "Walk")
			{
				string text = "W";
				text += RoleManager.Instance.GetRole(currentRole).transform.position.ToString();
				text += RoleManager.Instance.GetRole(currentRole).transform.rotation.ToString();
				AddText(text);
				if (CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim == null)
				{
					DebugUtils.Log(DebugType.Other, "123");
				}
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Add(text);
			}
			else if (selectAnim.captionText.text == "Run")
			{
				string text2 = "R";
				text2 += RoleManager.Instance.GetRole(currentRole).transform.position.ToString();
				text2 += RoleManager.Instance.GetRole(currentRole).transform.rotation.ToString();
				AddText(text2);
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Add(text2);
			}
			else if (selectAnim.captionText.text == "Flash")
			{
				string text3 = "F";
				text3 += RoleManager.Instance.GetRole(currentRole).transform.position.ToString();
				text3 += RoleManager.Instance.GetRole(currentRole).transform.rotation.ToString();
				AddText(text3);
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Add(text3);
			}
			else
			{
				AddText(selectAnim.captionText.text);
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Add(selectAnim.options[currentAnim].text);
			}
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void BtnDelClicked()
		{
			DelText();
			if (CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Count > 0)
			{
				CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.RemoveAt(CDActionManager.Instance.currentAction.roleConfig.roles[currentRole].anim.Count - 1);
			}
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void SelectRoleDropdownValueChanged(Dropdown change)
		{
			RoleType roleType = (RoleType)Enum.Parse(typeof(RoleType), selectRole.options[change.value].text);
			if (currentRole != roleType)
			{
				currentRole = roleType;
				SwitchRole(currentRole);
			}
		}

		public void SelectAnimDropdownValueChanged(Dropdown change)
		{
			DebugUtils.Log(DebugType.Other, "New Value : " + change.value);
			currentAnim = change.value;
		}

		private void AddText(string str)
		{
			GameObject obj = UnityEngine.Object.Instantiate(animText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(animTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_animTextList.Add(component);
		}

		private void DelText()
		{
			if (_animTextList.Count > 0)
			{
				UnityEngine.Object.Destroy(_animTextList[_animTextList.Count - 1].gameObject);
				_animTextList.RemoveAt(_animTextList.Count - 1);
			}
		}

		private void ClearText()
		{
			_animTextList.Clear();
			for (int i = 0; i < animTextPanel.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(animTextPanel.transform.GetChild(i).gameObject);
			}
		}
	}
}
