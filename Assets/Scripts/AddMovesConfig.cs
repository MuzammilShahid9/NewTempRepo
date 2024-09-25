using System.Collections.Generic;
using UnityEngine;

public class AddMovesConfig
{
	public static List<AddMovesConfigData> addMovesConfig;

	public static void Load()
	{
		Debug.Log("Processing Shop Infos...");
		addMovesConfig = JsonUtility.FromJson<AddMovesConfigDataList>((Resources.Load("Config/AddMoves/AddMovesConfig") as TextAsset).text).data;
	}

	public static AddMovesConfigData GetAddMovesData(int purchasingID)
	{
		AddMovesConfigData result = new AddMovesConfigData();
		for (int i = 0; i < addMovesConfig.Count; i++)
		{
			if (addMovesConfig[i].ID == purchasingID)
			{
				result = addMovesConfig[i];
			}
		}
		return result;
	}
}
