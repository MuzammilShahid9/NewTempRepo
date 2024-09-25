using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayInfinity.GameEngine.Libs.Common
{
	public class ButtonUtilities : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public float disableDuration = 2f;

		public bool hasCustomCallback;

		public bool playDefaultSound;

		[HideInInspector]
		public Material mat;

		public bool clicking;

		public bool hidingMat;

		public Button btn;

		private Image btnImage;

		private void Awake()
		{
			btn = base.gameObject.GetComponent<Button>();
			btnImage = GetComponent<Image>();
			if (btnImage.material != null && hidingMat)
			{
				mat = btnImage.material;
				ShowEffect(false);
			}
			btn.onClick.AddListener(delegate
			{
				if (playDefaultSound)
				{
					AddClickAudioSource();
				}
				if (!hasCustomCallback)
				{
					PreventMultiTap();
				}
			});
		}

		private void Start()
		{
			ChangeBtnDisabledColor();
		}

		private void AddClickAudioSource()
		{
		}

		private void ChangeBtnDisabledColor()
		{
			if (btn.transition == Selectable.Transition.ColorTint)
			{
				ColorBlock colors = btn.colors;
				colors.disabledColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				btn.colors = colors;
			}
		}

		private void PreventMultiTap()
		{
			btn.interactable = false;
			Invoke("ResumeClickable", disableDuration);
		}

		private void ResumeClickable()
		{
			btn.interactable = true;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			clicking = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			clicking = false;
		}

		public void ShowEffect(bool isShow)
		{
			if (isShow)
			{
				if (mat != null)
				{
					btnImage.material = mat;
				}
			}
			else if (btnImage.material != null)
			{
				btnImage.material = null;
			}
		}
	}
}
