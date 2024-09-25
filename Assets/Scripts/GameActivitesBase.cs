using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;

public abstract class GameActivitesBase
{
	protected UserData userData;

	public abstract bool IsRunning();

	public abstract void TurnOn();

	public abstract void TurnOff();

	public abstract void UpdateWhenRunning(float RemainingTime);

	public abstract bool CheckIsTurnOn();

	public abstract bool CheckIsTurnOff();

	public abstract void OnDestroy();

	public abstract void Init();

	public abstract void WhenDlgOpen(DialogType DlgType);

	public abstract void WhenDlgRecover(DialogType DlgType);
}
