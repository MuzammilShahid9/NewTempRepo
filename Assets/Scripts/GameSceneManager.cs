using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class GameSceneManager : BaseSceneManager
{
	protected override void Awake()
	{
		sceneUI = GameSceneUIManager.Instance;
		SceneTransManager.Instance.currentSceneManager = this;
	}

	protected override void Start()
	{
		base.Start();
		AudioManager.Instance.PlayAudioMusic("music_game");
	}

	protected override void OnApplicationPause(bool isPause)
	{
		DebugUtils.Log(DebugType.Other, "###ChapterScene OnApplicationPause");
		ApplicationController.ProcessApplicationPause(isPause);
	}

	protected override void OnDisable()
	{
		if (sceneUI != null)
		{
			Object.Destroy(sceneUI.gameObject);
		}
	}
}
