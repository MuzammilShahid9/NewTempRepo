using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Core
{
	public class BaseDialog : BaseUI
	{
		public Animator anim;

		public AnimationClip hidingAnimation;

		private UnityAction closeAction;

		public bool isAniming;

		public DialogLevel level;

		public DialogType type;

		public bool isOpenning;

		public bool isCanClose = true;

		public bool isCanOpen = true;

		public DialogType returnType;

		public bool isDestroy;

		private bool isHide;

		protected virtual void Awake()
		{
			if (anim == null)
			{
				anim = GetComponent<Animator>();
			}
			ProcessTexts();
		}

		protected new virtual void Start()
		{
			Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].onClick.AddListener(delegate
				{
					AudioManager.Instance.PlayAudioEffect("general_button");
				});
			}
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		public virtual void ChangeLangage(uint iMessageType, object arg)
		{
		}

		public virtual void Show()
		{
			DebugUtils.Log(DebugType.UI, "show dialog");
			base.gameObject.SetActive(true);
			base.transform.SetAsLastSibling();
			if (anim != null)
			{
				isAniming = true;
				isCanOpen = false;
				isCanClose = false;
				anim.SetTrigger("show");
				AudioManager.Instance.PlayAudioEffect("window_open");
				Timer.Schedule(this, hidingAnimation.length, delegate
				{
					isCanOpen = false;
					isCanClose = true;
					isAniming = false;
				});
			}
			else
			{
				AudioManager.Instance.PlayAudioEffect("window_open");
				isAniming = false;
				isCanOpen = false;
				isCanClose = true;
			}
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
		}

		public virtual void Show(object obj)
		{
			if (SceneManager.GetActiveScene().name == "CastleScene")
			{
				if (obj != null && Convert.ToString(obj).Equals("HideDown"))
				{
					CastleSceneUIManager.Instance.HideTaskAndEnterGameBtn();
				}
				else
				{
					CastleSceneUIManager.Instance.HideAllBtn();
				}
			}
			DebugUtils.Log(DebugType.UI, "show dialog");
			base.gameObject.SetActive(true);
			base.transform.SetAsLastSibling();
			if (anim != null)
			{
				isAniming = true;
				isCanOpen = false;
				isCanClose = false;
				anim.SetTrigger("show");
				AudioManager.Instance.PlayAudioEffect("window_open");
				Timer.Schedule(this, hidingAnimation.length, delegate
				{
					isCanOpen = false;
					isCanClose = true;
					isAniming = false;
				});
			}
			else
			{
				AudioManager.Instance.PlayAudioEffect("window_open");
				isAniming = false;
				isCanOpen = false;
				isCanClose = true;
			}
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
		}

		public virtual void PressEsc(uint iMessageType, object arg)
		{
			DialogManagerTemp.Instance.CloseDialog(type);
		}

		public void Recover()
		{
			Show();
		}

		public virtual void Close(bool isAnim = true, bool isShowAllBtn = true)
		{
			if (anim != null && hidingAnimation != null && isAnim)
			{
				DebugUtils.Log(DebugType.UI, "close with anim");
				anim.SetTrigger("hide");
				AudioManager.Instance.PlayAudioEffect("window_close");
				isAniming = true;
				isCanOpen = false;
				isCanClose = false;
				Timer.Schedule(this, hidingAnimation.length, Hide);
			}
			else
			{
				isAniming = true;
				isCanOpen = false;
				isCanClose = false;
				DebugUtils.Log(DebugType.UI, "close instantly");
				Hide();
			}
		}

		public virtual void Hide()
		{
			isAniming = false;
			isCanOpen = true;
			isCanClose = true;
			base.gameObject.SetActive(false);
			if (anim != null)
			{
				anim.transform.localScale = Vector3.one;
			}
		}

		public virtual void BeCover(DialogType type)
		{
			Hide();
		}
	}
}
