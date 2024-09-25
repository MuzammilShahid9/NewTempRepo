using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ItemFadeIn : ItemAnim
{
	private string animStep = "S(0.95,1.05,1)0.2;S(1.05,0.95,0)0.2;S(1,1,1)0.2";

	public float fadeInTime = 1.5f;

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		StartCoroutine(FadeIn(fadeInTime, roleAnimWaitEffectTime));
	}

	private IEnumerator FadeIn(float time, float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		yield return null;
		SpriteRenderer[] childrenSprite = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
		float timer = 0f;
		while (timer <= time)
		{
			yield return null;
			timer += Time.deltaTime;
			float a = timer / time;
			for (int i = 0; i < childrenSprite.Length; i++)
			{
				childrenSprite[i].color = new Color(1f, 1f, 1f, a);
			}
		}
		StartCoroutine(DoAnimator(animStep));
	}

	private IEnumerator DoAnimator(string animStep)
	{
		yield return null;
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			StartCoroutine(ChildrenDoAnimator(componentsInChildren[j], animStep));
		}
		string[] animStepArray = animStep.Split(';');
		for (int i = 0; i < animStepArray.Length; i++)
		{
			if (!(animStepArray[i] != ""))
			{
				continue;
			}
			float num = 0f;
			string[] array = animStepArray[i].Split('|');
			for (int k = 0; k < array.Length; k++)
			{
				if (!(array[k] != ""))
				{
					continue;
				}
				new Vector3(0f, 0f, 0f);
				if (array[k].Substring(0, 1) == "S" || array[k].Substring(0, 1) == "M" || array[k].Substring(0, 1) == "R")
				{
					array[k].Substring(0, 1);
					float num2 = Convert.ToSingle(array[k].Split(')')[1]);
					if (num2 > num)
					{
						num = num2;
					}
					string[] array2 = array[k].Split('(')[1].Split(')')[0].Split(',');
					new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
				}
				else if (array[k].Substring(0, 1) == "D")
				{
					float num2 = Convert.ToSingle(array[k].Substring(1));
					num += num2;
				}
			}
			yield return new WaitForSeconds(num);
		}
		ActiveSelectObject(0);
		PlotItemAniManager.Instance.FinishStep();
	}

	private IEnumerator ChildrenDoAnimator(Transform childrenTrans, string animStep)
	{
		yield return null;
		Vector3 startPosition = childrenTrans.position;
		Vector3 startScale = childrenTrans.localScale;
		string[] animStepArray = animStep.Split(';');
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
			yield return new WaitForSeconds(maxWaitTime);
		}
	}
}
