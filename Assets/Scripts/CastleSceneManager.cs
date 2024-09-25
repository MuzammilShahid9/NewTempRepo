using System;
using System.Collections.Generic;
using System.Globalization;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.AliceMatch3.IdleActionDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CastleSceneManager : BaseSceneManager
{
	private static CastleSceneManager instance;

	public bool cinemaMode;

	public bool IdleEditorMode;

	public GameObject chars;

	public GameObject idleActionDirector;

	public static CastleSceneManager Instance
	{
		get
		{
			return instance;
		}
	}

	protected override void Awake()
	{
		instance = this;
		sceneUI = CastleSceneUIManager.Instance;
		Application.targetFrameRate = 60;
		Timer.Schedule(this, 0.4f, delegate
		{
			SceneTransition.Instance.PlayFadeInAnim();
		});
	}

	protected override void Start()
	{
		CinemaDirector.Instance.gameObject.SetActive(false);
		AudioManager.Instance.PlayAudioMusic("music_castle");
		CastleSceneUIManager.Instance.ShowUI();
		if (UserDataManager.Instance.GetService().tutorialProgress == 3)
		{
			TutorialManager.Instance.ShowTutorial();
		}
	}

	public bool ShowDailyBonus()
	{
		if (SceneManager.GetActiveScene().name != "CastleScene" || UserDataManager.Instance.GetProgress() < 20 || !PlotManager.Instance.isPlotFinish || SceneTransManager.Instance.isReadySwitch)
		{
			return false;
		}
		string[] array = DateTime.Now.ToString("yyyy:MM:dd").Split(':');
		int num = int.Parse(array[2]);
		int num2 = int.Parse(array[1]);
		int num3 = int.Parse(array[0]);
		if (string.IsNullOrEmpty(UserDataManager.Instance.GetService().PreGetDailyBonusTime2))
		{
			UserDataManager.Instance.GetService().PreGetDailyBonusTime2 = UserDataManager.Instance.GetService().PreGetDailyBonusTime.ToString("yyyy:MM:dd");
		}
		string preGetDailyBonusTime = UserDataManager.Instance.GetService().PreGetDailyBonusTime2;
		string[] array2 = preGetDailyBonusTime.Split(':');
		int num4 = int.Parse(array2[2]);
		int num5 = int.Parse(array2[1]);
		int num6 = int.Parse(array2[0]);
		DebugUtils.Log(DebugType.Other, num3 + "-" + num2 + "-" + num + "   &&   " + num6 + "-" + num5 + "-" + num4);
		if (num3 > num6 || (num3 == num6 && num2 > num5) || (num3 == num6 && num2 == num5 && num > num4))
		{
			DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
			dateTimeFormatInfo.ShortDatePattern = "yyyy:MM:dd";
			string[] array3 = Convert.ToDateTime(preGetDailyBonusTime.Replace(':', '-'), dateTimeFormatInfo).AddDays(1.0).ToString("yyyy:MM:dd")
				.Split(':');
			int num7 = int.Parse(array3[2]);
			int num8 = int.Parse(array3[1]);
			int num9 = int.Parse(array3[0]);
			DebugUtils.Log(DebugType.Other, "   &&%%%%%%%%%%%%%   " + num9 + "-" + num8 + "-" + num7);
			if (num3 > num9 || (num3 == num9 && num2 > num8) || (num3 == num9 && num2 == num8 && num > num7))
			{
				UserDataManager.Instance.GetService().DailyBonuseArray.Clear();
				UserDataManager.Instance.GetService().DailyBonuseLevel = 0;
			}
			DialogManagerTemp.Instance.OpenDailyBonusDlg(1f, null);
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			UserDataManager.Instance.GetService().unlimitedLife = true;
			UserDataManager.Instance.GetService().unlimitedLifeStartTM = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.GetService().unlimitedLifeTM = 60L;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			ToggleCinemaShow();
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			CinemaDirector.Instance.gameObject.SetActive(!CinemaDirector.Instance.gameObject.activeSelf);
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			ToggleIdleActionEditor();
		}
	}

	public void ToggleIdleActionEditor()
	{
		idleActionDirector.SetActive(true);
		if (!IdleEditorMode)
		{
			IdleEditorMode = true;
			IdleActionDirector.Instance.gameObject.SetActive(true);
			CastleSceneUIManager.Instance.HideAllBtn();
			CameraControl.Instance.camera3D.UnderCinemaControl();
		}
		else
		{
			IdleEditorMode = false;
			IdleActionDirector.Instance.gameObject.SetActive(false);
			CastleSceneUIManager.Instance.ShowAllBtn();
			CameraControl.Instance.camera3D.ReleaseCinemaControl();
		}
	}

	public void ToggleCinemaShow()
	{
		if (!cinemaMode)
		{
			cinemaMode = true;
			CinemaDirector.Instance.gameObject.SetActive(true);
			sceneUI.gameObject.SetActive(false);
			CameraControl.Instance.camera3D.UnderCinemaControl();
		}
		else
		{
			cinemaMode = false;
			CinemaDirector.Instance.gameObject.SetActive(false);
			sceneUI.gameObject.SetActive(true);
			CameraControl.Instance.camera3D.ReleaseCinemaControl();
		}
	}

	public void ShowUI()
	{
		sceneUI.gameObject.SetActive(true);
	}

	protected override void OnApplicationPause(bool isPause)
	{
		DebugUtils.Log(DebugType.Other, "###ChapterScene OnApplicationPause");
		ApplicationController.ProcessApplicationPause(isPause);
	}
}
