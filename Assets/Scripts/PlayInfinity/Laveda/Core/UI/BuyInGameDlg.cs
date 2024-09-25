using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using Umeng;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class BuyInGameDlg : BaseDialog
	{
		private static BuyInGameDlg instance;

		public LocalizationText titleText;

		public Image itemImage;

		public LocalizationText itemDescriptionText;

		public Text coinCost;

		public DropType buyIndex;

		public ParticleSystem effect1;

		public ParticleSystem effect2;

		private string ShopInfo = "";

		private int coinCostNumber;

		private int buyItemNumber;

		public static BuyInGameDlg Instance
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

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.BuyInGameDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			ShopInfo = "";
			base.gameObject.SetActive(true);
			DropType dropType = (buyIndex = (DropType)obj);
			buyItemNumber = 3;
			itemImage.GetComponent<Canvas>().sortingLayerName = "UI";
			if (dropType != 0)
			{
				itemImage.sprite = Resources.Load<GameObject>("Textures/Elements2/" + dropType).GetComponent<SpriteRenderer>().sprite;
			}
			switch (dropType)
			{
			case DropType.AreaBomb:
				ShopInfo = "AreaBomb";
				coinCostNumber = GeneralConfig.ItemBuyPrice[0];
				titleText.SetKeyString("BuyInGameDlg_Bomb");
				itemDescriptionText.SetKeyString("BuyInGameDlg_BombDetails");
				break;
			case DropType.ColorBomb:
				ShopInfo = "ColorBomb";
				coinCostNumber = GeneralConfig.ItemBuyPrice[1];
				titleText.SetKeyString("BuyInGameDlg_Crown");
				itemDescriptionText.SetKeyString("BuyInGameDlg_CrownDetails");
				break;
			case DropType.DoubleBee:
				ShopInfo = "DoubleBee";
				coinCostNumber = GeneralConfig.ItemBuyPrice[2];
				titleText.SetKeyString("BuyInGameDlg_DoubleBees");
				itemDescriptionText.SetKeyString("BuyInGameDlg_DoubleBeesDetails");
				break;
			case DropType.Spoon:
				ShopInfo = "Spoon";
				coinCostNumber = GeneralConfig.ItemBuyPrice[3];
				titleText.SetKeyString("BuyInGameDlg_Spoon");
				itemDescriptionText.SetKeyString("BuyInGameDlg_SpoonDetails");
				break;
			case DropType.Hammer:
				ShopInfo = "Hammer";
				coinCostNumber = GeneralConfig.ItemBuyPrice[4];
				titleText.SetKeyString("BuyInGameDlg_MagicMallet");
				itemDescriptionText.SetKeyString("BuyInGameDlg_MagicMalletDetails");
				break;
			case DropType.Glove:
				ShopInfo = "Glove";
				coinCostNumber = GeneralConfig.ItemBuyPrice[5];
				titleText.SetKeyString("BuyInGameDlg_Glove");
				itemDescriptionText.SetKeyString("BuyInGameDlg_GloveDetails");
				break;
			}
			coinCost.text = coinCostNumber.ToString();
			StartCoroutine(ShowEffect());
		}

		private IEnumerator ShowEffect()
		{
			yield return null;
			Renderer[] componentsInChildren = effect1.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].material.HasProperty("_TintColor"))
				{
					Color color = componentsInChildren[i].material.GetColor("_TintColor");
					color.a = 0f;
					componentsInChildren[i].material.SetColor("_TintColor", color);
					Color color2 = componentsInChildren[i].material.GetColor("_TintColor");
					color2.a = 0.5f;
					componentsInChildren[i].material.DOColor(color2, "_TintColor", 3f);
				}
			}
			Renderer[] componentsInChildren2 = effect2.gameObject.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].material.HasProperty("_TintColor"))
				{
					Color color3 = componentsInChildren2[j].material.GetColor("_TintColor");
					color3.a = 0f;
					componentsInChildren2[j].material.SetColor("_TintColor", color3);
					Color color4 = componentsInChildren[j].material.GetColor("_TintColor");
					color4.a = 0.5f;
					componentsInChildren2[j].material.DOColor(color4, "_TintColor", 3f);
				}
			}
		}

		public void BuyBtnCLick()
		{
			if (UserDataManager.Instance.GetService().coin >= coinCostNumber)
			{
				UserDataManager.Instance.GetService().coin -= coinCostNumber;
				AddItem();
				UserDataManager.Instance.Save();
				SendBuyBoosterUmengAnalytic();
				if (UserDataManager.Instance.GetService().coin < GeneralConfig.SendNoLifeLevelDataCoinNumber && !UserDataManager.Instance.GetService().NoGoldLevelSend && UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("NoGoldLevel", UserDataManager.Instance.GetService().level.ToString());
					Analytics.Event("NoGoldLevel", dictionary);
					Analytics.Event("NoGoldLevelCalculate", dictionary, UserDataManager.Instance.GetService().level);
					UserDataManager.Instance.GetService().NoGoldLevelSend = true;
					UserDataManager.Instance.Save();
				}
				if (buyIndex < DropType.DoubleBee)
				{
					BtnCloseClicked();
					UserDataManager.Instance.GetService().boosterPurchaseData[(int)(buyIndex - 1)] += 3;
					DialogManagerTemp.Instance.ShowDialog(DialogType.EnterGameDlg);
				}
				else
				{
					GameSceneUIManager.Instance.ShowItem();
					BtnCloseClicked();
				}
			}
			else if (SceneManager.GetActiveScene().name != "CastleScene")
			{
				DialogManagerTemp.Instance.OpenShopDlg("BuyInEnterGamePlay");
			}
			else
			{
				DialogManagerTemp.Instance.OpenShopDlg("BuyInGameSceneBooster");
			}
		}

		private void SendBuyBoosterUmengAnalytic()
		{
			if (buyIndex == DropType.AreaBomb)
			{
				GA.Buy("TNT", 3, coinCostNumber / 3);
			}
			else if (buyIndex == DropType.ColorBomb)
			{
				GA.Buy("Crown", 3, coinCostNumber / 3);
			}
			else if (buyIndex == DropType.DoubleBee)
			{
				GA.Buy("Bee", 3, coinCostNumber / 3);
			}
			else if (buyIndex == DropType.Spoon)
			{
				GA.Buy("Spoon", 3, coinCostNumber / 3);
			}
			else if (buyIndex == DropType.Hammer)
			{
				GA.Buy("Hammer", 3, coinCostNumber / 3);
			}
			else if (buyIndex == DropType.Glove)
			{
				GA.Buy("Glove", 3, coinCostNumber / 3);
			}
		}

		private void AddItem()
		{
			if (buyIndex == DropType.AreaBomb)
			{
				UserDataManager.Instance.GetService().bombNumber += buyItemNumber;
			}
			else if (buyIndex == DropType.ColorBomb)
			{
				UserDataManager.Instance.GetService().rainBowBallNumber += buyItemNumber;
			}
			else if (buyIndex == DropType.DoubleBee)
			{
				UserDataManager.Instance.GetService().doubleBeesNumber += buyItemNumber;
			}
			else if (buyIndex == DropType.Spoon)
			{
				UserDataManager.Instance.GetService().malletNumber += buyItemNumber;
			}
			else if (buyIndex == DropType.Hammer)
			{
				UserDataManager.Instance.GetService().magicMalletNumber += buyItemNumber;
			}
			else if (buyIndex == DropType.Glove)
			{
				UserDataManager.Instance.GetService().gloveNumber += buyItemNumber;
			}
		}

		public void BtnCloseClicked()
		{
			HideEffect();
			if (buyIndex < DropType.DoubleBee)
			{
				StartCoroutine(ShowEnterGameDlg());
			}
			DialogManagerTemp.Instance.CloseDialog(DialogType.BuyInGameDlg);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}

		public void HideEffect()
		{
			Renderer[] componentsInChildren = effect1.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].material.HasProperty("_TintColor"))
				{
					Color color = componentsInChildren[i].material.GetColor("_TintColor");
					color.a = 0.5f;
					componentsInChildren[i].material.SetColor("_TintColor", color);
					Color color2 = componentsInChildren[i].material.GetColor("_TintColor");
					color2.a = 0f;
					componentsInChildren[i].material.DOColor(color2, "_TintColor", hidingAnimation.length);
				}
			}
			Renderer[] componentsInChildren2 = effect2.gameObject.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].material.HasProperty("_TintColor"))
				{
					Color color3 = componentsInChildren2[j].material.GetColor("_TintColor");
					color3.a = 0.5f;
					componentsInChildren2[j].material.SetColor("_TintColor", color3);
					Color color4 = componentsInChildren[j].material.GetColor("_TintColor");
					color4.a = 0f;
					componentsInChildren2[j].material.DOColor(color4, "_TintColor", hidingAnimation.length);
				}
			}
		}

		private IEnumerator ShowEnterGameDlg()
		{
			yield return new WaitForSeconds(hidingAnimation.length);
			DialogManagerTemp.Instance.ShowDialog(DialogType.EnterGameDlg);
		}
	}
}
