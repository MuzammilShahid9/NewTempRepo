using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.AliceMatch3.Core.UI;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using PlayInfinity.Pandora.Core.UI;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

[Hotfix(HotfixFlag.Stateless)]
public class CastleSceneUIManager : BaseUIManager
{
	private static CastleSceneUIManager instance;

	public Text scrollNumText;

	public Text coinNumText;

	public Text levelNumText;

	public Text lifeText;

	public Text lifeRecoveTimeText;

	public SelectItemUIManager SelectItemUI;

	public GameObject flyScrollStartPos;

	public GameObject flyCoinEndPos;

	public GameObject flyImage;

	public GameObject plotBubble;

	public GameObject bubble;

	public GameObject taskArrow;

	public GameObject transArrow;

	public GameObject stageRewardMidPosition;

	public GameObject upStartPosition;

	public GameObject upEndPosition;

	public GameObject downStartPosition;

	public GameObject downEndPosition;

	public bool isPlotMaskShow;

	public bool isInitBtnPositionInfo;

	public Button taskBtn;

	public Button enterGameBtn;

	public Button lifeBtn;

	public Button glodBtn;

	public Button scrollBtn;

	public Button settingBtn;

	public Button inboxBtn;

	public Button skipButton;

	public Image topMask;

	public Image lifeImage;

	public Image goldImage;

	public Image bottomMask;

	public Image unlimitLifeImage;

	public Image stageRewardImage;

	public GameObject comingTip;

	public Button SaleBtn;

	public Button BankBtn;

	public GameObject LeftGrid;

	public Text SaleTimeText;

	private Vector3 scrollStartPos;

	private List<GameObject> bubbleList = new List<GameObject>();

	private Vector3[] btnStartPositionArray = new Vector3[9];

	private Vector3[] btnEndPositionArray = new Vector3[9];

	private Vector3 taskArrowStartPosition;

	private Vector3 transArrowStartPosition;

	private Tweener taskArrowTweener;

	private Tweener transArrowTweener;

	private ItemAnim currSelectItem;

	public GameObject Mask;

	public Text BankNum;

	private bool isChecking;

	public float BankCollectDelay;

	public bool isMoreThan7k;

	public bool isLastOpen;

	private bool isNoMap;

	public GameObject BankObjPrefab;

	public GameObject tailEffect;

	public Transform BankPos;

	public Transform EnterGamePos;

	public Text BankTime;

	public Text BankTipText;

	public GameObject BankTip;

	public Text ComboTime;

	public Button ComboBtn;

	private Sequence seq;

	public static CastleSceneUIManager Instance
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
		taskArrowStartPosition = taskArrow.transform.localPosition;
		transArrowStartPosition = transArrow.transform.localPosition;
		scrollStartPos = flyScrollStartPos.transform.Find("Image").transform.localPosition;
		if (GameObject.Find("UI") != null)
		{
			base.gameObject.transform.SetParent(GameObject.Find("UI").transform);
		}
		base.gameObject.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
		ShopConfig.Load();
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(32u, CloseAllDialogs);
		BtnDisable();
	}

	private void CloseAllDialogs(uint iMessageType, object arg)
	{
		if (isChecking)
		{
			return;
		}
		float currentTime = 0f;
		float waitTimeToShowAllBtn = 0.2f;
		UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (!DialogManagerTemp.Instance.IsDialogShowing() && SceneManager.GetActiveScene().name == "CastleScene" && PlotManager.Instance.isPlotFinish)
			{
				if (currentTime > waitTimeToShowAllBtn)
				{
					ShowAllBtn();
					isChecking = false;
					return true;
				}
				currentTime += duration;
				return false;
			}
			isChecking = false;
			return true;
		}));
	}

	protected override void Start()
	{
		CinemaDirector.Instance.gameObject.SetActive(false);
		skipButton.onClick.AddListener(delegate
		{
			HideMask();
			PlotManager.Instance.SkipNewStep();
		});
		ShowArrow();
		if (UserDataManager.Instance.GetService().tutorialProgress <= 2 && (!TestConfig.active || !TestConfig.cinemaMode))
		{
			ForceHideUpButton();
			ForceHideDownButton();
		}
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(21u, PressEsc);
		if (!isInitBtnPositionInfo)
		{
			InitBtnPositionInfo();
		}
	}

	public void BtnDisable()
	{
		lifeBtn.enabled = false;
		glodBtn.enabled = false;
		scrollBtn.enabled = false;
		settingBtn.enabled = false;
		inboxBtn.enabled = false;
		taskBtn.enabled = false;
		enterGameBtn.enabled = false;
		SaleBtn.enabled = false;
		BankBtn.enabled = false;
		ComboBtn.enabled = false;
	}

	public void MaskCastleUI()
	{
		GlobalVariables.IsMaskCastleUI = true;
		Mask.SetActive(true);
	}

	public void CancelMaskCastleUI()
	{
		GlobalVariables.IsMaskCastleUI = false;
		Mask.SetActive(false);
	}

	public void InitBtnPositionInfo()
	{
		btnStartPositionArray[0] = lifeBtn.transform.position;
		btnStartPositionArray[1] = glodBtn.transform.position;
		btnStartPositionArray[2] = scrollBtn.transform.position;
		btnStartPositionArray[3] = settingBtn.transform.position;
		btnStartPositionArray[4] = inboxBtn.transform.position;
		btnStartPositionArray[5] = taskBtn.transform.position;
		btnStartPositionArray[6] = enterGameBtn.transform.position;
		btnStartPositionArray[7] = LeftGrid.transform.position;
		btnStartPositionArray[8] = BankBtn.transform.position;
		btnEndPositionArray[0] = lifeBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[1] = glodBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[2] = scrollBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[3] = settingBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[4] = inboxBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[5] = taskBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[6] = enterGameBtn.transform.Find("EndPos").transform.position;
		btnEndPositionArray[7] = LeftGrid.transform.Find("EndPos").transform.position;
		btnEndPositionArray[8] = BankBtn.transform.Find("EndPos").transform.position;
		BtnEnable();
	}

	public void BtnEnable()
	{
		lifeBtn.enabled = true;
		glodBtn.enabled = true;
		scrollBtn.enabled = true;
		settingBtn.enabled = true;
		inboxBtn.enabled = true;
		taskBtn.enabled = true;
		enterGameBtn.enabled = true;
		SaleBtn.enabled = true;
		BankBtn.enabled = true;
		ComboBtn.enabled = true;
	}

	public void ShowArrow()
	{
		int tutorialProgress = UserDataManager.Instance.GetService().tutorialProgress;
		int level = UserDataManager.Instance.GetService().level;
		int scrollNum = UserDataManager.Instance.GetService().scrollNum;
		if (tutorialProgress >= 6 && level <= 5 && scrollNum > 0)
		{
			taskArrow.SetActive(true);
			transArrow.SetActive(false);
			transArrowTweener.Pause();
		}
		else if (tutorialProgress >= 6 && level <= 5 && scrollNum <= 0)
		{
			taskArrow.SetActive(false);
			transArrow.SetActive(true);
			transArrowTweener.Play();
		}
		else
		{
			HideArrow();
		}
	}

	public void HideArrow()
	{
		taskArrow.SetActive(false);
		transArrow.SetActive(false);
		transArrowTweener.Pause();
	}

	public void ShowPlotBubble()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(plotBubble, base.transform);
		gameObject.transform.SetAsFirstSibling();
		bubbleList.Add(gameObject);
		gameObject.SetActive(true);
	}

	public void ShowBubble(Role target, string content)
	{
		GameObject obj = UnityEngine.Object.Instantiate(bubble, base.transform);
		obj.SetActive(true);
		BubbleManager component = obj.GetComponent<BubbleManager>();
		component.Enter(target, content);
		target.SetBubble(component);
	}

	public void ClearBubble()
	{
		for (int i = 0; i < bubbleList.Count; i++)
		{
			bubbleList[i].SetActive(false);
			UnityEngine.Object.Destroy(bubbleList[i]);
		}
		bubbleList.Clear();
	}

	public override void ShowUI()
	{
		base.ShowUI();
		if (!isInitBtnPositionInfo)
		{
			InitBtnPositionInfo();
		}
		DebugUtils.Log(DebugType.Other, "CastleScene ShowUI");
		UpdateBtnText();
		UpdateBtnSprite();
		BankNum.text = ((BankDlg.Instance.CurrentBankNum >= 12000) ? LanguageConfig.GetString("UI_Full") : string.Concat(BankDlg.Instance.CurrentBankNum));
		DialogManagerTemp.Instance.MaskAllDlg();
		MaskCastleUI();
		BankCollectDelay = 0f;
		CheckBankActive();
		if (UserDataManager.Instance.GetIsBanking())
		{
			if (CheckBankCollectCoin(out BankCollectDelay, out isMoreThan7k))
			{
				Timer.Schedule(this, BankCollectDelay, delegate
				{
					BankCollectDelay = 0f;
					bool flag7 = CheckShowCombo();
					bool num3 = CheckShowBank();
					bool flag8 = CheckShowSaleDlg();
					bool flag9 = CastleSceneManager.Instance.ShowDailyBonus();
					bool flag10 = ShowUnLockDrop();
					if (!num3 && !flag8 && !flag9 && !flag10 && !flag7)
					{
						CancelMaskCastleUI();
						DialogManagerTemp.Instance.CancelMaskAllDlg();
					}
				});
			}
			else
			{
				BankCollectDelay = 0f;
				bool flag = CheckShowCombo();
				bool num = CheckShowSaleDlg();
				bool flag2 = CastleSceneManager.Instance.ShowDailyBonus();
				bool flag3 = ShowUnLockDrop();
				if (!num && !flag2 && !flag3 && !flag)
				{
					CancelMaskCastleUI();
					DialogManagerTemp.Instance.CancelMaskAllDlg();
				}
			}
		}
		else
		{
			BankCollectDelay = 0f;
			BankBtn.gameObject.SetActive(false);
			bool flag4 = CheckShowCombo();
			bool num2 = CheckShowSaleDlg();
			bool flag5 = CastleSceneManager.Instance.ShowDailyBonus();
			bool flag6 = ShowUnLockDrop();
			if (!num2 && !flag5 && !flag6 && !flag4)
			{
				CancelMaskCastleUI();
				DialogManagerTemp.Instance.CancelMaskAllDlg();
			}
		}
		base.gameObject.SetActive(true);
		ShowLevelDialog();
	}

	public bool CheckShowBank()
	{
		if (isMoreThan7k)
		{
			DialogManagerTemp.Instance.OpenBankDlg(0.1f, false);
			return true;
		}
		return false;
	}

	public bool ShowUnLockDrop()
	{
		int num = 0;
		bool flag = false;
		int[] itemUnlockLevel = GeneralConfig.ItemUnlockLevel;
		foreach (int num2 in itemUnlockLevel)
		{
			num++;
			flag = flag || UserDataManager.Instance.UnlockDrop((DropType)num, UserDataManager.Instance.GetProgress() >= num2);
		}
		return flag;
	}

	public void ShowMagicPlot()
	{
		if (!UserDataManager.Instance.GetService().ShowMagicBook && (float)UserDataManager.Instance.GetService().level == GeneralConfig.ShowMagicLevel)
		{
			StartMagicPlot();
		}
	}

	public void StartMagicPlot()
	{
	}

	public bool ShowLevelDialog()
	{
		if (GlobalVariables.LevelFirstPass && UserDataManager.Instance.GetService().rateVersion != Application.version)
		{
			bool flag = false;
			for (int i = 0; i < GeneralConfig.ShowRateLevel.Length; i++)
			{
				if (UserDataManager.Instance.GetService().level == GeneralConfig.ShowRateLevel[i])
				{
					flag = true;
				}
			}
			if (flag)
			{
				ForceHideUpButton();
				ForceHideDownButton();
				DialogManagerTemp.Instance.ShowDialog(DialogType.HavingFunDlg);
			}
			GlobalVariables.LevelFirstPass = false;
			return flag;
		}
		return false;
	}

	public void ForceHideUpButton()
	{
		lifeBtn.gameObject.SetActive(false);
		glodBtn.gameObject.SetActive(false);
		scrollBtn.gameObject.SetActive(false);
		settingBtn.gameObject.SetActive(false);
	}

	public void ForceHideDownButton()
	{
		taskBtn.gameObject.SetActive(false);
		enterGameBtn.gameObject.SetActive(false);
		inboxBtn.gameObject.SetActive(false);
		LeftGrid.gameObject.SetActive(false);
		BankBtn.gameObject.SetActive(false);
	}

	public void ChangeUpButtonFrontSortingLayer()
	{
		lifeBtn.enabled = false;
		glodBtn.enabled = false;
		scrollBtn.enabled = false;
		settingBtn.enabled = false;
	}

	public void ChangeUpButtonBehindSortingLayer()
	{
		lifeBtn.enabled = true;
		glodBtn.enabled = true;
		scrollBtn.enabled = true;
		settingBtn.enabled = true;
	}

	public void HideAllBtn()
	{
		HideBtn(lifeBtn.transform);
		HideBtn(glodBtn.transform);
		HideBtn(scrollBtn.transform);
		HideBtn(settingBtn.transform);
		HideBtn(inboxBtn.transform);
		HideBtn(LeftGrid.transform);
		HideBtn(BankBtn.transform);
		HideTaskAndEnterGameBtn();
	}

	public void ShowAllBtn()
	{
		if (lifeBtn != null)
		{
			ShowUpButton();
			ShowTaskAndEnterGameBtn();
			BtnEnable();
		}
	}

	public void ShowMask()
	{
		topMask.gameObject.SetActive(true);
		bottomMask.gameObject.SetActive(true);
		skipButton.gameObject.SetActive(false);
		topMask.transform.DOLocalMoveY(upEndPosition.transform.localPosition.y, 0.2f);
		bottomMask.transform.DOLocalMoveY(downEndPosition.transform.localPosition.y, 0.2f);
		isPlotMaskShow = true;
	}

	public void ShowSkipButton()
	{
		skipButton.gameObject.SetActive(true);
	}

	public void ShowUpButton()
	{
		if (lifeBtn != null)
		{
			ShowBtn(lifeBtn.transform);
		}
		if (glodBtn != null)
		{
			ShowBtn(glodBtn.transform);
		}
		if (scrollBtn != null)
		{
			ShowBtn(scrollBtn.transform);
		}
		if (settingBtn != null)
		{
			ShowBtn(settingBtn.transform);
		}
	}

	public void HideMask()
	{
		topMask.transform.DOLocalMoveY(upStartPosition.transform.localPosition.y, 0.2f);
		bottomMask.transform.DOLocalMoveY(downStartPosition.transform.localPosition.y, 0.2f);
		isPlotMaskShow = false;
	}

	public void HideTaskAndEnterGameBtn()
	{
		if (taskBtn != null)
		{
			HideBtn(taskBtn.transform);
		}
		if (enterGameBtn != null)
		{
			HideBtn(enterGameBtn.transform);
		}
		if (inboxBtn != null)
		{
			HideBtn(inboxBtn.transform);
		}
		if (LeftGrid != null)
		{
			HideBtn(LeftGrid.transform);
		}
		if (BankBtn != null)
		{
			HideBtn(BankBtn.transform);
		}
	}

	public void ShowTaskAndEnterGameBtn()
	{
		if (taskBtn != null)
		{
			ShowBtn(taskBtn.transform);
		}
		if (enterGameBtn != null)
		{
			ShowBtn(enterGameBtn.transform);
		}
		if (inboxBtn != null)
		{
			ShowBtn(inboxBtn.transform);
		}
		if (LeftGrid != null)
		{
			ShowBtn(LeftGrid.transform);
		}
		if (BankBtn != null)
		{
			ShowBtn(BankBtn.transform);
		}
	}

	public Vector3 GetBtnStartPosition(Transform tempTransform)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (tempTransform == lifeBtn.transform)
		{
			return btnStartPositionArray[0];
		}
		if (tempTransform == glodBtn.transform)
		{
			return btnStartPositionArray[1];
		}
		if (tempTransform == scrollBtn.transform)
		{
			return btnStartPositionArray[2];
		}
		if (tempTransform == settingBtn.transform)
		{
			return btnStartPositionArray[3];
		}
		if (tempTransform == inboxBtn.transform)
		{
			return btnStartPositionArray[4];
		}
		if (tempTransform == taskBtn.transform)
		{
			return btnStartPositionArray[5];
		}
		if (tempTransform == enterGameBtn.transform)
		{
			return btnStartPositionArray[6];
		}
		if (tempTransform == LeftGrid.transform)
		{
			return btnStartPositionArray[7];
		}
		if (tempTransform == BankBtn.transform)
		{
			return btnStartPositionArray[8];
		}
		return result;
	}

	public Vector3 GetBtnEndPosition(Transform tempTransform)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (tempTransform == lifeBtn.transform)
		{
			return btnEndPositionArray[0];
		}
		if (tempTransform == glodBtn.transform)
		{
			return btnEndPositionArray[1];
		}
		if (tempTransform == scrollBtn.transform)
		{
			return btnEndPositionArray[2];
		}
		if (tempTransform == settingBtn.transform)
		{
			return btnEndPositionArray[3];
		}
		if (tempTransform == inboxBtn.transform)
		{
			return btnEndPositionArray[4];
		}
		if (tempTransform == taskBtn.transform)
		{
			return btnEndPositionArray[5];
		}
		if (tempTransform == enterGameBtn.transform)
		{
			return btnEndPositionArray[6];
		}
		if (tempTransform == LeftGrid.transform)
		{
			return btnEndPositionArray[7];
		}
		if (tempTransform == BankBtn.transform)
		{
			return btnEndPositionArray[8];
		}
		return result;
	}

	public void HideBtn(Transform tempTransform)
	{
		if (!(tempTransform.position != GetBtnEndPosition(tempTransform)))
		{
			return;
		}
		Button[] componentsInChildren = tempTransform.GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		tempTransform.position = GetBtnStartPosition(tempTransform);
		Vector3[] path = new Vector3[3]
		{
			GetBtnStartPosition(tempTransform),
			GetBtnStartPosition(tempTransform) + (GetBtnStartPosition(tempTransform) - GetBtnEndPosition(tempTransform)) * 0.02f,
			GetBtnEndPosition(tempTransform)
		};
		Sequence sequence = DOTween.Sequence();
		sequence.Append(tempTransform.DOPath(path, 0.3f, PathType.CatmullRom));
		sequence.OnComplete(delegate
		{
			Button[] componentsInChildren2 = tempTransform.GetComponentsInChildren<Button>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].enabled = true;
			}
		});
	}

	public void ShowBtn(Transform tempTransform)
	{
		tempTransform.gameObject.SetActive(true);
		if (!(tempTransform.position != GetBtnStartPosition(tempTransform)))
		{
			return;
		}
		Button[] componentsInChildren = tempTransform.GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		tempTransform.position = GetBtnEndPosition(tempTransform);
		Vector3[] path = new Vector3[3]
		{
			GetBtnEndPosition(tempTransform),
			GetBtnEndPosition(tempTransform) + (GetBtnStartPosition(tempTransform) - GetBtnEndPosition(tempTransform)) * 1.02f,
			GetBtnStartPosition(tempTransform)
		};
		Sequence sequence = DOTween.Sequence();
		sequence.Append(tempTransform.DOPath(path, 0.3f, PathType.CatmullRom));
		sequence.OnComplete(delegate
		{
			Button[] componentsInChildren2 = tempTransform.GetComponentsInChildren<Button>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].enabled = true;
			}
		});
	}

	public void StageRewardFly()
	{
		Vector3[] array = new Vector3[3];
		array[0] = TaskPanelManager.Instance.rewardBtn.transform.position;
		array[1] = new Vector3(stageRewardMidPosition.transform.position.x, stageRewardMidPosition.transform.position.y, array[0].z);
		array[2] = StageRewardDlg.Instance.boxImage.transform.position + new Vector3(-0.43f, 0.04f, 0f);
		stageRewardImage.gameObject.SetActive(true);
		stageRewardImage.transform.position = array[0];
		DialogManagerTemp.Instance.ShowDialog(DialogType.StageRewardDlg);
		stageRewardImage.transform.DOPath(array, 1.5f, PathType.CatmullRom);
		float x = 3.6f;
		float y = 3.6f;
		stageRewardImage.transform.DOScale(new Vector3(x, y, 1f), 1.5f);
		HideAllBtn();
	}

	public void LifeImageScale()
	{
		Sequence s = DOTween.Sequence();
		s.Append(lifeImage.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
		s.Append(lifeImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
	}

	public void EnterGameBtnScale()
	{
		Sequence s = DOTween.Sequence();
		s.Append(enterGameBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
		s.Append(enterGameBtn.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
	}

	public void GoldImageScale()
	{
		Sequence s = DOTween.Sequence();
		s.Append(goldImage.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
		s.Append(goldImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
		UpdateBtnText();
	}

	public void BankImageScale()
	{
		Sequence s = DOTween.Sequence();
		s.Append(BankBtn.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
		s.Append(BankBtn.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
	}

	public void HideStageRewardImage()
	{
		stageRewardImage.transform.localScale = new Vector3(1f, 1f, 1f);
		stageRewardImage.gameObject.SetActive(false);
	}

	public void DoTaskBtnClick(int plotID, Vector3 targetPosition, int costScrollNum)
	{
		scrollNumText.text = (UserDataManager.Instance.GetScrollNum() - costScrollNum).ToString();
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
	}

	[LuaCallCSharp(GenFlag.No)]
	public void InboxBtnClick()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.InboxDlg);
	}

	public void UpdateBtnText()
	{
		if (scrollNumText != null)
		{
			scrollNumText.text = UserDataManager.Instance.GetScrollNum().ToString();
		}
		if (coinNumText != null)
		{
			coinNumText.text = UserDataManager.Instance.GetCoin().ToString();
		}
		if (levelNumText != null)
		{
			levelNumText.text = UserDataManager.Instance.GetProgress().ToString();
		}
		if (lifeText != null)
		{
			lifeText.text = UserDataManager.Instance.GetService().life.ToString();
		}
	}

	public void UpdateBtnSprite()
	{
		int progress = UserDataManager.Instance.GetProgress();
		isNoMap = false;
		JSONNode jSONNode = HotFixScript.Instance.LoadLevelData(progress);
		if (jSONNode == null)
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Levels/Level_" + progress);
			if (textAsset == null)
			{
				isNoMap = true;
				return;
			}
			jSONNode = JSONNode.Parse(textAsset.text);
		}
		switch (int.Parse(jSONNode["hard"]))
		{
		case 0:
			enterGameBtn.GetComponent<Image>().sprite = GeneralConfig.UIPictures[UIType.Easy_buttonRuZhan];
			break;
		case 1:
			enterGameBtn.GetComponent<Image>().sprite = GeneralConfig.UIPictures[UIType.Normal_buttonRuZhan];
			break;
		case 2:
			enterGameBtn.GetComponent<Image>().sprite = GeneralConfig.UIPictures[UIType.Hard_buttonRuZhan];
			break;
		}
		UserDataManager.Instance.Save();
	}

	private bool CheckBankCollectCoin(out float delya, out bool isMoreThan7k)
	{
		isMoreThan7k = false;
		if (BankDlg.Instance.isInit)
		{
			BankDlg.Instance.isInit = false;
			BankNum.text = ((UserDataManager.Instance.GetBankNum() >= 12000) ? LanguageConfig.GetString("UI_Full") : string.Concat(UserDataManager.Instance.GetBankNum()));
			delya = 0f;
			return false;
		}
		if (BankDlg.Instance.CurrentBankNum < UserDataManager.Instance.GetBankNum())
		{
			int preBankNum = BankDlg.Instance.CurrentBankNum;
			int num = UserDataManager.Instance.GetBankNum() - preBankNum;
			BankDlg.Instance.RefreshCoin();
			BankNum.text = ((preBankNum >= 12000) ? LanguageConfig.GetString("UI_Full") : string.Concat(preBankNum));
			if (UserDataManager.Instance.GetBankNum() >= 7000 && preBankNum < 7000)
			{
				isMoreThan7k = true;
			}
			if (UserDataManager.Instance.GetBankNum() >= 12000 && preBankNum < 12000)
			{
				isMoreThan7k = true;
			}
			int count = (int)Mathf.Ceil((float)num / 100000f);
			int lastNum = num % 1000000;
			if (num > 0)
			{
				float time = 1f;
				int currCount = 1;
				float delayStart = 0f;
				UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (delayStart > 1.3f && time > 0.1f)
					{
						time = 0f;
						if (currCount > count)
						{
							return true;
						}
						int num2 = 1000000;
						if (currCount == count && lastNum > 0)
						{
							num2 = lastNum;
						}
						currCount++;
						GameObject go = UnityEngine.Object.Instantiate(BankObjPrefab, base.transform);
						go.transform.localScale = Vector3.one * 1.5f;
						UnityEngine.Object.Instantiate(tailEffect, go.transform);
						go.transform.position = EnterGamePos.position;
						ShortcutExtensions.DOPath(path: new Vector3[3]
						{
							EnterGamePos.position,
							(EnterGamePos.position + BankPos.position) * 0.5f + new Vector3(-1f, 0f, 0f),
							BankPos.position
						}, target: go.transform, duration: 0.25f, pathType: PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(delegate
						{
							if (Instance != null)
							{
								BankTipText.text = string.Concat(num2);
								BankTip.SetActive(false);
								BankTip.SetActive(true);
								Instance.BankImageScale();
								preBankNum += num2;
								BankNum.text = ((preBankNum >= 12000) ? LanguageConfig.GetString("UI_Full") : string.Concat(preBankNum));
							}
							UnityEngine.Object.Destroy(go);
						});
					}
					delayStart += duration;
					time += duration;
					return false;
				}));
			}
			delya = 0.12f * (float)count + 0.6f + 1.3f;
			return true;
		}
		delya = 0f;
		return false;
	}

	private bool CheckBankActive()
	{
		if (DateTime.Now.Ticks / 10000000 < UserDataManager.Instance.GetService().BankSaleStartTM)
		{
			DebugUtils.LogError(DebugType.UI, "User Changed System time !");
			return false;
		}
		DebugUtils.Log(DebugType.UI, UserDataManager.Instance.GetProgress() + "   ***   " + Singleton<PlayGameData>.Instance().gameConfig.BankSaleActiveLevel);
		if (UserDataManager.Instance.GetService().BankSaleShowNum == 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.BankSaleActiveLevel)
		{
			BankBtn.gameObject.SetActive(true);
			UserDataManager.Instance.SetBankNum(0);
			BankDlg.Instance.RefreshCoin();
			UserDataManager.Instance.GetService().BankSaleShowNum++;
			UserDataManager.Instance.GetService().BankSaleStartTM = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.SetIsBanking(true);
			UserDataManager.Instance.GetService().BankSaleTM = Singleton<PlayGameData>.Instance().gameConfig.BankSaleTime;
			DialogManagerTemp.Instance.OpenBankDlg(1f, false);
			return true;
		}
		if (UserDataManager.Instance.GetService().BankSaleShowNum > 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.BankSaleActiveLevel)
		{
			long num = DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().BankSaleStartTM;
			DebugUtils.Log(DebugType.UI, DateTime.Now.Ticks / 10000000 + "   |Time|   " + UserDataManager.Instance.GetService().BankSaleStartTM + "    | -----|    " + Singleton<PlayGameData>.Instance().gameConfig.BankSaleCDTime);
			if (num > Singleton<PlayGameData>.Instance().gameConfig.BankSaleCDTime)
			{
				DebugUtils.Log(DebugType.UI, num + "   &&&   " + Singleton<PlayGameData>.Instance().gameConfig.BankSaleCDTime);
				DebugUtils.Log(DebugType.UI, "Bank duringDaysTwoSale is bigger then " + Singleton<PlayGameData>.Instance().gameConfig.BankSaleCDTime + "s   " + num);
				BankBtn.gameObject.SetActive(true);
				BankDlg.Instance.RefreshCoin();
				UserDataManager.Instance.GetService().BankSaleShowNum++;
				UserDataManager.Instance.GetService().BankSaleStartTM = DateTime.Now.Ticks / 10000000;
				UserDataManager.Instance.SetIsBanking(true);
				UserDataManager.Instance.GetService().BankSaleTM = Singleton<PlayGameData>.Instance().gameConfig.BankSaleTime;
				DialogManagerTemp.Instance.OpenBankDlg(1f, false);
				return true;
			}
			if (UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.BankSaleActiveLevel && !UserDataManager.Instance.GetIsBanking() && !UserDataManager.Instance.GetService().isShowBankLastTime)
			{
				UserDataManager.Instance.GetService().isShowBankLastTime = true;
				DialogManagerTemp.Instance.OpenBankDlg(1f, false);
			}
		}
		else if (UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.BankSaleActiveLevel && !UserDataManager.Instance.GetIsBanking() && !UserDataManager.Instance.GetService().isShowBankLastTime)
		{
			UserDataManager.Instance.GetService().isShowBankLastTime = true;
			DialogManagerTemp.Instance.OpenBankDlg(1f, false);
		}
		return false;
	}

	private bool CheckShowCombo()
	{
		if (DateTime.Now.Ticks / 10000000 < UserDataManager.Instance.GetService().ComboStartTM)
		{
			DebugUtils.LogError(DebugType.UI, "User Changed System time !");
			return false;
		}
		DebugUtils.Log(DebugType.UI, UserDataManager.Instance.GetService().ComboShowNum + "   ***   " + UserDataManager.Instance.GetProgress() + "   ***   " + Singleton<PlayGameData>.Instance().gameConfig.ComboActiveLevel);
		if (UserDataManager.Instance.GetService().ComboShowNum == 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.ComboActiveLevel)
		{
			ComboBtn.gameObject.SetActive(true);
			GlobalVariables.ComboNum = 0;
			UserDataManager.Instance.GetService().ComboShowNum++;
			UserDataManager.Instance.GetService().ComboStartTM = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.SetIsComboing(true);
			UserDataManager.Instance.GetService().ComboTM = Singleton<PlayGameData>.Instance().gameConfig.ComboTime;
			DialogManagerTemp.Instance.OpenComboDlg(1f, true);
			return true;
		}
		if (UserDataManager.Instance.GetService().ComboShowNum > 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.ComboActiveLevel)
		{
			long num = DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().ComboStartTM;
			DebugUtils.Log(DebugType.UI, DateTime.Now.Ticks / 10000000 + "   |Time|   " + UserDataManager.Instance.GetService().ComboStartTM + "    | -----|    " + Singleton<PlayGameData>.Instance().gameConfig.ComboCDTime);
			if (num > Singleton<PlayGameData>.Instance().gameConfig.ComboCDTime)
			{
				DebugUtils.Log(DebugType.UI, num + "   &&&   " + Singleton<PlayGameData>.Instance().gameConfig.ComboCDTime);
				ComboBtn.gameObject.SetActive(true);
				GlobalVariables.ComboNum = 0;
				UserDataManager.Instance.GetService().ComboShowNum++;
				UserDataManager.Instance.GetService().ComboStartTM = DateTime.Now.Ticks / 10000000;
				UserDataManager.Instance.SetIsComboing(true);
				UserDataManager.Instance.GetService().ComboTM = Singleton<PlayGameData>.Instance().gameConfig.ComboTime;
				DialogManagerTemp.Instance.OpenComboDlg(1f, true);
				return true;
			}
			return false;
		}
		return false;
	}

	private bool CheckShowSaleDlg()
	{
		if (DateTime.Now.Ticks / 10000000 < UserDataManager.Instance.GetService().SaleShowTime_L)
		{
			DebugUtils.LogError(DebugType.UI, "User Changed System time !");
			return false;
		}
		DebugUtils.Log(DebugType.UI, UserDataManager.Instance.GetService().SaleShowNum + "   ***   " + UserDataManager.Instance.GetProgress() + "   ***   " + Singleton<PlayGameData>.Instance().gameConfig.SaleActiveLevel);
		if (UserDataManager.Instance.GetService().SaleShowNum == 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.SaleActiveLevel)
		{
			if (UserDataManager.Instance.GetService().moneySpend <= 20)
			{
				DebugUtils.Log(DebugType.UI, "first Open Sale By normal&free user !");
				DialogManagerTemp.Instance.OpenSaleDlg(1f, true);
				UserDataManager.Instance.GetService().isFreshMan = false;
				return true;
			}
			long num = DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().LastBuyTime;
			DebugUtils.Log(DebugType.UI, num + "   &&&   " + Singleton<PlayGameData>.Instance().gameConfig.HighUserCheckTime);
			if (num >= Singleton<PlayGameData>.Instance().gameConfig.HighUserCheckTime)
			{
				DebugUtils.Log(DebugType.UI, "first Open Sale By high user !");
				DialogManagerTemp.Instance.OpenSaleDlg(1f, true);
				UserDataManager.Instance.GetService().isFreshMan = false;
				return true;
			}
			return false;
		}
		if (UserDataManager.Instance.GetService().SaleShowNum > 0 && UserDataManager.Instance.GetProgress() >= Singleton<PlayGameData>.Instance().gameConfig.SaleActiveLevel)
		{
			long num2 = DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().SaleShowTime_L;
			DebugUtils.Log(DebugType.UI, DateTime.Now.Ticks / 10000000 + "   |Time|   " + UserDataManager.Instance.GetService().SaleShowTime_L + "    | -----|    " + Singleton<PlayGameData>.Instance().gameConfig.SaleCDTime);
			if (num2 > Singleton<PlayGameData>.Instance().gameConfig.SaleCDTime)
			{
				DebugUtils.Log(DebugType.UI, num2 + "   &&&   " + Singleton<PlayGameData>.Instance().gameConfig.SaleCDTime);
				DebugUtils.Log(DebugType.UI, "duringDaysTwoSale is bigger then " + Singleton<PlayGameData>.Instance().gameConfig.SaleCDTime + "s   " + num2);
				if (UserDataManager.Instance.GetService().moneySpend > 20)
				{
					DebugUtils.Log(DebugType.UI, "check high user duringDaysTwoBug is bigger then " + Singleton<PlayGameData>.Instance().gameConfig.HighUserCheckTime + "s ");
					long num3 = DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().LastBuyTime;
					DebugUtils.Log(DebugType.UI, "high user duringDaysTwoBug is " + num3 + "s ");
					if (num3 >= Singleton<PlayGameData>.Instance().gameConfig.HighUserCheckTime)
					{
						DebugUtils.Log(DebugType.UI, "Open Sale By high user !");
						DebugUtils.Log(DebugType.UI, "high user duringDaysTwoBug is bigger then " + Singleton<PlayGameData>.Instance().gameConfig.HighUserCheckTime + "s      " + num3);
						DialogManagerTemp.Instance.OpenSaleDlg(1f, true);
						return true;
					}
					return false;
				}
				DebugUtils.Log(DebugType.UI, "Open Sale By normal&free user !");
				DialogManagerTemp.Instance.OpenSaleDlg(1f, true);
				return true;
			}
			return false;
		}
		return false;
	}

	public bool GetSelectItemUIStatu()
	{
		if (SelectItemUI != null)
		{
			return SelectItemUI.gameObject.activeInHierarchy;
		}
		return false;
	}

	public void SelectItemCancelBtnClick()
	{
		SelectItemUI.gameObject.SetActive(false);
		scrollNumText.text = UserDataManager.Instance.GetScrollNum().ToString();
		ShowAllBtn();
	}

	private IEnumerator FlyIcon(int plotID, Vector3 targetPosition, int costScrollNum)
	{
		GameObject[] flyScroll = new GameObject[costScrollNum];
		for (int i = 0; i < costScrollNum; i++)
		{
			GameObject gameObject = (flyScroll[i] = UnityEngine.Object.Instantiate(flyImage.gameObject));
			gameObject.transform.SetParent(flyScrollStartPos.gameObject.transform, true);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			float num = UnityEngine.Random.Range(-2f, 2f);
			float num2 = UnityEngine.Random.Range(-2f, 2f);
			Vector3 localPosition = new Vector3(scrollStartPos.x + num, scrollStartPos.y + num2, 0f);
			gameObject.transform.localPosition = localPosition;
			targetPosition += base.transform.position;
			targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0f);
			DebugUtils.Log(DebugType.Other, targetPosition);
			gameObject.SetActive(true);
			gameObject.transform.DOMove(targetPosition, 1f).SetEase(Ease.InBack);
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds(1f);
		for (int j = 0; j < costScrollNum; j++)
		{
			flyScroll[j].SetActive(false);
			UnityEngine.Object.Destroy(flyScroll[j]);
		}
		yield return new WaitForSeconds(1f);
		TaskAndRoleDlg.Instance.HideSelf();
		PlotManager.Instance.StartPlot(plotID);
	}

	public void ShowChangeItemUI(Transform targetTransform, bool underBuildManager = false)
	{
		ItemAnim component = targetTransform.GetComponent<ItemAnim>();
		if (currSelectItem != null)
		{
			currSelectItem.ShowImage(UserDataManager.Instance.GetItemUnlockInfo(currSelectItem.roomID, currSelectItem.itemID));
		}
		currSelectItem = component;
		HideTaskAndEnterGameBtn();
		BtnDisable();
		StartCoroutine(DelayShowSelectItemUI(component));
	}

	private IEnumerator DelayShowSelectItemUI(ItemAnim selectItem)
	{
		int index = UnityEngine.Random.Range(0, 3);
		if (UserDataManager.Instance.GetItemUnlockInfo(currSelectItem.roomID, currSelectItem.itemID) != -1)
		{
			index = UserDataManager.Instance.GetItemUnlockInfo(currSelectItem.roomID, currSelectItem.itemID);
		}
		selectItem.SelectShowImage(index, true);
		yield return new WaitForSeconds(0.2f);
		SelectItemUI.Show(selectItem);
	}

	public void ShowChangeItemUI(Item selectItem, bool underBuildManager = false)
	{
		SelectItemUI.Show(selectItem.GetComponent<ItemSelect>());
		HideTaskAndEnterGameBtn();
	}

	public void TaskBtnClick()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.TaskAndRoleDlg, "HideDown");
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
	}

	public void HideTaskPanel()
	{
		TaskAndRoleDlg.Instance.HideSelf();
	}

	public void TransBtnClick()
	{
		if (isNoMap)
		{
			comingTip.SetActive(false);
			comingTip.SetActive(true);
		}
		else if (UserDataManager.Instance.GetService().life == 0 && !UserDataManager.Instance.GetService().unlimitedLife)
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.GetLifeDlg);
		}
		else
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.EnterGameDlg);
		}
	}

	public void LifeBtnClick()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.GetLifeDlg);
	}

	public void SettingBtnClick()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.SettingDlg);
	}

	public void GoldBtnClick()
	{
		DialogManagerTemp.Instance.OpenShopDlg("BuyInCastleScene");
	}

	public void ShakeInBoxBtn(bool isShake)
	{
	}

	public void ScrollBtnClick()
	{
		DialogManagerTemp.Instance.ShowDialog(DialogType.HelpDlg);
	}

	public void SaleBtnClick()
	{
		DialogManagerTemp.Instance.OpenSaleDlg(0f, false);
	}

	public void BankBtnClick()
	{
		DialogManagerTemp.Instance.OpenBankDlg(0f);
	}

	public void ComboBtnClick()
	{
		DialogManagerTemp.Instance.OpenComboDlg(0f, true);
	}

	private void Update()
	{
		UpdateTimeText();
		UpdateSaleTimeText();
		if (Input.GetKeyDown(KeyCode.Q))
		{
			UserDataManager.Instance.GetService().unlimitedLife = true;
			UserDataManager.Instance.GetService().unlimitedLifeStartTM = DateTime.Now.Ticks / 10000000;
			UserDataManager.Instance.GetService().unlimitedLifeTM = 60L;
		}
	}

	private void UpdateSaleTimeText()
	{
		long num = -1L;
		if (UserDataManager.Instance.GetService().isSaling && Application.internetReachability != 0 && Purchaser.Instance.IsInitialized())
		{
			SaleBtn.gameObject.SetActive(true);
			if (UserDataManager.Instance.GetService().SaleShowNum <= 1)
			{
				SaleBtn.transform.GetChild(0).gameObject.SetActive(false);
				SaleBtn.transform.GetChild(1).gameObject.SetActive(true);
			}
			else
			{
				SaleBtn.transform.GetChild(0).gameObject.SetActive(false);
				SaleBtn.transform.GetChild(1).gameObject.SetActive(true);
			}
			num = UserDataManager.Instance.GetService().SaleTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().SaleStartTM);
			if (num <= 0)
			{
				UserDataManager.Instance.GetService().isSaling = false;
				UserDataManager.Instance.GetService().SaleTM = -1L;
				UserDataManager.Instance.GetService().SaleStartTM = -1L;
				SaleBtn.gameObject.SetActive(false);
				return;
			}
			int num2 = (int)num / 3600;
			int num3 = (int)(num - num2 * 60 * 60) / 60;
			int num4 = (int)num - num2 * 60 * 60 - num3 * 60;
			SaleTimeText.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
		}
		else
		{
			SaleBtn.gameObject.SetActive(false);
		}
	}

	private void UpdateTimeText()
	{
		long num = -1L;
		if (UserDataManager.Instance.GetService().unlimitedLife)
		{
			unlimitLifeImage.gameObject.SetActive(true);
			lifeText.text = "";
			num = UserDataManager.Instance.GetService().unlimitedLifeTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().unlimitedLifeStartTM);
		}
		else if (UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal)
		{
			unlimitLifeImage.gameObject.SetActive(false);
			if (UserDataManager.Instance.GetService().lifeConsumeTime == -1)
			{
				return;
			}
			if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal)
			{
				lifeRecoveTimeText.text = "Full";
				return;
			}
			num = GeneralConfig.LifeRecoverTime - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().lifeConsumeTime);
		}
		else
		{
			unlimitLifeImage.gameObject.SetActive(false);
		}
		int num2 = (int)num / 3600;
		int num3 = (int)(num - num2 * 60 * 60) / 60;
		int num4 = (int)num - num2 * 60 * 60 - num3 * 60;
		if (num2 > 0)
		{
			lifeRecoveTimeText.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0');
		}
		else if (num3 > 0)
		{
			lifeRecoveTimeText.text = num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
		}
		else if (num4 > 0)
		{
			lifeRecoveTimeText.text = "00:" + num4.ToString().PadLeft(2, '0');
		}
		else
		{
			lifeRecoveTimeText.text = "Full";
		}
	}

	public void UpdateLifeNumberText()
	{
		if (lifeText != null)
		{
			lifeText.text = UserDataManager.Instance.GetService().life.ToString();
		}
	}

	private void OnDestroy()
	{
		Destroy();
	}

	public void Destroy()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(21u, PressEsc);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(32u, CloseAllDialogs);
	}

	private void PressEsc(uint iMessageType, object arg)
	{
		if (!DialogManagerTemp.Instance.IsDialogShowing() && PlotManager.Instance.isStepFinished && !GlobalVariables.IsMaskCastleUI && !GlobalVariables.IsMaskDialog)
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.QuitGameDlg);
		}
	}

	public void TestRewardedVideoBtnClicked()
	{

	}
}
