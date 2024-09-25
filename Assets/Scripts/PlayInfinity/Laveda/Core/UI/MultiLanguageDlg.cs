using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class MultiLanguageDlg : BaseDialog
	{
		public Transform GridTransform;

		public RectTransform viewPointTransform;

		public RectTransform contentTransform;

		public RectTransform scrollViewTransform;

		public Dictionary<SystemLanguage, Toggle> LanguageToggle = new Dictionary<SystemLanguage, Toggle>();

		private static MultiLanguageDlg instance;

		private int LanguageNum;

		private ToggleGroup tg;

		public static MultiLanguageDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.MultiLanguageDlg);
		}

		public override void Show(object obj)
		{
			CastleSceneUIManager.Instance.HideAllBtn();
			RoleManager.Instance.HideAllRoles();
			if (LanguageToggle.Count == 0)
			{
				LanguageNum = LanguageConfig.LanguageTable.Count;
				tg = GridTransform.GetComponent<ToggleGroup>();
				foreach (KeyValuePair<SystemLanguage, Dictionary<string, string>> item2 in LanguageConfig.LanguageTable)
				{
					GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/LanguageToggle"));
					gameObject.name = item2.Key.ToString();
					switch (item2.Key)
					{
					case SystemLanguage.Chinese:
						gameObject.name = "中文";
						gameObject.GetComponentInChildren<Text>().text = "中文";
						break;
					case SystemLanguage.German:
						gameObject.name = "Deutsch";
						gameObject.GetComponentInChildren<Text>().text = "Deutsch";
						break;
					case SystemLanguage.English:
						gameObject.name = "English";
						gameObject.GetComponentInChildren<Text>().text = "English";
						break;
					case SystemLanguage.French:
						gameObject.name = "Français";
						gameObject.GetComponentInChildren<Text>().text = "Français";
						break;
					case SystemLanguage.Spanish:
						gameObject.name = "Español";
						gameObject.GetComponentInChildren<Text>().text = "Español";
						break;
					case SystemLanguage.Japanese:
						gameObject.name = "日本語";
						gameObject.GetComponentInChildren<Text>().text = "日本語";
						break;
					case SystemLanguage.Korean:
						gameObject.name = "한국어";
						gameObject.GetComponentInChildren<Text>().text = "한국어";
						break;
					case SystemLanguage.Russian:
						gameObject.name = "Pусский";
						gameObject.GetComponentInChildren<Text>().text = "Pусский";
						break;
					case SystemLanguage.Italian:
						gameObject.name = "Italiano";
						gameObject.GetComponentInChildren<Text>().text = "Italiano";
						break;
					case SystemLanguage.Portuguese:
						gameObject.name = "Português";
						gameObject.GetComponentInChildren<Text>().text = "Português";
						break;
					}
					gameObject.transform.SetParent(GridTransform, false);
					Toggle component = gameObject.GetComponent<Toggle>();
					LanguageToggle[item2.Key] = component;
					component.group = tg;
					component.isOn = false;
					gameObject.transform.GetChild(0).Find("Flag").GetComponent<Image>()
						.sprite = Resources.Load<GameObject>("Textures/Elements2/" + item2.Key).GetComponent<SpriteRenderer>().sprite;
				}
				foreach (KeyValuePair<SystemLanguage, Toggle> item in LanguageToggle)
				{
					if (item.Key == LanguageConfig.GetCurrentLanguage())
					{
						item.Value.isOn = true;
					}
					item.Value.onValueChanged.AddListener(delegate(bool isOn)
					{
						if (isOn)
						{
							LanguageConfig.ChangeLangage(item.Key);
						}
					});
				}
			}
			foreach (KeyValuePair<SystemLanguage, Toggle> item3 in LanguageToggle)
			{
				if (item3.Value.isOn)
				{
					CenterToSelected(item3.Value.gameObject);
				}
			}
			base.Show(obj);
		}

		private void CenterToSelected(GameObject selected)
		{
			float num = scrollViewTransform.rect.height / 2f;
			float num2 = Mathf.Abs(selected.transform.localPosition.y);
			Vector3 localPosition = contentTransform.localPosition;
			localPosition.y = num2 - num;
			contentTransform.localPosition = localPosition;
		}

		public void BtnCloseClicked()
		{
			RoleManager.Instance.ShowAllRoles();
			DialogManagerTemp.Instance.CloseDialog(DialogType.MultiLanguageDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public override void ChangeLangage(uint iMessageType, object arg)
		{
		}
	}
}
