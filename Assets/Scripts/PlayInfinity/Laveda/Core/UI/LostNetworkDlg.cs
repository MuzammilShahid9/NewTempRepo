using PlayInfinity.AliceMatch3.Core;

namespace PlayInfinity.Laveda.Core.UI
{
	public class LostNetworkDlg : BaseDialog
	{
		private static LostNetworkDlg instance;

		public static LostNetworkDlg Instance
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

		public void BtnCloseClicked()
		{
			Close();
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		public void Close(bool isAnim = true)
		{
			DebugUtils.Log(DebugType.NetWork, "NetWork Faile! ");
			DialogManagerTemp.Instance.CloseDialog(DialogType.LostNetworkDlg);
		}
	}
}
