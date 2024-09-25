using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayInfinity.AliceMatch3.Core
{
	public class DialogManagerTemp : MonoBehaviour
	{
		public Dictionary<DialogType, BaseDialog> dialogRegisterDic = new Dictionary<DialogType, BaseDialog>();

		public Transform DlgParent;

		public GameObject DlgMask;

		private static DialogManagerTemp instance;

		public string SceneName = "";

		public Stack<BaseDialog> ReturnDialogStack = new Stack<BaseDialog>();

		public List<BaseDialog> OpenningDialog = new List<BaseDialog>();

		public static DialogManagerTemp Instance
		{
			get
			{
				return instance;
			}
		}

		public void Awake()
		{
			instance = this;
			SceneName = SceneManager.GetActiveScene().name;
			SceneManager.sceneLoaded += loadedEve;
			DlgMask.SetActive(false);
		}

		private void loadedEve(Scene s, LoadSceneMode l)
		{
			if (SceneName != s.name)
			{
				SceneName = s.name;
			}
			UpdateCanvasCamera();
		}

		public void UpdateCanvasCamera()
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
			if (gameObject != null)
			{
				GetComponent<Canvas>().worldCamera = gameObject.GetComponent<Camera>();
				GetComponent<Canvas>().planeDistance = 3.5f;
				GetComponent<Canvas>().sortingLayerName = "UI";
				GetComponent<Canvas>().sortingOrder = UIConfig.DialogSortingOrder;
				GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
			}
		}

		public void MaskAllDlg()
		{
			GlobalVariables.IsMaskDialog = true;
			DlgMask.SetActive(true);
		}

		public void CancelMaskAllDlg()
		{
			GlobalVariables.IsMaskDialog = false;
			DlgMask.SetActive(false);
		}

		public void CreateDialog(DialogType type)
		{
			if (!dialogRegisterDic.ContainsKey(type) && type != 0)
			{
				DebugUtils.Log(DebugType.UI, "create dialog " + Enum.GetName(typeof(DialogType), type));
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Dialogs/" + Enum.GetName(typeof(DialogType), type)));
				gameObject.name = gameObject.name.Replace("(Clone)", "");
				gameObject.transform.SetParent(DlgParent, false);
				gameObject.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
				gameObject.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
				dialogRegisterDic.Add(type, gameObject.GetComponent<BaseDialog>());
				gameObject.GetComponent<BaseDialog>().type = type;
				gameObject.GetComponent<BaseDialog>().name = type.ToString();
				gameObject.GetComponent<BaseDialog>().level = DialogLevel.window;
				gameObject.SetActive(false);
			}
		}

		public void ShowDialog(DialogType type, object obj = null, DialogType closeType = DialogType.None)
		{
			DebugUtils.Log(DebugType.Other, type.ToString() + DialogType.None);
			DebugUtils.Assert(type != DialogType.None, "type is null !");
			string text = type.ToString();
			DebugUtils.Log(DebugType.UI, "show dialog " + text);
			BaseDialog value = null;
			if (dialogRegisterDic.TryGetValue(type, out value))
			{
				if (!value.isCanOpen)
				{
					DebugUtils.Log(DebugType.UI, text + " is Anim Openning , so cant be Open!");
					return;
				}
				if (value.isOpenning)
				{
					DebugUtils.Log(DebugType.UI, text + " is Openning , so cant be Open!");
					return;
				}
				if (closeType == DialogType.None && ReturnDialogStack.Count != 0)
				{
					BaseDialog baseDialog = ReturnDialogStack.Peek();
					baseDialog.isOpenning = false;
					for (int num = OpenningDialog.Count - 1; num >= 0; num--)
					{
						if (OpenningDialog[num].type == type)
						{
							OpenningDialog.RemoveAt(num);
							DebugUtils.Log(DebugType.UI, text + " be Removed From OpenningList !");
						}
					}
					baseDialog.BeCover(type);
					DebugUtils.Log(DebugType.UI, baseDialog.name + " be Cover !");
				}
				if (value.level == DialogLevel.window && (ReturnDialogStack.Count == 0 || ReturnDialogStack.Peek().type != value.type))
				{
					ReturnDialogStack.Push(value);
					DebugUtils.Log(DebugType.UI, text + " be push in returnList!");
				}
				if (closeType != 0)
				{
					CloseDialog(closeType, false, false);
				}
				value.isOpenning = true;
				OpenningDialog.Add(value);
				value.Show(obj);
				Singleton<MessageDispatcher>.Instance().SendMessage(37u, type);
				DebugUtils.Log(DebugType.UI, text + " has been opened !");
			}
			else
			{
				DebugUtils.Log(DebugType.UI, text + " is not be create !");
			}
		}

		public void RecoveryDialog(DialogType type)
		{
			string text = type.ToString();
			DebugUtils.Log(DebugType.UI, "Try to Recovery UI : " + text);
			BaseDialog value = null;
			if (dialogRegisterDic.TryGetValue(type, out value))
			{
				if (!value.isCanOpen)
				{
					DebugUtils.Log(DebugType.UI, text + " is Anim Openning , so cant be Open!");
					return;
				}
				value.isOpenning = true;
				OpenningDialog.Add(value);
				value.Recover();
				Singleton<MessageDispatcher>.Instance().SendMessage(38u, type);
				DebugUtils.Log(DebugType.UI, text + " has been Recovery !");
			}
			else
			{
				DebugUtils.Log(DebugType.UI, text + " is not be create !");
			}
		}

		public void CloseDialog(DialogType type, bool isAnim = true, bool isCheckReturnList = true)
		{
			string text = type.ToString();
			DebugUtils.Log(DebugType.UI, "Try to Close UI : " + text);
			BaseDialog value = null;
			if (dialogRegisterDic.TryGetValue(type, out value))
			{
				if (!value.isCanClose)
				{
					DebugUtils.Log(DebugType.UI, text + " is Anim Closing , so cant be close!");
					return;
				}
				if (!value.isOpenning)
				{
					DebugUtils.Log(DebugType.UI, text + " is not Openning , so cant be close!");
					return;
				}
				if (!value.isCanClose)
				{
					DebugUtils.Log(DebugType.UI, text + " now is cant Be Closed!");
					return;
				}
				value.isOpenning = false;
				if (ReturnDialogStack.Count > 0 && ReturnDialogStack.Peek().type == type)
				{
					if (isCheckReturnList && (value.returnType > DialogType.None || ReturnDialogStack.Count - 1 > 0))
					{
						value.Close(false);
					}
					else
					{
						value.Close(isAnim);
					}
					ReturnDialogStack.Pop();
					DebugUtils.Log(DebugType.UI, text + " be removed from ReturnList!");
					if (ReturnDialogStack.Count == 0)
					{
						DebugUtils.Log(DebugType.Other, "ReturnDialogStack.Count == 0");
						Singleton<MessageDispatcher>.Instance().SendMessage(32u, null);
						Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
					}
				}
				else
				{
					value.Close(false);
				}
				for (int num = OpenningDialog.Count - 1; num >= 0; num--)
				{
					if (OpenningDialog[num].type == type)
					{
						OpenningDialog.RemoveAt(num);
						DebugUtils.Log(DebugType.UI, text + " be Removed From OpenningList !");
					}
				}
				if (isCheckReturnList)
				{
					if (value.returnType > DialogType.None)
					{
						DebugUtils.Log(DebugType.UI, string.Format("Close Dialog -{0}- and Open Dialog -{1}-", text, value.returnType.ToString()));
						ShowDialog(value.returnType);
					}
					else if (value.returnType == DialogType.None && ReturnDialogStack.Count > 0)
					{
						RecoveryDialog(ReturnDialogStack.Peek().type);
					}
				}
				if (value.isDestroy)
				{
					DebugUtils.Log(DebugType.UI, text + " Destroy !");
					DestroyDialog(type);
				}
				DebugUtils.Log(DebugType.UI, text + " Close Success!");
			}
			else
			{
				DebugUtils.Log(DebugType.UI, text + " is not be create !");
			}
		}

		private void DestroyDialog(DialogType type)
		{
			string text = type.ToString();
			DebugUtils.Log(DebugType.UI, "Try to Destroy UI : " + text);
			BaseDialog value = null;
			if (dialogRegisterDic.TryGetValue(type, out value))
			{
				UnityEngine.Object.Destroy(value.gameObject);
				dialogRegisterDic.Remove(type);
			}
		}

		public void ShowDialogAndPopAll(DialogType type, object obj = null)
		{
			DebugUtils.Log(DebugType.UI, "show dialog " + type);
			CloseAllDialogs();
			ShowDialog(type, obj);
		}

		public void CloseAllDialogs()
		{
			DebugUtils.Log(DebugType.UI, "Close All Dialog !");
			ReturnDialogStack.Clear();
			foreach (KeyValuePair<DialogType, BaseDialog> item in dialogRegisterDic)
			{
				if (item.Value.isOpenning)
				{
					CloseDialog(item.Key, false);
				}
			}
			Singleton<MessageDispatcher>.Instance().SendMessage(32u, null);
			OpenningDialog.Clear();
		}

		public bool IsDialogShowing()
		{
			return OpenningDialog.Count != 0;
		}

		public int IsDialogOpening()
		{
			return OpenningDialog.Count;
		}

		public void OpenBankDlg(float DelayTime, bool isShowTip = true, DialogType closeType = DialogType.None)
		{
			if (DelayTime < 0.05f)
			{
				Instance.ShowDialog(DialogType.BankDlg, closeType);
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				return;
			}
			Timer.Schedule(this, DelayTime, delegate
			{
				Instance.ShowDialog(DialogType.BankDlg, closeType);
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
			});
		}

		public void OpenShopDlg(string from, DialogType closeType = DialogType.None)
		{
//#if !UNITY_WSA || UNITY_EDITOR
			if (Application.internetReachability != 0 && Purchaser.Instance.IsInitialized())
            {
                Instance.ShowDialog(DialogType.ShopDlg, from, closeType);
            }
            else
            {
                Instance.ShowDialog(DialogType.LostNetworkDlg, null, closeType);
            }
//#else
//			var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
//			if (connectionProfile == null)
//			{
//				Instance.ShowDialog(DialogType.LostNetworkDlg, null, closeType);
//				//return false;
//			}
//			else
//			{
//				Instance.ShowDialog(DialogType.ShopDlg, from, closeType);
//				//return connectionProfile == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;
//			}
//#endif

		}

		public void OpenSaleDlg(float DelayTime, object obj, DialogType closeType = DialogType.None)
		{
			if (Application.internetReachability != 0 && Purchaser.Instance.IsInitialized())
			{
				if (DelayTime < 0.05f)
				{
					Instance.CancelMaskAllDlg();
					CastleSceneUIManager.Instance.CancelMaskCastleUI();
					Instance.ShowDialog(DialogType.SaleDlg, obj, closeType);
					return;
				}
				Timer.Schedule(this, DelayTime, delegate
				{
					Instance.CancelMaskAllDlg();
					CastleSceneUIManager.Instance.CancelMaskCastleUI();
					Instance.ShowDialog(DialogType.SaleDlg, obj, closeType);
				});
			}
			else
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				DebugUtils.Log(DebugType.Other, "Cant Open Sale DLg connect is " + Application.internetReachability.ToString() + "       purchase init is " + Purchaser.Instance.IsInitialized());
			}
		}

		public void OpenDailyBonusDlg(float DelayTime, object obj, DialogType closeType = DialogType.None)
		{
			if (DelayTime < 0.05f)
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.DailyBonusDlg, obj, closeType);
				return;
			}
			Timer.Schedule(this, DelayTime, delegate
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.DailyBonusDlg, obj, closeType);
			});
		}

		public void OpenGetRewardDlg(float DelayTime, object obj, DialogType closeType = DialogType.None)
		{
			if (DelayTime < 0.05f)
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.GetRewardDlg, obj, closeType);
				return;
			}
			Timer.Schedule(this, DelayTime, delegate
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.GetRewardDlg, obj, closeType);
			});
		}

		public void OpenComboDlg(float DelayTime, object obj, DialogType closeType = DialogType.None)
		{
			if (DelayTime < 0.05f)
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.ComboDlg, obj, closeType);
				return;
			}
			Timer.Schedule(this, DelayTime, delegate
			{
				Instance.CancelMaskAllDlg();
				CastleSceneUIManager.Instance.CancelMaskCastleUI();
				Instance.ShowDialog(DialogType.ComboDlg, obj, closeType);
			});
		}
	}
}
