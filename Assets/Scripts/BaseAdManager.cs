using UnityEngine;

public abstract class BaseAdManager : MonoBehaviour
{
	public abstract void RequestBanner();

	public abstract void RequestRewardVideo();

	public abstract void RequestInterstitial();

	public abstract void ShowBanner();

	public abstract void HideBanner();

	public abstract void DestroyBanner();

	public abstract void ShowRewardVideo();

	public abstract void ShowInterstitial();

	protected void GiveVideoReward()
	{
	}

	public abstract bool IsInterstitialLoaded();

	public abstract bool IsRewardVideoLoaded();

	public abstract bool IsBannerLoaded();
}
