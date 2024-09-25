using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class ItemSelect : ItemAnim
{
	private bool isBuildFinish;

	private Tweener selectShowTweener;

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
			effectGameObject = Object.Instantiate(effectObj);
			effectGameObject.transform.SetParent(base.transform);
			effectGameObject.transform.localPosition = imageArray[selectImage].transform.localPosition + effectPositionOffset;
			effectGameObject.transform.localScale = imageArray[selectImage].transform.localScale;
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
		yield return new WaitUntil(() => isAnimFinish);
		ShowImage(selectImage);
		PlotItemAniManager.Instance.FinishStep();
	}

	public override IEnumerator WaitForChangeImage()
	{
		yield return new WaitUntil(() => isAnimFinish);
		ActiveSelectObject(selectImage);
		PlotItemAniManager.Instance.FinishStep();
	}

	public override void FinishBuild()
	{
		selectShowTweener.Kill();
		isBuildFinish = true;
	}

	public override void ShowImage(int index)
	{
		if (selectShowTweener != null)
		{
			selectShowTweener.Kill();
		}
		if (index != -1)
		{
			imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
			originalImage.gameObject.SetActive(false);
			for (int i = 0; i < imageArray.Length; i++)
			{
				imageArray[i].SetActive(false);
				if (imageArray[i].GetComponent<Animator>() != null && imageArray[i].GetComponent<Animator>().enabled)
				{
					imageArray[i].GetComponent<Animator>().enabled = false;
				}
			}
			imageArray[index].SetActive(true);
		}
		else
		{
			originalImage.gameObject.SetActive(true);
			for (int j = 0; j < imageArray.Length; j++)
			{
				imageArray[j].SetActive(false);
			}
		}
		if (effectGameObject != null)
		{
			Object.Destroy(effectGameObject);
		}
	}

	public void HideAllImage()
	{
		for (int i = 0; i < imageArray.Length; i++)
		{
			imageArray[i].SetActive(false);
		}
	}

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
		ShowImage(index);
		selectImage = index;
		if (selectShowTweener != null)
		{
			selectShowTweener.Kill();
		}
		imageArray[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		selectShowTweener = imageArray[index].GetComponent<SpriteRenderer>().DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.6f).SetLoops(-1, LoopType.Yoyo);
		selectShowTweener.Play();
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
				if (imageArray[i].GetComponent<Animator>() != null)
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
