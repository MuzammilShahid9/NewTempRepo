using System;
using UnityEngine.Purchasing;

[Serializable]
public class IAPItem
{
	public string richProductID;

	public string productID;

	public ProductType productType;

	public int coinNum;

	public int price;

	public bool removeAds;

	public bool onetime;

	public int tag;
}
