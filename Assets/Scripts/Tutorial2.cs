using UnityEngine;
using UnityEngine.UI;

public class Tutorial2 : MonoBehaviour
{
	public GameObject arrow;

	public Button doTaskBtn;

	private Vector3 arrowStartPosition;

	private void Start()
	{
		base.transform.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").transform.GetComponent<Camera>();
		TaskSonPanel targetTaskSonPanel = TaskPanelManager.Instance.taskSonPanelList[0].GetComponent<TaskSonPanel>();
		doTaskBtn.onClick.AddListener(delegate
		{
			targetTaskSonPanel.TaskBtnClick();
		});
	}
}
