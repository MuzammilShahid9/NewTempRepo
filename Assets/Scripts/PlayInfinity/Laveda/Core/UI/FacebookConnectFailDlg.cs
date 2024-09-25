using PlayInfinity.AliceMatch3.Core;

namespace PlayInfinity.Laveda.Core.UI
{
	public class FacebookConnectFailDlg : BaseDialog
	{
		public LocalizationText info;

		private static FacebookConnectFailDlg instance;

		public static FacebookConnectFailDlg Instance
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
			if (obj != null)
			{
				info.KeyString = obj.ToString();
			}
			else
			{
				info.KeyString = "FacebookConnectFailDlg_FacebookConnectedFail";
			}
		}

		public void Close(bool isAnim = true)
		{
			DebugUtils.Log(DebugType.NetWork, "FacebookConnect Faile! ");
			DialogManagerTemp.Instance.CloseDialog(DialogType.FacebookConnectFailDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}
	}
}
