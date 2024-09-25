using PlayInfinity.AliceMatch3.Core;

namespace PlayInfinity.Laveda.Core.UI
{
	public class LoadingDlg : BaseDialog
	{
		private static LoadingDlg instance;

		public static LoadingDlg Instance
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

		public override void PressEsc(uint iMessageType, object arg)
		{
		}
	}
}
