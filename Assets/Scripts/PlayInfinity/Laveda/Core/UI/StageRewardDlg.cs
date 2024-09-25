using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class StageRewardDlg : BaseDialog
	{
		public Image boxImage;

		public Image maskImage;

		public Image chapterCompleteImage;

		public LocalizationText continueText;

		public bool showBoxEffect;

		public GameObject effectPosition;

		public GameObject rewardItem;

		public GameObject lifeMidPosition;

		public GameObject lifeEndPosition;

		public GameObject coinMidPosition;

		public GameObject coinEndPosition;

		public GameObject itemMidPosition;

		public GameObject itemEndPosition;

		public GameObject startPosition;

		public GameObject[] midPosition;

		public GameObject[] endPosition;

		private List<RewardItem> rewardItems = new List<RewardItem>();

		private bool animFinished;

		private GameObject effectObj;

		private bool rewardShowFinish;

		private bool isClick;

		private static StageRewardDlg instance;

		public static StageRewardDlg Instance
		{
			get
			{
				return instance;
			}
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.StageRewardDlg);
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			showBoxEffect = false;
			animFinished = false;
			rewardShowFinish = false;
			isClick = false;
			boxImage.gameObject.SetActive(false);
			maskImage.color = new Color(0f, 0f, 0f, 0.5f);
			chapterCompleteImage.color = new Color(1f, 1f, 1f, 1f);
			effectPosition.SetActive(true);
			continueText.color = new Color(1f, 1f, 1f, 1f);
			StartCoroutine(ShowBoxImage());
		}

		private IEnumerator ShowBoxImage()
		{
			yield return new WaitForSeconds(1.5f);
			AudioManager.Instance.PlayAudioEffect("gift_jump", true, 0.46f);
			animFinished = true;
			boxImage.gameObject.SetActive(true);
			CastleSceneUIManager.Instance.HideStageRewardImage();
		}

		private void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				PlotFinishStageManager.Instance.FinishStep();
				if (!showBoxEffect && animFinished)
				{
					showBoxEffect = true;
					StartCoroutine(ShowRewardEffect());
				}
				else if (rewardShowFinish && !isClick)
				{
					isClick = true;
					StartCoroutine(ShowGetRewardEffect());
					StartCoroutine(DelayHide());
				}
			}
		}

		private IEnumerator DelayHide()
		{
			yield return new WaitForSeconds(0.7f);
			BtnCloseClicked();
		}

		private IEnumerator ShowRewardEffect()
		{
			yield return null;
			AudioManager.Instance.StopAudioEffect("gift_jump");
			boxImage.gameObject.SetActive(false);
			GameObject original = Resources.Load("Effect/Eff/UI/eff_ui_Ani_lihe2", typeof(GameObject)) as GameObject;
			effectObj = UnityEngine.Object.Instantiate(original, effectPosition.transform);
			effectObj.transform.localPosition = new Vector3(0f, 0f, 0f);
			yield return new WaitForSeconds(0.8f);
			AudioManager.Instance.PlayAudioEffect("gift_open");
			yield return new WaitForSeconds(0.3f);
			int stage = UserDataManager.Instance.GetService().stage;
			string stageAward = StageManage.Instance.GetStageAward(stage);
			string[] awardItemArray = stageAward.Split(';');
			for (int i = 0; i < endPosition.Length; i++)
			{
				endPosition[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < awardItemArray.Length; j++)
			{
				endPosition[j].gameObject.SetActive(true);
			}
			yield return null;
			rewardItems.Clear();
			for (int k = 0; k < awardItemArray.Length; k++)
			{
				if (awardItemArray[k] != "")
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(rewardItem, startPosition.transform.position, Quaternion.identity, startPosition.transform);
					gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
					Vector3[] path = new Vector3[3]
					{
						gameObject.transform.position,
						endPosition[k].transform.position,
						endPosition[k].transform.position
					};
					Sequence s = DOTween.Sequence();
					s.Append(gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f));
					s.Join(gameObject.transform.DOPath(path, 0.5f));
					s.Join(gameObject.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.3f));
					RewardItem component = gameObject.GetComponent<RewardItem>();
					component.Enter(awardItemArray[k]);
					rewardItems.Add(component);
				}
			}
			rewardShowFinish = true;
		}

		private IEnumerator ShowGetRewardEffect()
		{
			CastleSceneUIManager.Instance.ShowAllBtn();
			yield return null;
			maskImage.DOFade(0f, 0.3f);
			chapterCompleteImage.DOFade(0f, 0.3f);
			effectPosition.SetActive(false);
			continueText.DOFade(0f, 0.3f);
			AudioManager.Instance.PlayAudioEffect("boosters_collect");
			for (int i = 0; i < rewardItems.Count; i++)
			{
				Vector3[] array = new Vector3[3];
				if (rewardItems[i].rewardInfos.rewardType == RewardType.Life)
				{
					array[0] = rewardItems[i].transform.position;
					array[1] = lifeMidPosition.transform.position;
					array[2] = lifeEndPosition.transform.position;
					Sequence sequence = DOTween.Sequence();
					sequence.Append(rewardItems[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f));
					StartCoroutine(FlyIconEnumerator(rewardItems[i].gameObject, array, 0.6f));
					sequence.Join(rewardItems[i].transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.2f));
					sequence.OnComplete(delegate
					{
						CastleSceneUIManager.Instance.LifeImageScale();
					});
				}
				else if (rewardItems[i].rewardInfos.rewardType != RewardType.Coin)
				{
					array[0] = rewardItems[i].transform.position;
					array[1] = itemMidPosition.transform.position;
					array[2] = itemEndPosition.transform.position;
					Sequence sequence2 = DOTween.Sequence();
					sequence2.Append(rewardItems[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.6f));
					StartCoroutine(FlyIconEnumerator(rewardItems[i].gameObject, array, 0.6f));
					sequence2.Join(rewardItems[i].transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.2f));
					sequence2.OnComplete(delegate
					{
						CastleSceneUIManager.Instance.EnterGameBtnScale();
					});
				}
				else if (rewardItems[i].rewardInfos.rewardType == RewardType.Coin)
				{
					array[0] = rewardItems[i].transform.position;
					array[1] = coinMidPosition.transform.position;
					array[2] = coinEndPosition.transform.position;
					Sequence sequence3 = DOTween.Sequence();
					sequence3.Append(rewardItems[i].transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.6f));
					StartCoroutine(FlyIconEnumerator(rewardItems[i].gameObject, array, 0.6f));
					sequence3.Join(rewardItems[i].transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.2f));
					sequence3.OnComplete(delegate
					{
						CastleSceneUIManager.Instance.GoldImageScale();
					});
				}
			}
			GetRewards();
			TaskManager.Instance.DealStageTask();
			UserDataManager.Instance.Save();
		}

		private IEnumerator FlyIconEnumerator(GameObject scroll, Vector3[] targetPosition, float flyTime)
		{
			float timer = 0f;
			yield return null;
			Vector3[] path = new Vector3[3]
			{
				targetPosition[0],
				targetPosition[1],
				targetPosition[2]
			};
			while (timer <= flyTime)
			{
				Vector3 position = (flyTime - timer) / flyTime * ((flyTime - timer) / flyTime * path[0] + timer / flyTime * path[1]) + timer / flyTime * ((flyTime - timer) / flyTime * path[1] + timer / flyTime * path[2]);
				if (scroll != null)
				{
					scroll.transform.position = position;
				}
				timer += Time.deltaTime;
				yield return null;
			}
			if (scroll != null)
			{
				scroll.transform.position = targetPosition[2];
			}
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public void GetRewards()
		{
			for (int i = 0; i < rewardItems.Count; i++)
			{
				GetReward(rewardItems[i].rewardInfos);
			}
		}

		public void GetReward(RewardInfo rewardInfo)
		{
			switch (rewardInfo.rewardType)
			{
			case RewardType.Life:
				if (UserDataManager.Instance.GetService().unlimitedLife)
				{
					UserDataManager.Instance.GetService().unlimitedLifeTM += rewardInfo.rewardNum * 60;
					break;
				}
				UserDataManager.Instance.GetService().unlimitedLife = true;
				UserDataManager.Instance.GetService().unlimitedLifeStartTM = DateTime.Now.Ticks / 10000000;
				UserDataManager.Instance.GetService().unlimitedLifeTM += rewardInfo.rewardNum * 60;
				break;
			case RewardType.Bomb:
				UserDataManager.Instance.GetService().bombNumber += rewardInfo.rewardNum;
				break;
			case RewardType.RainBowBall:
				UserDataManager.Instance.GetService().rainBowBallNumber += rewardInfo.rewardNum;
				break;
			case RewardType.DoubleBees:
				UserDataManager.Instance.GetService().doubleBeesNumber += rewardInfo.rewardNum;
				break;
			case RewardType.Mallet:
				DebugUtils.Log(DebugType.Other, "malletNumber" + UserDataManager.Instance.GetService().malletNumber);
				UserDataManager.Instance.GetService().malletNumber += rewardInfo.rewardNum;
				break;
			case RewardType.MagicMallet:
				UserDataManager.Instance.GetService().magicMalletNumber += rewardInfo.rewardNum;
				break;
			case RewardType.Glove:
				UserDataManager.Instance.GetService().gloveNumber += rewardInfo.rewardNum;
				break;
			case RewardType.Coin:
				DebugUtils.Log(DebugType.Other, "malletNumber" + UserDataManager.Instance.GetService().malletNumber);
				UserDataManager.Instance.GetService().coin += rewardInfo.rewardNum;
				break;
			}
		}

		public void BtnCloseClicked()
		{
			effectObj.gameObject.SetActive(false);
			for (int i = 0; i < rewardItems.Count; i++)
			{
				UnityEngine.Object.Destroy(rewardItems[i].gameObject);
			}
			DialogManagerTemp.Instance.CloseDialog(DialogType.StageRewardDlg);
		}

		public void PlayBtnClicked()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.StageRewardDlg);
		}
	}
}
