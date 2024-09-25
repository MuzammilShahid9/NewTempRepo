using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class RenameDlg : BaseDialog
	{
		public LocalizationText titleText;

		public Button closeBtn;

		public InputField nameInput;

		private int showType;

		private static RenameDlg instance;

		public static RenameDlg Instance
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
			DialogManagerTemp.Instance.CloseDialog(DialogType.RenameDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			base.gameObject.SetActive(true);
			showType = (int)obj;
			if (showType == 0)
			{
				titleText.SetKeyString("RenameDlg_CastlesName");
				if (UserDataManager.Instance.GetService().castleName == "")
				{
					titleText.text = "";
					nameInput.text = LanguageConfig.GetString("RenameDlg_CastlesDefaultName");
				}
				else
				{
					nameInput.text = UserDataManager.Instance.GetService().castleName;
				}
			}
			else if (showType == 1)
			{
				titleText.SetKeyString("RenameDlg_CatsName");
				if (UserDataManager.Instance.GetService().catName == "")
				{
					nameInput.text = LanguageConfig.GetString("RenameDlg_CatDefaultName");
				}
				else
				{
					nameInput.text = UserDataManager.Instance.GetService().catName;
				}
			}
		}

		public void BtnCloseClicked()
		{
			if (!PlotManager.Instance.isPlotFinish)
			{
				PlayBtnClicked();
			}
			else
			{
				Close();
			}
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public void PlayBtnClicked()
		{
			if (showType == 0)
			{
				UserDataManager.Instance.GetService().castleName = nameInput.text;
			}
			else if (showType == 1)
			{
				UserDataManager.Instance.GetService().catName = nameInput.text;
			}
			UserDataManager.Instance.Save();
			if (showType == 0)
			{
				MyServerManager.Instance.SendCastleNameData();
			}
			else if (showType == 1)
			{
				MyServerManager.Instance.SendCatNameData();
			}
			PlotRenameManager.Instance.FinishStep();
			Close();
		}
	}
}
