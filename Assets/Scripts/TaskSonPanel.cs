using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.AliceMatch3.Core.UI;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

public class TaskSonPanel : BaseUI
{
	public Image taskImage;

	public LocalizationText nameText;

	public Text costScrollNumText;

	public Image[] stepImage;

	public Button doTaskBtn;

	public Text decreaseScrollNumText;

	public GameObject decreaseScroll;

	public GameObject scrollPosition;

	public Image finishImage;

	public int currTaskID;

	public TaskConfigData currTaskData;

	private int currStep = 2;

	private Vector3 startPosition;

	public int CurrStep
	{
		get
		{
			return currStep;
		}
		set
		{
			currStep = value;
		}
	}

	private void Awake()
	{
		Show();
	}

	private void OnEnable()
	{
		startPosition = base.transform.localPosition;
		decreaseScroll.gameObject.SetActive(false);
		doTaskBtn.enabled = true;
	}

	private void Show()
	{
		currTaskData = TaskPanelManager.Instance.showTaskData;
		TaskPanelManager.Instance.showOneTask();
		ShowDetail(currTaskData);
	}

	public void Enter(TaskConfigData taskData, bool isDestory = false)
	{
		base.gameObject.SetActive(true);
		currTaskData = taskData;
		ShowDetail(currTaskData);
		if (isDestory)
		{
			doTaskBtn.gameObject.SetActive(false);
			for (int i = 0; i < 3; i++)
			{
				stepImage[i].transform.Find("Image").gameObject.SetActive(true);
			}
			StartCoroutine(FinishProcess());
		}
	}

	private void ShowDetail(TaskConfigData taskData)
	{
		currTaskID = taskData.ID;
		doTaskBtn.onClick.RemoveAllListeners();
		doTaskBtn.enabled = true;
		doTaskBtn.gameObject.SetActive(true);
		doTaskBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		doTaskBtn.transform.Find("Text").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
		taskImage.sprite = Resources.Load("Textures/TaskIcon/" + taskData.TitleImage, typeof(Sprite)) as Sprite;
		nameText.SetKeyString(taskData.Name);
		costScrollNumText.text = "x" + taskData.CostScrollNum;
		decreaseScroll.gameObject.SetActive(false);
		decreaseScrollNumText.text = "-" + taskData.CostScrollNum;
		if (taskData.FinishConditionNum < 2)
		{
			stepImage[0].gameObject.SetActive(false);
			stepImage[1].gameObject.SetActive(false);
			stepImage[2].gameObject.SetActive(false);
		}
		else
		{
			for (int i = 0; i < stepImage.Length; i++)
			{
				if (i < taskData.FinishConditionNum)
				{
					stepImage[i].gameObject.SetActive(true);
				}
				else
				{
					stepImage[i].gameObject.SetActive(false);
				}
			}
		}
		string[] array = UserDataManager.Instance.GetCurrDoTaskString().Split(';');
		CurrStep = -1;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] != "" && taskData.ID == int.Parse(array[j].Split(',')[0]))
			{
				CurrStep = int.Parse(array[j].Split(',')[1]) + 1;
			}
		}
		if (CurrStep == -1 && taskData.FinishConditionNum > 1)
		{
			CurrStep = 0;
		}
		for (int k = 0; k < 3; k++)
		{
			if (k < CurrStep)
			{
				stepImage[k].transform.Find("Image").gameObject.SetActive(true);
			}
			else
			{
				stepImage[k].transform.Find("Image").gameObject.SetActive(false);
			}
		}
		doTaskBtn.onClick.AddListener(delegate
		{
			TaskBtnClick();
		});
	}

	public void TaskBtnClick()
	{
		TaskPanelManager.Instance.HideArrow();
		if (UserDataManager.Instance.GetScrollNum() >= currTaskData.CostScrollNum)
		{
			doTaskBtn.enabled = false;
			StartCoroutine(HideDoTaskBtn());
			TaskManager.Instance.StartTask(currTaskData);
			TaskPanelManager.Instance.CloseBtnToggle(false);
			TaskPanelManager.Instance.ShowMask();
			TaskAndRoleDlg.Instance.StartFlyIcon(currTaskID, CurrStep, scrollPosition.transform.position, currTaskData.CostScrollNum);
			CastleSceneUIManager.Instance.DoTaskBtnClick(currTaskID, doTaskBtn.transform.position, currTaskData.CostScrollNum);
		}
		else
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.LackOfScrollDlg, null, DialogType.TaskAndRoleDlg);
		}
	}

	private IEnumerator HideDoTaskBtn()
	{
		yield return new WaitForSeconds(0.8f + 0.2f * (float)(currTaskData.CostScrollNum - 1));
		doTaskBtn.GetComponent<Image>().DOFade(0f, 0.5f);
		doTaskBtn.transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.5f);
		yield return new WaitForSeconds(0.3f);
		decreaseScroll.gameObject.SetActive(true);
	}

	private IEnumerator FinishProcess()
	{
		doTaskBtn.gameObject.SetActive(false);
		decreaseScroll.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		finishImage.gameObject.SetActive(true);
		finishImage.DOFade(1f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		base.transform.DOLocalMoveX(startPosition.x - 1600f, 0.5f).SetEase(Ease.Linear);
		finishImage.DOFade(0f, 0.5f);
	}
}
