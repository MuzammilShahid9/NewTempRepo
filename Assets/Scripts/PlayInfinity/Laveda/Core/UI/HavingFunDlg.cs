using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class HavingFunDlg : BaseDialog
	{
		private static HavingFunDlg instance;

		public Image contentBg;

		public Image leftRoleImage;

		public GameObject leftStartPosition;

		public GameObject leftEndPosition;

		public GameObject detail;

		public Text leftRoleName;

		public static HavingFunDlg Instance
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

		public override void Show()
		{
			Show(null);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			StartCoroutine(HideRoleAndUI());
			leftRoleName.transform.parent.gameObject.SetActive(false);
			detail.gameObject.SetActive(false);
			leftRoleImage.transform.localPosition = leftStartPosition.transform.localPosition;
			contentBg.transform.localScale = new Vector3(0f, 1f, 1f);
			StartCoroutine(ShowDlgAnim());
		}

		private IEnumerator ShowDlgAnim()
		{
			yield return new WaitForSeconds(1f);
			CastleSceneUIManager.Instance.ShowMask();
			yield return new WaitForSeconds(0.2f);
			base.transform.parent.parent.GetComponent<Canvas>().sortingOrder = 302;
			contentBg.transform.DOScaleX(1f, 0.2f);
			leftRoleImage.transform.localScale = new Vector3(1f, 1f, 1f);
			leftRoleImage.transform.DOLocalMoveX(leftEndPosition.transform.localPosition.x, 0.2f);
			yield return new WaitForSeconds(0.2f);
			leftRoleName.transform.parent.gameObject.SetActive(true);
			detail.gameObject.SetActive(true);
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.HavingFunDlg);
		}

		private IEnumerator HideRoleAndUI()
		{
			yield return new WaitForSeconds(0.1f);
			CastleSceneUIManager.Instance.HideAllBtn();
			RoleManager.Instance.HideAllRoles();
		}

		public void BtnCloseClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.HavingFunDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			CancelBtnClick();
		}

		public void CancelBtnClick()
		{
			ShowFeedbackDlg();
		}

		public void ShowFeedbackDlg()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.HavingFunDlg, false, false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.FeedbackDlg);
		}

		public void RateBtnClick()
		{
			ShowRateDlg();
		}

		public void ShowRateDlg()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.HavingFunDlg, false, false);
			DialogManagerTemp.Instance.ShowDialog(DialogType.RateDlg);
		}

		public void Close()
		{
			DebugUtils.Log(DebugType.UI, "close with anim");
			anim.SetTrigger("hide");
			AudioManager.Instance.PlayAudioEffect("window_close");
			isAniming = true;
			Timer.Schedule(this, hidingAnimation.length, base.Hide);
		}
	}
}
