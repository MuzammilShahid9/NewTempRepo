using System;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUtils
{
	public static bool DebugMode = true;

	private static Dictionary<DebugType, string> type2string = new Dictionary<DebugType, string>
	{
		{
			DebugType.UI,
			" UI : "
		},
		{
			DebugType.NetWork,
			" NetWork : "
		},
		{
			DebugType.Other,
			" Other : "
		}
	};

	public static bool isCanSend = true;

	public static void Assert(bool cond, string message = "")
	{
		if (DebugMode && !cond)
		{
			throw new Exception(message);
		}
	}

	public static void Log(DebugType type, string message)
	{
		if (DebugMode && type2string.ContainsKey(type))
		{
			Debug.Log(type2string[type] + message);
		}
	}

	public static void Log(DebugType type, object obj)
	{
		if (DebugMode && type2string.ContainsKey(type))
		{
			Debug.Log(type2string[type] + obj);
		}
	}

	public static void LogWarning(DebugType type, string message)
	{
		if (DebugMode && type2string.ContainsKey(type))
		{
			Debug.LogWarning(type2string[type] + message);
		}
	}

	public static void LogError(DebugType type, string message)
	{
		Debug.LogError(type.ToString() + " : " + message);
	}

	public static void LogError(DebugType type, object obj)
	{
		Debug.LogError(type.ToString() + " : " + obj);
	}

	private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
	{
		if (type != 0 && type != LogType.Exception)
		{
			return;
		}
		LogOnScreen(message, type, stackTrace);
		if (TestConfig.isCommitBugToServer && Application.internetReachability != 0 && isCanSend && type == LogType.Exception)
		{
			UserData service = UserDataManager.Instance.GetService();
			string text = "Error";
			if (service != null)
			{
				text = string.Format("level : {0}\nPlot : {1} - {2}\nFaceBookID : {3}\nDevice : {4}\nLanguage : {5}\nSceneName : {6}\nMoneySpend : {7}\nGold : {8}\nScroll : {9}\nIsLogon : {10}\nFirstDownloadVersion : {11}\nlastVersion : {12}\nnowVersion : {13}", service.level, service.LastFinishTaskStage, service.LastFinishTaskID, service.facebookId, SystemInfo.deviceModel, service.language.ToString(), SceneManager.GetActiveScene().name, service.moneySpend, service.coin, service.scrollNum, FacebookUtilities.Instance.CheckFacebookLogin(), service.FirstDownloadVersion, service.lastVersion, service.nowVersion);
			}
			string text2 = string.Format("[{3}]:{0}:{1}\n{2}\n{4}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message, stackTrace, type, text);
			if (!text2.ToLower().Contains("mopub"))
			{
				isCanSend = false;
				FacebookUtilities.Instance.SendBugInfo(text2);
			}
		}
	}

	public static void ProcessExceptionReportCustom(string message)
	{
		if (TestConfig.isCommitBugToServer && Application.internetReachability != 0)
		{
			UserData service = UserDataManager.Instance.GetService();
			string arg = "Error";
			if (service != null)
			{
				arg = string.Format("level : {0}\nPlot : {1} - {2}\nFaceBookID : {3}\nDevice : {4}\nLanguage : {5}\nSceneName : {6}\nMoneySpend : {7}\nGold : {8}\nScroll : {9}\nIsLogon : {10}\nFirstDownloadVersion : {11}\nlastVersion : {12}\nnowVersion : {13}", service.level, service.LastFinishTaskStage, service.LastFinishTaskID, service.facebookId, SystemInfo.deviceModel, service.language.ToString(), SceneManager.GetActiveScene().name, service.moneySpend, service.coin, service.scrollNum, FacebookUtilities.Instance.CheckFacebookLogin(), service.FirstDownloadVersion, service.lastVersion, service.nowVersion);
			}
			string text = string.Format("{0}:{1}\n{2}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message, arg);
			if (!text.ToLower().Contains("mopub"))
			{
				FacebookUtilities.Instance.SendBugInfo(text);
			}
		}
	}

	public static void LogOnScreen(string message, LogType type, string stackTrace = null)
	{
		if (!string.IsNullOrEmpty(message))
		{
			if (string.IsNullOrEmpty(stackTrace))
			{
				stackTrace = Environment.StackTrace;
			}
			message = string.Format("[{3}]:{0}:{1}'\n'{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message, stackTrace, type);
			DebugToScreen.PostException(message);
		}
	}

	public static void Init(GameObject go)
	{
		Application.logMessageReceivedThreaded += ProcessExceptionReport;
		if (!TestConfig.isEnableDebug)
		{
			Debug.Log("Unity DEBUG mode is off!");
			DebugMode = false;
		}
		else
		{
			Debug.Log("Unity DEBUG mode is on!");
			DebugMode = true;
		}
		if (TestConfig.isCommitBugToServer)
		{
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (!isCanSend && time > 30f)
				{
					isCanSend = true;
					time = 0f;
				}
				if (!isCanSend)
				{
					time += duration;
				}
				return false;
			}));
		}
		if (TestConfig.isShowBugWindow)
		{
			DebugToScreen.Init(go);
		}
	}

	public static void Release()
	{
	}
}
