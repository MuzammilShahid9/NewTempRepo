using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Pandora.Core.UI
{
	public class BaseUIManager : BaseUI
	{
		public Image coinImg;

		public Text coinNum;

		public Image coinBg;

		protected virtual void Awake()
		{
			if (GameObject.Find("UI") != null)
			{
				base.gameObject.transform.SetParent(GameObject.Find("UI").transform);
			}
			base.gameObject.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			base.gameObject.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 1f);
			base.gameObject.SetActive(true);
			Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].onClick.AddListener(delegate
				{
					AudioManager.Instance.PlayAudioEffect("general_button");
				});
			}
		}

		protected new virtual void Start()
		{
		}

		public virtual void ShowUI()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void HideUI()
		{
			base.gameObject.SetActive(false);
		}
	}
}
