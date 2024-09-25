using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
	[Serializable]
	public class BuildConfigDataList
	{
		public List<BuildConfigData> data = new List<BuildConfigData>();
	}

	[Serializable]
	public class BuildConfigData
	{
		public int ID;

		public int ItemID;

		public string CameraPositon;

		public string CameraMoveTime;
	}

	private static BuildManager instance;

	private List<BuildConfigData> buildConfig;

	private BuildConfigData currBuildData;

	private ItemAnim currItemSelect;

	private bool isStepFinished;

	private bool isCameraMoveFinish;

	private bool isSelectStepFinish;

	public static BuildManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		isStepFinished = true;
		BuildConfigDataList buildConfigDataList = JsonUtility.FromJson<BuildConfigDataList>((Resources.Load("Config/Plot/BuildConfig") as TextAsset).text);
		buildConfig = buildConfigDataList.data;
	}

	public void StartBuild(Transform tempTransform)
	{
		currItemSelect = tempTransform.GetComponent<ItemAnim>();
		isStepFinished = false;
		DealBuild();
	}

	public int GetAnimItemInfo(string itemAniID)
	{
		int result = -1;
		for (int i = 0; i < buildConfig.Count; i++)
		{
			if (buildConfig[i].ID == Convert.ToInt32(itemAniID))
			{
				result = buildConfig[i].ItemID;
			}
		}
		return result;
	}

	public void DealBuild()
	{
		isCameraMoveFinish = false;
		isSelectStepFinish = false;
		if (currItemSelect.cameraPositon == 0)
		{
			MoveFinish();
		}
		else if (currItemSelect.cameraPositon != 1 && currItemSelect.cameraPositon == 2)
		{
			MoveFinish();
		}
		StartCoroutine(StartBuildItem(currItemSelect));
	}

	private void HideBuildItem()
	{
		ItemManager.Instance.GetItemInfo(currBuildData.ItemID).gameObject.SetActive(false);
	}

	private void ShowBuildItem()
	{
		ItemManager.Instance.GetItemInfo(currBuildData.ItemID).gameObject.SetActive(true);
	}

	public void MoveFinish()
	{
		if (!isStepFinished)
		{
			isCameraMoveFinish = true;
		}
	}

	public void SelectFinish()
	{
		if (!isStepFinished)
		{
			isSelectStepFinish = true;
		}
	}

	private IEnumerator StartBuildItem(ItemAnim buildItem)
	{
		yield return new WaitUntil(() => isCameraMoveFinish);
		CastleSceneUIManager.Instance.gameObject.SetActive(true);
		CastleSceneUIManager.Instance.ShowChangeItemUI(buildItem.transform, true);
		yield return new WaitUntil(() => isSelectStepFinish);
		FinishStep();
	}

	public void FinishStep()
	{
		if (!isStepFinished)
		{
			isStepFinished = true;
			currItemSelect.FinishBuild();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
