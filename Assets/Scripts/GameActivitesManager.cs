using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;

public class GameActivitesManager : Singleton<GameActivitesManager>
{
	private Dictionary<ActivitesType, GameActivitesBase> AllGameActivies = new Dictionary<ActivitesType, GameActivitesBase>();

	public void Init()
	{
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(37u, OpenDlg);
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(38u, RecoverDlg);
		foreach (GameActivitesBase value in AllGameActivies.Values)
		{
			value.Init();
		}
	}

	private void RecoverDlg(uint iMessageType, object arg)
	{
		DialogType dlgType = (DialogType)arg;
		foreach (GameActivitesBase value in AllGameActivies.Values)
		{
			value.WhenDlgRecover(dlgType);
		}
	}

	private void OpenDlg(uint iMessageType, object arg)
	{
		DialogType dlgType = (DialogType)arg;
		foreach (GameActivitesBase value in AllGameActivies.Values)
		{
			value.WhenDlgOpen(dlgType);
		}
	}

	public void UpdateByOneSecond()
	{
		foreach (GameActivitesBase value in AllGameActivies.Values)
		{
			if (!value.IsRunning() && value.CheckIsTurnOn())
			{
				value.TurnOn();
			}
		}
	}

	public void Destroy()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(37u, OpenDlg);
		foreach (GameActivitesBase value in AllGameActivies.Values)
		{
			value.OnDestroy();
		}
	}

	public bool GetActivitesIsRunning(ActivitesType type)
	{
		return AllGameActivies[type].IsRunning();
	}
}
