using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
	private Tilemap tilemap;

	private Item item;

	private bool nextAnimStart;

	private string needChangeName;

	public List<bool> animFinishCondition = new List<bool>();

	public Grid grid;

	private void Start()
	{
		animFinishCondition.Clear();
		tilemap = base.transform.GetComponent<Tilemap>();
		nextAnimStart = true;
	}

	public void DoEffect(string effectDirection, Item targetItem)
	{
		item = targetItem;
		StartCoroutine(DoLocalEffect(effectDirection));
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
					HideEffect();
					item.Showtile(item.selectImage);
				}
				break;
			}
		}
	}

	public void HideEffect()
	{
		SelectItemUIManager[] componentsInChildren = base.transform.GetComponentsInChildren<SelectItemUIManager>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
		PlotItemAniManager.Instance.FinishOneCondition();
	}

	private IEnumerator DoLocalEffect(string effectDirection)
	{
		yield return null;
		item.Showtile(-1);
		int count = 0;
		string[] array = item.configData.Tile.Split(';');
		needChangeName = array[item.tempSelectIndex + 1];
		BoundsInt bounds = tilemap.cellBounds;
		if (effectDirection == "R")
		{
			for (int x2 = bounds.xMin; x2 <= bounds.xMax; x2++)
			{
				for (int y2 = bounds.yMin; y2 <= bounds.yMax; y2++)
				{
					TileBase tile = tilemap.GetTile(new Vector3Int(x2, y2, 0));
					item.configData.Tile.Split(';');
					if (tile != null && tile.name == needChangeName)
					{
						CreateEffect(x2, y2, count);
						yield return new WaitUntil(() => nextAnimStart);
						count++;
					}
				}
			}
		}
		else
		{
			if (!(effectDirection == "L"))
			{
				yield break;
			}
			for (int x2 = bounds.xMax; x2 >= bounds.xMin; x2--)
			{
				for (int y2 = bounds.yMax; y2 >= bounds.yMin; y2--)
				{
					TileBase tile2 = tilemap.GetTile(new Vector3Int(x2, y2, 0));
					item.configData.Tile.Split(';');
					if (tile2 != null && tile2.name == needChangeName)
					{
						CreateEffect(x2, y2, count);
						yield return new WaitUntil(() => nextAnimStart);
						count++;
					}
				}
			}
		}
	}

	public void CreateEffect(int x, int y, int count)
	{
		animFinishCondition.Add(false);
		nextAnimStart = false;
		GameObject original = Resources.Load(item.configData.Effect.Split('|')[0], typeof(GameObject)) as GameObject;
		Vector3 vector = grid.CellToWorld(new Vector3Int(x, y, 0));
		vector = new Vector3(vector.x + base.transform.localPosition.x, vector.y + base.transform.localPosition.y, -10f);
		GameObject obj = Object.Instantiate(original, vector, Quaternion.identity, tilemap.transform);
		int sortingOrder = obj.GetComponentInChildren<SpriteRenderer>().sortingOrder;
		obj.GetComponent<TileEffectChangeTexture>();
	}
}
