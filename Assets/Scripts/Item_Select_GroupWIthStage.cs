using System.Collections;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class Item_Select_GroupWIthStage : ItemAnim
{
	public GroupSingleItem[] groupItems;

	public float itemPerWaitTime = 1f;

	public float totalWaitTime = 2f;

	public float[] stageTotalWaitTimeArray;

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

	public override void PlayStageEffect(int stage, float roleAnimWaitEffectTime)
	{
		switch (stage)
		{
		case 0:
			PlayEffect(roleAnimWaitEffectTime);
			break;
		case 1:
		{
			selectImage += 3;
			for (int i = 0; i < groupItems.Length; i++)
			{
				if (groupItems[i] != null)
				{
					groupItems[i].ShowImage(selectImage);
				}
			}
			StartCoroutine(WaitForFinishAnim());
			break;
		}
		}
	}

	private IEnumerator WaitForFinishAnim()
	{
		yield return new WaitForSeconds(stageTotalWaitTimeArray[1]);
		PlotItemAniManager.Instance.FinishStep();
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
		DealHideGameObject(false);
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
		if (itemPerWaitTime * (float)groupItems.Length < stageTotalWaitTimeArray[0])
		{
			yield return new WaitForSeconds(stageTotalWaitTimeArray[0] - itemPerWaitTime * (float)groupItems.Length);
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

	public override void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
		ShowImageWithSelect(index);
		selectImage = index;
	}

	private void ShowImageWithSelect(int index)
	{
		for (int i = 0; i < groupItems.Length; i++)
		{
			if (groupItems[i] != null)
			{
				groupItems[i].Enter(index, true);
			}
		}
	}
}
