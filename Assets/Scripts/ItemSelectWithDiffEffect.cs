using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class ItemSelectWithDiffEffect : ItemAnim
{
	public Vector3[] positionOffset;

	public GameObject[] effectArray;

	public bool isSpecialLocateAnim;

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
		base.transform.GetComponent<Item>();
		effectIsChild = false;
		SpriteRenderer[] componentsInChildren = imageArray[selectImage].transform.GetComponentsInChildren<SpriteRenderer>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (j == 0)
			{
				effectGameObject = Object.Instantiate(effectArray[selectImage]);
				effectGameObject.transform.SetParent(base.transform);
				effectGameObject.transform.localPosition = imageArray[selectImage].transform.localPosition + positionOffset[selectImage];
				ChangeTexture component = effectGameObject.GetComponent<ChangeTexture>();
				if (component != null && j == 0)
				{
					component.Enter(base.transform, true);
				}
				else
				{
					component.Enter(base.transform, false);
				}
			}
		}
		yield return new WaitUntil(() => isAnimFinish);
		if (isImageAfterEffectShow)
		{
			ShowImage(selectImage);
		}
		PlotItemAniManager.Instance.FinishStep();
	}

	public override void FinishBuild()
	{
		isBuildFinish = true;
	}

	public override void ShowImage(int index)
	{
		for (int i = 0; i < selectShowTweener.Count; i++)
		{
			selectShowTweener[i].Kill();
		}
		selectShowTweener.Clear();
		if (index != -1)
		{
			imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			SpriteRenderer[] componentsInChildren = imageArray[index].transform.GetComponentsInChildren<SpriteRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].color = new Color(1f, 1f, 1f, 1f);
			}
			originalImage.gameObject.SetActive(false);
			for (int k = 0; k < imageArray.Length; k++)
			{
				if (k != index)
				{
					imageArray[k].SetActive(false);
					imageArray[k].transform.parent.gameObject.SetActive(true);
					if (imageArray[k].transform.parent.GetComponent<Animator>() != null && imageArray[k].transform.parent.GetComponent<Animator>().enabled)
					{
						imageArray[k].transform.parent.GetComponent<Animator>().enabled = false;
					}
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
		if (effectGameObject != null)
		{
			Object.Destroy(effectGameObject);
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
				if (isSpecialLocateAnim)
				{
					if (imageArray[i].transform.parent.GetComponent<Animator>() != null && !imageArray[i].transform.parent.GetComponent<Animator>().enabled)
					{
						imageArray[i].transform.parent.GetComponent<Animator>().enabled = true;
						imageArray[i].transform.parent.GetComponent<Animator>().Play(0);
					}
				}
				else if (imageArray[i].GetComponent<Animator>() != null)
				{
					imageArray[i].GetComponent<Animator>().enabled = true;
				}
			}
			else if (isSpecialLocateAnim)
			{
				if (imageArray[i].transform.parent.GetComponent<Animator>() != null)
				{
					imageArray[i].transform.parent.GetComponent<Animator>().enabled = false;
				}
			}
			else if (imageArray[i].GetComponent<Animator>() != null)
			{
				imageArray[i].GetComponent<Animator>().enabled = false;
			}
		}
	}
}
