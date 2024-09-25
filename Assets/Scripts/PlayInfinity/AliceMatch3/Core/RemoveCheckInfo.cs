using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public class RemoveCheckInfo
	{
		public ElementType type;

		public List<Element> list;

		public List<Element> otherList;

		public Board board;

		public Element owner;

		public List<Element> vList;

		public List<Element> hList;

		public List<Element> fList;

		public int totalWeight;

		public RemoveCheckInfo(ElementType type, List<Element> list, Board board, Element owner)
		{
			this.type = type;
			this.list = ((list == null) ? null : ProcessTool.DelReapet(list));
			this.board = board;
			this.owner = owner;
			if (list != null)
			{
				foreach (Element item in list)
				{
					totalWeight += item.getWeight();
				}
			}
			otherList = new List<Element>();
		}

		public void AutoMatch()
		{
			if (GameLogic.Instance.TotleMoveCount >= GameLogic.Instance.levelData.move)
			{
				return;
			}
			foreach (Element item in list)
			{
				if (item == null)
				{
					return;
				}
			}
			foreach (Element other in otherList)
			{
				if (other == null)
				{
					return;
				}
			}
			foreach (Element item2 in list)
			{
				if (item2 == owner && !item2.IsBomb())
				{
					Element element = otherList[0];
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, item2, element, true, Direction.Down)));
					break;
				}
				if (item2.IsBomb())
				{
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.ProcessDoubleClick, new ProcessDoubleClick(board, item2)));
					break;
				}
			}
		}

		public void Tip()
		{
			foreach (Element item2 in list)
			{
				if (item2 == null)
				{
					DebugUtils.Log(DebugType.Other, item2.row + "   " + item2.col + "   is null!");
					GameLogic.Instance.isTiping = false;
					return;
				}
			}
			foreach (Element other in otherList)
			{
				if (other == null)
				{
					DebugUtils.Log(DebugType.Other, other.row + "   " + other.col + "   is null!");
					GameLogic.Instance.isTiping = false;
					return;
				}
			}
			foreach (Element item in list)
			{
				Sequence sequence = UpdateManager.Instance.GetSequence();
				if (item == owner && !item.IsBomb())
				{
					Element element = otherList[0];
					Vector3 zero = Vector3.zero;
					float num = 0.12f;
					if (element.row > item.row)
					{
						zero.y += num;
					}
					else if (element.row < item.row)
					{
						zero.y -= num;
					}
					else if (element.col > item.col)
					{
						zero.x += num;
					}
					else if (element.col < item.col)
					{
						zero.x -= num;
					}
					Vector3 start = item.transform.localPosition;
					sequence.Append(item.transform.DOScale(new Vector3(1.07f, 1.07f, 0f), 0.6f).SetEase(Ease.Linear));
					sequence.Join(item.transform.DOLocalMove(start + zero, 0.6f));
					sequence.Append(item.transform.DOScale(new Vector3(1f, 1f, 1f), 0.6f).SetEase(Ease.Linear));
					sequence.Join(item.transform.DOLocalMove(start, 0.6f));
					sequence.SetLoops(-1).OnKill(delegate
					{
						if (item != null)
						{
							item.transform.localScale = Vector3.one;
							item.transform.localPosition = start;
						}
					});
				}
				else if (!item.IsBomb())
				{
					sequence.Append(item.transform.DOScale(new Vector3(1.07f, 1.07f, 0f), 0.6f));
					sequence.Append(item.transform.DOScale(new Vector3(1f, 1f, 1f), 0.6f));
					sequence.SetLoops(-1).OnKill(delegate
					{
						if (item != null)
						{
							item.transform.localScale = Vector3.one;
						}
					});
				}
				else
				{
					sequence.Append(item.transform.DOScale(new Vector3(1.07f, 1.07f, 0f), 0.6f).SetEase(Ease.OutQuad));
					sequence.Append(item.transform.DOScale(new Vector3(1f, 1f, 1f), 0.6f).SetEase(Ease.OutQuad));
					sequence.SetLoops(-1).OnKill(delegate
					{
						if (item != null)
						{
							item.transform.localScale = Vector3.one;
						}
					});
				}
				int row = item.row;
				int col = item.col;
				board.cells[row, col].ActiveTipEffect(sequence);
			}
			foreach (Element other2 in otherList)
			{
				int row2 = other2.row;
				int col2 = other2.col;
				board.cells[row2, col2].ActiveTipEffect();
			}
		}
	}
}
