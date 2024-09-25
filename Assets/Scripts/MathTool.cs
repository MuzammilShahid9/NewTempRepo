using System.Collections.Generic;
using UnityEngine;

public class MathTool
{
	public static string GetRandomKeyFromWeightDict(Dictionary<string, int> dict)
	{
		int num = 0;
		foreach (KeyValuePair<string, int> item in dict)
		{
			num += item.Value;
		}
		int num2 = Random.Range(1, num + 1);
		List<string> list = new List<string>(dict.Keys);
		int num3 = 0;
		foreach (string item2 in list)
		{
			num3 += dict[item2];
			if (num2 <= num3)
			{
				return item2;
			}
		}
		return "None";
	}

	public static Vector3 GetVec3ByString(string str)
	{
		str = str.Replace("(", "").Replace(")", "");
		string[] array = str.Split(',');
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}
}
