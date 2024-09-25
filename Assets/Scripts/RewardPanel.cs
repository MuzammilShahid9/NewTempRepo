using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class RewardPanel : MonoBehaviour
{
	public GameObject rewardItem;

	public GameObject grid;

	private void Start()
	{
		int stage = UserDataManager.Instance.GetService().stage;
		string stageAward = StageManage.Instance.GetStageAward(stage);
		ShowAward(stageAward);
	}

	public void ShowAward(string awardString)
	{
		int num = 0;
		string[] array = awardString.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != "")
			{
				Object.Instantiate(rewardItem, grid.transform).GetComponent<RewardItem>().Enter(array[i]);
				num++;
			}
		}
		grid.GetComponent<RectTransform>().sizeDelta = new Vector2(num * 100, 120f);
		base.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(num * 100 + 40, 140f);
	}

	private void Update()
	{
		if (base.gameObject.activeInHierarchy && Input.GetMouseButtonDown(0))
		{
			base.gameObject.SetActive(false);
			TaskPanelManager.Instance.RewardPanelHide();
		}
	}
}
