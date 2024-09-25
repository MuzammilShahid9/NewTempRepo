using System;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class GetLifeDlg : BaseDialog
	{
		public LocalizationText upContent;

		public LocalizationText downContent;

		public Text timeContent;

		public Button askForLifeBtn;

		public Button buyLifeBtn;

		public Image lifeImage;

		public GameObject lifeEffect;

		public Text HeartNum;

		public GameObject wuxian;

		private static GetLifeDlg instance;

		public static GetLifeDlg Instance
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
			lifeEffect.SetActive(false);
		}

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.GetLifeDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			lifeEffect.SetActive(true);
			ShowDetail();
		}

		public void AskForLife()
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.AskForLifeDlg, null, DialogType.GetLifeDlg);
		}

		public void ShowDetail()
		{
			lifeImage.GetComponent<Canvas>().sortingLayerName = "UI";
			HeartNum.text = string.Concat(UserDataManager.Instance.GetService().life);
			wuxian.SetActive(false);
			HeartNum.gameObject.SetActive(true);
			if (UserDataManager.Instance.GetService().unlimitedLife)
			{
				HeartNum.gameObject.SetActive(false);
				wuxian.SetActive(true);
				upContent.enabled = true;
				upContent.SetKeyString("GetLifeDlg_UnlimitedLives");
				downContent.SetKeyString("GetLifeDlg_YouHaveUnlimitedLives");
				AdjustBtn(false);
			}
			else if (UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal && UserDataManager.Instance.GetService().life != 0)
			{
				upContent.enabled = true;
				upContent.SetKeyString("GetLifeDlg_TimeToNextLife");
				downContent.SetKeyString("GetLifeDlg_YouHaveLives");
				AdjustBtn(false);
			}
			else if (UserDataManager.Instance.GetService().life == GeneralConfig.LifeTotal)
			{
				upContent.enabled = false;
				downContent.SetKeyString("GetLifeDlg_YouHaveLives");
				AdjustBtn(false);
			}
			else if (UserDataManager.Instance.GetService().life == 0)
			{
				upContent.enabled = true;
				upContent.SetKeyString("GetLifeDlg_TimeToNextLife");
				downContent.SetKeyString("GetLifeDlg_AskOrBuyLives");
				AdjustBtn(true);
			}
		}

		private void AdjustBtn(bool showBuyLifeBtn)
		{
			if (showBuyLifeBtn)
			{
				buyLifeBtn.gameObject.SetActive(true);
				Vector3 localPosition = buyLifeBtn.transform.localPosition;
				askForLifeBtn.transform.localPosition = new Vector3(0f - localPosition.x, localPosition.y, localPosition.z);
			}
			else
			{
				buyLifeBtn.gameObject.SetActive(false);
				Vector3 localPosition2 = askForLifeBtn.transform.localPosition;
				askForLifeBtn.transform.localPosition = new Vector3(0f, localPosition2.y, localPosition2.z);
			}
		}

		public void BuyLifeBtnClicked()
		{
			if (UserDataManager.Instance.GetService().coin >= GeneralConfig.BuyFullLifeCost)
			{
				UserDataManager.Instance.GetService().coin -= GeneralConfig.BuyFullLifeCost;
				UserDataManager.Instance.GetService().life = GeneralConfig.LifeTotal;
				UserDataManager.Instance.Save();
				GA.Buy("RefillLife", 1, GeneralConfig.BuyFullLifeCost);
				GA.Use("RefillLife", 1, GeneralConfig.BuyFullLifeCost);
				if (UserDataManager.Instance.GetService().coin < GeneralConfig.SendNoLifeLevelDataCoinNumber && !UserDataManager.Instance.GetService().NoGoldLevelSend && UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("NoGoldLevel", UserDataManager.Instance.GetService().level.ToString());
					Analytics.Event("NoGoldLevel", dictionary);
					Analytics.Event("NoGoldLevelCalculate", dictionary, UserDataManager.Instance.GetService().level);
					UserDataManager.Instance.GetService().NoGoldLevelSend = true;
					UserDataManager.Instance.Save();
				}
				CastleSceneUIManager.Instance.UpdateBtnText();
				timeContent.text = " ";
				DialogManagerTemp.Instance.MaskAllDlg();
				float time = 0f;
				float time2 = 0f;
				int num = 0;
				UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time > 1f)
					{
						Close();
						DialogManagerTemp.Instance.CancelMaskAllDlg();
						return true;
					}
					if (num == 5)
					{
						ShowDetail();
					}
					if (num < 5 && time2 > 0.142857149f)
					{
						time2 = 0f;
						num++;
						HeartNum.text = string.Concat(num);
						HeartNum.transform.DOScale(1.3f, 0.13f).OnComplete(delegate
						{
							HeartNum.transform.DOScale(1f, 0.13f);
						});
					}
					time += duration;
					time2 += duration;
					return false;
				}));
			}
			else
			{
				string text = "";
				text = ((!(SceneManager.GetActiveScene().name != "CastleScene")) ? "BuyInGameSceneRetry" : "BuyInEnterGamePlay");
				DialogManagerTemp.Instance.OpenShopDlg(text, DialogType.GetLifeDlg);
			}
		}

		private void Update()
		{
			UpdateTimeText();
		}

		private void UpdateTimeText()
		{
			long num = -1L;
			if (UserDataManager.Instance.GetService().unlimitedLife)
			{
				num = UserDataManager.Instance.GetService().unlimitedLifeTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().unlimitedLifeStartTM);
			}
			else
			{
				if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal)
				{
					timeContent.text = " ";
					return;
				}
				if (UserDataManager.Instance.GetService().lifeConsumeTime == -1)
				{
					return;
				}
				num = GeneralConfig.LifeRecoverTime - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().lifeConsumeTime);
			}
			int num2 = (int)num / 3600;
			int num3 = (int)(num - num2 * 60 * 60) / 60;
			int num4 = (int)num - num2 * 60 * 60 - num3 * 60;
			if (num2 > 0)
			{
				timeContent.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0');
				return;
			}
			if (num3 > 0)
			{
				timeContent.text = num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
				return;
			}
			if (num4 > 0)
			{
				timeContent.text = "00:" + num4.ToString().PadLeft(2, '0');
				return;
			}
			timeContent.text = " ";
			ShowDetail();
		}
	}
}
