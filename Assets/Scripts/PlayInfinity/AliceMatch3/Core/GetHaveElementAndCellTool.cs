using System.Collections.Generic;
using System.Linq;
using PlayInfinity.AliceMatch3.Editor;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class GetHaveElementAndCellTool
	{
		public static Direction GetDirToElements(Element one, Element two)
		{
			if (one.row == two.row && one.col > two.col)
			{
				return Direction.Left;
			}
			if (one.row == two.row && one.col < two.col)
			{
				return Direction.Right;
			}
			if (one.row > two.row && one.col == two.col)
			{
				return Direction.Down;
			}
			if (one.row < two.row && one.col == two.col)
			{
				return Direction.Up;
			}
			return Direction.Mix;
		}

		public static bool HavePreviousCell(Board board, Cell cell)
		{
			if (cell.isHead || cell.empty)
			{
				return false;
			}
			return true;
		}

		public static Cell GetPreviousCell(Board board, Cell cell)
		{
			return cell.PreCell;
		}

		public static bool HaveNextCell(Board board, Cell cell)
		{
			if (cell.isTail || cell.empty || cell.NextCell == null)
			{
				if (cell.NextCell == null)
				{
					DebugUtils.Log(DebugType.Battle, "Cell row : " + cell.row + "  col : " + cell.col + " 路径没有下一个！");
				}
				return false;
			}
			return true;
		}

		public static Cell GetNextCell(Board board, Cell cell)
		{
			return cell.NextCell;
		}

		public static bool HaveUpLeftCell(Board board, Cell cell)
		{
			switch (cell.GetDirection())
			{
			case Direction.Up:
				if (cell.col + 1 > board.MaxCol - 1 || cell.row - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Down:
				if (cell.col - 1 < 0 || cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row - 1 < 0 || cell.col - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row + 1 > board.MaxRow - 1 || cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetUpLeftCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Up:
				if (cell.col + 1 <= board.MaxCol - 1 && cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col + 1];
				}
				break;
			case Direction.Down:
				if (cell.col - 1 >= 0 && cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col - 1];
				}
				break;
			case Direction.Right:
				if (cell.row - 1 >= 0 && cell.col - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col - 1];
				}
				break;
			case Direction.Left:
				if (cell.row + 1 <= board.MaxRow - 1 && cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row + 1, cell.col + 1];
				}
				break;
			}
			return cell;
		}

		public static bool HaveDownLeftCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cell2 = board.cells;
			switch (direction)
			{
			case Direction.Up:
				if (cell.col + 1 > board.MaxCol - 1 || cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			case Direction.Down:
				if (cell.col - 1 < 0 || cell.row - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row - 1 < 0 || cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row + 1 > board.MaxRow - 1 || cell.col - 1 < 0)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetDownLeftCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Up:
				if (cell.col + 1 <= board.MaxCol - 1 && cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col + 1];
				}
				break;
			case Direction.Down:
				if (cell.col - 1 >= 0 && cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col - 1];
				}
				break;
			case Direction.Right:
				if (cell.row - 1 >= 0 && cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row - 1, cell.col + 1];
				}
				break;
			case Direction.Left:
				if (cell.row + 1 <= board.MaxRow - 1 && cell.col - 1 >= 0)
				{
					return cells[cell.row + 1, cell.col - 1];
				}
				break;
			}
			return cell;
		}

		public static bool HaveLeftCell(Board board, Cell cell)
		{
			switch (cell.GetDirection())
			{
			case Direction.Down:
				if (cell.col - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Up:
				if (cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row - 1 < 0)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetLeftCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Down:
				if (cell.col - 1 >= 0)
				{
					return cells[cell.row, cell.col - 1];
				}
				break;
			case Direction.Up:
				if (cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row, cell.col + 1];
				}
				break;
			case Direction.Left:
				if (cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col];
				}
				break;
			case Direction.Right:
				if (cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col];
				}
				break;
			}
			return cell;
		}

		public static bool HaveUpRightCell(Board board, Cell cell)
		{
			switch (cell.GetDirection())
			{
			case Direction.Up:
				if (cell.col - 1 < 0 || cell.row - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Down:
				if (cell.col + 1 > board.MaxCol - 1 || cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row + 1 > board.MaxRow - 1 || cell.col - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row - 1 < 0 || cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetUpRightCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Up:
				if (cell.col - 1 >= 0 && cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col - 1];
				}
				break;
			case Direction.Down:
				if (cell.col + 1 <= board.MaxCol - 1 && cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col + 1];
				}
				break;
			case Direction.Right:
				if (cell.row + 1 <= board.MaxRow - 1 && cell.col - 1 >= 0)
				{
					return cells[cell.row + 1, cell.col - 1];
				}
				break;
			case Direction.Left:
				if (cell.row - 1 >= 0 && cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row - 1, cell.col + 1];
				}
				break;
			}
			return cell;
		}

		public static bool HaveDownRightCell(Board board, Cell cell)
		{
			switch (cell.GetDirection())
			{
			case Direction.Up:
				if (cell.col - 1 < 0 || cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			case Direction.Down:
				if (cell.col + 1 > board.MaxCol - 1 || cell.row - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row + 1 > board.MaxRow - 1 || cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row - 1 < 0 || cell.col - 1 < 0)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetDownRightCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Up:
				if (cell.row + 1 <= board.MaxRow - 1 && cell.col - 1 >= 0)
				{
					return cells[cell.row + 1, cell.col - 1];
				}
				break;
			case Direction.Down:
				if (cell.row - 1 >= 0 && cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row - 1, cell.col + 1];
				}
				break;
			case Direction.Right:
				if (cell.col + 1 <= board.MaxCol - 1 && cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col + 1];
				}
				break;
			case Direction.Left:
				if (cell.col - 1 >= 0 && cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col - 1];
				}
				break;
			}
			return cell;
		}

		public static bool HaveRightCell(Board board, Cell cell)
		{
			switch (cell.GetDirection())
			{
			case Direction.Down:
				if (cell.col + 1 > board.MaxCol - 1)
				{
					return false;
				}
				break;
			case Direction.Up:
				if (cell.col - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Left:
				if (cell.row - 1 < 0)
				{
					return false;
				}
				break;
			case Direction.Right:
				if (cell.row + 1 > board.MaxRow - 1)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static Cell GetRightCell(Board board, Cell cell)
		{
			Direction direction = cell.GetDirection();
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Down:
				if (cell.col + 1 <= board.MaxCol - 1)
				{
					return cells[cell.row, cell.col + 1];
				}
				break;
			case Direction.Up:
				if (cell.col - 1 >= 0)
				{
					return cells[cell.row, cell.col - 1];
				}
				break;
			case Direction.Left:
				if (cell.row - 1 >= 0)
				{
					return cells[cell.row - 1, cell.col];
				}
				break;
			case Direction.Right:
				if (cell.row + 1 <= board.MaxRow - 1)
				{
					return cells[cell.row + 1, cell.col];
				}
				break;
			}
			return cell;
		}

		public static Creater isCanRandom(Board board, Cell cell)
		{
			if (cell == null || GameLogic.Instance.isFinish)
			{
				return null;
			}
			Creater[] createrList = board.data.createrList;
			foreach (Creater creater in createrList)
			{
				if (creater.p.x == cell.col && creater.p.y == cell.row)
				{
					return creater;
				}
			}
			return null;
		}

		public static int RandomCreateElement(Board board, Cell cell = null)
		{
			int[] probabilityList = GameLogic.Instance.GetProbabilityList();
			Creater creater = isCanRandom(board, cell);
			if (cell != null && creater != null)
			{
				if (GameLogic.Instance.canDropJewel && creater.index == 1 && board.JewelNum == 0 && !ProcessTool.isCollectFinish(22))
				{
					return 22;
				}
				float num = (float)creater.probability / 100f;
				if (Random.Range(0f, 1f) < num)
				{
					switch (creater.index)
					{
					case 2:
						return 10;
					case 3:
						if (Random.Range(0, 2) != 0)
						{
							return 11;
						}
						return 12;
					case 4:
						return 13;
					case 5:
						return 14;
					case 6:
						return 21;
					case 7:
						if (Random.Range(0, 2) != 0)
						{
							return 21;
						}
						return 10;
					case 8:
						if (Random.Range(0, 2) != 0)
						{
							if (Random.Range(0, 2) != 0)
							{
								return 11;
							}
							return 12;
						}
						return 10;
					case 9:
						if (Random.Range(0, 2) != 0)
						{
							return 13;
						}
						return 10;
					case 10:
						if (Random.Range(0, 2) != 0)
						{
							return 21;
						}
						return 14;
					case 11:
					{
						float num3 = Random.Range(0f, 1f);
						if (num3 < 0.33f)
						{
							return 10;
						}
						if (num3 < 0.66f)
						{
							if (Random.Range(0, 2) != 0)
							{
								return 11;
							}
							return 12;
						}
						return 13;
					}
					case 12:
					{
						float num2 = Random.Range(0f, 1f);
						if (num2 < 0.33f)
						{
							return 21;
						}
						if (num2 < 0.66f)
						{
							if (Random.Range(0, 2) != 0)
							{
								return 11;
							}
							return 12;
						}
						return 13;
					}
					case 13:
						return 52;
					}
				}
			}
			int num4 = probabilityList.Sum();
			int num5 = Random.Range(1, num4 + 1);
			int num6 = 0;
			for (int i = 0; i < probabilityList.Length; i++)
			{
				num6 += probabilityList[i];
				if (num6 >= num5)
				{
					return i + 1;
				}
			}
			return 1;
		}

		public static ElementType GetRandomColorInBoard(Board board)
		{
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			Cell[,] cells = board.cells;
			Dictionary<ElementType, int> dictionary = new Dictionary<ElementType, int>();
			for (int i = levelRowStart; i <= levelRowEnd; i++)
			{
				for (int j = levelColStart; j <= levelColEnd; j++)
				{
					Cell cell = cells[i, j];
					Element element = cell.element;
					if (cell.empty || cell.element == null || cell.element.moving || !(element != null))
					{
						continue;
					}
					ElementType type = element.type;
					if (type >= ElementType.Standard_0 && type <= ElementType.Standard_6 && (!cell.Blocked() || (cell.Blocked() && cell.HaveBramble())))
					{
						if (dictionary.ContainsKey(type))
						{
							dictionary[type]++;
						}
						else
						{
							dictionary.Add(type, 1);
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				return dictionary.Aggregate((KeyValuePair<ElementType, int> x, KeyValuePair<ElementType, int> y) => (x.Value <= y.Value) ? y : x).Key;
			}
			return ElementType.Standard_0;
		}

		public static List<int> RandomCreateElementList(Board board, Cell cell = null)
		{
			List<int> list = new List<int>();
			int[] probabilityList = GameLogic.Instance.GetProbabilityList();
			for (int i = 0; i < probabilityList.Length; i++)
			{
				if (probabilityList[i] != 0)
				{
					list.Add(i + 1);
				}
			}
			return list;
		}

		public static bool HaveSpaceToMove(Board board, Cell cell)
		{
			if (cell.isPortalEnter)
			{
				return true;
			}
			if (cell.pathDirection == Direction.Up)
			{
				return cell.row + 1 <= board.MaxRow;
			}
			if (cell.pathDirection == Direction.Left)
			{
				return cell.col - 1 >= 0;
			}
			if (cell.pathDirection == Direction.Right)
			{
				return cell.col + 1 <= board.MaxCol;
			}
			return cell.row - 1 >= 0;
		}

		public static bool CheckCellInCrrentArea(Board board, Element element)
		{
			bool num = element.row >= board.levelRowStart && element.row <= board.levelRowEnd;
			bool flag = element.col >= board.levelColStart && element.col <= board.levelColEnd;
			return num && flag;
		}

		public static bool CheckCellInNextArea(Board board, Element element)
		{
			if (board.currArea == board.data.areaList.Length - 1)
			{
				return false;
			}
			Area obj = board.data.areaList[board.currArea + 1];
			int x = obj.start.x;
			int x2 = obj.end.x;
			int y = obj.end.y;
			int y2 = obj.start.y;
			bool flag = element.row >= y && element.row <= y2;
			bool flag2 = element.col >= x && element.col <= x2;
			if (flag && flag2)
			{
				DebugUtils.Log(DebugType.Other, element.row + "      " + element.col + "      is next");
			}
			return flag && flag2;
		}

		public static bool CheckCellInCustomArea(Board board, Element element, int areaNum)
		{
			if (areaNum > board.data.areaList.Length - 1)
			{
				return false;
			}
			Area obj = board.data.areaList[areaNum];
			int x = obj.start.x;
			int x2 = obj.end.x;
			int y = obj.end.y;
			int y2 = obj.start.y;
			bool num = element.row >= y && element.row <= y2;
			bool flag = element.col >= x && element.col <= x2;
			return num && flag;
		}

		public static bool getHaveObjInThisCol(Board board, Cell cell)
		{
			Cell cell2 = cell;
			while (HavePreviousCell(board, cell2))
			{
				cell2 = GetPreviousCell(board, cell2);
				if (!(cell2 != null))
				{
					continue;
				}
				if (cell2.isTopElementClear() && cell2.element != null && !cell2.Blocked())
				{
					if (!cell2.element.removed)
					{
						return false;
					}
				}
				else if (cell2.Blocked())
				{
					return true;
				}
			}
			return false;
		}

		public static Element GetElement(Board board, Direction direction, int row, int col)
		{
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Left:
				if (col > board.levelColStart)
				{
					return cells[row, col - 1].element;
				}
				break;
			case Direction.Right:
				if (col < board.levelColEnd)
				{
					return cells[row, col + 1].element;
				}
				break;
			case Direction.Up:
				if (row < board.levelRowEnd)
				{
					return cells[row + 1, col].element;
				}
				break;
			case Direction.Down:
				if (row > board.levelRowStart)
				{
					return cells[row - 1, col].element;
				}
				break;
			}
			return null;
		}

		public static Cell GetCell(Board board, Direction direction, int row, int col)
		{
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Left:
				if (col > board.levelColStart)
				{
					return cells[row, col - 1];
				}
				break;
			case Direction.Right:
				if (col < board.levelColEnd)
				{
					return cells[row, col + 1];
				}
				break;
			case Direction.Up:
				if (row < board.levelRowEnd)
				{
					return cells[row + 1, col];
				}
				break;
			case Direction.Down:
				if (row > board.levelRowStart)
				{
					return cells[row - 1, col];
				}
				break;
			case Direction.LeftDown:
				if (row > board.levelRowStart && col > board.levelColStart)
				{
					return cells[row - 1, col - 1];
				}
				break;
			case Direction.LeftUp:
				if (row < board.levelRowEnd && col > board.levelColStart)
				{
					return cells[row + 1, col - 1];
				}
				break;
			case Direction.RightDown:
				if (row > board.levelRowStart && col < board.levelColEnd)
				{
					return cells[row - 1, col + 1];
				}
				break;
			case Direction.RightUp:
				if (row < board.levelRowEnd && col < board.levelColEnd)
				{
					return cells[row + 1, col + 1];
				}
				break;
			}
			return null;
		}

		public static Cell GetCellInMaxMap(Board board, Direction direction, int row, int col)
		{
			Cell[,] cells = board.cells;
			switch (direction)
			{
			case Direction.Left:
				if (col > 0)
				{
					return cells[row, col - 1];
				}
				break;
			case Direction.Right:
				if (col < board.MaxCol - 1)
				{
					return cells[row, col + 1];
				}
				break;
			case Direction.Up:
				if (row < board.MaxRow - 1)
				{
					return cells[row + 1, col];
				}
				break;
			case Direction.Down:
				if (row > 0)
				{
					return cells[row - 1, col];
				}
				break;
			case Direction.LeftDown:
				if (row > 0 && col > 0)
				{
					return cells[row - 1, col - 1];
				}
				break;
			case Direction.LeftUp:
				if (row < board.MaxRow - 1 && col > 0)
				{
					return cells[row + 1, col - 1];
				}
				break;
			case Direction.RightDown:
				if (row > 0 && col < board.MaxCol - 1)
				{
					return cells[row - 1, col + 1];
				}
				break;
			case Direction.RightUp:
				if (row < board.MaxRow - 1 && col < board.MaxCol - 1)
				{
					return cells[row + 1, col + 1];
				}
				break;
			}
			return null;
		}

		public static Cell GetCellLeftByNoDir(Board board, int row, int col)
		{
			Cell[,] cells = board.cells;
			if (col > 0)
			{
				return cells[row, col - 1];
			}
			return null;
		}

		public static Cell GetCellRightByNoDir(Board board, int row, int col)
		{
			Cell[,] cells = board.cells;
			if (col < board.MaxCol - 1)
			{
				return cells[row, col + 1];
			}
			return null;
		}

		public static Cell GetCellUpByNoDir(Board board, int row, int col)
		{
			Cell[,] cells = board.cells;
			if (row < board.MaxRow - 1)
			{
				return cells[row + 1, col];
			}
			return null;
		}

		public static Cell GetCellDownByNoDir(Board board, int row, int col)
		{
			Cell[,] cells = board.cells;
			if (row > 0)
			{
				return cells[row - 1, col];
			}
			return null;
		}

		public static List<Cell> GetCellAround4(Board board, int row, int col)
		{
			List<Cell> list = new List<Cell>();
			Cell[,] cells = board.cells;
			if (col > board.levelColStart)
			{
				list.Add(cells[row, col - 1]);
			}
			if (col < board.levelColEnd)
			{
				list.Add(cells[row, col + 1]);
			}
			if (row < board.levelRowEnd)
			{
				list.Add(cells[row + 1, col]);
			}
			if (row > board.levelRowStart)
			{
				list.Add(cells[row - 1, col]);
			}
			return ProcessTool.ListRandom(list);
		}

		public static bool CellsIsNeighbor(Cell one, Cell two)
		{
			if (one.row != two.row || Mathf.Abs(one.col - two.col) != 1)
			{
				if (one.col == two.col)
				{
					return Mathf.Abs(one.row - two.row) == 1;
				}
				return false;
			}
			return true;
		}

		public static bool ElementsIsNeighbor(Element one, Element two)
		{
			if (one.row != two.row || Mathf.Abs(one.col - two.col) != 1)
			{
				if (one.col == two.col)
				{
					return Mathf.Abs(one.row - two.row) == 1;
				}
				return false;
			}
			return true;
		}

		public static List<Cell> GetNormalCellList(Board board)
		{
			Cell[,] cells = board.cells;
			List<Cell> list = new List<Cell>();
			for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
			{
				for (int j = board.levelColStart; j <= board.levelColEnd; j++)
				{
					if (cells[i, j] != null && cells[i, j].element != null)
					{
						Element element = cells[i, j].element;
						if (!element.moving && !element.removed && !cells[i, j].Blocked() && element.type >= ElementType.Standard_0 && element.type <= ElementType.Standard_6)
						{
							list.Add(cells[i, j]);
						}
					}
				}
			}
			return list;
		}

		public static List<TwoCellNear> GetTwoNormalCellNear(Board board, out List<Cell> clist)
		{
			List<TwoCellNear> list = new List<TwoCellNear>();
			clist = GetNormalCellList(board);
			if (clist.Count >= 2)
			{
				for (int i = 0; i < clist.Count - 1; i++)
				{
					for (int j = i + 1; j < clist.Count; j++)
					{
						if (CellsIsNeighbor(clist[i], clist[j]))
						{
							list.Add(new TwoCellNear(clist[i], clist[j]));
						}
					}
				}
			}
			return list;
		}

		public static List<Cell> GetBombCellList(Board board)
		{
			Cell[,] cells = board.cells;
			List<Cell> list = new List<Cell>();
			for (int i = 0; i <= board.levelRowEnd; i++)
			{
				for (int j = 0; j <= board.levelColEnd; j++)
				{
					if (cells[i, j].element != null && !cells[i, j].Blocked() && cells[i, j].element.type >= ElementType.FlyBomb && cells[i, j].element.type <= ElementType.ColorBomb)
					{
						list.Add(cells[i, j]);
					}
				}
			}
			return list;
		}

		public static Dictionary<int, List<Cell>> GetBombRemoveList(Board board, int row, int col, int circleNum)
		{
			Dictionary<int, List<Cell>> dictionary = new Dictionary<int, List<Cell>>();
			int levelRowStart = board.levelRowStart;
			int levelRowEnd = board.levelRowEnd;
			int levelColStart = board.levelColStart;
			int levelColEnd = board.levelColEnd;
			for (int i = 1; i <= circleNum; i++)
			{
				List<Cell> list = new List<Cell>();
				Vector2Int vector2Int = new Vector2Int(row + i, col - i);
				Vector2Int vector2Int2 = new Vector2Int(row - i, col + i);
				Vector2Int vector2Int3 = new Vector2Int(row + i, col + i);
				Vector2Int vector2Int4 = new Vector2Int(row - i, col - i);
				int num = i + i + 1;
				if (vector2Int.x <= levelRowEnd)
				{
					for (int j = 0; j < num; j++)
					{
						int num2 = vector2Int.y + j;
						if (num2 <= levelColEnd && num2 >= levelColStart)
						{
							list.Add(board.cells[vector2Int.x, num2]);
						}
					}
				}
				if (vector2Int3.y <= levelColEnd)
				{
					for (int k = 1; k < num; k++)
					{
						int num3 = vector2Int3.x - k;
						if (num3 >= levelRowStart && num3 <= levelRowEnd)
						{
							list.Add(board.cells[num3, vector2Int3.y]);
						}
					}
				}
				if (vector2Int2.x >= levelRowStart)
				{
					for (int l = 1; l < num; l++)
					{
						int num4 = vector2Int2.y - l;
						if (num4 >= levelColStart && num4 <= levelColEnd)
						{
							list.Add(board.cells[vector2Int2.x, num4]);
						}
					}
				}
				if (vector2Int4.y >= levelColStart)
				{
					for (int m = 1; m < num; m++)
					{
						int num5 = vector2Int4.x + m;
						if (num5 <= levelRowEnd && num5 >= levelRowStart)
						{
							list.Add(board.cells[num5, vector2Int4.y]);
						}
					}
				}
				dictionary.Add(i, list);
			}
			return dictionary;
		}

		public static Sprite GetPicture(int tarID)
		{
			Sprite result = null;
			if (tarID >= 1 && tarID <= 7)
			{
				result = GeneralConfig.ElementPictures[tarID];
			}
			else
			{
				switch (tarID)
				{
				case 8:
					result = GeneralConfig.UIPictures[UIType.Vase];
					break;
				case 9:
					result = GeneralConfig.ElementPictures[600];
					break;
				case 10:
					result = GeneralConfig.ElementPictures[23];
					break;
				case 11:
					result = GeneralConfig.ElementPictures[22];
					break;
				case 12:
					result = GeneralConfig.ElementPictures[41];
					break;
				case 13:
					result = GeneralConfig.ElementPictures[81];
					break;
				case 14:
					result = GeneralConfig.ElementPictures[24];
					break;
				}
			}
			return result;
		}

		public static Sprite GetComboPicture(int ComboLevel)
		{
			Sprite result = null;
			switch (ComboLevel)
			{
			case 1:
				result = GeneralConfig.UIPictures[UIType.ComboLevel1];
				break;
			case 2:
				result = GeneralConfig.UIPictures[UIType.ComboLevel2];
				break;
			case 3:
				result = GeneralConfig.UIPictures[UIType.ComboLevel3];
				break;
			case 4:
				result = GeneralConfig.UIPictures[UIType.ComboLevel4];
				break;
			}
			return result;
		}
	}
}
