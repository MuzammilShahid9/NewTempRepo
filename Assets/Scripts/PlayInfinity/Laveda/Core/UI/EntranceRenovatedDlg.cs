using System.Collections;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class EntranceRenovatedDlg : BaseDialog
	{
		private static EntranceRenovatedDlg instance;

		public Sprite[] spriteArray;

		public Sprite[] effectTextureArray;

		public Image roomNameImage;

		public Button closeBtn;

		public Button continueBtn;

		public GameObject effectObj;

		public GameObject effectParent;

		public static EntranceRenovatedDlg Instance
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

		public void Enter()
		{
			Show(null);
			int stage = UserDataManager.Instance.GetService().stage;
			base.gameObject.SetActive(true);
			closeBtn.enabled = false;
			continueBtn.enabled = false;
			roomNameImage.sprite = spriteArray[stage - 1];
			GameObject obj = Object.Instantiate(effectObj, effectParent.transform);
			obj.transform.localPosition = new Vector3(0f, 0f, 0f);
			obj.transform.Find("pingyu").Find("pingyu").gameObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture = effectTextureArray[stage - 1].texture;
			StartCoroutine(DelaySetBtnStatu());
		}

		public override void Show(object obj)
		{
			DebugUtils.Log(DebugType.Other, "show dialog");
			base.gameObject.SetActive(true);
			base.transform.SetAsLastSibling();
			if (anim != null)
			{
				isAniming = true;
				anim.SetTrigger("show");
				AudioManager.Instance.PlayAudioEffect("window_open");
				float time = 0f;
				UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time >= hidingAnimation.length)
					{
						isAniming = false;
						return true;
					}
					time += duration;
					return false;
				}));
			}
			else
			{
				isAniming = false;
			}
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
		}

		private IEnumerator DelaySetBtnStatu()
		{
			yield return new WaitForSeconds(3f);
			closeBtn.enabled = true;
			continueBtn.enabled = true;
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.EntranceRenovatedDlg);
		}

		public override void Close(bool isAnim = true, bool isShowAllBtn = true)
		{
			if (anim != null && hidingAnimation != null && isAnim)
			{
				DebugUtils.Log(DebugType.Other, "close with anim");
				anim.SetTrigger("hide");
				AudioManager.Instance.PlayAudioEffect("window_close");
				isAniming = true;
				Timer.Schedule(this, hidingAnimation.length, Hide);
			}
			else
			{
				DebugUtils.Log(DebugType.Other, "close instantly");
				Hide();
			}
		}

		public void BtnCloseClick()
		{
			Close();
			PlotManager.Instance.FinishOneCondition();
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			BtnCloseClick();
		}
	}
}
