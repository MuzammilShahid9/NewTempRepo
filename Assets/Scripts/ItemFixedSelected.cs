using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class ItemFixedSelected : ItemAnim
{
	public GameObject effect;

	public Animator effectAnimator;

	private bool isBuildFinish;

	private List<Tweener> selectShowTweener = new List<Tweener>();

	public override void Awake()
	{
		buileType = BuildType.Select;
	}

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		isBuildFinish = false;
		BuildManager.Instance.StartBuild(base.transform);
		StartCoroutine(WaitForPlayEffect(roleAnimWaitEffectTime));
	}

	private IEnumerator WaitForPlayEffect(float roleAnimWaitEffectTime)
	{
		yield return new WaitUntil(() => isBuildFinish);
		PlotManager.Instance.PlotInsertRoleAction();
		HideAllImage();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		StartCoroutine(StartPlayEffect());
	}

	public void HideAllImage()
	{
		for (int i = 0; i < imageArray.Length; i++)
		{
			imageArray[i].SetActive(false);
		}
	}

	private IEnumerator StartPlayEffect()
	{
		for (int i = 0; i < imageArray.Length; i++)
		{
			imageArray[i].SetActive(false);
		}
		effectAnimator.enabled = true;
		if (effect != null)
		{
			effect.gameObject.SetActive(true);
		}
		imageArray[selectImage].SetActive(true);
		yield return new WaitForSeconds(effectAnimator.GetCurrentAnimatorStateInfo(0).length);
		FinishAnim();
		if (effect != null)
		{
			effect.gameObject.SetActive(false);
		}
		StartCoroutine(WaitForChangeImage());
	}

	public override IEnumerator WaitForChangeImage()
	{
		yield return new WaitUntil(() => isAnimFinish);
		ActiveSelectObject(selectImage);
		PlotItemAniManager.Instance.FinishStep();
	}

	public override void FinishBuild()
	{
		isBuildFinish = true;
	}

	public override void ShowImage(int index)
	{
		if (index != -1)
		{
			for (int i = 0; i < selectShowTweener.Count; i++)
			{
				selectShowTweener[i].Kill();
			}
			selectShowTweener.Clear();
			imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			SpriteRenderer[] componentsInChildren = imageArray[index].transform.GetComponentsInChildren<SpriteRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].color = new Color(1f, 1f, 1f, 1f);
			}
			originalImage.gameObject.SetActive(false);
			for (int k = 0; k < imageArray.Length; k++)
			{
				imageArray[k].SetActive(false);
				if (imageArray[k].GetComponent<Animator>() != null && imageArray[k].GetComponent<Animator>().enabled)
				{
					imageArray[k].GetComponent<Animator>().enabled = false;
				}
			}
			imageArray[index].SetActive(true);
		}
		else
		{
			originalImage.gameObject.SetActive(true);
			for (int l = 0; l < imageArray.Length; l++)
			{
				imageArray[l].SetActive(false);
			}
		}
	}

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
		ShowImage(index);
		selectImage = index;
		for (int i = 0; i < selectShowTweener.Count; i++)
		{
			selectShowTweener[i].Kill();
		}
		selectShowTweener.Clear();
		SpriteRenderer[] componentsInChildren = imageArray[index].transform.GetComponentsInChildren<SpriteRenderer>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].color = new Color(1f, 1f, 1f, 1f);
			Tweener tweener = componentsInChildren[j].DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.6f).SetLoops(-1, LoopType.Yoyo);
			tweener.Play();
			selectShowTweener.Add(tweener);
		}
		if (!notShowSelectAnim && isShake)
		{
			PlaySelectAnim(index);
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
