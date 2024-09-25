using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class AskForLifeDlg : BaseDialog
	{
		public Toggle selectAll;

		public Button selectAllBtn;

		public Transform grid;

		public GameObject Loading;

		public Text tip;

		public Text inviteTip;

		public GameObject InviteButton;

		public GameObject AskButton;

		public GameObject Connect;

		private static AskForLifeDlg instance;

		public Dictionary<string, FriendItem> friendItemDic = new Dictionary<string, FriendItem>();

		public Dictionary<string, FriendItem> ActivefriendItemDic = new Dictionary<string, FriendItem>();

		private Dictionary<string, Dictionary<string, string>> TotalfriendsData;

		public static AskForLifeDlg Instance
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
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(25u, GetMyFriendFaceBookData);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(35u, RefreshAskCD);
		}

		public void RefreshAskCD(uint iMessageType, object arg)
		{
			DebugUtils.Log(DebugType.NetWork, "Refresh Ask CD !");
			if (TotalfriendsData != null)
			{
				DebugUtils.Log(DebugType.NetWork, "Refresh Ask CD ! ------- TotalfriendsData != null");
				GetMyFriendFaceBookData(999u, TotalfriendsData);
			}
		}

		private void GetMyFriendFaceBookData(uint iMessageType, object arg)
		{
			Loading.SetActive(false);
			Connect.SetActive(false);
			selectAll.gameObject.SetActive(true);
			selectAllBtn.gameObject.SetActive(true);
			AskButton.GetComponent<Button>().interactable = true;
			TotalfriendsData = (Dictionary<string, Dictionary<string, string>>)arg;
			Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>(TotalfriendsData);
			if (dictionary.Count == 0)
			{
				DebugUtils.Log(DebugType.NetWork, "Invite your friends!!!!!!!!!!");
				inviteTip.gameObject.SetActive(true);
				InviteButton.SetActive(true);
				AskButton.SetActive(false);
				selectAll.gameObject.SetActive(false);
				selectAllBtn.gameObject.SetActive(false);
				return;
			}
			inviteTip.gameObject.SetActive(false);
			DebugUtils.Log(DebugType.NetWork, "Check is Be Ban!");
			DebugUtils.Log(DebugType.NetWork, "del be ban friend");
			foreach (string sentGiftFriendId in UserDataManager.Instance.GetService().sentGiftFriendIds)
			{
				if (dictionary.ContainsKey(sentGiftFriendId))
				{
					dictionary.Remove(sentGiftFriendId);
				}
			}
			if (dictionary.Count == 0)
			{
				DebugUtils.Log(DebugType.NetWork, "All Ban");
				tip.gameObject.SetActive(true);
				selectAll.gameObject.SetActive(false);
				selectAllBtn.gameObject.SetActive(false);
				ChangeButton(false);
			}
			else
			{
				ChangeButton(true);
			}
			List<string> list = new List<string>();
			DebugUtils.Log(DebugType.NetWork, "Del Be del Friend");
			foreach (KeyValuePair<string, FriendItem> item in friendItemDic)
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					list.Add(item.Key);
				}
			}
			foreach (string item2 in list)
			{
				Object.Destroy(friendItemDic[item2].gameObject);
				friendItemDic.Remove(item2);
			}
			DebugUtils.Log(DebugType.NetWork, "update Friend info");
			foreach (KeyValuePair<string, Dictionary<string, string>> item3 in dictionary)
			{
				string key = item3.Key;
				string text = item3.Value["name"];
				string text2 = item3.Value["picture"];
				if (friendItemDic.ContainsKey(key))
				{
					DebugUtils.Log(DebugType.NetWork, "update Friend info id is :" + key);
					if (friendItemDic[key].GetName() != text)
					{
						friendItemDic[key].SetName(text);
					}
					if (friendItemDic[key].GetUrl() != text2)
					{
						friendItemDic[key].SetUrl(text2);
					}
				}
				else
				{
					DebugUtils.Log(DebugType.NetWork, "Create New Friend info id is :" + key);
					GameObject obj = Object.Instantiate(GeneralConfig.ItemCollect[2]);
					FriendItem component = obj.GetComponent<FriendItem>();
					component.toggle.isOn = false;
					obj.transform.SetParent(grid, false);
					component.SetName(text);
					component.SetId(key);
					component.SetUrl(text2);
					friendItemDic.Add(key, component);
				}
			}
			if (base.gameObject.activeSelf)
			{
				CheckAll();
			}
		}

		public void RefreshCD()
		{
		}

		private void ChangeButton(bool isCanAskLife)
		{
			DebugUtils.Log(DebugType.NetWork, "isCanAskLife is :" + isCanAskLife);
			InviteButton.SetActive(!isCanAskLife);
			AskButton.SetActive(isCanAskLife);
			selectAllBtn.interactable = isCanAskLife;
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

		private void GetData()
		{
			if (FacebookUtilities.Instance.CheckFacebookLogin())
			{
				SelectAllItem(true);
				return;
			}
			selectAll.gameObject.SetActive(false);
			selectAllBtn.gameObject.SetActive(false);
			AskButton.SetActive(false);
			Connect.SetActive(true);
			foreach (KeyValuePair<string, FriendItem> item in friendItemDic)
			{
				Object.Destroy(item.Value.gameObject);
			}
			friendItemDic.Clear();
			ActivefriendItemDic.Clear();
		}

		public void ConnectFacebook()
		{
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				FacebookUtilities.Instance.LoginWithFacebook();
			}
		}

		public void ActiveToggle(bool isOn, FriendItem item)
		{
			DebugUtils.Log(DebugType.NetWork, item.GetId() + " toggle isOn is " + isOn);
			if (isOn)
			{
				if (!ActivefriendItemDic.ContainsKey(item.GetId()))
				{
					ActivefriendItemDic.Add(item.GetId(), item);
					if (ActivefriendItemDic.Count == friendItemDic.Count && !selectAll.isOn)
					{
						selectAll.isOn = true;
					}
				}
			}
			else if (ActivefriendItemDic.ContainsKey(item.GetId()))
			{
				ActivefriendItemDic.Remove(item.GetId());
				if (selectAll.isOn)
				{
					selectAll.isOn = false;
				}
			}
		}

		public void CheckAll()
		{
			bool flag = true;
			foreach (KeyValuePair<string, FriendItem> item in friendItemDic)
			{
				if (!item.Value.toggle.isOn)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				selectAll.isOn = true;
			}
			else
			{
				selectAll.isOn = false;
			}
		}

		public void InviteFriend()
		{
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.FacebookConnectFailDlg);
			}
			
		}

		public void AskLife()
		{
			Analytics.Event("AskOrSendHeart", new Dictionary<string, string> { { "AskOrSendHeart", "AskLife" } });
			DebugUtils.Log(DebugType.NetWork, "AskOrSendHeart | AskLife");
			if (!FacebookUtilities.Instance.CheckFacebookLogin())
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.FacebookConnectFailDlg, null, DialogType.AskForLifeDlg);
				Analytics.Event("AskOrSendHeart", new Dictionary<string, string> { { "AskOrSendHeart", "AskLifeFail" } });
				DebugUtils.Log(DebugType.NetWork, "AskOrSendHeart | AskLifeFail");
				return;
			}
			List<string> delList = new List<string>();
			foreach (KeyValuePair<string, FriendItem> item in ActivefriendItemDic)
			{
				delList.Add(item.Key);
				DebugUtils.Log(DebugType.NetWork, "ready to send info friend id is :" + item.Key);
			}
			Loading.SetActive(true);
			AskButton.GetComponent<Button>().interactable = false;
			
		}

		public void SelectAllItem()
		{
			if (selectAll.isOn)
			{
				selectAll.isOn = false;
				ActivefriendItemDic.Clear();
				foreach (KeyValuePair<string, FriendItem> item in friendItemDic)
				{
					item.Value.toggle.isOn = false;
				}
			}
			else
			{
				if (friendItemDic.Count == 0)
				{
					return;
				}
				selectAll.isOn = true;
				foreach (KeyValuePair<string, FriendItem> item2 in friendItemDic)
				{
					item2.Value.toggle.isOn = true;
				}
			}
		}

		public void SelectAllItem(bool isOn)
		{
			if (!isOn)
			{
				selectAll.isOn = false;
				ActivefriendItemDic.Clear();
				foreach (KeyValuePair<string, FriendItem> item in friendItemDic)
				{
					item.Value.toggle.isOn = false;
				}
			}
			else
			{
				if (friendItemDic.Count == 0)
				{
					return;
				}
				selectAll.isOn = true;
				foreach (KeyValuePair<string, FriendItem> item2 in friendItemDic)
				{
					item2.Value.toggle.isOn = true;
				}
			}
		}

		public void Close(bool isAnim = true)
		{
			selectAll.isOn = false;
			SelectAllItem(false);
			DialogManagerTemp.Instance.CloseDialog(DialogType.AskForLifeDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		private void OnDestroy()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(25u, GetMyFriendFaceBookData);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(35u, RefreshAskCD);
		}
	}
}
