using DG.Tweening;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class DropAndMoveTool
	{
		public static void DoDropMove(Board board, Cell currentCell, bool startMove = false, Cell PreviousCell = null)
		{
			Sequence sequence = UpdateManager.Instance.GetSequence();
			Cell cell = ((PreviousCell == null) ? GetHaveElementAndCellTool.GetPreviousCell(board, currentCell) : PreviousCell);
			Cell[,] cells = board.cells;
			if (cell == null && !currentCell.Blocked() && currentCell.element == null)
			{
				int row = currentCell.row;
				int col = currentCell.col;
				GameObject gameObject = ElementGenerator.Instance.Create(board, GetHaveElementAndCellTool.RandomCreateElement(board, currentCell), row, col);
				gameObject.transform.SetParent(board.container.transform);
				gameObject.transform.localPosition = ProcessTool.GetPositionByDirection(board, row, col, cells[row, col].GetDirection());
				gameObject.name = "elem_" + row + "_" + col;
				gameObject.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 15;
				cells[row, col].element = gameObject.GetComponent<Element>();
				cells[row, col].element.board = board;
				cells[row, col].element.row = row;
				cells[row, col].element.col = col;
				cells[row, col].element.moving = true;
				cells[row, col].element.isMoveToTargetCell = true;
				UpdateManager.Instance.GetSequence();
				Cell NextCell = cells[row, col];
				Element elem2 = cells[row, col].element;
				elem2.SpeedUp();
				elem2.transform.DOKill();
				Vector3 Pos = ProcessTool.GetPosition(board, row, col);
				elem2.transform.DOGameTweenLocalMove(Pos, elem2.moveSpeed).SetEase(Ease.Linear).SetSpeedBased()
					.OnComplete(delegate
					{
						elem2.moving = false;
						elem2.isMoveToTargetCell = false;
						if (!CheckDrop(board, NextCell))
						{
							NextCell.element.ResetSpeed();
							elem2.ResetSpeed();
							if (GameLogic.Instance.BombAutoBomb && NextCell.element.IsBomb() && !NextCell.element.exploded)
							{
								NextCell.element.Explode(NextCell.HaveGrass());
							}
							else if (NextCell.element.type >= ElementType.Standard_0 && NextCell.element.type <= ElementType.Standard_6)
							{
								GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
								if (GameLogic.Instance.RemoveTrue == ElementType.None)
								{
									NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
								}
							}
							else
							{
								if (NextCell.element.type == ElementType.Jewel)
								{
									GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
									if (GameLogic.Instance.RemoveTrue != ElementType.None)
									{
										return;
									}
									if (GetHaveElementAndCellTool.CheckCellInNextArea(board, NextCell.element))
									{
										Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
									}
								}
								NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
							}
						}
					});
			}
			else
			{
				if (cell == null || cell.element == null || cell.element.removed || cell.element.exploded || cell.Blocked() || cell.element.isReadyToBomb)
				{
					return;
				}
				Element element = cell.element;
				int row2 = cell.row;
				int col2 = cell.col;
				if (GetHaveElementAndCellTool.HaveSpaceToMove(board, cell))
				{
					bool num = GetHaveElementAndCellTool.HavePreviousCell(board, cell);
					bool flag = GetHaveElementAndCellTool.HaveNextCell(board, cell);
					bool flag2 = GetHaveElementAndCellTool.HaveDownLeftCell(board, cell);
					bool flag3 = GetHaveElementAndCellTool.HaveDownRightCell(board, cell);
					Cell cell2 = null;
					Cell cell3 = null;
					Cell cell4 = null;
					if (num)
					{
						GetHaveElementAndCellTool.GetPreviousCell(board, cell);
					}
					if (flag)
					{
						cell2 = GetHaveElementAndCellTool.GetNextCell(board, cell);
					}
					if (flag2)
					{
						cell3 = GetHaveElementAndCellTool.GetDownLeftCell(board, cell);
					}
					if (flag3)
					{
						cell4 = GetHaveElementAndCellTool.GetDownRightCell(board, cell);
					}
					bool flag4 = flag2 && !cell3.Blocked() && cell3.element == null && GetHaveElementAndCellTool.getHaveObjInThisCol(board, cell3);
					bool flag5 = flag3 && !cell4.Blocked() && cell4.element == null && GetHaveElementAndCellTool.getHaveObjInThisCol(board, cell4);
					if (cell2 != null && !cell2.Blocked() && cell2.element == null)
					{
						MoveOneElement(board, cell, cell2, sequence, true);
					}
					else if (flag4 && !flag5)
					{
						MoveOneElement(board, cell, cell3, sequence);
					}
					else if (!flag4 && flag5)
					{
						MoveOneElement(board, cell, cell4, sequence);
					}
					else if (flag4 && flag5)
					{
						MoveOneElement(board, cell, (Random.Range(0, 2) == 0) ? cell4 : cell3, sequence);
					}
					else
					{
						GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, element);
					}
				}
				else
				{
					float y = element.transform.localPosition.y;
					sequence.Append(element.transform.DOScale(new Vector3(1.12f, 0.88f, 1f), 0.12f).SetEase(Ease.Linear));
					sequence.Join(element.transform.DOLocalMoveY(y - 0.08f, 0.12f).SetEase(Ease.Linear));
					sequence.Append(element.transform.DOScale(new Vector3(1f, 1f, 1f), 0.12f).SetEase(Ease.Linear));
					sequence.Join(element.transform.DOLocalMoveY(y, 0.12f).SetEase(Ease.Linear));
				}
			}
		}

		public static Cell MoveOneElement(Board board, Cell CurrentCell, Cell NextCell, Sequence seq, bool isNextCell = false)
		{
			CurrentCell.element.moving = true;
			CurrentCell.element.isMoveToTargetCell = true;
			NextCell.element = CurrentCell.element;
			CurrentCell.element.name = "elem_" + NextCell.row + "_" + NextCell.col;
			NextCell.element.row = NextCell.row;
			NextCell.element.col = NextCell.col;
			CurrentCell.element.SpeedUp();
			NextCell.element.transform.DOKill();
			Vector3 Pos = ProcessTool.GetPosition(board, NextCell.row, NextCell.col);
			if (!isNextCell || (isNextCell && GetHaveElementAndCellTool.CellsIsNeighbor(NextCell, CurrentCell)))
			{
				NextCell.element.transform.DOGameTweenLocalMove(Pos, NextCell.element.moveSpeed).SetEase(Ease.Linear).SetSpeedBased()
					.OnComplete(delegate
					{
						if (NextCell.element == null)
						{
							DebugUtils.LogError(DebugType.Other, " Element is Null, May Change MovingCount!");
						}
						else
						{
							NextCell.element.moving = false;
							NextCell.element.isMoveToTargetCell = false;
							if (!CheckDrop(board, NextCell))
							{
								NextCell.element.ResetSpeed();
								if (GameLogic.Instance.BombAutoBomb && NextCell.element.IsBomb() && !NextCell.element.exploded)
								{
									NextCell.element.Explode(NextCell.HaveGrass());
								}
								else if (NextCell.element.type >= ElementType.Standard_0 && NextCell.element.type <= ElementType.Standard_6)
								{
									GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
									if (GameLogic.Instance.RemoveTrue == ElementType.None)
									{
										NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
									}
								}
								else
								{
									if (NextCell.element.type == ElementType.Jewel)
									{
										GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
										if (GameLogic.Instance.RemoveTrue != ElementType.None)
										{
											return;
										}
										if (GetHaveElementAndCellTool.CheckCellInNextArea(board, NextCell.element))
										{
											Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
										}
									}
									NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
								}
							}
						}
					});
			}
			else
			{
				NextCell.element.removed = true;
				Vector3 positionByDirection = ProcessTool.GetPositionByDirection(board, CurrentCell.row, CurrentCell.col, CurrentCell.GetPreviousDirection());
				Vector3 PosTwo = ProcessTool.GetPosition(board, NextCell.row, NextCell.col);
				switch (NextCell.GetDirection())
				{
				case Direction.Left:
					PosTwo = ProcessTool.GetPositionByDirection(board, NextCell.row, NextCell.col, Direction.Left);
					break;
				case Direction.Right:
					PosTwo = ProcessTool.GetPositionByDirection(board, NextCell.row, NextCell.col, Direction.Right);
					break;
				case Direction.Down:
					PosTwo = ProcessTool.GetPositionByDirection(board, NextCell.row, NextCell.col, Direction.Down);
					break;
				case Direction.Up:
					PosTwo = ProcessTool.GetPositionByDirection(board, NextCell.row, NextCell.col, Direction.Up);
					break;
				}
				NextCell.element.transform.DOGameTweenLocalMove(positionByDirection, CurrentCell.element.moveSpeed).SetEase(Ease.Linear).SetSpeedBased()
					.OnComplete(delegate
					{
						NextCell.element.SpeedUp();
						NextCell.element.transform.localPosition = PosTwo;
						NextCell.element.transform.DOLocalMove(Pos, NextCell.element.moveSpeed).SetEase(Ease.Linear).SetSpeedBased()
							.OnComplete(delegate
							{
								if (!(NextCell.element == null))
								{
									NextCell.element.removed = false;
									NextCell.element.moving = false;
									NextCell.element.isMoveToTargetCell = false;
									if (!CheckDrop(board, NextCell))
									{
										NextCell.element.ResetSpeed();
										if (GameLogic.Instance.BombAutoBomb && NextCell.element.IsBomb() && !NextCell.element.exploded)
										{
											NextCell.element.Explode(NextCell.HaveGrass());
										}
										else if (NextCell.element.type >= ElementType.Standard_0 && NextCell.element.type <= ElementType.Standard_6)
										{
											GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
											if (GameLogic.Instance.RemoveTrue == ElementType.None)
											{
												NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
											}
										}
										else
										{
											if (NextCell.element.type == ElementType.Jewel)
											{
												GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, NextCell.element);
												if (GameLogic.Instance.RemoveTrue != ElementType.None)
												{
													return;
												}
												if (GetHaveElementAndCellTool.CheckCellInNextArea(board, NextCell.element))
												{
													Singleton<MessageDispatcher>.Instance().SendMessage(5u, board);
												}
											}
											NextCell.element.transform.DOLocalJump(Pos, -0.06f, 1, 0.3f);
										}
									}
								}
							});
					});
			}
			CurrentCell.element = null;
			return NextCell;
		}

		public static bool CheckDrop(Board board, Cell cell)
		{
			if (GetHaveElementAndCellTool.HaveNextCell(board, cell))
			{
				Cell nextCell = GetHaveElementAndCellTool.GetNextCell(board, cell);
				if (nextCell != null && !nextCell.Blocked() && nextCell.element == null)
				{
					DoDropMove(board, nextCell, true, cell);
					return true;
				}
			}
			else if (cell.element != null && cell.element.type == ElementType.Jewel)
			{
				RemoveMatchTool.RemoveMatch(board, cell.element);
			}
			if (GetHaveElementAndCellTool.HaveDownLeftCell(board, cell))
			{
				Cell downLeftCell = GetHaveElementAndCellTool.GetDownLeftCell(board, cell);
				GetHaveElementAndCellTool.GetLeftCell(board, cell);
				if (downLeftCell != null && !downLeftCell.Blocked() && downLeftCell.element == null && GetHaveElementAndCellTool.getHaveObjInThisCol(board, downLeftCell))
				{
					DoDropMove(board, downLeftCell, true, cell);
					return true;
				}
			}
			if (GetHaveElementAndCellTool.HaveDownRightCell(board, cell))
			{
				Cell downRightCell = GetHaveElementAndCellTool.GetDownRightCell(board, cell);
				GetHaveElementAndCellTool.GetRightCell(board, cell);
				if (downRightCell != null && !downRightCell.Blocked() && downRightCell.element == null && GetHaveElementAndCellTool.getHaveObjInThisCol(board, downRightCell))
				{
					DoDropMove(board, downRightCell, true, cell);
					return true;
				}
			}
			return false;
		}

		public static void CheckUpDrop(Board board, Cell cell)
		{
			bool flag = GetHaveElementAndCellTool.HavePreviousCell(board, cell);
			bool flag2 = GetHaveElementAndCellTool.HaveUpLeftCell(board, cell);
			bool flag3 = GetHaveElementAndCellTool.HaveUpRightCell(board, cell);
			Cell previousCell = GetHaveElementAndCellTool.GetPreviousCell(board, cell);
			Cell upLeftCell = GetHaveElementAndCellTool.GetUpLeftCell(board, cell);
			Cell upRightCell = GetHaveElementAndCellTool.GetUpRightCell(board, cell);
			if (flag)
			{
				if ((previousCell.element == null && !previousCell.Blocked() && !GetHaveElementAndCellTool.getHaveObjInThisCol(board, cell)) || (previousCell.element != null && !previousCell.Blocked() && (previousCell.element.removed || previousCell.element.moving || previousCell.element.isReadyToBomb) && !GetHaveElementAndCellTool.getHaveObjInThisCol(board, cell)))
				{
					return;
				}
				if (GetHaveElementAndCellTool.GetNextCell(board, previousCell) != cell)
				{
					flag = false;
				}
			}
			if (flag2 && GetHaveElementAndCellTool.GetDownRightCell(board, upLeftCell) != cell && GetHaveElementAndCellTool.GetDownLeftCell(board, upLeftCell) != cell)
			{
				flag2 = false;
			}
			if (flag3 && GetHaveElementAndCellTool.GetDownLeftCell(board, upRightCell) != cell && GetHaveElementAndCellTool.GetDownRightCell(board, upRightCell) != cell)
			{
				flag3 = false;
			}
			bool flag4 = flag && previousCell.element != null && !previousCell.element.removed && !previousCell.element.isReadyToBomb && previousCell.isTopElementClear() && !previousCell.isBox() && !previousCell.isButton() && !previousCell.Blocked();
			bool flag5 = flag2 && upLeftCell.element != null && !upLeftCell.element.removed && !upLeftCell.element.isReadyToBomb && upLeftCell.isTopElementClear() && !upLeftCell.isBox() && !upLeftCell.isButton() && !upLeftCell.Blocked();
			bool flag6 = flag3 && upRightCell.element != null && !upRightCell.element.removed && !upRightCell.element.isReadyToBomb && upRightCell.isTopElementClear() && !upRightCell.isBox() && !upRightCell.isButton() && !upRightCell.Blocked();
			if (!flag)
			{
				DoDropMove(board, cell, true);
			}
			else if (flag4)
			{
				DoDropMove(board, cell, true);
			}
			else if (flag5 && !flag6)
			{
				DoDropMove(board, cell, true, upLeftCell);
			}
			else if (!flag5 && flag6)
			{
				DoDropMove(board, cell, true, upRightCell);
			}
			else if (flag5 && flag6)
			{
				int num = Random.Range(0, 2);
				DoDropMove(board, cell, true, (num == 0) ? upLeftCell : upRightCell);
			}
		}
	}
}
