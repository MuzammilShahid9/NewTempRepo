using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	public List<ItemAnim> items;

	public GameObject UnlockEffect;

	public float UnlockEffectTime;

	private int roomIndex;

	private void Start()
	{
	}

	public void Enter(int index)
	{
		roomIndex = index;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i] != null)
			{
				items[i].Enter(roomIndex, i + 1);
			}
		}
	}

	public ItemAnim GetItem(int itemIndex)
	{
		if (itemIndex < items.Count)
		{
			return items[itemIndex];
		}
		return null;
	}

	public void ShowUnlockEffect()
	{
		if (UnlockEffect != null)
		{
			UnlockEffect.SetActive(true);
		}
	}

	public void HideUnlockEffect()
	{
		if (UnlockEffect != null)
		{
			UnlockEffect.SetActive(false);
		}
	}
}
