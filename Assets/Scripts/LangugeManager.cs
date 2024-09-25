using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class LangugeManager : MonoBehaviour
{
	private static LangugeManager instance;

	private List<LanguageConfigData> plotLanguageConfig;

	private Dictionary<string, LanguageConfigData> languageDictionary = new Dictionary<string, LanguageConfigData>();

	public static LangugeManager Instance
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

	public string GetContent(string key)
	{
		if (languageDictionary.ContainsKey(key))
		{
			return languageDictionary[key].EN.Replace("#CatName", UserDataManager.Instance.GetService().catName).Replace("#CastleName", UserDataManager.Instance.GetService().castleName);
		}
		return "";
	}
}
