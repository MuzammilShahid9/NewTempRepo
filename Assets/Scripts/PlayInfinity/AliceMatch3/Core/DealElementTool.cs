using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class DealElementTool
	{
		public static void RemoveRow(Board board, int row, int col, bool grassFlag, ElementType removeFromType = ElementType.None)
		{
			Cell[,] cells = board.cells;
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			Element element = cells[row, col].element;
			if (cells[row, col].HaveHoney() || cells[row, col].HaveBox() || cells[row, col].HaveButton() || cells[row, col].HaveTreasure())
			{
				grassFlag = false;
			}
			bool flag = grassFlag;
			bool flag2 = grassFlag;
			List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.HBombExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
			if (element != null && !element.moving && !element.removed)
			{
				if (element.IsBomb())
				{
					element.moving = true;
				}
				list.Add(new ElementRemoveInfo(cells[row, col], false, true, grassFlag, 0.1f, new BombInfo(ElementType.HorizontalBomb, row)));
			}
			bool flag3 = true;
			bool flag4 = true;
			for (int i = 1; i <= levelColEnd; i++)
			{
				if (col - i >= levelColStart && flag3)
				{
					Cell cell = cells[row, col - i];
					if (!flag)
					{
						flag = !cell.HaveHoney() && !cells[row, col].HaveBox() && !cells[row, col].HaveButton() && !cells[row, col].HaveTreasure() && cell.HaveGrass();
					}
					list.Add(new ElementRemoveInfo(cell, false, true, flag, 0.1f, new BombInfo(ElementType.HorizontalBomb, row)));
					if (cell.element != null && cell.isTopElementClear() && cell.element.type == ElementType.Shell && !cell.element.isReadyToRemove && !cell.element.removed)
					{
						flag3 = false;
						cell.element.isReadyToRemove = true;
					}
				}
				if (col + i <= levelColEnd && flag4)
				{
					Cell cell2 = cells[row, col + i];
					if (!flag2)
					{
						flag2 = !cell2.HaveHoney() && !cells[row, col].HaveBox() && !cells[row, col].HaveButton() && !cells[row, col].HaveTreasure() && cell2.HaveGrass();
					}
					list.Add(new ElementRemoveInfo(cell2, false, true, flag2, 0.1f, new BombInfo(ElementType.HorizontalBomb, row)));
					if (cell2.element != null && cell2.isTopElementClear() && cell2.element.type == ElementType.Shell && !cell2.element.isReadyToRemove && !cell2.element.removed)
					{
						flag4 = false;
						cell2.element.isReadyToRemove = true;
					}
				}
			}
			UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0.02f, board, 0.24f, null, removeFromType)));
		}

		public static void RemoveCol(Board board, int row, int col, bool grassFlag, ElementType removeFromType = ElementType.None)
		{
			Cell[,] cells = board.cells;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Element element = cells[row, col].element;
			if (cells[row, col].HaveHoney() || cells[row, col].HaveBox() || cells[row, col].HaveButton() || cells[row, col].HaveTreasure())
			{
				grassFlag = false;
			}
			bool flag = grassFlag;
			bool flag2 = grassFlag;
			List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
			bool flag3 = true;
			bool flag4 = true;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.VBombExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
			if (element != null && !element.moving && !element.removed)
			{
				list.Add(new ElementRemoveInfo(cells[row, col], false, true, grassFlag, 0.1f, new BombInfo(ElementType.VerticalBomb, col)));
				if (element.IsBomb())
				{
					element.moving = true;
				}
			}
			for (int i = 1; i <= levelRowEnd; i++)
			{
				if (row - i >= levelRowStart && flag4)
				{
					Cell cell = cells[row - i, col];
					if (!flag)
					{
						flag = !cell.HaveHoney() && !cells[row, col].HaveBox() && !cells[row, col].HaveButton() && !cells[row, col].HaveTreasure() && cell.HaveGrass();
					}
					list.Add(new ElementRemoveInfo(cell, false, true, flag, 0.1f, new BombInfo(ElementType.VerticalBomb, col)));
					if (cell.element != null && cell.isTopElementClear() && cell.element.type == ElementType.Shell && !cell.element.isReadyToRemove && !cell.element.removed)
					{
						flag4 = false;
						cell.element.isReadyToRemove = true;
					}
				}
				if (row + i <= levelRowEnd && flag3)
				{
					Cell cell2 = cells[row + i, col];
					if (!flag2)
					{
						flag2 = !cell2.HaveHoney() && !cells[row, col].HaveBox() && !cells[row, col].HaveButton() && !cells[row, col].HaveTreasure() && cell2.HaveGrass();
					}
					list.Add(new ElementRemoveInfo(cell2, false, true, flag2, 0.1f, new BombInfo(ElementType.VerticalBomb, col)));
					if (cell2.element != null && cell2.isTopElementClear() && cell2.element.type == ElementType.Shell && !cell2.element.isReadyToRemove && !cell2.element.removed)
					{
						flag3 = false;
						cell2.element.isReadyToRemove = true;
					}
				}
			}
			UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0.02f, board, 0.24f, null, removeFromType)));
		}

		public static void RemoveArea(Board board, int row, int col, int size, bool grassFlag)
		{
			int gap = ((size % 2 != 0) ? ((size - 1) / 2) : (size / 2));
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Cell[,] cells = board.cells;
			if (size <= 5)
			{
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.AreaBombExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
				List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
				for (int i = col - gap; i <= col + gap; i++)
				{
					for (int num = row + gap; num >= row - gap; num--)
					{
						if (num >= levelRowStart && num <= levelRowEnd && i >= levelColStart && i <= levelColEnd)
						{
							Cell cell = cells[num, i];
							list.Add(new ElementRemoveInfo(cell, false, true, grassFlag, 0.1f, new BombInfo(ElementType.AreaBomb, new Vector2(i, num), new Vector2(row - gap, col + gap))));
						}
					}
				}
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
				return;
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CombineAreaBombExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
			Element elem = cells[row, col].element;
			elem.moving = true;
			float time1 = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time1 > 0.05f)
				{
					if (cells[row, col].element != null && cells[row, col].element == elem)
					{
						board.cells[row, col].element.moving = false;
						List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
						for (int j = col - gap; j <= col + gap; j++)
						{
							for (int num2 = row + gap; num2 >= row - gap; num2--)
							{
								if (num2 >= levelRowStart && num2 <= levelRowEnd && j >= levelColStart && j <= levelColEnd)
								{
									Cell cell2 = cells[num2, j];
									list2.Add(new ElementRemoveInfo(cell2, false, true, grassFlag, 0.1f, new BombInfo(ElementType.AreaBomb, new Vector2(j, num2), new Vector2(row - gap, col + gap))));
								}
							}
						}
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
					}
					return true;
				}
				time1 += duration;
				return false;
			}));
		}

		public static void RemoveAll(Board board, int row, int col, bool grassFlag)
		{
			Cell[,] cells = board.cells;
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Dictionary<int, List<Cell>> dic = GetHaveElementAndCellTool.GetBombRemoveList(board, row, col, 12);
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CombineColorBombExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
			Element elem = cells[row, col].element;
			elem.moving = true;
			elem.transform.localScale = Vector3.zero;
			float time1 = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time1 > 1f)
				{
					if (cells[row, col].element != null && cells[row, col].element == elem)
					{
						board.cells[row, col].element.moving = false;
						List<ElementRemoveInfo> list = new List<ElementRemoveInfo>
						{
							new ElementRemoveInfo(cells[row, col], false, true, grassFlag, 0.1f, new BombInfo(ElementType.AreaBomb, new Vector2(levelRowEnd, levelColStart), new Vector2(levelRowStart, levelColEnd)))
						};
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
					}
					int circleNum = dic.Count;
					int currentCircleNum = 0;
					float time2 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration2)
					{
						if (time2 > (float)currentCircleNum * 0.07f)
						{
							currentCircleNum++;
							List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
							foreach (Cell item in dic[currentCircleNum])
							{
								list2.Add(new ElementRemoveInfo(item, false, true, grassFlag, 0.1f, new BombInfo(ElementType.AreaBomb, new Vector2(levelRowEnd, levelColStart), new Vector2(levelRowStart, levelColEnd))));
							}
							UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
							if (currentCircleNum == circleNum)
							{
								return true;
							}
						}
						time2 += duration2;
						return false;
					}));
					return true;
				}
				time1 += duration;
				return false;
			}));
		}

		public static void FlyRemove(Board board, int row, int col, bool grassFlag)
		{
			Cell[,] cells = board.cells;
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CombinBeesExplode, new BombExplode(cells[row, col].transform.position, board.container.transform)));
			List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
			if (row + 1 <= levelRowEnd)
			{
				list.Add(new ElementRemoveInfo(cells[row + 1, col], false, true, grassFlag));
			}
			if (row - 1 >= levelRowStart)
			{
				list.Add(new ElementRemoveInfo(cells[row - 1, col], false, true, grassFlag));
			}
			if (col + 1 <= levelColEnd)
			{
				list.Add(new ElementRemoveInfo(cells[row, col + 1], false, true, grassFlag));
			}
			if (col - 1 >= levelColStart)
			{
				list.Add(new ElementRemoveInfo(cells[row, col - 1], false, true, grassFlag));
			}
			list.Add(new ElementRemoveInfo(cells[row, col], true, true, grassFlag));
			UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
		}

		public static void FlyRemoveOneTarger(Board board, int row, int col, int num, ElementType type, bool grassFlag)
		{
			Cell[,] cells = board.cells;
			Element element = cells[row, col].element;
			if (element != null)
			{
				element.transform.localScale = Vector3.zero;
				element.moving = true;
				List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
				list.Add(new ElementRemoveInfo(board.cells[row, col], true, true, grassFlag, 0.1f));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
			}
			else
			{
				Debug.Log(row + " |null| " + col);
			}
			List<Cell> flyRemoveTarget = GetFlyRemoveTarget(board, row, col, num, grassFlag);
			float checkTime = 0f;
			foreach (Cell target in flyRemoveTarget)
			{
				int trow = target.row;
				int tcol = target.col;
				Vector3 position = ProcessTool.GetPosition(board, trow, tcol);
				board.movingCount++;
				float flyTime = Vector3.Distance(position, ProcessTool.GetPosition(board, row, col)) / Singleton<PlayGameData>.Instance().gameConfig.BeeBoomFlySpeed;
				SingleBeeExplod message = new SingleBeeExplod(cells[row, col].transform.position, board.container.transform, ProcessTool.GetPosition(board, row, col), new Vector2Int(row, col), ProcessTool.GetPosition(board, trow, tcol), checkTime, flyTime, 99f, Singleton<PlayGameData>.Instance().gameConfig.BeeBoomFlySpeed, board, type);
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.SingleBeeExplod, message));
				float time = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time > checkTime + flyTime)
					{
						RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.BeeHitOne, new BeeHitOne(target.transform.position, type, board.container.transform)));
						if (type == ElementType.AreaBomb)
						{
							RemoveArea(board, trow, tcol, Singleton<PlayGameData>.Instance().gameConfig.AreaBombNum, grassFlag);
						}
						else if (type == ElementType.HorizontalBomb)
						{
							RemoveRow(board, trow, tcol, grassFlag);
						}
						else if (type == ElementType.VerticalBomb)
						{
							RemoveCol(board, trow, tcol, grassFlag);
						}
						else if (type == ElementType.ColorBomb)
						{
							if (target.element != null)
							{
								RemoveColor(board, target.element.color, target, grassFlag);
							}
						}
						else
						{
							List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>
							{
								new ElementRemoveInfo(target, false, false, grassFlag)
							};
							UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
						}
						board.movingCount--;
						return true;
					}
					time += duration;
					return false;
				}));
			}
		}

		public static List<Cell> GetFlyRemoveTarget(Board board, int row, int col, int num = 1, bool grassFlag = false)
		{
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Cell[,] cells = board.cells;
			List<Cell> list = new List<Cell>();
			for (int i = levelRowStart; i <= levelRowEnd; i++)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Insert(0, "\t");
				for (int num2 = levelColEnd; num2 >= levelColStart; num2--)
				{
					Cell cell = cells[i, num2];
					if (cell.empty || (i == row && num2 == col) || (cell.element != null && cell.element.removed))
					{
						stringBuilder.Insert(0, "\t|\t");
					}
					else
					{
						stringBuilder.Insert(0, cell.CalWeight(grassFlag) + "\t|\t");
						if (!cell.isBeeFlyToThis)
						{
							if (list.Count < num)
							{
								cell.isBeeFlyToThis = true;
								list.Add(cell);
								cell.CalWeight(grassFlag);
							}
							else
							{
								int num3 = cell.CalWeight(grassFlag);
								list.Sort((Cell x, Cell y) => x.totalWeight.CompareTo(y.totalWeight));
								if (list[0].totalWeight < num3)
								{
									list[0].isBeeFlyToThis = false;
									list[0] = cell;
									cell.isBeeFlyToThis = true;
								}
							}
						}
					}
				}
				stringBuilder.Insert(0, "\t");
				DebugUtils.Log(DebugType.UI, stringBuilder.ToString());
			}
			return list;
		}

		public static void RemoveColor(Board board, int color, Cell colorSelf, bool grassFlag, ElementType type = ElementType.None)
		{
			colorSelf.element.moving = true;
			colorSelf.element.removed = true;
			colorSelf.element.transform.localScale = Vector3.zero;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			List<ElementRemoveInfo> bombList = new List<ElementRemoveInfo>();
			List<Vector3> list = new List<Vector3>();
			List<Element> list2 = new List<Element>();
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Cell[,] cells = board.cells;
			float delayTime = 0f;
			for (int i = levelRowStart; i <= levelRowEnd; i++)
			{
				for (int j = levelColStart; j <= levelColEnd; j++)
				{
					Cell cell = cells[i, j];
					Element element = cell.element;
					if (cell.empty || !(element != null) || element.color != color || element.isReadyToBomb || element.moving || element.removed || (cell.Blocked() && (!cell.Blocked() || !cell.HaveBramble())))
					{
						continue;
					}
					element.exploded = false;
					element.moving = true;
					element.removed = true;
					bombList.Add(new ElementRemoveInfo(cell, false, true, cell.HaveGrass() || grassFlag, 0.1f, null, ElementType.None, ElementType.ColorBomb));
					list.Add(element.transform.position);
					if (type == ElementType.None)
					{
						if (!cell.HaveBramble())
						{
						}
					}
					else
					{
						list2.Add(element);
					}
				}
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ColorBombExplode, new ColorBombExplode(colorSelf.transform.position, board, list, list2, type)));
			int bombListCount = bombList.Count;
			if (type == ElementType.None)
			{
				delayTime = 0f;
			}
			else
			{
				delayTime = 0.05f;
				for (int num = bombList.Count - 1; num >= 0; num--)
				{
					if (bombList[num].cell.HaveBramble())
					{
						bombList.RemoveAt(num);
					}
				}
			}
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.06f * (float)bombListCount + 1f)
				{
					if (colorSelf.element != null)
					{
						colorSelf.element.moving = false;
						colorSelf.element.removed = false;
					}
					List<ElementRemoveInfo> list3 = new List<ElementRemoveInfo>
					{
						new ElementRemoveInfo(colorSelf, false, false, grassFlag)
					};
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list3, 0f, board)));
					foreach (ElementRemoveInfo item in bombList)
					{
						if (item != null && item.cell.element != null)
						{
							item.cell.element.removed = false;
						}
					}
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(bombList, delayTime, board, 0f, delegate
					{
						Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
					})));
					return true;
				}
				time += duration;
				return false;
			}));
		}

		public static void RemoveElement(Board board, Cell cell, bool force = false, bool showAnim = true, bool grassFlag = false, float delay = 0.2f, BombInfo bombInfo = null, ElementType ChangeToBomb = ElementType.None, ElementType RemoveFrom = ElementType.None, bool isCollect = false)
		{
			if (cell.empty)
			{
				return;
			}
			Element elem = cell.element;
			Element topElement = cell.topElement;
			if (cell.HaveBramble())
			{
				ProcessTool.ProcessBramble(board, topElement);
			}
			else if (cell.HaveHoney())
			{
				ProcessTool.ProcessHoney(board, topElement);
			}
			else if (elem != null && !elem.moving)
			{
				if (elem.color == 22)
				{
					ProcessTool.ProcessJewel(board, cell);
				}
				else if (elem.color == 24)
				{
					ProcessTool.ProcessCat(board, cell);
				}
				else if (elem.color == 25)
				{
					ProcessTool.ProcessFish(board, cell);
				}
				else if (elem.color != 23)
				{
					if (cell.HaveBox())
					{
						ProcessTool.ProcessBox(board, cell.element);
					}
					else if (cell.HaveButton())
					{
						ProcessTool.ProcessButton(board, cell.element);
					}
					else if (cell.HaveTreasure())
					{
						if (cell.element.color >= 51 && elem.color <= 52)
						{
							ProcessTool.ProcessTreasure(board, cell.element);
						}
						else
						{
							ProcessTool.ProcessNullTreasure(board, cell.element);
						}
					}
					else if (cell.HaveWhitCloud())
					{
						ProcessTool.ProcessWhiteCloud(board, cell.element);
					}
					else if (cell.HaveBlackCloud())
					{
						ProcessTool.ProcessBlackCloud(board, cell.element);
					}
					else if (cell.HaveShell())
					{
						ProcessTool.ProcessShell(board, cell.element);
					}
					else if (elem.IsBomb() && !elem.exploded && !force)
					{
						elem.Explode(grassFlag);
					}
					else
					{
						if (grassFlag)
						{
							ProcessTool.ProcessGrass(board, cell);
						}
						else
						{
							ProcessTool.ProcessIce(board, cell.bottomElement);
						}
						if (!elem.removed && !elem.moving)
						{
							elem.removed = true;
							if (!GameLogic.Instance.isFinish)
							{
								ProcessTool.UpdateSkillPower(board, elem.type, cell);
							}
							if (ChangeToBomb != ElementType.None)
							{
								ProcessTool.ProcessRound(board, cell, bombInfo);
								if (isCollect && !ProcessTool.isCollectFinish(elem.color))
								{
									ProcessTool.UpdateTargetState(board, elem.color, 0, elem);
								}
								elem.removed = false;
								elem.CreateBomb(ChangeToBomb, grassFlag, true, RemoveFrom != ElementType.ColorBomb);
							}
							else
							{
								ProcessTool.ProcessRound(board, cell, bombInfo);
								elem.gameObject.SetActive(false);
								Debug.Log(elem.color + "     **********");
								if (isCollect && !ProcessTool.isCollectFinish(elem.color))
								{
									ProcessTool.UpdateTargetState(board, elem.color, 0, elem);
								}
								else if (showAnim)
								{
									RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.RemoveElement, new RemoveElement(elem, elem.transform.position)));
								}
								float time = 0f;
								UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
								{
									if (time > delay)
									{
										if (elem != null && cell.element != elem)
										{
											elem.gameObject.SetActive(true);
											DebugUtils.Log(DebugType.Other, elem.row + " , " + elem.col + "  处删除时，cell内的element已经不是当初的element，被重复删除多遍！！！！！！！");
										}
										else
										{
											elem.moving = false;
											if (elem.IsStandard())
											{
												GameLogic.Instance.RemoveNum++;
											}
											if (elem != null && elem.gameObject != null)
											{
												Object.Destroy(elem.gameObject);
											}
											if (cell.element == elem)
											{
												cell.element = null;
											}
										}
										return true;
									}
									time += deltaTime;
									return false;
								}));
							}
						}
					}
				}
			}
			else if (grassFlag)
			{
				ProcessTool.ProcessGrass(board, cell);
			}
			else
			{
				ProcessTool.ProcessIce(board, cell.bottomElement);
			}
			cell.isBeeFlyToThis = false;
		}

		public static void CombineExplode(Board board, Element elem1, Element elem2, bool grassFlag)
		{
			elem1.exploded = true;
			elem2.exploded = true;
			Cell[,] cells = board.cells;
			Cell cell = cells[elem1.row, elem1.col];
			Cell cell2 = cells[elem2.row, elem2.col];
			if ((elem1.type == ElementType.HorizontalBomb || elem1.type == ElementType.VerticalBomb) && (elem2.type == ElementType.VerticalBomb || elem2.type == ElementType.HorizontalBomb))
			{
				AudioManager.Instance.PlayAudioEffect("boosters_rocket_double");
				RemoveRow(board, elem1.row, elem1.col, grassFlag);
				RemoveCol(board, elem1.row, elem1.col, grassFlag);
			}
			else if (((elem1.type == ElementType.HorizontalBomb || elem1.type == ElementType.VerticalBomb) && elem2.type == ElementType.AreaBomb) || ((elem2.type == ElementType.HorizontalBomb || elem2.type == ElementType.VerticalBomb) && elem1.type == ElementType.AreaBomb))
			{
				List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
				list.Add(new ElementRemoveInfo(cell2, false, true, cells[elem2.row, elem2.col].HaveGrass()));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CombinAreaAndVHExplode, new BombExplode(cell.transform.position, board.container.transform)));
				Element elem3 = cell.element;
				elem3.moving = true;
				elem3.transform.localScale = Vector3.zero;
				float time3 = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration3)
				{
					if (time3 > 1f)
					{
						elem3.moving = false;
						AudioManager.Instance.PlayAudioEffect("boosters_rocket_bomb");
						if (elem1.row + 1 <= board.MaxRow - 1)
						{
							RemoveRow(board, elem1.row + 1, elem1.col, grassFlag, ElementType.VH3Bomb);
						}
						RemoveRow(board, elem1.row, elem1.col, grassFlag, ElementType.VH3Bomb);
						if (elem1.row - 1 >= 0)
						{
							RemoveRow(board, elem1.row - 1, elem1.col, grassFlag, ElementType.VH3Bomb);
						}
						board.movingCount++;
						float time5 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time5 > 1.5f)
							{
								board.movingCount--;
								return true;
							}
							time5 += duration;
							return false;
						}));
						float time4 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time4 > 0.6f)
							{
								if (elem1.col + 1 <= board.MaxCol - 1)
								{
									RemoveCol(board, elem1.row, elem1.col + 1, grassFlag, ElementType.VH3Bomb);
								}
								RemoveCol(board, elem1.row, elem1.col, grassFlag, ElementType.VH3Bomb);
								if (elem1.col - 1 >= 0)
								{
									RemoveCol(board, elem1.row, elem1.col - 1, grassFlag, ElementType.VH3Bomb);
								}
								return true;
							}
							time4 += duration;
							return false;
						}));
						return true;
					}
					time3 += duration3;
					return false;
				}));
			}
			else if (elem1.type == ElementType.AreaBomb && elem2.type == ElementType.AreaBomb)
			{
				RemoveArea(board, elem1.row, elem1.col, Singleton<PlayGameData>.Instance().gameConfig.CombineAreaBombNum, grassFlag);
			}
			else if (elem1.type == ElementType.FlyBomb && elem2.type == ElementType.FlyBomb)
			{
				FlyRemove(board, elem1.row, elem1.col, grassFlag);
				FlyRemoveOneTarger(board, elem1.row, elem1.col, Singleton<PlayGameData>.Instance().gameConfig.CombineBeeBombNum * GameLogic.Instance.BeeLevel, ElementType.None, grassFlag);
			}
			else if ((elem1.type == ElementType.FlyBomb && elem2.type == ElementType.VerticalBomb) || (elem2.type == ElementType.FlyBomb && elem1.type == ElementType.VerticalBomb))
			{
				if (elem1.type == ElementType.FlyBomb)
				{
					Element element = elem1;
				}
				List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
				list2.Add(new ElementRemoveInfo(cell, true, true, grassFlag, 0.1f));
				list2.Add(new ElementRemoveInfo(cell2, true, true, grassFlag, 0.1f));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
				int num = GameLogic.Instance.BeeLevel - 1;
				int beeBombNum = Singleton<PlayGameData>.Instance().gameConfig.BeeBombNum;
				FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum, ElementType.VerticalBomb, grassFlag);
				for (int i = 0; i < num; i++)
				{
					FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum, ElementType.None, grassFlag);
				}
			}
			else if ((elem1.type == ElementType.FlyBomb && elem2.type == ElementType.HorizontalBomb) || (elem2.type == ElementType.FlyBomb && elem1.type == ElementType.HorizontalBomb))
			{
				int num2 = GameLogic.Instance.BeeLevel - 1;
				List<ElementRemoveInfo> list3 = new List<ElementRemoveInfo>();
				list3.Add(new ElementRemoveInfo(cell, true, true, grassFlag, 0.1f));
				list3.Add(new ElementRemoveInfo(cell2, true, true, grassFlag, 0.1f));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list3, 0f, board)));
				int beeBombNum2 = Singleton<PlayGameData>.Instance().gameConfig.BeeBombNum;
				FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum2, ElementType.HorizontalBomb, grassFlag);
				for (int j = 0; j < num2; j++)
				{
					FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum2, ElementType.None, grassFlag);
				}
			}
			else if ((elem1.type == ElementType.FlyBomb && elem2.type == ElementType.ColorBomb) || (elem2.type == ElementType.FlyBomb && elem1.type == ElementType.ColorBomb))
			{
				Cell obj = ((elem1.type == ElementType.FlyBomb) ? cell : cell2);
				Cell colorSelf = ((elem1.type == ElementType.ColorBomb) ? cell : cell2);
				int randomColorInBoard = (int)GetHaveElementAndCellTool.GetRandomColorInBoard(board);
				obj.element.color = randomColorInBoard;
				obj.element.type = (ElementType)randomColorInBoard;
				RemoveColor(board, randomColorInBoard, colorSelf, grassFlag, ElementType.FlyBomb);
			}
			else if ((elem1.type == ElementType.FlyBomb && elem2.type == ElementType.AreaBomb) || (elem2.type == ElementType.FlyBomb && elem1.type == ElementType.AreaBomb))
			{
				int num3 = GameLogic.Instance.BeeLevel - 1;
				List<ElementRemoveInfo> list4 = new List<ElementRemoveInfo>();
				list4.Add(new ElementRemoveInfo(cell, true, true, grassFlag, 0.1f));
				list4.Add(new ElementRemoveInfo(cell2, true, true, grassFlag, 0.1f));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list4, 0f, board)));
				int beeBombNum3 = Singleton<PlayGameData>.Instance().gameConfig.BeeBombNum;
				FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum3, ElementType.AreaBomb, grassFlag);
				for (int k = 0; k < num3; k++)
				{
					FlyRemoveOneTarger(board, elem1.row, elem1.col, beeBombNum3, ElementType.None, grassFlag);
				}
			}
			else if (((elem1.type == ElementType.VerticalBomb || elem1.type == ElementType.HorizontalBomb) && elem2.type == ElementType.ColorBomb) || ((elem2.type == ElementType.VerticalBomb || elem2.type == ElementType.HorizontalBomb) && elem1.type == ElementType.ColorBomb))
			{
				Cell obj2 = ((elem1.type == ElementType.VerticalBomb || elem1.type == ElementType.HorizontalBomb) ? cell : cell2);
				Cell colorSelf2 = ((elem1.type == ElementType.ColorBomb) ? cell : cell2);
				int randomColorInBoard2 = (int)GetHaveElementAndCellTool.GetRandomColorInBoard(board);
				obj2.element.color = randomColorInBoard2;
				obj2.element.type = (ElementType)randomColorInBoard2;
				RemoveColor(board, randomColorInBoard2, colorSelf2, grassFlag, ElementType.RandomVorHBomb);
			}
			else if ((elem1.type == ElementType.AreaBomb && elem2.type == ElementType.ColorBomb) || (elem2.type == ElementType.AreaBomb && elem1.type == ElementType.ColorBomb))
			{
				Cell obj3 = ((elem1.type == ElementType.AreaBomb) ? cell : cell2);
				Cell colorSelf3 = ((elem1.type == ElementType.ColorBomb) ? cell : cell2);
				int randomColorInBoard3 = (int)GetHaveElementAndCellTool.GetRandomColorInBoard(board);
				obj3.element.color = randomColorInBoard3;
				obj3.element.type = (ElementType)randomColorInBoard3;
				RemoveColor(board, randomColorInBoard3, colorSelf3, grassFlag, ElementType.AreaBomb);
			}
			else if (elem1.type == ElementType.ColorBomb && elem2.type == ElementType.ColorBomb)
			{
				List<ElementRemoveInfo> list5 = new List<ElementRemoveInfo>();
				list5.Add(new ElementRemoveInfo(cell2, false, true, cells[elem2.row, elem2.col].HaveGrass()));
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list5, 0f, board)));
				RemoveAll(board, elem1.row, elem1.col, grassFlag);
			}
		}

		public static bool SwitchElement(Board board, Element elem1, Element elem2, bool checkMatch, bool isUseGlove = false)
		{
			Cell[,] cells = board.cells;
			if (cells[elem1.row, elem1.col].Blocked() || cells[elem2.row, elem2.col].Blocked())
			{
				return false;
			}
			if (cells[elem2.row, elem2.col].element == null || cells[elem1.row, elem1.col].element == null)
			{
				return false;
			}
			if (cells[elem2.row, elem2.col].element.removed || cells[elem1.row, elem1.col].element.removed)
			{
				return false;
			}
			if (cells[elem2.row, elem2.col].element.moving || cells[elem1.row, elem1.col].element.moving)
			{
				return false;
			}
			if (elem1.color > 22 || elem2.color > 22)
			{
				if (elem1.color <= 22)
				{
					if (elem2.color > 22 && !elem2.IsTreasure())
					{
					}
				}
				else if (elem2.color <= 22)
				{
					if (elem1.color > 22 && !elem1.IsTreasure())
					{
					}
				}
				else if (!elem1.IsTreasure() || !elem2.IsTreasure())
				{
					return false;
				}
			}
			int row = elem1.row;
			int col = elem1.col;
			Vector3 position = ProcessTool.GetPosition(board, elem2.row, elem2.col);
			Vector3 position2 = ProcessTool.GetPosition(board, elem1.row, elem1.col);
			cells[elem2.row, elem2.col].element = elem1;
			cells[row, col].element = elem2;
			elem1.row = elem2.row;
			elem1.col = elem2.col;
			elem1.name = "element" + elem1.row + "_" + elem1.col;
			elem2.row = row;
			elem2.col = col;
			elem2.name = "element" + elem2.row + "_" + elem2.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.SwitchElement, new SwitchElement(position, position2, board.container.transform)));
			elem1.moving = true;
			elem1.transform.DOGameTweenLocalMove(position, 0.2f).OnComplete(delegate
			{
				elem1.moving = false;
				if (!elem2.moving)
				{
					if (isUseGlove)
					{
						isUseGlove = false;
						if (elem2.IsJewel() || elem1.IsJewel())
						{
							Element element2 = (elem1.IsJewel() ? elem1 : elem2);
							Element elem4 = (elem1.IsJewel() ? elem2 : elem1);
							GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, element2);
							RemoveMatchTool.RemoveMatch(board, elem4);
							if (GameLogic.Instance.RemoveTrue == ElementType.None && GetHaveElementAndCellTool.CheckCellInNextArea(board, element2))
							{
								Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
							}
						}
						else
						{
							RemoveMatchTool.RemoveMatch(board, elem1);
							RemoveMatchTool.RemoveMatch(board, elem2);
						}
					}
					else
					{
						DealSwitchElementEvent(board, elem1, elem2, checkMatch);
					}
				}
			});
			elem2.moving = true;
			elem2.transform.DOGameTweenLocalMove(position2, 0.2f).OnComplete(delegate
			{
				elem2.moving = false;
				if (!elem1.moving)
				{
					if (isUseGlove)
					{
						isUseGlove = false;
						if (elem2.IsJewel() || elem1.IsJewel())
						{
							Element element = (elem1.IsJewel() ? elem1 : elem2);
							Element elem3 = (elem1.IsJewel() ? elem2 : elem1);
							GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, element);
							RemoveMatchTool.RemoveMatch(board, elem3);
							if (GameLogic.Instance.RemoveTrue == ElementType.None && GetHaveElementAndCellTool.CheckCellInNextArea(board, element))
							{
								Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
							}
						}
						else
						{
							RemoveMatchTool.RemoveMatch(board, elem1);
							RemoveMatchTool.RemoveMatch(board, elem2);
						}
					}
					else
					{
						DealSwitchElementEvent(board, elem1, elem2, checkMatch);
					}
				}
			});
			return true;
		}

		public static void DealSwitchElementEvent(Board board, Element elem1, Element elem2, bool checkMatch)
		{
			bool flag = true;
			new List<Element>();
			new List<Element>();
			bool flag2 = false;
			bool flag3 = false;
			Cell[,] cells = board.cells;
			bool grassFlag = cells[elem1.row, elem1.col].HaveGrass() || cells[elem2.row, elem2.col].HaveGrass();
			if (elem1.IsBomb() && elem2.IsBomb())
			{
				flag = false;
				CombineExplode(board, elem1, elem2, grassFlag);
				flag2 = true;
				flag3 = true;
			}
			else if (elem1.IsBomb())
			{
				if (!elem2.IsJewel())
				{
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, elem2);
					flag3 = GameLogic.Instance.RemoveTrue != ElementType.None;
				}
				elem1.Explode(grassFlag, (elem2.IsBomb() || elem2.IsJewel() || elem2.IsShell() || elem2.IsTreasure()) ? (-1) : elem2.color);
				flag = false;
				flag2 = true;
			}
			else if (elem2.IsBomb())
			{
				if (!elem1.IsJewel())
				{
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, elem1);
					flag2 = GameLogic.Instance.RemoveTrue != ElementType.None;
				}
				flag = false;
				elem2.Explode(grassFlag, (elem1.IsBomb() || elem1.IsJewel() || elem1.IsShell() || elem2.IsTreasure()) ? (-1) : elem1.color);
				flag3 = true;
			}
			else
			{
				if (!elem1.IsJewel())
				{
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, elem1);
					flag2 = GameLogic.Instance.RemoveTrue != ElementType.None;
				}
				if (!elem2.IsJewel())
				{
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, elem2);
					flag3 = GameLogic.Instance.RemoveTrue != ElementType.None;
				}
			}
			if (!flag2 && !flag3)
			{
				flag = true;
			}
			else
			{
				flag = false;
				if (elem2.IsJewel() || elem1.IsJewel())
				{
					Element element = (elem1.IsJewel() ? elem1 : elem2);
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, element);
					if (GameLogic.Instance.RemoveTrue == ElementType.None && element.type == ElementType.Jewel && GetHaveElementAndCellTool.CheckCellInNextArea(board, element))
					{
						Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
					}
				}
			}
			if (checkMatch)
			{
				if (flag)
				{
					AudioManager.Instance.PlayAudioEffect("moving_fail");
					Direction dirToElements = GetHaveElementAndCellTool.GetDirToElements(elem1, elem2);
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, elem1, elem2, false, dirToElements)));
				}
				else
				{
					board.moveStep++;
					GameLogic.Instance.TotleMoveCount++;
					GameLogic.Instance.isMoveCountReduce = true;
				}
			}
			else
			{
				DropAndMoveTool.CheckDrop(board, cells[elem1.row, elem1.col]);
				DropAndMoveTool.CheckDrop(board, cells[elem2.row, elem2.col]);
			}
		}
	}
}
