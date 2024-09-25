using UnityEngine;
using UnityEngine.UI;

public class SalePackage : MonoBehaviour
{
	public Text off;

	public Text Bomb;

	public Text ColorBomb;

	public Text DoubleBee;

	public Text Spoon;

	public Text Hammar;

	public Text Glove;

	public Text Gold;

	public Text Heart;

	public void UpdateInfo(ShopConfigData data)
	{
		string[] array = data.Goods.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "")
			{
				int goodsID = int.Parse(array[i].Split(',')[0]);
				int goodsNum = int.Parse(array[i].Split(',')[1]);
				DealGoods(goodsID, goodsNum);
			}
		}
		if (data.GoldNum > 0)
		{
			Gold.text = string.Concat(data.GoldNum);
		}
		if (data.IsPack > 0)
		{
			off.text = data.IsPack + "%\nOFF";
		}
	}

	private void DealGoods(int goodsID, int goodsNum)
	{
		string text = "";
		if (goodsID == 7)
		{
			int num = goodsNum / 60;
			int num2 = goodsNum % 60;
			if (num > 0)
			{
				text = num + LanguageConfig.GetString("Time_Hour");
			}
			if (num2 > 0)
			{
				text = text + num2 + LanguageConfig.GetString("Time_Minute");
			}
		}
		else
		{
			text = "X" + goodsNum;
		}
		switch (goodsID)
		{
		case 1:
			Bomb.text = text;
			break;
		case 2:
			ColorBomb.text = text;
			break;
		case 3:
			DoubleBee.text = text;
			break;
		case 4:
			Spoon.text = text;
			break;
		case 5:
			Hammar.text = text;
			break;
		case 6:
			Glove.text = text;
			break;
		case 7:
			Heart.text = text;
			break;
		}
	}
}
