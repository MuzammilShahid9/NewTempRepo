using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class ItemChangeWallPaper : ItemAnim
{
	public GameObject[] effectArray;

	public ChangeWallSpriteArray[] changeSpriteArray;

	public ChangeWallSpriteArray[] changeSpriteArray1;

	public Vector3[] positionOffsetArray;

	public float singleEffectWaitTime;

	public bool isLinkShow;

	public bool isFlip;

	public float delayTime;

	private List<bool> animFinishCondition = new List<bool>();

	private bool isBuildFinish;

	private List<GameObject> creatEffectArray = new List<GameObject>();

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
		for (int i = 0; i < base.transform.childCount; i++)
		{
			SpriteRenderer[] componentsInChildren = base.transform.GetChild(i).gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sprite = changeSpriteArray[i].changeSprite[0];
			}
		}
		DealHideGameObject(false);
	}

	private IEnumerator StartPlayEffect()
	{
		ShowImage(-1);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			SpriteRenderer[] childSpriteArray = gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
			for (int j = 0; j < childSpriteArray.Length; j++)
			{
				animFinishCondition.Add(false);
				GameObject gameObject2 = Object.Instantiate(effectArray[i]);
				gameObject2.transform.SetParent(childSpriteArray[j].transform);
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f) + positionOffsetArray[i];
				creatEffectArray.Add(gameObject2);
				int count = childSpriteArray[j].sortingOrder + 2;
				WallPaperEffectChange component = gameObject2.GetComponent<WallPaperEffectChange>();
				if (component != null)
				{
					component.Enter(base.transform, count, i);
				}
				yield return new WaitForSeconds(singleEffectWaitTime);
			}
			if (isLinkShow)
			{
				yield return new WaitUntil(() => isAnimFinish);
			}
			else
			{
				yield return new WaitForSeconds(delayTime);
			}
		}
		yield return new WaitUntil(() => isAnimFinish);
		for (int k = 0; k < creatEffectArray.Count; k++)
		{
			Object.Destroy(creatEffectArray[k]);
		}
		ShowImage(selectImage);
		PlotItemAniManager.Instance.FinishStep();
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
			for (int j = 0; j < base.transform.childCount; j++)
			{
				SpriteRenderer[] componentsInChildren = base.transform.GetChild(j).gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].sprite = changeSpriteArray[j].changeSprite[index + 1];
					componentsInChildren[k].color = new Color(1f, 1f, 1f, 1f);
				}
			}
			DealHideGameObject(true);
			return;
		}
		for (int l = 0; l < base.transform.childCount; l++)
		{
			SpriteRenderer[] componentsInChildren2 = base.transform.GetChild(l).gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
			for (int m = 0; m < componentsInChildren2.Length; m++)
			{
				componentsInChildren2[m].sprite = changeSpriteArray[l].changeSprite[0];
				componentsInChildren2[m].color = new Color(1f, 1f, 1f, 1f);
			}
		}
		DealHideGameObject(false);
	}

	public void FinishOneAni()
	{
		for (int i = 0; i < animFinishCondition.Count; i++)
		{
			if (!animFinishCondition[i])
			{
				animFinishCondition[i] = true;
				if (i == animFinishCondition.Count - 1)
				{
					isAnimFinish = true;
				}
				break;
			}
		}
	}

	public override void DealItemUnlock()
	{
		selectImage = UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID);
		if (selectImage != -1)
		{
			ShowImage(selectImage);
		}
	}

	public override void FinishBuild()
	{
		isBuildFinish = true;
	}

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
		selectImage = index;
		for (int i = 0; i < selectShowTweener.Count; i++)
		{
			selectShowTweener[i].Kill();
		}
		selectShowTweener.Clear();
		for (int j = 0; j < base.transform.childCount; j++)
		{
			SpriteRenderer[] componentsInChildren = base.transform.GetChild(j).gameObject.transform.GetComponentsInChildren<SpriteRenderer>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				componentsInChildren[k].sprite = changeSpriteArray[j].changeSprite[index + 1];
				componentsInChildren[k].color = new Color(1f, 1f, 1f, 1f);
				Tweener tweener = componentsInChildren[k].DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.6f).SetLoops(-1, LoopType.Yoyo);
				tweener.Play();
				selectShowTweener.Add(tweener);
			}
		}
		DealHideGameObject(true);
	}
}
