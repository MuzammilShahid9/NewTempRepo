using System;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class ConfirmDataDlg : BaseDialog
	{
		public InputField inputField;

		private Action action;

		private static ConfirmDataDlg instance;

		public static ConfirmDataDlg Instance
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
			action = (Action)obj;
		}

		public void OK()
		{
			if (inputField.text == "OK")
			{
				action();
			}
			inputField.text = "";
		}

		public void Close(bool isAnim = true)
		{
			inputField.text = "";
			DialogManagerTemp.Instance.CloseDialog(DialogType.ConfirmDataDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}

		public void StringChange(string str)
		{
			inputField.text = inputField.text.ToUpper();
		}
	}
}
