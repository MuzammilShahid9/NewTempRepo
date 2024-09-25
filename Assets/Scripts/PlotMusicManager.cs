using System.Collections.Generic;
using UnityEngine;

public class PlotMusicManager : MonoBehaviour
{
	private List<MusicConfigData> musicConfig;

	private List<EffectConfigData> effectConfig;

	private MusicConfigData currMusicData;

	private EffectConfigData currEffectData;

	private int currMusicPlotStep;

	private int currEffectPlotStep;

	private bool isStepFinished;

	private bool isEffectFinished;

	private static PlotMusicManager instance;

	public static PlotMusicManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		MusicConfigDataList musicConfigDataList = JsonUtility.FromJson<MusicConfigDataList>((Resources.Load("Config/Plot/MusicConfig") as TextAsset).text);
		musicConfig = musicConfigDataList.data;
		EffectConfigDataList effectConfigDataList = JsonUtility.FromJson<EffectConfigDataList>((Resources.Load("Config/Plot/EffectConfig") as TextAsset).text);
		effectConfig = effectConfigDataList.data;
	}

	public void StartMusic(MusicConfigData musicData, int plotStep)
	{
		currMusicPlotStep = plotStep;
		currMusicData = musicData;
		DealMusic();
	}

	public void DealMusic()
	{
		isStepFinished = false;
		AudioManager.Instance.PlotPlayMusic(currMusicData);
	}

	public void StartEffect(EffectConfigData effectData, int plotStep)
	{
		currEffectPlotStep = plotStep;
		currEffectData = effectData;
		DealEffect();
	}

	public void DealEffect()
	{
		isEffectFinished = false;
		AudioManager.Instance.PlotPlayEffect(currEffectData);
	}

	public void FinishEffectStep()
	{
		if (!isEffectFinished)
		{
			isEffectFinished = true;
			PlotManager.Instance.FinishOneCondition(currEffectPlotStep);
		}
	}

	public void FinishStep()
	{
		if (!isStepFinished)
		{
			isStepFinished = true;
			PlotManager.Instance.FinishOneCondition(currMusicPlotStep);
		}
	}

	public void RestortPlotStep()
	{
		currMusicPlotStep = -2;
		currEffectPlotStep = -2;
	}

	public void StopStep()
	{
		isStepFinished = true;
		AudioManager.Instance.StopAudioEffect(currMusicData.MusicName, true);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
