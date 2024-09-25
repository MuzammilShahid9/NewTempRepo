using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class RateDlg : BaseDialog
	{
		private static RateDlg instance;

		public Image contentBg;

		public Image leftRoleImage;

		public GameObject leftStartPosition;

		public GameObject leftEndPosition;

		public GameObject detail;

		public Text leftRoleName;

		public GameObject starImage;

		public static RateDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			anim.enabled = false;
			leftRoleName.transform.parent.gameObject.SetActive(false);
			leftRoleImage.transform.localPosition = leftStartPosition.transform.localPosition;
			detail.gameObject.SetActive(false);
			starImage.SetActive(false);
			StartCoroutine(ShowDlgAnim());
		}

		private IEnumerator ShowDlgAnim()
		{
			contentBg.transform.localScale = new Vector3(0f, 1f, 1f);
			contentBg.transform.DOScaleX(1f, 0.2f);
			leftRoleImage.transform.localScale = new Vector3(1f, 1f, 1f);
			leftRoleImage.transform.DOLocalMoveX(leftEndPosition.transform.localPosition.x, 0.2f);
			yield return new WaitForSeconds(0.2f);
			leftRoleName.transform.parent.gameObject.SetActive(true);
			detail.gameObject.SetActive(true);
			starImage.SetActive(true);
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.RateDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClick();
		}

		public void BtnCloseClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.RateDlg);
			base.transform.parent.parent.GetComponent<Canvas>().sortingOrder = 200;
			CastleSceneUIManager.Instance.HideMask();
			RoleManager.Instance.ShowAllRoles();
			base.gameObject.SetActive(false);
		}

		public void BtnRateClick()
		{
			base.transform.parent.parent.GetComponent<Canvas>().sortingOrder = 200;
			UserDataManager.Instance.GetService().rateVersion = Application.version;
			UserDataManager.Instance.Save();
			CastleSceneUIManager.Instance.HideMask();
			Application.OpenURL("market://details?id=" + GeneralConfig.PackageName);
			DialogManagerTemp.Instance.CloseDialog(DialogType.RateDlg);
			RoleManager.Instance.ShowAllRoles();
			base.gameObject.SetActive(false);
		}
	}
}
