using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DecreaseScroll : MonoBehaviour
{
	private Vector3 startPosition;

	private Color originalColor;

	private void Awake()
	{
		startPosition = base.transform.position;
		originalColor = base.transform.Find("Text").GetComponent<Text>().color;
	}

	private void OnEnable()
	{
		StartCoroutine(ImageAnim());
	}

	private IEnumerator ImageAnim()
	{
		base.transform.Find("Image").GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		base.transform.Find("Text").GetComponent<Text>().color = originalColor;
		base.transform.position = startPosition;
		base.transform.DOLocalMoveY(30f, 1f);
		yield return new WaitForSeconds(0.5f);
		base.transform.Find("Image").GetComponent<Image>().DOFade(0f, 0.5f);
		base.transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.5f);
	}
}
