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
	public class SaleDlg : BaseDialog
	{
		private static SaleDlg instance;

		public GameObject BG2;

		public LocalizationText buyNewBtn;

		public GameObject Loading;

		private string currentBuyName = "Sale_1";

		public ShopConfigData shopdata;

		public Transform packageNewRoot;

		private GameObject pack;

		private int id;

		public Text SalePrice;

		public Text FullPrice;

		private string FullPriceID = GeneralConfig.PackageName + ".sale";

		public static SaleDlg Instance
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

		public override void Show()
		{
			base.Show();
			SalePrice.text = Purchaser.Instance.GetProductPrice(shopdata.ID + ShopConfig.GetShopItemCount());
			FullPrice.text = Purchaser.Instance.GetProductPrice(FullPriceID);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			if ((bool)obj)
			{
				FullPriceID = GeneralConfig.PackageName + ".sale";
				UserDataManager.Instance.GetService().SaleShowNum++;
				UserDataManager.Instance.GetService().SaleShowTime_L = DateTime.Now.Ticks / 10000000;
				if (UserDataManager.Instance.GetService().isSaling)
				{
					UserDataManager.Instance.GetService().SaleTM = Singleton<PlayGameData>.Instance().gameConfig.SaleTime;
				}
				else
				{
					UserDataManager.Instance.GetService().isSaling = true;
					UserDataManager.Instance.GetService().SaleStartTM = DateTime.Now.Ticks / 10000000;
					UserDataManager.Instance.GetService().SaleTM = Singleton<PlayGameData>.Instance().gameConfig.SaleTime;
				}
				if (pack != null)
				{
					UnityEngine.Object.Destroy(pack);
				}
				id = GetSalePackID();
				if (id == -1)
				{
					DebugUtils.LogError(DebugType.UI, "GetSalePackID is error !");
					return;
				}
				UserDataManager.Instance.GetService().SalePackID = id;
				Loading.SetActive(false);
				shopdata = ShopConfig.saleConfig[id];
				FullPriceID += shopdata.RichMoney;
				if (id < 2)
				{
					BG2.SetActive(true);
					currentBuyName = "SalePackage" + id;
					pack = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameScene/SalePackage" + id) as GameObject);
					pack.transform.SetParent(packageNewRoot, false);
					pack.transform.localPosition = Vector3.zero;
				}
				else
				{
					BG2.SetActive(true);
					currentBuyName = "SaleNewPackage" + id;
					pack = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameScene/SaleNewPackage" + id) as GameObject);
					pack.transform.SetParent(packageNewRoot, false);
					pack.transform.localPosition = Vector3.zero;
				}
				UserDataManager.Instance.Save();
			}
			else if (shopdata.Goods == "")
			{
				id = UserDataManager.Instance.GetService().SalePackID;
				if (id == -1)
				{
					DebugUtils.LogError(DebugType.UI, "GetSalePackID is error !");
					return;
				}
				Loading.SetActive(false);
				shopdata = ShopConfig.saleConfig[id];
				FullPriceID += shopdata.RichMoney;
				if (id < 2)
				{
					BG2.SetActive(true);
					currentBuyName = "SalePackage" + id;
					pack = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameScene/SalePackage" + id) as GameObject);
					pack.transform.SetParent(packageNewRoot, false);
					pack.transform.localPosition = Vector3.zero;
				}
				else
				{
					BG2.SetActive(true);
					currentBuyName = "SaleNewPackage" + id;
					pack = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameScene/SaleNewPackage" + id) as GameObject);
					pack.transform.SetParent(packageNewRoot, false);
					pack.transform.localPosition = Vector3.zero;
				}
			}
			pack.GetComponent<SalePackage>().UpdateInfo(shopdata);
			SalePrice.text = Purchaser.Instance.GetProductPrice(shopdata.ID + ShopConfig.GetShopItemCount());
			FullPrice.text = Purchaser.Instance.GetProductPrice(FullPriceID);
		}

		private void Init()
		{
		}

		private int GetSalePackID()
		{
			if (UserDataManager.Instance.GetService().moneySpend > 20)
			{
				int num = UnityEngine.Random.Range(12, 15);
				DebugUtils.Log(DebugType.UI, "GetSalePackID by high user id is " + num);
				return num;
			}
			if (UserDataManager.Instance.GetService().SaleShowNum <= 1)
			{
				switch (Singleton<PlayGameData>.Instance().gameConfig.FirstSaleType)
				{
				case 1:
					DebugUtils.Log(DebugType.UI, "GetSalePackID id = 0");
					return 0;
				case 2:
					DebugUtils.Log(DebugType.UI, "GetSalePackID id = 1");
					return 1;
				default:
					if (UserDataManager.Instance.GetService().moneySpend <= 0)
					{
						DebugUtils.Log(DebugType.UI, "GetSalePackID by free user id is 0");
						return 0;
					}
					if (UserDataManager.Instance.GetService().moneySpend > 0)
					{
						DebugUtils.Log(DebugType.UI, "GetSalePackID by normal user id is 1");
						return 1;
					}
					return 0;
				}
			}
			if (UserDataManager.Instance.GetService().SaleShowNum % 3 != 1)
			{
				if (UserDataManager.Instance.GetService().moneySpend == 0)
				{
					int num2 = UnityEngine.Random.Range(2, 4);
					DebugUtils.Log(DebugType.UI, "GetSalePackID by free user first of Sale id is " + num2);
					return num2;
				}
				if (UserDataManager.Instance.GetService().moneySpend <= 20)
				{
					int num3 = UnityEngine.Random.Range(4, 10);
					DebugUtils.Log(DebugType.UI, "GetSalePackID by normal user first of Sale id is " + num3);
					return num3;
				}
			}
			else
			{
				if (UserDataManager.Instance.GetService().moneySpend == 0)
				{
					int num4 = UnityEngine.Random.Range(15, 17);
					DebugUtils.Log(DebugType.UI, "GetSalePackID by free user Second of Sale id is " + num4);
					return num4;
				}
				if (UserDataManager.Instance.GetService().moneySpend <= 20)
				{
					int num5 = UnityEngine.Random.Range(10, 12);
					DebugUtils.Log(DebugType.UI, "GetSalePackID by normal user Second of Sale id is " + num5);
					return num5;
				}
			}
			return -1;
		}

		public void Close()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.SaleDlg);
		}

		public void Buy()
		{
			ActiveLoading();
			Purchase(shopdata.ID);
		}

		public void Purchase(int purchasingID)
		{
			GlobalVariables.Purchasing = true;
			GlobalVariables.isSale = true;
			GlobalVariables.isBank = false;
			GlobalVariables.PurchasingID = purchasingID;
			Purchaser.Instance.BuyProduct(purchasingID + ShopConfig.GetShopItemCount());
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
				Analytics.Event("PurchaseSuccess", new Dictionary<string, string> { { "PurchaseSuccess", "Sale" } });
				Analytics.Event("IAPType", new Dictionary<string, string> { { "IAPType", currentBuyName } });
				DebugUtils.Log(DebugType.NetWork, "PurchaseSuccess | Sale");
				DebugUtils.Log(DebugType.NetWork, "IAPType | " + currentBuyName);
			}
			if (SceneManager.GetActiveScene().name == "CastleScene")
			{
				CastleSceneUIManager.Instance.UpdateBtnText();
			}
			UserDataManager.Instance.GetService().isSaling = false;
			UserDataManager.Instance.GetService().SaleStartTM = -1L;
			UserDataManager.Instance.GetService().SaleTM = -1L;
			DialogManagerTemp.Instance.CloseDialog(DialogType.SaleDlg, false, false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg, arg);
		}

		private void ProcessPurchaseFail(uint iMessageType, object arg)
		{
			HideLoading();
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("PurchaseFail", new Dictionary<string, string> { 
				{
					"PurchaseFail",
					"Sale_" + currentBuyName
				} });
				DebugUtils.Log(DebugType.NetWork, "PurchaseFail | Sale_" + currentBuyName);
			}
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg);
		}

		private new void OnDisable()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(33u, ProcessPurchaseFail);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(34u, ProcessPurchaseSuccess);
		}

		public void ActiveLoading()
		{
			Loading.SetActive(true);
		}

		public void HideLoading()
		{
			Loading.SetActive(false);
		}
	}
}
