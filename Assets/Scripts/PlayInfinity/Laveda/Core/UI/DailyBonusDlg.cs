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
	public class DailyBonusDlg : BaseDialog
	{
		private static DailyBonusDlg instance;

		public GameObject btn;

		public GameObject itemMoveMidPos;

		public GameObject itemMoveEndPos;

		public GameObject lifeMoveMidPos;

		public GameObject lifeMoveEndPos;

		public GameObject lifeImage;

		public GameObject tailEffect;

		public List<DailyBonusItem> itemArray;

		public List<DropType> reward = new List<DropType>
		{
			DropType.ColorBomb,
			DropType.AreaBomb,
			DropType.DoubleBee,
			DropType.Glove,
			DropType.Hammer,
			DropType.Spoon
		};

		private int Bounouslevel;

		private List<DropType> dropArrayUser;

		public static DailyBonusDlg Instance
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

		public override void Show(object obj)
		{
			btn.GetComponent<Image>().raycastTarget = false;
			btn.SetActive(false);
			List<DropType> list = new List<DropType>(reward);
			int count = UserDataManager.Instance.GetService().DailyBonuseArray.Count;
			int dailyBonuseLevel = UserDataManager.Instance.GetService().DailyBonuseLevel;
			if (count != dailyBonuseLevel)
			{
				UserDataManager.Instance.GetService().DailyBonuseLevel = UserDataManager.Instance.GetService().DailyBonuseArray.Count;
			}
			Bounouslevel = UserDataManager.Instance.GetService().DailyBonuseLevel + 1;
			dropArrayUser = new List<DropType>(UserDataManager.Instance.GetService().DailyBonuseArray);
			DropType item = list[UnityEngine.Random.Range(0, 6)];
			dropArrayUser.Add(item);
			GetGoods(item);
			for (int i = 0; i < itemArray.Count; i++)
			{
				itemArray[i].HideIcon();
			}
			for (int j = 0; j < itemArray.Count; j++)
			{
				if (j < Bounouslevel - 1)
				{
					itemArray[j].SetImage(dropArrayUser[j]);
					itemArray[j].ShowIcon();
				}
				else if (j == Bounouslevel - 1)
				{
					DebugUtils.Log(DebugType.Other, j);
					itemArray[j].SetImage(dropArrayUser[j]);
				}
				else if (j >= Bounouslevel)
				{
					itemArray[j].HideIcon();
				}
			}
			base.Show(obj);
			float time = 0f;
			bool isfirst = true;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (isfirst && time > 1f)
				{
					isfirst = false;
					itemArray[Bounouslevel - 1].GetReward();
					btn.SetActive(true);
					AudioManager.Instance.PlayAudioEffect("kuijia1");
					btn.GetComponent<CanvasGroup>().DOFade(0f, 0f);
					btn.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(1f)
						.OnComplete(delegate
						{
							btn.GetComponent<Image>().raycastTarget = true;
						});
					return true;
				}
				time += duration;
				return false;
			}));
		}

		private void GetGoods(DropType type, int goodsNum = 1)
		{
			switch (type)
			{
			case DropType.AreaBomb:
				UserDataManager.Instance.GetService().bombNumber += goodsNum;
				break;
			case DropType.ColorBomb:
				UserDataManager.Instance.GetService().rainBowBallNumber += goodsNum;
				break;
			case DropType.DoubleBee:
				UserDataManager.Instance.GetService().doubleBeesNumber += goodsNum;
				break;
			case DropType.Hammer:
				UserDataManager.Instance.GetService().magicMalletNumber += goodsNum;
				break;
			case DropType.Spoon:
				UserDataManager.Instance.GetService().malletNumber += goodsNum;
				break;
			case DropType.Glove:
				UserDataManager.Instance.GetService().gloveNumber += goodsNum;
				break;
			}
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.DailyBonusDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			QuitBtnClick();
		}

		public void QuitBtnClick()
		{
			AudioManager.Instance.PlayAudioEffect("scroll_collect");
			UserDataManager.Instance.GetService().DailyBonuseArray = dropArrayUser;
			UserDataManager.Instance.GetService().DailyBonuseLevel++;
			UserDataManager.Instance.GetService().PreGetDailyBonusTime2 = DateTime.Now.ToString("yyyy:MM:dd");
			string[] array = UserDataManager.Instance.GetService().PreGetDailyBonusTime2.Split(':');
			int.Parse(array[2]);
			int.Parse(array[1]);
			int.Parse(array[0]);
			if (Bounouslevel == 7)
			{
				if (UserDataManager.Instance.GetService().unlimitedLife)
				{
					UserDataManager.Instance.GetService().unlimitedLifeTM += 3600L;
				}
				else
				{
					UserDataManager.Instance.GetService().unlimitedLife = true;
					UserDataManager.Instance.GetService().unlimitedLifeStartTM = DateTime.Now.Ticks / 10000000;
					UserDataManager.Instance.GetService().unlimitedLifeTM = 3600L;
				}
				StartCoroutine(StartFlyLife());
				UserDataManager.Instance.GetService().DailyBonuseLevel = 0;
				UserDataManager.Instance.GetService().DailyBonuseArray.Clear();
			}
			UserDataManager.Instance.Save();
			itemArray[Bounouslevel - 1].ShowSelectEffect();
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.7f)
				{
					Close();
					return true;
				}
				time += duration;
				return false;
			}));
		}

		private IEnumerator StartFlyLife()
		{
			yield return new WaitForSeconds(0.7f);
			GameObject go = UnityEngine.Object.Instantiate(lifeImage.gameObject, base.transform.parent);
			UnityEngine.Object.Instantiate(tailEffect.gameObject, go.transform);
			go.transform.position = lifeImage.transform.position;
			ShortcutExtensions.DOPath(path: new Vector3[3]
			{
				go.transform.position,
				Instance.lifeMoveMidPos.transform.position,
				Instance.lifeMoveEndPos.transform.position
			}, target: go.transform, duration: 0.7f, pathType: PathType.CatmullRom).OnComplete(delegate
			{
				CastleSceneUIManager.Instance.LifeImageScale();
				UnityEngine.Object.Destroy(go);
			});
		}
	}
}
