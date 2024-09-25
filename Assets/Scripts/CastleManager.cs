using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CastleManager : MonoBehaviour
{
	public List<RoomManager> rooms;

	private static CastleManager instance;

	public static CastleManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		for (int i = 0; i < rooms.Count; i++)
		{
			if (rooms[i] != null)
			{
				rooms[i].Enter(i + 1);
			}
		}
	}

	private void Start()
	{
		DealOldPlayerData();
		DealUnlockRoom();
	}

	private void DealUnlockRoom()
	{
		for (int i = 0; i < UserDataManager.Instance.GetService().UnlockRoomIDList.Count; i++)
		{
			ShowRoom(UserDataManager.Instance.GetService().UnlockRoomIDList[i]);
		}
	}

	private void DealOldPlayerData()
	{
		if (JudgeUnlockRoom())
		{
			UserDataManager.Instance.AddUnlockRoomInfo(4);
			UserDataManager.Instance.Save();
		}
	}

	private bool JudgeUnlockRoom()
	{
		if (UserDataManager.Instance.GetService().stage >= 8)
		{
			return true;
		}
		if (UserDataManager.Instance.GetService().stage == 7)
		{
			string[] array = UserDataManager.Instance.GetService().finishTaskString.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == "15")
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ShowRoom(int roomIndex)
	{
		SpriteRenderer[] componentsInChildren = rooms[roomIndex].transform.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.activeInHierarchy)
			{
				componentsInChildren[i].color = new Color(1f, 1f, 1f, 1f);
			}
		}
		Tilemap[] componentsInChildren2 = rooms[roomIndex].transform.GetComponentsInChildren<Tilemap>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].gameObject.activeInHierarchy)
			{
				componentsInChildren2[j].color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	public RoomManager GetRoom(int roomIndex)
	{
		return rooms[roomIndex];
	}

	public ItemAnim GetItem(int roomIndex, int itemIndex)
	{
		if (roomIndex < rooms.Count)
		{
			RoomManager roomManager = rooms[roomIndex];
			if (itemIndex < roomManager.items.Count)
			{
				return roomManager.items[itemIndex];
			}
			return null;
		}
		return null;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			UnlockRoom(4);
		}
	}

	public void UnlockRoom(int roomIndex)
	{
		Debug.Log("UnlockRoom" + roomIndex);
		SpriteRenderer[] componentsInChildren = rooms[roomIndex].transform.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.activeInHierarchy)
			{
				componentsInChildren[i].DOColor(new Color(1f, 1f, 1f, 1f), 0.5f);
			}
		}
		Tilemap[] componentsInChildren2 = rooms[roomIndex].transform.GetComponentsInChildren<Tilemap>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].gameObject.activeInHierarchy)
			{
				StartCoroutine(TilemapDoColor(componentsInChildren2[j]));
			}
		}
		rooms[roomIndex].ShowUnlockEffect();
		StartCoroutine(DelayFinishPlot(roomIndex));
	}

	private IEnumerator DelayFinishPlot(int roomIndex)
	{
		float seconds = ((rooms[roomIndex].UnlockEffectTime > GeneralConfig.RoomUnlockFadeInTime) ? rooms[roomIndex].UnlockEffectTime : GeneralConfig.RoomUnlockFadeInTime);
		yield return new WaitForSeconds(seconds);
		rooms[roomIndex].HideUnlockEffect();
		PlotRoomUnlockManager.Instance.FinishStep();
	}

	private IEnumerator TilemapDoColor(Tilemap tilemap)
	{
		float timer = 0f;
		yield return null;
		Color color = tilemap.color;
		float flyTime = GeneralConfig.RoomUnlockFadeInTime;
		while (timer <= flyTime)
		{
			float r = Mathf.Lerp(tilemap.color.r, 1f, timer / flyTime);
			float g = Mathf.Lerp(tilemap.color.g, 1f, timer / flyTime);
			float b = Mathf.Lerp(tilemap.color.b, 1f, timer / flyTime);
			float a = Mathf.Lerp(tilemap.color.a, 1f, timer / flyTime);
			tilemap.color = new Color(r, g, b, a);
			timer += Time.deltaTime;
			yield return null;
		}
		tilemap.color = new Color(1f, 1f, 1f, 1f);
	}
}
