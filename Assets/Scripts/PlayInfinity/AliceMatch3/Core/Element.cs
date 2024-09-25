using System;
using System.Collections;
using DG.Tweening;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public class Element : MonoBehaviour, IComparable
	{
		public GameObject img;

		private int _row;

		private int _col;

		public int color;

		public bool dragged;

		public Direction dragDirection;

		public Vector3 dragStartPos;

		private float minSpeed = 6f;

		private float moveMaxSpeed = 1000f;

		public float moveSpeed;

		public int FindFishCount;

		public Board board;

		public bool isMoveToTargetCell;

		private bool _Moving;

		public bool exploded;

		public bool removed;

		public bool isShowAnim = true;

		public bool isReadyToRemove;

		public bool isReadyToBomb;

		public bool isWaitForMatch;

		public bool isCanClose;

		public GameObject LeftXian;

		public GameObject RightXian;

		public GameObject UpXian;

		public GameObject DownXian;

		public ElementType TypeShow;

		private ElementType _type;

		public bool isPipeLine;

		public GameObject StandByBomb;

		public int row
		{
			get
			{
				return _row;
			}
			set
			{
				if (_row != value)
				{
					isWaitForMatch = false;
				}
				_row = value;
			}
		}

		public int col
		{
			get
			{
				return _col;
			}
			set
			{
				if (_col != value)
				{
					isWaitForMatch = false;
				}
				_col = value;
			}
		}

		public bool moving
		{
			get
			{
				return _Moving;
			}
			set
			{
				if (value && !_Moving)
				{
					board.movingCount++;
				}
				else if (!value && _Moving)
				{
					board.movingCount--;
				}
				_Moving = value;
			}
		}

		public ElementType type
		{
			get
			{
				return _type;
			}
			set
			{
				TypeShow = value;
				_type = value;
				if (value == ElementType.Treasure_0 || value == ElementType.NullTreasure_0)
				{
					isCanClose = true;
				}
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowStandbyEffect, new ShowStandbyEffect(this, value, img.transform, isShowAnim)));
				if (!GameLogic.Instance.BombAutoBomb || value < ElementType.FlyBomb || value > ElementType.ColorBomb)
				{
					return;
				}
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					if (base.gameObject.activeSelf)
					{
						StartCoroutine("Bomb");
					}
					return true;
				}));
			}
		}

		private IEnumerator Bomb()
		{
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.3f)
				{
					return true;
				}
				time += duration;
				return false;
			}));
			yield return new WaitUntil(() => time > 0.3f);
			Cell cell = board.cells[row, col];
			if (IsBomb() && !moving && !removed && !exploded)
			{
				Explode(cell.HaveGrass());
			}
		}

		private void Awake()
		{
			moveSpeed = Singleton<PlayGameData>.Instance().gameConfig.DropSpeed;
		}

		public int getWeight()
		{
			int value = -1;
			if (!GameLogic.Instance.WeightDic.TryGetValue(color, out value))
			{
				value = -1;
			}
			if (ProcessTool.ElementInTargetState(color) && !ProcessTool.isCollectFinish(color))
			{
				return value + 150;
			}
			return value;
		}

		private void OnMouseExit()
		{
			if (!(GameLogic.Instance == null) && GameLogic.Instance.isUserCanPlay && (IsBomb() || IsStandard() || IsJewel() || IsShell() || IsTreasure()) && dragged && !moving)
			{
				board.TapEffect.SetActive(false);
				dragged = false;
				board.currentTouchElem = null;
				Vector3 deltaPos = dragStartPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
				ProcessDrag(deltaPos);
			}
		}

		public void Upgrade()
		{
			if (color == 51)
			{
				color++;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.UpgradeTreasure, this));
			}
			else if (color == 61)
			{
				color++;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.UpgradeNullTreasure, this));
			}
		}

		public void ProcessDrag(Vector3 deltaPos)
		{
			if (!(Vector3.Magnitude(deltaPos) > 0.1f))
			{
				return;
			}
			if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x > 0f)
			{
				Element element = GetHaveElementAndCellTool.GetElement(board, Direction.Left, row, col);
				if (element != null)
				{
					Collider2D collider2D = Physics2D.OverlapPoint(element.transform.position);
					if (collider2D != null && collider2D.gameObject.GetComponent<Element>() != null)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, this, element, true, Direction.Left)));
					}
					else
					{
						MoveFail(Direction.Left);
					}
				}
				else
				{
					MoveFail(Direction.Left);
				}
			}
			else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x < 0f)
			{
				Element element = GetHaveElementAndCellTool.GetElement(board, Direction.Right, row, col);
				if (element != null)
				{
					Collider2D collider2D2 = Physics2D.OverlapPoint(element.transform.position);
					if (collider2D2 != null && collider2D2.gameObject.GetComponent<Element>() != null)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, this, element, true, Direction.Right)));
					}
					else
					{
						MoveFail(Direction.Right);
					}
				}
				else
				{
					MoveFail(Direction.Right);
				}
			}
			else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y > 0f)
			{
				Element element = GetHaveElementAndCellTool.GetElement(board, Direction.Down, row, col);
				if (element != null)
				{
					Collider2D collider2D3 = Physics2D.OverlapPoint(element.transform.position);
					if (collider2D3 != null && collider2D3.gameObject.GetComponent<Element>() != null)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, this, element, true, Direction.Down)));
					}
					else
					{
						MoveFail(Direction.Down);
					}
				}
				else
				{
					MoveFail(Direction.Down);
				}
			}
			else
			{
				if (!(Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y)) || !(deltaPos.y < 0f))
				{
					return;
				}
				Element element = GetHaveElementAndCellTool.GetElement(board, Direction.Up, row, col);
				if (element != null)
				{
					Collider2D collider2D4 = Physics2D.OverlapPoint(element.transform.position);
					if (collider2D4 != null && collider2D4.gameObject.GetComponent<Element>() != null)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, this, element, true, Direction.Up)));
					}
					else
					{
						MoveFail(Direction.Up);
					}
				}
				else
				{
					MoveFail(Direction.Up);
				}
			}
		}

		public void MoveFail(Direction dir)
		{
			float num = 0.2f;
			Vector3 vector = Vector3.zero;
			switch (dir)
			{
			case Direction.Left:
				vector = Vector3.left * num;
				break;
			case Direction.Right:
				vector = -Vector3.left * num;
				break;
			case Direction.Up:
				vector = Vector3.up * num;
				break;
			case Direction.Down:
				vector = -Vector3.up * num;
				break;
			}
			Vector3 localPosition = base.transform.localPosition;
			Vector3 endValue = base.transform.localPosition + vector;
			GetComponentInChildren<Collider2D>().enabled = false;
			base.transform.DOLocalMove(endValue, 0.125f).SetEase(Ease.OutQuad);
			base.transform.DOLocalMove(localPosition, 0.125f).SetEase(Ease.OutQuad).SetDelay(0.124f)
				.OnComplete(delegate
				{
					GetComponentInChildren<Collider2D>().enabled = true;
				});
		}

		public void ProcessDoubleClick()
		{
			if (IsBomb() && board.cells[row, col].isTopElementClear() && !exploded && !moving && !removed)
			{
				Singleton<MessageDispatcher>.Instance().SendMessage(2u, board);
				board.TapEffect.SetActive(false);
				Explode(board.cells[row, col].HaveGrass());
				GameLogic.Instance.TotleMoveCount++;
				GameLogic.Instance.isMoveCountReduce = true;
			}
		}

		public void CreateStandard(int color, bool isAnim = true, bool isUpdateTargetInfo = true)
		{
			int num = this.color;
			img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[color];
			img.GetComponent<SpriteRenderer>().sortingOrder = 15;
			this.color = color;
			isShowAnim = isAnim;
			type = (ElementType)color;
			switch (color)
			{
			case 22:
				board.JewelNum++;
				break;
			case 41:
			{
				SpriteRenderer renderer = img.GetComponent<SpriteRenderer>();
				GameLogic.Instance.currentWhiteCloudNum++;
				if (ProcessTool.ElementInTargetState(color))
				{
					GameLogic.Instance.levelData.targetList[11]++;
					GameLogic.Instance.levelData.targetListByCollect[11]++;
				}
				if (isUpdateTargetInfo)
				{
					GameSceneUIManager.Instance.UpdateTargetNum();
				}
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer, false)));
				if (isAnim)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.WhiteCloudCreate, new CloudCreate(base.transform.position, board.container.transform)));
					float time = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time > 0.4f)
						{
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer, true)));
							return true;
						}
						time += duration;
						return false;
					}));
				}
				else
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer, true)));
				}
				break;
			}
			case 42:
			{
				SpriteRenderer renderer2 = img.GetComponent<SpriteRenderer>();
				GameLogic.Instance.currentBlackCloudNum++;
				if (ProcessTool.ElementInTargetState(color))
				{
					GameLogic.Instance.levelData.targetList[11]++;
					GameLogic.Instance.levelData.targetListByCollect[11]++;
				}
				if (isUpdateTargetInfo)
				{
					GameSceneUIManager.Instance.UpdateTargetNum();
				}
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer2, false)));
				if (isAnim)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.BlackCloudCreate, new CloudCreate(base.transform.position, board.container.transform)));
					float time2 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time2 > 0.4f)
						{
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer2, true)));
							return true;
						}
						time2 += duration;
						return false;
					}));
				}
				else
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer2, true)));
				}
				break;
			}
			case 43:
				img.GetComponent<SpriteRenderer>();
				if (num == 42)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, num)));
				}
				else
				{
					GameLogic.Instance.currentBlackCloudNum++;
				}
				if (isAnim)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.BlackCloudLevelUp, new CloudCreate(base.transform.position, board.container.transform)));
					float time3 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time3 > 0.4f)
						{
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
							return true;
						}
						time3 += duration;
						return false;
					}));
				}
				else
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
				}
				break;
			case 21:
				if (!board.cells[row, col].isTopElementClear())
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ActiveShell, new ActiveShellInfo(this, false)));
				}
				break;
			case 71:
			case 72:
			case 73:
			case 74:
			{
				Cell cellLeftByNoDir = GetHaveElementAndCellTool.GetCellLeftByNoDir(board, row, col);
				if (cellLeftByNoDir != null && cellLeftByNoDir.HaveButton() && LeftXian == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/Shengzi"), base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
					LeftXian = gameObject;
					cellLeftByNoDir.element.RightXian = gameObject;
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellRightByNoDir(board, row, col);
				if (cellLeftByNoDir != null && cellLeftByNoDir.HaveButton() && RightXian == null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/Shengzi"), base.transform);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					RightXian = gameObject2;
					cellLeftByNoDir.element.LeftXian = gameObject2;
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellUpByNoDir(board, row, col);
				if (cellLeftByNoDir != null && cellLeftByNoDir.HaveButton() && UpXian == null)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/Shengzi"), base.transform);
					gameObject3.transform.localPosition = Vector3.zero;
					gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
					UpXian = gameObject3;
					cellLeftByNoDir.element.DownXian = gameObject3;
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellDownByNoDir(board, row, col);
				if (cellLeftByNoDir != null && cellLeftByNoDir.HaveButton() && DownXian == null)
				{
					GameObject gameObject4 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/Shengzi"), base.transform);
					gameObject4.transform.localPosition = Vector3.zero;
					gameObject4.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
					DownXian = gameObject4;
					cellLeftByNoDir.element.UpXian = gameObject4;
				}
				break;
			}
			}
		}

		public void CreateBottom(int color, bool isAnim = true, bool isUpdateTarget = true)
		{
			Cell cell = board.cells[row, col];
			base.transform.SetParent(board.container.transform);
			float x = 0.78f * (float)cell.col + board.offsetx;
			float y = 0.78f * (float)cell.row + board.offsety;
			base.transform.localPosition = new Vector2(x, y);
			base.name = "bottom_elem_" + cell.row + "_" + cell.col;
			base.transform.Find("Img").GetComponent<SpriteRenderer>().sortingOrder = 10;
			GetComponentInChildren<Collider2D>().enabled = false;
			cell.bottomElement = this;
			SpriteRenderer renderer = img.GetComponent<SpriteRenderer>();
			if (color == 600)
			{
				GameLogic.Instance.TotalNumBeGrass--;
				cell.isHavaGrass = true;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
				this.color = color;
				type = (ElementType)color;
				if (isAnim)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer, false)));
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.GrassCreate, new GrassCreate(base.transform.position, board.container.transform)));
					float time = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time > 0.4f)
						{
							RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ShowOrHideSprite, new ShowOrHideSprite(renderer, true)));
							return true;
						}
						time += duration;
						return false;
					}));
				}
				Cell cellLeftByNoDir = GetHaveElementAndCellTool.GetCellLeftByNoDir(board, row, col);
				if (cellLeftByNoDir != null && !cellLeftByNoDir.haveRightGrass && cellLeftByNoDir.HaveGrass())
				{
					cell.haveLeftGrass = true;
					GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/caodizuoyou"), cell.bottomElement.transform);
					obj.transform.localPosition = Vector3.zero;
					obj.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellRightByNoDir(board, row, col);
				if (cellLeftByNoDir != null && !cellLeftByNoDir.haveLeftGrass && cellLeftByNoDir.HaveGrass())
				{
					cell.haveRightGrass = true;
					GameObject obj2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/caodizuoyou"), cell.bottomElement.transform);
					obj2.transform.localPosition = Vector3.zero;
					obj2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellUpByNoDir(board, row, col);
				if (cellLeftByNoDir != null && !cellLeftByNoDir.haveDownGrass && cellLeftByNoDir.HaveGrass())
				{
					cell.haveUpGrass = true;
					GameObject obj3 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/caodishangxia"), cell.bottomElement.transform);
					obj3.transform.localPosition = Vector3.zero;
					obj3.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				}
				cellLeftByNoDir = GetHaveElementAndCellTool.GetCellDownByNoDir(board, row, col);
				if (cellLeftByNoDir != null && !cellLeftByNoDir.haveUpGrass && cellLeftByNoDir.HaveGrass())
				{
					cell.haveDownGrass = true;
					GameObject obj4 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Textures/GameElements/caodishangxia"), cell.bottomElement.transform);
					obj4.transform.localPosition = Vector3.zero;
					obj4.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
				}
				return;
			}
			if (color == 302 && isAnim)
			{
				Transform transform = base.transform.Find("Img");
				transform.GetComponent<SpriteRenderer>().enabled = false;
				Transform transform2 = PoolManager.Ins.SpawnEffect(color, transform).transform;
				transform2.localPosition = Vector3.zero;
				if (board.transMatList.Count == 0)
				{
					transform2.GetComponentInChildren<MeshRenderer>().GetSharedMaterials(board.transMatList);
				}
			}
			else
			{
				switch (color)
				{
				case 301:
					base.transform.Find("Img").localScale = Vector3.one * 0.95f;
					break;
				case 302:
					base.transform.Find("Img").localPosition = new Vector3(0f, 0f, 0f);
					base.transform.Find("Img").localScale = new Vector3(0.9525813f, 0.961f, 0f);
					break;
				case 303:
					base.transform.Find("Img").localScale = Vector3.one * 0.95f;
					break;
				case 304:
					base.transform.Find("Img").localPosition = new Vector3(0f, 0f, 0f);
					base.transform.Find("Img").localScale = new Vector3(0.9525813f, 0.961f, 0f);
					break;
				}
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
			this.color = color;
			type = (ElementType)color;
		}

		public void CreateTop(int color, bool isAnim = true, bool isUpdateTarget = true)
		{
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
			this.color = color;
			type = (ElementType)color;
		}

		public void CreateLast(int color, float xScale = 1f, float yScale = 1f, float zScale = 1f, float xRot = 0f, float yRot = 0f, float zRot = 0f, bool isAnim = true, bool isUpdateTarget = true)
		{
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
			img.transform.localScale = new Vector3(xScale, yScale, zScale);
			img.transform.rotation = Quaternion.Euler(xRot, yRot, zRot);
			base.transform.GetComponentInChildren<Collider2D>().enabled = false;
			this.color = color;
			type = (ElementType)color;
			Cell[,] cells = board.cells;
			cells[row, col].isInVase = true;
			switch (type)
			{
			case ElementType.Collect_1_2:
				cells[row - 1, col].isInVase = true;
				break;
			case ElementType.Collect_2_1:
				cells[row, col + 1].isInVase = true;
				break;
			case ElementType.Collect_2_4:
				cells[row, col + 1].isInVase = true;
				cells[row - 1, col].isInVase = true;
				cells[row - 2, col].isInVase = true;
				cells[row - 3, col].isInVase = true;
				cells[row - 1, col + 1].isInVase = true;
				cells[row - 2, col + 1].isInVase = true;
				cells[row - 3, col + 1].isInVase = true;
				break;
			case ElementType.Collect_4_2:
				cells[row, col + 1].isInVase = true;
				cells[row, col + 2].isInVase = true;
				cells[row, col + 3].isInVase = true;
				cells[row - 1, col].isInVase = true;
				cells[row - 1, col + 1].isInVase = true;
				cells[row - 1, col + 2].isInVase = true;
				cells[row - 1, col + 3].isInVase = true;
				break;
			case ElementType.Collect_3_6:
				cells[row, col + 1].isInVase = true;
				cells[row, col + 2].isInVase = true;
				cells[row - 1, col].isInVase = true;
				cells[row - 1, col + 1].isInVase = true;
				cells[row - 1, col + 2].isInVase = true;
				cells[row - 2, col].isInVase = true;
				cells[row - 2, col + 1].isInVase = true;
				cells[row - 2, col + 2].isInVase = true;
				cells[row - 3, col].isInVase = true;
				cells[row - 3, col + 1].isInVase = true;
				cells[row - 3, col + 2].isInVase = true;
				cells[row - 4, col].isInVase = true;
				cells[row - 4, col + 1].isInVase = true;
				cells[row - 4, col + 2].isInVase = true;
				cells[row - 5, col].isInVase = true;
				cells[row - 5, col + 1].isInVase = true;
				cells[row - 5, col + 2].isInVase = true;
				break;
			case ElementType.Collect_6_3:
				cells[row - 1, col].isInVase = true;
				cells[row - 2, col].isInVase = true;
				cells[row, col + 1].isInVase = true;
				cells[row, col + 2].isInVase = true;
				cells[row, col + 3].isInVase = true;
				cells[row, col + 4].isInVase = true;
				cells[row, col + 5].isInVase = true;
				cells[row - 1, col + 1].isInVase = true;
				cells[row - 2, col + 1].isInVase = true;
				cells[row - 1, col + 2].isInVase = true;
				cells[row - 2, col + 2].isInVase = true;
				cells[row - 1, col + 3].isInVase = true;
				cells[row - 2, col + 3].isInVase = true;
				cells[row - 1, col + 4].isInVase = true;
				cells[row - 2, col + 4].isInVase = true;
				cells[row - 1, col + 5].isInVase = true;
				cells[row - 2, col + 5].isInVase = true;
				break;
			}
		}

		public void CreateFirst(int color, bool isAnim = true, bool isUpdateTarget = true)
		{
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, color)));
			this.color = color;
			type = (ElementType)color;
		}

		public void CreateBomb(ElementType type, bool grassFlag = false, bool isUpdateTarget = true, bool isDrop = true, bool isAnim = true, bool isUpdateTarget2 = true)
		{
			int color2 = color;
			color = -1;
			isShowAnim = isAnim;
			this.type = type;
			base.transform.DOKill();
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ChangeSprite, new ChangeSprite(img, (int)type)));
			img.GetComponent<SpriteRenderer>().sortingOrder = 15;
			if (isDrop)
			{
				DropAndMoveTool.CheckDrop(board, board.cells[row, col]);
			}
		}

		public bool IsBomb()
		{
			if (type >= ElementType.FlyBomb && type <= ElementType.ColorBomb)
			{
				return true;
			}
			return false;
		}

		public bool IsStandard()
		{
			if (type >= ElementType.Standard_0 && type <= ElementType.Standard_6)
			{
				return true;
			}
			return false;
		}

		public bool IsStandard(ElementType checkType)
		{
			if (checkType >= ElementType.Standard_0 && checkType <= ElementType.Standard_6)
			{
				return true;
			}
			return false;
		}

		public bool IsJewel()
		{
			if (type == ElementType.Jewel)
			{
				return true;
			}
			return false;
		}

		public bool IsShell()
		{
			if (type == ElementType.Shell)
			{
				return true;
			}
			return false;
		}

		public bool IsTreasure()
		{
			if (type == ElementType.Treasure_0 || type == ElementType.Treasure_1 || type == ElementType.NullTreasure_0 || type == ElementType.NullTreasure_1)
			{
				return true;
			}
			return false;
		}

		public void Explode(bool grassFlag, int color = -1)
		{
			if (exploded)
			{
				return;
			}
			exploded = true;
			if (StandByBomb != null)
			{
				PoolManager.Ins.DeSpawnEffect(StandByBomb);
			}
			base.transform.DOKill();
			GameLogic.Instance.RemoveNum++;
			if (type == ElementType.HorizontalBomb)
			{
				DealElementTool.RemoveRow(board, row, col, grassFlag);
			}
			else if (type == ElementType.VerticalBomb)
			{
				DealElementTool.RemoveCol(board, row, col, grassFlag);
			}
			else if (type == ElementType.AreaBomb)
			{
				DealElementTool.RemoveArea(board, row, col, Singleton<PlayGameData>.Instance().gameConfig.AreaBombNum, grassFlag);
			}
			else if (type == ElementType.FlyBomb)
			{
				DealElementTool.FlyRemoveOneTarger(board, row, col, Singleton<PlayGameData>.Instance().gameConfig.BeeBombNum * GameLogic.Instance.BeeLevel, ElementType.None, grassFlag);
			}
			else if (type == ElementType.ColorBomb)
			{
				if (color == -1)
				{
					DealElementTool.RemoveColor(board, (int)GetHaveElementAndCellTool.GetRandomColorInBoard(board), board.cells[row, col], grassFlag);
				}
				else
				{
					DealElementTool.RemoveColor(board, color, board.cells[row, col], grassFlag);
				}
			}
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			if (GameLogic.Instance.BombToGold)
			{
				int coinNumByBombType = ProcessTool.GetCoinNumByBombType(type);
				Vector3 goldEndPos = GameSceneUIManager.Instance.GetGoldEndPos();
				goldEndPos.z = 0f;
				float time = Vector3.Distance(base.transform.position, goldEndPos) / gameConfig.CoinFlySpeed;
				UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.GoldFlyToTarget, new GoldFlyToTargetMessage(base.transform.position, goldEndPos, time, coinNumByBombType, board, board.container.transform)));
			}
		}

		public void SpeedUp()
		{
			moveSpeed = Mathf.Min(moveSpeed + Time.deltaTime * 20f, moveMaxSpeed);
		}

		public void ResetSpeed()
		{
			moveSpeed = Singleton<PlayGameData>.Instance().gameConfig.DropSpeed;
		}

		public int CompareTo(object obj)
		{
			Element element = obj as Element;
			if (row > element.row)
			{
				return 0;
			}
			return 1;
		}
	}
}
