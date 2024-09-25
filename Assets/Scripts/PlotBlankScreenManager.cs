using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class PlotBlankScreenManager : MonoBehaviour
{
	[Serializable]
	public class BlankScreenConfigDataList
	{
		public List<BlankScreenConfigData> data = new List<BlankScreenConfigData>();
	}

	[Serializable]
	public class BlankScreenConfigData
	{
		public string Key;

		public string Content;
	}

	private List<BlankScreenConfigData> blankScreenConfig;

	private int plotStep;

	private bool isStepFinished;

	private BlankScreenConfigData currBlankScreenData;

	private static PlotBlankScreenManager instance;

	public static PlotBlankScreenManager Instance
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

	public void StartBlankScreen(int currStep)
	{
		plotStep = currStep;
		isStepFinished = false;
		DealBlankScreen();
	}

	public void DealBlankScreen()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.BlankScreenDlg, "1");
	}

	public void StopStep()
	{
		BlankScreenDlg.Instance.Exit();
	}

	public void FinishStep()
	{
		if (!isStepFinished)
		{
			isStepFinished = true;
			PlotManager.Instance.FinishOneCondition(plotStep);
		}
	}

	public void RestortPlotStep()
	{
		plotStep = -2;
	}
}
