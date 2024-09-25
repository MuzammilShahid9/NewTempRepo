using System;
using DG.Tweening;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public class Cell : MonoBehaviour, IComparable
	{
		public int row;

		public int col;

		private Direction _pathDirection;

		public Direction previousDirection;

		public Cell NextCell;

		public Cell PreCell;

		public SpriteRenderer bg;

		public SpriteRenderer DirTip;

		public Board board;

		private Element _element;

		public Element bottomElement;

		public Element lastElement;

		public Element topElement;

		public bool empty;

		public bool isInVase;

		public bool isPort;

		public bool isBlocked;

		public int totalWeight;

		public int eleWeight;

		public int bottomWeight;

		public bool isReadyToRemove;

		public bool DestroyBlock;

		public bool isHead;

		public bool isTail;

		public bool isHavaGrass;

		public bool isBeeFlyToThis;

		public bool isPortalEnter;

		public bool isPortalExit;

		public bool haveLeftGrass;

		public bool haveRightGrass;

		public bool haveUpGrass;

		public bool haveDownGrass;

		public bool isTransHead;

		public bool isTransTail;

		public bool isCatHead;

		public bool isCatTail;

		public Cell TransNextCell;

		public Cell TransPreCell;

		public Cell CatNextCell;

		public Cell CatPreCell;

		public Direction transPathDirection;

		public Direction transPrePathDirection;

		public Direction catPathDirection;

		public Direction catPrePathDirection;

		public GameObject Mask;

		public GameObject UpTipShow;

		public GameObject DownTipShow;

		public GameObject LeftTipShow;

		public GameObject RightTipShow;

		public Sequence seq;

		public Tweener myTweener;

		public Direction pathDirection
		{
			get
			{
				return _pathDirection;
			}
			set
			{
				_pathDirection = value;
				switch (value)
				{
				case Direction.Up:
					DirTip.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
					break;
				case Direction.Down:
					DirTip.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					break;
				case Direction.Left:
					DirTip.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
					break;
				case Direction.Right:
					DirTip.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				}
			}
		}

		public Element element
		{
			get
			{
				return _element;
			}
			set
			{
				if (value == null && base.gameObject != null && base.gameObject.activeSelf)
				{
					float time = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time > 0.02f)
						{
							DropAndMoveTool.CheckUpDrop(board, this);
							return true;
						}
						time += duration;
						return false;
					}));
				}
				_element = value;
			}
		}

		public bool Blocked()
		{
			if (isBlocked)
			{
				return true;
			}
			if (empty)
			{
				return true;
			}
			if (topElement != null)
			{
				return true;
			}
			if (isButton() || isBox() || isCat() || isFish() || HaveWhitCloud() || HaveBlackCloud() || HaveButterfly())
			{
				return true;
			}
			return false;
		}

		public bool HaveGrass()
		{
			return isHavaGrass;
		}

		public bool isCat()
		{
			if (element != null)
			{
				return element.type == ElementType.Cat;
			}
			return false;
		}

		public bool isFish()
		{
			if (element != null)
			{
				return element.type == ElementType.Fish;
			}
			return false;
		}

		public bool isBox()
		{
			if (element != null)
			{
				if (element.type >= ElementType.Box_0)
				{
					return element.type <= ElementType.Box_4;
				}
				return false;
			}
			return false;
		}

		public bool isButton()
		{
			if (element != null)
			{
				if (element.type >= ElementType.Button_0)
				{
					return element.type <= ElementType.Button_3;
				}
				return false;
			}
			return false;
		}

		public bool isTreasure()
		{
			if (element != null)
			{
				if (element.type < ElementType.Treasure_0 || element.type > ElementType.Treasure_1)
				{
					if (element.type >= ElementType.NullTreasure_0)
					{
						return element.type <= ElementType.NullTreasure_1;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public bool HaveBox()
		{
			if (element != null && element.color >= 31 && element.color <= 35)
			{
				return true;
			}
			return false;
		}

		public bool HaveButton()
		{
			if (element != null && element.color >= 71 && element.color <= 74)
			{
				return true;
			}
			return false;
		}

		public bool HaveTreasure()
		{
			if (element != null && ((element.color >= 51 && element.color <= 52) || (element.color >= 61 && element.color <= 62)))
			{
				return true;
			}
			return false;
		}

		public bool HaveWhitCloud()
		{
			if (element != null && element.color == 41 && !element.removed)
			{
				return true;
			}
			return false;
		}

		public bool HaveBlackCloud()
		{
			if (element != null && element.color >= 42 && element.color <= 43 && !element.removed)
			{
				return true;
			}
			return false;
		}

		public bool HaveShell()
		{
			if (element != null && element.color == 21 && !element.removed)
			{
				return true;
			}
			return false;
		}

		public bool HaveJewel()
		{
			if (element != null && element.color == 22)
			{
				return true;
			}
			return false;
		}

		public bool HaveHoney()
		{
			if (topElement != null && topElement.color >= 20000 && topElement.color <= 24000)
			{
				return true;
			}
			return false;
		}

		public bool HaveBramble()
		{
			if (topElement != null && topElement.color >= 10000 && topElement.color <= 13000)
			{
				return true;
			}
			return false;
		}

		public bool HaveIce()
		{
			if (bottomElement != null && bottomElement.color >= 100 && bottomElement.color <= 300)
			{
				return true;
			}
			return false;
		}

		public bool HaveButterfly()
		{
			if (element != null && element.color == 23)
			{
				return true;
			}
			return false;
		}

		public bool HaveVase()
		{
			if (lastElement != null && lastElement.color >= 1000000 && lastElement.color <= 1500000)
			{
				return true;
			}
			return false;
		}

		public int GetColor()
		{
			if (empty)
			{
				return 0;
			}
			if (element == null)
			{
				return 0;
			}
			return element.color;
		}

		public Direction GetDirection()
		{
			return pathDirection;
		}

		public Direction GetPreviousDirection()
		{
			return previousDirection;
		}

		public int CalWeight(bool grassFlag)
		{
			totalWeight = 0;
			if (GetHaveElementAndCellTool.HavePreviousCell(board, this))
			{
				Cell previousCell = GetHaveElementAndCellTool.GetPreviousCell(board, this);
				if (!empty && !HaveJewel() && previousCell.element != null && previousCell.element.color == 22)
				{
					totalWeight += 150;
				}
			}
			if (element != null && element.color == 22 && topElement != null)
			{
				totalWeight += 300;
			}
			if (grassFlag && element != null && element.color != 22 && !HaveGrass())
			{
				totalWeight += 150;
			}
			if (isInVase && bottomElement != null)
			{
				if (element != null)
				{
					if (element.color != 22)
					{
						totalWeight += 150;
					}
				}
				else
				{
					totalWeight += 150;
				}
			}
			int num = ((!(element == null)) ? element.getWeight() : 0);
			int num2 = ((!isTopElementClear()) ? topElement.getWeight() : 0);
			totalWeight += num;
			totalWeight += num2;
			return totalWeight;
		}

		public int CompareTo(object obj)
		{
			Cell cell = obj as Cell;
			if (totalWeight > cell.totalWeight)
			{
				return 1;
			}
			return 0;
		}

		public void ActiveTipEffect(Sequence seq = null)
		{
			this.seq = seq;
		}

		public bool isTopElementClear()
		{
			if (topElement == null)
			{
				return true;
			}
			if (topElement.type >= ElementType.Bramble_0 && topElement.type <= ElementType.Bramble_3)
			{
				return topElement.color < 10000;
			}
			if (topElement.type >= ElementType.Honey_0 && topElement.type <= ElementType.Honey_4)
			{
				return topElement.color < 20000;
			}
			DebugUtils.LogError(DebugType.Battle, "Current Element Type is " + topElement.type);
			return false;
		}
	}
}
