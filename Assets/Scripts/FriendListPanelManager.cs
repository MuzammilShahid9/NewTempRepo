using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using UnityEngine;

public class FriendListPanelManager : MonoBehaviour
{
	public Transform Grid;

	public GameObject Loading;

	public GameObject NoConnect;

	public GameObject NoInfo;

	public GameObject InviteBtn;

	public GameObject InviteInfo;

	private static FriendListPanelManager instance;

	private Dictionary<int, FriendListItem> RankDic = new Dictionary<int, FriendListItem>();

	public static FriendListPanelManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		base.gameObject.SetActive(false);
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(27u, CompleteFriendSort);
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(28u, NoFriendRankInfo);
	}

	private void NoFriendRankInfo(uint iMessageType, object arg)
	{
		DebugUtils.Log(DebugType.NetWork, "NoFriendRankInfo");
		NoInfo.SetActive(true);
		Loading.SetActive(false);
		NoConnect.SetActive(false);
		InviteInfo.SetActive(false);
		InviteBtn.SetActive(false);
	}

	private void CompleteFriendSort(uint iMessageType, object arg)
	{
		Loading.SetActive(false);
		NoConnect.SetActive(false);
		List<KeyValuePair<string, Dictionary<string, string>>> list = (List<KeyValuePair<string, Dictionary<string, string>>>)arg;
		DebugUtils.Log(DebugType.NetWork, "update FriendList info");
		if (RankDic.Count > list.Count)
		{
			int num = RankDic.Count - list.Count;
			DebugUtils.Log(DebugType.NetWork, "del FriendList count is " + num);
			for (int i = list.Count + 1; i <= RankDic.Count; i++)
			{
				Object.Destroy(RankDic[i].gameObject);
				RankDic.Remove(i);
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<string, Dictionary<string, string>> item in list)
		{
			num2++;
			string key = item.Key;
			string text = item.Value["name"];
			string url = item.Value["picture"];
			string level = item.Value["level"];
			if (RankDic.ContainsKey(num2) && RankDic[num2] != null)
			{
				RankDic[num2].UpdateInfo(key, text, level, url, num2);
				continue;
			}
			GameObject gameObject = Object.Instantiate(GeneralConfig.ItemCollect[4]);
			RankDic[num2] = gameObject.GetComponent<FriendListItem>();
			gameObject.transform.SetParent(Grid, false);
			RankDic[num2].UpdateInfo(key, text, level, url, num2);
			if (key == UserDataManager.Instance.GetService().facebookId)
			{
				RankDic[num2].Heart.gameObject.SetActive(false);
				RankDic[num2].visit.gameObject.SetActive(false);
			}
			else
			{
				RankDic[num2].Heart.gameObject.SetActive(true);
				RankDic[num2].visit.gameObject.SetActive(false);
			}
		}
		if (list.Count == 0)
		{
			DebugUtils.Log(DebugType.NetWork, "Invite your friends!!!!!!!!!!");
			Loading.SetActive(false);
			NoConnect.SetActive(false);
			NoInfo.SetActive(false);
			InviteBtn.SetActive(true);
			InviteInfo.SetActive(true);
		}
	}

	public void InviteFriend()
	{
		if (!FacebookUtilities.Instance.CheckFacebookLogin())
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.FacebookConnectFailDlg);
		}
		
	}

	private void OnEnable()
	{
		if (FacebookUtilities.Instance.CheckFacebookLogin())
		{
			foreach (KeyValuePair<int, FriendListItem> item in RankDic)
			{
				item.Value.UpdateLevelText();
				item.Value.UpdateSendList();
			}
			return;
		}
		DebugUtils.Log(DebugType.NetWork, "NoLogin");
		Loading.SetActive(false);
		NoConnect.SetActive(true);
		NoInfo.SetActive(false);
		InviteBtn.SetActive(false);
		InviteInfo.SetActive(false);
		for (int num = RankDic.Count; num >= 0; num--)
		{
			if (RankDic.ContainsKey(num) && RankDic[num] != null)
			{
				Object.Destroy(RankDic[num].gameObject);
				RankDic.Remove(num);
			}
		}
	}

	public void Connect()
	{
		if (!FacebookUtilities.Instance.CheckFacebookLogin())
		{
			FacebookUtilities.Instance.LoginWithFacebook();
		}
	}

	private void OnDestroy()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(27u, CompleteFriendSort);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(28u, NoFriendRankInfo);
	}
}
