using System;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public static class NotificationManager
{
	public static List<int> notificationIds = new List<int>();

	public static void RegisterNotifications()
	{
		DebugUtils.Log(DebugType.Other, "RegisterNotifications");
		RegisterRemindPlayNotifications(2);
		RegisterRemindRewardNotifications(4);
		RegisterRemindRewardNotifications(6);
		RegisterRemindRewardNotifications(8);
		RegisterRemindRewardNotifications(11);
		RegisterRemindRewardNotifications(14);
		RegisterRemindRewardNotifications(24);
		RegisterLifeRecoverNotifications();
	}

	public static int GetNotificationRewardDay(int leaveDay)
	{
		List<int> list = new List<int>();
		list.Add(5);
		list.Add(7);
		list.Add(9);
		list.Add(12);
		list.Sort();
		list.Reverse();
		foreach (int item in list)
		{
			if (leaveDay >= item - 1)
			{
				return item;
			}
		}
		return 0;
	}

	public static void RegisterDailyBonusNotifications(int delayDay)
	{
		long value = (long)delayDay * 24L * 60 * 60 * Constants.TicksToSeconds;
		DateTime dateTime = DateTime.Now.AddTicks(value);
		TimeSpan delay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 30, 0).Subtract(DateTime.Now);
		string[] obj = new string[3] { "Time to claim your daily reward! Come back!", "Open Castle Story to claim your daily reward and have a nice day!", "Your daily bonus is ready! Come and collect it!" };
		int num = UnityEngine.Random.Range(0, 3);
		string message = obj[num];
		notificationIds.Add(GeneralConfig.RemindDailyBonusNotificationID + delayDay);
		NotificationHelper.SendNotification(GeneralConfig.RemindDailyBonusNotificationID + delayDay, delay, "Castle Story", message, new Color32(0, 0, 0, byte.MaxValue));
	}

	public static void RegisterHintNotifications(int delayDay)
	{
		if (DateTime.Now.Hour < 18)
		{
			long value = (long)delayDay * 24L * 60 * 60 * Constants.TicksToSeconds;
			DateTime dateTime = DateTime.Now.AddTicks(value);
			TimeSpan delay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 30, 0).Subtract(DateTime.Now);
			string[] obj = new string[3] { "Try {0} and keep going in Castle Story!", "{0} may be the word your are looking for!", "Need a clue? Try the word {0}" };
			int num = UnityEngine.Random.Range(0, 3);
			string message = string.Format(obj[num], "word");
			notificationIds.Add(GeneralConfig.RemindHintNotificationID + delayDay);
			NotificationHelper.SendNotification(GeneralConfig.RemindHintNotificationID + delayDay, delay, "Castle Story", message, new Color32(0, 0, 0, byte.MaxValue));
		}
	}

	public static void RegisterRemindPlayNotifications(int delayDay)
	{
		long value = (long)delayDay * 24L * 60 * 60 * Constants.TicksToSeconds;
		DateTime dateTime = DateTime.Now.AddTicks(value);
		TimeSpan delay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 7, 30, 0).Subtract(DateTime.Now);
		string message = (new string[3] { "Castle Story needs you, my dear friend!", "Keep your mind sharp with Castle Story!", "Come and visit Castle Story! We miss you so much!" })[UnityEngine.Random.Range(0, 3)];
		notificationIds.Add(GeneralConfig.RemindPlayNotificationID + delayDay);
		NotificationHelper.SendNotification(GeneralConfig.RemindPlayNotificationID + delayDay, delay, "Castle Story", message, new Color32(0, 0, 0, byte.MaxValue));
	}

	public static void RegisterRemindRewardNotifications(int delayDay)
	{
		long value = (long)delayDay * 24L * 60 * 60 * Constants.TicksToSeconds;
		DateTime dateTime = DateTime.Now.AddTicks(value);
		TimeSpan delay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 18, 30, 0).Subtract(DateTime.Now);
		string message = (new string[3] { "Gift only for you! Now get it and play Castle Story!", "Free rewards is waiting you in Castle Story!", "Big and sweet rewards awaits you! Collect coins right now!" })[UnityEngine.Random.Range(0, 3)];
		notificationIds.Add(GeneralConfig.RemindRewardNotificationID + delayDay);
		NotificationHelper.SendNotification(GeneralConfig.RemindRewardNotificationID + delayDay, delay, "Castle Story", message, new Color32(0, 0, 0, byte.MaxValue));
	}

	public static void RegisterLifeRecoverNotifications()
	{
		if (UserDataManager.Instance.GetService().notificationEnabled && UserDataManager.Instance.GetService().lifeConsumeTime != -1 && !UserDataManager.Instance.GetService().unlimitedLife && UserDataManager.Instance.GetService().life != GeneralConfig.LifeTotal)
		{
			DebugUtils.Log(DebugType.Other, "RegisterLifeRecoverNotifications");
			TimeSpan timeSpan = new TimeSpan((UserDataManager.Instance.GetService().lifeConsumeTime + 900) * SystemConfig.TickToSecnd);
			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan delay = timeSpan.Subtract(ts).Duration();
			DebugUtils.Log(DebugType.Other, "RegisterLifeRecoverNotifications one life " + delay.Minutes);
			NotificationHelper.SendNotification(GeneralConfig.RemindOneLifeRecoverNotificationID, delay, "Castle Story", LanguageConfig.GetString("Notification_OneLife"), new Color32(0, 0, 0, byte.MaxValue));
			notificationIds.Add(GeneralConfig.RemindOneLifeRecoverNotificationID);
			if (UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal - 1)
			{
				long num = (GeneralConfig.LifeTotal - UserDataManager.Instance.GetService().life - 1) * GeneralConfig.LifeRecoverTime;
				timeSpan = new TimeSpan((UserDataManager.Instance.GetService().lifeConsumeTime + num + 900) * SystemConfig.TickToSecnd);
				ts = new TimeSpan(DateTime.Now.Ticks);
				delay = timeSpan.Subtract(ts).Duration();
				DebugUtils.Log(DebugType.Other, "RegisterLifeRecoverNotifications full life" + delay.Minutes);
				NotificationHelper.SendNotification(GeneralConfig.RemindLifeRecoverNotificationID, delay, "Castle Story", LanguageConfig.GetString("Notification_LivesRefill"), new Color32(0, 0, 0, byte.MaxValue));
				notificationIds.Add(GeneralConfig.RemindLifeRecoverNotificationID);
			}
		}
	}

	public static void NotificationMessage(string title, string message, DateTime newDate)
	{
		DebugUtils.Log(DebugType.Other, "Call NotificationMessage");
	}

	public static void UnregisterAllNotifications()
	{
		DebugUtils.Log(DebugType.Other, "UnregisterAllNotifications");
		foreach (int notificationId in notificationIds)
		{
			NotificationHelper.CancelNotification(notificationId);
		}
	}

	public static void ClearNotifications()
	{
		DebugUtils.Log(DebugType.Other, "ClearNotifications");
		NotificationHelper.ClearNotifications();
	}

	public static void PrintNotification()
	{
		DebugUtils.Log(DebugType.Other, "PrintNotification");
	}

	public static void RegisterForNotification()
	{
	}
}
