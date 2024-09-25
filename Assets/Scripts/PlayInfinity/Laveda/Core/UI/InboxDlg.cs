using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class InboxDlg : BaseDialog
	{
		private static InboxDlg instance;

		public GameObject NoConnectTip;

		public GameObject ScrollView;

		public Button AcceptButton;

		public Button SendButton;

		public Transform Grid;

		public GameObject InviteBtn;

		public LocalizationText noMessage;

		public GameObject Loading;

		private Dictionary<string, RequestItem> sendLifedic = new Dictionary<string, RequestItem>();

		private Dictionary<string, RequestItem> askLifedic = new Dictionary<string, RequestItem>();

		public static InboxDlg Instance
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
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(26u, GetMyFaceBookRequest);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(36u, FullHeart);
		}

		private void FullHeart(uint iMessageType, object arg)
		{
			AcceptButton.interactable = false;
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			GetData();
		}

		public override void Show()
		{
			base.Show();
			GetData();
		}

		public void GetData()
		{
			noMessage.gameObject.SetActive(false);
			NoConnectTip.SetActive(false);
			Loading.SetActive(false);
			InviteBtn.SetActive(false);
			ScrollView.SetActive(true);
			DebugUtils.Log(DebugType.NetWork, "GetMyFaceBookRequest sendLifeMessages count is  ######### " + FacebookUtilities.Instance.sendLifeMessages.Count + " AskLifeMessages count is  " + FacebookUtilities.Instance.AskLifeMessages.Count);
			if (FacebookUtilities.Instance.sendLifeMessages.Count == 0)
			{
				SendButton.interactable = false;
			}
			else
			{
				SendButton.interactable = true;
			}
			if (FacebookUtilities.Instance.AskLifeMessages.Count == 0 || UserDataManager.Instance.GetService().life == GeneralConfig.LifeTotal)
			{
				AcceptButton.interactable = false;
			}
			else if (FacebookUtilities.Instance.AskLifeMessages.Count > 0 && UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal)
			{
				AcceptButton.interactable = true;
			}
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				BanButton(true);
				NoConnectTip.SetActive(true);
				ScrollView.SetActive(false);
				List<GameObject> list = new List<GameObject>();
				foreach (KeyValuePair<string, RequestItem> item in sendLifedic)
				{
					list.Add(item.Value.gameObject);
				}
				foreach (KeyValuePair<string, RequestItem> item2 in askLifedic)
				{
					list.Add(item2.Value.gameObject);
				}
				for (int i = 0; i < list.Count; i++)
				{
					Object.Destroy(list[i]);
				}
				list.Clear();
				askLifedic.Clear();
				sendLifedic.Clear();
				return;
			}
			if (IsInBoxClear())
			{
				BanButton(true);
				noMessage.gameObject.SetActive(true);
				InviteBtn.SetActive(true);
				ScrollView.SetActive(false);
				return;
			}
			foreach (KeyValuePair<string, RequestItem> item3 in sendLifedic)
			{
				item3.Value.SetText();
			}
			foreach (KeyValuePair<string, RequestItem> item4 in askLifedic)
			{
				item4.Value.SetText();
			}
		}

		public void Connect()
		{
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				FacebookUtilities.Instance.LoginWithFacebook();
			}
		}

		private void GetMyFaceBookRequest(uint iMessageType, object arg)
		{
			NoConnectTip.SetActive(false);
			DebugUtils.Log(DebugType.NetWork, "GetMyFaceBookRequest sendLifeMessages count is " + FacebookUtilities.Instance.sendLifeMessages.Count + " AskLifeMessages count is  " + FacebookUtilities.Instance.AskLifeMessages.Count);
			foreach (KeyValuePair<string, string> sendLifeMessage in FacebookUtilities.Instance.sendLifeMessages)
			{
				string key = sendLifeMessage.Key;
				string value = sendLifeMessage.Value;
				if (sendLifedic.ContainsKey(value))
				{
					RequestItem requestItem = sendLifedic[value];
					if (requestItem.name != FacebookUtilities.Instance.friendsData[key]["name"])
					{
						requestItem.SetName(FacebookUtilities.Instance.friendsData[key]["name"]);
						requestItem.SetText();
					}
					if (requestItem.GetUrl() != FacebookUtilities.Instance.friendsData[key]["picture"])
					{
						requestItem.SetUrl(FacebookUtilities.Instance.friendsData[key]["picture"]);
					}
				}
				else
				{
					DebugUtils.Log(DebugType.NetWork, "Create one InboxItem sendLife ");
					GameObject obj = Object.Instantiate(GeneralConfig.ItemCollect[3]);
					RequestItem component = obj.GetComponent<RequestItem>();
					component.SetName(FacebookUtilities.Instance.friendsData[key]["name"]);
					component.SetMessage(" Give me a Life !");
					component.SetMessageID(value);
					component.SetTargetID(key);
					component.SetUrl(FacebookUtilities.Instance.friendsData[key]["picture"]);
					obj.transform.SetParent(Grid, false);
					sendLifedic.Add(value, component);
					DebugUtils.Log(DebugType.NetWork, "Finish Create one InboxItem name is --- " + component.GetName() + "  messageis is " + value);
				}
			}
			foreach (KeyValuePair<string, string> askLifeMessage in FacebookUtilities.Instance.AskLifeMessages)
			{
				string key2 = askLifeMessage.Key;
				string value2 = askLifeMessage.Value;
				if (askLifedic.ContainsKey(value2))
				{
					RequestItem requestItem2 = askLifedic[value2];
					if (requestItem2.name != FacebookUtilities.Instance.friendsData[key2]["name"])
					{
						requestItem2.SetName(FacebookUtilities.Instance.friendsData[key2]["name"]);
						requestItem2.SetText();
					}
					if (requestItem2.GetUrl() != FacebookUtilities.Instance.friendsData[key2]["picture"])
					{
						requestItem2.SetUrl(FacebookUtilities.Instance.friendsData[key2]["picture"]);
					}
				}
				else
				{
					DebugUtils.Log(DebugType.NetWork, "Create one InboxItem askLife ");
					GameObject obj2 = Object.Instantiate(GeneralConfig.ItemCollect[3]);
					RequestItem component2 = obj2.GetComponent<RequestItem>();
					component2.SetName(FacebookUtilities.Instance.friendsData[key2]["name"]);
					component2.SetMessage(" Give you a Life !");
					component2.SetMessageID(value2);
					component2.SetTargetID(key2);
					component2.SetUrl(FacebookUtilities.Instance.friendsData[key2]["picture"]);
					obj2.transform.SetParent(Grid, false);
					askLifedic.Add(value2, component2);
					DebugUtils.Log(DebugType.NetWork, "Finish Create one InboxItem name is --- " + component2.GetName() + "  messageis is " + value2);
				}
			}
			Loading.SetActive(false);
			if (IsInBoxClear())
			{
				noMessage.gameObject.SetActive(true);
				InviteBtn.SetActive(true);
				ScrollView.SetActive(false);
			}
		}

		private void BanButton(bool isBan)
		{
			AcceptButton.interactable = !isBan;
			SendButton.interactable = !isBan;
		}

		public void DelOneItem(string messageId, bool isSend)
		{
			if (isSend)
			{
				if (sendLifedic.ContainsKey(messageId))
				{
					Object.Destroy(sendLifedic[messageId].gameObject);
					sendLifedic.Remove(messageId);
				}
			}
			else if (askLifedic.ContainsKey(messageId))
			{
				Object.Destroy(askLifedic[messageId].gameObject);
				askLifedic.Remove(messageId);
			}
			if (sendLifedic.Count == 0)
			{
				SendButton.interactable = false;
			}
			else
			{
				SendButton.interactable = true;
			}
			if (askLifedic.Count == 0)
			{
				AcceptButton.interactable = false;
			}
			else
			{
				AcceptButton.interactable = true;
			}
			if (sendLifedic.Count == 0 && askLifedic.Count == 0)
			{
				noMessage.gameObject.SetActive(true);
				InviteBtn.SetActive(true);
				ScrollView.SetActive(false);
				CastleSceneUIManager.Instance.ShakeInBoxBtn(false);
			}
		}

		public void SendButtonClick()
		{
			Analytics.Event("AskOrSendHeart", new Dictionary<string, string> { { "AskOrSendHeart", "SendLife" } });
			DebugUtils.Log(DebugType.NetWork, "AskOrSendHeart | SendLife");
			List<string> IdList = new List<string>();
			List<string> messageIdList = new List<string>();
			foreach (KeyValuePair<string, string> sendLifeMessage in FacebookUtilities.Instance.sendLifeMessages)
			{
				IdList.Add(sendLifeMessage.Key);
				messageIdList.Add(sendLifeMessage.Value);
			}
			
		}

		public void AcceptAllButton()
		{
			int num = GeneralConfig.LifeTotal - UserDataManager.Instance.GetService().life;
			int num2 = 0;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (FacebookUtilities.Instance.AskLifeMessages.Count == num)
			{
				AcceptButton.interactable = false;
			}
			foreach (KeyValuePair<string, string> askLifeMessage in FacebookUtilities.Instance.AskLifeMessages)
			{
				if (num2 != num)
				{
					list.Add(askLifeMessage.Key);
					list2.Add(askLifeMessage.Value);
					LifeManager.Instance.AddUserLife(1);
					num2++;
					continue;
				}
				AcceptButton.interactable = false;
				break;
			}
			foreach (string item in list2)
			{
				DelOneItem(item, false);
			}
			foreach (string item2 in list)
			{
				FacebookUtilities.Instance.AskLifeMessages.Remove(item2);
			}
			if (FacebookUtilities.Instance.AskLifeMessages.Count > 0)
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.NoticeDlg);
			}
			UserDataManager.Instance.Save();
		}

		private void OnDestroy()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(26u, GetMyFaceBookRequest);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(36u, FullHeart);
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.InboxDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		public bool IsInBoxClear()
		{
			if (FacebookUtilities.Instance.sendLifeMessages.Count == 0)
			{
				return FacebookUtilities.Instance.AskLifeMessages.Count == 0;
			}
			return false;
		}

		public void InviteFriend()
		{
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.FacebookConnectFailDlg);
			}
			
		}

		public override void BeCover(DialogType type)
		{
			if (type != DialogType.NoticeDlg)
			{
				base.BeCover(type);
			}
			isAniming = false;
			isCanOpen = true;
			isCanClose = true;
		}
	}
}
