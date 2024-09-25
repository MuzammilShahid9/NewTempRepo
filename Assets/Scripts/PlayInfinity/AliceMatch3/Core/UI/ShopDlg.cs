using System;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayInfinity.AliceMatch3.Core.UI
{
	public class ShopDlg : BaseDialog
	{
		private static ShopDlg instance;

		public GameObject firstEnterPanel;

		public GameObject allGoodsPanel;

		public GameObject shopSonPanelCoin;

		public GameObject shopSonPanelPack;

		public GameObject firstEnterPanelGridPack;

		public GameObject firstEnterPanelGridCoin;

		public GameObject allGoodsPanelGrid;

		public GameObject CoinsGrid;

		public GameObject moreGoodsBtn;

		public GameObject Loading;

		private List<ShopSonPanel> coinSon = new List<ShopSonPanel>();

		private List<GameObject> packageSon = new List<GameObject>();

		private string From = "";

		public int num;

		public GameObject sonGrid;

		private string currentBuyName = "";

		public static ShopDlg Instance
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

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.ShopDlg);
		}

		private new void OnEnable()
		{
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(33u, ProcessPurchaseFail);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(34u, ProcessPurchaseSuccess);
		}

		private void ProcessPurchaseSuccess(uint iMessageType, object arg)
		{
			HideLoading();
			string from = From;
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("PurchaseSuccess", new Dictionary<string, string> { { "PurchaseSuccess", from } });
				Analytics.Event("IAPType", new Dictionary<string, string> { { "IAPType", currentBuyName } });
				DebugUtils.Log(DebugType.NetWork, "PurchaseSuccess | " + from);
				DebugUtils.Log(DebugType.NetWork, "IAPType | " + currentBuyName);
			}
			currentBuyName = "";
			if (SceneManager.GetActiveScene().name == "CastleScene")
			{
				CastleSceneUIManager.Instance.UpdateBtnText();
			}
			DialogManagerTemp.Instance.CloseDialog(DialogType.ShopDlg, false, false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg, arg);
		}

		private void ProcessPurchaseFail(uint iMessageType, object arg)
		{
			HideLoading();
			string from = From;
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("PurchaseFail", new Dictionary<string, string> { 
				{
					"PurchaseFail",
					from + "_" + currentBuyName
				} });
				DebugUtils.Log(DebugType.NetWork, "PurchaseFail | " + from + "_" + currentBuyName);
			}
			currentBuyName = "";
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg);
		}

		private new void OnDisable()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(33u, ProcessPurchaseFail);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(34u, ProcessPurchaseSuccess);
		}

		public override void Show(object obj)
		{
			From = "";
			base.Show(obj);
			From = Convert.ToString(obj);
			DebugUtils.Log(DebugType.Other, "From : " + From);
			if (UserDataManager.Instance.GetService().coin < 5000000)
			{
				Analytics.Event("OpenShopDlg", new Dictionary<string, string> { { "OpenShopDlg", From } });
				DebugUtils.Log(DebugType.NetWork, "OpenShopDlg | " + From);
			}
			HideLoading();
			moreGoodsBtn.SetActive(true);
			firstEnterPanel.SetActive(true);
			allGoodsPanel.SetActive(false);
			for (int i = 0; i < coinSon.Count; i++)
			{
				UnityEngine.Object.Destroy(coinSon[i].gameObject);
			}
			for (int j = 0; j < packageSon.Count; j++)
			{
				UnityEngine.Object.Destroy(packageSon[j].gameObject);
			}
			coinSon.Clear();
			packageSon.Clear();
			for (int k = 0; k < ShopConfig.shopConfig.Count; k++)
			{
				if (ShopConfig.shopConfig[k].IsPack == 0)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(shopSonPanelCoin);
					gameObject.GetComponent<ShopSonPanel>().Enter(ShopConfig.shopConfig[k]);
					if (num == 0)
					{
						sonGrid = UnityEngine.Object.Instantiate(CoinsGrid, allGoodsPanelGrid.transform);
						num = 1;
						gameObject.transform.SetParent(sonGrid.transform, false);
						packageSon.Add(sonGrid);
					}
					else
					{
						num = 0;
						gameObject.transform.SetParent(sonGrid.transform, false);
					}
					if (ShopConfig.shopConfig[k].FirstEnterShow == 1)
					{
						ShopSonPanel component = UnityEngine.Object.Instantiate(shopSonPanelCoin, firstEnterPanelGridCoin.transform).GetComponent<ShopSonPanel>();
						component.Enter(ShopConfig.shopConfig[k]);
						coinSon.Add(component);
					}
				}
				else if (ShopConfig.shopConfig[k].IsPack == 1)
				{
					ShopSonPanel component2 = UnityEngine.Object.Instantiate(shopSonPanelPack, allGoodsPanelGrid.transform).GetComponent<ShopSonPanel>();
					component2.Enter(ShopConfig.shopConfig[k]);
					packageSon.Add(component2.gameObject);
					if (ShopConfig.shopConfig[k].FirstEnterShow == 1)
					{
						ShopSonPanel component3 = UnityEngine.Object.Instantiate(shopSonPanelPack, firstEnterPanelGridPack.transform).GetComponent<ShopSonPanel>();
						component3.Enter(ShopConfig.shopConfig[k]);
						coinSon.Add(component3);
					}
				}
			}
			Vector2 size = firstEnterPanelGridPack.GetComponent<RectTransform>().rect.size;
			Vector2 size2 = shopSonPanelCoin.GetComponent<RectTransform>().rect.size;
			allGoodsPanelGrid.GetComponent<RectTransform>().sizeDelta = new Vector2((float)packageSon.Count * (size2.x + 21f), size.y);
		}

		public void Purchase(int purchasingID, string name)
		{
			currentBuyName = name;
			GlobalVariables.Purchasing = true;
			GlobalVariables.isSale = false;
			GlobalVariables.isBank = false;
			GlobalVariables.PurchasingID = purchasingID;
			Purchaser.Instance.BuyProduct(purchasingID);
		}

		public void CoinBtnClicked()
		{
			firstEnterPanel.SetActive(true);
			allGoodsPanel.SetActive(false);
		}

		public void PackageBtnClicked()
		{
			firstEnterPanel.SetActive(false);
			allGoodsPanel.SetActive(true);
			moreGoodsBtn.SetActive(false);
			foreach (GameObject item in packageSon)
			{
				item.GetComponentInChildren<ShopSonPanel>().Show();
			}
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
