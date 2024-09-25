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
	public class GameLoseDlg : BaseDialog
	{
		public Text MoveCount;

		public GameObject Plus;

		public GameObject FlyBomb;

		public GameObject AreaBomb;

		public GameObject HSBomb;

		public GameObject ColorBomb;

		public Text GlodNum;

		public Transform Grid;

		private GameObject tarPrefab;

		public Text totalGold;

		public GameObject Combo;

		private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();

		private static GameLoseDlg instance;

		private List<GameObject> target = new List<GameObject>();

		public static GameLoseDlg Instance
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
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(34u, PurchaseSuccess);
			tarPrefab = Resources.Load("Prefabs/GameScene/Image2", typeof(GameObject)) as GameObject;
		}

		private void PurchaseSuccess(uint iMessageType, object arg)
		{
			totalGold.text = string.Concat(UserDataManager.Instance.GetService().coin);
		}

		public override void Show()
		{
			base.Show();
			totalGold.transform.parent.localScale = Vector3.zero;
			float time2 = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time2 > 0.4f)
				{
					totalGold.transform.parent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
					return true;
				}
				time2 += duration;
				return false;
			}));
			totalGold.text = string.Concat(UserDataManager.Instance.GetService().coin);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			Combo.SetActive(false);
			if (UserDataManager.Instance.GetIsComboing() && GlobalVariables.ComboNum >= 1)
			{
				Combo.SetActive(true);
				Combo.transform.Find("Icon").GetComponent<Image>().sprite = GetHaveElementAndCellTool.GetComboPicture(GlobalVariables.ComboNum);
				DialogManagerTemp.Instance.MaskAllDlg();
				float time3 = 0f;
				UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time3 > 0.3f)
					{
						DialogManagerTemp.Instance.CancelMaskAllDlg();
						return true;
					}
					time3 += duration;
					return false;
				}));
			}
			AudioManager.Instance.PlayAudioEffect("level_fail");
			AudioManager.Instance.ChangeMusicVolume(0.3f, 0.5f);
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 2.612f)
				{
					AudioManager.Instance.ChangeMusicVolume(1f, 0.5f);
					return true;
				}
				time += duration;
				return false;
			}));
			totalGold.transform.parent.localScale = Vector3.zero;
			float time2 = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time2 > 0.4f)
				{
					totalGold.transform.parent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
					return true;
				}
				time2 += duration;
				return false;
			}));
			totalGold.text = string.Concat(UserDataManager.Instance.GetService().coin);
			AddMovesConfigData addMovesData = AddMovesConfig.GetAddMovesData(GameLogic.Instance.continueNum);
			MoveCount.text = "+" + addMovesData.Moves;
			Plus.SetActive(addMovesData.Bee == 1 || addMovesData.Bee == 1 || addMovesData.Crown == 1 || addMovesData.Rocket == 1);
			FlyBomb.SetActive(addMovesData.Bee == 1);
			AreaBomb.SetActive(addMovesData.Bomb == 1);
			ColorBomb.SetActive(addMovesData.Crown == 1);
			HSBomb.SetActive(addMovesData.Rocket == 1);
			GlodNum.text = string.Concat(addMovesData.GoldNum);
			for (int i = 0; i < target.Count; i++)
			{
				UnityEngine.Object.Destroy(target[i]);
			}
			target.Clear();
			target = GameSceneUIManager.Instance.GetTargetGameObject(tarPrefab);
			if (target == null)
			{
				return;
			}
			foreach (GameObject item in target)
			{
				item.transform.SetParent(Grid, false);
			}
		}

		public void BtnConfirmClicked()
		{
			AddMovesConfigData addMovesData = AddMovesConfig.GetAddMovesData(GameLogic.Instance.continueNum);
			if (UserDataManager.Instance.GetService().coin >= addMovesData.GoldNum)
			{
				Close();
				UserDataManager.Instance.GetService().coin -= addMovesData.GoldNum;
				totalGold.text = string.Concat(UserDataManager.Instance.GetService().coin);
				UserDataManager.Instance.GetService().boosterPurchaseData[6]++;
				GA.Buy("Add5Move", 1, addMovesData.GoldNum);
				GA.Use("Add5Move", 1, addMovesData.GoldNum);
				GlobalVariables.UseAddStep = true;
				UserDataManager.Instance.Save();
				if (UserDataManager.Instance.GetService().coin < GeneralConfig.SendNoLifeLevelDataCoinNumber && !UserDataManager.Instance.GetService().NoGoldLevelSend && UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("NoGoldLevel", UserDataManager.Instance.GetService().level.ToString());
					Analytics.Event("NoGoldLevel", dictionary);
					Analytics.Event("NoGoldLevelCalculate", dictionary, UserDataManager.Instance.GetService().level);
					UserDataManager.Instance.GetService().NoGoldLevelSend = true;
					UserDataManager.Instance.Save();
				}
				GameLogic.Instance.levelData.move += addMovesData.Moves;
				GameSceneUIManager.Instance.SetMoveText(addMovesData.Moves);
				List<Cell> normalCellList = GetHaveElementAndCellTool.GetNormalCellList(GameLogic.Instance.currentBoard);
				normalCellList = ProcessTool.ListRandom(normalCellList);
				bool flag = false;
				new List<ElementRemoveInfo>();
				if (addMovesData.Bee == 1)
				{
					flag = true;
					GameLogic.Instance.currentBoard.RandomOneCellToCreateCustomElement(ElementType.FlyBomb);
					normalCellList.RemoveAt(normalCellList.Count - 1);
				}
				if (addMovesData.Rocket == 1)
				{
					flag = true;
					GameLogic.Instance.currentBoard.RandomOneCellToCreateCustomElement((UnityEngine.Random.Range(0, 2) == 0) ? ElementType.VerticalBomb : ElementType.HorizontalBomb);
					normalCellList.RemoveAt(normalCellList.Count - 1);
				}
				if (addMovesData.Crown == 1)
				{
					flag = true;
					GameLogic.Instance.currentBoard.RandomOneCellToCreateCustomElement(ElementType.ColorBomb);
					normalCellList.RemoveAt(normalCellList.Count - 1);
				}
				if (addMovesData.Bomb == 1)
				{
					flag = true;
					GameLogic.Instance.currentBoard.RandomOneCellToCreateCustomElement(ElementType.AreaBomb);
					normalCellList.RemoveAt(normalCellList.Count - 1);
				}
				if (flag)
				{
					Timer.Schedule(GameLogic.Instance, 1f, delegate
					{
						Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
					});
				}
				else
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
				}
			}
			else
			{
				DialogManagerTemp.Instance.OpenShopDlg("BuyInGameSceneMoves");
			}
		}

		public void BtnShareClicked()
		{
		}

		public void Close(bool isAnim = true)
		{
			totalGold.transform.parent.localScale = Vector3.zero;
			DialogManagerTemp.Instance.CloseDialog(DialogType.GameLoseDlg);
		}

		public void BtnCloseClicked()
		{
			totalGold.transform.parent.localScale = Vector3.zero;
			DialogManagerTemp.Instance.ShowDialog(DialogType.EnterGameDlg, 1, DialogType.GameLoseDlg);
			if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal - 1)
			{
				UserDataManager.Instance.GetService().lifeConsumeTime = DateTime.Now.Ticks / 10000000;
			}
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		private void OnDestroy()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(34u, PurchaseSuccess);
		}
	}
}
