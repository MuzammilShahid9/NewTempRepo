using System.Collections.Generic;
using UnityEngine;

public class ShopConfig
{
	public static List<ShopConfigData> shopConfig;

	public static List<ShopConfigData> saleConfig;

	public static void Load()
	{
		DebugUtils.Log(DebugType.Other, "Processing Shop Infos...");
		shopConfig = JsonUtility.FromJson<ShopConfigDataList>((Resources.Load("Config/Shop/ShopConfig") as TextAsset).text).data;
		DebugUtils.Log(DebugType.Other, "Processing Sale Infos...");
		saleConfig = JsonUtility.FromJson<ShopConfigDataList>((Resources.Load("Config/Sale/SaleConfig") as TextAsset).text).data;
	}

	public static ShopConfigData GetshopData(int purchasingID)
	{
		ShopConfigData result = new ShopConfigData();
		for (int i = 0; i < shopConfig.Count; i++)
		{
			if (shopConfig[i].ID == purchasingID)
			{
				result = shopConfig[i];
			}
		}
		return result;
	}

	public static ShopConfigData GetSaleData(int purchasingID)
	{
		ShopConfigData result = new ShopConfigData();
		for (int i = 0; i < saleConfig.Count; i++)
		{
			if (saleConfig[i].ID == purchasingID)
			{
				result = saleConfig[i];
			}
		}
		return result;
	}

	public static int GetShopItemCount()
	{
		return shopConfig.Count;
	}

	public static int GetSaleItemCount()
	{
		return saleConfig.Count;
	}
}
