using System;
using System.Collections.Generic;
using UnityEngine;

namespace Umeng
{
	public class Analytics
	{
		public enum DeviceType
		{
			Phone,
			Box
		}

		private static bool hasInit = false;


		private const string Version = "3.2";

		public static void StartWithAppKeyAndChannelId(string appKey, string channelId)
		{
			UMCommonSetAppkeyAndChannel(appKey, channelId);
			
		}

		private static void UMCommonSetAppkeyAndChannel(string appkey, string channelId, DeviceType deviceType = DeviceType.Phone, string pushSecret = null)
		{
			
		}

		public static void SetLogEnabled(bool value)
		{
			
		}

		public static void Event(string eventId)
		{
			
		}

		public static void Event(string eventId, string label)
		{
			
		}

		public static void Event(string eventId, Dictionary<string, string> attributes)
		{
			
		}

		public static void PageBegin(string pageName)
		{
			
		}

		public static void PageEnd(string pageName)
		{
			
		}

		public static void Event(string eventId, Dictionary<string, string> attributes, int value)
		{
			try
			{
				if (attributes == null)
				{
					attributes = new Dictionary<string, string>();
				}
				if (attributes.ContainsKey("__ct__"))
				{
					attributes["__ct__"] = value.ToString();
					Event(eventId, attributes);
				}
				else
				{
					attributes.Add("__ct__", value.ToString());
					Event(eventId, attributes);
					attributes.Remove("__ct__");
				}
			}
			catch (Exception)
			{
			}
		}

		public static string GetTestDeviceInfo()
		{
			DebugUtils.Log(DebugType.Other, "GetDeviceInfo return ");
			return string.Empty;
		}

		public static void SetLogEncryptEnabled(bool value)
		{
			
		}

		public static void SetLatency(int value)
		{
			
		}

		public static void SetContinueSessionMillis(long milliseconds)
		{
			
		}

		public static void SetCheckDevice(bool value)
		{
			
		}

		private static void AddUmengAndroidLifeCycleCallBack()
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<UmengAndroidLifeCycleCallBack>();
			gameObject.name = "UmengManager";
		}

		public static void onResume()
		{
			
		}

		public static void onPause()
		{
			
		}

		public static void onKillProcess()
		{
			
		}

		private static AndroidJavaObject AndroidJavaJsonObject(JSONObject jSONObject)
		{
			return new AndroidJavaObject("org.json.JSONObject", jSONObject.ToString());
		}

		private static JSONObject jsonObjectFromJava(AndroidJavaObject androidJavaJsonObject)
		{
			return (JSONObject)JSONNode.Parse(androidJavaJsonObject.Call<string>("toString", Array.Empty<object>()));
		}

		private static AndroidJavaObject ToJavaObject(object obj)
		{
			if (obj is int)
			{
				return new AndroidJavaObject("java.lang.Integer", obj);
			}
			if (obj is long)
			{
				return new AndroidJavaObject("java.lang.Long", obj);
			}
			if (obj is float)
			{
				return new AndroidJavaObject("java.lang.Float", obj);
			}
			if (obj is double)
			{
				return new AndroidJavaObject("java.lang.Double", obj);
			}
			if (obj is string)
			{
				return new AndroidJavaObject("java.lang.String", obj);
			}
			if (obj is bool)
			{
				return new AndroidJavaObject("java.lang.Integer", Convert.ToInt32((bool)obj));
			}
			DebugUtils.Log(DebugType.Other, string.Concat("不支持加入", obj.GetType(), "类型,此kv对被丢弃"));
			return null;
		}

		private static AndroidJavaObject ToJavaHashMap(Dictionary<string, object> dic)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
			foreach (KeyValuePair<string, object> item in dic)
			{
				AndroidJavaObject androidJavaObject2 = ToJavaObject(item.Value);
				if (androidJavaObject2 != null)
				{
					androidJavaObject.Call<AndroidJavaObject>("put", new object[2] { item.Key, androidJavaObject2 });
				}
			}
			return androidJavaObject;
		}

		private static AndroidJavaObject ToJavaHashMap(Dictionary<string, string> dic)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap");
			foreach (KeyValuePair<string, string> item in dic)
			{
				androidJavaObject.Call<AndroidJavaObject>("put", new object[2] { item.Key, item.Value });
			}
			return androidJavaObject;
		}

		private static AndroidJavaObject ToJavaList(string[] list)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.ArrayList");
			foreach (string text in list)
			{
				androidJavaObject.Call<bool>("add", new object[1] { text });
			}
			return androidJavaObject;
		}

		public static void EventObject(string eventID, Dictionary<string, object> dict)
		{
			
		}

		public static void RegisterPreProperties(JSONObject jsonObject)
		{
			JSONObject jSONObject = new JSONObject();
			foreach (KeyValuePair<string, JSONNode> item in jsonObject)
			{
				if (item.Value.IsObject || item.Value.IsArray)
				{
					Debug.LogError("不支持加入Object/Array类型,此kv对被丢弃");
				}
				else if (item.Value.IsBoolean)
				{
					jSONObject.Add(item.Key, Convert.ToInt32(item.Value.AsBool));
				}
				else
				{
					jSONObject.Add(item.Key, item.Value);
				}
			}
			
		}

		public static void UnregisterPreProperty(string propertyName)
		{
			
		}

		

		public static void ClearPreProperties()
		{
			
		}

		public static void SetFirstLaunchEvent(string[] trackID)
		{
			
		}
	}
}
