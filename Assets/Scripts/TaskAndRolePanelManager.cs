using UnityEngine;
using UnityEngine.UI;

public class TaskAndRolePanelManager : MonoBehaviour
{
	public GameObject taskPanel;

	public GameObject rolePanel;

	public Button closeBtn;

	public Button taskPanelBtn;

	public Button rolePanelBtn;

	private static TaskAndRolePanelManager instance;

	public static TaskAndRolePanelManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void TaskBtnClick()
	{
		taskPanel.SetActive(true);
		rolePanel.SetActive(false);
	}

	public void RoleBtnClick()
	{
		taskPanel.SetActive(false);
		rolePanel.SetActive(true);
	}

	public void CloseTaskPanel()
	{
		base.gameObject.SetActive(false);
		CastleSceneUIManager.Instance.ShowTaskAndEnterGameBtn();
	}
}
