using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Core.UI
{
	public class SettingDlg : BaseDialog
	{
		private static SettingDlg instance;

		public InputField castleNameInput;

		public InputField catNameInput;

		public Button musicBtn;

		public Button soundBtn;

		public GameObject catImage;

		public GameObject catNameBg;

		public Image notificationImage;

		public Image LoginBtnImage;

		public LocalizationText LoginText;

		public LocalizationText Version;

		public Image FaceBookPic;

		public Text FaceBookName;

		private Sprite soundOpen;

		private Sprite soundClose;

		private Sprite musicOpen;

		private Sprite musicClose;

		private Sprite notificationEnabled;

		private Sprite notificationDisEnabled;

		public static SettingDlg Instance
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
			soundOpen = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_sound_1").GetComponent<SpriteRenderer>().sprite;
			soundClose = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_sound_close_1").GetComponent<SpriteRenderer>().sprite;
			musicOpen = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_music_1").GetComponent<SpriteRenderer>().sprite;
			musicClose = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_music_close_1").GetComponent<SpriteRenderer>().sprite;
			notificationEnabled = Resources.Load<GameObject>("Textures/Elements2/icon_noti").GetComponent<SpriteRenderer>().sprite;
			notificationDisEnabled = Resources.Load<GameObject>("Textures/Elements2/icon_noti_1").GetComponent<SpriteRenderer>().sprite;
		}

		public override void Show()
		{
			base.Show(null);
			Show(null);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			Version.text = string.Format(LanguageConfig.GetString("SettingDlg_Version"), Application.version);
			DebugUtils.Log(DebugType.NetWork, "RegisterMessageHandler((uint)MessageDispatcherType.FacebookStatusChanged)");
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(22u, FacebookStatusChanged);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(23u, UpdatePanelInfo);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(24u, GetMyFaceBookData);
			ShowCastleInfo();
			ShowCatInfo();
			ShowSoundInfo();
			ShowMusicInfo();
			ShowNotificationInfo();
			UpdateLoginBtn();
		}

		private void ShowCastleInfo()
		{
			castleNameInput.interactable = false;
			if (UserDataManager.Instance.GetService().castleName == "")
			{
				castleNameInput.text = "MoonLight";
			}
			else
			{
				castleNameInput.text = UserDataManager.Instance.GetService().castleName;
			}
		}

		private void ShowFaceBookInfo()
		{
			DebugUtils.Log(DebugType.NetWork, "ShowFaceBookInfo");
			string facebookId = UserDataManager.Instance.GetService().facebookId;
			Dictionary<string, string> myData = FacebookUtilities.Instance.myData;
			DebugUtils.Log(DebugType.NetWork, "————————————————————");
			DebugUtils.Log(DebugType.NetWork, ("dic is null = " + ((myData == null) ? "true" : "false")) ?? "");
			if (facebookId != "" && myData != null && myData.ContainsKey("name") && myData.ContainsKey("picture"))
			{
				FaceBookName.text = myData["name"];
				FacebookUtilities.Instance.GetPictureFromUrl(FaceBookPic, myData["picture"]);
			}
			else
			{
				FaceBookName.text = "Name ID";
				FaceBookPic.sprite = GeneralConfig.UIPictures[UIType.DefultHeadPic];
			}
		}

		private void UpdateLoginBtn()
		{
			if (FacebookUtilities.Instance.CheckFacebookLogin())
			{
				LoginBtnImage.sprite = GeneralConfig.UIPictures[UIType.Easy_red];
				LoginText.SetKeyString("SettingDlg_Logout");
				ShowFaceBookInfo();
			}
			else
			{
				LoginBtnImage.sprite = GeneralConfig.UIPictures[UIType.blueBtn];
				LoginText.SetKeyString("SettingDlg_Login");
				FaceBookName.text = "Name ID";
				FaceBookPic.sprite = GeneralConfig.UIPictures[UIType.DefultHeadPic];
			}
		}

		private void ShowCatInfo()
		{
			catNameInput.interactable = false;
			if (UserDataManager.Instance.GetService().UnlockRoleIDList.Contains(4))
			{
				if (UserDataManager.Instance.GetService().catName == "")
				{
					catNameInput.text = "无";
				}
				else
				{
					catNameInput.text = UserDataManager.Instance.GetService().catName;
				}
			}
			else
			{
				catImage.SetActive(false);
				catNameBg.SetActive(false);
			}
		}

		private void ShowSoundInfo()
		{
			if (UserDataManager.Instance.GetService().soundEnabled)
			{
				soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundOpen;
			}
			else
			{
				soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundClose;
			}
		}

		private void ShowMusicInfo()
		{
			if (UserDataManager.Instance.GetService().musicEnabled)
			{
				musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicOpen;
			}
			else
			{
				musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicClose;
			}
		}

		private void ShowNotificationInfo()
		{
			if (UserDataManager.Instance.GetService().notificationEnabled)
			{
				notificationImage.sprite = notificationEnabled;
			}
			else
			{
				notificationImage.sprite = notificationDisEnabled;
			}
		}

		public void CastleNameBtnClicked()
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.RenameDlg, 0);
		}

		public void CatNameBtnClicked()
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.RenameDlg, 1);
		}

		public void BtnSoundClicked()
		{
			if (UserDataManager.Instance.GetService().soundEnabled)
			{
				UserDataManager.Instance.SetSoundEnabled(false);
				soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundClose;
			}
			else
			{
				soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundOpen;
				UserDataManager.Instance.SetSoundEnabled(true);
			}
			AudioManager.Instance.PlayAudioEffect("button");
			UserDataManager.Instance.Save();
		}

		public void BtnMusicClicked()
		{
			if (UserDataManager.Instance.GetService().musicEnabled)
			{
				UserDataManager.Instance.SetMusicEnabled(false);
				musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicClose;
			}
			else
			{
				musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicOpen;
				UserDataManager.Instance.SetMusicEnabled(true);
			}
			AudioManager.Instance.PlayAudioEffect("button");
			UserDataManager.Instance.Save();
		}

		public void BtnContactUsClicked()
		{
			DebugUtils.Log(DebugType.Other, "BtnContactUsClicked");
			Function.Instance.SendMail();
		}

		public void BtnLoginFacebookClicked()
		{
			DebugUtils.Log(DebugType.Other, "BtnLoginFacebookClicked");
			if (FacebookUtilities.Instance.CheckFacebookLogin())
			{
				FacebookUtilities.Instance.LogOutFacebook();
			}
			else
			{
				FacebookUtilities.Instance.LoginWithFacebook();
			}
		}

		public void BtnMultiLanguageClicked()
		{
			DebugUtils.Log(DebugType.Other, "BtnMultiLanguageClicked");
			DialogManagerTemp.Instance.ShowDialog(DialogType.MultiLanguageDlg);
		}

		public void BtnNotificationClicked()
		{
			if (UserDataManager.Instance.GetService().notificationEnabled)
			{
				UserDataManager.Instance.GetService().notificationEnabled = false;
				notificationImage.sprite = notificationDisEnabled;
				UserDataManager.Instance.GetService().notificationEnabled = false;
				DebugUtils.Log(DebugType.Other, "disable notification");
			}
			else
			{
				notificationImage.sprite = notificationEnabled;
				UserDataManager.Instance.GetService().notificationEnabled = true;
				AudioManager.Instance.SetAudioEffectMute(false);
			}
			UserDataManager.Instance.Save();
		}

		public void Close(bool isAnim = true)
		{
			DebugUtils.Log(DebugType.NetWork, "UnRegisterMessageHandler((uint)MessageDispatcherType.FacebookStatusChanged)");
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(22u, FacebookStatusChanged);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(23u, UpdatePanelInfo);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(24u, GetMyFaceBookData);
			DialogManagerTemp.Instance.CloseDialog(DialogType.SettingDlg);
		}

		private void GetMyFaceBookData(uint iMessageType, object arg)
		{
			ShowFaceBookInfo();
		}

		private void UpdatePanelInfo(uint iMessageType, object arg)
		{
			castleNameInput.text = UserDataManager.Instance.GetService().castleName;
			catNameInput.text = UserDataManager.Instance.GetService().catName;
		}

		private void FacebookStatusChanged(uint iMessageType, object arg)
		{
			UpdateLoginBtn();
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}
	}
}
