using System.Collections;
using PlayInfinity.AliceMatch3.Core.UI;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

public class RoleDetailSonPanel : BaseUI
{
	public Image roleImage;

	public LocalizationText roleNameText;

	public LocalizationText tagText1;

	public LocalizationText tagText2;

	public LocalizationText tagText3;

	public LocalizationText detailInfo;

	public Image detailBg;

	public Image detailUnderLineImage;

	public float gridMinHeight = 23f;

	private RoleConfigData currRoleData;

	public void Enter(RoleConfigData roleData)
	{
		currRoleData = roleData;
		ShowDetail();
	}

	public void ShowDetail()
	{
		roleImage.sprite = Resources.Load("Textures/RoleHead/" + currRoleData.Image, typeof(Sprite)) as Sprite;
		roleNameText.SetKeyString(currRoleData.RoleName);
		if (currRoleData.ID == 4)
		{
			if (UserDataManager.Instance.GetService().catName != "")
			{
				roleNameText.SetKeyString("");
				roleNameText.text = UserDataManager.Instance.GetService().catName;
			}
			else
			{
				roleNameText.SetKeyString("");
				roleNameText.text = "";
			}
		}
		tagText1.SetKeyString("");
		tagText1.text = LanguageConfig.GetString(currRoleData.RoleTag.Split(';')[0]).Split('.')[0];
		tagText2.SetKeyString(currRoleData.RoleTag.Split(';')[1]);
		tagText3.SetKeyString(currRoleData.RoleTag.Split(';')[2]);
		detailInfo.SetKeyString(currRoleData.RoleExperience);
		StartCoroutine(SetTextHeight());
	}

	private IEnumerator SetTextHeight()
	{
		float preferredHeight = detailInfo.preferredHeight;
		if (preferredHeight < gridMinHeight)
		{
			preferredHeight = gridMinHeight;
		}
		detailInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(detailInfo.GetComponent<RectTransform>().sizeDelta.x, preferredHeight);
		detailBg.GetComponent<RectTransform>().sizeDelta = new Vector2(detailBg.GetComponent<RectTransform>().sizeDelta.x, preferredHeight + 50f);
		Vector3 localPosition = detailUnderLineImage.transform.localPosition;
		detailUnderLineImage.transform.localPosition = new Vector3(localPosition.x, -30f - preferredHeight, localPosition.z);
		yield return null;
	}

	public void ReturnBtnClick()
	{
		TaskAndRoleDlg.Instance.ReturnBtnClick();
	}
}
