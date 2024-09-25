using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class DirectorSceneManager : BaseSceneManager
{
	protected override void Awake()
	{
		sceneUI = CastleSceneUIManager.Instance;
		Application.targetFrameRate = 60;
		if (UserDataManager.Instance == null)
		{
			UserDataManager.Instance.Load();
		}
	}

	protected override void Start()
	{
	}

	protected override void OnApplicationPause(bool isPause)
	{
		DebugUtils.Log(DebugType.Other, "###ChapterScene OnApplicationPause");
	}
}
