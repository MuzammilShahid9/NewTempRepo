using PlayInfinity.AliceMatch3.Core.UI;
using UnityEngine;
using UnityEngine.UI;

public class ShopSonPanel : BaseUI
{
	public LocalizationText nameText;

	public Image image;

	public Text goldText;

	public Text costText;

	public ShopConfigData shopdata;

	public GameObject goodsGrid;

	public GameObject goodsGrid2;

	public GameObject goodsItem;

	public GameObject bestImage;

	public GameObject mostImage;

	private int id;

	public void Show()
	{
		if (id == 1)
		{
			if (base.transform.parent.parent.gameObject.activeSelf)
			{
				Timer.Schedule(this, 0.4f, delegate
				{
					bestImage.SetActive(true);
				});
			}
			mostImage.SetActive(false);
			return;
		}
		if (id == 5)
		{
			if (base.transform.parent.parent.gameObject.activeSelf)
			{
				Timer.Schedule(this, 0.4f, delegate
				{
					mostImage.SetActive(true);
				});
			}
			bestImage.SetActive(false);
			return;
		}
		if (bestImage != null)
		{
			bestImage.SetActive(false);
		}
		if (mostImage != null)
		{
			mostImage.SetActive(false);
		}
	}

	public void Enter(ShopConfigData shopConfig)
	{
		base.gameObject.SetActive(true);
		if (nameText != null)
		{
			nameText.SetKeyString(shopConfig.Name);
		}
		id = shopConfig.ID;
		Sprite sprite = Resources.Load<GameObject>("Textures/Elements2/" + shopConfig.ImageName).GetComponent<SpriteRenderer>().sprite;
		image.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
		image.sprite = sprite;
		Show();
		goldText.text = shopConfig.GoldNum.ToString();
		shopdata = shopConfig;
		string productPrice = Purchaser.Instance.GetProductPrice(shopdata.ID);
		if (string.IsNullOrEmpty(productPrice))
		{
			costText.text = "Error";
		}
		else
		{
			costText.text = productPrice;
		}
		if (shopConfig.IsPack == 1)
		{
			DealGoods(shopConfig);
		}
	}

	private void DealGoods(ShopConfigData shopConfig)
	{
		string[] array = shopConfig.Goods.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != ""))
			{
				continue;
			}
			int goodsID = int.Parse(array[i].Split(',')[0]);
			int goodsNum = int.Parse(array[i].Split(',')[1]);
			if (array.Length == 3)
			{
				if (i == 2)
				{
					CreatGoods(goodsID, goodsNum, 2);
				}
				else
				{
					CreatGoods(goodsID, goodsNum, 1);
				}
			}
			else if (i > 2)
			{
				CreatGoods(goodsID, goodsNum, 2);
			}
			else
			{
				CreatGoods(goodsID, goodsNum, 1);
			}
		}
	}

	private void CreatGoods(int goodsID, int goodsNum, int index)
	{
		GameObject gameObject = null;
		gameObject = ((index != 1) ? Object.Instantiate(goodsItem, goodsGrid2.transform) : Object.Instantiate(goodsItem, goodsGrid.transform));
		Image component = gameObject.transform.Find("Image").GetComponent<Image>();
		gameObject.transform.Find("Text").GetComponent<Text>().text = "x" + goodsNum;
		switch (goodsID)
		{
		case 1:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/zhadantong").GetComponent<SpriteRenderer>().sprite;
			break;
		case 2:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/huangguan").GetComponent<SpriteRenderer>().sprite;
			break;
		case 3:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/shuangmifeng").GetComponent<SpriteRenderer>().sprite;
			break;
		case 4:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/shaozi").GetComponent<SpriteRenderer>().sprite;
			break;
		case 5:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/chuizi").GetComponent<SpriteRenderer>().sprite;
			break;
		case 6:
			component.sprite = Resources.Load<GameObject>("Textures/Shop/shoutao").GetComponent<SpriteRenderer>().sprite;
			break;
		}
	}

	public void BuyBtnClicked()
	{
		ShopDlg.Instance.ActiveLoading();
		ShopDlg.Instance.Purchase(shopdata.ID, shopdata.Name);
	}
}
