using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CinemaDirector : MonoBehaviour
	{
		private static CinemaDirector instance;

		public GameObject infoBanner;

		public Text infoText;

		public static CinemaDirector Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			Color color = infoBanner.transform.GetComponent<Image>().color;
			infoBanner.transform.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0f);
			color = infoText.color;
			infoText.color = new Color(color.r, color.g, color.b, 0f);
			infoBanner.SetActive(false);
		}

		public void ShowText(string str)
		{
			infoBanner.SetActive(true);
			infoText.text = str;
			infoText.DOFade(1f, 0.4f);
			Sequence sequence = DOTween.Sequence();
			sequence.Append(infoBanner.GetComponent<Image>().DOFade(0.5f, 0.4f));
			sequence.OnComplete(delegate
			{
				StartCoroutine("HideText");
			});
		}

		private IEnumerator HideText()
		{
			yield return new WaitForSeconds(1.2f);
			infoBanner.GetComponent<Image>().DOFade(0f, 0.4f);
			infoText.DOFade(0f, 0.4f);
			infoBanner.SetActive(false);
		}
	}
}
