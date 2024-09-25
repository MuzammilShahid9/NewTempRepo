using System;

public class ActionUpdate : IUpdate
{
	private bool isFinish;

	private Func<float, bool> action;

	private Action finishAction;

	public ActionUpdate(Func<float, bool> action, Action finishAction = null)
	{
		this.action = action;
		this.finishAction = finishAction;
	}

	public bool IsFinish()
	{
		return isFinish;
	}

	public void ToUpdate(float deltaTime)
	{
		if (!IsFinish())
		{
			isFinish = action(deltaTime);
		}
	}

	public void Finish()
	{
		if (finishAction != null)
		{
			finishAction();
		}
	}
}
