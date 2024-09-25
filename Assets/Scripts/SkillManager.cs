using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
	private static SkillManager _instance;

	private int requestNum;

	private bool isClosingBook;

	private bool isOpeningBook;

	private bool isSkilling;

	public static SkillManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Start()
	{
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(16u, ActiveSkill);
	}

	private void Update()
	{
		if (!UpdateManager.Instance.isPause && !isSkilling && requestNum > 0)
		{
			OpenBook();
		}
	}

	public bool GetIsSkilling()
	{
		return isSkilling;
	}

	private void OpenBook()
	{
		AudioManager.Instance.PlayAudioEffect("book_open");
		RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ControllBook, true));
		isOpeningBook = true;
		isSkilling = true;
		float time = 0f;
		UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (time > 1f)
			{
				isOpeningBook = false;
				float time2 = 0f;
				float cdTime = 0.2f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration2)
				{
					if (requestNum <= 0)
					{
						CloseBook();
						return true;
					}
					if (time2 > cdTime)
					{
						time2 = 0f;
						requestNum--;
						Attack();
					}
					time2 += duration2;
					return false;
				}));
				return true;
			}
			time += duration;
			return false;
		}));
	}

	private void Attack()
	{
		Board currentBoard = GameLogic.Instance.currentBoard;
		List<Cell> clist = new List<Cell>();
		List<TwoCellNear> twoNormalCellNear = GetHaveElementAndCellTool.GetTwoNormalCellNear(currentBoard, out clist);
		if (clist.Count <= 0)
		{
			DebugUtils.LogError(DebugType.Other, "场景中没有可以放置炸弹的位置");
			return;
		}
		clist = ProcessTool.ListRandom(clist);
		twoNormalCellNear = ProcessTool.ListRandom(twoNormalCellNear);
		List<ActiveSkill> skillInfo = ProcessTool.GetSkillInfo();
		List<ElementType> list = new List<ElementType>();
		int num = 0;
		foreach (ActiveSkill item in skillInfo)
		{
			num = ((item.boomType != ElementType.VH3Bomb && item.boomType != ElementType.DoubleFlyBomb) ? (num + item.num) : (num + item.num * 2));
			for (int i = 0; i < num; i++)
			{
				list.Add(item.boomType);
			}
		}
		Vector3 position = GameSceneUIManager.Instance.ButtonTip.transform.position;
		position.z = 0f;
		List<ElementType> list2 = new List<ElementType>();
		foreach (ElementType item2 in list)
		{
			if (item2 == ElementType.VH3Bomb || item2 == ElementType.DoubleFlyBomb)
			{
				if (twoNormalCellNear.Count >= 1)
				{
					TwoCellNear twoCellNear = twoNormalCellNear[0];
					clist.Remove(twoCellNear.cell1);
					clist.Remove(twoCellNear.cell2);
					clist.Insert(0, twoCellNear.cell1);
					clist.Insert(0, twoCellNear.cell2);
				}
				if (item2 == ElementType.VH3Bomb)
				{
					list2.Add(ElementType.AreaBomb);
					list2.Add(ElementType.VerticalBomb);
				}
				else
				{
					list2.Add(ElementType.FlyBomb);
					list2.Add(ElementType.FlyBomb);
				}
			}
			else
			{
				list2.Add(item2);
			}
		}
		List<Cell> list3 = new List<Cell>();
		for (int j = 0; j < ((num > clist.Count) ? clist.Count : num); j++)
		{
			list3.Add(clist[j]);
		}
		for (int k = 0; k < list3.Count; k++)
		{
			AudioManager.Instance.PlayAudioEffect("moves_to_booster_fly");
			Cell cell = list3[k];
			Vector3 position2 = cell.transform.position;
			position2.z = 0f;
			ElementType type = list2[k];
			switch (type)
			{
			case ElementType.AreaBomb:
				position = GameSceneUIManager.Instance.AreaBombStartFlyPos.position;
				break;
			case ElementType.ColorBomb:
				position = GameSceneUIManager.Instance.ColorBombStartFlyPos.position;
				break;
			case ElementType.FlyBomb:
				position = GameSceneUIManager.Instance.flyBombStartFlyPos.position;
				break;
			case ElementType.HorizontalBomb:
			case ElementType.VerticalBomb:
				position = GameSceneUIManager.Instance.VHBombStartFlyPos.position;
				break;
			}
			float time = Vector3.Distance(position, position2) / 14f;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ActiveSkill, new CollectSkillPower(position, position2, time)));
			currentBoard.movingCount++;
			float time2 = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time2 > time)
				{
					currentBoard.movingCount--;
					DealElementTool.RemoveElement(currentBoard, cell, false, true, false, 0.2f, null, type, ElementType.ColorBomb, true);
					return true;
				}
				time2 += duration;
				return false;
			}));
		}
	}

	private void CloseBook()
	{
		AudioManager.Instance.PlayAudioEffect("book_close");
		RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ControllBook, false));
		isClosingBook = true;
		float time = 0f;
		UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (time > 1f)
			{
				isClosingBook = false;
				isSkilling = false;
				return true;
			}
			time += duration;
			return false;
		}));
	}

	private void ActiveSkill(uint iMessageType, object arg)
	{
		requestNum++;
	}

	private void OnDestroy()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(16u, ActiveSkill);
	}
}
