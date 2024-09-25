using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class LackOfScrollDlg : BaseDialog
	{
		public GameObject arrow;

		private Tweener arrowTweener;

		private Vector3 arrowStartPosition;

		private static LackOfScrollDlg instance;

		public static LackOfScrollDlg Instance
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
			arrowStartPosition = arrow.transform.localPosition;
		}

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.LackOfScrollDlg);
		}

		public override void Show(object obj)
		{
			int tutorialProgress = UserDataManager.Instance.GetService().tutorialProgress;
			int num = UserDataManager.Instance.GetService().level;
			if (tutorialProgress >= 6 && num <= 5)
			{
				arrow.SetActive(true);
			}
			else
			{
				arrow.SetActive(false);
			}
			base.Show(obj);
		}

		public void BtnCloseClicked()
		{
			TaskPanelManager.Instance.ShowArrow();
			DialogManagerTemp.Instance.CloseDialog(DialogType.LackOfScrollDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public void PlayBtnClicked()
		{
			TaskPanelManager.Instance.ShowArrow();
			arrow.SetActive(false);
			DialogManagerTemp.Instance.ShowDialogAndPopAll(DialogType.EnterGameDlg);
		}
	}
}
