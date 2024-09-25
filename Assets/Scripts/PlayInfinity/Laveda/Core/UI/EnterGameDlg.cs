using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.AliceMatch3.Editor;
using PlayInfinity.GameEngine.Common;
using SimpleJSON;
using Umeng;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class EnterGameDlg : BaseDialog
	{
		public Image Title;

		public Image DlgBG;

		public Image DlgBG2;

		public Image DlgBG3;

		public Image playBtn;

		public GameObject arrow;

		public Button startGameBtn;

		public LocalizationText titleText;

		public LocalizationText playText;

		public GameObject targetItem1;

		public GameObject targetItem2;

		public Image firstTargetImage;

		public Text firstTargetText;

		public LevelData levelData = new LevelData();

		public Image secondTargetImage;

		public Text secondTargetText;

		public Image[] itemUnlockImageArray;

		public Image[] itemNumImageArray;

		public Text[] itemNumTextArray;

		public Image[] itemBuyImageArray;

		public Image[] selectImageArray;

		public Image[] selectBgArray;

		public GameObject[] FreeArray;

		public LocalizationText[] lockTextArray;

		public GameObject[] effectArray;

		private int[] selectIndex;

		private IEnumerator lockTextShowCor;

		private int showType;

		private Tweener arrowTweener;

		private Vector3 arrowStartPosition;

		private bool retry;

		public GameObject Combo;

		private static EnterGameDlg instance;

		private int GrassNum;

		private int CloudNum;

		public static EnterGameDlg Instance
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
			arrowStartPosition = arrow.transform.localPosition;
			startGameBtn.onClick.AddListener(delegate
			{
				StartBtnClick();
			});
			GeneralConfig.Load();
		}

		public void StartBtnClick()
		{
			if (UserDataManager.Instance.GetService().life > 0 || UserDataManager.Instance.GetService().unlimitedLife)
			{
				if (!UserDataManager.Instance.GetService().unlimitedLife)
				{
					if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal)
					{
						UserDataManager.Instance.GetService().lifeConsumeTime = DateTime.Now.Ticks / 10000000;
					}
					UserDataManager.Instance.GetService().life--;
				}
				DealItem();
				UserDataManager.Instance.Save();
				showType = 0;
				UpdateManager.Instance.ClearLoopData();
				UpdateManager.Instance.PlayGame();
				if (UserDataManager.Instance.GetIsComboing() && retry)
				{
					GlobalVariables.ComboNum--;
				}
				SceneTransManager.Instance.ChangeSceneWithEffect(delegate
				{
					DialogManagerTemp.Instance.CloseAllDialogs();
					SceneTransManager.Instance.TransToSwitch(SceneType.GameScene);
				});
			}
			else
			{
				DialogManagerTemp.Instance.ShowDialog(DialogType.GetLifeDlg, null, DialogType.EnterGameDlg);
			}
		}

		public void DealItem()
		{
			if (GameConfig.CreateOneAreaBoom && UserDataManager.Instance.GetProgress() != 14)
			{
				UserDataManager.Instance.GetService().bombNumber--;
				GA.Use("TNT", 1, GeneralConfig.ItemBuyPrice[0] / 3);
			}
			if (GameConfig.CreateOneColorBoom && UserDataManager.Instance.GetProgress() != 25)
			{
				UserDataManager.Instance.GetService().rainBowBallNumber--;
				GA.Use("Crown", 1, GeneralConfig.ItemBuyPrice[1] / 3);
			}
			if (GameConfig.BeeLevel == 2 && UserDataManager.Instance.GetProgress() != 34)
			{
				UserDataManager.Instance.GetService().doubleBeesNumber--;
				GA.Use("Bee", 1, GeneralConfig.ItemBuyPrice[2] / 3);
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void Show()
		{
			base.Show();
			ShowItem();
		}

		public override void Show(object obj)
		{
			UpdateUISprite();
			Combo.SetActive(false);
			if (obj != null)
			{
				if ((int)obj == 1 || (int)obj == 2)
				{
					selectIndex = new int[3] { -1, -1, -1 };
					showType = (int)obj;
					if ((int)obj == 1 || (int)obj == 2)
					{
						GlobalVariables.ComboNum--;
						retry = true;
						UpdateManager.Instance.PauseGame();
						base.gameObject.SetActive(true);
						titleText.text = string.Format(LanguageConfig.GetString("EnterGameDlg_level"), UserDataManager.Instance.GetService().level);
						GetLevelData();
						ShowTarget();
						ShowItem();
						UpdateSelectImage();
						playText.SetKeyString("EnterGameDlg_Retry");
						base.Show(obj);
					}
				}
			}
			else
			{
				retry = false;
				selectIndex = new int[3] { -1, -1, -1 };
				CastleSceneUIManager.Instance.HideAllBtn();
				RoleManager.Instance.HideAllRoles();
				int tutorialProgress = UserDataManager.Instance.GetService().tutorialProgress;
				int num = UserDataManager.Instance.GetService().level;
				if (tutorialProgress >= 6 && num <= 5)
				{
					arrow.SetActive(true);
				}
				else
				{
					arrow.SetActive(false);
				}
				titleText.text = string.Format(LanguageConfig.GetString("EnterGameDlg_level"), UserDataManager.Instance.GetService().level);
				GetLevelData();
				ShowTarget();
				ShowItem();
				UpdateSelectImage();
				playText.SetKeyString("EnterGameDlg_Start");
				base.Show(obj);
				startGameBtn.enabled = false;
				StartCoroutine(ShowTutorialDlg());
			}
			if (!UserDataManager.Instance.GetIsComboing() || GlobalVariables.ComboNum < 1)
			{
				return;
			}
			Combo.SetActive(true);
			Combo.GetComponentInChildren<Image>().sprite = GetHaveElementAndCellTool.GetComboPicture(GlobalVariables.ComboNum);
			DialogManagerTemp.Instance.MaskAllDlg();
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.3f)
				{
					DialogManagerTemp.Instance.CancelMaskAllDlg();
					return true;
				}
				time += duration;
				return false;
			}));
		}

		private void UpdateUISprite()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Levels/Level_" + UserDataManager.Instance.GetProgress());
			SimpleJSON.JSONNode jSONNode = HotFixScript.Instance.LoadLevelData(UserDataManager.Instance.GetProgress());
			if (jSONNode == null)
			{
				jSONNode = SimpleJSON.JSONNode.Parse(textAsset.text);
			}
			switch (int.Parse(jSONNode["hard"]))
			{
			case 0:
				Title.sprite = GeneralConfig.UIPictures[UIType.Easy_title];
				DlgBG.sprite = GeneralConfig.UIPictures[UIType.Easy_DlgBG];
				DlgBG2.sprite = GeneralConfig.UIPictures[UIType.Easy_DlgBG2];
				DlgBG3.sprite = GeneralConfig.UIPictures[UIType.Easy_DlgBG3];
				playBtn.sprite = GeneralConfig.UIPictures[UIType.Easy_green];
				break;
			case 1:
				Title.sprite = GeneralConfig.UIPictures[UIType.Normal_title];
				DlgBG.sprite = GeneralConfig.UIPictures[UIType.Normal_DlgBG];
				DlgBG2.sprite = GeneralConfig.UIPictures[UIType.Normal_DlgBG2];
				DlgBG3.sprite = GeneralConfig.UIPictures[UIType.Normal_DlgBG3];
				playBtn.sprite = GeneralConfig.UIPictures[UIType.Normal_green];
				break;
			case 2:
				Title.sprite = GeneralConfig.UIPictures[UIType.Hard_title];
				DlgBG.sprite = GeneralConfig.UIPictures[UIType.Hard_DlgBG];
				DlgBG2.sprite = GeneralConfig.UIPictures[UIType.Hard_DlgBG2];
				DlgBG3.sprite = GeneralConfig.UIPictures[UIType.Hard_DlgBG3];
				playBtn.sprite = GeneralConfig.UIPictures[UIType.Hard_green];
				break;
			}
		}

		public void ShowItem()
		{
			for (int i = 0; i < 3; i++)
			{
				effectArray[i].gameObject.SetActive(false);
				FreeArray[i].gameObject.SetActive(false);
				int startLevel = UserDataManager.Instance.GetService().level;
				if (TestConfig.active)
				{
					startLevel = TestConfig.startLevel;
				}
				if (startLevel >= GeneralConfig.ItemUnlockLevel[i])
				{
					itemUnlockImageArray[i].transform.parent.gameObject.SetActive(false);
					itemNumImageArray[i].gameObject.SetActive(true);
					switch (i)
					{
					case 0:
					{
						int bombNumber = UserDataManager.Instance.GetService().bombNumber;
						itemNumTextArray[i].text = bombNumber.ToString();
						DealBuyBtn(bombNumber, 0);
						break;
					}
					case 1:
					{
						int rainBowBallNumber = UserDataManager.Instance.GetService().rainBowBallNumber;
						itemNumTextArray[i].text = rainBowBallNumber.ToString();
						DealBuyBtn(rainBowBallNumber, 1);
						break;
					}
					case 2:
					{
						int doubleBeesNumber = UserDataManager.Instance.GetService().doubleBeesNumber;
						itemNumTextArray[i].text = doubleBeesNumber.ToString();
						DealBuyBtn(doubleBeesNumber, 2);
						break;
					}
					}
				}
				else
				{
					itemNumImageArray[i].gameObject.SetActive(false);
					itemUnlockImageArray[i].transform.parent.gameObject.SetActive(true);
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
				if (i == index)
				{
					continue;
				}
				int selfIndex = i;
				if (lockTextArray[i].transform.parent.gameObject.activeSelf)
				{
					lockTextArray[i].transform.parent.GetComponent<Animation>().Stop();
					lockTextArray[i].transform.parent.DOScale(Vector3.zero, 0.15f).OnComplete(delegate
					{
						lockTextArray[selfIndex].transform.parent.gameObject.SetActive(false);
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
			yield return new WaitForSeconds(3f);
			go.SetActive(false);
		}

		private IEnumerator ShowTutorialDlg()
		{
			yield return new WaitForSeconds(hidingAnimation.length);
			startGameBtn.enabled = true;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				if (UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[i])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				TutorialManager.Instance.ShowBoosterTutorial();
			}
		}

		public void DealBuyBtn(int number, int index)
		{
			if (number <= 0)
			{
				itemBuyImageArray[index].gameObject.SetActive(true);
			}
			else
			{
				itemBuyImageArray[index].gameObject.SetActive(false);
			}
			switch (index)
			{
			case 0:
				if (UserDataManager.Instance.GetProgress() == 14)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
				}
				break;
			case 1:
				if (UserDataManager.Instance.GetProgress() == 25)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
				}
				break;
			case 2:
				if (UserDataManager.Instance.GetProgress() == 34)
				{
					itemNumImageArray[index].gameObject.SetActive(false);
					FreeArray[index].SetActive(true);
				}
				break;
			}
		}

		public void GetLevelData()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Levels/Level_" + UserDataManager.Instance.GetProgress());
			SimpleJSON.JSONNode jSONNode = HotFixScript.Instance.LoadLevelData(UserDataManager.Instance.GetProgress());
			if (jSONNode == null)
			{
				jSONNode = SimpleJSON.JSONNode.Parse(textAsset.text);
			}
			levelData.targetList = new int[jSONNode["targetList"].Count];
			for (int i = 0; i < jSONNode["targetList"].Count; i++)
			{
				levelData.targetList[i] = int.Parse(jSONNode["targetList"][i]);
			}
			levelData.boardData = new BoardData[jSONNode["boardData"].Count];
			for (int j = 0; j < jSONNode["boardData"].Count; j++)
			{
				levelData.boardData[j] = new BoardData();
				int num = int.Parse(jSONNode["boardData"][j]["width"]);
				levelData.boardData[j].width = num;
				int num2 = int.Parse(jSONNode["boardData"][j]["height"]);
				levelData.boardData[j].height = num2;
				int num3 = num * num2;
				levelData.boardData[j].map = new int[num3];
				for (int k = 0; k < num3; k++)
				{
					levelData.boardData[j].map[k] = int.Parse(jSONNode["boardData"][j]["map"][k]);
				}
			}
			bool flag = levelData.targetList[8] > 0;
			bool flag2 = levelData.targetList[11] > 0;
			for (int l = 0; l < levelData.boardData.Length; l++)
			{
				BoardData boardData = levelData.boardData[l];
				int width = boardData.width;
				int height = boardData.height;
				if (!(flag || flag2))
				{
					continue;
				}
				Debug.Log(height + "    |    " + width);
				for (int m = 0; m < height; m++)
				{
					StringBuilder stringBuilder = new StringBuilder("\t");
					for (int n = 0; n < width; n++)
					{
						int num4 = boardData.map[m * boardData.width + n];
						if (flag && num4 >= 0 && num4 / 100 != 6)
						{
							GrassNum++;
						}
						if (flag2)
						{
							int num5 = num4 % 100;
							if (num5 >= 41 && num5 <= 43)
							{
								CloudNum++;
							}
						}
						stringBuilder.Append(num4 + "\t|");
					}
					Debug.Log(stringBuilder.ToString());
				}
			}
		}

		public void FirstBtnClick()
		{
			ItemBtnClick(0);
		}

		public void SecondBtnClick()
		{
			ItemBtnClick(1);
		}

		public void ThirdBtnClick()
		{
			ItemBtnClick(2);
		}

		public void FirstBuyBtnClick()
		{
			base.gameObject.SetActive(false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.BuyInGameDlg, DropType.AreaBomb);
		}

		public void SecondBuyBtnClick()
		{
			base.gameObject.SetActive(false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.BuyInGameDlg, DropType.ColorBomb);
		}

		public void ThirdBuyBtnClick()
		{
			base.gameObject.SetActive(false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.BuyInGameDlg, DropType.DoubleBee);
		}

		public bool JudgeCanUse(int index)
		{
			switch (index)
			{
			case 0:
				if (UserDataManager.Instance.GetService().bombNumber > 0)
				{
					return true;
				}
				return false;
			case 1:
				if (UserDataManager.Instance.GetService().rainBowBallNumber > 0)
				{
					return true;
				}
				return false;
			default:
				if (UserDataManager.Instance.GetService().doubleBeesNumber > 0)
				{
					return true;
				}
				return false;
			}
		}

		public void ItemBtnClick(int index)
		{
			bool flag = false;
			if (selectIndex[index] != -1)
			{
				flag = true;
				selectIndex[index] = -1;
				switch (index)
				{
				case 0:
					GameConfig.CreateOneAreaBoom = false;
					break;
				case 1:
					GameConfig.CreateOneColorBoom = false;
					break;
				case 2:
					GameConfig.BeeLevel = 1;
					break;
				}
			}
			if (!flag && JudgeCanUse(index))
			{
				selectIndex[index] = 1;
				switch (index)
				{
				case 0:
					GameConfig.CreateOneAreaBoom = true;
					break;
				case 1:
					GameConfig.CreateOneColorBoom = true;
					break;
				case 2:
					GameConfig.BeeLevel = 2;
					break;
				}
			}
			else if (!JudgeCanUse(index))
			{
				base.gameObject.SetActive(false);
				DropType dropType = DropType.None;
				switch (index)
				{
				case 0:
					dropType = DropType.AreaBomb;
					break;
				case 1:
					dropType = DropType.ColorBomb;
					break;
				case 2:
					dropType = DropType.DoubleBee;
					break;
				}
				DialogManagerTemp.Instance.ShowDialog(DialogType.BuyInGameDlg, dropType);
			}
			UpdateSelectImage();
		}

		public void UpdateSelectImage()
		{
			for (int i = 0; i < selectIndex.Length; i++)
			{
				if (selectIndex[i] != -1)
				{
					effectArray[i].SetActive(true);
					selectImageArray[i].gameObject.SetActive(true);
					selectBgArray[i].gameObject.SetActive(true);
				}
				else
				{
					effectArray[i].SetActive(false);
					selectImageArray[i].gameObject.SetActive(false);
					selectBgArray[i].gameObject.SetActive(false);
				}
			}
		}

		public void ShowTarget()
		{
			int num = 0;
			int[] targetList = levelData.targetList;
			for (int i = 0; i < targetList.Length; i++)
			{
				if (targetList[i] != 0)
				{
					num++;
				}
			}
			int[] array = new int[num + 1];
			num = 0;
			for (int j = 0; j < targetList.Length; j++)
			{
				if (targetList[j] != 0)
				{
					num++;
					array[num] = j;
				}
			}
			if (array.Length < 3)
			{
				targetItem2.SetActive(false);
				int num2 = targetList[array[1]];
				firstTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(array[1] + 1);
				if (array[1] == 8)
				{
					firstTargetText.text = GrassNum.ToString();
				}
				else if (array[1] == 11)
				{
					firstTargetText.text = CloudNum.ToString();
				}
				else
				{
					firstTargetText.text = num2.ToString();
				}
			}
			else
			{
				targetItem2.SetActive(true);
				int num3 = targetList[array[1]];
				firstTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(array[1] + 1);
				if (array[1] == 8)
				{
					firstTargetText.text = GrassNum.ToString();
				}
				else if (array[1] == 11)
				{
					firstTargetText.text = CloudNum.ToString();
				}
				else
				{
					firstTargetText.text = num3.ToString();
				}
				num3 = targetList[array[2]];
				secondTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(array[2] + 1);
				if (array[2] == 8)
				{
					secondTargetText.text = GrassNum.ToString();
				}
				else if (array[2] == 11)
				{
					secondTargetText.text = CloudNum.ToString();
				}
				else
				{
					secondTargetText.text = num3.ToString();
				}
			}
			GrassNum = 0;
			CloudNum = 0;
		}

		public void Close(bool isAnim = true)
		{
			for (int i = 0; i < 3; i++)
			{
				lockTextArray[i].transform.parent.GetComponent<Animation>().Stop();
				lockTextArray[i].transform.parent.DOKill();
				lockTextArray[i].transform.parent.gameObject.SetActive(false);
			}
			if (SceneManager.GetActiveScene().name == "CastleScene")
			{
				RoleManager.Instance.ShowAllRoles();
			}
			if (showType == 1 || showType == 2)
			{
				UpdateManager.Instance.ClearLoopData();
				UpdateManager.Instance.PlayGame();
				Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
				SceneTransManager.Instance.ChangeSceneWithEffect(delegate
				{
					DialogManagerTemp.Instance.CloseAllDialogs();
					SceneTransManager.Instance.TransToSwitch(SceneType.CastleScene);
				});
				if (showType == 2)
				{
					GameLogic.Instance.StartAnalyticsIEnumerator(3);
				}
				showType = 0;
			}
			else
			{
				DialogManagerTemp.Instance.CloseDialog(DialogType.EnterGameDlg);
			}
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		private IEnumerator TransScene(SceneType type)
		{
			yield return new WaitForSeconds(3.5f);
			DialogManagerTemp.Instance.CloseAllDialogs();
			SceneTransManager.Instance.TransToSwitch(type);
		}
	}
}
