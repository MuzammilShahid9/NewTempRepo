using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class QuitGameDlg : BaseDialog
	{
		private static QuitGameDlg instance;

		public static QuitGameDlg Instance
		{
			get
			{
				return instance;
			}
		}

		private AndroidJavaObject currentActivity
		{
			get
			{
				return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
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

		public void Close()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitGameDlg);
		}

		public void CloseBtnClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitGameDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			CloseBtnClick();
		}

		public void QuitBtnClick()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.QuitGameDlg);
			GoHome2();
		}

		private void GoHome2()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent", androidJavaClass.GetStatic<AndroidJavaObject>("ACTION_MAIN"));
			int @static = androidJavaClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
			androidJavaObject.Call<AndroidJavaObject>("setFlags", new object[1] { @static });
			androidJavaObject.Call<AndroidJavaObject>("addCategory", new object[1] { androidJavaClass.GetStatic<AndroidJavaObject>("CATEGORY_HOME") });
			currentActivity.Call("startActivity", androidJavaObject);
		}
	}
}
