using System;
using System.Collections;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class ItemStageAnim : ItemAnim
{
	[Serializable]
	public class PositionOffsetArray
	{
		public Vector3[] offset;
	}

	public GameObject[] effectObjArray1;

	public GameObject[] effectObjArray2;

	public GameObject[] effectObjArray3;

	public PositionOffsetArray[] positionOffset;

	public override void DealItemUnlock()
	{
		selectImage = UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID);
		if (selectImage != -1)
		{
			ShowImage(selectImage + 1);
		}
	}

	public override void ShowImage(int index)
	{
		originalImage.gameObject.SetActive(false);
		for (int i = 0; i < imageArray.Length; i++)
		{
			imageArray[i].SetActive(false);
		}
		imageArray[index].SetActive(true);
	}

	public override void PlayStageEffect(int stage, float roleAnimWaitEffectTime)
	{
		StartCoroutine(WaitForPlayEffect(stage, roleAnimWaitEffectTime));
	}

	private IEnumerator WaitForPlayEffect(int stage, float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		isAnimFinish = false;
		effectIsChild = true;
		selectImage = stage;
		ActiveSelectObject(selectImage + 1);
		int childCount = imageArray[stage + 1].transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = null;
			switch (stage)
			{
			case 0:
				gameObject = ((i != 1) ? UnityEngine.Object.Instantiate(effectObjArray1[0]) : UnityEngine.Object.Instantiate(effectObjArray1[1]));
				break;
			case 1:
				gameObject = UnityEngine.Object.Instantiate(effectObjArray2[0]);
				break;
			case 2:
				gameObject = UnityEngine.Object.Instantiate(effectObjArray3[0]);
				break;
			}
			gameObject.transform.SetParent(imageArray[stage + 1].transform);
			Vector3 vector = new Vector3(0f, 0f, 0f);
			if (positionOffset.Length > stage && positionOffset[stage].offset.Length > i)
			{
				vector = positionOffset[stage].offset[i];
			}
			imageArray[stage + 1].transform.GetChild(i).gameObject.SetActive(false);
			gameObject.transform.localPosition = imageArray[stage + 1].transform.GetChild(i).transform.localPosition + vector;
			ChangeTexture component = gameObject.GetComponent<ChangeTexture>();
			if (component != null && i == 0)
			{
				component.Enter(base.transform, true);
			}
			else
			{
				component.Enter(base.transform, false);
			}
		}
		StartCoroutine(WaitForChangeImage());
	}

	public override IEnumerator WaitForChangeImage()
	{
		yield return new WaitUntil(() => isAnimFinish);
		PlotItemAniManager.Instance.FinishStep();
	}
}
