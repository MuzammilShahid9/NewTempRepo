using System.Collections.Generic;
using PlayInfinity.GameEngine.Libs.Log;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
	private Dictionary<string, UnityEvent> eventDictionary;

	private static EventManager eventManager;

	public static EventManager Instance
	{
		get
		{
			if (!eventManager)
			{
				eventManager = Object.FindObjectOfType(typeof(EventManager)) as EventManager;
				if (!eventManager)
				{
					LogManager.Debug(LogTags.GENERAL, "There needs to be one active EventManager script on a GameObject in your scene.");
				}
				else
				{
					Init();
				}
			}
			return eventManager;
		}
	}

	private static void Init()
	{
		if (Instance.eventDictionary == null)
		{
			Instance.eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	public void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent value = null;
		if (Instance.eventDictionary.TryGetValue(eventName, out value))
		{
			value.AddListener(listener);
			return;
		}
		value = new UnityEvent();
		value.AddListener(listener);
		Instance.eventDictionary.Add(eventName, value);
	}

	public void StopListening(string eventName, UnityAction listener)
	{
		if (!(eventManager == null))
		{
			UnityEvent value = null;
			if (Instance.eventDictionary.TryGetValue(eventName, out value))
			{
				value.RemoveListener(listener);
			}
		}
	}

	public void TriggerEvent(string eventName)
	{
		UnityEvent value = null;
		if (Instance.eventDictionary.TryGetValue(eventName, out value))
		{
			value.Invoke();
		}
	}
}
