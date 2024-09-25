using System;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class GameWinDlg : BaseDialog
	{
		private static GameWinDlg instance;

		public Text scrollNumText;

		public Text coinNumText;

		public Transform coinEndPos;

		public Transform scrollEndPos;

		public Transform coinStartPos;

		public Transform scrollStartPos;

		public Text coinNumTipText;

		public Text scrollNumTipText;

		public Transform btnParent;

		public Transform coinMidPoint;

		public Transform scrollMidPoint;

		public Transform GoldIcon;

		public Transform ScrollIcn;

		public Text LiftNum;

		public GameObject UnLimited;

		public Text LifeTime;

		public bool isCanCollect;

		public Transform scroll;

		public List<Transform> scrollFly;

		public GameObject FireBall;

		public Button DoubleReward;

		public GameObject Cat;

		public Text CatText;

		private int getCoinNum;

		private int getScrollNum;

		public static GameWinDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.GameWinDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			Cat.SetActive(UserDataManager.Instance.GetIsBanking());
			CatText.color = Color.white;
			coinNumText.color = Color.white;
			coinStartPos.GetComponent<Image>().color = Color.white;
			Cat.GetComponentInChildren<Image>().color = Color.white;
			isCanCollect = false;
			UpdateManager.Instance.ClearLoopData();
			UpdateManager.Instance.PlayGame();
			DoubleReward.GetComponent<CanvasGroup>().DOFade(0f, 0f);
			DoubleReward.GetComponent<CanvasGroup>().interactable = false;
			DoubleReward.gameObject.SetActive(false);
			if (UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.ActvieWatchVideoInGameWin && UserDataManager.Instance.GetService().watchVideoTimeTodayInGameWin <= Singleton<PlayGameData>.Instance().gameConfig.MaxTimeToWinWatchVideo && PlayInfinityAdManager.Instance.IsRewardVideoLoaded())
			{
				DoubleReward.gameObject.SetActive(true);
				DoubleReward.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).OnComplete(delegate
				{
					DoubleReward.GetComponent<CanvasGroup>().interactable = true;
				});
			}
			anim.enabled = true;
			AudioManager.Instance.ChangeMusicVolume(0.1f);
			AudioManager.Instance.PlayAudioEffect("music_game_reward");
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if ((double)time > 3.35)
				{
					AudioManager.Instance.ChangeMusicVolume(1f, 1f);
					return true;
				}
				time += duration;
				return false;
			}));
			getCoinNum = 0;
			if (GameLogic.Instance.initCoinsNum + GameSceneUIManager.Instance.GetCollectGoldNum() >= UserDataManager.Instance.GetService().coin)
			{
				getCoinNum = GameSceneUIManager.Instance.GetCollectGoldNum();
				UserDataManager.Instance.GetService().coin = GameLogic.Instance.initCoinsNum + getCoinNum;
				UserDataManager.Instance.SetBankNum(GameLogic.Instance.initBankNum + getCoinNum * Singleton<PlayGameData>.Instance().gameConfig.BankSaleMultiple);
			}
			else
			{
				getCoinNum = UserDataManager.Instance.GetService().coin - GameLogic.Instance.initCoinsNum;
			}
			getScrollNum = GameSceneUIManager.Instance.GetCollectBookNum();
			scroll = UnityEngine.Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_juanzouBguang_L", typeof(GameObject)) as GameObject, base.transform).transform;
			scroll.localPosition = new Vector3(0f, -27.5f, 0f);
			scroll.localScale = new Vector3(1f, 1f, 0f);
			scroll.gameObject.SetActive(true);
			coinStartPos.gameObject.SetActive(getCoinNum > 0);
			coinNumTipText.text = string.Concat(GameLogic.Instance.initCoinsNum);
			scrollNumTipText.text = string.Concat(GameLogic.Instance.initScrollNum);
			scrollNumText.text = getScrollNum.ToString();
			coinNumText.text = getCoinNum.ToString();
			CatText.text = string.Concat(getCoinNum * Singleton<PlayGameData>.Instance().gameConfig.BankSaleMultiple);
			GoldIcon.DOScale(1.3f, 0.1f).OnComplete(delegate
			{
				GoldIcon.DOScale(1f, 0.1f);
			}).SetAutoKill(false)
				.Pause();
			ScrollIcn.DOScale(1.3f, 0.1f).OnComplete(delegate
			{
				ScrollIcn.DOScale(1f, 0.1f);
			}).SetAutoKill(false)
				.Pause();
			scrollFly = new List<Transform>();
			for (int i = 0; i < getScrollNum; i++)
			{
				Transform transform = PoolManager.Ins.SpawnEffect(50000011).transform;
				transform.localScale = Vector3.one * 1.6f;
				scrollFly.Add(transform);
				transform.position = scrollStartPos.transform.position;
			}
			Timer.Schedule(this, 1f, delegate
			{
				isCanCollect = true;
				Timer.Schedule(this, 1.5f, delegate
				{
					FireBall.SetActive(true);
				});
			});
		}

		public void BtnShareClicked()
		{
		}

		public void StartCollect()
		{
			if (!isCanCollect)
			{
				return;
			}
			anim.enabled = false;
			isCanCollect = false;
			int stepNum = Singleton<PlayGameData>.Instance().gameConfig.OneCoinNum;
			int scrollstepNum = 1;
			int maxCount = Mathf.CeilToInt((float)getCoinNum / (float)stepNum);
			int maxscrollCount = Mathf.CeilToInt((float)getScrollNum / (float)scrollstepNum);
			int currentCount = 0;
			int scrollcurrentCount = 0;
			float onceTime = 0.06f;
			float currentTime = 1f;
			float scrollonceTime = 0.7f;
			float scrollcurrentTime = 1f;
			Vector3[] path = new Vector3[3];
			path[0] = coinStartPos.position;
			path[1] = coinMidPoint.position;
			path[2] = coinEndPos.position;
			Vector3[] scrollPath = new Vector3[3];
			scrollPath[0] = scrollStartPos.position;
			scrollPath[1] = scrollMidPoint.position;
			scrollPath[2] = scrollEndPos.position;
			coinNumText.DOFade(0f, 1.5f);
			coinStartPos.GetComponent<Image>().DOFade(0f, 0f);
			CatText.DOFade(0f, 1.5f);
			Cat.GetComponentInChildren<Image>().DOFade(0f, 1.5f);
			if (DoubleReward.gameObject.activeSelf)
			{
				DoubleReward.GetComponent<CanvasGroup>().interactable = false;
				DoubleReward.GetComponent<CanvasGroup>().DOFade(0f, 1f).OnComplete(delegate
				{
					DoubleReward.gameObject.SetActive(false);
				});
			}
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (currentTime > onceTime)
				{
					currentTime = 0f;
					currentCount++;
					if (currentCount > maxCount)
					{
						Timer.Schedule(this, Singleton<PlayGameData>.Instance().gameConfig.WinCoinFlyTime + 0.1f, delegate
						{
							BtnCloseClicked();
						});
						return true;
					}
					int value2 = getCoinNum - stepNum * currentCount;
					Transform effectGold = PoolManager.Ins.SpawnEffect(50000010, false).transform;
					effectGold.transform.localScale = Vector3.one * 1.85f;
					effectGold.transform.position = coinStartPos.position;
					effectGold.gameObject.SetActive(true);
					TrailRenderer[] componentsInChildren2 = effectGold.GetComponentsInChildren<TrailRenderer>();
					for (int k = 0; k < componentsInChildren2.Length; k++)
					{
						componentsInChildren2[k].emitting = true;
					}
					AudioManager.Instance.PlayAudioEffect("coin_fly");
					effectGold.transform.DOLocalPath(path, Singleton<PlayGameData>.Instance().gameConfig.WinCoinFlyTime, PathType.CatmullRom).SetEase(Ease.InQuad).OnComplete(delegate
					{
						PoolManager.Ins.DeSpawnEffect(effectGold.gameObject, 0f, delegate
						{
							TrailRenderer[] componentsInChildren3 = effectGold.GetComponentsInChildren<TrailRenderer>();
							foreach (TrailRenderer obj in componentsInChildren3)
							{
								obj.emitting = false;
								obj.Clear();
							}
						});
						AudioManager.Instance.PlayAudioEffect("coin_collect");
						try
						{
							coinNumTipText.text = string.Concat(int.Parse(coinNumTipText.text) + ((value2 < 0) ? (stepNum + value2) : stepNum));
							GoldIcon.DORestart();
						}
						catch (Exception ex2)
						{
							DebugUtils.LogError(DebugType.UI, "GameWinDlg Coin null !\n" + ex2);
							throw;
						}
					});
				}
				currentTime += duration;
				return false;
			}));
			Transform trans;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (scrollcurrentTime > scrollonceTime)
				{
					scrollcurrentTime = 0f;
					scrollcurrentCount++;
					if (scrollcurrentCount > maxscrollCount)
					{
						return true;
					}
					int value = getScrollNum - scrollstepNum * scrollcurrentCount;
					try
					{
						scrollNumText.text = string.Concat(Mathf.Max(0, value));
						trans = scrollFly[scrollcurrentCount - 1];
						trans.DOScale(Vector3.one * 0.66f, Singleton<PlayGameData>.Instance().gameConfig.WinScrollFlyTime);
						trans.DOLocalPath(scrollPath, Singleton<PlayGameData>.Instance().gameConfig.WinScrollFlyTime, PathType.CatmullRom).SetEase(Ease.InQuad).OnComplete(delegate
						{
							AudioManager.Instance.PlayAudioEffect("scroll_collect");
							PoolManager.Ins.DeSpawnEffect(trans.gameObject);
							scrollNumTipText.text = string.Concat(int.Parse(scrollNumTipText.text) + ((value < 0) ? (scrollstepNum + value) : scrollstepNum));
							ScrollIcn.DORestart();
						});
					}
					catch (Exception ex)
					{
						DebugUtils.LogError(DebugType.UI, "GameWinDlg Scroll null !\n" + ex);
						throw;
					}
					Renderer[] componentsInChildren = scroll.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Material[] materials = componentsInChildren[i].materials;
						for (int j = 0; j < materials.Length; j++)
						{
							if (materials[j].HasProperty("_Color"))
							{
								Color color = materials[j].color;
								color.a = 0f;
								materials[j].DOColor(color, 1.6f);
							}
							else if (materials[j].HasProperty("_TintColor"))
							{
								Color color2 = materials[j].GetColor("_TintColor");
								color2.a = 0f;
								materials[j].DOColor(color2, "_TintColor", 1.6f);
							}
						}
					}
					AudioManager.Instance.PlayAudioEffect("scroll_fly");
				}
				scrollcurrentTime += duration;
				return false;
			}));
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			StartCollect();
		}

		public void BtnCloseClicked()
		{
			DebugUtils.Log(DebugType.Other, hidingAnimation.length);
			SceneTransManager.Instance.ChangeSceneWithEffect(delegate
			{
				try
				{
					FireBall.SetActive(false);
					UnityEngine.Object.Destroy(scroll.gameObject);
				}
				catch (Exception ex)
				{
					DebugUtils.LogError(DebugType.Other, ex.ToString() + "  GameWinDlg!");
				}
				SceneTransManager.Instance.TransToSwitch(SceneType.CastleScene);
			});
			DialogManagerTemp.Instance.CloseDialog(DialogType.GameWinDlg);
		}

		private void Update()
		{
			UpdateTimeText();
		}

		private void UpdateTimeText()
		{
			long num = -1L;
			if (UserDataManager.Instance.GetService().unlimitedLife)
			{
				UnLimited.SetActive(true);
				LiftNum.text = "";
				num = UserDataManager.Instance.GetService().unlimitedLifeTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().unlimitedLifeStartTM);
			}
			else if (UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal)
			{
				LiftNum.text = UserDataManager.Instance.GetService().life.ToString();
				UnLimited.SetActive(false);
				if (UserDataManager.Instance.GetService().lifeConsumeTime == -1)
				{
					return;
				}
				if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal)
				{
					LifeTime.text = "Full";
					return;
				}
				num = GeneralConfig.LifeRecoverTime - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().lifeConsumeTime);
			}
			else
			{
				LiftNum.text = UserDataManager.Instance.GetService().life.ToString();
				UnLimited.SetActive(false);
			}
			int num2 = (int)num / 3600;
			int num3 = (int)(num - num2 * 60 * 60) / 60;
			int num4 = (int)num - num2 * 60 * 60 - num3 * 60;
			if (num2 > 0)
			{
				LifeTime.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0');
			}
			else if (num3 > 0)
			{
				LifeTime.text = num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
			}
			else if (num4 > 0)
			{
				LifeTime.text = "00:" + num4.ToString().PadLeft(2, '0');
			}
			else
			{
				LifeTime.text = "Full";
			}
		}

		public void ClickDoubleReward()
		{
			if (!PlayInfinityAdManager.Instance.IsRewardVideoLoaded())
			{
				return;
			}
			isCanCollect = false;
			Analytics.Event("ShowVideoType", new Dictionary<string, string> { { "ShowVideo", "InGameWin" } });
			PlayInfinityAdManager.Instance.ShowRewardVideo();
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 3f)
				{
					isCanCollect = true;
					return true;
				}
				time += duration;
				return false;
			}));
		}

		public void WatchVideoFinish()
		{
			UserDataManager.Instance.GetService().watchVideoTimeTodayInGameWin++;
			DoubleReward.gameObject.SetActive(false);
			getCoinNum *= 2;
			coinNumText.text = getCoinNum.ToString();
			CatText.text = string.Concat(getCoinNum * Singleton<PlayGameData>.Instance().gameConfig.BankSaleMultiple);
			UserDataManager.Instance.GetService().coin = GameLogic.Instance.initCoinsNum + getCoinNum;
			UserDataManager.Instance.SetBankNum(GameLogic.Instance.initBankNum + getCoinNum * Singleton<PlayGameData>.Instance().gameConfig.BankSaleMultiple);
		}
	}
}
