using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSIngleAnim : GroupSingleItem
{
	public List<GameObject> effectArray;

	public float perWaitTime;

	public override IEnumerator StartPlayEffect()
	{
		effectArray = new List<GameObject>();
		float waitTime = 0f;
		originalImage.SetActive(false);
		for (int j = 0; j < imageArray.Length; j++)
		{
			if (imageArray[j] != null)
			{
				imageArray[j].SetActive(false);
			}
		}
		imageArray[selectImage].SetActive(true);
		SpriteRenderer[] childSpriteArray = imageArray[selectImage].transform.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < childSpriteArray.Length; i++)
		{
			childSpriteArray[i].enabled = false;
			effectGameObject = Object.Instantiate(effect, childSpriteArray[i].transform);
			effectGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			if (childSpriteArray[i].flipX)
			{
				effectGameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			effectArray.Add(effectGameObject);
			waitTime = perWaitTime;
			yield return new WaitForSeconds(waitTime);
		}
		yield return new WaitForSeconds(effectArray[effectArray.Count - 1].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - waitTime);
		ShowImage(selectImage);
	}
}
