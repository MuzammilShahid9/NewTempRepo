using System;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;

public class ComboActivite : GameActivitesBase
{
	public override void Init()
	{
		userData = UserDataManager.Instance.GetService();
		float remainingTime = userData.ComboTM - (DateTime.Now.Ticks / 10000000 - userData.ComboStartTM);
		if (!(remainingTime > 0f))
		{
			return;
		}
		UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (remainingTime <= 0f)
			{
				TurnOff();
				return true;
			}
			UpdateWhenRunning(remainingTime);
			remainingTime -= duration;
			return false;
		}));
	}

	public override bool IsRunning()
	{
		return UserDataManager.Instance.GetIsComboing();
	}

	public override void OnDestroy()
	{
	}

	public override void TurnOff()
	{
		UserDataManager.Instance.SetIsComboing(false);
		userData.ComboTM = -1L;
		GlobalVariables.ComboNum = 0;
		Singleton<MessageDispatcher>.Instance().SendMessage(39u, null);
	}

	public override void TurnOn()
	{
		Singleton<MessageDispatcher>.Instance().SendMessage(40u, null);
		GlobalVariables.ComboNum = 0;
		userData.ComboShowNum++;
		userData.ComboStartTM = DateTime.Now.Ticks / 10000000;
		UserDataManager.Instance.SetIsComboing(true);
		userData.ComboTM = Singleton<PlayGameData>.Instance().gameConfig.ComboTime;
		DialogManagerTemp.Instance.OpenComboDlg(1f, true);
	}

	public override void UpdateWhenRunning(float RemainingTime)
	{
	}

	public override bool CheckIsTurnOff()
	{
		return (float)(userData.ComboTM - (DateTime.Now.Ticks / 10000000 - userData.ComboStartTM)) <= 0f;
	}

	public override bool CheckIsTurnOn()
	{
		if (userData.ComboShowNum == 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.ComboActiveLevel)
		{
			return true;
		}
		if (userData.ComboShowNum > 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.ComboActiveLevel)
		{
			long num = DateTime.Now.Ticks / 10000000 - userData.ComboStartTM;
			DebugUtils.Log(DebugType.UI, DateTime.Now.Ticks / 10000000 + "   |Time|   " + userData.ComboStartTM + "    | -----|    " + Singleton<PlayGameData>.Instance().gameConfig.ComboCDTime);
			if (num > Singleton<PlayGameData>.Instance().gameConfig.ComboCDTime)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public override void WhenDlgOpen(DialogType DlgType)
	{
		if (DlgType != DialogType.ComboDlg && DlgType != DialogType.EnterGameDlg && DlgType != DialogType.QuitLevelDlg)
		{
			int num = 2;
		}
	}

	public override void WhenDlgRecover(DialogType DlgType)
	{
		if (DlgType != DialogType.ComboDlg && DlgType != DialogType.EnterGameDlg && DlgType != DialogType.QuitLevelDlg)
		{
			int num = 2;
		}
	}
}
