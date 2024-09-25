using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class PlotRenameManager : MonoBehaviour
{
	private bool isStepFinished;

	private int plotStep;

	private static PlotRenameManager instance;

	public static PlotRenameManager Instance
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

	private void Start()
	{
		isStepFinished = true;
	}

	public void StartRename(int renameType, int currStep)
	{
		plotStep = currStep;
		isStepFinished = false;
		DialogManagerTemp.Instance.ShowDialog(DialogType.RenameDlg, renameType);
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
