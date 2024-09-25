using System;
using UnityEngine;

public class NotificationHelper
{
	private static string fullClassName = "com.playinfinity.plugin.notification.UnityNotificationManager";

	public static int SendNotification(int id, TimeSpan delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendNotification(id, (int)delay.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendNotification(int id, long delayMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		Debug.Log("register nogification id " + id + " " + delayMs);
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetNotification", id, delayMs, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, Application.identifier);
		}
		return id;
	}

	public static int SendRepeatingNotification(int id, TimeSpan delay, TimeSpan timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		return SendRepeatingNotification(id, (int)delay.TotalSeconds * 1000, (int)timeout.TotalSeconds * 1000, title, message, bgColor, sound, vibrate, lights, bigIcon);
	}

	public static int SendRepeatingNotification(int id, long delayMs, long timeoutMs, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetRepeatingNotification", id, delayMs, title, message, message, timeoutMs, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, Application.identifier);
		}
		return id;
	}

	public static void CancelNotification(int id)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelPendingNotification", id);
		}
	}

	public static void ClearNotifications()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("ClearShowingNotifications");
		}
	}
}
