using System.Collections;
using UnityEngine;

public class SortingLayerManager : MonoBehaviour
{
	private static SortingLayerManager instance;

	public SpriteRenderer purpleBedSprite;

	public static SortingLayerManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public void ChangePurpleBedSortingLayer()
	{
		StartCoroutine(DelayChangePurpleBedSortingLayer());
	}

	private IEnumerator DelayChangePurpleBedSortingLayer()
	{
		yield return new WaitForSeconds(1f);
		float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(purpleBedSprite.transform.position).y;
		if (purpleBedSprite != null)
		{
			purpleBedSprite.sortingOrder = 10000 - (int)(y * 10f) + 200 * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
	}

	public void RecoverPurpleBedSortingLayer()
	{
		StartCoroutine(DelayRecoverPurpleBedSortingLayer());
	}

	private IEnumerator DelayRecoverPurpleBedSortingLayer()
	{
		yield return new WaitForSeconds(1.5f);
		SpriteRenderer component = purpleBedSprite.transform.parent.transform.GetComponent<SpriteRenderer>();
		purpleBedSprite.sortingOrder = component.sortingOrder + 1;
	}
}
