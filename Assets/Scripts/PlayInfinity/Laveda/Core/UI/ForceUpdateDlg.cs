using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class ForceUpdateDlg : BaseDialog
	{
		private static ForceUpdateDlg instance;

		public static ForceUpdateDlg Instance
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
			DialogManagerTemp.Instance.CloseDialog(DialogType.ForceUpdateDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
		}

		public void QuitBtnClick()
		{
			Application.OpenURL("market://details?id=" + GeneralConfig.PackageName);
		}
	}
}
