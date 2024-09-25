using System.Collections.Generic;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class RemoveMatchTool
	{
		public static List<Element> CalVertical(Board board, Element elem, bool isAllArea = false)
		{
			Cell[,] cells = board.cells;
			List<Element> list = new List<Element>();
			int num = (isAllArea ? (board.MaxRow - 1) : board.levelRowEnd);
			int num2 = ((!isAllArea) ? board.levelRowStart : 0);
			if (elem.color < 1 || elem.color > 7 || elem.removed || elem.isWaitForMatch)
			{
				return list;
			}
			for (int i = 1; i < 5 && elem.row + i <= num && cells[elem.row + i, elem.col] != null && !cells[elem.row + i, elem.col].HaveHoney(); i++)
			{
				Element element = cells[elem.row + i, elem.col].element;
				if (!(element != null) || element.color == -1 || element.color != elem.color || element.removed || element.isWaitForMatch)
				{
					break;
				}
				list.Add(element);
			}
			list.Add(elem);
			for (int j = 1; j < 5 && elem.row - j >= num2 && cells[elem.row - j, elem.col].element != null && !cells[elem.row - j, elem.col].HaveHoney(); j++)
			{
				Element element2 = cells[elem.row - j, elem.col].element;
				if (!(element2 != null) || element2.color == -1 || element2.color != elem.color || element2.removed || element2.isWaitForMatch)
				{
					break;
				}
				list.Add(element2);
			}
			list.Sort();
			return list;
		}

		public static List<Element> CalHorizontal(Board board, Element elem, bool isAllArea = false)
		{
			Cell[,] cells = board.cells;
			List<Element> list = new List<Element>();
			if (elem.color < 1 || elem.color > 7 || elem.isWaitForMatch)
			{
				return list;
			}
			int num = (isAllArea ? (board.MaxCol - 1) : board.levelColEnd);
			int num2 = ((!isAllArea) ? board.levelColStart : 0);
			for (int i = 1; i < 5 && elem.col - i >= num2 && cells[elem.row, elem.col - i] != null && !cells[elem.row, elem.col - i].HaveHoney(); i++)
			{
				Element element = cells[elem.row, elem.col - i].element;
				if (!(element != null) || element.color == -1 || element.color != elem.color || element.removed || element.isWaitForMatch)
				{
					break;
				}
				list.Add(element);
			}
			list.Add(elem);
			for (int j = 1; j < 5 && elem.col + j <= num && cells[elem.row, elem.col + j].element != null && !cells[elem.row, elem.col + j].HaveHoney(); j++)
			{
				Element element2 = cells[elem.row, elem.col + j].element;
				if (!(element2 != null) || element2.color == -1 || element2.color != elem.color || element2.removed || element2.isWaitForMatch)
				{
					break;
				}
				list.Add(element2);
			}
			return list;
		}

		public static List<Element> FindFlyBomb(Board board, Element elem, bool isAllArea = false)
		{
			Cell[,] cells = board.cells;
			if (!isAllArea)
			{
				int levelColEnd = board.levelColEnd;
			}
			else
			{
				int maxCol = board.MaxCol;
			}
			int num = ((!isAllArea) ? board.levelColStart : 0);
			int num2 = (isAllArea ? (board.MaxCol - 1) : board.levelColEnd);
			int num3 = ((!isAllArea) ? board.levelRowStart : 0);
			int num4 = (isAllArea ? (board.MaxRow - 1) : board.levelRowEnd);
			Cell cell;
			Element element;
			if (elem.row + 1 <= num4)
			{
				cell = cells[elem.row + 1, elem.col];
				element = cell.element;
			}
			else
			{
				cell = null;
				element = null;
			}
			Cell cell2;
			Element element2;
			if (elem.row - 1 >= num3)
			{
				cell2 = cells[elem.row - 1, elem.col];
				element2 = cell2.element;
			}
			else
			{
				cell2 = null;
				element2 = null;
			}
			Cell cell3;
			Element element3;
			if (elem.col - 1 >= num)
			{
				cell3 = cells[elem.row, elem.col - 1];
				element3 = cell3.element;
			}
			else
			{
				cell3 = null;
				element3 = null;
			}
			Cell cell4;
			Element element4;
			if (elem.col + 1 <= num2)
			{
				cell4 = cells[elem.row, elem.col + 1];
				element4 = cell4.element;
			}
			else
			{
				cell4 = null;
				element4 = null;
			}
			Cell cell5;
			Element element5;
			if (elem.row + 1 <= num4 && elem.col + 1 <= num2)
			{
				cell5 = cells[elem.row + 1, elem.col + 1];
				element5 = cell5.element;
			}
			else
			{
				cell5 = null;
				element5 = null;
			}
			Cell cell6;
			Element element6;
			if (elem.row - 1 >= num3 && elem.col + 1 <= num2)
			{
				cell6 = cells[elem.row - 1, elem.col + 1];
				element6 = cell6.element;
			}
			else
			{
				cell6 = null;
				element6 = null;
			}
			Cell cell7;
			Element element7;
			if (elem.row + 1 <= num4 && elem.col - 1 >= num)
			{
				cell7 = cells[elem.row + 1, elem.col - 1];
				element7 = cell7.element;
			}
			else
			{
				cell7 = null;
				element7 = null;
			}
			Cell cell8;
			Element element8;
			if (elem.row - 1 >= num3 && elem.col - 1 >= num)
			{
				cell8 = cells[elem.row - 1, elem.col - 1];
				element8 = cell8.element;
			}
			else
			{
				element8 = null;
				cell8 = null;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			flag = element != null && element.color == elem.color && !element.removed && !element.isWaitForMatch && (!cell.Blocked() || (cell.Blocked() && cell.HaveBramble()));
			flag2 = element2 != null && element2.color == elem.color && !element2.removed && !element2.isWaitForMatch && (!cell2.Blocked() || (cell2.Blocked() && cell2.HaveBramble()));
			flag3 = element3 != null && element3.color == elem.color && !element3.removed && !element3.isWaitForMatch && (!cell3.Blocked() || (cell3.Blocked() && cell3.HaveBramble()));
			flag4 = element4 != null && element4.color == elem.color && !element4.removed && !element4.isWaitForMatch && (!cell4.Blocked() || (cell4.Blocked() && cell4.HaveBramble()));
			flag6 = element5 != null && element5.color == elem.color && !element5.removed && !element5.isWaitForMatch && (!cell5.Blocked() || (cell5.Blocked() && cell5.HaveBramble()));
			flag7 = element8 != null && element8.color == elem.color && !element8.removed && !element8.isWaitForMatch && (!cell8.Blocked() || (cell8.Blocked() && cell8.HaveBramble()));
			flag8 = element6 != null && element6.color == elem.color && !element6.removed && !element6.isWaitForMatch && (!cell6.Blocked() || (cell6.Blocked() && cell6.HaveBramble()));
			flag5 = element7 != null && element7.color == elem.color && !element7.removed && !element7.isWaitForMatch && (!cell7.Blocked() || (cell7.Blocked() && cell7.HaveBramble()));
			List<Element> list = new List<Element>();
			if (flag && flag3 && flag5)
			{
				list.Add(element);
				list.Add(element3);
				list.Add(element7);
				return list;
			}
			if (flag && flag4 && flag6)
			{
				list.Add(element);
				list.Add(element4);
				list.Add(element5);
				return list;
			}
			if (flag2 && flag7 && flag3)
			{
				list.Add(element2);
				list.Add(element3);
				list.Add(element8);
				return list;
			}
			if (flag2 && flag4 && flag8)
			{
				list.Add(element2);
				list.Add(element4);
				list.Add(element6);
				return list;
			}
			return list;
		}

		public static ElementType RemoveMatch(Board board, Element elem, bool fakeMove = false)
		{
			return Match(board, elem, fakeMove);
		}

		public static ElementType Match(Board board, Element elem, bool fakeMove = false)
		{
			if (elem == null || (!elem.IsStandard() && elem.type != ElementType.Jewel) || elem.isWaitForMatch)
			{
				return ElementType.None;
			}
			if (elem.moving || elem.removed || elem.type == ElementType.Shell || elem.type == ElementType.Treasure_0 || elem.type == ElementType.Treasure_1 || elem.type == ElementType.NullTreasure_0 || elem.type == ElementType.NullTreasure_1)
			{
				return ElementType.None;
			}
			if (!GetHaveElementAndCellTool.CheckCellInCrrentArea(board, elem))
			{
				return ElementType.None;
			}
			Cell[,] cells = board.cells;
			if (elem.type == ElementType.Jewel)
			{
				Cell cell = cells[elem.row, elem.col];
				if (cell.isTail && cell.HaveJewel())
				{
					GameLogic.Instance.canDropJewel = false;
					List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
					list.Add(new ElementRemoveInfo(cell, false, false));
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
					return ElementType.Jewel;
				}
				return ElementType.None;
			}
			bool flag = false;
			bool flag2 = false;
			bool grassFlag = false;
			new List<Element>();
			List<Element> list2 = CalVertical(board, elem);
			list2.Sort();
			List<Element> list3 = CalHorizontal(board, elem);
			int count = list2.Count;
			int count2 = list3.Count;
			List<Element> list4 = FindFlyBomb(board, elem);
			foreach (Element item in list2)
			{
				if (cells[item.row, item.col].HaveGrass())
				{
					grassFlag = true;
					flag = true;
					break;
				}
			}
			foreach (Element item2 in list3)
			{
				if (cells[item2.row, item2.col].HaveGrass())
				{
					grassFlag = true;
					flag2 = true;
					break;
				}
			}
			foreach (Element item3 in list4)
			{
				if (cells[item3.row, item3.col].HaveGrass())
				{
					grassFlag = true;
					break;
				}
			}
			bool flag3 = false;
			ElementType elementType = ElementType.None;
			if (count >= 5 || count2 >= 5)
			{
				flag3 = true;
				elementType = ElementType.ColorBomb;
			}
			else if ((count == 3 && (count2 == 3 || count2 == 4)) || (count == 4 && (count2 == 3 || count2 == 4)))
			{
				flag3 = true;
				elementType = ElementType.AreaBomb;
			}
			else if (count2 == 4)
			{
				flag3 = true;
				elementType = ElementType.VerticalBomb;
			}
			else if (count == 4)
			{
				flag3 = true;
				elementType = ElementType.HorizontalBomb;
			}
			else if (list4.Count != 0 && count >= 2 && count <= 3 && count2 >= 2 && count2 <= 3)
			{
				flag3 = true;
				elementType = ElementType.FlyBomb;
			}
			else if ((count == 3 && count2 < 3) || (count < 3 && count2 == 3))
			{
				elementType = ElementType.Standard_0;
			}
			List<Element> allElementList = new List<Element>();
			if (elementType == ElementType.FlyBomb)
			{
				allElementList.AddRange(list2);
				allElementList.AddRange(list3);
				allElementList.AddRange(list4);
			}
			else
			{
				if (count > 2)
				{
					allElementList.AddRange(list2);
				}
				if (count2 > 2)
				{
					allElementList.AddRange(list3);
				}
			}
			ProcessTool.DelReapet(allElementList);
			if (elementType == ElementType.None || allElementList.Count <= 2)
			{
				return ElementType.None;
			}
			Element element = elem;
			if (elementType != ElementType.ColorBomb)
			{
				foreach (Element item4 in allElementList)
				{
					RemoveCheckInfo removeCheckInfo = RemoveMatchCheck(board, item4);
					ElementType type = removeCheckInfo.type;
					if (type != ElementType.None && type > elementType)
					{
						element = item4;
					}
				}
			}
			if (element != elem)
			{
				return RemoveMatch(board, element);
			}
			List<Cell> allCellList = new List<Cell>(allElementList.Count);
			for (int i = 0; i < allElementList.Count; i++)
			{
				allCellList.Add(board.cells[allElementList[i].row, allElementList[i].col]);
			}
			bool flag4 = false;
			foreach (Element item5 in allElementList)
			{
				if (item5.moving)
				{
					flag4 = true;
				}
			}
			if (flag4)
			{
				bool isCanMatch = false;
				foreach (Element item6 in allElementList)
				{
					item6.isWaitForMatch = true;
				}
				int color = elem.color;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					isCanMatch = true;
					foreach (Cell item7 in allCellList)
					{
						if (item7.element == null || item7.element.color != color)
						{
							foreach (Element item8 in allElementList)
							{
								if (item8 != null)
								{
									item8.isWaitForMatch = false;
								}
							}
							elem.isWaitForMatch = false;
							RemoveMatch(board, elem);
							return true;
						}
						if (item7.element.moving)
						{
							isCanMatch = false;
						}
					}
					if (isCanMatch)
					{
						foreach (Element item9 in allElementList)
						{
							if (item9 != null)
							{
								item9.isWaitForMatch = false;
							}
						}
						elem.isWaitForMatch = false;
						RemoveMatch(board, elem);
						return true;
					}
					return false;
				}));
				return ElementType.None;
			}
			if (elementType != ElementType.None)
			{
				GameLogic.Instance.matchNum++;
			}
			if (flag3)
			{
				if (elementType == ElementType.FlyBomb)
				{
					ProcessTool.GetPosition(board, elem.row, elem.col);
					List<ElementRemoveInfo> list5 = new List<ElementRemoveInfo>();
					if (count >= 3)
					{
						list4.AddRange(list2);
					}
					if (count2 >= 3)
					{
						list4.AddRange(list3);
					}
					List<Element> list6 = ProcessTool.DelReapet(list4);
					list5.Add(new ElementRemoveInfo(cells[elem.row, elem.col], false, false, grassFlag, 0.2f, null, ElementType.FlyBomb));
					foreach (Element item10 in list6)
					{
						if (item10 != elem && !item10.moving)
						{
							Cell cell2 = cells[item10.row, item10.col];
							if (!cell2.HaveBramble())
							{
								item10.moving = true;
								item10.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 14;
								list5.Add(new ElementRemoveInfo(cell2, false, false, grassFlag));
							}
							else
							{
								item10.moving = true;
								list5.Add(new ElementRemoveInfo(cell2, false, false, grassFlag));
							}
						}
					}
					int num = 1;
					foreach (ElementRemoveInfo item11 in list5)
					{
						if (!item11.cell.Blocked() && item11.ChangeToBomb == ElementType.None)
						{
							num++;
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CreateOneMoveToBomb, new CreateOneMoveToBomb(board, (int)elem.type, item11, elem.transform.position, Singleton<PlayGameData>.Instance().gameConfig.CreateBombSpeed)));
						}
					}
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementListToCreateBomb, new RemoveElementListToBombMessage(list5, 0f, board, (int)elem.type, num)));
				}
				else
				{
					List<ElementRemoveInfo> list7 = new List<ElementRemoveInfo>();
					list7.Add(new ElementRemoveInfo(cells[elem.row, elem.col], false, false, flag ? flag : flag2, 0.1f, null, elementType));
					if (count >= 3)
					{
						foreach (Element item12 in list2)
						{
							if (item12 != elem)
							{
								Cell cell3 = cells[item12.row, item12.col];
								item12.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 14;
								if (!cell3.HaveBramble())
								{
									item12.moving = true;
									list7.Add(new ElementRemoveInfo(cell3, false, false, (elementType == ElementType.AreaBomb) ? (flag || flag2) : flag));
								}
								else
								{
									item12.moving = true;
									list7.Add(new ElementRemoveInfo(cell3, false, false, (elementType == ElementType.AreaBomb) ? (flag || flag2) : flag));
								}
							}
						}
					}
					if (count2 >= 3)
					{
						foreach (Element item13 in list3)
						{
							if (item13 != elem)
							{
								Cell cell4 = cells[item13.row, item13.col];
								item13.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 14;
								if (!cell4.HaveBramble())
								{
									item13.moving = true;
									list7.Add(new ElementRemoveInfo(cell4, false, false, (elementType == ElementType.AreaBomb) ? (flag || flag2) : flag2));
								}
								else
								{
									item13.moving = true;
									list7.Add(new ElementRemoveInfo(cell4, false, false, (elementType == ElementType.AreaBomb) ? (flag || flag2) : flag2));
								}
							}
						}
					}
					int num2 = 1;
					foreach (ElementRemoveInfo item14 in list7)
					{
						if (!item14.cell.Blocked() && item14.ChangeToBomb == ElementType.None)
						{
							num2++;
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CreateOneMoveToBomb, new CreateOneMoveToBomb(board, (int)elem.type, item14, elem.transform.position, Singleton<PlayGameData>.Instance().gameConfig.CreateBombSpeed)));
						}
						else if (item14.ChangeToBomb != ElementType.None)
						{
							item14.cell.element.isReadyToBomb = true;
						}
					}
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementListToCreateBomb, new RemoveElementListToBombMessage(list7, 0f, board, (int)elem.type, num2)));
				}
				float time = 0f;
				float totalTime = Singleton<PlayGameData>.Instance().gameConfig.CreateBombSpeed;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
				{
					if ((double)time > (double)totalTime - 0.2)
					{
						RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.MixElement, new MixElement(elem.transform)));
						return true;
					}
					time += deltaTime;
					return false;
				}));
				elem.transform.DOGameTweenScale(Vector3.one, 0f);
				return elementType;
			}
			if (elementType == ElementType.Standard_0)
			{
				List<Element> obj = ((count == 3) ? list2 : list3);
				List<ElementRemoveInfo> list8 = new List<ElementRemoveInfo>();
				foreach (Element item15 in obj)
				{
					item15.moving = true;
					list8.Add(new ElementRemoveInfo(cells[item15.row, item15.col], false, true, (count == 3) ? flag : flag2));
				}
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list8, 0f, board)));
				return elementType;
			}
			return ElementType.None;
		}

		public static RemoveCheckInfo RemoveMatchCheck(Board board, Element elem, bool isAllArea = false)
		{
			List<Element> list = CalVertical(board, elem, isAllArea);
			List<Element> list2 = CalHorizontal(board, elem, isAllArea);
			List<Element> list3 = FindFlyBomb(board, elem, isAllArea);
			list3.Add(elem);
			int count = list.Count;
			int count2 = list2.Count;
			ElementType type = ElementType.None;
			if (list3.Count == 1 && count < 3 && count2 < 3)
			{
				return new RemoveCheckInfo(type, null, board, elem);
			}
			if (count >= 5 || count2 >= 5)
			{
				type = ElementType.ColorBomb;
				return new RemoveCheckInfo(type, (count >= 5) ? list : list2, board, elem);
			}
			if ((count2 == 3 && count >= 3) || (count == 3 && count2 >= 3))
			{
				type = ElementType.AreaBomb;
				list2.InsertRange(list2.Count, list);
				list2 = ProcessTool.DelReapet(list2);
				return new RemoveCheckInfo(type, list2, board, elem);
			}
			if (count2 == 4)
			{
				type = ElementType.VerticalBomb;
				return new RemoveCheckInfo(type, list2, board, elem);
			}
			if (count == 4)
			{
				type = ElementType.HorizontalBomb;
				return new RemoveCheckInfo(type, list, board, elem);
			}
			if (list3.Count != 1)
			{
				type = ElementType.FlyBomb;
				list3.AddRange(list);
				list3.AddRange(list2);
				list3 = ProcessTool.DelReapet(list3);
				return new RemoveCheckInfo(type, list3, board, elem);
			}
			if ((count == 3 && count2 < 3) || (count < 3 && count2 == 3))
			{
				type = ElementType.Standard_0;
				return new RemoveCheckInfo(type, (count == 3) ? list : list2, board, elem);
			}
			return new RemoveCheckInfo(type, null, board, elem);
		}
	}
}
