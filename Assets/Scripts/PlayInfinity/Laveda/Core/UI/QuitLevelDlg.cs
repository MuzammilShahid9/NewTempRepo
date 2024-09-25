using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class QuitLevelDlg : BaseDialog
	{
		public GameObject Combo;

		private static QuitLevelDlg instance;

		public static QuitLevelDlg Instance
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
			Combo.SetActive(false);
			if (!UserDataManager.Instance.GetIsComboing() || GlobalVariables.ComboNum < 1)
			{
				return;
			}
			Combo.SetActive(true);
			Combo.transform.Find("Icon").GetComponent<Image>().sprite = GetHaveElementAndCellTool.GetComboPicture(GlobalVariables.ComboNum);
			DialogManagerTemp.Instance.MaskAllDlg();
			float time3 = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time3 > 0.3f)
				{
					DialogManagerTemp.Instance.CancelMaskAllDlg();
					return true;
				}
				time3 += duration;
				return false;
			}));
		}

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitLevelDlg);
		}

		public void CloseBtnClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitLevelDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			CloseBtnClick();
		}

		public void QuitBtnClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitLevelDlg, false);
			if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
			{
				GA.FailLevel(UserDataManager.Instance.GetService().level.ToString());
				GameLogic.Instance.StartAnalyticsIEnumerator(2);
				if (UserDataManager.Instance.GetService().life <= 0 && !UserDataManager.Instance.GetService().NoLifeLevelSend)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("NoLifeLevel", UserDataManager.Instance.GetService().level.ToString());
					Analytics.Event("NoLifeLevel", dictionary);
					Analytics.Event("NoLifeLevelCalculate", dictionary, UserDataManager.Instance.GetService().level);
					UserDataManager.Instance.GetService().NoLifeLevelSend = true;
					UserDataManager.Instance.Save();
				}
			}
			DialogManagerTemp.Instance.ShowDialog(DialogType.EnterGameDlg, 2);
			if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal - 1)
			{
				UserDataManager.Instance.GetService().lifeConsumeTime = DateTime.Now.Ticks / 10000000;
			}
		}
	}
}
