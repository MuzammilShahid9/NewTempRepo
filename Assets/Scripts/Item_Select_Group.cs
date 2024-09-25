using System.Collections;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class Item_Select_Group : ItemAnim
{
	public GroupSingleItem[] groupItems;

	public float itemPerWaitTime = 1f;

	public float totalWaitTime = 2f;

	private bool isBuildFinish;

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
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].HideImage();
			}
		}
	}

	private IEnumerator StartPlayEffect()
	{
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

	public override void FinishBuild()
	{
		isBuildFinish = true;
	}

	public override void ShowImage(int index)
	{
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].Enter(index);
			}
		}
	}

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
		ShowImageWithSelect(index, notShowSelectAnim);
		selectImage = index;
	}

	private void ShowImageWithSelect(int index, bool notShowSelectAnim = false)
	{
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].Enter(index, !notShowSelectAnim);
			}
		}
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
