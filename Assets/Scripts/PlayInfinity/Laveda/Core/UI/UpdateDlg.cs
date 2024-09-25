using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class UpdateDlg : BaseDialog
	{
		private static UpdateDlg instance;

		public static UpdateDlg Instance
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
		}

		public void Close(bool isAnim = true)
		{
			Invoke("LoadData", 1f);
			DialogManagerTemp.Instance.CloseDialog(DialogType.UpdateDlg);
		}

		private void LoadData()
		{
			Singleton<MessageDispatcher>.Instance().SendMessage(29u, null);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		public void QuitBtnClick()
		{
			Application.OpenURL("market://details?id=" + GeneralConfig.PackageName);
		}
	}
}
