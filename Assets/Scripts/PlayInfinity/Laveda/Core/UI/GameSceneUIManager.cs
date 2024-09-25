using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Pandora.Core.UI;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class GameSceneUIManager : BaseUIManager
	{
		public Text moveText;

		public Text levelText;

		public Sprite yellow;

		public Sprite Green;

		public GameObject tarPrefab;

		public GameObject settingPanel;

		public Button btnSetting;

		public GameObject skillTipPanel;

		public Transform Grid;

		public Transform GoldGrid;

		public Image[] targetImageArray;

		public Text[] targetTextArray;

		public Transform[] targetGouArray;

		public int[] itemUnlockLevel = new int[3] { 2, 3, 4 };

		public Image[] itemUnlockImageArray;

		public Image[] itemNumImageArray;

		public Text[] itemNumTextArray;

		public Image[] itemBuyImageArray;

		public GameObject[] ParticalArray;

		public Text[] lockTextArray;

		public Toggle[] itemToggleArray;

		public GameObject[] FreeArray;

		public Toggle AutoMatch;

		public GameObject TapToSkip;

		public GameObject Mask;

		private RectTransform rect;

		private IEnumerator lockTextShowCor;

		private IEnumerator SkillTipShowCor;

		private static GameSceneUIManager instance;

		public GameObject GoldNum;

		private Vector3 GoldNumV3;

		public GameObject Book;

		public Text GoldText;

		public Text BookText;

		public Transform Left;

		public Transform Right;

		public Button BtnSet;

		public Animation AnimBook;

		public Transform BookCollectPos;

		public Transform AreaBombStartFlyPos;

		public Transform ColorBombStartFlyPos;

		public Transform VHBombStartFlyPos;

		public Transform flyBombStartFlyPos;

		public Image PowerImage;

		public GameObject PingYu;

		private float powerValue;

		public GameObject ShuShowEff;

		public int sssss = 195;

		public float matchRate = 1f;

		private Tween tween1;

		private bool isOpenSkillTip;

		public Slider PowerCircle;

		public Image ButtonTip;

		public Image Button2Tip;

		public Image BombTip;

		public Text ElementNum;

		public GameObject SkillRoot;

		private bool _isCanSkipFinish;

		private int collectGoldNum;

		public static GameSceneUIManager Instance
		{
			get
			{
				return instance;
			}
		}

		public bool isCanSkipFinish
		{
			get
			{
				return _isCanSkipFinish;
			}
			set
			{
				if (value)
				{
					Image component = TapToSkip.GetComponent<Image>();
					Color color = component.color;
					color.a = 0f;
					component.color = color;
					component.DOFade(1f, 0.5f);
				}
				TapToSkip.SetActive(value);
				_isCanSkipFinish = value;
			}
		}

		public void ShowItem()
		{
			for (int i = 0; i < 3; i++)
			{
				ParticalArray[i].SetActive(false);
				FreeArray[i].SetActive(false);
				if (UserDataManager.Instance.GetProgress() >= GeneralConfig.ItemUnlockLevel[i + 3])
				{
					itemUnlockImageArray[i].transform.parent.gameObject.SetActive(false);
					itemNumImageArray[i].gameObject.SetActive(true);
					switch (i)
					{
					case 0:
					{
						int malletNumber = UserDataManager.Instance.GetService().malletNumber;
						itemNumTextArray[i].text = ((malletNumber <= 0) ? "" : malletNumber.ToString());
						DealBuyBtn(malletNumber, 0);
						break;
					}
					case 1:
					{
						int magicMalletNumber = UserDataManager.Instance.GetService().magicMalletNumber;
						itemNumTextArray[i].text = ((magicMalletNumber <= 0) ? "" : magicMalletNumber.ToString());
						DealBuyBtn(magicMalletNumber, 1);
						break;
					}
					case 2:
					{
						int gloveNumber = UserDataManager.Instance.GetService().gloveNumber;
						itemNumTextArray[i].text = ((gloveNumber <= 0) ? "" : gloveNumber.ToString());
						DealBuyBtn(gloveNumber, 2);
						break;
					}
					}
				}
				else
				{
					itemNumImageArray[i].gameObject.SetActive(false);
					itemUnlockImageArray[i].transform.parent.gameObject.SetActive(true);
					itemToggleArray[i].interactable = false;
				}
			}
		}

		public void FirstLockImageClick()
		{
			LockButtonClick(0);
		}

		public void SecondLockImageClick()
		{
			LockButtonClick(1);
		}

		public void ThirdLockImageClick()
		{
			LockButtonClick(2);
		}

		public void LockButtonClick(int index)
		{
			for (int i = 0; i < 3; i++)
			{
				if (i != index && lockTextArray[i].transform.parent.gameObject.activeSelf)
				{
					lockTextArray[i].transform.parent.GetComponent<Animation>().Stop();
					int flag = i;
					lockTextArray[i].transform.parent.DOScale(Vector3.zero, 0.15f).OnComplete(delegate
					{
						lockTextArray[flag].transform.parent.gameObject.SetActive(false);
					});
				}
			}
			lockTextArray[index].transform.parent.gameObject.SetActive(false);
			lockTextArray[index].transform.parent.gameObject.SetActive(true);
			if (lockTextShowCor != null)
			{
				StopCoroutine(lockTextShowCor);
			}
			lockTextShowCor = HideText(lockTextArray[index].transform.parent.gameObject);
			StartCoroutine(lockTextShowCor);
		}

		private IEnumerator HideText(GameObject go)
		{
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 2.5f)
				{
					return true;
				}
				time += duration;
				return false;
			}));
			yield return new WaitUntil(() => time > 2.5f);
			go.SetActive(false);
		}

		public void DealBuyBtn(int number, int index)
		{
			ParticalArray[index].SetActive(false);
			FreeArray[index].SetActive(false);
			itemNumImageArray[index].gameObject.SetActive(true);
			switch (index)
			{
			case 0:
				if (UserDataManager.Instance.GetProgress() == 9 && GameLogic.Instance.boosterUsageData[3] == 0)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
					number++;
				}
				break;
			case 1:
				if (UserDataManager.Instance.GetProgress() == 19 && GameLogic.Instance.boosterUsageData[4] == 0)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
					number++;
				}
				break;
			case 2:
				if (UserDataManager.Instance.GetProgress() == 28 && GameLogic.Instance.boosterUsageData[5] == 0)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
					number++;
				}
				break;
			}
			if (number <= 0)
			{
				itemToggleArray[index].interactable = false;
				itemBuyImageArray[index].gameObject.SetActive(true);
				itemNumImageArray[index].sprite = Green;
			}
			else
			{
				itemToggleArray[index].interactable = true;
				itemBuyImageArray[index].gameObject.SetActive(false);
				itemNumImageArray[index].sprite = yellow;
			}
		}

		public void InitTargetInfo(int[] tarArray)
		{
			targetImageArray = new Image[tarArray.Length + 1];
			targetTextArray = new Text[tarArray.Length + 1];
			targetGouArray = new Transform[tarArray.Length + 1];
			for (int i = 1; i < tarArray.Length; i++)
			{
				if (tarArray[i] == 8)
				{
					GameLogic.Instance.levelData.targetList[tarArray[i]] = GameLogic.Instance.TotalNumBeGrass;
					GameLogic.Instance.levelData.targetListByCollect[tarArray[i]] = GameLogic.Instance.TotalNumBeGrass;
				}
				else if (tarArray[i] == 11)
				{
					GameLogic.Instance.levelData.targetList[tarArray[i]] = GameLogic.Instance.TotalNumBeCloud;
					GameLogic.Instance.levelData.targetListByCollect[tarArray[i]] = GameLogic.Instance.TotalNumBeCloud;
				}
				int num = GameLogic.Instance.levelData.targetList[tarArray[i]];
				GameObject gameObject = UnityEngine.Object.Instantiate(tarPrefab);
				gameObject.transform.SetParent(Grid, false);
				targetImageArray[i] = gameObject.GetComponentInChildren<Image>();
				targetTextArray[i] = gameObject.GetComponentInChildren<Text>();
				targetImageArray[i].sprite = GetHaveElementAndCellTool.GetPicture(tarArray[i] + 1);
				targetTextArray[i].text = string.Concat(num);
				targetGouArray[i] = gameObject.transform.Find("Gou");
				targetGouArray[i].gameObject.SetActive(false);
				Transform target = targetImageArray[i].transform;
				target.DOScale(1f, 0.3f).OnComplete(delegate
				{
					target.DOScale(0.76f, 0.4f);
				}).SetAutoKill(false)
					.Pause();
			}
		}

		public List<GameObject> GetTargetGameObject()
		{
			if (targetGouArray == null)
			{
				return null;
			}
			List<GameObject> list = new List<GameObject>();
			for (int i = 1; i < targetGouArray.Length - 1; i++)
			{
				if (!targetGouArray[i].gameObject.activeSelf)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(tarPrefab);
					gameObject.GetComponentInChildren<Image>().sprite = targetImageArray[i].sprite;
					gameObject.GetComponentInChildren<Text>().text = targetTextArray[i].text;
					list.Add(gameObject);
				}
			}
			return list;
		}

		public List<GameObject> GetTargetGameObject(GameObject objj)
		{
			if (targetGouArray == null)
			{
				return null;
			}
			List<GameObject> list = new List<GameObject>();
			for (int i = 1; i < targetGouArray.Length - 1; i++)
			{
				if (!targetGouArray[i].gameObject.activeSelf)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(objj);
					gameObject.GetComponentInChildren<Image>().sprite = targetImageArray[i].sprite;
					gameObject.GetComponentInChildren<Text>().text = targetTextArray[i].text;
					list.Add(gameObject);
				}
			}
			return list;
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
			AddMovesConfig.Load();
			Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].onClick.AddListener(delegate
				{
					AudioManager.Instance.PlayAudioEffect("general_button");
				});
			}
		}

		public void OnToggleClick(Toggle toggle, bool isOn)
		{
		}

		protected override void Start()
		{
			ShuShowEff.SetActive(false);
			TapToSkip.SetActive(false);
			AutoMatch.onValueChanged.AddListener(AutoMatchOpen);
			if ((float)(Screen.width / Screen.height) >= 1.77777779f)
			{
				Left.GetComponent<RectTransform>().localPosition += new Vector3(30f, 0f, 0f);
				Right.GetComponent<RectTransform>().localPosition -= new Vector3(30f, 0f, 0f);
				SkillRoot.GetComponent<RectTransform>().localPosition += new Vector3(30f, 0f, 0f);
				settingPanel.GetComponent<RectTransform>().localPosition -= new Vector3(30f, 0f, 0f);
				btnSetting.GetComponent<RectTransform>().localPosition -= new Vector3(30f, 0f, 0f);
			}
			rect = GetComponent<RectTransform>();
			if (rect.rect.height > 720f)
			{
				float num = (rect.rect.height - 720f) / (float)sssss;
				Camera.main.orthographicSize = 3.8f + num;
			}
			else
			{
				Camera.main.orthographicSize = 3.8f;
			}
			matchRate = Camera.main.orthographicSize / 3.8f;
			if (UserDataManager.Instance.GetService().tutorialProgress <= 2)
			{
				btnSetting.interactable = false;
			}
			tarPrefab = Resources.Load("Prefabs/GameScene/Image", typeof(GameObject)) as GameObject;
			GoldNum = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GameScene/Gold", typeof(GameObject)) as GameObject);
			GoldNum.transform.SetParent(GoldGrid, false);
			GoldNum.GetComponent<RectTransform>().localPosition = Vector3.zero;
			GoldNumV3 = GoldNum.transform.position;
			GoldText = GoldNum.GetComponentInChildren<Text>();
			itemToggleArray[0].onValueChanged.AddListener(ToggleOneSeleted);
			itemToggleArray[1].onValueChanged.AddListener(ToggleTwoSeleted);
			itemToggleArray[2].onValueChanged.AddListener(ToggleThreeSeleted);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(34u, PurchaseSuccess);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(6u, EnableOrDeEnablePlayerControll);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(10u, GameEnd);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(15u, AddSkillPower);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(17u, InitSkillPanel);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(18u, DropGuide);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(19u, ActivePingYu);
		}

		private void PurchaseSuccess(uint iMessageType, object arg)
		{
			ShowItem();
		}

		public override void ShowUI()
		{
			base.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (!UpdateManager.Instance.isPause)
			{
				PowerCircle.value = Mathf.Lerp(PowerCircle.value, powerValue, Time.deltaTime * Singleton<PlayGameData>.Instance().gameConfig.powerValueSpeed);
			}
		}

		private void LateUpdate()
		{
			if (isCanSkipFinish && Input.GetMouseButtonDown(0))
			{
				UpdateManager.Instance.PauseGame();
				SkipFinish();
			}
		}

		private void OnDestroy()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(6u, EnableOrDeEnablePlayerControll);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(10u, GameEnd);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(15u, AddSkillPower);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(17u, InitSkillPanel);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(18u, DropGuide);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(19u, ActivePingYu);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(34u, PurchaseSuccess);
		}

		public void SetLevelText(int level)
		{
			levelText.text = "Lv." + level;
		}

		public void SetMoveText(int move)
		{
			move = ((move >= 0) ? move : 0);
			moveText.text = move.ToString();
		}

		public bool UpdateTargetNum()
		{
			bool flag = true;
			for (int i = 1; i < GameLogic.Instance.tarIDArray.Length; i++)
			{
				int num = GameLogic.Instance.levelData.targetList[GameLogic.Instance.tarIDArray[i]];
				if (Convert.ToInt32(targetTextArray[i].text) > num)
				{
					targetImageArray[i].transform.DORestart();
					targetImageArray[i].GetComponentInChildren<ParticleSystem>().Play();
				}
				if (Convert.ToInt32(targetTextArray[i].text) > 0 && num == 0)
				{
					targetTextArray[i].gameObject.SetActive(false);
					targetGouArray[i].gameObject.SetActive(true);
					AudioManager.Instance.PlayAudioEffect("collected_tick");
				}
				targetTextArray[i].text = string.Concat((num >= 0) ? num : 0);
				if (flag)
				{
					flag = ((num <= 0) ? true : false);
				}
			}
			return flag;
		}

		public void BtnSettingClicked()
		{
			settingPanel.SetActive(!settingPanel.activeSelf);
		}

		public void SkillTipBtnClicked(GameObject obj)
		{
			skillTipPanel.SetActive(false);
			skillTipPanel.SetActive(true);
			if (SkillTipShowCor != null)
			{
				StopCoroutine(SkillTipShowCor);
			}
			SkillTipShowCor = HideText(skillTipPanel.gameObject);
			StartCoroutine(SkillTipShowCor);
		}

		public void BtnPassClicked()
		{
			GameLogic.Instance.SettleAccounts(null, null);
			Singleton<MessageDispatcher>.Instance().SendMessage(10u, null);
		}

		public void SkipFinish()
		{
			if (GameLogic.Instance.initCoinsNum + GetCollectGoldNum() > UserDataManager.Instance.GetService().coin)
			{
				UserDataManager.Instance.GetService().coin = GameLogic.Instance.initCoinsNum + GetCollectGoldNum();
			}
			Singleton<MessageDispatcher>.Instance().SendMessage(10u, null);
		}

		public void FirstBuyBtnClick()
		{
			Debug.Log("BuySomeThing");
			ShowBuyItemDlg(DropType.Spoon);
		}

		public void SecondBuyBtnClick()
		{
			ShowBuyItemDlg(DropType.Hammer);
		}

		public void ThirdBuyBtnClick()
		{
			ShowBuyItemDlg(DropType.Glove);
		}

		public void ShowBuyItemDlg(DropType type)
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.BuyInGameDlg, type);
		}

		public void BtnFailClicked()
		{
			SceneTransManager.Instance.TransToSwitch(SceneType.CastleScene);
		}

		public Vector3 GetCollectEndPos(int index)
		{
			return targetImageArray[index].transform.position;
		}

		public Vector3 GetGoldEndPos()
		{
			if (GoldNum != null)
			{
				return GoldNum.transform.position;
			}
			return GoldNumV3;
		}

		public void ToggleOneSeleted(bool isSelected)
		{
			GameLogic.Instance.dropType = (isSelected ? DropType.Spoon : DropType.None);
			ParticalArray[0].SetActive(isSelected);
		}

		public void ToggleTwoSeleted(bool isSelected)
		{
			GameLogic.Instance.dropType = (isSelected ? DropType.Hammer : DropType.None);
			ParticalArray[1].SetActive(isSelected);
		}

		public void ToggleThreeSeleted(bool isSelected)
		{
			GameLogic.Instance.dropType = (isSelected ? DropType.Glove : DropType.None);
			ParticalArray[2].SetActive(isSelected);
		}

		public void DropUsed(DropType type, bool isSuccessful)
		{
			if (!isSuccessful)
			{
				Toggle[] array = itemToggleArray;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].isOn = false;
				}
				GameObject[] particalArray = ParticalArray;
				for (int i = 0; i < particalArray.Length; i++)
				{
					particalArray[i].SetActive(false);
				}
				GameLogic.Instance.dropType = DropType.None;
				return;
			}
			GameLogic.Instance.DropUseNum++;
			switch (type)
			{
			case DropType.Spoon:
			{
				GameLogic.Instance.boosterUsageData[3]++;
				if (UserDataManager.Instance.GetProgress() == 9 && GameLogic.Instance.boosterUsageData[3] == 1)
				{
					ShowItem();
				}
				else
				{
					UserDataManager.Instance.GetService().malletNumber--;
					GA.Use("Spoon", 1, GeneralConfig.ItemBuyPrice[3] / 3);
				}
				int malletNumber = UserDataManager.Instance.GetService().malletNumber;
				itemNumTextArray[0].text = ((malletNumber <= 0) ? "" : malletNumber.ToString());
				itemToggleArray[0].isOn = false;
				ParticalArray[0].SetActive(false);
				if (malletNumber <= 0)
				{
					itemToggleArray[0].interactable = false;
					itemBuyImageArray[0].gameObject.SetActive(true);
					itemNumImageArray[0].sprite = Green;
				}
				break;
			}
			case DropType.Hammer:
			{
				GameLogic.Instance.boosterUsageData[4]++;
				if (UserDataManager.Instance.GetProgress() == 19 && GameLogic.Instance.boosterUsageData[4] == 1)
				{
					ShowItem();
				}
				else
				{
					UserDataManager.Instance.GetService().magicMalletNumber--;
					GA.Use("Hammer", 1, GeneralConfig.ItemBuyPrice[4] / 3);
				}
				int magicMalletNumber = UserDataManager.Instance.GetService().magicMalletNumber;
				itemNumTextArray[1].text = ((magicMalletNumber <= 0) ? "" : magicMalletNumber.ToString());
				itemToggleArray[1].isOn = false;
				ParticalArray[1].SetActive(false);
				if (magicMalletNumber <= 0)
				{
					itemToggleArray[1].interactable = false;
					itemBuyImageArray[1].gameObject.SetActive(true);
					itemNumImageArray[1].sprite = Green;
				}
				break;
			}
			case DropType.Glove:
			{
				GameLogic.Instance.boosterUsageData[5]++;
				if (UserDataManager.Instance.GetProgress() == 28 && GameLogic.Instance.boosterUsageData[5] == 1)
				{
					ShowItem();
				}
				else
				{
					UserDataManager.Instance.GetService().gloveNumber--;
					GA.Use("Glove", 1, GeneralConfig.ItemBuyPrice[5] / 3);
				}
				int gloveNumber = UserDataManager.Instance.GetService().gloveNumber;
				itemNumTextArray[2].text = ((gloveNumber <= 0) ? "" : gloveNumber.ToString());
				itemToggleArray[2].isOn = false;
				ParticalArray[2].SetActive(false);
				if (gloveNumber <= 0)
				{
					itemToggleArray[2].interactable = false;
					itemBuyImageArray[2].gameObject.SetActive(true);
					itemNumImageArray[2].sprite = Green;
				}
				break;
			}
			}
			GameLogic.Instance.dropType = DropType.None;
		}

		public void ActiveCoinAndBookGrid(bool active = true)
		{
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			UpdateCoinNumInGameScene((GameLogic.Instance.levelData.hard == 0) ? gameConfig.normalCoinNum : gameConfig.hardCoinNum);
			Image[] componentsInChildren = Grid.GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].DOFade(0f, 0.5f);
                componentsInChildren[i].enabled = false;

            }
			Text[] componentsInChildren2 = Grid.GetComponentsInChildren<Text>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].DOFade(0f, 0.5f);
			}
			componentsInChildren = GoldGrid.GetComponentsInChildren<Image>();
			foreach (Image target in componentsInChildren)
			{
				target.DOFade(0f, 0f);
				target.DOFade(1f, 0.5f).SetDelay(0.5f);
			}
			componentsInChildren2 = GoldGrid.GetComponentsInChildren<Text>();
			foreach (Text target2 in componentsInChildren2)
			{
				target2.DOFade(0f, 0f);
				target2.DOFade(1f, 0.5f).SetDelay(0.5f);
			}
			GoldGrid.gameObject.SetActive(active);
		}

		public void UpdateCoinNumInGameScene(int num = 0)
		{
			collectGoldNum += num;
			if (GoldText != null)
			{
				GoldText.text = string.Concat(collectGoldNum);
			}
		}

		public int GetCollectGoldNum()
		{
			return collectGoldNum;
		}

		public int GetCollectBookNum()
		{
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			if (GameLogic.Instance.levelData.hard != 2)
			{
				return gameConfig.normalBookNum;
			}
			return gameConfig.hardBookNum;
		}

		public Vector3 GetSkillCollectEndPos()
		{
			return BookCollectPos.position;
		}

		public void InitSkillPanel(uint iMessageType, object arg)
		{
			InitSkillPanel initSkillPanel = (InitSkillPanel)arg;
			int num = -1;
			int num2 = -1;
			int[] skillProbabilityList = GameLogic.Instance.levelData.skillProbabilityList;
			for (int i = 0; i < skillProbabilityList.Length; i++)
			{
				if (skillProbabilityList[i] > 0)
				{
					num2 = skillProbabilityList[i];
					num = i + 1;
					break;
				}
			}
			switch (num)
			{
			case -1:
				SkillRoot.SetActive(false);
				return;
			case 1:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/lvtiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 2:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/baitiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 3:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/lantiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 4:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/chengtiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 5:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/hongtiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 6:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/huangtiao").GetComponent<SpriteRenderer>().sprite;
				break;
			case 7:
				PowerImage.sprite = Resources.Load<GameObject>("Textures/GameScene/zitiao").GetComponent<SpriteRenderer>().sprite;
				break;
			}
			PowerCircle.value = 0f;
			ButtonTip.sprite = GeneralConfig.ElementPictures[num];
			Button2Tip.sprite = ButtonTip.sprite;
			BombTip.sprite = GeneralConfig.SkillPictures[num];
			ElementNum.text = string.Concat(num2);
			SkillRoot.SetActive(true);
		}

		public void AddSkillPower(uint iMessageType, object arg)
		{
			powerValue += (float)arg;
			if (!ShuShowEff.activeSelf && !SkillManager.Instance.GetIsSkilling())
			{
				ShuShowEff.SetActive(true);
				float time = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time > 0.8f)
					{
						ShuShowEff.SetActive(false);
						return true;
					}
					time += duration;
					return false;
				}));
			}
			if (powerValue >= 1f)
			{
				powerValue = 1f - powerValue;
				Singleton<MessageDispatcher>.Instance().SendMessage(16u, null);
			}
		}

		public void EnableOrDeEnablePlayerControll(uint iMessageType, object arg)
		{
			bool flag = (bool)arg;
			Toggle[] array = itemToggleArray;
			foreach (Toggle toggle in array)
			{
				if (!GameLogic.Instance.isGuiding)
				{
					toggle.interactable = !GameLogic.Instance.isFinish && flag;
				}
			}
			DropUsed(GameLogic.Instance.dropType, false);
			int i;
			for (i = 0; i < 3; i++)
			{
				if (lockTextArray[i].transform.parent.gameObject.activeSelf)
				{
					lockTextArray[i].transform.parent.GetComponent<Animation>().Stop();
					lockTextArray[i].transform.parent.DOScale(Vector3.zero, 0.15f).OnComplete(delegate
					{
						lockTextArray[i].transform.parent.gameObject.SetActive(false);
					});
				}
			}
			if (isOpenSkillTip)
			{
				SkillTipBtnClicked(null);
			}
		}

		public void GameEnd(uint iMessageType, object arg)
		{
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			Left.DOLocalMoveX(Left.localPosition.x - 300f, 0.5f);
			Right.DOLocalMoveX(Right.localPosition.x + 300f, 0.5f);
			btnSetting.transform.DOLocalMoveX(Right.localPosition.x + 300f, 0.5f);
		}

		public void DropGuide(uint iMessageType, object arg)
		{
			int num = (int)arg;
			if (num == -1)
			{
				for (int i = 0; i < itemToggleArray.Length; i++)
				{
					itemToggleArray[i].interactable = true;
				}
				for (int j = 0; j < itemUnlockImageArray.Length; j++)
				{
					itemUnlockImageArray[j].raycastTarget = true;
				}
				BtnSet.interactable = true;
			}
			else
			{
				for (int k = 0; k < itemToggleArray.Length; k++)
				{
					itemToggleArray[k].interactable = ((k == num) ? true : false);
				}
				for (int l = 0; l < itemUnlockImageArray.Length; l++)
				{
					itemUnlockImageArray[l].raycastTarget = ((l == num) ? true : false);
				}
				BtnSet.interactable = false;
			}
		}

		public void ActivePingYu(uint iMessageType, object arg)
		{
			PingYuType key = (PingYuType)arg;
			AudioManager.Instance.PlayAudioEffect("comment_all");
			PingYu.GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture = GeneralConfig.CreaterTexturePictures[(int)key];
			PingYu.SetActive(true);
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 1.5f)
				{
					if (PingYu != null)
					{
						PingYu.SetActive(false);
					}
					return true;
				}
				time += duration;
				return false;
			}));
		}

		public void AutoMatchOpen(bool isOn)
		{
			TestConfig.isAutoMatch = isOn;
			GameLogic.Instance.IsRefeshElement(GameLogic.Instance.currentBoard);
		}

		public void MaskGameUI()
		{
			Mask.SetActive(true);
		}

		public void CancelMaskGameUI()
		{
			Mask.SetActive(false);
		}
	}
}
