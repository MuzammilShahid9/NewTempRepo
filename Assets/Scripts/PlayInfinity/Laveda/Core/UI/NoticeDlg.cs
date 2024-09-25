using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class NoticeDlg : BaseDialog
	{
		private static NoticeDlg instance;

		private bool isCanCheck;

		public static NoticeDlg Instance
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
			Timer.Schedule(this, 0.7f, delegate
			{
				isCanCheck = true;
			});
		}

		private void Update()
		{
			if (isCanCheck && Input.GetMouseButtonDown(0))
			{
				isCanCheck = false;
				Close();
			}
		}

		public void Close()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.NoticeDlg);
		}
	}
}
