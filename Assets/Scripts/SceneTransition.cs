using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
	public float max;

	public float min;

	public float transVelo;

	private float x;

	private float y;

	private Transform maskTransform;

	private GameObject bg;

	private Color TotalTrans = new Color(0f, 0f, 0f, 0f);

	private Color NoTrans = new Color(0f, 0f, 0f, 1f);

	private static SceneTransition instance;

	public static SceneTransition Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		bg = base.transform.Find("MaskBackground").gameObject;
		maskTransform = base.transform.Find("Mask");
	}

	private void Start()
	{
	}

	public void PlayFadeOutAnim()
	{
		StartCoroutine(TransFadeOut());
	}

	public void PlayFadeInAnim()
	{
		StartCoroutine(TransFadeIn());
	}

	private IEnumerator TransFadeOut()
	{
		Image bgImg = bg.GetComponent<Image>();
		float t = 0f;
		DebugUtils.Log(DebugType.Other, "Transition bg transparency: " + bgImg.color.a);
		while (bgImg.color.a < 1f)
		{
			bgImg.color = Color.Lerp(TotalTrans, NoTrans, t);
			t += transVelo * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator TransFadeIn()
	{
		Image bgImg = bg.GetComponent<Image>();
		float t = 0f;
		while (bgImg.color.a > 0f)
		{
			bgImg.color = Color.Lerp(NoTrans, TotalTrans, t);
			t += transVelo * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator SceneTransFadeOut()
	{
		x = max;
		y = max;
		float t = 0f;
		bool played = false;
		while (!played)
		{
			if ((x <= min) | (y <= min))
			{
				played = true;
			}
			x = Mathf.Lerp(max, min, t);
			y = Mathf.Lerp(max, min, t);
			maskTransform.localScale = new Vector3(x, y, 1f);
			t += transVelo * Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator SceneTransFadeIn()
	{
		float t = 0f;
		bool played = false;
		while (!played)
		{
			if ((x >= max) | (y >= max))
			{
				played = true;
			}
			x = Mathf.Lerp(min, max, t);
			y = Mathf.Lerp(min, max, t);
			maskTransform.localScale = new Vector3(x, y, 1f);
			t += transVelo * Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
	}
}
