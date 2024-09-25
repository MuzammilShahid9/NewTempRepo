using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ItemFadeOut : ItemAnim
{
	public string animStep = "M(0,1,0)2";

	public float fadeOutTime = 2f;

	public float perItemWaitTime = 0.5f;

	public AudioClip audioSource;

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		StartCoroutine(DoAnimator(animStep, roleAnimWaitEffectTime));
	}

	private IEnumerator DoAnimator(string animStep, float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		yield return null;
		if (imageArray.Length != 0)
		{
			for (int j = 0; j < imageArray.Length; j++)
			{
				imageArray[j].gameObject.SetActive(true);
			}
		}
		if (audioSource != null)
		{
			AudioManager.Instance.PlayAudioEffect(audioSource.name);
		}
		SpriteRenderer[] childrenArray = originalImage.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < childrenArray.Length; i++)
		{
			if (childrenArray[i].transform != originalImage.transform)
			{
				StartCoroutine(ChildrenDoAnimator(childrenArray[i].transform, animStep));
				yield return new WaitForSeconds(perItemWaitTime);
			}
		}
		PlotItemAniManager.Instance.FinishStep();
		yield return new WaitForSeconds(fadeOutTime + perItemWaitTime);
		originalImage.gameObject.SetActive(false);
	}

	private IEnumerator ChildrenDoAnimator(Transform childrenTrans, string animStep)
	{
		yield return null;
		if (effectObj != null)
		{
			UnityEngine.Object.Instantiate(effectObj, originalImage.transform).transform.localPosition = childrenTrans.localPosition;
		}
		Vector3 startPosition = childrenTrans.position;
		Vector3 startScale = childrenTrans.localScale;
		string[] animStepArray = animStep.Split(';');
		StartCoroutine(FadeOut(childrenTrans.GetComponent<SpriteRenderer>(), fadeOutTime));
		for (int i = 0; i < animStepArray.Length; i++)
		{
			if (!(animStepArray[i] != ""))
			{
				continue;
			}
			float maxWaitTime = 0f;
			string[] stepDetail = animStepArray[i].Split('|');
			for (int j = 0; j < stepDetail.Length; j++)
			{
				if (!(stepDetail[j] != ""))
				{
					continue;
				}
				float num = 0f;
				Vector3 vector = new Vector3(0f, 0f, 0f);
				if (stepDetail[j].Substring(0, 1) == "S" || stepDetail[j].Substring(0, 1) == "M" || stepDetail[j].Substring(0, 1) == "R")
				{
					stepDetail[j].Substring(0, 1);
					num = Convert.ToSingle(stepDetail[j].Split(')')[1]);
					if (num > maxWaitTime)
					{
						maxWaitTime = num;
					}
					string[] array = stepDetail[j].Split('(')[1].Split(')')[0].Split(',');
					vector = new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
				}
				if (stepDetail[j].Substring(0, 1) == "S")
				{
					vector = new Vector3(vector.x * startScale.x, vector.y * startScale.y, vector.z * startScale.z);
					childrenTrans.DOScale(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "M")
				{
					childrenTrans.DOMove(startPosition + vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "R")
				{
					childrenTrans.DORotate(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					yield return new WaitForSeconds(Convert.ToSingle(stepDetail[j].Substring(1)));
				}
			}
			yield return new WaitForSeconds(perItemWaitTime);
		}
	}

	private IEnumerator FadeOut(SpriteRenderer sprite, float time)
	{
		yield return null;
		float timer = 0f;
		while (timer <= time)
		{
			yield return null;
			timer += Time.deltaTime;
			float a = 1f - timer / time;
			sprite.color = new Color(1f, 1f, 1f, a);
		}
	}
}
