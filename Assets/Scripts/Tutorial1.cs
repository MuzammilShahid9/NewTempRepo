using UnityEngine;
using UnityEngine.UI;

public class Tutorial1 : MonoBehaviour
{
	public GameObject arrow;

	public Button taskBtn;

	private Vector3 arrowStartPosition;

	private void Start()
	{
		base.transform.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").transform.GetComponent<Camera>();
		taskBtn.onClick.AddListener(delegate
		{
			CastleSceneUIManager.Instance.TaskBtnClick();
		});
	}
}
