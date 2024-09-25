using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ItemPress : MonoBehaviour
{
	private bool itemPress;

	private float pressTime;

	private float showArrowTime;

	private Vector3 startPosition;

	private float pressTimer;

	private void Start()
	{
		startPosition = base.transform.position;
	}

	private void Update()
	{
		if (itemPress)
		{
			pressTimer += Time.deltaTime;
		}
		if (pressTimer >= pressTime && itemPress)
		{
			itemPress = false;
			StartCoroutine(ShowItemImage());
			CastleSceneUIManager.Instance.ShowChangeItemUI(ItemManager.Instance.GetItemInfo(1));
		}
	}

	private void OnMouseDown()
	{
		if (CastleSceneUIManager.Instance.GetSelectItemUIStatu())
		{
			CastleSceneUIManager.Instance.ShowChangeItemUI(ItemManager.Instance.GetItemInfo(1));
		}
		else
		{
			DebugUtils.Log(DebugType.Other, "玩蛇？");
		}
		itemPress = true;
	}

	private void OnMouseUp()
	{
		float pressTimer2 = pressTimer;
		float showArrowTime2 = showArrowTime;
		itemPress = false;
		pressTimer = 0f;
	}

	private IEnumerator ShowItemImage()
	{
		base.transform.DOMoveY(startPosition.y + 0.1f, 0.2f);
		base.transform.DOScale(new Vector3(0.95f, 1.05f, 1f), 0.2f);
		yield return new WaitForSeconds(0.2f);
		base.transform.DOMoveY(startPosition.y, 0.2f);
		base.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);
	}
}
