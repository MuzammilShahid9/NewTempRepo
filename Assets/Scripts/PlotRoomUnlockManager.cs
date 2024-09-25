using UnityEngine;

public class PlotRoomUnlockManager : MonoBehaviour
{
	private bool isStepFinished;

	private int plotStep;

	private static PlotRoomUnlockManager instance;

	public static PlotRoomUnlockManager Instance
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

	public void StartUnlock(int roomID, int currStep)
	{
		plotStep = currStep;
		isStepFinished = false;
		CastleManager.Instance.UnlockRoom(roomID);
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
