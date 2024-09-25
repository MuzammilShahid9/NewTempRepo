using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class CompareDataDlg : BaseDialog
	{
		private UserData nativeData;

		private UserData cloudData;

		public Text LeftLevel;

		public Text LeftCoin;

		public Text LeftScroll;

		public Text RightLevel;

		public Text RightCoin;

		public Text RightScroll;

		[Header("----------------------")]
		private bool isUseCloudData;

		private static CompareDataDlg instance;

		public static CompareDataDlg Instance
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

		public override void Show(object obj)
		{
			base.Show(obj);
			DebugUtils.Log(DebugType.NetWork, "CompareDataDlg Show! ");
			nativeData = UserDataManager.Instance.GetService();
			cloudData = (UserData)obj;
			LeftLevel.text = string.Concat(nativeData.level);
			LeftCoin.text = string.Concat(nativeData.coin);
			LeftScroll.text = string.Concat(nativeData.scrollNum);
			RightLevel.text = string.Concat(cloudData.level);
			RightCoin.text = string.Concat(cloudData.coin);
			RightScroll.text = string.Concat(cloudData.scrollNum);
		}

		public void UserNativeData()
		{
			Action obj = delegate
			{
				NetworkConfig.isSuccessfulGetDataFromService = true;
				nativeData.facebookId = cloudData.facebookId;
				UserDataManager.Instance.Save();
				DialogManagerTemp.Instance.CloseAllDialogs();
				Analytics.Event("DataRecovery", new Dictionary<string, string> { { "DataRecovery", "UseNativeData" } });
			};
			DialogManagerTemp.Instance.ShowDialog(DialogType.ConfirmDataDlg, obj);
		}

		public void UserCloudData()
		{
			Action obj = delegate
			{
				NetworkConfig.isSuccessfulGetDataFromService = true;
				UserDataManager.Instance.UpdateUserData(cloudData);
				DialogManagerTemp.Instance.CloseAllDialogs();
				UserDataManager.Instance.Save();
				UserDataManager.Instance.SetMusicEnabled(UserDataManager.Instance.GetService().musicEnabled);
				UserDataManager.Instance.SetSoundEnabled(UserDataManager.Instance.GetService().soundEnabled);
				Analytics.Event("DataRecovery", new Dictionary<string, string> { { "DataRecovery", "UseCloudData" } });
				if (SceneManager.GetActiveScene().name != SceneType.LogoScene.ToString())
				{
					BankDlg.Instance.isInit = true;
					SceneTransManager.Instance.TransTo(SceneType.LoadingScene);
				}
			};
			DialogManagerTemp.Instance.ShowDialog(DialogType.ConfirmDataDlg, obj);
		}

		public void Close(bool isAnim = true)
		{
			LeftLevel.text = "";
			LeftCoin.text = "";
			LeftScroll.text = "";
			RightLevel.text = "";
			RightCoin.text = "";
			RightScroll.text = "";
			DialogManagerTemp.Instance.CloseDialog(DialogType.CompareDataDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
		}
	}
}
