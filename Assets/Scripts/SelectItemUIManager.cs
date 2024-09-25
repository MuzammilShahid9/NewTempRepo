using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemUIManager : MonoBehaviour
{
	private ItemAnim selectItem;

	public Image[] imageArray;

	public Image[] bgImageArray;

	public Text[] textArray;

	private int selectedSprite;

	private int selectedIndex;

	public void Show(ItemAnim item)
	{
        Debug.Log("SelectItemUIManager Show");
		base.gameObject.SetActive(true);
		selectItem = item;
		selectedSprite = selectItem.selectImage;
		ShowImage();
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (i == selectItem.selectImage)
			{
				textArray[i].transform.parent.gameObject.SetActive(false);
				bgImageArray[i].gameObject.SetActive(true);
			}
			else
			{
				textArray[i].transform.parent.gameObject.SetActive(false);
				bgImageArray[i].gameObject.SetActive(false);
			}
		}
		if (selectedSprite != -1)
		{
			ImageClick(selectedSprite);
		}
		else if (selectItem.tempSelectIndex != -1)
		{
			ImageClick(selectItem.tempSelectIndex);
		}
	}

	public void ShowAllText()
	{
		textArray[0].text = LanguageConfig.GetString(PlotItemAniManager.Instance.GetItemNameWithID(selectItem.roomID - 1, selectItem.itemID - 1).Split(';')[0]);
		textArray[1].text = LanguageConfig.GetString(PlotItemAniManager.Instance.GetItemNameWithID(selectItem.roomID - 1, selectItem.itemID - 1).Split(';')[1]);
		textArray[2].text = LanguageConfig.GetString(PlotItemAniManager.Instance.GetItemNameWithID(selectItem.roomID - 1, selectItem.itemID - 1).Split(';')[2]);
	}

	public void ShowImage()
	{
		imageArray[0].sprite = selectItem.iconImageArray[0];
		imageArray[1].sprite = selectItem.iconImageArray[1];
		imageArray[2].sprite = selectItem.iconImageArray[2];
	}

	public void SelectFirstImage()
	{
		ImageClick(0);
	}

	public void SelectSecondImage()
	{
		ImageClick(1);
	}

	public void SelectThirdImage()
	{
		ImageClick(2);
	}

	public void ImageClick(int index)
	{
		selectedIndex = index;
		selectItem.SelectShowImage(index);
		ShowText(index);
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (i == index)
			{
				bgImageArray[i].gameObject.SetActive(true);
			}
			else
			{
				bgImageArray[i].gameObject.SetActive(false);
			}
		}
	}

	public void ShowText(int index)
	{
		for (int i = 0; i < textArray.Length; i++)
		{
			if (i == index)
			{
				textArray[i].transform.parent.gameObject.SetActive(false);
			}
			else
			{
				textArray[i].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	public void CancelBtnClick()
	{
		selectItem.ShowImage(UserDataManager.Instance.GetItemUnlockInfo(selectItem.roomID, selectItem.itemID));
		base.gameObject.SetActive(false);
		GlobalVariables.UnderChangeItemControl = false;
		PlotManager.Instance.CancelAllStep();
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
		CastleSceneUIManager.Instance.SelectItemCancelBtnClick();
	}

	public void ConfirmBtnClick()
	{
		selectItem.selectImage = selectedIndex;
		selectedSprite = selectedIndex;
		GlobalVariables.UnderChangeItemControl = false;
		selectItem.ShowImage(selectedSprite);
		if (UserDataManager.Instance.GetItemUnlockInfo(selectItem.roomID, selectItem.itemID) != -1)
		{
			UserDataManager.Instance.AddUnlockItemInfo(selectItem.roomID, selectItem.itemID, selectItem.selectImage);
			UserDataManager.Instance.Save();
		}
		else if (UserDataManager.Instance.GetCoin() < 5000000)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("SelectIndex", selectItem.selectImage.ToString());
			Analytics.Event("Build_" + (selectItem.roomID - 1) + "_" + (selectItem.itemID - 1) + "_ItemSelectIndex", dictionary);
			dictionary.Clear();
		}
		BuildManager.Instance.SelectFinish();
		base.gameObject.SetActive(false);
		if (PlotManager.Instance.isPlotFinish)
		{
			CastleSceneUIManager.Instance.ShowAllBtn();
		}
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
	}
}
