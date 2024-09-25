using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class RoleDetailPanelManager : BaseUI
{
	public GameObject leftArrow;

	public GameObject rightArrow;

	public GameObject leftDisableArrow;

	public GameObject rightDisableArrow;

	public GameObject roleDetialSonPanel;

	[HideInInspector]
	public RoleDetailSonPanel currRoleDetailSonPanel;

	private RoleConfigData currRoleData;

	private static RoleDetailPanelManager instance;

	public static RoleDetailPanelManager Instance
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

	public void Enter(RoleConfigData roleData)
	{
		currRoleData = roleData;
		GameObject gameObject = Object.Instantiate(roleDetialSonPanel, base.transform);
		currRoleDetailSonPanel = gameObject.GetComponent<RoleDetailSonPanel>();
		currRoleDetailSonPanel.Enter(currRoleData);
		ShowArrow();
	}

	public void ShowArrow()
	{
		int num = RolePanelManager.Instance.GerRoleIndex(currRoleData);
		RoleConfigData roleConfigData = RolePanelManager.Instance.GerNextRoleInfo(currRoleData);
		RoleConfigData roleConfigData2 = RolePanelManager.Instance.GerLastRoleInfo(currRoleData);
		if (num == 0 || roleConfigData2 == null)
		{
			leftArrow.SetActive(false);
			leftDisableArrow.SetActive(true);
		}
		else if (!JudgeIsRoleUnlock(roleConfigData2.ID) && !JudgeSpecialRoleUnlock(roleConfigData2))
		{
			leftArrow.SetActive(false);
			leftDisableArrow.SetActive(true);
		}
		else
		{
			leftArrow.SetActive(true);
			leftDisableArrow.SetActive(false);
		}
		if (num == RolePanelManager.Instance.roleConfig.Count - 1 || roleConfigData == null)
		{
			rightArrow.SetActive(false);
			rightDisableArrow.SetActive(true);
		}
		else if (!JudgeIsRoleUnlock(roleConfigData.ID) && !JudgeSpecialRoleUnlock(roleConfigData))
		{
			rightArrow.SetActive(false);
			rightDisableArrow.SetActive(true);
		}
		else
		{
			rightArrow.SetActive(true);
			rightDisableArrow.SetActive(false);
		}
	}

	public void HideArrow()
	{
		leftArrow.SetActive(false);
		rightArrow.SetActive(false);
	}

	public void NextBtnClick()
	{
		HideArrow();
		GameObject go1 = currRoleDetailSonPanel.gameObject;
		currRoleDetailSonPanel.transform.DOLocalMoveX(-800f, 0.3f);
		GameObject gameObject = Object.Instantiate(roleDetialSonPanel, base.transform);
		gameObject.transform.localPosition = new Vector3(800f, 0f, 0f);
		currRoleDetailSonPanel = gameObject.GetComponent<RoleDetailSonPanel>();
		currRoleData = RolePanelManager.Instance.GerNextRoleInfo(currRoleData);
		currRoleDetailSonPanel.Enter(currRoleData);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(gameObject.transform.DOLocalMoveX(0f, 0.3f));
		sequence.OnComplete(delegate
		{
			Object.Destroy(go1);
			ShowArrow();
		});
	}

	public void LastBtnClick()
	{
		HideArrow();
		GameObject go1 = currRoleDetailSonPanel.gameObject;
		currRoleDetailSonPanel.transform.DOLocalMoveX(800f, 0.3f);
		GameObject gameObject = Object.Instantiate(roleDetialSonPanel, base.transform);
		gameObject.transform.localPosition = new Vector3(-800f, 0f, 0f);
		currRoleDetailSonPanel = gameObject.GetComponent<RoleDetailSonPanel>();
		currRoleData = RolePanelManager.Instance.GerLastRoleInfo(currRoleData);
		currRoleDetailSonPanel.Enter(currRoleData);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(gameObject.transform.DOLocalMoveX(0f, 0.3f));
		sequence.OnComplete(delegate
		{
			Object.Destroy(go1);
			ShowArrow();
		});
	}

	public bool JudgeIsRoleUnlock(int roleID)
	{
		List<int> unlockRoleIDList = UserDataManager.Instance.GetService().UnlockRoleIDList;
		if (unlockRoleIDList == null)
		{
			return false;
		}
		for (int i = 0; i < unlockRoleIDList.Count; i++)
		{
			if (roleID == unlockRoleIDList[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool JudgeSpecialRoleUnlock(RoleConfigData roleData)
	{
		if (roleData.ID == 3)
		{
			if (UserDataManager.Instance.GetService().stage > int.Parse(roleData.UnlockPlotId.Split(',')[0]))
			{
				if (!UserDataManager.Instance.GetService().UnlockRoleIDList.Contains(3))
				{
					UserDataManager.Instance.GetService().UnlockRoleIDList.Add(3);
				}
				return true;
			}
			if (UserDataManager.Instance.GetService().stage == int.Parse(roleData.UnlockPlotId.Split(',')[0]) && TaskManager.Instance.finishTaskIDList.Count > 0)
			{
				if (!UserDataManager.Instance.GetService().UnlockRoleIDList.Contains(3))
				{
					UserDataManager.Instance.GetService().UnlockRoleIDList.Add(3);
				}
				return true;
			}
		}
		return false;
	}

	private void OnDisable()
	{
		currRoleData = null;
		if (currRoleDetailSonPanel != null)
		{
			Object.Destroy(currRoleDetailSonPanel.gameObject);
		}
	}
}
