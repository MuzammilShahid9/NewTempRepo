using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Core.UI
{
	public class PurchaseSuccessDlg : BaseDialog
	{
		private static PurchaseSuccessDlg instance;

		public GameObject successNode;

		public GameObject failNode;

		public GameObject successEffect;

		public Transform LifeEndPos;

		public Transform GoldEndPos;

		public Transform DropEndPos;

		public Transform LifeMidPos;

		public Transform GoldMidPos;

		public Transform DropsMidPos;

		public Transform StartPos;

		public GameObject ObjPrefab;

		public GameObject tailEffect;

		public GameObject BankEffect;

		public LocalizationText titleText;

		public Image purchaseItem;

		private ShopConfigData shopConfigData;

		private string[] imageNameArray = new string[8] { "Elements2/jinbi", "GameElements/area_bomb", "GameElements/color_bomb", "Elements2/DoubleBee", "Elements2/Spoon", "Elements2/hammer", "Elements2/glove", "Elements2/wuxianxin" };

		private Vector3 StartPosV3;

		private Vector3 GoldMidPosV3;

		private Vector3 GoldEndPosV3;

		private Vector3 DropsMidPosV3;

		private Vector3 DropsEndPosV3;

		private Vector3 LifeMidPosV3;

		private Vector3 LifeEndPosV3;

		public static PurchaseSuccessDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Start()
		{
			base.Start();
			StartPosV3 = StartPos.position;
			GoldMidPosV3 = GoldMidPos.position;
			GoldEndPosV3 = GoldEndPos.position;
			DropsMidPosV3 = DropsMidPos.position;
			DropsEndPosV3 = DropEndPos.position;
			LifeMidPosV3 = LifeMidPos.position;
			LifeEndPosV3 = LifeEndPos.position;
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.PurchaseSuccessDlg);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			if (obj != null)
			{
				shopConfigData = (ShopConfigData)obj;
				DebugUtils.Log(DebugType.Other, shopConfigData.Goods + " | coins : " + shopConfigData.GoldNum + "   Successful!");
			}
			else
			{
				shopConfigData = null;
			}
			if (GlobalVariables.PurchaseSuccess)
			{
				successNode.SetActive(true);
				failNode.SetActive(false);
				successEffect.SetActive(true);
				titleText.SetKeyString("PurchaseSuccessDlg_Congratulations");
				AudioManager.Instance.PlayAudioEffect("coin_collect");
				BankEffect.SetActive(GlobalVariables.isBank);
				if (GlobalVariables.isBank)
				{
					Timer.Schedule(this, 0.5f, delegate
					{
						AudioManager.Instance.PlayAudioEffect("KittyBankPurchase", 0.1f);
					});
				}
			}
			else
			{
				failNode.SetActive(true);
				successEffect.SetActive(false);
				successNode.SetActive(false);
				titleText.SetKeyString("PurchaseSuccessDlg_Canceled");
			}
			string text = "mao";
			text = (GlobalVariables.isBank ? "mao" : (GlobalVariables.isSale ? ShopConfig.saleConfig[GlobalVariables.PurchasingID - 1].ImageName : ShopConfig.shopConfig[GlobalVariables.PurchasingID - 1].ImageName));
			Sprite sprite = Resources.Load<GameObject>("Textures/Elements2/" + text).GetComponent<SpriteRenderer>().sprite;
			purchaseItem.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
			purchaseItem.GetComponent<Canvas>().sortingLayerName = "UI";
			purchaseItem.sprite = sprite;
			if (GlobalVariables.PurchaseSuccess)
			{
				purchaseItem.gameObject.SetActive(!GlobalVariables.isBank);
			}
			else
			{
				purchaseItem.gameObject.SetActive(true);
			}
		}

		public void BtnCloseClicked()
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.PurchaseSuccessDlg);
		}

		private void GetGoods(int goodsID, int goodsNum)
		{
			GameObject go = Object.Instantiate(ObjPrefab, StartPosV3, Quaternion.identity, base.transform.parent);
			Image componentInChildren = go.GetComponentInChildren<Image>();
			go.GetComponentInChildren<Text>().enabled = false;
			Object.Instantiate(tailEffect.gameObject, go.transform);
			go.transform.position = StartPosV3;
			Sprite sprite = Resources.Load<GameObject>("Textures/" + imageNameArray[goodsID]).GetComponent<SpriteRenderer>().sprite;
			if (goodsID != 7)
			{
				ShortcutExtensions.DOPath(path: new Vector3[3] { StartPosV3, DropsMidPosV3, DropsEndPosV3 }, target: go.transform, duration: 0.8f, pathType: PathType.CatmullRom).OnComplete(delegate
				{
					if (CastleSceneUIManager.Instance != null)
					{
						CastleSceneUIManager.Instance.EnterGameBtnScale();
					}
					Object.Destroy(go);
				});
				componentInChildren.sprite = sprite;
				componentInChildren.rectTransform.sizeDelta = new Vector2(150f, 150f);
				return;
			}
			ShortcutExtensions.DOPath(path: new Vector3[3] { StartPosV3, LifeMidPosV3, LifeEndPosV3 }, target: go.transform, duration: 0.8f, pathType: PathType.CatmullRom).OnComplete(delegate
			{
				if (CastleSceneUIManager.Instance != null)
				{
					CastleSceneUIManager.Instance.LifeImageScale();
				}
				Object.Destroy(go);
			});
			componentInChildren.sprite = sprite;
			componentInChildren.rectTransform.sizeDelta = new Vector2(150f, 150f);
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClicked();
		}
	}
}
