using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core.UI;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

public class RoleSonPanel : BaseUI
{
	public Image roleImage;

	public LocalizationText roleNameText;

	public LocalizationText lockText;

	private RoleConfigData roleData;

	private Vector3 textStartPosition;

	private bool lockTextIsShow;

	private void Awake()
	{
		textStartPosition = lockText.transform.localPosition;
	}

	public void Enter(RoleConfigData tempRoleData)
	{
		lockText.gameObject.SetActive(false);
		roleData = tempRoleData;
		if (JudgeIsRoleUnlock())
		{
			roleImage.sprite = Resources.Load("Textures/RoleHead/" + roleData.Image, typeof(Sprite)) as Sprite;
			roleNameText.SetKeyString(roleData.RoleName);
			if (roleData.ID == 4)
			{
				roleNameText.SetKeyString("");
				roleNameText.text = UserDataManager.Instance.GetService().catName;
			}
		}
		else
		{
			roleImage.sprite = Resources.Load("Textures/RoleHead/" + roleData.HideImage, typeof(Sprite)) as Sprite;
			roleNameText.text = "???";
		}
	}

	public bool JudgeIsRoleUnlock()
	{
		List<int> unlockRoleIDList = UserDataManager.Instance.GetService().UnlockRoleIDList;
		if (JudgeSpecialRoleUnlock())
		{
			return true;
		}
		if (unlockRoleIDList == null)
		{
			return false;
		}
		for (int i = 0; i < unlockRoleIDList.Count; i++)
		{
			if (roleData.ID == unlockRoleIDList[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool JudgeSpecialRoleUnlock()
	{
		if (roleData.ID == 3)
		{
			if (UserDataManager.Instance.GetService().stage > int.Parse(roleData.UnlockPlotId.Split(',')[0]))
			{
				return true;
			}
			if (UserDataManager.Instance.GetService().stage == int.Parse(roleData.UnlockPlotId.Split(',')[0]) && TaskManager.Instance.finishTaskIDList.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void RoleBtnClick()
	{
		if (!JudgeIsRoleUnlock() && !lockTextIsShow)
		{
			lockTextIsShow = true;
			lockText.gameObject.SetActive(true);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(lockText.transform.DOLocalMoveY(20f, 1f));
			sequence.OnComplete(delegate
			{
				lockText.transform.localPosition = textStartPosition;
			});
			StartCoroutine(FadeOut(lockText, 1f));
		}
		else if (JudgeIsRoleUnlock())
		{
			TaskAndRoleDlg.Instance.ShowRoleDetail(roleData);
		}
	}

	private IEnumerator FadeOut(Text text, float time)
	{
		yield return null;
		float timer = 0f;
		Color textColor = text.color;
		while (timer <= time)
		{
			yield return null;
			timer += Time.deltaTime;
			float a = 1f - timer / time;
			text.color = new Color(textColor.r, textColor.g, textColor.b, a);
		}
		lockText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
		lockText.gameObject.SetActive(false);
		lockTextIsShow = false;
	}

	public void Exist()
	{
		StopAllCoroutines();
		Color color = lockText.color;
		lockText.color = new Color(color.r, color.g, color.b, 1f);
		lockText.transform.localPosition = textStartPosition;
		lockText.gameObject.SetActive(false);
		lockTextIsShow = false;
	}
}
