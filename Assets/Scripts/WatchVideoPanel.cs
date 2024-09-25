using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

public class WatchVideoPanel : BaseUI
{
	public Image taskImage;

	public LocalizationText nameText;

	public LocalizationText buttonText;

	public Image[] stepImage;

	public Button doTaskBtn;

	public Image finishImage;

	public int currTaskID;

	private int currPlot = 2;

	private Vector3 startPosition;

	public int CurrPlot
	{
		get
		{
			return currPlot;
		}
		set
		{
			currPlot = value;
		}
	}

	private void Awake()
	{
		Show();
	}

	private void OnEnable()
	{
		startPosition = base.transform.localPosition;
	}

	private void Show()
	{
	}

	public void Enter()
	{
		StartCoroutine(FinishProcess());
	}

	public void WatchVideoBtnClicked()
	{
		if (PlayInfinityAdManager.Instance.IsRewardVideoLoaded() && UserDataManager.Instance.GetService().watchVideoTime < GeneralConfig.WatchVideoTimeLimit)
		{
			Analytics.Event("ShowVideoType", new Dictionary<string, string> { { "ShowVideo", "InTask" } });
			PlayInfinityAdManager.Instance.ShowRewardVideo();
		}
	}

	private IEnumerator FinishProcess()
	{
		doTaskBtn.gameObject.SetActive(false);
		doTaskBtn.enabled = false;
		TaskPanelManager.Instance.FlyIcon(doTaskBtn.transform.position);
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(0.5f);
		base.transform.DOLocalMoveX(startPosition.x - 1600f, 0.5f).SetEase(Ease.Linear);
		finishImage.DOFade(0f, 0.5f);
	}
}
