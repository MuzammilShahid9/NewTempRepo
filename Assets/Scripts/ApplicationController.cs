using System;
using PlayInfinity.GameEngine.Common;

public static class ApplicationController
{
	public static void ProcessApplicationQuit()
	{
		DebugUtils.Log(DebugType.Other, "ProcessApplicationQuit");
		UserDataManager.Instance.GetService().LastQuitTime = DateTime.Now.Ticks;
		UserDataManager.Instance.Save();
		NotificationManager.RegisterNotifications();
	}

	public static void ProcessApplicationPause(bool isPause)
	{
		DebugUtils.Log(DebugType.Other, "ProcessApplicationPause");
		if (isPause)
		{
			UserDataManager.Instance.Save();
			if (GlobalVariables.ResumeFromDesktop)
			{
				DebugUtils.Log(DebugType.Other, "###MainScene pause");
				NotificationManager.RegisterNotifications();
			}
		}
		else if (GlobalVariables.ResumeFromDesktop)
		{
			NotificationManager.UnregisterAllNotifications();
		}
		else
		{
			GlobalVariables.ResumeFromDesktop = true;
		}
	}
}
