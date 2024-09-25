using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
	private static PoolManager _ins;

	private GameObject poolObjParent;

	public static PoolManager Ins
	{
		get
		{
			return _ins ?? (_ins = new PoolManager());
		}
	}

	public Dictionary<int, OnePrefabPool> PoolScriptsDic { get; set; }

	public PoolManager()
	{
		PoolScriptsDic = new Dictionary<int, OnePrefabPool>();
	}

	public void AddObjPool(int name, GameObject Obj, int Num = 2)
	{
		PoolScriptsDic.Add(name, new OnePrefabPool(name, Obj, poolObjParent, Num));
	}

	public void ClearDic()
	{
		PoolScriptsDic.Clear();
	}

	public void Init()
	{
		ClearDic();
		poolObjParent = new GameObject
		{
			name = "PoolObjParent"
		};
		UnityEngine.Object.DontDestroyOnLoad(poolObjParent);
	}

	public void DeSpawnAllEffect()
	{
		foreach (KeyValuePair<int, OnePrefabPool> item in PoolScriptsDic)
		{
			for (int num = item.Value.PrefabActiveList.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = item.Value.PrefabActiveList[num];
				if (gameObject != null)
				{
					if (gameObject.transform.name == "DX_MToB")
					{
						DebugUtils.Log(DebugType.Other, "#####");
					}
					objType component = gameObject.GetComponent<objType>();
					if (component != null && PoolScriptsDic.ContainsKey(component.type))
					{
						PoolScriptsDic[component.type].DeSpawn(gameObject);
					}
					else
					{
						UnityEngine.Object.Destroy(gameObject);
					}
				}
			}
		}
	}

	public void PauseEffect()
	{
		foreach (KeyValuePair<int, OnePrefabPool> item in PoolScriptsDic)
		{
			for (int num = item.Value.PrefabActiveList.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = item.Value.PrefabActiveList[num];
				if (gameObject != null)
				{
					gameObject.GetComponent<objType>().Pause();
				}
			}
		}
	}

	public void PlayEffect()
	{
		foreach (KeyValuePair<int, OnePrefabPool> item in PoolScriptsDic)
		{
			for (int num = item.Value.PrefabActiveList.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = item.Value.PrefabActiveList[num];
				if (gameObject != null)
				{
					gameObject.GetComponent<objType>().Play();
				}
			}
		}
	}

	public GameObject SpawnEffect(int name, Transform parent, bool isActive = true)
	{
		GameObject gameObject = PoolScriptsDic[name].Spawn(isActive);
		if (parent != null)
		{
			gameObject.transform.SetParent(parent, false);
			if (UpdateManager.Instance.isPause)
			{
				gameObject.GetComponent<objType>().Pause();
			}
		}
		return gameObject;
	}

	public GameObject SpawnEffect(int name, bool isActive = true)
	{
		return PoolScriptsDic[name].Spawn(isActive);
	}

	public void DeSpawnEffect(GameObject obj, float time = 0f, Action action = null)
	{
		if (obj == null)
		{
			return;
		}
		float currentTime = 0f;
		float allTime = time;
		GameObject o = obj;
		string name = o.transform.name;
		UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (currentTime >= allTime)
			{
				if (o != null)
				{
					objType component = o.GetComponent<objType>();
					if (action != null)
					{
						action();
					}
					if (component != null && PoolScriptsDic.ContainsKey(component.type))
					{
						if (PoolScriptsDic[component.type].PrefabActiveList.Contains(o))
						{
							PoolScriptsDic[component.type].DeSpawn(o);
						}
					}
					else
					{
						UnityEngine.Object.Destroy(o);
					}
				}
				return true;
			}
			currentTime += duration;
			return false;
		}));
	}
}
