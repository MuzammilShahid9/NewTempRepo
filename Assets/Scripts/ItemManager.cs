using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private static ItemManager instance;

	public List<Item> itemList = new List<Item>();

	private List<ItemConfigData> itemConfig;

	public Dictionary<int, ItemConfigData> itemDictionary = new Dictionary<int, ItemConfigData>();

	public static ItemManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		ItemConfigDataList itemConfigDataList = JsonUtility.FromJson<ItemConfigDataList>((Resources.Load("Config/Item/ItemConfig") as TextAsset).text);
		itemConfig = itemConfigDataList.data;
		for (int i = 0; i < itemConfig.Count; i++)
		{
			itemDictionary.Add(itemConfig[i].ID, itemConfig[i]);
		}
	}

	public void AddItemToList(Item item)
	{
		itemList.Add(item);
	}

	public Item GetItemInfo(int itemID)
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			if (itemID == itemList[i].itemID)
			{
				return itemList[i];
			}
		}
		return null;
	}

	public string[] GetItemImageData(int itemID)
	{
		string image = itemDictionary[itemID].Image;
		if (image != "")
		{
			return image.Split(';');
		}
		return null;
	}

	public string[] GetItemImageNameData(int itemID)
	{
		string imageName = itemDictionary[itemID].ImageName;
		if (imageName != "")
		{
			return imageName.Split(';');
		}
		return null;
	}

	public ItemConfigData GetItemConfInfo(int itemID)
	{
		return itemDictionary[itemID];
	}

	public string GetItemName(int itemID)
	{
		return itemDictionary[itemID].Name;
	}
}
