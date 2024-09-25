using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GroupSingleDoTweenItem : GroupSingleItem
{
	public float perWaitTime;

	public bool isPlayAnim;

	public override IEnumerator StartPlayEffect()
	{
		originalImage.SetActive(false);
		for (int k = 0; k < imageArray.Length; k++)
		{
			if (imageArray[k] != null)
			{
				imageArray[k].SetActive(false);
			}
		}
		imageArray[selectImage].SetActive(true);
		if (isPlayAnim)
		{
			float waitTime = 0f;
			Animator[] childAnimatorArray = imageArray[selectImage].transform.GetComponentsInChildren<Animator>();
			for (int l = 0; l < childAnimatorArray.Length; l++)
			{
				if (childAnimatorArray[l].transform != imageArray[selectImage].transform)
				{
					childAnimatorArray[l].transform.parent.gameObject.SetActive(false);
				}
			}
			for (int j = 0; j < childAnimatorArray.Length; j++)
			{
				if (childAnimatorArray[j].transform != imageArray[selectImage].transform)
				{
					childAnimatorArray[j].transform.parent.gameObject.SetActive(true);
					childAnimatorArray[j].enabled = true;
					if (perWaitTime != 0f)
					{
						waitTime = perWaitTime;
						yield return new WaitForSeconds(waitTime);
					}
					else
					{
						waitTime = 1f;
						yield return new WaitForSeconds(waitTime);
					}
				}
			}
			yield return new WaitForSeconds(childAnimatorArray[childAnimatorArray.Length - 1].GetCurrentAnimatorStateInfo(0).length - waitTime);
			for (int m = 0; m < childAnimatorArray.Length; m++)
			{
				if (childAnimatorArray[m].transform != imageArray[selectImage].transform)
				{
					childAnimatorArray[m].enabled = false;
				}
			}
		}
		else
		{
			SpriteRenderer[] childSpriteArray = imageArray[selectImage].transform.GetComponentsInChildren<SpriteRenderer>();
			for (int n = 0; n < childSpriteArray.Length; n++)
			{
				if (childSpriteArray[n].transform != imageArray[selectImage].transform)
				{
					childSpriteArray[n].gameObject.SetActive(false);
				}
			}
			for (int j = 0; j < childSpriteArray.Length; j++)
			{
				if (childSpriteArray[j].transform != imageArray[selectImage].transform)
				{
					childSpriteArray[j].color = new Color(1f, 1f, 1f, 0f);
					childSpriteArray[j].gameObject.SetActive(true);
					Vector3 localPosition = childSpriteArray[j].transform.localPosition;
					childSpriteArray[j].transform.localPosition = new Vector3(localPosition.x, localPosition.y + 0.3f, localPosition.z);
					Sequence s = DOTween.Sequence();
					Vector3 localScale = childSpriteArray[j].transform.localScale;
					s.Append(childSpriteArray[j].DOFade(1f, 0.5f));
					s.Join(childSpriteArray[j].transform.DOLocalMoveY(localPosition.y, 0.5f));
					if (perWaitTime != 0f)
					{
						yield return new WaitForSeconds(perWaitTime);
					}
					else
					{
						yield return new WaitForSeconds(1f);
					}
				}
			}
		}
		yield return new WaitForSeconds(effectAnimator.GetCurrentAnimatorStateInfo(0).length);
		ShowImage(selectImage);
	}
}
