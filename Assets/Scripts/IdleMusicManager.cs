using UnityEngine;

public class IdleMusicManager : MonoBehaviour
{
	private static IdleMusicManager instance;

	private int[] idleMusicCurrStepArray;

	private int[] idleEffectCurrStepArray;

	private MusicConfigData[] currIdleMusicDataArray;

	private EffectConfigData[] currIdleEffectDataArray;

	private bool[] isIdleMusicStepFinishArray;

	private bool[] isIdleEffectStepFinishArray;

	public static IdleMusicManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		idleMusicCurrStepArray = new int[GeneralConfig.RoleNumber];
		idleEffectCurrStepArray = new int[GeneralConfig.RoleNumber];
		isIdleMusicStepFinishArray = new bool[GeneralConfig.RoleNumber];
		isIdleEffectStepFinishArray = new bool[GeneralConfig.RoleNumber];
		currIdleMusicDataArray = new MusicConfigData[GeneralConfig.RoleNumber];
		currIdleEffectDataArray = new EffectConfigData[GeneralConfig.RoleNumber];
	}

	public void StartMusic(MusicConfigData roleConfig, int currStep, RoleType roleType)
	{
		idleMusicCurrStepArray[(int)roleType] = currStep;
		currIdleMusicDataArray[(int)roleType] = roleConfig;
		DealMusic(roleType);
	}

	public void StartEffect(EffectConfigData roleConfig, int currStep, RoleType roleType)
	{
		idleEffectCurrStepArray[(int)roleType] = currStep;
		currIdleEffectDataArray[(int)roleType] = roleConfig;
		DealEffect(roleType);
	}

	public void DealEffect(RoleType roleType)
	{
		isIdleEffectStepFinishArray[(int)roleType] = false;
		AudioManager.Instance.IdlePlayEffect(currIdleEffectDataArray[(int)roleType], roleType);
	}

	public void DealMusic(RoleType roleType)
	{
		isIdleMusicStepFinishArray[(int)roleType] = false;
		AudioManager.Instance.IdlePlayMusic(currIdleMusicDataArray[(int)roleType], roleType);
	}

	public void FinishEffectStep(RoleType roleType)
	{
		Debug.Log("IdleMusicFinish");
		if (!isIdleEffectStepFinishArray[(int)roleType])
		{
			isIdleEffectStepFinishArray[(int)roleType] = true;
			IdleManager.Instance.FinishOneCondition(roleType, idleEffectCurrStepArray[(int)roleType]);
		}
	}

	public void StopIdle()
	{
		StopAllCoroutines();
	}
}
