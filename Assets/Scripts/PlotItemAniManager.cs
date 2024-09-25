using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using UnityEngine;

public class PlotItemAniManager : MonoBehaviour
{
	[Serializable]
	public class ItemAniConfigDataList
	{
		public List<ItemAniConfigData> data = new List<ItemAniConfigData>();
	}

	[Serializable]
	public class ItemAniConfigData
	{
		public string Key;

		public int RoomID;

		public int ItemID;

		public string Step;

		public float DelayTime;

		public string ImageName;
	}

	[Serializable]
	public class ItemAniStepConfigDataList
	{
		public List<ItemAniStepConfigData> data = new List<ItemAniStepConfigData>();
	}

	[Serializable]
	public class ItemAniStepConfigData
	{
		public string ID;

		public string AnimStep;
	}

	private List<ItemAniConfigData> itemAniConfig;

	private ItemAniConfigData currItemAniData;

	private List<ItemAniStepConfigData> itemAniStepConfig;

	private ItemAniStepConfigData currItemAniStepData;

	private bool isStepFinished;

	private bool isAnimStepFinish;

	private List<bool> stepFinishCondition = new List<bool>();

	private List<GameObject> effectTransform = new List<GameObject>();

	private bool isDelay;

	private Item currItem;

	private ItemAnim currItemAnim;

	private int plotStep;

	private static PlotItemAniManager instance;

	public static PlotItemAniManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		TextAsset textAsset = Resources.Load("Config/Plot/ItemAniConfig") as TextAsset;
		if (textAsset != null)
		{
			ItemAniConfigDataList itemAniConfigDataList = JsonUtility.FromJson<ItemAniConfigDataList>(textAsset.text);
			itemAniConfig = itemAniConfigDataList.data;
		}
		TextAsset textAsset2 = Resources.Load("Config/Plot/AniStepConfig") as TextAsset;
		if (textAsset2 != null)
		{
			ItemAniStepConfigDataList itemAniStepConfigDataList = JsonUtility.FromJson<ItemAniStepConfigDataList>(textAsset2.text);
			itemAniStepConfig = itemAniStepConfigDataList.data;
		}
		RemoveSpecialChar();
	}

	private void RemoveSpecialChar()
	{
		for (int i = 0; i < itemAniConfig.Count; i++)
		{
			if (itemAniConfig[i].Step != "")
			{
				string[] array = itemAniConfig[i].Step.Split('\n');
				string text = "";
				for (int j = 0; j < array.Length; j++)
				{
					text += array[j];
				}
				itemAniConfig[i].Step = text;
			}
		}
		for (int k = 0; k < itemAniStepConfig.Count; k++)
		{
			if (itemAniStepConfig[k].AnimStep != "")
			{
				string[] array2 = itemAniStepConfig[k].AnimStep.Split('\n');
				string text2 = "";
				for (int l = 0; l < array2.Length; l++)
				{
					text2 += array2[l];
				}
				itemAniStepConfig[k].AnimStep = text2;
			}
		}
	}

	public void StartItemAni(string itemAniID)
	{
		for (int i = 0; i < itemAniConfig.Count; i++)
		{
			if (itemAniConfig[i].Key == itemAniID)
			{
				currItemAniData = itemAniConfig[i];
			}
		}
		StartCoroutine(DealItemAni());
	}

	public void StartItemAnimAction(CDBuildConfig currBuildConfig, int currStep)
	{
		plotStep = currStep;
		currItemAniData = new ItemAniConfigData();
		currItemAniData.RoomID = currBuildConfig.roomID;
		currItemAniData.ItemID = currBuildConfig.itemID;
		currItemAniData.DelayTime = currBuildConfig.delayTime;
		if (currBuildConfig.stageID > 0)
		{
			currItemAniData.Step = (currBuildConfig.stageID - 1).ToString();
		}
		else
		{
			currItemAniData.Step = null;
		}
		StartCoroutine(DealItemAni());
	}

	public ItemIndex GetAnimItemInfo(string itemAniID)
	{
		ItemIndex itemIndex = null;
		ItemAniConfigData itemAniConfigData = null;
		for (int i = 0; i < itemAniConfig.Count; i++)
		{
			if (itemAniConfig[i].Key == itemAniID)
			{
				itemAniConfigData = itemAniConfig[i];
			}
		}
		if (itemAniConfigData != null)
		{
			itemIndex = new ItemIndex();
			itemIndex.roomIndex = itemAniConfigData.RoomID;
			itemIndex.itemIndex = itemAniConfigData.ItemID;
		}
		return itemIndex;
	}

	private IEnumerator DealItemAni()
	{
		yield return null;
		isStepFinished = false;
		if (currItemAniData == null)
		{
			FinishStep();
			yield break;
		}
		RoomManager room = CastleManager.Instance.GetRoom(currItemAniData.RoomID);
		if (room == null)
		{
			FinishStep();
			yield break;
		}
		ItemAnim item = room.GetItem(currItemAniData.ItemID);
		DebugUtils.Log(DebugType.Plot, "###StartItemAnim  RoomID: " + currItemAniData.RoomID + " ItemID: " + currItemAniData.ItemID);
		if (item == null)
		{
			FinishStep();
			yield break;
		}
		currItemAnim = room.GetItem(currItemAniData.ItemID).GetComponent<ItemAnim>();
		if (currItemAniData.Step == null || currItemAniData.Step == "")
		{
			currItemAnim.PlayEffect(currItemAniData.DelayTime);
			yield break;
		}
		currItemAnim.PlayStageEffect(Convert.ToInt32(currItemAniData.Step.Split('.')[0]), currItemAniData.DelayTime);
	}

	public void PlayEffect()
	{
		string effect = currItem.configData.Effect;
		SpriteRenderer[] componentsInChildren = currItem.GetComponentsInChildren<SpriteRenderer>();
		float x = 0f;
		float y = 0f;
		if (currItem.configData.EffectPosition != null && currItem.configData.EffectPosition != "")
		{
			string[] array = currItem.configData.EffectPosition.Split(';');
			x = Convert.ToSingle(array[currItem.selectImage].Split(',')[0].Substring(1));
			y = Convert.ToSingle(array[currItem.selectImage].Split(',')[1]);
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(effect) as GameObject);
			effectTransform.Add(gameObject);
			gameObject.transform.SetParent(componentsInChildren[i].transform);
			gameObject.transform.localPosition = new Vector3(x, y, 0f);
			gameObject.GetComponent<ChangeTexture>();
		}
	}

	private IEnumerator ItemFadeIn(float time)
	{
		yield return null;
		SpriteRenderer sprite = currItem.transform.GetComponent<SpriteRenderer>();
		SpriteRenderer[] childrenSprite = currItem.gameObject.GetComponentsInChildren<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 0f);
		float timer = 0f;
		while (timer <= time)
		{
			yield return null;
			timer += Time.deltaTime;
			float a = timer / time;
			sprite.color = new Color(1f, 1f, 1f, a);
			for (int i = 0; i < childrenSprite.Length; i++)
			{
				childrenSprite[i].color = new Color(1f, 1f, 1f, a);
			}
		}
		FinishOneCondition();
	}

	private IEnumerator ItemFadeOut(float time)
	{
		yield return null;
		SpriteRenderer sprite = currItem.transform.GetComponent<SpriteRenderer>();
		SpriteRenderer[] childrenSprite = currItem.gameObject.GetComponentsInChildren<SpriteRenderer>();
		sprite.color = new Color(1f, 1f, 1f, 1f);
		float timer = 0f;
		while (timer <= time)
		{
			yield return null;
			timer += Time.deltaTime;
			float a = 1f - timer / time;
			sprite.color = new Color(1f, 1f, 1f, a);
			for (int i = 0; i < childrenSprite.Length; i++)
			{
				childrenSprite[i].color = new Color(1f, 1f, 1f, a);
			}
		}
		FinishOneCondition();
	}

	private IEnumerator StartDealy(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		isDelay = false;
		if (!isAnimStepFinish)
		{
			FinishOneCondition();
		}
	}

	private IEnumerator ChildrenDoAnimator(Transform childrenTrans, string animStep)
	{
		yield return null;
		Vector3 startPosition = childrenTrans.position;
		Vector3 startScale = childrenTrans.localScale;
		string[] animStepArray = animStep.Split(';');
		for (int i = 0; i < animStepArray.Length; i++)
		{
			if (!(animStepArray[i] != ""))
			{
				continue;
			}
			float maxWaitTime = 0f;
			string[] stepDetail = animStepArray[i].Split('|');
			for (int j = 0; j < stepDetail.Length; j++)
			{
				if (!(stepDetail[j] != ""))
				{
					continue;
				}
				float num = 0f;
				Vector3 vector = new Vector3(0f, 0f, 0f);
				if (stepDetail[j].Substring(0, 1) == "S" || stepDetail[j].Substring(0, 1) == "M" || stepDetail[j].Substring(0, 1) == "R")
				{
					stepDetail[j].Substring(0, 1);
					num = Convert.ToSingle(stepDetail[j].Split(')')[1]);
					if (num > maxWaitTime)
					{
						maxWaitTime = num;
					}
					string[] array = stepDetail[j].Split('(')[1].Split(')')[0].Split(',');
					vector = new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
				}
				if (stepDetail[j].Substring(0, 1) == "S")
				{
					vector = new Vector3(vector.x * startScale.x, vector.y * startScale.y, vector.z * startScale.z);
					childrenTrans.DOScale(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "M")
				{
					childrenTrans.DOMove(startPosition + vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "R")
				{
					childrenTrans.DORotate(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					yield return new WaitForSeconds(Convert.ToSingle(stepDetail[j].Substring(1)));
				}
			}
			yield return new WaitForSeconds(maxWaitTime);
		}
	}

	private IEnumerator DoAnimator(string animStep)
	{
		yield return null;
		Transform[] componentsInChildren = currItem.gameObject.GetComponentsInChildren<Transform>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			StartCoroutine(ChildrenDoAnimator(componentsInChildren[j], animStep));
		}
		string[] animStepArray = animStep.Split(';');
		for (int i = 0; i < animStepArray.Length; i++)
		{
			if (!(animStepArray[i] != ""))
			{
				continue;
			}
			float num = 0f;
			string[] array = animStepArray[i].Split('|');
			for (int k = 0; k < array.Length; k++)
			{
				if (!(array[k] != ""))
				{
					continue;
				}
				new Vector3(0f, 0f, 0f);
				if (array[k].Substring(0, 1) == "S" || array[k].Substring(0, 1) == "M" || array[k].Substring(0, 1) == "R")
				{
					array[k].Substring(0, 1);
					float num2 = Convert.ToSingle(array[k].Split(')')[1]);
					if (num2 > num)
					{
						num = num2;
					}
					string[] array2 = array[k].Split('(')[1].Split(')')[0].Split(',');
					new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
				}
				else if (array[k].Substring(0, 1) == "D")
				{
					float num2 = Convert.ToSingle(array[k].Substring(1));
					num += num2;
				}
			}
			yield return new WaitForSeconds(num);
		}
		FinishOneCondition();
	}

	public void FinishOneCondition(GameObject effectGo = null)
	{
		if (effectGo != null)
		{
			for (int i = 0; i < effectTransform.Count; i++)
			{
				if (effectTransform[i] == effectGo)
				{
					effectGo.SetActive(false);
				}
			}
		}
		if (isAnimStepFinish)
		{
			return;
		}
		for (int j = 0; j < stepFinishCondition.Count; j++)
		{
			if (!stepFinishCondition[j])
			{
				stepFinishCondition[j] = true;
				break;
			}
		}
		ChangeStepFinished();
	}

	private void ChangeStepFinished()
	{
		bool flag = true;
		for (int i = 0; i < stepFinishCondition.Count; i++)
		{
			if (!stepFinishCondition[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			isAnimStepFinish = true;
		}
		else
		{
			isAnimStepFinish = false;
		}
	}

	public void StopStep()
	{
		isStepFinished = true;
		currItemAnim.StopAllAni();
	}

	public void FinishStep()
	{
		if (!isStepFinished)
		{
			isStepFinished = true;
			PlotManager.Instance.FinishOneCondition(plotStep);
		}
	}

	public void RestortPlotStep()
	{
		plotStep = -2;
	}

	public string GetItemNameWithID(int roomID, int itemID)
	{
		for (int i = 0; i < itemAniConfig.Count; i++)
		{
			if (itemAniConfig[i].RoomID == roomID && itemAniConfig[i].ItemID == itemID)
			{
				return itemAniConfig[i].ImageName;
			}
		}
		return "";
	}
}
