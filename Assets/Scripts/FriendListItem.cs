using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

public class FriendListItem : BaseUI
{
	public Image Head;

	public new Text name;

	public LocalizationText levelText;

	public Button visit;

	public Button Heart;

	public int RankData;

	public Text RankText;

	public Image RankImage;

	private string url = "";

	public string id = "";

	public string level = "";

	protected override void Start()
	{
		base.Start();
	}

	public void HeartClick()
	{
		Analytics.Event("AskOrSendHeart", new Dictionary<string, string> { { "AskOrSendHeart", "SendLife" } });
		DebugUtils.Log(DebugType.NetWork, "AskOrSendHeart | SendLife");
		if (!FacebookUtilities.Instance.CheckFacebookLogin())
		{
			Analytics.Event("AskOrSendHeart", new Dictionary<string, string> { { "AskOrSendHeart", "SendLifeFail" } });
			DebugUtils.Log(DebugType.NetWork, "AskOrSendHeart | SendLifeFail");
			DialogManagerTemp.Instance.ShowDialog(DialogType.FacebookConnectFailDlg);
			return;
		}
		FriendListPanelManager.Instance.Loading.SetActive(true);
		
	}

	private void ActiveSelf(bool isOn)
	{
	}

	public void SetUrl(string url)
	{
		this.url = url;
		FacebookUtilities.Instance.GetPictureFromUrl(Head, url);
	}

	public string GetUrl()
	{
		return url;
	}

	public void SetName(string name)
	{
		this.name.text = name;
	}

	public string GetName()
	{
		return name.text;
	}

	public void SetId(string id)
	{
		this.id = id;
	}

	public string GetId()
	{
		return id;
	}

	public void SetLevel(string Level)
	{
		level = Level;
		levelText.text = string.Format(LanguageConfig.GetString("FriendListDlg_Lv"), level);
	}

	public void UpdateLevelText()
	{
		levelText.text = string.Format(LanguageConfig.GetString("FriendListDlg_Lv"), level);
	}

	public void SetRankData(int data)
	{
		RankData = data;
		RankText.text = string.Concat(data);
		switch (data)
		{
		case 1:
			RankImage.sprite = Resources.Load<GameObject>("Textures/Elements2/jin").GetComponent<SpriteRenderer>().sprite;
			RankImage.gameObject.SetActive(true);
			break;
		case 2:
			RankImage.sprite = Resources.Load<GameObject>("Textures/Elements2/yin").GetComponent<SpriteRenderer>().sprite;
			RankImage.gameObject.SetActive(true);
			break;
		case 3:
			RankImage.sprite = Resources.Load<GameObject>("Textures/Elements2/tong").GetComponent<SpriteRenderer>().sprite;
			RankImage.gameObject.SetActive(true);
			break;
		default:
			RankImage.gameObject.SetActive(false);
			break;
		}
	}

	public int GetRankData()
	{
		return RankData;
	}

	public void UpdateInfo(string id, string name, string level, string url, int rank)
	{
		DebugUtils.Log(DebugType.NetWork, string.Format("update FriendList info \nid : {0} \nname : {1} \nlevel : {2} \nurl : {3} \n rank : {4} ", id, name, level, url, rank));
		this.id = id;
		SetName(name);
		SetLevel(level);
		SetRankData(rank);
		if (url != GetUrl())
		{
			SetUrl(url);
		}
		if (visit != null && Heart != null)
		{
			visit.interactable = false;
			Heart.interactable = !UserDataManager.Instance.GetService().sentGiftFriendIds.Contains(id);
		}
	}

	public void UpdateSendList()
	{
		if (Heart != null)
		{
			Heart.interactable = !UserDataManager.Instance.GetService().sentGiftFriendIds.Contains(id);
		}
	}
}
