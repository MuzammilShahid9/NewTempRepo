using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using PlayInfinity.Leah.Core;
using UnityEngine.UI;

public class RequestItem : BaseUI
{
	public Image Head;

	public LocalizationText messageText;

	public LocalizationText nameText;

	public Button button;

	private string url;

	public string messageId;

	public string id;

	private string message;

	private new string name;

	private bool isSendButton;

	private string key = "";

	public void SetMessage(string info)
	{
		if (info == " Give me a Life !")
		{
			isSendButton = true;
			key = "InboxDlg_AskForYouALife";
		}
		else
		{
			key = "InboxDlg_SendYouALife";
		}
		SetText();
		UpdateButton();
	}

	private void UpdateButton()
	{
		if (isSendButton)
		{
			button.GetComponentInChildren<LocalizationText>().SetKeyString("InboxDlg_Send");
			button.onClick.AddListener(delegate
			{
				
			});
			return;
		}
		button.GetComponentInChildren<LocalizationText>().SetKeyString("InboxDlg_Accept");
		button.onClick.AddListener(delegate
		{
			if (UserDataManager.Instance.GetService().life >= 5)
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.NoticeDlg);
			}
			else
			{
				FacebookUtilities.Instance.AskLifeMessages.Remove(messageId);
				InboxDlg.Instance.DelOneItem(messageId, isSendButton);
				LifeManager.Instance.AddUserLife(1);
				UserDataManager.Instance.Save();
			}
		});
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

	public void SetText()
	{
		nameText.text = name;
		messageText.text = string.Format(LanguageConfig.GetString(key), name);
	}

	public string GetName()
	{
		return name;
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public void SetMessageID(string id)
	{
		messageId = id;
	}

	public void SetTargetID(string id)
	{
		this.id = id;
	}
}
