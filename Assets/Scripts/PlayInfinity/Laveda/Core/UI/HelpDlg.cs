using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class HelpDlg : BaseDialog
	{
		private static HelpDlg instance;

		public static HelpDlg Instance
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

		protected override void Start()
		{
			base.Start();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.HelpDlg);
		}

		public override void Show(object obj)
		{
			CastleSceneUIManager.Instance.HideAllBtn();
			RoleManager.Instance.HideAllRoles();
			base.Show(obj);
		}

		public void BtnCloseClicked()
		{
			RoleManager.Instance.ShowAllRoles();
			DialogManagerTemp.Instance.CloseDialog(DialogType.HelpDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		private void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				BtnCloseClicked();
			}
		}
	}
}
