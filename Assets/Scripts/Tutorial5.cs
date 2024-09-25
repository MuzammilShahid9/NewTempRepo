using PlayInfinity.Laveda.Core.UI;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial5 : MonoBehaviour
{
	public Button boosterBtn;

	public Image arrow;

	private Vector3 arrowStartPosition;

	private void Start()
	{
		base.transform.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").transform.GetComponent<Camera>();
		boosterBtn.onClick.AddListener(delegate
		{
			EnterGameDlg.Instance.SecondBtnClick();
			TutorialManager.Instance.ShowEnterGameBtnMask();
		});
	}
}
