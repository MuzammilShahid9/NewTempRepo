using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemTileChange : ItemAnim
{
	public bool isWallPaper;

	public bool isLinkShow;

	public bool noEffect;

	public float delayTime;

	public float singleEffectWaitTime;

	public float lineEffectWaitTime = 0.2f;

	public AudioClip audioSource;

	public TileArray[] tileChangeArray;

	public ChangeSpriteArray[] changeSpriteArray;

	public GameObject[] effectGameObjectArray;

	public Vector3 positionOffset = new Vector3(-1.8f, 0.38f, 0f);

	private bool isBuildFinish;

	private bool isLineChange;

	private bool nextAnimStart;

	private Tilemap tileMap;

	private List<bool> animFinishCondition = new List<bool>();

	private Coroutine selectTileIEnumerator;

	public Grid grid;

	public override void Awake()
	{
		buileType = BuildType.Select;
	}

	public override void DealItemUnlock()
	{
		tileMap = base.transform.parent.GetComponent<Tilemap>();
		tempSelectIndex = -1;
		selectImage = UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID);
		if (selectImage != -1)
		{
			base.transform.parent.gameObject.SetActive(true);
			ShowImage(selectImage);
		}
	}

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		isBuildFinish = false;
		base.transform.parent.gameObject.SetActive(true);
		BuildManager.Instance.StartBuild(base.transform);
		StartCoroutine(WaitForPlayEffect(roleAnimWaitEffectTime));
	}

	private IEnumerator WaitForPlayEffect(float roleAnimWaitEffectTime)
	{
		yield return new WaitUntil(() => isBuildFinish);
		PlotManager.Instance.PlotInsertRoleAction();
		if (!noEffect)
		{
			ShowImage(-1);
			yield return new WaitForSeconds(roleAnimWaitEffectTime);
			StartCoroutine(StartPlayEffect());
		}
		else
		{
			Showtile(selectImage);
			PlotItemAniManager.Instance.FinishStep();
		}
	}

	private IEnumerator StartPlayEffect()
	{
		Showtile(-1);
		if (audioSource != null)
		{
			AudioManager.Instance.PlayAudioEffect(audioSource.name);
		}
		for (int i = 0; i < tileChangeArray.Length; i++)
		{
			isAnimFinish = false;
			StartCoroutine(ShowSingleEffect(i));
			if (isLinkShow)
			{
				yield return new WaitUntil(() => isAnimFinish);
			}
			else
			{
				yield return new WaitForSeconds(delayTime);
			}
		}
		StartCoroutine(WaitForChangeImage());
	}

	private IEnumerator ShowSingleEffect(int index)
	{
		isAnimFinish = false;
		BoundsInt bounds = tileMap.cellBounds;
		int count = 0;
		bool flag = false;
		Vector2 vector = new Vector2(0f, 0f);
		TileBase needChange = tileChangeArray[index].tiles[tempSelectIndex + 1];
		for (int i = bounds.xMin; i <= bounds.xMax; i++)
		{
			for (int j = bounds.yMin; j <= bounds.yMax; j++)
			{
				TileBase tile = tileMap.GetTile(new Vector3Int(i, j, 0));
				if (tile != null && tile == needChange)
				{
					flag = true;
					vector.x = i;
					vector.y = j;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			for (int x = bounds.xMin; x <= bounds.xMax; x++)
			{
				isLineChange = false;
				for (int k = bounds.yMin; k <= bounds.yMax; k++)
				{
					TileBase tile2 = tileMap.GetTile(new Vector3Int(x, k, 0));
					if (tile2 != null && tile2 == needChange)
					{
						isLineChange = true;
						break;
					}
				}
				StartCoroutine(CreateLineEffect(index, x, count));
				if (isLineChange)
				{
					yield return new WaitForSeconds(lineEffectWaitTime);
				}
			}
		}
		yield return new WaitForSeconds(singleEffectWaitTime);
	}

	public IEnumerator CreateLineEffect(int index, int x, int count)
	{
		BoundsInt bounds = tileMap.cellBounds;
		TileBase needChange = tileChangeArray[index].tiles[tempSelectIndex + 1];
		for (int y = bounds.yMin; y <= bounds.yMax; y++)
		{
			TileBase tile = tileMap.GetTile(new Vector3Int(x, y, 0));
			if (tile != null && tile == needChange)
			{
				CreateEffect(index, x, y, count);
				yield return new WaitForSeconds(singleEffectWaitTime);
			}
		}
	}

	public void CreateEffect(int index, int x, int y, int count)
	{
		animFinishCondition.Add(false);
		nextAnimStart = false;
		if (grid == null)
		{
			PlotItemAniManager.Instance.FinishStep();
		}
		Vector3 vector = grid.CellToWorld(new Vector3Int(x, y, 0));
		vector = new Vector3(vector.x + positionOffset.x, vector.y + positionOffset.y, vector.z);
		GameObject obj = Object.Instantiate(effectGameObjectArray[index], base.transform);
		obj.transform.position = vector;
		int sortingOrder = obj.GetComponentInChildren<SpriteRenderer>().sortingOrder;
		sortingOrder += count;
		TileEffectChangeTexture component = obj.GetComponent<TileEffectChangeTexture>();
		if (component != null)
		{
			component.Enter(base.transform, sortingOrder, index);
		}
	}

	public override IEnumerator WaitForChangeImage()
	{
		yield return new WaitUntil(() => isAnimFinish);
		HideEffect();
		Showtile(selectImage);
		PlotItemAniManager.Instance.FinishStep();
	}

	public override void FinishBuild()
	{
		isBuildFinish = true;
	}

	public override void ShowImage(int index)
	{
		if (selectTileIEnumerator != null)
		{
			StopCoroutine(selectTileIEnumerator);
		}
		tileMap.color = new Color(1f, 1f, 1f, 1f);
		Showtile(index);
	}

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{

        Debug.Log("SelectShowImage");
		Showtile(index);
		if (selectTileIEnumerator != null)
		{
			StopCoroutine(selectTileIEnumerator);
		}
		tileMap.color = new Color(1f, 1f, 1f, 1f);
		selectTileIEnumerator = StartCoroutine(SelectTileChangeColor());
	}

	private IEnumerator SelectTileChangeColor()
	{
		yield return null;
		float timer = 0f;
		float lastTime = 0.6f;
		while (timer <= lastTime * 2f)
		{
			while (timer <= lastTime)
			{
				yield return null;
				timer += Time.deltaTime;
				float num = 1f - timer / lastTime * 0.2f;
				tileMap.color = new Color(num, num, num, 1f);
			}
			while (timer > lastTime && timer < lastTime * 2f)
			{
				yield return null;
				timer += Time.deltaTime;
				float num2 = 0.8f + (timer - lastTime) / lastTime * 0.2f;
				tileMap.color = new Color(num2, num2, num2, 1f);
			}
			yield return null;
			timer += Time.deltaTime;
			if (timer >= lastTime * 2f)
			{
				timer -= lastTime * 2f;
			}
		}
	}

	public void Showtile(int index)
	{
        Debug.Log("Show tile " + index + " " + tileMap);
		BoundsInt cellBounds = tileMap.cellBounds;
        Debug.Log("cellBounds xMin " + cellBounds.xMin + " xMax " + cellBounds.xMax + " yMin " + cellBounds.yMin + " yMax " + cellBounds.yMax);
		for (int i = cellBounds.xMin; i < cellBounds.xMax; i++)
		{
			for (int j = cellBounds.yMin; j < cellBounds.yMax; j++)
			{
				TileBase tile = tileMap.GetTile(new Vector3Int(i, j, 0));
				for (int k = 0; k < tileChangeArray.Length; k++)
				{
					if (tile != null && tile == tileChangeArray[k].tiles[tempSelectIndex + 1])
					{
						TileBase tile2 = tileChangeArray[k].tiles[index + 1];
						tileMap.SetTile(new Vector3Int(i, j, 0), tile2);
					}
				}
			}
		}
		tempSelectIndex = index;
	}

	public void StartNextAnim()
	{
		if (!nextAnimStart)
		{
			nextAnimStart = true;
		}
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

	public void HideEffect()
	{
		TileEffectChangeTexture[] componentsInChildren = base.transform.GetComponentsInChildren<TileEffectChangeTexture>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
		FinishAnim();
	}
}
