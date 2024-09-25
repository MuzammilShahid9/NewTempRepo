using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core.UI;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

public class TaskPanelManager : MonoBehaviour
{
	private static TaskPanelManager instance;

	public GameObject grid;

	public GameObject taskSonPanel;

	public GameObject watchVideoPanel;

	public GameObject arrow;

	public GameObject rewardMidPosition;

	public GameObject coinImage;

	public GameObject finishImage;

	public LocalizationText stageText;

	public LocalizationText stageNameText;

	public Image slideImage;

	public Text slideProgressText;

	public Button rewardBtn;

	public Button closeBtn;

	public GameObject rewardPanel;

	public GameObject flyCoinMidPos;

	public GameObject mask;

	public List<GameObject> taskSonPanelList = new List<GameObject>();

	[HideInInspector]
	public List<TaskConfigData> taskDataList = new List<TaskConfigData>();

	public TaskConfigData showTaskData;

	public List<TaskConfigData> notShowTaskData = new List<TaskConfigData>();

	public Vector2 startSize;

	private int showTaskIndex;

	private bool rewardPanelShow;

	private bool destoryFinish;

	private Tweener arrowTweener;

	private Vector3 arrowStartPosition;

	private GameObject watchVideoReward;

	public static TaskPanelManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		arrowStartPosition = arrow.transform.localPosition;
		startSize = slideImage.GetComponent<RectTransform>().rect.size;
		showTaskIndex = 0;
		taskDataList = TaskManager.Instance.showTaskList;
		for (int i = 0; i < taskDataList.Count; i++)
		{
			notShowTaskData.Add(taskDataList[i]);
		}
		if (notShowTaskData.Count > 0)
		{
			showTaskData = notShowTaskData[showTaskIndex];
		}
		if (taskDataList.Count >= 1)
		{
			for (int j = 0; j < taskDataList.Count; j++)
			{
				GameObject item = Object.Instantiate(taskSonPanel, grid.transform);
				taskSonPanelList.Add(item);
			}
			if (watchVideoReward == null && UserDataManager.Instance.GetService().watchVideoTime < Singleton<PlayGameData>.Instance().gameConfig.TaskVideoLimit && UserDataManager.Instance.GetService().level >= Singleton<PlayGameData>.Instance().gameConfig.ActvieWatchVideoInTask)
			{
				watchVideoReward = Object.Instantiate(watchVideoPanel, grid.transform);
			}
		}
	}

	private void OnEnable()
	{
		ShowArrow();
		mask.SetActive(false);
		destoryFinish = true;
		slideImage.fillAmount = TaskManager.Instance.taskProgress;
		string[] array = LanguageConfig.GetString("TaskAndRoleDlg_Stage" + UserDataManager.Instance.GetStage()).Split(':');
		stageText.text = array[0] + ":";
		if (array.Length > 1)
		{
			stageNameText.text = array[1];
		}
		CloseBtnToggle(true);
		slideProgressText.text = (int)(TaskManager.Instance.taskProgress * 100f) + "%";
		if (TaskManager.Instance.taskProgress >= 1f)
		{
			ShowStageReward();
		}
		rewardBtn.gameObject.SetActive(true);
		showTaskIndex = 0;
		taskDataList = TaskManager.Instance.showTaskList;
		if (UserDataManager.Instance.GetService().stage > GeneralConfig.MaxStage)
		{
			string[] array2 = LanguageConfig.GetString("TaskAndRoleDlg_Stage" + (UserDataManager.Instance.GetStage() - 1)).Split(':');
			stageText.text = array2[0] + ":";
			stageNameText.text = array2[1];
			finishImage.SetActive(true);
			slideProgressText.text = "100%";
			slideImage.fillAmount = 1f;
			grid.SetActive(false);
			return;
		}
		grid.SetActive(true);
		finishImage.SetActive(false);
		for (int i = 0; i < taskSonPanelList.Count; i++)
		{
			bool flag = false;
			TaskConfigData currTaskData = taskSonPanelList[i].GetComponent<TaskSonPanel>().currTaskData;
			if (currTaskData == null)
			{
				continue;
			}
			for (int j = 0; j < taskDataList.Count; j++)
			{
				if (taskDataList[j].ID == currTaskData.ID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				destoryFinish = false;
				mask.SetActive(true);
				StartCoroutine(DelayDestory(taskSonPanelList[i]));
			}
			else
			{
				TaskSonPanel component = taskSonPanelList[i].GetComponent<TaskSonPanel>();
				component.Enter(component.currTaskData);
			}
		}
		if (!destoryFinish && TaskManager.Instance.taskProgress != 0f)
		{
			StartCoroutine(ProgressIncress(1f / (float)TaskManager.Instance.currStageTask.Count));
		}
		if (taskSonPanelList.Count < 1)
		{
			destoryFinish = true;
		}
		notShowTaskData.Clear();
		for (int k = 0; k < taskDataList.Count; k++)
		{
			bool flag2 = false;
			for (int l = 0; l < taskSonPanelList.Count; l++)
			{
				TaskConfigData currTaskData2 = taskSonPanelList[l].GetComponent<TaskSonPanel>().currTaskData;
				if (taskDataList[k].ID == currTaskData2.ID)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				notShowTaskData.Add(taskDataList[k]);
				StartCoroutine(DelayInstantiate());
			}
		}
		if (taskDataList.Count >= 1)
		{
			if (showTaskIndex < notShowTaskData.Count)
			{
				showTaskData = notShowTaskData[showTaskIndex];
			}
			if (watchVideoReward == null && UserDataManager.Instance.GetService().watchVideoTime < Singleton<PlayGameData>.Instance().gameConfig.TaskVideoLimit && PlayInfinityAdManager.Instance.IsRewardVideoLoaded() && UserDataManager.Instance.GetService().level >= Singleton<PlayGameData>.Instance().gameConfig.ActvieWatchVideoInTask)
			{
				watchVideoReward = Object.Instantiate(watchVideoPanel, grid.transform);
				watchVideoReward.transform.SetAsLastSibling();
			}
			else if (watchVideoReward != null && !PlayInfinityAdManager.Instance.IsRewardVideoLoaded())
			{
				Object.Destroy(watchVideoReward);
			}
		}
	}

	public void FlyIcon(Vector3 startPosition)
	{
		CloseBtnToggle(false);
		StartCoroutine(FlyIconIenumerator(startPosition));
	}

	private IEnumerator FlyIconIenumerator(Vector3 startPosition)
	{
		int num = Random.Range(0, GeneralConfig.WatchVideoRewardCoinNumber.Length);
		int rewardCoinNumber = GeneralConfig.WatchVideoRewardCoinNumber[num];
		int costScrollNum = rewardCoinNumber / GeneralConfig.PerCoinImageContainCoinNumber;
		Vector3[] path = new Vector3[3]
		{
			startPosition,
			flyCoinMidPos.transform.position,
			CastleSceneUIManager.Instance.flyCoinEndPos.transform.position
		};
		for (int i = 0; i < costScrollNum; i++)
		{
			GameObject coin = Object.Instantiate(coinImage);
			AudioManager.Instance.PlayAudioEffect("coins_buy_success");
			coin.transform.SetParent(base.transform);
			coin.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
			coin.transform.position = startPosition;
			coin.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(coin.transform.DOPath(path, 1.5f, PathType.CatmullRom).SetEase(Ease.Linear));
			sequence.OnComplete(delegate
			{
				coin.SetActive(false);
				Object.Destroy(coin);
			});
		}
		yield return new WaitForSeconds(1.5f + 0.1f * (float)costScrollNum);
		UserDataManager.Instance.GetService().watchVideoTime++;
		UserDataManager.Instance.GetService().coin += rewardCoinNumber;
		CastleSceneUIManager.Instance.UpdateBtnText();
		UserDataManager.Instance.Save();
		CloseBtnToggle(true);
	}

	public void WatchVideoFinish()
	{
		StartCoroutine(DelayDestoryVideoPanel(watchVideoReward));
	}

	private IEnumerator ProgressIncress(float number)
	{
		yield return null;
		float startNum = TaskManager.Instance.taskProgress - number;
		float timer = 0f;
		while (timer <= 1f)
		{
			yield return null;
			timer += Time.deltaTime;
			slideImage.fillAmount = startNum + timer * number;
		}
		AudioManager.Instance.PlayAudioEffect("collected_tick");
	}

	public void ShowStageReward()
	{
		CloseBtnToggle(false);
		StartCoroutine(RewardBtnShake());
	}

	public void CloseBtnToggle(bool index)
	{
		closeBtn.enabled = index;
	}

	public void ShowMask()
	{
		mask.SetActive(true);
	}

	private IEnumerator RewardBtnShake()
	{
		mask.SetActive(true);
		Vector3[] array = new Vector3[3]
		{
			rewardBtn.transform.position,
			rewardMidPosition.transform.position,
			base.transform.position
		};
		yield return null;
		rewardBtn.transform.DOShakeRotation(0.5f, new Vector3(1f, 1f, 20f), 20, 90f, false).SetLoops(3);
		yield return new WaitForSeconds(1.5f);
		AudioManager.Instance.PlayAudioEffect("gift_get");
		rewardBtn.gameObject.SetActive(false);
		TaskAndRoleDlg.Instance.CloseTaskPanel();
		CastleSceneUIManager.Instance.StageRewardFly();
	}

	private IEnumerator DelayDestoryVideoPanel(GameObject tartgetGameObject)
	{
		tartgetGameObject.GetComponent<WatchVideoPanel>().Enter();
		yield return new WaitForSeconds(1.5f);
		Object.Destroy(tartgetGameObject);
		watchVideoReward = null;
		destoryFinish = true;
		yield return new WaitForSeconds(1.5f);
		if (UserDataManager.Instance.GetService().watchVideoTime < Singleton<PlayGameData>.Instance().gameConfig.TaskVideoLimit && PlayInfinityAdManager.Instance.IsRewardVideoLoaded() && UserDataManager.Instance.GetService().level >= Singleton<PlayGameData>.Instance().gameConfig.ActvieWatchVideoInTask)
		{
			watchVideoReward = Object.Instantiate(watchVideoPanel, grid.transform);
			watchVideoReward.transform.SetAsLastSibling();
		}
	}

	private IEnumerator DelayDestory(GameObject tartgetGameObject)
	{
		arrow.gameObject.SetActive(false);
		TaskSonPanel component = tartgetGameObject.GetComponent<TaskSonPanel>();
		component.Enter(component.currTaskData, true);
		yield return new WaitForSeconds(1.5f);
		Object.Destroy(tartgetGameObject);
		taskSonPanelList.Remove(tartgetGameObject);
		destoryFinish = true;
		yield return null;
		mask.SetActive(false);
	}

	private IEnumerator DelayInstantiate()
	{
		yield return new WaitUntil(() => destoryFinish);
		AudioManager.Instance.PlayAudioEffect("new_task");
		GameObject item = Object.Instantiate(taskSonPanel, grid.transform);
		taskSonPanelList.Add(item);
		if (watchVideoReward != null)
		{
			watchVideoReward.transform.SetAsLastSibling();
		}
		ShowArrow();
	}

	public void ShowArrow()
	{
		int tutorialProgress = UserDataManager.Instance.GetService().tutorialProgress;
		int level = UserDataManager.Instance.GetService().level;
		if (tutorialProgress >= 6 && !arrow.activeInHierarchy && level <= 5 && notShowTaskData.Count > 0)
		{
			arrow.gameObject.SetActive(true);
		}
		else
		{
			arrow.gameObject.SetActive(false);
		}
	}

	public void RewardBtnClick()
	{
		if (UserDataManager.Instance.GetService().stage <= GeneralConfig.MaxStage && !rewardPanelShow)
		{
			mask.SetActive(true);
			rewardPanel.SetActive(true);
			rewardPanelShow = true;
		}
	}

	public void RewardPanelHide()
	{
		mask.SetActive(false);
		rewardPanelShow = false;
	}

	public void HideArrow()
	{
		arrow.gameObject.SetActive(false);
	}

	public void showOneTask()
	{
		if (showTaskIndex + 1 < notShowTaskData.Count)
		{
			showTaskIndex++;
			showTaskData = notShowTaskData[showTaskIndex];
		}
	}

	private void OnDisable()
	{
		HideArrow();
	}
}
