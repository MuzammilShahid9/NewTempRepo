using UnityEngine;

public class PlotFinishStageManager : MonoBehaviour
{
	private int plotStep;

	private static PlotFinishStageManager instance;

	private bool isStepFinished;

	public static PlotFinishStageManager Instance
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

	public void StartFinishStage(int currStep)
	{
		plotStep = currStep;
		StageManage.Instance.ShowStageFinish();
		isStepFinished = false;
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
