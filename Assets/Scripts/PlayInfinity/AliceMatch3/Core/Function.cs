using System;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayInfinity.AliceMatch3.Core
{
	public class Function : MonoBehaviour
	{
		private static Function instance;

		public static Function Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
		}

		public void SendMail()
		{
			string text = "support@playinfinity.cn";
			string text2 = EscapeUrl("CastleStory Feedback");
			string str = "My suggestion is: \n\n\n\n\n\n\n\n\n\n\nPlease don’t delete the important info below!\nGame Version: " + Application.version + "\nSystem Info: " + SystemInfo.operatingSystem + "\nDevice Info: " + SystemInfo.deviceModel + "\nTimezone: " + GetCurrentTimeZone() + "\nScreen Resolution: " + Screen.currentResolution.ToString() + "\nUserLevel: " + UserDataManager.Instance.GetService().level + "\nUserStage: " + UserDataManager.Instance.GetService().LastFinishTaskStage + "UserTask:" + UserDataManager.Instance.GetService().LastFinishTaskID + "\nSystem Language: " + Application.systemLanguage;
			str = EscapeUrl(str);
			Application.OpenURL("mailto:" + text + "?subject=" + text2 + "&body=" + str);
			DebugUtils.Log(DebugType.Other, GetCurrentTimeZone());
		}

		public string EscapeUrl(string str)
		{
			return UnityWebRequest.EscapeURL(str);
		}

		public string GetCurrentTimeZone()
		{
			return TimeZoneInfo.Local.StandardName;
		}
	}
}
