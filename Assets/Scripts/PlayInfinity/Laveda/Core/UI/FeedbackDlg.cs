using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class FeedbackDlg : BaseDialog
	{
		private static FeedbackDlg instance;

		public Image contentBg;

		public Image leftRoleImage;

		public GameObject leftStartPosition;

		public GameObject leftEndPosition;

		public GameObject detail;

		public GameObject mailImage;

		public Text leftRoleName;

		public static FeedbackDlg Instance
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

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.FeedbackDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			anim.enabled = false;
			leftRoleName.transform.parent.gameObject.SetActive(false);
			detail.gameObject.SetActive(false);
			leftRoleImage.transform.localPosition = leftStartPosition.transform.localPosition;
			mailImage.SetActive(false);
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
			mailImage.SetActive(true);
		}

		public void BtnCloseClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.FeedbackDlg, false);
			base.transform.parent.parent.GetComponent<Canvas>().sortingOrder = 200;
			CastleSceneUIManager.Instance.HideMask();
			RoleManager.Instance.ShowAllRoles();
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClick();
		}

		public void BtnSendClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.FeedbackDlg, false);
			base.transform.parent.parent.GetComponent<Canvas>().sortingOrder = 200;
			CastleSceneUIManager.Instance.HideMask();
			Function.Instance.SendMail();
			RoleManager.Instance.ShowAllRoles();
		}
	}
}
