using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{
	public Dictionary<Text, string> multiLangTexts;

	protected virtual void Start()
	{
		Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onClick.AddListener(delegate
			{
				AudioManager.Instance.PlayAudioEffect("general_button");
			});
		}
	}

	public virtual void ProcessTexts()
	{
	}
}
