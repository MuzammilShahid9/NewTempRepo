using System.Collections.Generic;
using UnityEngine;

public class OnePrefabPool
{
	public GameObject Prefab;

	public GameObject PrefabCollection;

	public GameObject CollectionObj;

	public int PrefabForInstantiateNum;

	public List<GameObject> PrefabDeActiveList;

	public List<GameObject> PrefabActiveList;

	public int name;

	public OnePrefabPool(int name, GameObject prefab, GameObject prefabParent, int Num)
	{
		Prefab = prefab;
		PrefabForInstantiateNum = Num;
		CollectionObj = prefabParent;
		this.name = name;
		Init();
	}

	public void Init()
	{
		PrefabCollection = new GameObject();
		PrefabCollection.name = Prefab.transform.name + "_Collection";
		PrefabCollection.transform.parent = CollectionObj.transform;
		PrefabDeActiveList = new List<GameObject>(PrefabForInstantiateNum);
		PrefabActiveList = new List<GameObject>();
		PrefabDeActiveList.Clear();
		for (int i = 0; i < PrefabForInstantiateNum; i++)
		{
			GameObject gameObject = Object.Instantiate(Prefab);
			gameObject.SetActive(false);
			gameObject.AddComponent<objType>().type = name;
			gameObject.transform.parent = PrefabCollection.transform;
			PrefabDeActiveList.Add(gameObject);
		}
	}

	public GameObject Spawn(bool isActive)
	{
		if (Prefab != null && CheckListIsNull(PrefabDeActiveList))
		{
			for (int i = 0; i < 4; i++)
			{
				GameObject gameObject = Object.Instantiate(Prefab);
				gameObject.SetActive(false);
				gameObject.AddComponent<objType>().type = name;
				gameObject.transform.SetParent(PrefabCollection.transform);
				PrefabDeActiveList.Add(gameObject);
			}
		}
		while (PrefabDeActiveList.Count > 0 && PrefabDeActiveList[0] == null)
		{
			DebugUtils.Log(DebugType.Other, name + "       PrefabDeActiveList[0] is null ");
			PrefabDeActiveList.RemoveAt(0);
		}
		GameObject gameObject2 = null;
		if (PrefabDeActiveList.Count == 0)
		{
			gameObject2 = Spawn(isActive);
		}
		else
		{
			gameObject2 = PrefabDeActiveList[0];
			PrefabDeActiveList.RemoveAt(0);
			PrefabActiveList.Add(gameObject2);
		}
		gameObject2.GetComponent<objType>().Show();
		gameObject2.SetActive(isActive);
		return gameObject2;
	}

	public void DeSpawn(GameObject prefabToPool)
	{
		CheckListIsFull(PrefabDeActiveList);
		prefabToPool.SetActive(false);
		PrefabActiveList.Remove(prefabToPool);
		PrefabDeActiveList.Add(prefabToPool);
		prefabToPool.transform.SetParent(PrefabCollection.transform, false);
	}

	public bool CheckListIsNull(List<GameObject> listNeedToCheck)
	{
		return listNeedToCheck.Count == 0;
	}

	public bool CheckListIsFull(List<GameObject> listNeedToCheck)
	{
		return listNeedToCheck.Count == listNeedToCheck.Capacity;
	}
}
