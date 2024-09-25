using System.Collections;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.Networking;

public class MyServerManager : MonoBehaviour
{
	private static MyServerManager _instance;

	public static MyServerManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public void SendCastleNameData()
	{
		StartCoroutine(SendCastleNameDataIEnumerator());
	}

	private IEnumerator SendCastleNameDataIEnumerator()
	{
		if (!(UserDataManager.Instance.GetService().castleName == LanguageConfig.GetString("RenameDlg_CastlesDefaultName")))
		{
			string analyticsCastltNameDataAddress = NetworkConfig.AnalyticsCastltNameDataAddress;
			string value = "CastleStoryAnalyticsAndroid|" + UserDataManager.Instance.GetService().castleName;
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", value);
			UnityWebRequest www = UnityWebRequest.Post(analyticsCastltNameDataAddress, wWWForm);
			yield return www.Send();
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics|    ");
			}
			else
			{
				DebugUtils.Log(DebugType.Other, www.downloadHandler.text);
			}
			DebugUtils.Log(DebugType.Other, "SendNameDataIEnumerator success");
		}
	}

	public void SendCatNameData()
	{
		StartCoroutine(SendCatNameDataIEnumerator());
	}

	private IEnumerator SendCatNameDataIEnumerator()
	{
		if (!(UserDataManager.Instance.GetService().catName == LanguageConfig.GetString("RenameDlg_CatDefaultName")))
		{
			string analyticsCatNameDataAddress = NetworkConfig.AnalyticsCatNameDataAddress;
			string text = "CastleStoryAnalyticsAndroid|" + UserDataManager.Instance.GetService().catName;
			DebugUtils.Log(DebugType.Other, text);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", text);
			UnityWebRequest www = UnityWebRequest.Post(analyticsCatNameDataAddress, wWWForm);
			yield return www.Send();
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics|    ");
			}
			else
			{
				DebugUtils.Log(DebugType.Other, www.downloadHandler.text);
			}
			DebugUtils.Log(DebugType.Other, "SendNameDataIEnumerator success");
		}
	}
}
