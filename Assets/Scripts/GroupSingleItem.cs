using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GroupSingleItem : MonoBehaviour
{
	public GameObject[] imageArray;

	public Animator effectAnimator;

	public GameObject effect;

	public GameObject originalImage;

	public AudioClip audioSource;

	public bool isShake;

	public int selectImage;

	public bool isEffectAlwayShow;

	public bool isFadeIn;

	public float fadeInTime = 1.5f;

	public bool isNotHideSelectedImage;

	[HideInInspector]
	public GameObject effectGameObject;

	public bool notHide;

	public bool selectFinishNotShowOriginal;

	private Tweener selectShowTweener;

	public virtual void Enter(int index, bool isSelect = false)
	{
		base.gameObject.SetActive(true);
		ShowImage(index, isSelect);
		if (effectAnimator == null)
		{
			effectAnimator = imageArray[0].GetComponent<Animator>();
		}
		effectAnimator.enabled = false;
	}

	public void HideImage()
	{
		if (selectFinishNotShowOriginal)
		{
			originalImage.gameObject.SetActive(false);
		}
		else
		{
			originalImage.gameObject.SetActive(true);
		}
		for (int i = 0; i < imageArray.Length; i++)
		{
			imageArray[i].SetActive(false);
		}
	}

	public void ShowImage(int index, bool isSelect = false)
	{
		selectImage = index;
		if (index != -1)
		{
			originalImage.gameObject.SetActive(false);
			for (int i = 0; i < imageArray.Length; i++)
			{
				if (imageArray[i] != null)
				{
					imageArray[i].SetActive(false);
				}
			}
			imageArray[index].SetActive(true);
			if (isSelect)
			{
				if (selectShowTweener != null)
				{
					selectShowTweener.Kill();
				}
				imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				selectShowTweener = imageArray[index].GetComponent<SpriteRenderer>().DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.6f).SetLoops(-1, LoopType.Yoyo);
				selectShowTweener.Play();
				if (isShake)
				{
					PlaySelectAnim(index);
				}
			}
			else
			{
				if (selectShowTweener != null)
				{
					selectShowTweener.Kill();
				}
				imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				for (int j = 0; j < imageArray.Length; j++)
				{
					if (isShake && imageArray[j].GetComponent<Animator>() != null && imageArray[j].GetComponent<Animator>().enabled)
					{
						imageArray[j].GetComponent<Animator>().enabled = false;
					}
				}
			}
		}
		else
		{
			originalImage.gameObject.SetActive(true);
			for (int k = 0; k < imageArray.Length; k++)
			{
				imageArray[k].SetActive(false);
			}
		}
		if (effectGameObject != null)
		{
			Object.Destroy(effectGameObject);
		}
	}

	public void PlayEffect()
	{
		StartCoroutine(StartPlayEffect());
	}

	public virtual IEnumerator StartPlayEffect()
	{
		if (!notHide)
		{
			originalImage.gameObject.SetActive(false);
		}
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (imageArray[i] != null)
			{
				imageArray[i].SetActive(false);
			}
		}
		if (effectAnimator != null)
		{
			effectAnimator.enabled = true;
		}
		if (audioSource != null)
		{
			AudioManager.Instance.PlayAudioEffect(audioSource.name);
		}
		if (!isEffectAlwayShow)
		{
			ShowImage(selectImage);
		}
		if (effect != null)
		{
			effect.gameObject.SetActive(true);
		}
		AnimatorStateInfo animatorStateInfo = default(AnimatorStateInfo);
		if (effectAnimator != null)
		{
			animatorStateInfo = effectAnimator.GetCurrentAnimatorStateInfo(0);
		}
		else
		{
			ShowImage(selectImage);
			animatorStateInfo = imageArray[0].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		}
		if (isFadeIn)
		{
			DealImageFadeIn();
			yield return new WaitForSeconds(fadeInTime);
		}
		else
		{
			yield return new WaitForSeconds(animatorStateInfo.length);
		}
		if (!isEffectAlwayShow)
		{
			if (effectAnimator != null)
			{
				effectAnimator.enabled = false;
			}
			imageArray[selectImage].SetActive(true);
			if (effect != null)
			{
				effect.gameObject.SetActive(false);
			}
			if (isNotHideSelectedImage)
			{
				ShowWithNotHide(selectImage);
			}
			else
			{
				ShowImage(selectImage);
			}
			PlotItemAniManager.Instance.FinishStep();
		}
	}

	private void ShowWithNotHide(int index)
	{
		originalImage.gameObject.SetActive(false);
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (imageArray[i] != null && i != index)
			{
				imageArray[i].SetActive(false);
			}
		}
		imageArray[index].SetActive(true);
		selectImage = index;
	}

	private void DealImageFadeIn()
	{
		SpriteRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = new Color(1f, 1f, 1f, 0f);
			componentsInChildren[i].DOFade(1f, fadeInTime);
		}
	}

	public void PlaySelectAnim(int index)
	{
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (i == index)
			{
				if (imageArray[i].GetComponent<Animator>() != null && !imageArray[i].GetComponent<Animator>().enabled)
				{
					imageArray[i].GetComponent<Animator>().enabled = true;
				}
			}
			else if (imageArray[i].GetComponent<Animator>() != null)
			{
				imageArray[i].GetComponent<Animator>().enabled = false;
			}
		}
	}
}
