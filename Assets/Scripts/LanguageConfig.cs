using System.Collections.Generic;
using System.Globalization;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class LanguageConfig
{
	public delegate void GameVoidDelegate();

	public static Dictionary<SystemLanguage, Dictionary<string, string>> LanguageTable;

	private static List<LanguageConfigData> languageConfig;

	private static List<LanguageConfigData> plotLanguageConfig;

	public static SystemLanguage languageStr = Application.systemLanguage;

	public static GameVoidDelegate OnLocalize;

	public static Dictionary<SystemLanguage, Font> FontCollect;

	public static void Load()
	{
		FontCollect = new Dictionary<SystemLanguage, Font>();
		FontCollect.Add(SystemLanguage.English, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Chinese, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Japanese, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Korean, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Russian, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Portuguese, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Italian, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.French, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.Spanish, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		FontCollect.Add(SystemLanguage.German, Resources.Load("Font/Poetsen One", typeof(Font)) as Font);
		if (UserDataManager.Instance.GetService().language == SystemLanguage.ChineseSimplified || UserDataManager.Instance.GetService().language == SystemLanguage.ChineseTraditional)
		{
			languageStr = SystemLanguage.Chinese;
		}
		else
		{
			languageStr = UserDataManager.Instance.GetService().language;
		}
		DebugUtils.Log(DebugType.Other, "Processing Language Infos..." + languageStr.ToString() + "   operation language : " + Application.systemLanguage);
		DebugUtils.Log(DebugType.Other, languageStr);
		LanguageTable = new Dictionary<SystemLanguage, Dictionary<string, string>>();
		languageConfig = JsonUtility.FromJson<LanguageConfigDataList>((Resources.Load("Config/Language/LanguageConfig") as TextAsset).text).data;
		TextAsset textAsset = Resources.Load("Config/Plot/LanguageConfig") as TextAsset;
		if (textAsset != null)
		{
			plotLanguageConfig = JsonUtility.FromJson<LanguageConfigDataList>(textAsset.text).data;
		}
		DicInit();
		if (!LanguageTable.ContainsKey(languageStr))
		{
			languageStr = SystemLanguage.English;
		}
		UserDataManager.Instance.GetService().language = languageStr;
	}

	public static void ChangeLangage(SystemLanguage type)
	{
		languageStr = type;
		if (Application.isPlaying)
		{
			UserDataManager.Instance.GetService().language = languageStr;
			UserDataManager.Instance.Save();
		}
		if (OnLocalize != null)
		{
			OnLocalize();
		}
		DebugUtils.Log(DebugType.Other, "change : " + type);
	}

	private static void DicInit()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary6 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary7 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary8 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary9 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary10 = new Dictionary<string, string>();
		for (int i = 0; i < languageConfig.Count; i++)
		{
			if (languageConfig[i].CN != "")
			{
				dictionary.Add(languageConfig[i].Key, languageConfig[i].CN);
			}
			if (languageConfig[i].EN != "")
			{
				dictionary2.Add(languageConfig[i].Key, languageConfig[i].EN);
			}
			if (languageConfig[i].FR != "")
			{
				dictionary3.Add(languageConfig[i].Key, languageConfig[i].FR);
			}
			if (languageConfig[i].ES != "")
			{
				dictionary4.Add(languageConfig[i].Key, languageConfig[i].ES);
			}
			if (languageConfig[i].DE != "")
			{
				dictionary5.Add(languageConfig[i].Key, languageConfig[i].DE);
			}
			if (languageConfig[i].PT != "")
			{
				dictionary6.Add(languageConfig[i].Key, languageConfig[i].PT);
			}
			if (languageConfig[i].KR != "")
			{
				dictionary8.Add(languageConfig[i].Key, languageConfig[i].KR);
			}
			if (languageConfig[i].RU != "")
			{
				dictionary9.Add(languageConfig[i].Key, languageConfig[i].RU);
			}
			if (languageConfig[i].IT != "")
			{
				dictionary10.Add(languageConfig[i].Key, languageConfig[i].IT);
			}
			if (languageConfig[i].JP != "")
			{
				dictionary7.Add(languageConfig[i].Key, languageConfig[i].JP);
			}
		}
		for (int j = 0; j < plotLanguageConfig.Count; j++)
		{
			if (plotLanguageConfig[j].CN != "")
			{
				dictionary.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].CN);
			}
			if (plotLanguageConfig[j].EN != "")
			{
				dictionary2.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].EN);
			}
			if (plotLanguageConfig[j].FR != "")
			{
				dictionary3.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].FR);
			}
			if (plotLanguageConfig[j].ES != "")
			{
				dictionary4.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].ES);
			}
			if (plotLanguageConfig[j].DE != "")
			{
				dictionary5.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].DE);
			}
			if (plotLanguageConfig[j].PT != "")
			{
				dictionary6.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].PT);
			}
			if (plotLanguageConfig[j].KR != "")
			{
				dictionary8.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].KR);
			}
			if (plotLanguageConfig[j].RU != "")
			{
				dictionary9.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].RU);
			}
			if (plotLanguageConfig[j].IT != "")
			{
				dictionary10.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].IT);
			}
			if (plotLanguageConfig[j].JP != "")
			{
				dictionary7.Add(plotLanguageConfig[j].Key, plotLanguageConfig[j].JP);
			}
		}
		LanguageTable.Add(SystemLanguage.Chinese, dictionary);
		LanguageTable.Add(SystemLanguage.English, dictionary2);
		LanguageTable.Add(SystemLanguage.French, dictionary3);
		LanguageTable.Add(SystemLanguage.Spanish, dictionary4);
		LanguageTable.Add(SystemLanguage.German, dictionary5);
		LanguageTable.Add(SystemLanguage.Portuguese, dictionary6);
		LanguageTable.Add(SystemLanguage.Japanese, dictionary7);
		LanguageTable.Add(SystemLanguage.Korean, dictionary8);
		LanguageTable.Add(SystemLanguage.Russian, dictionary9);
		LanguageTable.Add(SystemLanguage.Italian, dictionary10);
	}

	public static string GetString(string key)
	{
		Dictionary<string, string> dictionary = LanguageTable[languageStr];
		if (dictionary.ContainsKey(key))
		{
			string text = dictionary[key];
			if (Application.isPlaying)
			{
				text = text.Replace("#CatName", UserDataManager.Instance.GetService().catName);
				text = text.Replace("#CastleName", UserDataManager.Instance.GetService().castleName);
			}
			return text;
		}
		return key;
	}

	public static bool IsContainKey(string key)
	{
		return LanguageTable[languageStr].ContainsKey(key);
	}

	public static SystemLanguage GetCurrentLanguage()
	{
		return languageStr;
	}

	public static void SetCurrentLanguage(SystemLanguage language)
	{
		languageStr = language;
	}

	public static void GetCountry()
	{
		string englishName = RegionInfo.CurrentRegion.EnglishName;
	}
}
