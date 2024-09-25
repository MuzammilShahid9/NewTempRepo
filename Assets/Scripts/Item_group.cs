using System.Collections;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class Item_group : ItemAnim
{
	public GroupSingleItem[] groupItems;

	public float itemPerWaitTime = 0.5f;

	public float totalWaitTime = 2f;

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		StartCoroutine(StartPlayEffect(roleAnimWaitEffectTime));
	}

	private IEnumerator StartPlayEffect(float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		yield return null;
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].PlayEffect();
				yield return new WaitForSeconds(itemPerWaitTime);
			}
		}
		if (itemPerWaitTime * (float)groupItems.Length < totalWaitTime)
		{
			yield return new WaitForSeconds(totalWaitTime - itemPerWaitTime * (float)groupItems.Length);
		}
		PlotItemAniManager.Instance.FinishStep();
	}

	public override void DealItemUnlock()
	{
		if (itemType == ItemType.Image)
		{
			selectImage = UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID);
		}
		if (selectImage == -1)
		{
			return;
		}
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].Enter(selectImage);
			}
		}
	}
}
