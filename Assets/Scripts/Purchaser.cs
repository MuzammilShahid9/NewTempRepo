using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.SceneManagement;

public class Purchaser : MonoBehaviour, IStoreListener
{
	private static IStoreController m_StoreController;

	private static IExtensionProvider m_StoreExtensionProvider;

	private IAPItem[] iapItems;

	private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

	private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

	private static Purchaser instance = null;

	public static Purchaser Instance
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
		DebugUtils.Log(DebugType.Other, "Purchaser Start");
		ShopConfig.Load();
		iapItems = new IAPItem[100];
		for (int i = 1; i <= ShopConfig.GetShopItemCount(); i++)
		{
			DebugUtils.Log(DebugType.Other, "init shop coin item " + i);
			ShopConfigData shopConfigData = ShopConfig.GetshopData(i);
			int num = Mathf.RoundToInt(float.Parse(shopConfigData.CostMoney));
			if (shopConfigData.IsPack == 0)
			{
				DebugUtils.Log(DebugType.Other, "init shop coin item " + GeneralConfig.PackageName + ".coin" + num);
				iapItems[i] = new IAPItem();
				iapItems[i].productID = GeneralConfig.PackageName + ".coin" + num;
				iapItems[i].productType = ProductType.Consumable;
				iapItems[i].coinNum = shopConfigData.GoldNum;
				iapItems[i].price = num;
				iapItems[i].onetime = false;
				iapItems[i].tag = i;
				iapItems[i].richProductID = "";
			}
			else
			{
				DebugUtils.Log(DebugType.Other, "init shop pack item " + GeneralConfig.PackageName + ".coin" + num);
				iapItems[i] = new IAPItem();
				iapItems[i].productID = GeneralConfig.PackageName + ".pack" + num;
				iapItems[i].productType = ProductType.Consumable;
				iapItems[i].coinNum = shopConfigData.GoldNum;
				iapItems[i].price = num;
				iapItems[i].onetime = false;
				iapItems[i].tag = i;
				iapItems[i].richProductID = "";
			}
		}
		for (int j = 1; j <= ShopConfig.GetSaleItemCount(); j++)
		{
			DebugUtils.Log(DebugType.Other, "init sale item " + j);
			ShopConfigData saleData = ShopConfig.GetSaleData(j);
			int num2 = Mathf.RoundToInt(float.Parse(saleData.CostMoney));
			int num3 = ShopConfig.GetShopItemCount() + j;
			iapItems[num3] = new IAPItem();
			iapItems[num3].productType = ProductType.Consumable;
			iapItems[num3].coinNum = saleData.GoldNum;
			iapItems[num3].price = num2;
			iapItems[num3].onetime = false;
			iapItems[num3].tag = num3;
			iapItems[num3].productID = GeneralConfig.PackageName + ".sale" + num2;
			iapItems[num3].richProductID = GeneralConfig.PackageName + ".sale" + saleData.RichMoney;
			DebugUtils.Log(DebugType.Other, "init sale pack item " + GeneralConfig.PackageName + ".sale" + num2);
		}
		if (m_StoreController == null)
		{
			InitializePurchasing();
		}
	}

	public void InitializePurchasing()
	{
		if (IsInitialized())
		{
			return;
		}
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		IAPItem[] array = iapItems;
		foreach (IAPItem iAPItem in array)
		{
			if (iAPItem != null)
			{
				configurationBuilder.AddProduct(iAPItem.productID, iAPItem.productType);
				if (iAPItem.richProductID != "")
				{
					configurationBuilder.AddProduct(iAPItem.richProductID, iAPItem.productType);
				}
			}
		}
		configurationBuilder.AddProduct(GeneralConfig.PackageName + ".bank6", ProductType.Consumable);
		UnityPurchasing.Initialize(this, configurationBuilder);
	}

	public bool IsInitialized()
	{
		if (m_StoreController != null)
		{
			return m_StoreExtensionProvider != null;
		}
		return false;
	}

	public void BuyProduct(int index)
	{
		DebugUtils.Log(DebugType.Other, "buy product index " + index);
		DebugUtils.Log(DebugType.Other, "buy product " + iapItems[index].productID);
		BuyProductID(iapItems[index].productID);
	}

	private void GetGoods(int goodsID, int goodsNum)
	{
		switch (goodsID)
		{
		case 1:
			UserDataManager.Instance.GetService().bombNumber += goodsNum;
			break;
		case 2:
			UserDataManager.Instance.GetService().rainBowBallNumber += goodsNum;
			break;
		case 3:
			UserDataManager.Instance.GetService().doubleBeesNumber += goodsNum;
			break;
		case 4:
			UserDataManager.Instance.GetService().malletNumber += goodsNum;
			break;
		case 5:
			UserDataManager.Instance.GetService().magicMalletNumber += goodsNum;
			break;
		case 6:
			UserDataManager.Instance.GetService().gloveNumber += goodsNum;
			break;
		case 7:
			if (UserDataManager.Instance.GetService().unlimitedLife)
			{
				UserDataManager.Instance.GetService().unlimitedLifeTM += goodsNum * 60;
				break;
			}
			UserDataManager.Instance.GetService().unlimitedLife = true;
			UserDataManager.Instance.GetService().unlimitedLifeStartTM = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.GetService().unlimitedLifeTM = goodsNum * 60;
			break;
		}
	}

	private void SendPurchaseAnalytics(ShopConfigData purchasingData)
	{
		int num = Mathf.RoundToInt(float.Parse(purchasingData.CostMoney));
		if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
		{
			GA.Pay(num, GA.PaySource.AppStore, purchasingData.GoldNum);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("af_currency", "USD");
			dictionary.Add("af_revenue", ((double)num * 0.7).ToString());
			dictionary.Add("af_quantity", "1");
			
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["mygame_packagename"] = GeneralConfig.PackageName;

		}
	}

	private void ProcessPurchaseSuccess()
	{
		GlobalVariables.PurchaseSuccess = true;
		ShopConfigData productData = UserDataManager.Instance.GetService().productData;
		if (productData != null)
		{
			if (SceneManager.GetActiveScene().name == "LogoScene")
			{
				DebugUtils.LogError(DebugType.Other, "PurchaseSuccessException");
				Timer.Schedule(this, 1f, delegate
				{
					DebugUtils.ProcessExceptionReportCustom("Crash When Resume!");
				});
				Analytics.Event("PurchaseSuccessException", new Dictionary<string, string> { { "PurchaseSuccessException", "Crash When Resume!" } });
			}
			string[] array = productData.Goods.Split(';');
			UserDataManager.Instance.GetService().coin += productData.GoldNum;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != "")
				{
					int goodsID = int.Parse(array[i].Split(',')[0]);
					int goodsNum = int.Parse(array[i].Split(',')[1]);
					GetGoods(goodsID, goodsNum);
				}
			}
			int num = Mathf.RoundToInt(float.Parse(productData.CostMoney));
			AnalyticsMoneySpend(num);
			UserDataManager.Instance.GetService().moneySpend += num;
			UserDataManager.Instance.GetService().LastBuyTime = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.Save();
			SendPurchaseAnalytics(productData);
			Singleton<MessageDispatcher>.Instance().SendMessage(34u, productData);
			DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg, productData);
		}
		else
		{
			Timer.Schedule(this, 1f, delegate
			{
				DebugUtils.ProcessExceptionReportCustom("No purchasingData");
			});
			DebugUtils.LogError(DebugType.Other, "PurchaseSuccessException-NO");
			Analytics.Event("PurchaseSuccessException", new Dictionary<string, string> { { "PurchaseSuccessException", "No purchasingData" } });
		}
	}

	public void ProcessPurchaseFail()
	{
		GlobalVariables.PurchaseSuccess = false;
		Singleton<MessageDispatcher>.Instance().SendMessage(33u, null);
		DialogManagerTemp.Instance.ShowDialog(DialogType.PurchaseSuccessDlg);
	}

	public void FakeProcessPurchase(string productID)
	{
		ProcessPurchaseSuccess();
	}

	public string GetProductPrice(int productID, out decimal Price)
	{
		Price = default(decimal);
		if (m_StoreController == null)
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		Product product = m_StoreController.products.WithID(iapItems[productID].productID);
		if (product == null)
		{
			DebugUtils.Log(DebugType.Other, productID + " || is error!");
			return LanguageConfig.GetString("Network_Unavailable");
		}
		DebugUtils.Log(DebugType.Other, productID + "    |||    " + iapItems[productID].productID + "    |||    " + product.metadata.localizedPriceString);
		Price = product.metadata.localizedPrice;
		if (!(Price > 0.1m))
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		return product.metadata.localizedPriceString;
	}

	public string temp;
	[ContextMenu("Gett pticr")]
	public void GetProdcutPricee()
	{
		Debug.Log("Price: " + GetProductPrice(temp));
	}

	public string GetProductPrice(int productID)
	{
		if (m_StoreController == null)
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		Product product = m_StoreController.products.WithID(iapItems[productID].productID);
		if (product == null)
		{
			DebugUtils.Log(DebugType.Other, productID + " || is error!");
			return LanguageConfig.GetString("Network_Unavailable");
		}
		decimal localizedPrice = product.metadata.localizedPrice;
		DebugUtils.Log(DebugType.Other, productID + "    |||    " + iapItems[productID].productID + "    |||    " + product.metadata.localizedPriceString);
		if (!(localizedPrice > 0.1m))
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		return product.metadata.localizedPriceString;
	}

	public string GetProductPrice(string productID)
	{
		if (m_StoreController == null)
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		Product product = m_StoreController.products.WithID(productID);
		if (product == null)
		{
			DebugUtils.Log(DebugType.Other, productID + " || is error!");
			return LanguageConfig.GetString("Network_Unavailable");
		}
		decimal localizedPrice = product.metadata.localizedPrice;
		DebugUtils.Log(DebugType.Other, productID + "    |||    " + product.metadata.localizedPriceString);
		if (!(localizedPrice > 0.1m))
		{
			return LanguageConfig.GetString("Network_Unavailable");
		}
		return product.metadata.localizedPriceString;
	}

	public void BuyProductID(string productId)
	{
		DebugUtils.Log(DebugType.Other, "buy product id " + productId);
		ShopConfigData shopConfigData = null;
		shopConfigData = ((!GlobalVariables.isBank) ? (GlobalVariables.isSale ? ShopConfig.GetSaleData(GlobalVariables.PurchasingID) : ShopConfig.GetshopData(GlobalVariables.PurchasingID)) : new ShopConfigData
		{
			CostMoney = "6",
			Goods = "",
			GoldNum = UserDataManager.Instance.GetBankNum()
		});
		UserDataManager.Instance.GetService().productData = shopConfigData;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			DebugUtils.Log(DebugType.Other, "Network NotReachable");
			ProcessPurchaseFail();
		}
		else if (IsInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);
			if (product != null && product.availableToPurchase)
			{
				DebugUtils.Log(DebugType.Other, string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				GlobalVariables.ResumeFromDesktop = false;
				GlobalVariables.Purchasing = true;
				m_StoreController.InitiatePurchase(product);
			}
			else
			{
				ProcessPurchaseFail();
				DebugUtils.Log(DebugType.Other, "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
		else
		{
			DebugUtils.Log(DebugType.Other, "IAP Not IsInitialized");
			ProcessPurchaseFail();
		}
	}

	public void RestorePurchases()
	{
		if (!IsInitialized())
		{
			ProcessPurchaseFail();
			DebugUtils.Log(DebugType.Other, "RestorePurchases FAIL. Not initialized.");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			DebugUtils.Log(DebugType.Other, "RestorePurchases started ...");
			m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(delegate(bool result)
			{
				DebugUtils.Log(DebugType.Other, "RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		else
		{
			ProcessPurchaseFail();
			DebugUtils.Log(DebugType.Other, "RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		DebugUtils.Log(DebugType.Other, "OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		DebugUtils.Log(DebugType.Other, "OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		bool flag = true;
		// crossPlatformValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

		try
		{
			//IPurchaseReceipt[] array = crossPlatformValidator.Validate(args.purchasedProduct.receipt);
			DebugUtils.Log(DebugType.Other, "Receipt is valid. Contents:");
			//IPurchaseReceipt[] array2 = array;
			/*foreach (IPurchaseReceipt purchaseReceipt in array2)
			{
				DebugUtils.Log(DebugType.Other, purchaseReceipt.productID);
				DebugUtils.Log(DebugType.Other, purchaseReceipt.purchaseDate);
				DebugUtils.Log(DebugType.Other, purchaseReceipt.transactionID);
			}*/
		}
		catch (IAPSecurityException)
		{
			ProcessPurchaseFail();
			DebugUtils.Log(DebugType.Other, "Invalid receipt, not unlocking content");
			flag = false;
		}
		DebugUtils.Log(DebugType.Other, string.Concat(args.ToString(), "  ||  ", args.purchasedProduct.receipt, "  ||  ", args.purchasedProduct.definition.id, "  ||  ", args.purchasedProduct.definition.type, "  ||  ", args.purchasedProduct.transactionID, "    &*&*&*&*&   ", GlobalVariables.PurchasingID));
		if (flag)
		{
			DebugUtils.Log(DebugType.Other, "validPurchase success");
			ProcessPurchaseSuccess();
		}
		else
		{
			ProcessPurchaseFail();
			DebugUtils.Log(DebugType.Other, "validPurchase failed");
		}
		Timer.Schedule(this, 60f, delegate
		{
			GlobalVariables.Purchasing = false;
		});
		return PurchaseProcessingResult.Complete;
	}

	private void GiveProduct()
	{
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		DebugUtils.Log(DebugType.Other, string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		ProcessPurchaseFail();
		GlobalVariables.PurchaseSuccess = false;
	}

	public void AnalyticsMoneySpend(int Money)
	{
		StartCoroutine(MoneySpend(Money));
	}

	private IEnumerator MoneySpend(int Money)
	{
		string text = "Android";
		string[] array = Application.version.ToString().Split('.');
		string text2 = "";
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "" && array[i] != "0")
			{
				flag = false;
				text2 += array[i];
			}
			else if (array[i] != "" && array[i] == "0" && !flag)
			{
				text2 += array[i];
			}
		}
		string analyticsMoneySpendAddress = NetworkConfig.AnalyticsMoneySpendAddress;
		string sendString = "CastleStoryAnalytics" + text + "," + text2 + "," + (UserDataManager.Instance.GetLevel() - 1) + "," + Money;
		DebugUtils.LogError(DebugType.NetWork, "   |      " + sendString + "    Ready Send|    ");
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("data", sendString);
		UnityWebRequest www = UnityWebRequest.Post(analyticsMoneySpendAddress, wWWForm);
		yield return www.Send();
		if (!string.IsNullOrEmpty(www.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics Money Spend|    ");
			SendAnalyticsDataFailSave(sendString);
			yield break;
		}
		DebugUtils.Log(DebugType.Other, www.downloadHandler.text);
		if (www.downloadHandler.text != "success")
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics Money Spend 2|    ");
			SendAnalyticsDataFailSave(sendString);
		}
		else
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + sendString + "    Send|    ");
		}
	}

	public void SendAnalyticsDataFailSave(string savaData)
	{
		string text = "";
		if (File.Exists(Application.persistentDataPath + "/MoneySpend.txt"))
		{
			text = File.ReadAllText(Application.persistentDataPath + "/MoneySpend.txt");
		}
		if (text != null && text != "" && text != " ")
		{
			File.WriteAllText(Application.persistentDataPath + "/MoneySpend.txt", text + "|" + savaData);
		}
		else
		{
			File.WriteAllText(Application.persistentDataPath + "/MoneySpend.txt", savaData);
		}
	}

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
