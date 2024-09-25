using PlayInfinity.Pandora.Core.UI;
using UnityEngine;

public class CastleSceneMainUI : BaseUIManager
{
	private static CastleSceneMainUI instance;

	public GameObject TaskPanel;

	public static CastleSceneMainUI Instance
	{
		get
		{
			return instance;
		}
	}

	private new void Awake()
	{
		instance = this;
		base.gameObject.transform.SetParent(GameObject.Find("UI").transform);
		base.gameObject.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
		base.gameObject.SetActive(false);
	}

	public new void ShowUI()
	{
		DebugUtils.Log(DebugType.Other, "CastleScene ShowUI");
		base.gameObject.transform.SetParent(GameObject.Find("UI").transform);
		base.gameObject.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
		base.gameObject.SetActive(true);
	}

	public void StarBtnClick()
	{
		TaskPanel.SetActive(true);
	}

	public void DoTaskBtnClick()
	{
		TaskPanel.SetActive(false);
		PlotManager.Instance.StartPlot(0);
	}
}
