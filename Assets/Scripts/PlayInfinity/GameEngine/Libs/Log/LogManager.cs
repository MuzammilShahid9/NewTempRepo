using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PlayInfinity.GameEngine.Libs.Log
{
	public class LogManager
	{
		private static LogManager _instance;

		private static bool _enabled = true;

		private static bool _logToConsole = true;

		private static object _locker = new object();

		public static bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		public static bool LogToConsole
		{
			get
			{
				return _logToConsole;
			}
			set
			{
				_logToConsole = value;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private LogManager()
		{
			lock (_locker)
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
				}
				GameObject gameObject = new GameObject();
				gameObject.name = "[ LOG MANAGER ]";
				gameObject.AddComponent<LogManagerDestroyer>();
			}
		}

		private static void Init()
		{
			if (!_enabled || _instance != null)
			{
				return;
			}
			lock (_locker)
			{
				if (_instance == null)
				{
					try
					{
						_instance = new LogManager();
					}
					catch (Exception ex)
					{
						DebugUtils.LogError(DebugType.Other, "Error on initing LogManager:" + ex.Message);
					}
				}
			}
		}

		public static void Log(string message, object[] arguments = null, LogType type = LogType.Log)
		{
			if (!_enabled)
			{
				return;
			}
			if (_instance == null)
			{
				Init();
			}
			if (_instance == null)
			{
				return;
			}
			if (message.Length > 5000)
			{
				message = message.Substring(0, 5000);
			}
			if (_logToConsole)
			{
				if (arguments == null)
				{
					DebugUtils.Log(DebugType.Other, message);
				}
				else
				{
					DebugUtils.Log(DebugType.Other, string.Format(message, arguments));
				}
			}
		}

		public static void Debug(string tag, string message, params object[] arguments)
		{
			Log(tag + " " + message, arguments);
		}

		public static void Error(string tag, string message, params object[] arguments)
		{
			Log(tag + " " + message, arguments, LogType.Error);
		}

		public static void Warning(string tag, string message, params object[] arguments)
		{
			Log(tag + " " + message, arguments, LogType.Warning);
		}

		public static void EnableNativeLogs()
		{
			if (_instance == null)
			{
				Init();
			}
			LogManager instance = _instance;
		}

		public static void SetAutoSaveInterval(int interval_ms)
		{
			if (_instance == null)
			{
				Init();
			}
			LogManager instance = _instance;
		}
	}
}
