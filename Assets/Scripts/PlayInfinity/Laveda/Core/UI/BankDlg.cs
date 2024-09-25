using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class BankDlg : BaseDialog
	{
		private static BankDlg instance;

		public int CurrentBankNum;

		public GameObject Loading;

		private string ProductID = "";

		public LocalizationText buyNewBtn;

		public GameObject InfoPanel;

		public Text NowGoldNum;

		public Button buyBtn;

		public RectTransform rect;

		public bool isInit = true;

		public GameObject lockBtn;

		public LocalizationText info;

		public static BankDlg Instance
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
			CurrentBankNum = UserDataManager.Instance.GetBankNum();
		}

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.BankDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			RefreshCoin();
			if (Purchaser.Instance.GetProductPrice(ProductID) != LanguageConfig.GetString("Network_Unavailable"))
			{
				buyNewBtn.text = string.Format(LanguageConfig.GetString("KittyBank_Buy"), Purchaser.Instance.GetProductPrice(ProductID));
			}
			else
			{
				buyNewBtn.text = LanguageConfig.GetString("Network_Unavailable");
			}
		}

		public override void Show()
		{
			base.Show();
			RefreshCoin();
			if (Purchaser.Instance.GetProductPrice(ProductID) != LanguageConfig.GetString("Network_Unavailable"))
			{
				buyNewBtn.text = string.Format(LanguageConfig.GetString("KittyBank_Buy"), Purchaser.Instance.GetProductPrice(ProductID));
			}
			else
			{
				buyNewBtn.text = LanguageConfig.GetString("Network_Unavailable");
			}
		}

		public void BtnCloseClicked()
		{
			InfoPanel.SetActive(false);
			DialogManagerTemp.Instance.CloseDialog(DialogType.BankDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public void RefreshCoin()
		{
			CurrentBankNum = UserDataManager.Instance.GetBankNum();
			ProductID = GeneralConfig.PackageName + ".bank6";
			NowGoldNum.text = string.Concat(CurrentBankNum);
			InfoPanel.SetActive(false);
			float num = (float)CurrentBankNum / 12000f;
			buyBtn.interactable = CurrentBankNum >= 6999;
			lockBtn.SetActive(!buyBtn.interactable);
			rect.sizeDelta = new Vector2(713f * num, 26.14f);
			if (CastleSceneUIManager.Instance != null)
			{
				CastleSceneUIManager.Instance.BankNum.text = ((CurrentBankNum >= 12000) ? LanguageConfig.GetString("UI_Full") : string.Concat(CurrentBankNum));
			}
			if (CurrentBankNum >= 7000 && CurrentBankNum < 12000)
			{
				info.SetKeyString("Bank_Detail7000");
			}
			else if (CurrentBankNum >= 12000)
			{
				info.SetKeyString("Bank_Detail12000");
			}
			else
			{
				info.SetKeyString("Bank_Detail");
			}
		}

		public void Buy()
		{
			InfoPanel.SetActive(false);
			ActiveLoading();
			Purchase(ProductID);
		}

		public void ActiveLoading()
		{
			Loading.SetActive(true);
		}

		public void HideLoading()
		{
			Loading.SetActive(false);
		}

		public void Purchase(string purchasingID)
		{
			GlobalVariables.Purchasing = true;
			GlobalVariables.isSale = false;
			GlobalVariables.isBank = true;
			Purchaser.Instance.BuyProductID(purchasingID);
		}

		public void ShowInfo()
		{
			InfoPanel.SetActive(!InfoPanel.activeSelf);
		}

		private new void OnEnable()
		{
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(33u, ProcessPurchaseFail);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(34u, ProcessPurchaseSuccess);
		}

		private void ProcessPurchaseSuccess(uint iMessageType, object arg)
		{
			HideLoading();
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("PurchaseSuccess", new Dictionary<string, string> { { "PurchaseSuccess", "Bank" } });
				Analytics.Event("IAPType", new Dictionary<string, string> { { "IAPType", "Bank" } });
				DebugUtils.Log(DebugType.NetWork, "PurchaseSuccess | Bank");
				DebugUtils.Log(DebugType.NetWork, "IAPType | Bank");
			}
			UserDataManager.Instance.SetBankNum(0);
			CastleSceneUIManager.Instance.UpdateBtnText();
			DialogManagerTemp.Instance.CloseDialog(DialogType.BankDlg, false, false);
			InfoPanel.SetActive(false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg, arg);
			RefreshCoin();
		}

		private void ProcessPurchaseFail(uint iMessageType, object arg)
		{
			HideLoading();
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("PurchaseFail", new Dictionary<string, string> { { "PurchaseFail", "Bank_Bank" } });
				DebugUtils.Log(DebugType.NetWork, "PurchaseFail | Bank_Bank");
			}
			InfoPanel.SetActive(false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg);
		}

		private new void OnDisable()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(33u, ProcessPurchaseFail);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(34u, ProcessPurchaseSuccess);
		}
	}
}
