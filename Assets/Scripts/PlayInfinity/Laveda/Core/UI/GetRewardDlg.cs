using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class GetRewardDlg : BaseDialog
	{
		private static GetRewardDlg instance;

		public LocalizationText info;

		public Image Drop;

		public static GetRewardDlg Instance
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

		public override void Show()
		{
			base.Show();
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			DropType dropType = (DropType)obj;
			Drop.sprite = Resources.Load<GameObject>("Textures/Elements2/" + dropType).GetComponent<SpriteRenderer>().sprite;
			switch (dropType)
			{
			case DropType.AreaBomb:
				info.SetKeyString("GetRewardDlg_BombDetails");
				
				UserDataManager.Instance.GetService().bombNumber = GeneralConfig.ItemUnlockSendNumber[0];
				break;
			case DropType.ColorBomb:
				info.SetKeyString("GetRewardDlg_CrownDetails");
				
				UserDataManager.Instance.GetService().rainBowBallNumber = GeneralConfig.ItemUnlockSendNumber[1];
				break;
			case DropType.DoubleBee:
				info.SetKeyString("GetRewardDlg_DoubleBeesDetails");
				
				UserDataManager.Instance.GetService().doubleBeesNumber = GeneralConfig.ItemUnlockSendNumber[2];
				break;
			case DropType.Spoon:
				info.SetKeyString("GetRewardDlg_SpoonDetails");
				UserDataManager.Instance.GetService().malletNumber = GeneralConfig.ItemUnlockSendNumber[3];
				break;
			case DropType.Hammer:
				info.SetKeyString("GetRewardDlg_MagicMalletDetails");
				
				UserDataManager.Instance.GetService().magicMalletNumber = GeneralConfig.ItemUnlockSendNumber[4];
				break;
			case DropType.Glove:
				info.SetKeyString("GetRewardDlg_GloveDetails");
				
				UserDataManager.Instance.GetService().gloveNumber = GeneralConfig.ItemUnlockSendNumber[5];
				break;
			}
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.GetRewardDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			Close();
		}
	}
}
