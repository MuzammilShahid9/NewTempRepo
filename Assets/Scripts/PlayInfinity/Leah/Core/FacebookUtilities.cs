using System;
using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.Leah.Core
{
	public class FacebookUtilities : MonoBehaviour
	{
		public delegate void SaveDataCallbackDelegate();

		private static FacebookUtilities instance;

		public bool ProcessingFbFunc;

		public Dictionary<string, Dictionary<string, string>> friendsData;

		public List<KeyValuePair<string, Dictionary<string, string>>> sortedFriendsData;

		public Dictionary<string, Dictionary<string, string>> invitableFriendsData;

		public Dictionary<string, string> myData;

		public List<string> friendsId;

		public string selfID;

		public SaveDataCallbackDelegate saveDataCallback;

		public Dictionary<string, string> sendLifeMessages;

		public Dictionary<string, string> AskLifeMessages;

		private Coroutine LogOutFacebookCoroutine;

		private bool isSuccess;

		private Coroutine GetFriendsLevelCallbackCoroutine;

		private Coroutine SubmitCurrentLevelCoroutine;

		private Coroutine SendAllDataCoroutine;

		private bool isConnectTimeOut = true;

		private Coroutine retrieveData;

		private Coroutine timeOut;

		private UserData userData;

		public static FacebookUtilities Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			saveDataCallback = null;
			InitData();
			InitFacebookSDK();
		}

		private void InitFacebookSDK()
		{
			
		}

		private void InitCallback()
		{
			
		}

		private void InitData()
		{
			friendsData = new Dictionary<string, Dictionary<string, string>>();
			sortedFriendsData = new List<KeyValuePair<string, Dictionary<string, string>>>();
			invitableFriendsData = new Dictionary<string, Dictionary<string, string>>();
			friendsId = new List<string>();
			sendLifeMessages = new Dictionary<string, string>();
			AskLifeMessages = new Dictionary<string, string>();
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}

		public void ActivateFacebookSDK()
		{
			
		}

		public bool CheckFacebookInit()
		{
			return false;
		}

		public bool CheckFacebookLogin()
		{
			return false;
		}

		public void LoginWithFacebook()
		{
			GlobalVariables.ResumeFromDesktop = false;
			DebugUtils.Log(DebugType.NetWork, "Logon FaceBook!");
			
		}

		

		public void GetAllData(bool isCheckUserData = false)
		{
			
			
		}

		public string GetCurrentLoginUserId()
		{
			string result = "";
			
			return result;
		}

		public void LogOutFacebook()
		{
			UserDataManager.Instance.Save();
			NetworkConfig.isSuccessfulGetDataFromService = false;
			if (LogOutFacebookCoroutine != null)
			{
				StopCoroutine(LogOutFacebookCoroutine);
			}
			LogOutFacebookCoroutine = StartCoroutine(WaitForLogout());
		}

		private IEnumerator WaitForLogout()
		{
			
			yield return new WaitUntil(() => !CheckFacebookLogin());
			ClearAllFacebookData();
			LogOutFacebookCoroutine = null;
		}

		private void ClearAllFacebookData()
		{
			selfID = "";
			myData = null;
			friendsData.Clear();
			sortedFriendsData.Clear();
			friendsId.Clear();
			sendLifeMessages.Clear();
			AskLifeMessages.Clear();
			invitableFriendsData.Clear();
			Singleton<MessageDispatcher>.Instance().SendMessage(22u, "Logout");
		}

		private void GetMyData()
		{
			DebugUtils.Log(DebugType.NetWork, "GetMyData!  Path is : " + NetworkConfig.GetMyFbDataAddress);
			
		}

		public bool TryGetGetFacebookFriends()
		{
			if (GameConfig.GetFaceBookUserData)
			{
				GetFacebookFriends();
				return true;
			}
			return false;
		}

		public void GetFacebookFriends()
		{
			
		}

		private void GetInvitableFriendsData()
		{
			
		}

		
		private string GetDataValueForKey(Dictionary<string, object> dict, string key)
		{
			object value;
			if (dict.TryGetValue(key, out value))
			{
				return value.ToString();
			}
			return "";
		}

		private void GetFriendsLevel()
		{
			DebugUtils.Log(DebugType.NetWork, ("GetFriendsLevel  self ID is " + UserDataManager.Instance.GetService().facebookId) ?? "");
			if (Instance.CheckFacebookLogin() && !(UserDataManager.Instance.GetService().facebookId == "") && friendsId != null)
			{
				string text = selfID + "-";
				for (int i = 0; i < friendsId.Count; i++)
				{
					text = ((i >= friendsId.Count - 1) ? (text + friendsId[i]) : (text + friendsId[i] + "-"));
				}
				isSuccess = false;
				string text2 = NetworkConfig.GetRankList + text;
				DebugUtils.Log(DebugType.NetWork, "GetFriendsLevel  url is  " + text2);
				if (GetFriendsLevelCallbackCoroutine != null)
				{
					StopCoroutine(GetFriendsLevelCallbackCoroutine);
				}
				GetFriendsLevelCallbackCoroutine = StartCoroutine(GetFriendsLevelCallback(text2));
				Invoke("GetFriendsLevelCallFail", 2.3f);
			}
		}

		public void GetFriendsLevelCallFail()
		{
			if (!isSuccess && GetFriendsLevelCallbackCoroutine != null)
			{
				StopCoroutine(GetFriendsLevelCallbackCoroutine);
				GetFriendsLevelCallbackCoroutine = null;
				Singleton<MessageDispatcher>.Instance().SendMessage(28u, null);
			}
		}

		private IEnumerator GetFriendsLevelCallback(string url)
		{
			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.Send();
			isSuccess = true;
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.Log(DebugType.Other, "   |      " + www.error + "    Rank|    ");
				DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Rank|    ");
				Singleton<MessageDispatcher>.Instance().SendMessage(28u, null);
			}
			else
			{
				string text = www.downloadHandler.text;
				DebugUtils.Log(DebugType.Other, "@@@@@@@@@@@@ " + text);
				if (text != null && text != "" && text != "NoData")
				{
					ProcessFriendsLevel(text);
					SortFriendData();
				}
				else
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(28u, null);
				}
			}
			GetFriendsLevelCallbackCoroutine = null;
		}

		private void ProcessFriendsLevel(string data)
		{
			try
			{
				string[] array = data.Split('-');
				DebugUtils.Log(DebugType.NetWork, "ProcessFriendsLevel");
				string[] array2 = array;
				foreach (string message in array2)
				{
					DebugUtils.Log(DebugType.NetWork, message);
				}
				for (int j = 0; j < array.Length; j++)
				{
					string[] array3 = array[j].Split(',');
					string text = array3[0];
					string text2 = array3[1];
					if (friendsData.ContainsKey(text))
					{
						if (friendsData[text].ContainsKey("level"))
						{
							DebugUtils.Log(DebugType.NetWork, text + " update level data level is : " + text2);
							friendsData[text]["level"] = text2;
						}
						else
						{
							DebugUtils.Log(DebugType.NetWork, text + " first add level data level is : " + text2);
							friendsData[text].Add("level", text2);
						}
					}
					else if (text == selfID)
					{
						if (myData.ContainsKey("level"))
						{
							DebugUtils.Log(DebugType.NetWork, "self update level data level is : " + text2);
							myData["level"] = text2;
						}
						else
						{
							DebugUtils.Log(DebugType.NetWork, "self first add level data level is : " + text2);
							myData.Add("level", text2);
						}
					}
					else
					{
						DebugUtils.Log(DebugType.NetWork, text + " is not self and dont contain into friendData !  ");
					}
				}
			}
			catch (Exception ex)
			{
				Singleton<MessageDispatcher>.Instance().SendMessage(28u, null);
				DebugUtils.Log(DebugType.NetWork, "Retrieve data failed.\n" + ex.ToString());
			}
		}

		private void SortFriendData()
		{
			try
			{
				List<KeyValuePair<string, Dictionary<string, string>>> list = new List<KeyValuePair<string, Dictionary<string, string>>>();
				list.Add(new KeyValuePair<string, Dictionary<string, string>>(selfID, myData));
				foreach (KeyValuePair<string, Dictionary<string, string>> friendsDatum in friendsData)
				{
					if (friendsDatum.Value.ContainsKey("level"))
					{
						DebugUtils.Log(DebugType.Other, "add one !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + friendsDatum.Value["level"]);
						list.Add(friendsDatum);
					}
				}
				if (list.Count >= 2)
				{
					list.Sort((KeyValuePair<string, Dictionary<string, string>> pair2, KeyValuePair<string, Dictionary<string, string>> pair1) => Convert.ToInt32(pair1.Value["level"]).CompareTo(Convert.ToInt32(pair2.Value["level"])));
				}
				Singleton<MessageDispatcher>.Instance().SendMessage(27u, list);
			}
			catch (Exception ex)
			{
				DebugUtils.Log(DebugType.Other, "Friend data is not correct." + ex);
			}
		}

		private IEnumerator SendData(string url)
		{
			WWWForm formData = new WWWForm();
			UnityWebRequest www = UnityWebRequest.Post(url, formData);
			yield return www.Send();
			if ((www.error != null) | (www.error != ""))
			{
				DebugUtils.Log(DebugType.Other, www.error);
			}
			if (www.GetResponseHeader("response") != null)
			{
				DebugUtils.Log(DebugType.Other, www.GetResponseHeader("response"));
			}
		}

		public void SubmitCurrentLevel()
		{
			if (SubmitCurrentLevelCoroutine != null)
			{
				StopCoroutine(SubmitCurrentLevelCoroutine);
			}
			SubmitCurrentLevelCoroutine = StartCoroutine(SubmitLevel());
		}

		private IEnumerator SubmitLevel()
		{
			if (Instance.CheckFacebookLogin() && UserDataManager.Instance.GetService().facebookId != "")
			{
				string text = UserDataManager.Instance.GetService().facebookId + "&level=" + UserDataManager.Instance.GetService().level;
				string text2 = NetworkConfig.SubmitScoreAddress + text;
				DebugUtils.Log(DebugType.NetWork, "SubmitLevel url is " + text2);
				UnityWebRequest www = UnityWebRequest.Get(text2);
				yield return www.Send();
				string text3 = www.downloadHandler.text;
				DebugUtils.Log(DebugType.NetWork, "SubmitLevel rawdata is " + text3);
				text3 = text3.Trim();
				if (text3.Equals("success~"))
				{
					DebugUtils.Log(DebugType.NetWork, "Successfully submitted level data to server.");
				}
				else
				{
					DebugUtils.Log(DebugType.NetWork, "Failed to submit level data");
				}
			}
			else
			{
				DebugUtils.Log(DebugType.NetWork, "UserDataManager.Instance.GetService ().facebookId is null ");
			}
			SubmitCurrentLevelCoroutine = null;
		}

		public void SaveAllData(object data)
		{
			string data2 = JsonUtility.ToJson(data);
			if (SendAllDataCoroutine != null)
			{
				StopCoroutine(SendAllDataCoroutine);
			}
			SendAllDataCoroutine = StartCoroutine(SendAllData(data2));
		}

		private IEnumerator SendAllData(string data)
		{
			string saveAllDataAddress = NetworkConfig.SaveAllDataAddress;
			DebugUtils.Log(DebugType.Other, UserDataManager.Instance.GetService().facebookId);
			string value = ((!Instance.CheckFacebookLogin() || UserDataManager.Instance.GetService().facebookId == null) ? "" : UserDataManager.Instance.GetService().facebookId);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("id", value);
			wWWForm.AddField("data", data);
			UnityWebRequest www = UnityWebRequest.Post(saveAllDataAddress, wWWForm);
			yield return www.Send();
			DebugUtils.Log(DebugType.Other, "Send Finish!");
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.LogError(DebugType.NetWork, www.error + "   save Data");
			}
			else
			{
				DebugUtils.Log(DebugType.Other, "Upload success.");
				if (www.GetResponseHeader("response") != null)
				{
					DebugUtils.Log(DebugType.Other, www.GetResponseHeader("response"));
				}
				if (saveDataCallback != null)
				{
					saveDataCallback();
					saveDataCallback = null;
				}
			}
			SendAllDataCoroutine = null;
		}

		public void RetrieveData(string id, bool isCheckUserData)
		{
			isConnectTimeOut = true;
			string text = NetworkConfig.GetDataAddress + id;
			if (!InitGame.Instance.isStarting)
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.LoadingDlg);
			}
			DebugUtils.Log(DebugType.NetWork, "RetrieveData!!  url is : " + text);
			if (retrieveData != null)
			{
				StopCoroutine(retrieveData);
			}
			retrieveData = StartCoroutine(RetrieveDataCallback(text, id, isCheckUserData));
			if (timeOut != null)
			{
				StopCoroutine(timeOut);
			}
			timeOut = StartCoroutine(TimeOut());
		}

		private IEnumerator TimeOut()
		{
			yield return new WaitForSeconds(3f);
			DebugUtils.LogError(DebugType.NetWork, "   |      Compare data Time out    |    ");
			if (retrieveData != null)
			{
				if (!InitGame.Instance.isStarting)
				{
					DialogManagerTemp.Instance.CloseDialog(DialogType.LoadingDlg, false);
				}
				StopCoroutine(retrieveData);
				retrieveData = null;
				DebugUtils.Log(DebugType.Other, "   |      TimeOut    |    ");
				DebugUtils.LogError(DebugType.NetWork, "   |      TimeOut    |    ");
				NetworkConfig.isSuccessfulGetDataFromService = false;
			}
			timeOut = null;
		}

		private IEnumerator RetrieveDataCallback(string url, string id, bool isCheckUserData)
		{
			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.Send();
			if (timeOut != null)
			{
				StopCoroutine(timeOut);
				timeOut = null;
			}
			isConnectTimeOut = false;
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.Log(DebugType.Other, "   |      " + www.error + "    |    ");
				DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    |    ");
				NetworkConfig.isSuccessfulGetDataFromService = false;
				if (!InitGame.Instance.isStarting)
				{
					DialogManagerTemp.Instance.CloseDialog(DialogType.LoadingDlg, false);
				}
			}
			else
			{
				string text = www.downloadHandler.text;
				DebugUtils.Log(DebugType.NetWork, "RetrieveData  successful!!  id is : " + id + "    rawData is : " + text);
				if (text != null && text != "NoData" && isCheckUserData)
				{
					userData = JsonUtility.FromJson<UserData>(text);
					DebugUtils.Log(DebugType.NetWork, "   |      Compare data do not Time out    |    ");
					bool flag = CompareDateEqual(UserDataManager.Instance.GetService(), userData);
					bool flag2 = UserDataManager.Instance.GetService().isNewData();
					DebugUtils.Log(DebugType.NetWork, "userdata is euqal :  " + flag + "  is New : " + flag2);
					if (!flag && !flag2)
					{
						Analytics.Event("DataRecovery", new Dictionary<string, string> { { "DataRecovery", "ShowCompareDataDlg" } });
						DialogManagerTemp.Instance.ShowDialog(DialogType.CompareDataDlg, userData);
					}
					else if (!flag && flag2)
					{
						if (!InitGame.Instance.isStarting)
						{
							DialogManagerTemp.Instance.CloseDialog(DialogType.LoadingDlg, false);
						}
						UserDataManager.Instance.UpdateUserData(userData);
						DialogManagerTemp.Instance.CloseAllDialogs();
						NetworkConfig.isSuccessfulGetDataFromService = true;
						UserDataManager.Instance.Save();
						if (SceneManager.GetActiveScene().name != SceneType.LogoScene.ToString())
						{
							SceneTransManager.Instance.TransTo(SceneType.LoadingScene);
						}
					}
					else
					{
						if (!InitGame.Instance.isStarting)
						{
							DialogManagerTemp.Instance.CloseDialog(DialogType.LoadingDlg, false);
						}
						NetworkConfig.isSuccessfulGetDataFromService = true;
						UserDataManager.Instance.GetService().facebookId = userData.facebookId;
						UserDataManager.Instance.Save();
					}
				}
				else
				{
					if (!InitGame.Instance.isStarting)
					{
						DialogManagerTemp.Instance.CloseDialog(DialogType.LoadingDlg, false);
					}
					NetworkConfig.isSuccessfulGetDataFromService = true;
					UserDataManager.Instance.GetService().facebookId = id;
					UserDataManager.Instance.Save();
				}
			}
			retrieveData = null;
		}

		private bool CompareDateEqual(UserData native, UserData cloud)
		{
			if (native.level == cloud.level && native.coin == cloud.coin)
			{
				return native.scrollNum == cloud.scrollNum;
			}
			return false;
		}

		
		public void InviteFriend(List<string> friendIDs)
		{
			GlobalVariables.ResumeFromDesktop = false;
			
		}

		

		public void AskFriend()
		{
			
		}

	
		public void ShareLink()
		{
			
			GlobalVariables.ResumeFromDesktop = false;
		}

	

		public void GetPictureFromUrl(Image img, string url)
		{
			StartCoroutine(GetTextureByUrl(img, url));
		}

		private IEnumerator GetTextureByUrl(Image img, string url)
		{
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
			yield return www.Send();
			if (CheckFacebookLogin())
			{
				try
				{
					Texture2D content = DownloadHandlerTexture.GetContent(www);
					Sprite sprite2 = (img.sprite = Sprite.Create(content, new Rect(Vector2.zero, new Vector2(content.width, content.height)), new Vector2(0.5f, 0.5f)));
				}
				catch (Exception)
				{
					DebugUtils.LogError(DebugType.NetWork, "unable to complete ssl connection");
				}
			}
		}

		public void SendBugInfo(string info)
		{
			StopCoroutine("SendBug");
			StartCoroutine("SendBug", info);
		}

		private IEnumerator SendBug(string data)
		{
			string uri = "http://analytics.playinfinity.cn:8000/log/upload_error_log_v2.php";
			string value = "Android";
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", data);
			wWWForm.AddField("platform", value);
			wWWForm.AddField("version", Convert.ToInt32(Application.version.Replace(".", "")));
			UnityWebRequest www = UnityWebRequest.Post(uri, wWWForm);
			yield return www.Send();
			DebugUtils.Log(DebugType.Other, "Send Finish!");
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.LogError(DebugType.NetWork, www.error + "   commit bug");
			}
			else
			{
				DebugUtils.Log(DebugType.Other, "commit bugs success.");
			}
		}
	}
}
