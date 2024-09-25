using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;

public class PlayInfinityAdManager : MonoBehaviour
{
	public bool bannerActive;

	private static PlayInfinityAdManager instance;

	public static PlayInfinityAdManager Instance
	{
		get
		{
			return instance;
		}
	}

	public float BannerHeight
	{
		get
		{
			return 100f;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public void Init()
	{
	}

	public bool IsShowInterstitial()
	{
		return false;
	}

	public void ShowInterstitial(int adShowType)
	{
		DebugUtils.Log(DebugType.Other, "show interstitial type: " + adShowType);
		if (IsShowInterstitial())
		{
			if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
			{
				Analytics.Event("ShowInterstitial");
			}
			GlobalVariables.ResumeFromDesktop = false;

		}
	}

	public void DestroyBanner()
	{

	}

	public void InterstitialCloseCallback()
	{
	}

	public void RewardVideoCloseCallback()
	{
		TaskPanelManager.Instance.WatchVideoFinish();
	}

	public bool IsShowBanner()
	{
		return false;
	}

	public void RequestBanner()
	{
		if (IsShowBanner())
		{
			
		}
	}

	public void ShowBanner()
	{
		
	}

	public void HideBanner()
	{
		
	}

	public bool IsShowRewardVideo()
	{
		return true;
	}

	public bool IsRewardVideoLoaded()
	{
		return false;
	}

	public void ShowRewardVideo()
	{
		if (IsShowRewardVideo())
		{
			Analytics.Event("ShowVideo", new Dictionary<string, string> { { "ShowVideo", "ShowVideo" } });
			DebugUtils.Log(DebugType.NetWork, "ShowVideo");
			GlobalVariables.ResumeFromDesktop = false;
			
		}
	}
}
