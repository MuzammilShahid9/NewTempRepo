using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
	private Vector3 startPosition;

	private void Start()
	{
		startPosition = base.transform.position;
	}

	private void Update()
	{
	}

	public void StartAnim(string animStep)
	{
		StopAllCoroutines();
		StartCoroutine(DoAnimator(animStep));
	}

	private IEnumerator DoAnimator(string animStep)
	{
		yield return null;
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
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					num = Convert.ToSingle(stepDetail[j].Substring(1));
					if (num > maxWaitTime)
					{
						maxWaitTime += num;
					}
				}
				if (stepDetail[j].Substring(0, 1) == "S")
				{
					base.transform.DOScale(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "M")
				{
					base.transform.DOMove(startPosition + vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "R")
				{
					base.transform.DORotate(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					yield return new WaitForSeconds(Convert.ToSingle(stepDetail[j].Substring(1)));
				}
			}
			yield return new WaitForSeconds(maxWaitTime);
		}
		PlotItemAniManager.Instance.FinishStep();
	}
}
