using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Editor;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public class Board : MonoBehaviour
	{
		public GameObject container;

		public GameObject CellPrefab;

		public Sprite bg1;

		public Sprite bg2;

		public Cell[,] cells;

		public int levelRowStart;

		public int levelRowEnd = 8;

		public int levelColStart;

		public int levelColEnd = 10;

		public int MaxRow = 9;

		public int MaxCol = 11;

		public int BoardIndex;

		public const float BoardWidth = 858f;

		public const float BoardHeight = 702f;

		public const float CellSize = 0.78f;

		public float offsetx;

		public float offsety;

		public bool switching;

		public int moveCount;

		public int currArea;

		public int moveStep;

		public DateTime lastMoveTime = DateTime.MaxValue;

		public Element currentDragElem;

		public Element currentTouchElem;

		public bool canCheckAreaChange;

		public bool canCheckMapChange;

		public bool MoveToJewel;

		public int JewelNum;

		public BoardData data;

		public Dictionary<int, Element> FishCellDic = new Dictionary<int, Element>();

		public Dictionary<int, List<Cell>> transporterCellDic = new Dictionary<int, List<Cell>>();

		public Dictionary<int, List<Cell>> catCellDic = new Dictionary<int, List<Cell>>();

		public List<Material> transMatList = new List<Material>();

		public int lastMoveCount;

		private int _MovingCount;

		public GameObject TapEffect;

		public List<Cell> HeadCellCollect = new List<Cell>();

		private List<int> colorList = new List<int>();

		public Dictionary<int, Vector2> AreaPosDic = new Dictionary<int, Vector2>();

		public int movingCount
		{
			get
			{
				return _MovingCount;
			}
			set
			{
				_MovingCount = value;
				Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
				GameLogic.Instance.currentCD = 0f;
				GameLogic.Instance.currentTipWaitCD = 0f;
				GameLogic.Instance.currentClickCD = 0f;
				GameLogic.Instance.currentCheckDieCD = 0f;
				GameLogic.Instance.StepFinish = 0f;
				if (value <= 0)
				{
					_MovingCount = 0;
					GameLogic.Instance.isTiping = false;
				}
			}
		}

		public void Create(int currMap)
		{
			TapEffect = PoolManager.Ins.SpawnEffect(50000000, container.transform);
			TapEffect.transform.SetParent(container.transform);
			TapEffect.SetActive(false);
			data = GameLogic.Instance.levelData.boardData[currMap];
			if (data.areaList.Length == 0)
			{
				DebugUtils.LogError(DebugType.Other, "没有区域信息！");
			}
			AreaPosDic = AreaMoveTool.InitAreaPos(this);
			Area area = data.areaList[currArea];
			MaxCol = data.width;
			MaxRow = data.height;
			colorList = GetHaveElementAndCellTool.RandomCreateElementList(this);
			GameSceneUIManager.Instance.SetLevelText(UserDataManager.Instance.GetProgress());
			GameSceneUIManager.Instance.SetMoveText(GameLogic.Instance.levelData.move);
			GameSceneUIManager.Instance.ShowItem();
			AreaMoveTool.SetAnchorPointByInit(this, data.areaList.Length - 1);
			levelColStart = area.start.x;
			levelColEnd = area.end.x;
			levelRowStart = area.end.y;
			levelRowEnd = area.start.y;
			cells = new Cell[MaxRow, MaxCol];
			offsetx = -0.78f * (float)MaxCol / 2f + 0.39f;
			offsety = -0.78f * (float)MaxRow / 2f + 0.39f;
			for (int i = 0; i < MaxRow; i++)
			{
				for (int j = 0; j < MaxCol; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(CellPrefab, container.transform);
					float x = 0.78f * (float)j + offsetx;
					float y = 0.78f * (float)i + offsety;
					gameObject.transform.localPosition = new Vector2(x, y);
					gameObject.name = "bg_" + i + "_" + j;
					cells[i, j] = gameObject.GetComponent<Cell>();
				}
			}
			Path[] pathList = data.pathList;
			foreach (Path path in pathList)
			{
				for (int l = 0; l < path.v.Length; l++)
				{
					Pos pos = path.v[l];
					if (l == 0)
					{
						cells[pos.y, pos.x].isHead = true;
						HeadCellCollect.Add(cells[pos.y, pos.x]);
					}
					else if (l == path.v.Length - 1)
					{
						cells[pos.y, pos.x].isTail = true;
					}
					if (l + 1 < path.v.Length)
					{
						Pos pos2 = path.v[l + 1];
						cells[pos.y, pos.x].NextCell = cells[pos2.y, pos2.x];
						if (pos2.y > pos.y)
						{
							cells[pos.y, pos.x].pathDirection = Direction.Up;
						}
						else if (pos2.x > pos.x)
						{
							cells[pos.y, pos.x].pathDirection = Direction.Right;
						}
						else if (pos2.x < pos.x)
						{
							cells[pos.y, pos.x].pathDirection = Direction.Left;
						}
						else
						{
							cells[pos.y, pos.x].pathDirection = Direction.Down;
						}
					}
					else if (l - 1 >= 0)
					{
						Pos pos3 = path.v[l - 1];
						cells[pos.y, pos.x].pathDirection = cells[pos3.y, pos3.x].pathDirection;
					}
					else
					{
						cells[pos.y, pos.x].pathDirection = Direction.Down;
					}
				}
				for (int num = path.v.Length - 1; num >= 0; num--)
				{
					Pos pos4 = path.v[num];
					if (num == 0)
					{
						cells[pos4.y, pos4.x].isHead = true;
					}
					else if (num == path.v.Length - 1)
					{
						cells[pos4.y, pos4.x].isTail = true;
					}
					if (num - 1 >= 0)
					{
						Pos pos5 = path.v[num - 1];
						cells[pos4.y, pos4.x].PreCell = cells[pos5.y, pos5.x];
						if (pos5.y > pos4.y)
						{
							cells[pos4.y, pos4.x].previousDirection = Direction.Up;
						}
						else if (pos5.x > pos4.x)
						{
							cells[pos4.y, pos4.x].previousDirection = Direction.Right;
						}
						else if (pos5.x < pos4.x)
						{
							cells[pos4.y, pos4.x].previousDirection = Direction.Left;
						}
						else
						{
							cells[pos4.y, pos4.x].previousDirection = Direction.Down;
						}
					}
					else if (num + 1 < path.v.Length)
					{
						Pos pos6 = path.v[num + 1];
						cells[pos4.y, pos4.x].previousDirection = cells[pos6.y, pos6.x].previousDirection;
					}
					else
					{
						cells[pos4.y, pos4.x].previousDirection = Direction.Down;
					}
				}
			}
			Creater[] createrList = data.createrList;
			foreach (Creater creater in createrList)
			{
				Cell cell = cells[creater.p.y, creater.p.x];
				GameObject gameObject2 = UnityEngine.Object.Instantiate(GeneralConfig.ItemCollect[5]);
				if (creater.index == 1)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000004];
				}
				else if (creater.index == 2)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000005];
				}
				else if (creater.index == 3)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000008];
				}
				else if (creater.index == 4)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000006];
				}
				else if (creater.index == 5)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000007];
				}
				else if (creater.index == 6)
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[40000009];
				}
				else
				{
					gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = null;
				}
				gameObject2.transform.SetParent(cell.transform, false);
				gameObject2.transform.localPosition = Vector3.zero;
				if (cell.pathDirection == Direction.Up)
				{
					gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
				}
				else if (cell.pathDirection == Direction.Down)
				{
					gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
				else if (cell.pathDirection == Direction.Left)
				{
					gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
				}
				else if (cell.pathDirection == Direction.Right)
				{
					gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
				}
			}
			List<Cell> list = new List<Cell>();
			for (int m = 0; m < MaxRow; m++)
			{
				for (int n = 0; n < MaxCol; n++)
				{
					int num2 = data.map[m * data.width + n];
					if (num2 >= 0)
					{
						GameLogic.Instance.TotalNumBeGrass++;
					}
					if (num2 != 0 && num2 % 100 != 0)
					{
						InitOneElement(m, n, data.map[m * data.width + n]);
						if (cells[m, n].element != null && !cells[m, n].Blocked() && cells[m, n].element.type >= ElementType.Standard_0 && cells[m, n].element.type <= ElementType.Standard_6)
						{
							list.Add(cells[m, n]);
						}
					}
				}
			}
			for (int num3 = 0; num3 < MaxRow; num3++)
			{
				for (int num4 = 0; num4 < MaxCol; num4++)
				{
					int num5 = data.map[num3 * data.width + num4];
					if (num5 == 0 || num5 % 100 == 0)
					{
						InitOneElement(num3, num4, data.map[num3 * data.width + num4]);
						if (!cells[num3, num4].Blocked() && cells[num3, num4].element.type >= ElementType.Standard_0 && cells[num3, num4].element.type <= ElementType.Standard_6)
						{
							list.Add(cells[num3, num4]);
						}
					}
				}
			}
			if (ProcessTool.ElementInTargetState(22))
			{
				for (int num6 = 0; num6 < MaxRow; num6++)
				{
					for (int num7 = 0; num7 < MaxCol; num7++)
					{
						if (cells[num6, num7].isTail)
						{
							GameObject gameObject3 = UnityEngine.Object.Instantiate(GeneralConfig.ItemCollect[6]);
							gameObject3.transform.SetParent(cells[num6, num7].transform, false);
							gameObject3.transform.localPosition = Vector3.zero;
							switch (cells[num6, num7].pathDirection)
							{
							case Direction.Up:
								gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
								break;
							case Direction.Down:
								gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
								break;
							case Direction.Left:
								gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
								break;
							case Direction.Right:
								gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
								break;
							}
						}
					}
				}
			}
			for (int num8 = 0; num8 < MaxRow; num8++)
			{
				for (int num9 = 0; num9 < MaxCol; num9++)
				{
					if (!cells[num8, num9].empty && cells[num8, num9].NextCell != null && !GetHaveElementAndCellTool.CellsIsNeighbor(cells[num8, num9], cells[num8, num9].NextCell))
					{
						cells[num8, num9].isPortalEnter = true;
						DebugUtils.Log(DebugType.Other, "isPortalEnter " + num8 + "," + num9);
					}
					else if (!cells[num8, num9].empty && cells[num8, num9].PreCell != null && !GetHaveElementAndCellTool.CellsIsNeighbor(cells[num8, num9], cells[num8, num9].PreCell))
					{
						cells[num8, num9].isPortalExit = true;
						DebugUtils.Log(DebugType.Other, "isPortalExit " + num8 + "," + num9);
					}
					if (cells[num8, num9].isPortalEnter)
					{
						if (cells[num8, num9].PreCell != null)
						{
							cells[num8, num9].pathDirection = cells[num8, num9].PreCell.pathDirection;
							GameObject gameObject4 = PoolManager.Ins.SpawnEffect(50000045, container.transform);
							gameObject4.transform.SetParent(cells[num8, num9].transform, false);
							gameObject4.transform.localPosition = Vector3.zero;
							switch (cells[num8, num9].pathDirection)
							{
							case Direction.Up:
								gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
								break;
							case Direction.Down:
								gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
								break;
							case Direction.Left:
								gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
								break;
							case Direction.Right:
								gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
								break;
							}
						}
						else
						{
							DebugUtils.Log(DebugType.Other, num8 + "    " + num9);
						}
					}
					else if (cells[num8, num9].isPortalExit)
					{
						GameObject gameObject5 = PoolManager.Ins.SpawnEffect(50000045, container.transform);
						gameObject5.transform.SetParent(cells[num8, num9].transform, false);
						gameObject5.transform.localPosition = Vector3.zero;
						switch (cells[num8, num9].pathDirection)
						{
						case Direction.Up:
							gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
							break;
						case Direction.Down:
							gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
							break;
						case Direction.Left:
							gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
							break;
						case Direction.Right:
							gameObject5.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
							break;
						}
					}
					GameLogic.Instance.IsRefeshElementByInit(this);
					CreateMapBoard(num8, num9);
					if (!cells[num8, num9].Blocked() && cells[num8, num9].element == null)
					{
						cells[num8, num9].element = null;
					}
				}
			}
			if (data.transporterPathList != null)
			{
				int num10 = 0;
				for (int num11 = 0; num11 < data.transporterPathList.Length; num11++)
				{
					transporterCellDic.Add(num11, new List<Cell>());
				}
				pathList = data.transporterPathList;
				foreach (Path path2 in pathList)
				{
					bool isAnim = true;
					for (int num12 = 0; num12 < path2.v.Length; num12++)
					{
						int num13 = path2.v.Length;
						Pos pos7 = path2.v[num12];
						Cell cell2 = cells[pos7.y, pos7.x];
						transporterCellDic[num10].Add(cell2);
						if (num12 == 0)
						{
							cell2.isTransHead = true;
						}
						else if (num12 == path2.v.Length - 1)
						{
							cell2.isTransTail = true;
						}
						Pos pos8 = path2.v[(num12 + 1) % num13];
						cell2.TransNextCell = cells[pos8.y, pos8.x];
						Pos pos9 = path2.v[(num12 - 1 < 0) ? (num13 - 1) : (num12 - 1)];
						cell2.TransPreCell = cells[pos9.y, pos9.x];
						if (pos8.y > pos7.y)
						{
							cells[pos7.y, pos7.x].transPathDirection = Direction.Up;
						}
						else if (pos8.x > pos7.x)
						{
							cells[pos7.y, pos7.x].transPathDirection = Direction.Right;
						}
						else if (pos8.x < pos7.x)
						{
							cells[pos7.y, pos7.x].transPathDirection = Direction.Left;
						}
						else
						{
							cells[pos7.y, pos7.x].transPathDirection = Direction.Down;
						}
						if (!cell2.isTransHead && !cell2.isTransTail && cell2.transPathDirection != cell2.TransPreCell.transPathDirection)
						{
							Debug.Log(string.Concat(cell2.transPathDirection, "   ", cell2.TransPreCell.transPathDirection, " ", num12));
							isAnim = false;
						}
					}
					for (int num14 = 0; num14 < path2.v.Length; num14++)
					{
						Pos pos10 = path2.v[num14];
						Cell cell3 = cells[pos10.y, pos10.x];
						if (!cell3.empty)
						{
							Cell transPreCell = cell3.TransPreCell;
							cell3.transPrePathDirection = transPreCell.transPathDirection;
							Direction transPathDirection = transPreCell.transPathDirection;
							Direction transPathDirection2 = cell3.transPathDirection;
							if (transPathDirection2 == Direction.Up && transPathDirection == Direction.Up)
							{
								ElementGenerator.Instance.Create(this, 302, pos10.y, pos10.x, isAnim).transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
							}
							else if (transPathDirection2 == Direction.Up && transPathDirection == Direction.Right)
							{
								GameObject obj = ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim);
								obj.transform.localScale = new Vector3(1f, 1f, 0f);
								obj.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
							}
							else if (transPathDirection2 == Direction.Up && transPathDirection == Direction.Left)
							{
								GameObject obj2 = ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim);
								obj2.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
								obj2.transform.localScale = new Vector3(1f, -1f, 0f);
							}
							else if (transPathDirection2 == Direction.Down && transPathDirection == Direction.Down)
							{
								ElementGenerator.Instance.Create(this, 302, pos10.y, pos10.x, isAnim).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
							}
							else if (transPathDirection2 == Direction.Down && transPathDirection == Direction.Right)
							{
								GameObject obj3 = ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim);
								obj3.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
								obj3.transform.localScale = new Vector3(-1f, 1f, 0f);
							}
							else if (transPathDirection2 == Direction.Down && transPathDirection == Direction.Left)
							{
								GameObject obj4 = ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim);
								obj4.transform.localScale = new Vector3(1f, 1f, 0f);
								obj4.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
							}
							else if (transPathDirection2 == Direction.Left && transPathDirection == Direction.Left)
							{
								ElementGenerator.Instance.Create(this, 302, pos10.y, pos10.x, isAnim);
							}
							else if (transPathDirection2 == Direction.Left && transPathDirection == Direction.Up)
							{
								ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim);
							}
							else if (transPathDirection2 == Direction.Left && transPathDirection == Direction.Down)
							{
								ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim).transform.localScale = new Vector3(1f, -1f, 0f);
							}
							else if (transPathDirection2 == Direction.Right && transPathDirection == Direction.Right)
							{
								ElementGenerator.Instance.Create(this, 302, pos10.y, pos10.x, isAnim).transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
							}
							else if (transPathDirection2 == Direction.Right && transPathDirection == Direction.Up)
							{
								ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim).transform.localScale = new Vector3(-1f, 1f, 0f);
							}
							else if (transPathDirection2 == Direction.Right && transPathDirection == Direction.Down)
							{
								ElementGenerator.Instance.Create(this, 301, pos10.y, pos10.x, isAnim).transform.localScale = new Vector3(-1f, -1f, 0f);
							}
						}
					}
					num10++;
				}
			}
			if (data.catPathList == null)
			{
				return;
			}
			int num15 = 0;
			for (int num16 = 0; num16 < data.catPathList.Length; num16++)
			{
				catCellDic.Add(num16, new List<Cell>());
			}
			pathList = data.catPathList;
			foreach (Path path3 in pathList)
			{
				for (int num17 = 0; num17 < path3.v.Length; num17++)
				{
					int num18 = path3.v.Length;
					Pos pos11 = path3.v[num17];
					Cell cell4 = cells[pos11.y, pos11.x];
					catCellDic[num15].Add(cell4);
					if (num17 == 0)
					{
						cell4.isCatHead = true;
						cell4.element.color = 25;
						cell4.element.type = ElementType.Fish;
						FishCellDic.Add(num15, cell4.element);
					}
					else if (num17 == path3.v.Length - 1)
					{
						cell4.isCatTail = true;
						cell4.element.color = 24;
						cell4.element.type = ElementType.Cat;
					}
					Pos pos12 = path3.v[(num17 + 1) % num18];
					cell4.CatNextCell = cells[pos12.y, pos12.x];
					Pos pos13 = path3.v[(num17 - 1 < 0) ? (num18 - 1) : (num17 - 1)];
					cell4.CatPreCell = cells[pos13.y, pos13.x];
					if (pos12.y > pos11.y)
					{
						cells[pos11.y, pos11.x].catPathDirection = Direction.Up;
					}
					else if (pos12.x > pos11.x)
					{
						cells[pos11.y, pos11.x].catPathDirection = Direction.Right;
					}
					else if (pos12.x < pos11.x)
					{
						cells[pos11.y, pos11.x].catPathDirection = Direction.Left;
					}
					else
					{
						cells[pos11.y, pos11.x].catPathDirection = Direction.Down;
					}
				}
				for (int num19 = 0; num19 < path3.v.Length - 1; num19++)
				{
					Pos pos14 = path3.v[num19];
					Cell cell5 = cells[pos14.y, pos14.x];
					if (!cell5.empty)
					{
						Cell catPreCell = cell5.CatPreCell;
						cell5.catPrePathDirection = catPreCell.catPathDirection;
						Direction catPathDirection = catPreCell.catPathDirection;
						Direction catPathDirection2 = cell5.catPathDirection;
						if (catPathDirection2 == Direction.Up && catPathDirection == Direction.Up)
						{
							ElementGenerator.Instance.Create(this, 304, pos14.y, pos14.x).transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
						}
						else if (catPathDirection2 == Direction.Up && catPathDirection == Direction.Right)
						{
							GameObject obj5 = ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x);
							obj5.transform.localScale = new Vector3(1f, 1f, 0f);
							obj5.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
						}
						else if (catPathDirection2 == Direction.Up && catPathDirection == Direction.Left)
						{
							GameObject obj6 = ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x);
							obj6.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
							obj6.transform.localScale = new Vector3(1f, -1f, 0f);
						}
						else if (catPathDirection2 == Direction.Down && catPathDirection == Direction.Down)
						{
							ElementGenerator.Instance.Create(this, 304, pos14.y, pos14.x).transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
						}
						else if (catPathDirection2 == Direction.Down && catPathDirection == Direction.Right)
						{
							GameObject obj7 = ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x);
							obj7.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
							obj7.transform.localScale = new Vector3(-1f, 1f, 0f);
						}
						else if (catPathDirection2 == Direction.Down && catPathDirection == Direction.Left)
						{
							GameObject obj8 = ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x);
							obj8.transform.localScale = new Vector3(1f, 1f, 0f);
							obj8.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
						}
						else if (catPathDirection2 == Direction.Left && catPathDirection == Direction.Left)
						{
							ElementGenerator.Instance.Create(this, 304, pos14.y, pos14.x);
						}
						else if (catPathDirection2 == Direction.Left && catPathDirection == Direction.Up)
						{
							ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x);
						}
						else if (catPathDirection2 == Direction.Left && catPathDirection == Direction.Down)
						{
							ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x).transform.localScale = new Vector3(1f, -1f, 0f);
						}
						else if (catPathDirection2 == Direction.Right && catPathDirection == Direction.Right)
						{
							ElementGenerator.Instance.Create(this, 304, pos14.y, pos14.x).transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
						}
						else if (catPathDirection2 == Direction.Right && catPathDirection == Direction.Up)
						{
							ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x).transform.localScale = new Vector3(-1f, 1f, 0f);
						}
						else if (catPathDirection2 == Direction.Right && catPathDirection == Direction.Down)
						{
							ElementGenerator.Instance.Create(this, 303, pos14.y, pos14.x).transform.localScale = new Vector3(-1f, -1f, 0f);
						}
					}
				}
				num15++;
			}
		}

		public void RandomOneCellToCreateCustomElement(ElementType type)
		{
			List<Cell> list = ProcessTool.ListRandom(GetHaveElementAndCellTool.GetNormalCellList(this));
			if (list.Count >= 1)
			{
				list[list.Count - 1].element.CreateBomb(type, false, false);
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.CreateOneElementByDrop, list[list.Count - 1].transform.position));
		}

		public void Reset()
		{
			currArea = 0;
			moveStep = 0;
			foreach (Transform item in container.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		private bool CheckHaveStepTemp(bool showHint)
		{
			for (int i = 0; i < MaxRow; i++)
			{
				for (int j = 0; j < MaxCol; j++)
				{
					int color = cells[i, j].GetColor();
					if (color <= 0)
					{
						continue;
					}
					if (i >= 4 && color == cells[i - 1, j].GetColor() && color == cells[i - 3, j].GetColor() && color == cells[i - 4, j].GetColor())
					{
						if (j > 1 && cells[i - 2, j - 1].GetColor() == color)
						{
							return true;
						}
						if (j < MaxCol && cells[i - 2, j + 1].GetColor() == color)
						{
							return true;
						}
					}
					if (j >= 4 && color == cells[i, j - 1].GetColor() && color == cells[i, j - 3].GetColor() && color == cells[i, j - 4].GetColor())
					{
						if (i >= 1 && cells[i - 1, j - 2].GetColor() == color)
						{
							return true;
						}
						if (i <= MaxRow - 1 && cells[i + 1, j - 2].GetColor() == color)
						{
							return true;
						}
					}
				}
			}
			for (int k = 0; k < MaxRow; k++)
			{
				for (int l = 0; l < MaxCol; l++)
				{
					int color2 = cells[k, l].GetColor();
					if (color2 == 0)
					{
						continue;
					}
					if (k >= 2 && cells[k - 1, l].GetColor() == color2)
					{
						if (l >= 2 && cells[k - 2, l - 1].GetColor() == color2 && cells[k - 2, l - 2].GetColor() == color2)
						{
							if (l <= MaxCol - 1 && cells[k - 2, l + 1].GetColor() == color2)
							{
								return true;
							}
							if (k >= 3 && cells[k - 3, l].GetColor() == color2)
							{
								return true;
							}
						}
						if (l <= MaxCol - 2 && cells[k - 2, l + 1].GetColor() == color2 && cells[k - 2, l + 2].GetColor() == color2)
						{
							if (l >= 1 && cells[k - 2, l - 1].GetColor() == color2)
							{
								return true;
							}
							if (k >= 3 && cells[k - 3, l].GetColor() == color2)
							{
								return true;
							}
						}
						if (l >= 1 && l <= MaxCol - 1 && cells[k - 2, l + 1].GetColor() == color2 && cells[k - 2, l - 1].GetColor() == color2 && k >= 3 && cells[k - 3, l].GetColor() == color2)
						{
							return true;
						}
					}
					if (k <= MaxRow - 2 && cells[k + 1, l].GetColor() == color2)
					{
						if (l >= 2 && cells[k + 2, l - 1].GetColor() == color2 && cells[k + 2, l - 2].GetColor() == color2)
						{
							if (l <= MaxCol - 1 && cells[k + 2, l + 1].GetColor() == color2)
							{
								return true;
							}
							if (k <= MaxRow - 3 && cells[k + 3, l].GetColor() == color2)
							{
								return true;
							}
						}
						if (l <= MaxCol - 2 && cells[k + 2, l + 1].GetColor() == color2 && cells[k + 2, l + 2].GetColor() == color2)
						{
							if (l >= 1 && cells[k + 2, l - 1].GetColor() == color2)
							{
								return true;
							}
							if (k <= MaxRow - 3 && cells[k + 3, l].GetColor() == color2)
							{
								return true;
							}
						}
						if (l >= 1 && l <= MaxCol - 1 && cells[k + 2, l + 1].GetColor() == color2 && cells[k + 2, l - 1].GetColor() == color2 && k <= MaxRow - 3 && cells[k + 3, l].GetColor() == color2)
						{
							return true;
						}
					}
					if (l < 2 || cells[k, l - 1].GetColor() != color2)
					{
						continue;
					}
					if (k >= 2 && cells[k - 1, l - 2].GetColor() == color2 && cells[k - 2, l - 2].GetColor() == color2)
					{
						if (k <= MaxRow - 1 && cells[k + 1, l - 2].GetColor() == color2)
						{
							return true;
						}
						if (l >= 3 && cells[k, l - 3].GetColor() == color2)
						{
							return true;
						}
					}
					if (k <= MaxRow - 2 && cells[k + 1, l - 2].GetColor() == color2 && cells[k + 2, l - 2].GetColor() == color2)
					{
						if (k >= 1 && cells[k - 1, l - 2].GetColor() == color2)
						{
							return true;
						}
						if (l >= 3 && cells[k, l - 3].GetColor() == color2)
						{
							return true;
						}
					}
					if (l >= 1 && l <= MaxCol - 1 && cells[k + 2, l + 1].GetColor() == color2 && cells[k + 2, l - 1].GetColor() == color2 && l >= 3 && cells[k, l - 3].GetColor() == color2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void RefreshBoard()
		{
			for (int i = levelRowStart; i <= levelRowEnd; i++)
			{
				for (int j = levelColStart; j <= levelColEnd; j++)
				{
					if (cells[i, j].GetColor() <= 0 || cells[i, j].Blocked() || cells[i, j].HaveJewel() || cells[i, j].HaveShell() || cells[i, j].HaveTreasure())
					{
						continue;
					}
					List<int> list = ProcessTool.ListRandom(colorList);
					int num = 1;
					Element element = cells[i, j].element;
					foreach (int item in list)
					{
						element.CreateStandard(item);
						if (RemoveMatchTool.RemoveMatchCheck(this, element, true).list == null)
						{
							break;
						}
					}
				}
			}
		}

		private void ShowHint(Element elem, Element target)
		{
			lastMoveTime = DateTime.Now.AddSeconds(5.0);
			DebugUtils.Log(DebugType.Other, "ShowHint " + lastMoveTime);
		}

		public void InitOneElement(int row, int col, int fblag)
		{
			cells[row, col].Mask.SetActive(false);
			float num = 0.78f * (float)col + offsetx;
			float num2 = 0.78f * (float)row + offsety;
			if (row % 2 == 0)
			{
				cells[row, col].bg.gameObject.SetActive(col % 2 == 0);
			}
			else
			{
				cells[row, col].bg.gameObject.SetActive(col % 2 != 0);
			}
			cells[row, col].board = this;
			cells[row, col].row = row;
			cells[row, col].col = col;
			int num3 = fblag;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			if (num3 >= 0 && num3 != 0)
			{
				num6 = num3 % 100;
				num3 -= num6;
				num5 = num3 % 1000;
				num3 -= num5;
				num7 = num3 % 100000;
				num3 -= num7;
				num4 = num3 % 10000000;
				num3 -= num4;
				int num10 = num3 % 100000000;
			}
			if (num4 != 0)
			{
				GameObject gameObject = ElementGenerator.Instance.Create(this, num4, row, col, false, false);
				gameObject.transform.SetParent(container.transform);
				float num8 = 0f;
				float num9 = 0f;
				switch (num4)
				{
				case 1000000:
					num9 = -0.39f;
					break;
				case 1100000:
					num8 = 0.39f;
					num9 = -1.17f;
					break;
				case 1200000:
					num8 = 0.78f;
					num9 = -1.94999993f;
					break;
				case 1300000:
					num8 = 0.39f;
					break;
				case 1400000:
					num8 = 1.17f;
					num9 = -0.39f;
					break;
				case 1500000:
					num8 = 1.94999993f;
					num9 = -0.78f;
					break;
				}
				gameObject.transform.localPosition = new Vector2(num + num8, num2 + num9);
				gameObject.name = "last_elem_" + row + "_" + col;
				gameObject.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 3;
				cells[row, col].lastElement = gameObject.GetComponent<Element>();
			}
			if (num5 != 0)
			{
				GameObject gameObject2 = ElementGenerator.Instance.Create(this, num5, row, col, false, false);
				gameObject2.transform.SetParent(container.transform);
				gameObject2.transform.localPosition = new Vector2(num, num2);
				gameObject2.name = "bottom_elem_" + row + "_" + col;
				gameObject2.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 10;
				gameObject2.GetComponentInChildren<Collider2D>().enabled = false;
				cells[row, col].bottomElement = gameObject2.GetComponent<Element>();
			}
			if (num7 != 0)
			{
				GameObject gameObject3 = ElementGenerator.Instance.Create(this, num7, row, col, false, false);
				gameObject3.transform.SetParent(container.transform);
				gameObject3.transform.localPosition = new Vector2(num, num2);
				gameObject3.name = "top_elem_" + row + "_" + col;
				gameObject3.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 35;
				cells[row, col].topElement = gameObject3.GetComponent<Element>();
			}
			if (num6 == 0 && num3 >= 0)
			{
				num6 = GetHaveElementAndCellTool.RandomCreateElement(this);
			}
			if (num3 < 0)
			{
				cells[row, col].empty = true;
				if (num3 == -1)
				{
					cells[row, col].gameObject.SetActive(true);
					cells[row, col].Mask.SetActive(true);
					SpriteRenderer[] componentsInChildren = cells[row, col].transform.GetComponentsInChildren<SpriteRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].gameObject.SetActive(false);
					}
					return;
				}
				cells[row, col].isPort = true;
				GameObject gameObject4 = ElementGenerator.Instance.Create(this, num3, row, col, false, false);
				if (gameObject4 != null)
				{
					gameObject4.transform.SetParent(container.transform);
					gameObject4.transform.localPosition = new Vector2(num, num2);
					gameObject4.name = "elem_" + row + "_" + col;
					gameObject4.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 35;
				}
				return;
			}
			cells[row, col].empty = false;
			GameObject gameObject5 = ElementGenerator.Instance.Create(this, num6, row, col, false, false);
			if (gameObject5 != null)
			{
				gameObject5.transform.SetParent(container.transform);
				gameObject5.transform.localPosition = new Vector2(num, num2);
				gameObject5.name = "elem_" + row + "_" + col;
				gameObject5.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 15;
				cells[row, col].element = gameObject5.GetComponent<Element>();
				cells[row, col].element.board = this;
			}
			if (num6 < 1 || num6 > 7 || RemoveMatchTool.RemoveMatchCheck(this, cells[row, col].element, true).list == null)
			{
				return;
			}
			List<int> list = ProcessTool.ListRandom(colorList);
			num6 = 1;
			Element element = cells[row, col].element;
			foreach (int item in list)
			{
				element.CreateStandard(item);
				if (RemoveMatchTool.RemoveMatchCheck(this, element, true).list == null)
				{
					break;
				}
			}
		}

		public void CreateMapBoard(int row, int col)
		{
			Cell cell = cells[row, col];
			Cell cellInMaxMap = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.Up, row, col);
			Cell cellInMaxMap2 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.Down, row, col);
			Cell cellInMaxMap3 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.Left, row, col);
			Cell cellInMaxMap4 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.Right, row, col);
			Cell cellInMaxMap5 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.LeftDown, row, col);
			Cell cellInMaxMap6 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.RightDown, row, col);
			Cell cellInMaxMap7 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.LeftUp, row, col);
			Cell cellInMaxMap8 = GetHaveElementAndCellTool.GetCellInMaxMap(this, Direction.RightUp, row, col);
			bool flag = cellInMaxMap == null || (cellInMaxMap.empty && !cellInMaxMap.isPort);
			bool flag2 = cellInMaxMap2 == null || (cellInMaxMap2.empty && !cellInMaxMap2.isPort);
			bool flag3 = cellInMaxMap3 == null || (cellInMaxMap3.empty && !cellInMaxMap3.isPort);
			bool flag4 = cellInMaxMap4 == null || (cellInMaxMap4.empty && !cellInMaxMap4.isPort);
			bool flag5 = cellInMaxMap7 == null || (cellInMaxMap7.empty && !cellInMaxMap7.isPort);
			bool flag6 = cellInMaxMap8 == null || (cellInMaxMap8.empty && !cellInMaxMap8.isPort);
			bool flag7 = cellInMaxMap5 == null || (cellInMaxMap5.empty && !cellInMaxMap5.isPort);
			bool flag8 = cellInMaxMap6 == null || (cellInMaxMap6.empty && !cellInMaxMap6.isPort);
			if (!cell.empty || cell.isPort)
			{
				if (flag && flag3 && flag5)
				{
					GameObject obj = PoolManager.Ins.SpawnEffect(50000056);
					obj.transform.SetParent(container.transform);
					obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					if (!flag4 && flag6)
					{
						GameObject obj2 = PoolManager.Ins.SpawnEffect(50000065);
						obj2.transform.SetParent(container.transform);
						obj2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj2.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag && flag4 && flag6)
				{
					GameObject obj3 = PoolManager.Ins.SpawnEffect(50000058);
					obj3.transform.SetParent(container.transform);
					obj3.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj3.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					if (!flag3 && flag5)
					{
						GameObject obj4 = PoolManager.Ins.SpawnEffect(50000064);
						obj4.transform.SetParent(container.transform);
						obj4.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj4.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag2 && flag3 && flag7)
				{
					GameObject obj5 = PoolManager.Ins.SpawnEffect(50000060);
					obj5.transform.SetParent(container.transform);
					obj5.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj5.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					if (!flag4 && flag8)
					{
						GameObject obj6 = PoolManager.Ins.SpawnEffect(50000070);
						obj6.transform.SetParent(container.transform);
						obj6.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj6.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag2 && flag4 && flag8)
				{
					GameObject obj7 = PoolManager.Ins.SpawnEffect(50000062);
					obj7.transform.SetParent(container.transform);
					obj7.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj7.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					if (!flag3 && flag7)
					{
						GameObject obj8 = PoolManager.Ins.SpawnEffect(50000071);
						obj8.transform.SetParent(container.transform);
						obj8.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj8.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag3)
				{
					if (!flag && !flag2)
					{
						if (flag5 && flag7)
						{
							GameObject obj9 = PoolManager.Ins.SpawnEffect(50000050);
							obj9.transform.SetParent(container.transform);
							obj9.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
							obj9.transform.localPosition = ProcessTool.GetPosition(this, row, col);
						}
					}
					else if (flag && !flag2 && flag7)
					{
						GameObject obj10 = PoolManager.Ins.SpawnEffect(50000067);
						obj10.transform.SetParent(container.transform);
						obj10.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj10.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
					else if (flag2 && !flag && flag5)
					{
						GameObject obj11 = PoolManager.Ins.SpawnEffect(50000066);
						obj11.transform.SetParent(container.transform);
						obj11.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj11.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag4)
				{
					if (!flag && !flag2)
					{
						if (flag6 && flag8)
						{
							GameObject obj12 = PoolManager.Ins.SpawnEffect(50000052);
							obj12.transform.SetParent(container.transform);
							obj12.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
							obj12.transform.localPosition = ProcessTool.GetPosition(this, row, col);
						}
					}
					else if (flag && !flag2 && flag8)
					{
						GameObject obj13 = PoolManager.Ins.SpawnEffect(50000069);
						obj13.transform.SetParent(container.transform);
						obj13.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj13.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
					else if (flag2 && !flag && flag6)
					{
						GameObject obj14 = PoolManager.Ins.SpawnEffect(50000068);
						obj14.transform.SetParent(container.transform);
						obj14.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
						obj14.transform.localPosition = ProcessTool.GetPosition(this, row, col);
					}
				}
				if (flag && !flag3 && !flag4 && flag5 && flag6)
				{
					GameObject obj15 = PoolManager.Ins.SpawnEffect(50000048);
					obj15.transform.SetParent(container.transform);
					obj15.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj15.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				if (flag2 && !flag3 && !flag4 && flag7 && flag8)
				{
					GameObject obj16 = PoolManager.Ins.SpawnEffect(50000054);
					obj16.transform.SetParent(container.transform);
					obj16.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj16.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				return;
			}
			if (!flag && !flag3)
			{
				GameObject obj17 = PoolManager.Ins.SpawnEffect(50000057);
				obj17.transform.SetParent(container.transform);
				obj17.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				obj17.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				if (flag4 && !flag6)
				{
					GameObject obj18 = PoolManager.Ins.SpawnEffect(50000073);
					obj18.transform.SetParent(container.transform);
					obj18.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj18.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				if (!flag7 && flag2)
				{
					GameObject obj19 = PoolManager.Ins.SpawnEffect(50000074);
					obj19.transform.SetParent(container.transform);
					obj19.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj19.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
			}
			if (!flag && !flag4)
			{
				GameObject obj20 = PoolManager.Ins.SpawnEffect(50000059);
				obj20.transform.SetParent(container.transform);
				obj20.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				obj20.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				if (flag3 && !flag5)
				{
					GameObject obj21 = PoolManager.Ins.SpawnEffect(50000072);
					obj21.transform.SetParent(container.transform);
					obj21.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj21.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				if (!flag8 && flag2)
				{
					GameObject obj22 = PoolManager.Ins.SpawnEffect(50000077);
					obj22.transform.SetParent(container.transform);
					obj22.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj22.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
			}
			if (!flag2 && !flag3)
			{
				GameObject obj23 = PoolManager.Ins.SpawnEffect(50000061);
				obj23.transform.SetParent(container.transform);
				obj23.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				obj23.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				if (flag4 && !flag8)
				{
					GameObject obj24 = PoolManager.Ins.SpawnEffect(50000078);
					obj24.transform.SetParent(container.transform);
					obj24.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj24.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				if (!flag5 && flag)
				{
					GameObject obj25 = PoolManager.Ins.SpawnEffect(50000075);
					obj25.transform.SetParent(container.transform);
					obj25.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj25.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
			}
			if (!flag2 && !flag4)
			{
				GameObject obj26 = PoolManager.Ins.SpawnEffect(50000063);
				obj26.transform.SetParent(container.transform);
				obj26.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				obj26.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				if (flag3 && !flag7)
				{
					GameObject obj27 = PoolManager.Ins.SpawnEffect(50000079);
					obj27.transform.SetParent(container.transform);
					obj27.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj27.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
				if (!flag6 && flag)
				{
					GameObject obj28 = PoolManager.Ins.SpawnEffect(50000076);
					obj28.transform.SetParent(container.transform);
					obj28.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					obj28.transform.localPosition = ProcessTool.GetPosition(this, row, col);
				}
			}
		}
	}
}
