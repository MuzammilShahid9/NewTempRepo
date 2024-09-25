using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Leah.Core;

namespace PlayInfinity.Laveda.Core.UI
{
	public class FacebookConnectDlg : BaseDialog
	{
		private static FacebookConnectDlg instance;

		public static FacebookConnectDlg Instance
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
			DebugUtils.Log(DebugType.UI, "close instantly");
			DebugUtils.Log(DebugType.NetWork, "FacebookConnectDlg Closed! ");
			DialogManagerTemp.Instance.CloseDialog(DialogType.FacebookConnectDlg, true, false);
			FacebookUtilities.Instance.GetAllData(true);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}
	}
}
