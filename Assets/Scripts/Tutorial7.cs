using PlayInfinity.Laveda.Core.UI;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial7 : MonoBehaviour
{
	public Button startBtn;

	private void Start()
	{
		base.transform.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").transform.GetComponent<Camera>();
		startBtn.onClick.AddListener(delegate
		{
			EnterGameDlg.Instance.StartBtnClick();
			TutorialManager.Instance.FinishBoosterTutorial();
		});
	}
}
