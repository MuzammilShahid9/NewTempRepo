using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Editor;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class ProcessTool
	{
		public static Vector3 GetPosition(Board board, int row, int col)
		{
			float x = 0.78f * (float)col + board.offsetx;
			float y = 0.78f * (float)row + board.offsety;
			return new Vector3(x, y, 0f);
		}

		public static void ProcessBramble(Board board, Element elem)
		{
			if (!(elem != null))
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			Vector3 position = elem.transform.position;
			elem.color = (cell.DestroyBlock ? 9000 : (elem.color - 1000));
			int color = elem.color;
			cell.DestroyBlock = false;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessBramble, new ProcessBramble(elem, position)));
			board.movingCount++;
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (color < 10000)
					{
						cell.topElement = null;
						if (cell.element == null)
						{
							DropAndMoveTool.CheckUpDrop(board, cell);
						}
						else if (cell.element != null && !cell.element.removed && !cell.element.moving)
						{
							if (GameLogic.Instance.BombAutoBomb && cell.element.IsBomb() && !cell.element.exploded)
							{
								cell.element.Explode(cell.HaveGrass());
							}
							else if (cell.element.type == ElementType.Shell)
							{
								RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ActiveShell, new ActiveShellInfo(cell.element, true)));
								DropAndMoveTool.CheckDrop(board, cell);
							}
							else
							{
								DropAndMoveTool.CheckDrop(board, cell);
							}
						}
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessHoney(Board board, Element elem)
		{
			if (!(elem != null))
			{
				return;
			}
			Cell[,] cells = board.cells;
			Cell cell = cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 19999 : (elem.color - 1000));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessHoney, new ProcessHoney(elem, position)));
			board.movingCount++;
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (color < 20000)
					{
						cell.topElement = null;
						if (cell.HaveButterfly())
						{
							ProcessButterFly(board, cell.element);
						}
						else if (cell.element == null)
						{
							DropAndMoveTool.CheckUpDrop(board, cell);
						}
						else if (cell.element != null)
						{
							if (GameLogic.Instance.BombAutoBomb && cell.element.IsBomb() && !cell.element.exploded)
							{
								cell.element.Explode(cell.HaveGrass());
							}
							else if (cell.element.type == ElementType.Shell)
							{
								RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ActiveShell, new ActiveShellInfo(cell.element, true)));
								DropAndMoveTool.CheckDrop(board, cell);
							}
							else if (!DropAndMoveTool.CheckDrop(board, cell))
							{
								RemoveMatchTool.RemoveMatch(board, cell.element);
							}
						}
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessIce(Board board, Element elem)
		{
			if (!(elem != null) || elem.color < 100 || elem.color > 300)
			{
				return;
			}
			elem.color -= 100;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessIce, new ProcessIce(elem, position, elem.color + 100)));
			board.movingCount++;
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (color < 100)
					{
						board.cells[row, col].bottomElement = null;
						UnityEngine.Object.Destroy(elem.gameObject);
						GetIceCollectNum(board);
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessGrass(Board board, Cell cell)
		{
			if (!cell.HaveGrass())
			{
				cell.isHavaGrass = true;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessGrass, new ProcessGrass(board, cell.row, cell.col)));
				UpdateTargetState(board, 600);
			}
		}

		public static void ProcessShell(Board board, Element elem)
		{
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessShell, new ProcessShell(elem, position)));
			board.movingCount++;
			float time = 0f;
			elem.removed = true;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (board.cells[row, col].element == elem)
					{
						board.cells[row, col].element = null;
					}
					UnityEngine.Object.Destroy(elem.gameObject);
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessButterFly(Board board, Element elem)
		{
			UpdateTargetState(board, elem.color, 0, elem);
			board.cells[elem.row, elem.col].element = null;
		}

		public static void ProcessJewel(Board board, Cell cell)
		{
			Element element = cell.element;
			if (cell.isTail)
			{
				board.JewelNum--;
				AudioManager.Instance.PlayAudioEffect("collect_ring");
				UpdateTargetState(board, element.color, 0, element);
				board.cells[element.row, element.col].element = null;
				if (GetHaveElementAndCellTool.CheckCellInCrrentArea(board, element))
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(4u, board);
				}
			}
		}

		public static void ProcessCat(Board board, Cell cell)
		{
			Element elem = cell.element;
			int row = elem.row;
			int col = elem.col;
			UpdateTargetState(board, 24, 0, elem);
			float time = 0f;
			elem.removed = true;
			board.movingCount++;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (board.cells[row, col].element == elem)
					{
						board.cells[row, col].element = null;
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessFish(Board board, Cell cell)
		{
			Element element = cell.element;
			Vector3 position = element.transform.position;
			int color = element.color;
			int row = element.row;
			int col = element.col;
			element.FindFishCount = Mathf.Clamp(element.FindFishCount + 3, 0, 7);
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessFish, new ProcessShell(element, position)));
		}

		public static void ProcessRound(Board board, Cell cell, BombInfo bombinfo)
		{
			List<Cell> list = new List<Cell>();
			int row = cell.row;
			int col = cell.col;
			Cell cellUpByNoDir = GetHaveElementAndCellTool.GetCellUpByNoDir(board, row, col);
			if (cellUpByNoDir != null)
			{
				list.Add(cellUpByNoDir);
			}
			Cell cellDownByNoDir = GetHaveElementAndCellTool.GetCellDownByNoDir(board, row, col);
			if (cellDownByNoDir != null)
			{
				list.Add(cellDownByNoDir);
			}
			Cell cellLeftByNoDir = GetHaveElementAndCellTool.GetCellLeftByNoDir(board, row, col);
			if (cellLeftByNoDir != null)
			{
				list.Add(cellLeftByNoDir);
			}
			Cell cellRightByNoDir = GetHaveElementAndCellTool.GetCellRightByNoDir(board, row, col);
			if (cellRightByNoDir != null)
			{
				list.Add(cellRightByNoDir);
			}
			List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
			foreach (Cell item in list)
			{
				if (bombinfo != null)
				{
					bool flag = false;
					flag = ((bombinfo.type == ElementType.HorizontalBomb) ? ((item.row == bombinfo.bombColOrRow) ? true : false) : ((bombinfo.type != ElementType.VerticalBomb) ? (((float)item.row >= bombinfo.posRightDown.x && (float)item.row <= bombinfo.posLeftUP.x && (float)item.col >= bombinfo.posLeftUP.y && (float)item.col <= bombinfo.posRightDown.y) ? true : false) : ((item.col == bombinfo.bombColOrRow) ? true : false)));
					if (!flag && (item.HaveButton() || item.HaveBox() || item.HaveWhitCloud() || item.HaveBlackCloud() || item.HaveHoney() || item.HaveShell() || item.HaveTreasure() || item.isFish()))
					{
						list2.Add(new ElementRemoveInfo(item, false, true, false, 0.1f));
					}
				}
				else if (item.HaveButton() || item.HaveBox() || item.HaveWhitCloud() || item.HaveBlackCloud() || item.HaveHoney() || item.HaveShell() || item.HaveTreasure() || item.isFish())
				{
					list2.Add(new ElementRemoveInfo(item, false, true, false, 0.1f));
				}
			}
			if (list2.Count > 0)
			{
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
			}
		}

		public static void ProcessBox(Board board, Element elem)
		{
			if (!(elem != null) || elem.color < 31 || elem.color > 35)
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 30 : (elem.color - 1));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessBox, new ProcessBox(elem, position, (ElementType)(elem.color + 1))));
			board.movingCount++;
			float time = 0f;
			Element elem2 = elem;
			elem2.removed = true;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (elem2 != null)
					{
						if (color < 31 && board.cells[row, col].element != null)
						{
							if (elem2 == board.cells[row, col].element)
							{
								elem2.moving = false;
								board.cells[row, col].element = null;
								UnityEngine.Object.Destroy(elem2.gameObject);
							}
							else
							{
								elem2.moving = false;
								UnityEngine.Object.Destroy(elem2.gameObject);
							}
						}
						else
						{
							elem2.removed = false;
						}
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessButton(Board board, Element elem)
		{
			if (!(elem != null) || elem.color < 71 || elem.color > 74)
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 70 : (elem.color - 1));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			if (color < 71)
			{
				foreach (Cell item in GetHaveElementAndCellTool.GetCellAround4(board, cell.row, cell.col))
				{
					if (item.HaveButton() && !item.element.isReadyToRemove)
					{
						item.DestroyBlock = true;
						item.element.isReadyToRemove = true;
						List<ElementRemoveInfo> list = new List<ElementRemoveInfo>
						{
							new ElementRemoveInfo(item)
						};
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0.05f, board)));
					}
				}
				if (elem.LeftXian != null && elem.LeftXian.activeSelf)
				{
					elem.LeftXian.SetActive(false);
				}
				if (elem.RightXian != null && elem.RightXian.activeSelf)
				{
					elem.RightXian.SetActive(false);
				}
				if (elem.UpXian != null && elem.UpXian.activeSelf)
				{
					elem.UpXian.SetActive(false);
				}
				if (elem.DownXian != null && elem.DownXian.activeSelf)
				{
					elem.DownXian.SetActive(false);
				}
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessButton, new ProcessBox(elem, position, (ElementType)(elem.color + 1))));
			board.movingCount++;
			float time = 0f;
			Element elem2 = elem;
			elem2.removed = true;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (elem2 != null)
					{
						Cell cell2 = board.cells[row, col];
						if (color < 71 && cell2.element != null)
						{
							if (elem2 == cell2.element)
							{
								elem2.moving = false;
								cell2.element = null;
								UnityEngine.Object.Destroy(elem2.gameObject);
							}
							else
							{
								elem2.moving = false;
								UnityEngine.Object.Destroy(elem2.gameObject);
							}
						}
						else
						{
							elem2.removed = false;
						}
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static List<Cell> GetSameTypeCellNeighbor(Board board, Cell cell)
		{
			List<Cell> list = new List<Cell>();
			List<Cell> cellAround = GetHaveElementAndCellTool.GetCellAround4(board, cell.row, cell.col);
			bool flag = true;
			foreach (Cell item in cellAround)
			{
				if (item.HaveButton() && !item.element.removed && !item.element.isReadyToRemove)
				{
					flag = false;
					item.element.isReadyToRemove = true;
					item.DestroyBlock = true;
					list.Add(item);
				}
			}
			if (flag)
			{
				return list;
			}
			List<Cell> list2 = new List<Cell>();
			foreach (Cell item2 in list)
			{
				list2.AddRange(GetSameTypeCellNeighbor(board, item2));
			}
			list2 = DelReapet(list2);
			list.AddRange(list2);
			return list;
		}

		public static void ProcessTreasure(Board board, Element elem)
		{
			if (!(elem != null) || elem.removed || elem.color < 51 || elem.color > 52)
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 50 : (elem.color - 1));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			if (color < 51 && !isCollectFinish(51))
			{
				UpdateTargetState(board, 51, 0, elem);
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessTreasure, new ProcessTreasure(elem, position, (ElementType)(elem.color + 1))));
			board.movingCount++;
			float time = 0f;
			Element elem2 = elem;
			elem2.removed = true;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					Cell cell2 = board.cells[row, col];
					if (color < 51 && cell2.element != null)
					{
						if (elem2 == cell2.element)
						{
							elem2.moving = false;
							cell2.element = null;
							UnityEngine.Object.Destroy(elem2.gameObject);
						}
						else
						{
							elem2.moving = false;
							UnityEngine.Object.Destroy(elem2.gameObject);
						}
					}
					else
					{
						elem2.removed = false;
						DropAndMoveTool.CheckDrop(board, cell2);
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessNullTreasure(Board board, Element elem)
		{
			if (!(elem != null) || elem.removed || elem.color < 61 || elem.color > 62)
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 60 : (elem.color - 1));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessNullTreasure, new ProcessTreasure(elem, position, (ElementType)(elem.color + 1))));
			board.movingCount++;
			float time = 0f;
			Element elem2 = elem;
			elem2.removed = true;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					Cell cell2 = board.cells[row, col];
					if (color < 61 && cell2.element != null)
					{
						if (elem2 == cell2.element)
						{
							elem2.moving = false;
							cell2.element = null;
							UnityEngine.Object.Destroy(elem2.gameObject);
						}
						else
						{
							elem2.moving = false;
							UnityEngine.Object.Destroy(elem2.gameObject);
						}
					}
					else
					{
						elem2.removed = false;
						DropAndMoveTool.CheckDrop(board, cell2);
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
		}

		public static void ProcessWhiteCloud(Board board, Element elem)
		{
			Vector3 position = elem.transform.position;
			int row = elem.row;
			int col = elem.col;
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 40 : (elem.color - 1));
			cell.DestroyBlock = false;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessWhiteCloud, new ProcessWhiteCloud(elem, position)));
			UpdateTargetState(board, 41, 0, elem);
			float time = 0f;
			elem.removed = true;
			board.movingCount++;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (time > 0.4f)
				{
					if (board.cells[row, col].element == elem)
					{
						board.cells[row, col].element = null;
					}
					board.movingCount--;
					return true;
				}
				time += deltaTime;
				return false;
			}));
			GameLogic.Instance.currentWhiteCloudNum--;
			GameLogic.Instance.isDestroyWhiteCloud = true;
		}

		public static void ProcessBlackCloud(Board board, Element elem)
		{
			if (!(elem != null) || elem.color < 42 || elem.color > 43)
			{
				return;
			}
			Cell cell = board.cells[elem.row, elem.col];
			elem.color = (cell.DestroyBlock ? 41 : (elem.color - 1));
			cell.DestroyBlock = false;
			Vector3 position = elem.transform.position;
			int color = elem.color;
			int row = elem.row;
			int col = elem.col;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ProcessBlackCloud, new ProcessBlackCloud(elem, position)));
			if (color < 42)
			{
				UpdateTargetState(board, 42, 0, elem);
				float time = 0f;
				Element elem2 = elem;
				elem2.removed = true;
				board.movingCount++;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
				{
					if (time > 0.4f)
					{
						elem2.moving = false;
						if (board.cells[row, col].element == elem2)
						{
							board.cells[row, col].element = null;
						}
						board.movingCount--;
						return true;
					}
					time += deltaTime;
					return false;
				}));
				GameLogic.Instance.currentBlackCloudNum--;
			}
			GameLogic.Instance.isDestroyBlackCloud = true;
		}

		public static void UpdateTargetState(Board board, int flag, int layer = 0, Element element = null)
		{
			if (flag <= 0)
			{
				return;
			}
			if (flag <= 7 && flag >= 1)
			{
				flag--;
			}
			else if (flag <= 1500000 && flag >= 1000000)
			{
				flag = 7;
			}
			else if (flag == 600)
			{
				flag = 8;
			}
			else if (flag == 23)
			{
				flag = 9;
			}
			else if (flag == 22)
			{
				flag = 10;
			}
			else if (flag == 41 || flag == 42 || flag == 43)
			{
				flag = 11;
			}
			else if (flag == 51 || flag == 52)
			{
				flag = 12;
			}
			else
			{
				if (flag != 24)
				{
					return;
				}
				flag = 13;
			}
			int[] targetListByCollect = GameLogic.Instance.levelData.targetListByCollect;
			GameLogic.Instance.levelData.targetListByCollect[flag]--;
			if (element != null)
			{
				int num = -99;
				for (int i = 1; i < GameLogic.Instance.tarIDArray.Length; i++)
				{
					if (GameLogic.Instance.tarIDArray[i] == flag)
					{
						num = i;
						break;
					}
				}
				if (num != -99)
				{
					Vector3 collectEndPos = GameSceneUIManager.Instance.GetCollectEndPos(num);
					collectEndPos.z = 0f;
					float delay = Vector3.Distance(element.transform.position, collectEndPos) / 7f;
					if (flag >= 0 && flag <= 6)
					{
						RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CollectStandardElement, new CollectStandardElement(board, element, flag + 1, collectEndPos, delay, layer)));
						return;
					}
					if (flag == 12)
					{
						float para = 0.5f;
						RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CollectPearl, new CollectVaseElement(board, element.transform, collectEndPos, delay, para)));
						GameLogic.Instance.isFinish = isFinishCheck(board, flag);
						float time3 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time3 > delay + 0.95f)
							{
								Statements(board, flag);
								return true;
							}
							time3 += duration;
							return false;
						}));
						return;
					}
					if (flag == 9)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.ButterFlyToTarget, new ButterFlyToTargetMessage(element.transform, collectEndPos, delay, board, flag)));
						return;
					}
					float para2 = 0.5f;
					if (element.type == ElementType.Collect_1_2 || element.type == ElementType.Collect_2_1)
					{
						para2 = 0.5f;
					}
					else if (element.type == ElementType.Collect_2_4 || element.type == ElementType.Collect_4_2)
					{
						para2 = 0.25f;
					}
					else if (element.type == ElementType.Collect_3_6 || element.type == ElementType.Collect_6_3)
					{
						para2 = 0.15f;
					}
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CollectVaseElement, new CollectVaseElement(board, element.transform, collectEndPos, delay, para2)));
					GameLogic.Instance.isFinish = isFinishCheck(board, flag);
					float time2 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time2 > delay + 0.95f)
						{
							Statements(board, flag);
							return true;
						}
						time2 += duration;
						return false;
					}));
				}
				else
				{
					DebugUtils.Log(DebugType.Other, "目标并非收集目标");
				}
			}
			else
			{
				GameLogic.Instance.isFinish = isFinishCheck(board, flag);
				Statements(board, flag);
			}
		}

		public static void UpdateSkillPower(Board board, ElementType type, Cell cell)
		{
			if (!cell.element.IsStandard() || GameLogic.Instance.levelData.skillProbabilityList.Length == 0)
			{
				return;
			}
			int num = GameLogic.Instance.levelData.skillProbabilityList[(int)(type - 1)];
			float num2 = 9f;
			float time = Vector3.Distance(cell.transform.position, GameSceneUIManager.Instance.GetSkillCollectEndPos()) / num2;
			if (num <= 0)
			{
				return;
			}
			board.movingCount++;
			float value = 1f / (float)num;
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CollectSkillPower, new CollectSkillPower(cell.transform.position, GameSceneUIManager.Instance.GetSkillCollectEndPos(), time, num2, type, board.container.transform)));
			float currentTime = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (currentTime > time)
				{
					board.movingCount--;
					Singleton<MessageDispatcher>.Instance().SendMessage(15u, value);
					return true;
				}
				currentTime += duration;
				return false;
			}));
		}

		public static bool isCollectFinish(int flag)
		{
			if (flag <= 0)
			{
				return true;
			}
			if (flag <= 7 && flag >= 1)
			{
				flag--;
			}
			else if (flag <= 1500000 && flag >= 1000000)
			{
				flag = 7;
			}
			else
			{
				switch (flag)
				{
				case 600:
					flag = 8;
					break;
				case 23:
					flag = 9;
					break;
				case 22:
					flag = 10;
					break;
				case 41:
				case 42:
				case 43:
					flag = 11;
					break;
				case 51:
				case 52:
					flag = 12;
					break;
				default:
					return true;
				}
			}
			if (GameLogic.Instance.levelData.targetListByCollect[flag] > 0)
			{
				return false;
			}
			return true;
		}

		public static bool isFinishCheck(Board board, int flag)
		{
			bool flag2 = true;
			int[] targetListByCollect = GameLogic.Instance.levelData.targetListByCollect;
			for (int i = 0; i < GameLogic.Instance.levelData.targetListByCollect.Length; i++)
			{
				if (flag2)
				{
					flag2 = ((GameLogic.Instance.levelData.targetListByCollect[i] <= 0) ? true : false);
				}
			}
			return flag2;
		}

		public static int GetCoinNumByBombType(ElementType type)
		{
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			int result = 0;
			switch (type)
			{
			case ElementType.ColorBomb:
				result = gameConfig.colorBombCoin;
				break;
			case ElementType.AreaBomb:
				result = gameConfig.ABombCoin;
				break;
			case ElementType.FlyBomb:
				result = gameConfig.flyBombCoin;
				break;
			case ElementType.HorizontalBomb:
				result = gameConfig.HBombCoin;
				break;
			case ElementType.VerticalBomb:
				result = gameConfig.VBombCoin;
				break;
			}
			return result;
		}

		public static int GetAllCoin(List<Cell> list)
		{
			if (list == null)
			{
				return 0;
			}
			int num = 0;
			foreach (Cell item in list)
			{
				if (item.element != null)
				{
					num += GetCoinNumByBombType(item.element.type);
				}
			}
			return num;
		}

		public static int GetAllCoin(List<ElementType> list)
		{
			if (list == null)
			{
				return 0;
			}
			int num = 0;
			foreach (ElementType item in list)
			{
				num += GetCoinNumByBombType(item);
			}
			return num;
		}

		public static void Statements(Board board, int flag)
		{
			GameLogic.Instance.levelData.targetList[flag]--;
			GameSceneUIManager.Instance.UpdateTargetNum();
			if (GameLogic.Instance.isFinishing || !GameLogic.Instance.isFinish)
			{
				return;
			}
			GameLogic.Instance.isFinishing = true;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			int num = GameLogic.Instance.levelData.move - GameLogic.Instance.TotleMoveCount;
			List<Cell> bombCellList = GetHaveElementAndCellTool.GetBombCellList(board);
			List<ElementType> TypeList = new List<ElementType>();
			for (int i = 0; i < num; i++)
			{
				TypeList.Add((ElementType)UnityEngine.Random.Range(10, 14));
			}
			TypeList = ListRandom(TypeList);
			GameLogic.Instance.SettleAccounts(bombCellList, TypeList);
			float maxTime3 = 0.5f;
			float waitTime = 2f;
			float currentTime3 = 0f;
			bool calTime = false;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime3)
			{
				if (board.movingCount == 0)
				{
					currentTime3 += deltaTime3;
					if (currentTime3 >= maxTime3 && !calTime)
					{
						calTime = true;
						GameLogic.Instance.BombToGold = true;
						DialogManagerTemp.Instance.CloseAllDialogs();
						DialogManagerTemp.Instance.ShowDialog(DialogType.CongratulationDlg);
						currentTime3 = 0f;
					}
					else if (currentTime3 >= waitTime && calTime)
					{
						GameSceneUIManager.Instance.ActiveCoinAndBookGrid();
						float maxTime4 = 0.1f;
						float currentTime4 = 0f;
						GameSceneUIManager.Instance.SkillRoot.SetActive(false);
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime1)
						{
							if (board.movingCount == 0)
							{
								currentTime4 += deltaTime1;
								if (currentTime4 >= maxTime4)
								{
									GameLogic.Instance.BombAutoBomb = true;
									List<ElementRemoveInfo> list = new List<ElementRemoveInfo>();
									List<Cell> bombCellList2 = GetHaveElementAndCellTool.GetBombCellList(board);
									if (bombCellList2.Count > 0)
									{
										foreach (Cell item in bombCellList2)
										{
											if (item.element != null)
											{
												list.Add(new ElementRemoveInfo(item, false, true, false, 0.1f, null, ElementType.None, ElementType.ColorBomb));
											}
											else
											{
												DebugUtils.Log(DebugType.Other, item.row + "   " + item.col + "    处元素为空！");
											}
										}
										Action action = delegate
										{
											float maxTime6 = 1f;
											float currentTime6 = 0f;
											UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
											{
												if (board.movingCount == 0)
												{
													currentTime6 += deltaTime;
													if (currentTime6 >= maxTime6)
													{
														GameLogic.Instance.BombAutoBomb = false;
														List<Cell> normalCellList2 = GetHaveElementAndCellTool.GetNormalCellList(board);
														List<Cell> list3 = new List<Cell>();
														normalCellList2 = ListRandom(normalCellList2);
														int num3 = GameLogic.Instance.levelData.move - GameLogic.Instance.TotleMoveCount;
														if (num3 >= normalCellList2.Count)
														{
															list3 = normalCellList2;
														}
														else
														{
															for (int k = 0; k < num3; k++)
															{
																if (normalCellList2[k].element != null)
																{
																	list3.Add(normalCellList2[k]);
																}
															}
														}
														CreateBombWhenFinish message2 = new CreateBombWhenFinish(list3, TypeList);
														UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.CreateBombWhenFinish, message2));
														return true;
													}
													return false;
												}
												currentTime6 = 0f;
												return false;
											}));
										};
										UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0.07f, board, 0f, action)));
									}
									else if (GameLogic.Instance.levelData.move - GameLogic.Instance.TotleMoveCount > 0)
									{
										float maxTime5 = 0.1f;
										float currentTime5 = 0f;
										UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
										{
											if (board.movingCount == 0)
											{
												currentTime5 += deltaTime;
												if (currentTime5 >= maxTime5)
												{
													GameLogic.Instance.BombAutoBomb = false;
													List<Cell> normalCellList = GetHaveElementAndCellTool.GetNormalCellList(board);
													List<Cell> list2 = new List<Cell>();
													normalCellList = ListRandom(normalCellList);
													int num2 = GameLogic.Instance.levelData.move - GameLogic.Instance.TotleMoveCount;
													if (num2 >= normalCellList.Count)
													{
														list2 = normalCellList;
													}
													else
													{
														for (int j = 0; j < num2; j++)
														{
															if (normalCellList[j].element != null)
															{
																list2.Add(normalCellList[j]);
															}
														}
													}
													CreateBombWhenFinish message = new CreateBombWhenFinish(list2, TypeList);
													UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.CreateBombWhenFinish, message));
													return true;
												}
												return false;
											}
											currentTime5 = 0f;
											return false;
										}));
									}
									else
									{
										GameSceneUIManager.Instance.isCanSkipFinish = false;
										GameEndUIDeal(board, delegate
										{
											GameLogic.Instance.BombToGold = false;
											GameLogic.Instance.BombAutoBomb = false;
											Singleton<MessageDispatcher>.Instance().SendMessage(10u, null);
										}, 0f);
									}
									return true;
								}
								return false;
							}
							currentTime4 = 0f;
							return false;
						}));
						return true;
					}
					return false;
				}
				currentTime3 = 0f;
				currentTime3 = 0f;
				return false;
			}));
		}

		public static void GetIceCollectNum(Board board)
		{
			List<Cell> list = new List<Cell>();
			Cell[,] cells = board.cells;
			if (list.Count == 0)
			{
				for (int i = 0; i < board.MaxRow; i++)
				{
					for (int j = 0; j < board.MaxCol; j++)
					{
						if (cells[i, j].HaveVase())
						{
							list.Add(cells[i, j]);
						}
					}
				}
			}
			foreach (Cell item in list)
			{
				int row = item.row;
				int col = item.col;
				if (item.lastElement == null)
				{
					continue;
				}
				if (item.lastElement.color == 1000000)
				{
					if (item.bottomElement == null && cells[row - 1, col].bottomElement == null)
					{
						UpdateTargetState(board, 1000000, 0, item.lastElement);
						item.lastElement = null;
					}
				}
				else if (item.lastElement.color == 1100000)
				{
					if (item.bottomElement == null && cells[row, col + 1].bottomElement == null && cells[row - 1, col].bottomElement == null && cells[row - 2, col].bottomElement == null && cells[row - 3, col].bottomElement == null && cells[row - 1, col + 1].bottomElement == null && cells[row - 2, col + 1].bottomElement == null && cells[row - 3, col + 1].bottomElement == null)
					{
						UpdateTargetState(board, 1100000, 0, item.lastElement);
						item.lastElement = null;
					}
				}
				else if (item.lastElement.color == 1200000)
				{
					if (item.bottomElement == null && cells[row, col + 1].bottomElement == null && cells[row, col + 2].bottomElement == null && cells[row - 1, col].bottomElement == null && cells[row - 1, col + 1].bottomElement == null && cells[row - 1, col + 2].bottomElement == null && cells[row - 2, col].bottomElement == null && cells[row - 2, col + 1].bottomElement == null && cells[row - 2, col + 2].bottomElement == null && cells[row - 3, col].bottomElement == null && cells[row - 3, col + 1].bottomElement == null && cells[row - 3, col + 2].bottomElement == null && cells[row - 4, col].bottomElement == null && cells[row - 4, col + 1].bottomElement == null && cells[row - 4, col + 2].bottomElement == null && cells[row - 5, col].bottomElement == null && cells[row - 5, col + 1].bottomElement == null && cells[row - 5, col + 2].bottomElement == null)
					{
						UpdateTargetState(board, 1200000, 0, item.lastElement);
						item.lastElement = null;
					}
				}
				else if (item.lastElement.color == 1300000)
				{
					if (item.bottomElement == null && cells[row, col + 1].bottomElement == null)
					{
						UpdateTargetState(board, 1300000, 0, item.lastElement);
						item.lastElement = null;
					}
				}
				else if (item.lastElement.color == 1400000)
				{
					if (item.bottomElement == null && cells[row, col + 1].bottomElement == null && cells[row, col + 2].bottomElement == null && cells[row, col + 3].bottomElement == null && cells[row - 1, col].bottomElement == null && cells[row - 1, col + 1].bottomElement == null && cells[row - 1, col + 2].bottomElement == null && cells[row - 1, col + 3].bottomElement == null)
					{
						UpdateTargetState(board, 1400000, 0, item.lastElement);
						item.lastElement = null;
					}
				}
				else if (item.lastElement.color == 1500000 && item.bottomElement == null && cells[row - 1, col].bottomElement == null && cells[row - 2, col].bottomElement == null && cells[row, col + 1].bottomElement == null && cells[row, col + 2].bottomElement == null && cells[row, col + 3].bottomElement == null && cells[row, col + 4].bottomElement == null && cells[row, col + 5].bottomElement == null && cells[row - 1, col + 1].bottomElement == null && cells[row - 2, col + 1].bottomElement == null && cells[row - 1, col + 2].bottomElement == null && cells[row - 2, col + 2].bottomElement == null && cells[row - 1, col + 3].bottomElement == null && cells[row - 2, col + 3].bottomElement == null && cells[row - 1, col + 4].bottomElement == null && cells[row - 2, col + 4].bottomElement == null && cells[row - 1, col + 5].bottomElement == null && cells[row - 2, col + 5].bottomElement == null)
				{
					UpdateTargetState(board, 1500000, 0, item.lastElement);
					item.lastElement = null;
				}
			}
		}

		public static bool CheckAllGrassFinished(Board board)
		{
			BoardData boardDatum = GameLogic.Instance.levelData.boardData[GameLogic.Instance.currMap];
			Cell[,] cells = board.cells;
			for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
			{
				for (int j = board.levelColStart; j <= board.levelColEnd; j++)
				{
					if (!cells[i, j].empty && !cells[i, j].HaveGrass())
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool CheckButterIsNone(Board board)
		{
			Cell[,] cells = board.cells;
			for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
			{
				for (int j = board.levelColStart; j <= board.levelColEnd; j++)
				{
					if (!cells[i, j].empty && cells[i, j].HaveButterfly())
					{
						return false;
					}
				}
			}
			return true;
		}

		public static Vector3 GetPositionByDirection(Board board, int row, int col, Direction dir)
		{
			float num = 0.78f;
			float offsetx = board.offsetx;
			float offsety = board.offsety;
			float x = 0f;
			float y = 0f;
			switch (dir)
			{
			case Direction.Down:
				x = num * (float)col + offsetx;
				y = num * (float)(row + 1) + offsety;
				break;
			case Direction.Up:
				x = num * (float)col + offsetx;
				y = num * (float)(row - 1) + offsety;
				break;
			case Direction.Left:
				x = num * (float)(col + 1) + offsetx;
				y = num * (float)row + offsety;
				break;
			case Direction.Right:
				x = num * (float)(col - 1) + offsetx;
				y = num * (float)row + offsety;
				break;
			}
			return new Vector3(x, y, 0f);
		}

		public static void GameEndUIDeal(Board board, Action action, float maxTime = 1.5f)
		{
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			float currentTime = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				if (board.movingCount == 0)
				{
					currentTime += deltaTime;
					if (currentTime >= maxTime)
					{
						action();
						return true;
					}
					return false;
				}
				currentTime = 0f;
				return false;
			}));
		}

		public static bool ElementInTargetState(int flag)
		{
			if (flag <= 0)
			{
				return false;
			}
			if (flag <= 7 && flag >= 1)
			{
				flag--;
			}
			else if (flag <= 1500000 && flag >= 1000000)
			{
				flag = 7;
			}
			else
			{
				switch (flag)
				{
				case 600:
					flag = 8;
					break;
				case 23:
					flag = 9;
					break;
				case 22:
					flag = 10;
					break;
				case 41:
				case 42:
				case 43:
					flag = 11;
					break;
				case 51:
				case 52:
					flag = 12;
					break;
				default:
					return false;
				}
			}
			long num = GameLogic.Instance.tarIDArray.LongLength;
			for (int i = 1; i <= GameLogic.Instance.tarIDArray.Length - 1; i++)
			{
				if (GameLogic.Instance.tarIDArray[i] == flag)
				{
					return true;
				}
			}
			return false;
		}

		public static List<Cell> GetTreasures(Board board)
		{
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			List<Cell> list = new List<Cell>();
			Cell[,] cells = board.cells;
			for (int i = levelColStart; i <= levelColEnd; i++)
			{
				for (int j = levelRowStart; j <= levelRowEnd; j++)
				{
					if (cells[j, i].HaveTreasure())
					{
						list.Add(cells[j, i]);
					}
				}
			}
			return list;
		}

		public static Element GetWhiteCloudTarget(Board board)
		{
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			List<Cell> list = new List<Cell>();
			Cell[,] cells = board.cells;
			for (int i = levelColStart; i <= levelColEnd; i++)
			{
				for (int j = levelRowStart; j <= levelRowEnd; j++)
				{
					if (cells[j, i].HaveWhitCloud())
					{
						list.Add(cells[j, i]);
					}
				}
			}
			List<Element> list2 = new List<Element>();
			List<Element> list3 = new List<Element>();
			foreach (Cell item in list)
			{
				foreach (Cell item2 in GetHaveElementAndCellTool.GetCellAround4(board, item.row, item.col))
				{
					if (item2.isTopElementClear() && item2.element != null)
					{
						if (item2.element.IsStandard())
						{
							list3.Add(item2.element);
						}
						else if (item2.element.IsBomb())
						{
							list2.Add(item2.element);
						}
					}
				}
			}
			if (list3.Count >= 1)
			{
				int index = UnityEngine.Random.Range(0, list3.Count);
				return list3[index];
			}
			if (list2.Count >= 1)
			{
				int index2 = UnityEngine.Random.Range(0, list2.Count);
				return list2[index2];
			}
			return null;
		}

		public static List<Element> GetBlackCloudTarget(Board board)
		{
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			List<Cell> list = new List<Cell>();
			List<Cell> list2 = new List<Cell>();
			Cell[,] cells = board.cells;
			int num = 0;
			int num2 = 0;
			for (int i = levelColStart; i <= levelColEnd; i++)
			{
				for (int j = levelRowStart; j <= levelRowEnd; j++)
				{
					if (cells[j, i].HaveBlackCloud())
					{
						if (cells[j, i].element.color == 42)
						{
							num++;
							list.Add(cells[j, i]);
						}
						else if (cells[j, i].element.color == 43)
						{
							num2++;
							list2.Add(cells[j, i]);
						}
					}
				}
			}
			list = ListRandom(list);
			list2 = ListRandom(list2);
			if (num == 0 && num2 == 0)
			{
				DebugUtils.LogError(DebugType.Other, "场上没有乌云！");
			}
			List<Element> list3 = new List<Element>();
			new List<Element>();
			foreach (Cell item in list)
			{
				list3.Add(item.element);
			}
			if (num >= 2)
			{
				return list3;
			}
			List<Element> list4 = new List<Element>();
			foreach (Cell item2 in list2)
			{
				foreach (Cell item3 in GetHaveElementAndCellTool.GetCellAround4(board, item2.row, item2.col))
				{
					if (item3.isTopElementClear() && item3.element != null && item3.element.IsStandard())
					{
						list4.Add(item3.element);
					}
				}
			}
			list4 = ListRandom(list4);
			list3.AddRange(list4);
			return list3;
		}

		public static List<T> ListRandom<T>(List<T> myList)
		{
			if (myList.Count < 2)
			{
				return myList;
			}
			System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
			new List<T>();
			T val = default(T);
			for (int i = 0; i < myList.Count - 1; i++)
			{
				int num = 0;
				int num2 = 0;
				num2 = random.Next(0, myList.Count);
				do
				{
					num = random.Next(0, myList.Count);
				}
				while (num == num2);
				val = myList[i];
				myList[i] = myList[num];
				myList[num] = val;
			}
			return myList;
		}

		public static List<T> DelReapet<T>(List<T> mtList)
		{
			for (int i = 0; i < mtList.Count; i++)
			{
				for (int num = mtList.Count - 1; num > i; num--)
				{
					if (mtList[i].Equals(mtList[num]))
					{
						mtList.RemoveAt(num);
					}
				}
			}
			return mtList;
		}

		public static List<Vector2> DelReapetVec2(List<Vector2> mtList)
		{
			int num = 0;
			int num2 = mtList.Count - 1;
			while (num < num2)
			{
				Vector2 value = mtList[num];
				mtList[num] = mtList[num2];
				mtList[num2] = value;
				num++;
				num2--;
			}
			for (int i = 0; i < mtList.Count; i++)
			{
				for (int num3 = mtList.Count - 1; num3 > i; num3--)
				{
					if (mtList[i].x.Equals(mtList[num3].x) && mtList[i].y.Equals(mtList[num3].y))
					{
						DebugUtils.Log(DebugType.Other, "存在一个重复元素");
						mtList.RemoveAt(num3);
					}
				}
			}
			num = 0;
			num2 = mtList.Count - 1;
			while (num < num2)
			{
				Vector2 value2 = mtList[num];
				mtList[num] = mtList[num2];
				mtList[num2] = value2;
				num++;
				num2--;
			}
			return mtList;
		}

		public static bool targetIsZero(Board board)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			bool flag4 = true;
			if (ElementInTargetState(600))
			{
				flag = CheckAllGrassFinished(board);
			}
			if (ElementInTargetState(23))
			{
				flag2 = CheckButterIsNone(board);
				if (flag2)
				{
					Cell[,] cells = board.cells;
					List<Cell> list = new List<Cell>();
					for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
					{
						for (int j = board.levelColStart; j <= board.levelColEnd; j++)
						{
							if (!cells[i, j].empty && cells[i, j].Blocked())
							{
								cells[i, j].DestroyBlock = true;
								list.Add(cells[i, j]);
							}
						}
					}
					List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
					foreach (Cell item in list)
					{
						list2.Add(new ElementRemoveInfo(item));
					}
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0f, board)));
				}
			}
			if (ElementInTargetState(1000000))
			{
				Cell[,] cells2 = board.cells;
				new List<Element>();
				for (int k = board.levelRowStart; k <= board.levelRowEnd; k++)
				{
					for (int l = board.levelColStart; l <= board.levelColEnd; l++)
					{
						if (cells2[k, l].isInVase && cells2[k, l].bottomElement != null)
						{
							flag3 = false;
							break;
						}
					}
				}
			}
			if (ElementInTargetState(22))
			{
				flag4 = false;
			}
			return flag && flag2 && flag3 && flag4;
		}

		public static List<ActiveSkill> GetSkillInfo()
		{
			int num = -1;
			List<ActiveSkill> list = new List<ActiveSkill>();
			int[] skillProbabilityList = GameLogic.Instance.levelData.skillProbabilityList;
			for (int i = 0; i < skillProbabilityList.Length; i++)
			{
				if (skillProbabilityList[i] > 0)
				{
					num = i + 1;
					break;
				}
			}
			switch (num)
			{
			case 1:
				list.Add(new ActiveSkill(1, ElementType.DoubleFlyBomb));
				break;
			case 2:
				list.Add(new ActiveSkill(1, ElementType.VerticalBomb));
				list.Add(new ActiveSkill(1, ElementType.HorizontalBomb));
				break;
			case 3:
				list.Add(new ActiveSkill(1, ElementType.AreaBomb));
				break;
			case 4:
				list.Add(new ActiveSkill(1, ElementType.ColorBomb));
				break;
			case 5:
			{
				for (int j = 0; j < 2; j++)
				{
					int boomType = UnityEngine.Random.Range(10, 15);
					list.Add(new ActiveSkill(1, (ElementType)boomType));
				}
				break;
			}
			case 6:
				list.Add(new ActiveSkill(1, ElementType.VH3Bomb));
				break;
			case 7:
				list.Add(new ActiveSkill(1, ElementType.DoubleFlyBomb));
				break;
			}
			return list;
		}
	}
}
