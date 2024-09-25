using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Editor;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using SimpleJSON;
using Spine.Unity;
using Umeng;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Core
{
	public class GameLogic : MonoBehaviour
	{
		private static GameLogic _instance;

		public LevelData levelData;

		public float currentCD;

		public float currentTipWaitCD;

		public float currentMoveAreaWaitCD;

		private float maxCD = 0.5f;

		private float MaxMoveAreaWaitCD = 0.4f;

		public float currentClickCD;

		private float pressClickCD = 0.1f;

		public float currentCheckDieCD;

		public float StepFinish = 20f;

		private List<Element> colorBombExplodeList;

		private Element[] elementArray;

		[HideInInspector]
		public Board[] boardArray;

		public int currMap;

		public Board currentBoard;

		public bool isUserCanPlay;

		public bool isFinish;

		public GameObject GamePrefab;

		public int[] tarIDArray;

		public int[] boosterUsageData = new int[12];

		private int totalNumBeGrass;

		public int currentWhiteCloudNum;

		public int currentBlackCloudNum;

		public int BeeLevel = 1;

		public bool CreateAreabomb;

		public bool CreateColorBomb;

		private DropType _dropType;

		private ElementType removeTrue = ElementType.None;

		private Coroutine coroutin;

		private float maxRefreshTime = 2f;

		private float maxWaitTime = 2f;

		public bool isTiping;

		public bool isTipEnable = true;

		public bool isFinishing;

		private bool _BombAutoBomb;

		public bool BombToGold;

		public RectTransform BoardParent;

		public bool canDropJewel = true;

		public bool isDestroyWhiteCloud;

		public bool isDestroyBlackCloud;

		public bool isDestroyAnyThing;

		public bool isGuiding;

		public bool isCanDoubleActiveBomb = true;

		public int RemoveNum;

		public int matchNum;

		public GameObject yifener;

		public int initCoinsNum;

		public int initScrollNum;

		public int initBankNum;

		private bool IsReadyToTrasport = true;

		private bool IsReadyToFindFish = true;

		public Dictionary<int, int> WeightDic;

		private Element _bomb;

		private Element EffectTipBomb;

		private RemoveCheckInfo _tipTarget;

		public RemoveCheckInfo EffectTipTarget;

		private bool isMapMoving;

		public bool isAreaMoving;

		private bool isRefreshing;

		private Coroutine co;

		private int[] probabilityList;

		private int AdjustNum = 2;

		private Dictionary<int, int[]> hardDic1 = new Dictionary<int, int[]>
		{
			{
				0,
				new int[4] { 1, 2, 3, 4 }
			},
			{
				1,
				new int[4] { 2, 3, 4, 5 }
			},
			{
				2,
				new int[4] { 3, 4, 5, 6 }
			}
		};

		private Dictionary<int, int[]> hardDic2 = new Dictionary<int, int[]>
		{
			{
				0,
				new int[4] { 2, 3, 4, 5 }
			},
			{
				1,
				new int[4] { 4, 5, 6, 7 }
			},
			{
				2,
				new int[4] { 6, 7, 8, 9 }
			}
		};

		private Dictionary<int, int[]> hardDic3 = new Dictionary<int, int[]>
		{
			{
				0,
				new int[4] { 3, 4, 5, 6 }
			},
			{
				1,
				new int[4] { 6, 7, 8, 9 }
			},
			{
				2,
				new int[4] { 9, 10, 11, 12 }
			}
		};

		public int DropUseNum;

		private int totleMoveCount;

		public bool isMoveCountReduce;

		public int[] targetState = new int[13];

		public int continueNum;

		private bool isFinishStep = true;

		private bool isGameRunning;

		public int[] target = new int[4];

		public int stepCount;

		private Dictionary<Cell, bool> MovedFishCell = new Dictionary<Cell, bool>();

		public static GameLogic Instance
		{
			get
			{
				return _instance;
			}
		}

		public int TotalNumBeGrass
		{
			get
			{
				return totalNumBeGrass;
			}
			set
			{
				totalNumBeGrass = value;
			}
		}

		public int TotalNumBeCloud
		{
			get
			{
				return currentWhiteCloudNum + currentBlackCloudNum;
			}
		}

		public DropType dropType
		{
			get
			{
				return _dropType;
			}
			set
			{
				_dropType = value;
				if (value != 0)
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(14u, value);
				}
			}
		}

		public ElementType RemoveTrue
		{
			get
			{
				return removeTrue;
			}
			set
			{
				removeTrue = value;
			}
		}

		public bool BombAutoBomb
		{
			get
			{
				return _BombAutoBomb;
			}
			set
			{
				_BombAutoBomb = value;
			}
		}

		private Element Bomb
		{
			get
			{
				return _bomb;
			}
			set
			{
				_bomb = value;
				if (value != null)
				{
					EffectTipBomb = value;
				}
			}
		}

		private RemoveCheckInfo tipTarget
		{
			get
			{
				return _tipTarget;
			}
			set
			{
				_tipTarget = value;
				if (value != null)
				{
					EffectTipTarget = value;
				}
			}
		}

		public int TotleMoveCount
		{
			get
			{
				return totleMoveCount;
			}
			set
			{
				totleMoveCount = value;
				GameSceneUIManager.Instance.SetMoveText(Instance.levelData.move - value);
				if (coroutin != null)
				{
					StopCoroutine(coroutin);
				}
				if (value >= levelData.move)
				{
					if (Instance.isFinish)
					{
						return;
					}
					ProcessTool.GameEndUIDeal(currentBoard, delegate
					{
						if (!Instance.isFinish)
						{
							DebugUtils.Log(DebugType.Other, "Game Lose Level " + UserDataManager.Instance.GetProgress());
							StartAnalyticsIEnumerator(2);
							if (UserDataManager.Instance.GetService().life <= 0 && !UserDataManager.Instance.GetService().NoLifeLevelSend && UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
							{
								Dictionary<string, string> attributes = new Dictionary<string, string> { 
								{
									"NoLifeLevel",
									UserDataManager.Instance.GetService().level.ToString()
								} };
								Analytics.Event("NoLifeLevel", attributes);
								Analytics.Event("NoLifeLevelCalculate", attributes, UserDataManager.Instance.GetService().level);
								UserDataManager.Instance.GetService().NoLifeLevelSend = true;
								UserDataManager.Instance.Save();
							}
							if (continueNum == 0)
							{
								if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
								{
									GA.FailLevel(UserDataManager.Instance.GetService().level.ToString());
								}
								for (int i = 1; i < Instance.tarIDArray.Length; i++)
								{
									target[i - 1] = Instance.levelData.targetList[Instance.tarIDArray[i]];
								}
							}
							continueNum++;
							DialogManagerTemp.Instance.ShowDialog(DialogType.GameLoseDlg);
						}
					});
				}
				else
				{
					isDestroyAnyThing = true;
				}
			}
		}

		private void Awake()
		{
			_instance = this;
			GamePrefab = Resources.Load("Prefabs/GameScene/Game", typeof(GameObject)) as GameObject;
		}

		private void Start()
		{
			BeeLevel = GameConfig.BeeLevel;
			GlobalVariables.LevelStartTime = (int)(DateTime.Now.Ticks / 10000000);
			CreateAreabomb = GameConfig.CreateOneAreaBoom;
			CreateColorBomb = GameConfig.CreateOneColorBoom;
			GlobalVariables.UseAddStep = false;
			GlobalVariables.ChallenageNum++;
			GlobalVariables.FlyingGoldCoinCount = 0;
			GameConfig.BeeLevel = 1;
			GameConfig.CreateOneAreaBoom = false;
			GameConfig.CreateOneColorBoom = false;
			WeightDic = new Dictionary<int, int>(Singleton<PlayGameData>.Instance().WeightDic);
			LoadSaveData();
			SetLevelInfo();
			if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
			{
				GA.StartLevel(UserDataManager.Instance.GetService().level.ToString());
			}
			boardArray = new Board[levelData.boardData.Length];
			BoardParent.transform.localPosition = new Vector3((levelData.boardData.Length - 1) * -12, 0f, 0f);
			for (int i = 0; i < levelData.boardData.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(GamePrefab);
				gameObject.transform.SetParent(BoardParent);
				LayoutRebuilder.ForceRebuildLayoutImmediate(BoardParent);
				boardArray[i] = gameObject.GetComponentInChildren<Board>();
				boardArray[i].Create(i);
			}
			currentBoard = boardArray[0];
			GameSceneUIManager.Instance.InitTargetInfo(tarIDArray);
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 1f)
				{
					if (levelData.boardData.Length > 1)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.ShowAllMap, currentBoard));
						Timer.Schedule(this, 2f, delegate
						{
							Init();
						});
					}
					else if (currentBoard.AreaPosDic.Count > 1)
					{
						UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.ShowAllArea, currentBoard));
						Timer.Schedule(this, (currentBoard.AreaPosDic.Count - 1) * 2, delegate
						{
							Init();
						});
					}
					else
					{
						Init();
					}
					return true;
				}
				time += duration;
				return false;
			}));
		}

		private void Init()
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.TargetNoticeDlg, tarIDArray);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(1u, StopTip);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(2u, StopTip);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(3u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(4u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(5u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(6u, EnableOrDeEnablePlayerControll);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(7u, MoveArea);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(8u, MoveMap);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(9u, GameStart);
			Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(10u, GameEnd);
		}

		public void AutoMatch(RemoveCheckInfo info)
		{
			if (!isFinish)
			{
				info.AutoMatch();
			}
		}

		private IEnumerator TipShow(RemoveCheckInfo info)
		{
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > maxWaitTime)
				{
					return true;
				}
				time += duration;
				return false;
			}));
			yield return new WaitUntil(() => time > maxWaitTime);
			if (!isFinish)
			{
				info.Tip();
			}
			coroutin = null;
		}

		private void SetTipTarget(RemoveCheckInfo info)
		{
			if (info.list == null || info.list.Count == 1 || info.otherList.Count == 0)
			{
				return;
			}
			if (tipTarget == null)
			{
				tipTarget = info;
			}
			else if (info.type > tipTarget.type)
			{
				tipTarget = info;
			}
			else if (info.type == tipTarget.type && info.type != ElementType.Standard_0)
			{
				if (info.totalWeight > tipTarget.totalWeight)
				{
					tipTarget = info;
				}
			}
			else if (tipTarget.type == ElementType.Standard_0 && info.type == ElementType.Standard_0 && info.totalWeight > tipTarget.totalWeight)
			{
				tipTarget = info;
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{
				StartCoroutine(SendAnalyticsData(1));
			}
			if (UpdateManager.Instance.isPause)
			{
				return;
			}
			StepFinish += Time.deltaTime;
			if (StepFinish > 0.4f && StepFinish < 30000f && !SkillManager.Instance.GetIsSkilling() && currentBoard.movingCount <= 0)
			{
				bool flag = false;
				bool flag2 = false;
				if (currentBoard.FishCellDic.Count > 0 && isMoveCountReduce && IsReadyToFindFish)
				{
					flag = true;
					IsReadyToFindFish = false;
					StartFindFish(currentBoard);
				}
				if (currentBoard.transporterCellDic.Count > 0 && isMoveCountReduce && IsReadyToTrasport)
				{
					flag2 = true;
					IsReadyToTrasport = false;
					StartTransporter(currentBoard);
				}
				if (!flag && !flag2)
				{
					StepFinish = 3E+08f;
					PlayerControlFinish(currentBoard);
				}
			}
			if (TestConfig.isAutoMatch && !isGuiding && !isFinish)
			{
				if (tipTarget != null && tipTarget.type == ElementType.Standard_0)
				{
					if (Bomb == null)
					{
						AutoMatch(tipTarget);
					}
					else
					{
						AutoMatch(new RemoveCheckInfo(ElementType.ColorBomb, new List<Element> { Bomb }, currentBoard, Bomb));
					}
				}
				else if (tipTarget != null && tipTarget.type != ElementType.Standard_0 && tipTarget.type != ElementType.None)
				{
					AutoMatch(tipTarget);
				}
				else if (tipTarget == null && Bomb != null)
				{
					AutoMatch(new RemoveCheckInfo(ElementType.ColorBomb, new List<Element> { Bomb }, currentBoard, Bomb));
				}
				tipTarget = null;
				Bomb = null;
			}
			if (isTipEnable && !isGuiding && !isTiping && !isFinish)
			{
				if (coroutin != null)
				{
					StopCoroutine(coroutin);
				}
				currentTipWaitCD += Time.deltaTime;
				if (currentTipWaitCD >= maxRefreshTime)
				{
					isTiping = true;
					if (tipTarget != null && tipTarget.type == ElementType.Standard_0)
					{
						if (Bomb == null)
						{
							if (coroutin != null)
							{
								StopCoroutine(coroutin);
							}
							RemoveCheckInfo tipTarget3 = tipTarget;
							UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
							{
								coroutin = StartCoroutine("TipShow", tipTarget3);
								return true;
							}));
						}
						else
						{
							if (coroutin != null)
							{
								StopCoroutine(coroutin);
							}
							Element Bomb3 = Bomb;
							UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
							{
								coroutin = StartCoroutine("TipShow", new RemoveCheckInfo(ElementType.ColorBomb, new List<Element> { Bomb3 }, currentBoard, Bomb));
								return true;
							}));
						}
					}
					else if (tipTarget != null && tipTarget.type != ElementType.Standard_0 && tipTarget.type != ElementType.None)
					{
						if (coroutin != null)
						{
							StopCoroutine(coroutin);
						}
						RemoveCheckInfo tipTarget2 = tipTarget;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
						{
							coroutin = StartCoroutine("TipShow", tipTarget2);
							return true;
						}));
					}
					else if (tipTarget == null && Bomb != null)
					{
						if (coroutin != null)
						{
							StopCoroutine(coroutin);
						}
						Element Bomb2 = Bomb;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
						{
							coroutin = StartCoroutine("TipShow", new RemoveCheckInfo(ElementType.ColorBomb, new List<Element> { Bomb2 }, currentBoard, Bomb));
							return true;
						}));
					}
					tipTarget = null;
					Bomb = null;
				}
			}
			currentClickCD += Time.deltaTime;
			if (Input.GetMouseButtonDown(0) && currentClickCD > pressClickCD && currentBoard.movingCount == 0)
			{
				currentClickCD = 0f;
				Collider2D collider2D = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				if (!(collider2D != null) || !(collider2D.gameObject.GetComponent<Element>() != null))
				{
					return;
				}
				Element component = collider2D.gameObject.GetComponent<Element>();
				if (!isUserCanPlay || !GetHaveElementAndCellTool.CheckCellInCrrentArea(currentBoard, component))
				{
					return;
				}
				Board board = component.board;
				int row = component.row;
				int col = component.col;
				if (Instance.dropType == DropType.Hammer)
				{
					board.TapEffect.SetActive(false);
					Vector3 position = board.cells[row, col].transform.position;
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.HammerHit, position));
					Instance.DropUsed(Instance.dropType, true);
					Singleton<MessageDispatcher>.Instance().SendMessage(2u, board);
					float time2 = 0f;
					float maxTime2 = 0.9f;
					board.movingCount++;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time2 >= maxTime2)
						{
							DealElementTool.RemoveRow(board, row, col, board.cells[row, col].HaveGrass());
							DealElementTool.RemoveCol(board, row, col, board.cells[row, col].HaveGrass());
							board.movingCount--;
							return true;
						}
						time2 += duration;
						return false;
					}));
				}
				else if (Instance.dropType == DropType.Spoon)
				{
					board.TapEffect.SetActive(false);
					Vector3 position2 = board.cells[row, col].transform.position;
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.SpoonHit, position2));
					Instance.DropUsed(Instance.dropType, true);
					Singleton<MessageDispatcher>.Instance().SendMessage(2u, board);
					float time = 0f;
					float maxTime = 0.9f;
					board.movingCount++;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time >= maxTime)
						{
							List<ElementRemoveInfo> list = new List<ElementRemoveInfo>
							{
								new ElementRemoveInfo(board.cells[row, col])
							};
							UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list, 0f, board)));
							board.movingCount--;
							return true;
						}
						time += duration;
						return false;
					}));
				}
				if (board.cells[row, col].Blocked() || (!component.IsBomb() && !component.IsStandard() && !component.IsJewel() && !component.IsShell() && !component.IsTreasure()) || component.moving)
				{
					return;
				}
				if (Instance.isCanDoubleActiveBomb && dropType != DropType.Glove && board.currentTouchElem == component)
				{
					board.currentTouchElem = null;
					component.ProcessDoubleClick();
				}
				else if (board.currentTouchElem == null || (board.currentTouchElem != null && !GetHaveElementAndCellTool.ElementsIsNeighbor(board.currentTouchElem, component)))
				{
					board.currentTouchElem = component;
					board.currentDragElem = component;
					component.dragged = true;
					component.dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					board.TapEffect.SetActive(true);
					board.TapEffect.transform.position = board.cells[row, col].transform.position;
					if (co != null)
					{
						StopCoroutine(co);
					}
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate
					{
						co = StartCoroutine(CancelCurrentTouchElem(board));
						return true;
					}));
				}
				else if (board.currentTouchElem != null && !board.currentTouchElem.moving && !component.moving)
				{
					board.TapEffect.SetActive(false);
					Direction dirToElements = GetHaveElementAndCellTool.GetDirToElements(board.currentTouchElem, component);
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.SwitchElement, new SwitchMessage(board, board.currentTouchElem, component, true, dirToElements)));
					board.currentTouchElem = null;
				}
			}
			else if (Input.GetMouseButtonUp(0) && currentBoard.currentDragElem != null)
			{
				currentBoard.currentDragElem.dragged = false;
				currentBoard.currentDragElem = null;
			}
		}

		private IEnumerator CancelCurrentTouchElem(Board board)
		{
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 2f)
				{
					return true;
				}
				time += duration;
				return false;
			}));
			yield return new WaitUntil(() => time > 2f);
			board.currentTouchElem = null;
		}

		private void SetLevelInfo()
		{
			int num = 0;
			int[] targetList = levelData.targetList;
			for (int i = 0; i < targetList.Length; i++)
			{
				if (targetList[i] != 0)
				{
					num++;
				}
			}
			tarIDArray = new int[num + 1];
			num = 0;
			for (int j = 0; j < targetList.Length; j++)
			{
				if (targetList[j] != 0)
				{
					num++;
					tarIDArray[num] = j;
				}
			}
		}

		public void LoadSaveData()
		{
			DebugUtils.Log(DebugType.Other, "Levels/Level_" + UserDataManager.Instance.GetProgress());
			TextAsset textAsset = Resources.Load<TextAsset>("Levels/Level_" + UserDataManager.Instance.GetProgress());
			SimpleJSON.JSONNode jSONNode = HotFixScript.Instance.LoadLevelData(UserDataManager.Instance.GetProgress());
			if (jSONNode == null)
			{
				DebugUtils.Log(DebugType.Other, "levelDataLoadFrom local");
				jSONNode = SimpleJSON.JSONNode.Parse(textAsset.text);
			}
			else
			{
				DebugUtils.Log(DebugType.Other, "levelDataLoadFrom ab");
			}
			DebugUtils.Log(DebugType.Other, jSONNode);
			levelData.move = int.Parse(jSONNode["move"]);
			levelData.hard = int.Parse(jSONNode["hard"]);
			if (GlobalVariables.probabilityList == null)
			{
				levelData.probabilityList = new int[jSONNode["probabilityList"].Count];
				for (int i = 0; i < jSONNode["probabilityList"].Count; i++)
				{
					levelData.probabilityList[i] = int.Parse(jSONNode["probabilityList"][i]);
				}
				GlobalVariables.probabilityList = levelData.probabilityList;
			}
			levelData.skillProbabilityList = new int[jSONNode["skillProbabilityList"].Count];
			for (int j = 0; j < jSONNode["skillProbabilityList"].Count; j++)
			{
				levelData.skillProbabilityList[j] = int.Parse(jSONNode["skillProbabilityList"][j]);
			}
			levelData.targetList = new int[jSONNode["targetList"].Count];
			levelData.targetListByCollect = new int[jSONNode["targetList"].Count];
			for (int k = 0; k < jSONNode["targetList"].Count; k++)
			{
				levelData.targetList[k] = int.Parse(jSONNode["targetList"][k]);
				levelData.targetListByCollect[k] = int.Parse(jSONNode["targetList"][k]);
			}
			levelData.boardData = new BoardData[jSONNode["boardData"].Count];
			for (int l = 0; l < jSONNode["boardData"].Count; l++)
			{
				levelData.boardData[l] = new BoardData();
				int num = int.Parse(jSONNode["boardData"][l]["width"]);
				levelData.boardData[l].width = num;
				int num2 = int.Parse(jSONNode["boardData"][l]["height"]);
				levelData.boardData[l].height = num2;
				levelData.boardData[l].pathList = new PlayInfinity.AliceMatch3.Editor.Path[jSONNode["boardData"][l]["pathList"].Count];
				for (int m = 0; m < jSONNode["boardData"][l]["pathList"].Count; m++)
				{
					levelData.boardData[l].pathList[m] = new PlayInfinity.AliceMatch3.Editor.Path();
					levelData.boardData[l].pathList[m].v = new Pos[jSONNode["boardData"][l]["pathList"][m]["v"].Count];
					for (int n = 0; n < jSONNode["boardData"][l]["pathList"][m]["v"].Count; n++)
					{
						levelData.boardData[l].pathList[m].v[n] = new Pos();
						levelData.boardData[l].pathList[m].v[n].x = int.Parse(jSONNode["boardData"][l]["pathList"][m]["v"][n]["x"]);
						levelData.boardData[l].pathList[m].v[n].y = int.Parse(jSONNode["boardData"][l]["pathList"][m]["v"][n]["y"]);
					}
				}
				if (jSONNode["boardData"][l]["transporterPathList"] != null)
				{
					levelData.boardData[l].transporterPathList = new PlayInfinity.AliceMatch3.Editor.Path[jSONNode["boardData"][l]["transporterPathList"].Count];
					for (int num3 = 0; num3 < jSONNode["boardData"][l]["transporterPathList"].Count; num3++)
					{
						levelData.boardData[l].transporterPathList[num3] = new PlayInfinity.AliceMatch3.Editor.Path();
						levelData.boardData[l].transporterPathList[num3].v = new Pos[jSONNode["boardData"][l]["transporterPathList"][num3]["v"].Count];
						for (int num4 = 0; num4 < jSONNode["boardData"][l]["transporterPathList"][num3]["v"].Count; num4++)
						{
							levelData.boardData[l].transporterPathList[num3].v[num4] = new Pos();
							levelData.boardData[l].transporterPathList[num3].v[num4].x = int.Parse(jSONNode["boardData"][l]["transporterPathList"][num3]["v"][num4]["x"]);
							levelData.boardData[l].transporterPathList[num3].v[num4].y = int.Parse(jSONNode["boardData"][l]["transporterPathList"][num3]["v"][num4]["y"]);
						}
					}
				}
				if (jSONNode["boardData"][l]["catPathList"] != null)
				{
					levelData.boardData[l].catPathList = new PlayInfinity.AliceMatch3.Editor.Path[jSONNode["boardData"][l]["catPathList"].Count];
					for (int num5 = 0; num5 < jSONNode["boardData"][l]["catPathList"].Count; num5++)
					{
						levelData.boardData[l].catPathList[num5] = new PlayInfinity.AliceMatch3.Editor.Path();
						levelData.boardData[l].catPathList[num5].v = new Pos[jSONNode["boardData"][l]["catPathList"][num5]["v"].Count];
						for (int num6 = 0; num6 < jSONNode["boardData"][l]["catPathList"][num5]["v"].Count; num6++)
						{
							levelData.boardData[l].catPathList[num5].v[num6] = new Pos();
							levelData.boardData[l].catPathList[num5].v[num6].x = int.Parse(jSONNode["boardData"][l]["catPathList"][num5]["v"][num6]["x"]);
							levelData.boardData[l].catPathList[num5].v[num6].y = int.Parse(jSONNode["boardData"][l]["catPathList"][num5]["v"][num6]["y"]);
						}
					}
				}
				levelData.boardData[l].transList = new Trans[jSONNode["boardData"][l]["transList"].Count];
				for (int num7 = 0; num7 < jSONNode["boardData"][l]["transList"].Count; num7++)
				{
					levelData.boardData[l].transList[num7] = new Trans();
					levelData.boardData[l].transList[num7].v = new Pos[jSONNode["boardData"][l]["transList"][num7]["v"].Count];
					for (int num8 = 0; num8 < jSONNode["boardData"][l]["transList"][num7]["v"].Count; num8++)
					{
						levelData.boardData[l].transList[num7].v[num8] = new Pos();
						levelData.boardData[l].transList[num7].v[num8].x = int.Parse(jSONNode["boardData"][l]["transList"][num7]["v"][num8]["x"]);
						levelData.boardData[l].transList[num7].v[num8].y = int.Parse(jSONNode["boardData"][l]["transList"][num7]["v"][num8]["y"]);
					}
				}
				levelData.boardData[l].areaList = new Area[jSONNode["boardData"][l]["areaList"].Count];
				for (int num9 = 0; num9 < jSONNode["boardData"][l]["areaList"].Count; num9++)
				{
					levelData.boardData[l].areaList[num9] = new Area();
					levelData.boardData[l].areaList[num9].index = int.Parse(jSONNode["boardData"][l]["areaList"][num9]["index"]);
					levelData.boardData[l].areaList[num9].start = new Pos();
					levelData.boardData[l].areaList[num9].start.x = int.Parse(jSONNode["boardData"][l]["areaList"][num9]["start"]["x"]);
					levelData.boardData[l].areaList[num9].start.y = int.Parse(jSONNode["boardData"][l]["areaList"][num9]["start"]["y"]);
					levelData.boardData[l].areaList[num9].end = new Pos();
					levelData.boardData[l].areaList[num9].end.x = int.Parse(jSONNode["boardData"][l]["areaList"][num9]["end"]["x"]);
					levelData.boardData[l].areaList[num9].end.y = int.Parse(jSONNode["boardData"][l]["areaList"][num9]["end"]["y"]);
				}
				levelData.boardData[l].createrList = new Creater[jSONNode["boardData"][l]["createrList"].Count];
				for (int num10 = 0; num10 < jSONNode["boardData"][l]["createrList"].Count; num10++)
				{
					levelData.boardData[l].createrList[num10] = new Creater();
					levelData.boardData[l].createrList[num10].index = int.Parse(jSONNode["boardData"][l]["createrList"][num10]["index"]);
					levelData.boardData[l].createrList[num10].probability = int.Parse(jSONNode["boardData"][l]["createrList"][num10]["probability"]);
					levelData.boardData[l].createrList[num10].p = new Pos();
					levelData.boardData[l].createrList[num10].p.x = int.Parse(jSONNode["boardData"][l]["createrList"][num10]["p"]["x"]);
					levelData.boardData[l].createrList[num10].p.y = int.Parse(jSONNode["boardData"][l]["createrList"][num10]["p"]["y"]);
				}
				int num11 = num * num2;
				levelData.boardData[l].map = new int[num11];
				for (int num12 = 0; num12 < num11; num12++)
				{
					levelData.boardData[l].map[num12] = int.Parse(jSONNode["boardData"][l]["map"][num12]);
				}
			}
			if (levelData.level <= 100)
			{
				AdjustHard(GlobalVariables.ChallenageNum);
			}
		}

		public void AdjustHard(int ChallenageNum)
		{
			Dictionary<int, int[]> dictionary = null;
			dictionary = ((levelData.level <= 20) ? hardDic1 : ((levelData.level > 40) ? hardDic3 : hardDic2));
			int hard = levelData.hard;
			probabilityList = GlobalVariables.probabilityList;
			int num = 0;
			int[] array = dictionary[hard];
			if (ChallenageNum != array[0] && ChallenageNum != array[1] && ChallenageNum != array[2] && ChallenageNum != array[3])
			{
				return;
			}
			for (int i = 0; i < probabilityList.Length; i++)
			{
				if (!ProcessTool.isCollectFinish(i + 1))
				{
					num++;
					probabilityList[i]++;
				}
			}
			if (num <= AdjustNum)
			{
				List<int> list = new List<int>();
				for (int j = 0; j < probabilityList.Length; j++)
				{
					if (probabilityList[j] > 0 && ProcessTool.isCollectFinish(j + 1))
					{
						list.Add(j);
					}
				}
				list = ProcessTool.ListRandom(list);
				List<int> list2 = new List<int> { list[0] };
				for (int k = 1; k < list.Count; k++)
				{
					if (probabilityList[list[k]] > probabilityList[list2[0]])
					{
						list2.Insert(0, list[k]);
					}
					else
					{
						list2.Add(list[k]);
					}
				}
				for (int l = 0; l < AdjustNum - num; l++)
				{
					probabilityList[list2[l]]++;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			int[] array2 = probabilityList;
			foreach (int num2 in array2)
			{
				stringBuilder.Append(num2 + "  |  ");
			}
			DebugUtils.Log(DebugType.Other, stringBuilder);
		}

		public int[] GetProbabilityList()
		{
			return probabilityList;
		}

		public void ResetGame()
		{
			GameSceneUIManager.Instance.ActiveCoinAndBookGrid(false);
			isGuiding = false;
			BombAutoBomb = false;
			BombToGold = false;
			isFinishing = false;
			for (int i = 0; i < boardArray.Length; i++)
			{
				boardArray[i].Reset();
			}
			currentWhiteCloudNum = 0;
			currentBlackCloudNum = 0;
			isFinish = false;
			TotleMoveCount = 0;
			currentBoard = boardArray[0];
			BeeLevel = GameConfig.BeeLevel;
			CreateAreabomb = GameConfig.CreateOneAreaBoom;
			CreateColorBomb = GameConfig.CreateOneColorBoom;
			GameConfig.BeeLevel = 1;
			GameConfig.CreateOneAreaBoom = false;
			GameConfig.CreateOneColorBoom = false;
			for (int j = 0; j < targetState.Length; j++)
			{
				targetState[j] = 0;
			}
			for (int k = 0; k < levelData.boardData.Length; k++)
			{
				boardArray[k].Create(k);
			}
		}

		public void PlayerControlFinish(Board board)
		{
			if (isFinish && !isFinishStep)
			{
				return;
			}
			canDropJewel = true;
			IsReadyToTrasport = true;
			isFinishStep = false;
			isMoveCountReduce = false;
			IsReadyToFindFish = true;
			bool flag = false;
			if (isDestroyAnyThing)
			{
				if (Instance.currentWhiteCloudNum != 0 && !Instance.isDestroyWhiteCloud)
				{
					flag = true;
					Element whiteCloudTarget = ProcessTool.GetWhiteCloudTarget(board);
					if (whiteCloudTarget != null)
					{
						AudioManager.Instance.PlayAudioEffect("cloud_spread");
						whiteCloudTarget.CreateStandard(41);
					}
				}
				if (Instance.currentBlackCloudNum != 0 && !Instance.isDestroyBlackCloud)
				{
					flag = true;
					List<Element> list = new List<Element>();
					list = ProcessTool.GetBlackCloudTarget(board);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < ((list.Count < 2) ? 1 : 2); i++)
						{
							if (list[i].color == 42)
							{
								AudioManager.Instance.PlayAudioEffect("cloud_spread");
								list[i].CreateStandard(43);
							}
							else
							{
								AudioManager.Instance.PlayAudioEffect("cloud_spread");
								list[i].CreateStandard(42);
							}
						}
					}
				}
				foreach (Cell treasure in ProcessTool.GetTreasures(board))
				{
					if (!treasure.element.isCanClose)
					{
						treasure.element.isCanClose = true;
					}
					else
					{
						treasure.element.Upgrade();
					}
				}
			}
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			bool flag2 = false;
			PingYuType a = PingYuType.None;
			if (RemoveNum >= gameConfig.magnificentRemoveNum)
			{
				flag2 = true;
				a = PingYuType.Magnificent;
			}
			else if (RemoveNum >= gameConfig.terrificRemoveNum)
			{
				flag2 = true;
				a = PingYuType.Terrific;
			}
			else if (RemoveNum >= gameConfig.amazingRemoveNum)
			{
				flag2 = true;
				a = PingYuType.Amazing;
			}
			else if (RemoveNum >= gameConfig.awesomeRemoveNum)
			{
				flag2 = true;
				a = PingYuType.Awesome;
			}
			RemoveNum = 0;
			PingYuType b = PingYuType.None;
			if (matchNum >= gameConfig.magnificentComborNum)
			{
				flag2 = true;
				b = PingYuType.Magnificent;
			}
			else if (matchNum >= gameConfig.terrificComborNum)
			{
				flag2 = true;
				b = PingYuType.Terrific;
			}
			else if (matchNum >= gameConfig.amazingComborNum)
			{
				flag2 = true;
				b = PingYuType.Amazing;
			}
			else if (matchNum >= gameConfig.awesomeComborNum)
			{
				flag2 = true;
				b = PingYuType.Awesome;
			}
			matchNum = 0;
			float time = 0f;
			if (flag2)
			{
				Singleton<MessageDispatcher>.Instance().SendMessage(19u, Mathf.Max((int)a, (int)b));
			}
			else
			{
				time = ((flag && isDestroyAnyThing) ? 0.3f : 0f);
			}
			float currentTime = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (currentTime > time)
				{
					if (currMap < boardArray.Length - 1 && !isMapMoving)
					{
						if (ProcessTool.targetIsZero(currentBoard))
						{
							Singleton<MessageDispatcher>.Instance().SendMessage(8u, new MoveAreaInfo(currentBoard, currMap + 1));
						}
						else if (!IsRefeshElement(currentBoard))
						{
							Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
						}
					}
					else if (board.currArea < board.data.areaList.Length - 1 && !isAreaMoving)
					{
						if (ProcessTool.targetIsZero(currentBoard))
						{
							Singleton<MessageDispatcher>.Instance().SendMessage(7u, new MoveAreaInfo(currentBoard, currentBoard.currArea + 1));
						}
						else if (!IsRefeshElement(currentBoard))
						{
							Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
						}
					}
					else if (!IsRefeshElement(currentBoard))
					{
						Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
					}
					return true;
				}
				currentTime += duration;
				return false;
			}));
			isDestroyWhiteCloud = false;
			isDestroyBlackCloud = false;
			isDestroyAnyThing = false;
		}

		public void DropUsed(DropType type, bool isSuccessful)
		{
			GameSceneUIManager.Instance.DropUsed(type, isSuccessful);
		}

		public void StopTip(uint iMessageType = 0u, object arg = null)
		{
			Board board = (Board)arg;
			List<Cell> list = new List<Cell>();
			if (coroutin != null)
			{
				StopCoroutine(coroutin);
			}
			if (EffectTipTarget != null)
			{
				foreach (Element item in EffectTipTarget.list)
				{
					if (item != null)
					{
						list.Add(board.cells[item.row, item.col]);
					}
				}
				foreach (Element other in EffectTipTarget.otherList)
				{
					if (other != null)
					{
						list.Add(board.cells[other.row, other.col]);
					}
				}
			}
			if (EffectTipBomb != null)
			{
				list.Add(board.cells[EffectTipBomb.row, EffectTipBomb.col]);
			}
			RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.StopTip, list));
		}

		private void OnDestroy()
		{
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(1u, StopTip);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(2u, StopTip);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(3u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(4u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(5u, TryToMoveCamera);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(6u, EnableOrDeEnablePlayerControll);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(7u, MoveArea);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(8u, MoveMap);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(9u, GameStart);
			Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(10u, GameEnd);
		}

		public void TryToMoveCamera(uint iMessageType, object arg)
		{
			Board board = (Board)arg;
			Element element = null;
			Cell[,] cells = board.cells;
			for (int i = 0; i < currentBoard.MaxRow; i++)
			{
				for (int j = 0; j < currentBoard.MaxCol; j++)
				{
					if (element == null && cells[i, j].HaveJewel() && !cells[i, j].element.removed)
					{
						element = cells[i, j].element;
					}
				}
			}
			if (element != null)
			{
				bool num = GetHaveElementAndCellTool.CheckCellInCrrentArea(board, element);
				bool flag = GetHaveElementAndCellTool.CheckCellInNextArea(board, element);
				if (!(!num || flag))
				{
					return;
				}
				for (int num2 = board.data.areaList.Length - 1; num2 >= 0; num2--)
				{
					if (GetHaveElementAndCellTool.CheckCellInCustomArea(board, element, num2))
					{
						Singleton<MessageDispatcher>.Instance().SendMessage(7u, new MoveAreaInfo(board, num2));
						break;
					}
				}
			}
			else if (!ProcessTool.isCollectFinish(22))
			{
				Singleton<MessageDispatcher>.Instance().SendMessage(7u, new MoveAreaInfo(board, 0));
			}
		}

		public void EnableOrDeEnablePlayerControll(uint iMessageType, object arg)
		{
			bool flag = (bool)arg;
			if (!flag || (!isFinish && TotleMoveCount < levelData.move))
			{
				isUserCanPlay = flag;
				StopTip(0u, currentBoard);
				isTipEnable = flag;
			}
		}

		public void MoveArea(uint iMessageType, object arg)
		{
			MoveAreaInfo obj = (MoveAreaInfo)arg;
			int newAreaIndex = obj.newAreaIndex;
			Board board = obj.board;
			board.canCheckAreaChange = true;
			board.currArea = newAreaIndex;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			AreaMoveTool.SetAnchorPoint(board, board.currArea);
		}

		public void MoveMap(uint iMessageType, object arg)
		{
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			MoveAreaInfo moveAreaInfo = (MoveAreaInfo)arg;
			Board board = moveAreaInfo.board;
			int newAreaIndex = moveAreaInfo.newAreaIndex;
			int num = ((moveAreaInfo.oldAreaIndex == 9999) ? Instance.currMap : moveAreaInfo.oldAreaIndex) - newAreaIndex;
			Vector3 targetPos = new Vector3(12f, 0f, 0f) * num + BoardParent.transform.localPosition;
			board.canCheckMapChange = true;
			currMap = newAreaIndex;
			isMapMoving = true;
			currentBoard.canCheckMapChange = false;
			AreaMoveTool.DoMapChange(BoardParent, targetPos, 2f, delegate
			{
				currentBoard = boardArray[currMap];
				Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
				isMapMoving = false;
			});
		}

		public void GameStart(uint iMessageType, object arg)
		{
			GameSceneUIManager.Instance.InitSkillPanel(17u, null);
			List<Combo> list = new List<Combo>();
			if (UserDataManager.Instance.GetIsComboing())
			{
				list = Singleton<PlayGameData>.Instance().ComboReward[GlobalVariables.ComboNum];
			}
			if (CreateAreabomb)
			{
				list.Add(new Combo(ElementType.AreaBomb, 1));
				CreateAreabomb = false;
				boosterUsageData[0] = 1;
			}
			if (CreateColorBomb)
			{
				list.Add(new Combo(ElementType.ColorBomb, 1));
				CreateAreabomb = false;
				boosterUsageData[1] = 1;
			}
			if (BeeLevel == 1 && list.Count == 0)
			{
				StartLogic();
				return;
			}
			float delayMaxTime = 0f;
			if (BeeLevel == 2)
			{
				AudioManager.Instance.PlayAudioEffect("booster3_double_bee");
				yifener.SetActive(true);
				boosterUsageData[2] = 1;
				delayMaxTime = 3f;
			}
			if (delayMaxTime < 1f)
			{
				delayMaxTime = 1f;
			}
			foreach (Combo item in list)
			{
				for (int i = 0; i < item.num; i++)
				{
					currentBoard.RandomOneCellToCreateCustomElement(item.type);
				}
			}
			float delayTime = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (delayTime >= delayMaxTime)
				{
					StartLogic();
					return true;
				}
				delayTime += duration;
				return false;
			}));
		}

		private void StartLogic()
		{
			isGameRunning = true;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
			float loopTime = 50f;
			float CD = 30f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (isFinish)
				{
					return true;
				}
				if (loopTime >= CD)
				{
					loopTime = 0f;
					UpdateManager.Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.TipMoveDirection, currentBoard));
				}
				loopTime += duration;
				return false;
			}));
		}

		public void GameEnd(uint iMessageType, object arg)
		{
			UpdateManager.Instance.PauseGame();
			GameSceneUIManager.Instance.isCanSkipFinish = false;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			SpriteRenderer[] componentsInChildren = currentBoard.GetComponentsInChildren<SpriteRenderer>();
			ParticleSystem[] componentsInChildren2 = currentBoard.GetComponentsInChildren<ParticleSystem>();
			MeshRenderer[] componentsInChildren3 = currentBoard.GetComponentsInChildren<MeshRenderer>();
			GlobalVariables.ChallenageNum = -1;
			GlobalVariables.probabilityList = null;
			if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
			{
				GA.FinishLevel(UserDataManager.Instance.GetService().level.ToString());
				GA.SetUserLevel(UserDataManager.Instance.GetService().level);
			}
			if (componentsInChildren3.Length != 0)
			{
				MeshRenderer[] array = componentsInChildren3;
				foreach (MeshRenderer meshRenderer in array)
				{
					Debug.Log(meshRenderer.transform.name);
					if (meshRenderer.material.HasProperty("_TintColor"))
					{
						Color color = meshRenderer.material.GetColor("_TintColor");
						color.a = 0f;
						meshRenderer.material.DOColor(color, "_TintColor", 0.3f);
					}
					else if (meshRenderer.material.HasProperty("_Color"))
					{
						if (meshRenderer.transform.GetComponent<SkeletonAnimation>() != null)
						{
							meshRenderer.transform.GetComponent<SkeletonAnimation>().valid = false;
						}
						Color color2 = meshRenderer.material.GetColor("_Color");
						color2.a = 0f;
						meshRenderer.material.DOColor(color2, "_Color", 0.3f).OnComplete(delegate
						{
						});
					}
				}
			}
			SpriteRenderer[] array2 = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array2)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.DOFade(0f, 0.5f);
				}
			}
			ParticleSystem[] array3 = componentsInChildren2;
			foreach (ParticleSystem particleSystem in array3)
			{
				if (!(particleSystem != null))
				{
					continue;
				}
				Renderer[] componentsInChildren4 = particleSystem.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren4.Length; j++)
				{
					Material[] materials = componentsInChildren4[j].materials;
					for (int k = 0; k < materials.Length; k++)
					{
						if (materials[k].HasProperty("_Color"))
						{
							Color color3 = materials[k].color;
							color3.a = 0f;
							materials[k].DOColor(color3, 0.3f);
						}
						else if (materials[k].HasProperty("_TintColor"))
						{
							Color color4 = materials[k].GetColor("_TintColor");
							color4.a = 0f;
							materials[k].DOColor(color4, "_TintColor", 0.3f);
						}
					}
				}
			}
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.35f)
				{
					Board[] array4 = boardArray;
					for (int l = 0; l < array4.Length; l++)
					{
						array4[l].gameObject.SetActive(false);
					}
					DialogManagerTemp.Instance.CloseAllDialogs();
					DialogManagerTemp.Instance.ShowDialog(DialogType.GameWinDlg);
					return true;
				}
				time += duration;
				return false;
			}));
		}

		public bool IsRefeshElementByInit(Board board)
		{
			if (isDie(board))
			{
				DebugUtils.Log(DebugType.Other, "死局！");
				board.RefreshBoard();
				int num = 10;
				int num2 = 0;
				while (num2 <= num && isDie(board))
				{
					num2++;
					board.RefreshBoard();
				}
				if (num2 > num)
				{
					DebugUtils.LogError(DebugType.Other, "刷新次数过多，场景中无法形成可消除组合！");
				}
				else
				{
					DebugUtils.Log(DebugType.Other, "刷新成功！");
				}
				return true;
			}
			return false;
		}

		public bool IsRefeshElement(Board board)
		{
			if (isDie(board))
			{
				DebugUtils.Log(DebugType.Other, "死局！");
				Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.RefreshElement, board));
				float time = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time > 0.6f)
					{
						board.RefreshBoard();
						int num = 10;
						int num2 = 0;
						while (num2 <= num && isDie(board))
						{
							num2++;
							board.RefreshBoard();
						}
						float time2 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration2)
						{
							if (time2 > 0.3f)
							{
								Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
								RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.FinishRefreshElement, board));
								return true;
							}
							time2 += duration2;
							return false;
						}));
						if (num2 > num)
						{
							DebugUtils.LogError(DebugType.Other, "刷新次数过多，场景中无法形成可消除组合！");
						}
						else
						{
							DebugUtils.Log(DebugType.Other, "刷新成功！");
						}
						return true;
					}
					time += duration;
					return false;
				}));
				return true;
			}
			return false;
		}

		private bool isDie(Board board)
		{
			Bomb = null;
			tipTarget = null;
			Cell[,] cells = board.cells;
			for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
			{
				for (int num = board.levelColEnd; num >= board.levelColStart; num--)
				{
					Cell cell = cells[i, num];
					Element element = cell.element;
					if (!(element == null) && element.type > (ElementType)0 && !cell.Blocked())
					{
						if (element.IsBomb())
						{
							if (Bomb == null)
							{
								Bomb = element;
							}
							else
							{
								Bomb = ((element.type > Bomb.type) ? element : Bomb);
							}
						}
						new List<Cell>();
						foreach (Cell item in GetHaveElementAndCellTool.GetCellAround4(board, i, num))
						{
							if (item.element != null && item.isTopElementClear() && item.element.type >= ElementType.Standard_0 && item.element.type <= ElementType.ColorBomb)
							{
								Element element2 = cell.element;
								cell.element = item.element;
								cell.element.row = cell.row;
								cell.element.col = cell.col;
								item.element = element2;
								item.element.row = item.row;
								item.element.col = item.col;
								RemoveCheckInfo removeCheckInfo = RemoveMatchTool.RemoveMatchCheck(board, cell.element);
								removeCheckInfo.otherList.Add(element2);
								SetTipTarget(removeCheckInfo);
								element2 = cell.element;
								cell.element = item.element;
								cell.element.row = cell.row;
								cell.element.col = cell.col;
								item.element = element2;
								item.element.row = item.row;
								item.element.col = item.col;
							}
						}
					}
				}
			}
			if (Bomb == null)
			{
				if (tipTarget != null)
				{
					return tipTarget.type == ElementType.None;
				}
				return true;
			}
			return false;
		}

		private void PressEsc(uint iMessageType, object arg)
		{
			if (!DialogManagerTemp.Instance.IsDialogShowing() && !isFinish && isUserCanPlay)
			{
				if (dropType != 0 && !isGuiding)
				{
					DropUsed(Instance.dropType, false);
				}
				else if (dropType == DropType.None && !isGuiding)
				{
					DialogManagerTemp.Instance.ShowDialog(DialogType.QuitLevelDlg);
				}
			}
		}

		public void SettleAccounts(List<Cell> CellList, List<ElementType> TypeList)
		{
			if (continueNum == 0)
			{
				stepCount = Instance.levelData.move - Instance.TotleMoveCount;
			}
			initCoinsNum = UserDataManager.Instance.GetService().coin;
			initScrollNum = UserDataManager.Instance.GetService().scrollNum;
			initBankNum = UserDataManager.Instance.GetBankNum();
			DemoConfig gameConfig = Singleton<PlayGameData>.Instance().gameConfig;
			DebugUtils.Log(DebugType.Other, "Game Win Level " + UserDataManager.Instance.GetProgress());
			int num = ((Instance.levelData.hard == 0) ? gameConfig.normalCoinNum : gameConfig.hardCoinNum);
			num += ProcessTool.GetAllCoin(TypeList);
			num += ProcessTool.GetAllCoin(CellList);
			UserDataManager.Instance.GetService().scrollNum = initScrollNum + ((Instance.levelData.hard == 2) ? gameConfig.hardBookNum : gameConfig.normalBookNum);
			UserDataManager.Instance.GetService().coin = initCoinsNum + num;
			UserDataManager.Instance.SetBankNum(initBankNum + num * Singleton<PlayGameData>.Instance().gameConfig.BankSaleMultiple);
			if (!GlobalVariables.UseAddStep)
			{
				StartAnalyticsIEnumerator(1);
			}
			if (UserDataManager.Instance.GetIsComboing())
			{
				GlobalVariables.ComboNum++;
			}
			UserDataManager.Instance.IncreaseLevel();
			if (!UserDataManager.Instance.GetService().unlimitedLife)
			{
				LifeManager.Instance.AddUserLife(1);
			}
			if (UserDataManager.Instance.GetService().tutorialProgress == 2)
			{
				UserDataManager.Instance.GetService().tutorialProgress = 3;
			}
			UserDataManager.Instance.Save();
			GlobalVariables.LevelFirstPass = true;
		}

		public void StartAnalyticsIEnumerator(int finishIndex)
		{
			StartCoroutine(SendAnalyticsData(finishIndex));
		}

		private IEnumerator SendAnalyticsData(int finishIndex)
		{
			if (RegionInfo.CurrentRegion.EnglishName == "China")
			{
				yield break;
			}
			DebugUtils.Log(DebugType.Other, "regionInfo.Name" + base.name);
			string text = "Android";
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (finishIndex == 1 && !GlobalVariables.UseAddStep)
			{
				num2 = 1;
			}
			else if (finishIndex == 2 || (finishIndex == 1 && GlobalVariables.UseAddStep))
			{
				num3 = 1;
			}
			else if (finishIndex == 3)
			{
				num = 1;
			}
			if (UserDataManager.Instance.GetCoin() > 5000000 || UserDataManager.Instance.GetCoin() < 0 || UserDataManager.Instance.GetScrollNum() > 1000 || UserDataManager.Instance.GetScrollNum() < 0)
			{
				yield break;
			}
			string text2 = "";
			string text3 = "";
			for (int i = 0; i < boosterUsageData.Length; i++)
			{
				text2 += boosterUsageData[i];
				text3 += UserDataManager.Instance.GetService().boosterPurchaseData[i];
				boosterUsageData[i] = 0;
				UserDataManager.Instance.GetService().boosterPurchaseData[i] = 0;
			}
			UserDataManager.Instance.Save();
			int num4 = (int)(DateTime.Now.Ticks / 10000000 - GlobalVariables.LevelStartTime);
			if (num4 > 108000)
			{
				yield break;
			}
			string[] array = Application.version.ToString().Split('.');
			string text4 = "";
			bool flag = true;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != "" && array[j] != "0")
				{
					flag = false;
					text4 += array[j];
				}
				else if (array[j] != "" && array[j] == "0" && !flag)
				{
					text4 += array[j];
				}
			}
			string analyticsDataAddress = NetworkConfig.AnalyticsDataAddress;
			string sendString = "CastleStoryAnalytics" + text + "," + UserDataManager.Instance.GetLevel() + "," + text4 + "," + UserDataManager.Instance.GetCoin() + "," + UserDataManager.Instance.GetScrollNum() + "," + num4 + ",1," + num + "," + num2 + "," + num3 + "," + text2 + "," + text3 + "," + target[0] + "," + target[1] + "," + target[2] + "," + target[3] + "," + stepCount;
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", sendString);
			UnityWebRequest www = UnityWebRequest.Post(analyticsDataAddress, wWWForm);
			yield return www.Send();
			if (!string.IsNullOrEmpty(www.error))
			{
				DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics|    ");
				SendAnalyticsDataFailSave(sendString);
			}
			else
			{
				DebugUtils.Log(DebugType.Other, www.downloadHandler.text);
				if (www.downloadHandler.text != "success")
				{
					SendAnalyticsDataFailSave(sendString);
				}
			}
			DebugUtils.Log(DebugType.Other, "SendAnalyticsData success.");
			if (www.GetResponseHeader("response") != null)
			{
				DebugUtils.Log(DebugType.Other, www.GetResponseHeader("response"));
			}
		}

		public void SendAnalyticsDataFailSave(string savaData)
		{
			string text = "";
			if (File.Exists(Application.persistentDataPath + "/levelData.txt"))
			{
				text = File.ReadAllText(Application.persistentDataPath + "/levelData.txt");
			}
			if (text != null && text != "" && text != " ")
			{
				File.WriteAllText(Application.persistentDataPath + "/levelData.txt", text + "|" + savaData);
			}
			else
			{
				File.WriteAllText(Application.persistentDataPath + "/levelData.txt", savaData);
			}
		}

		public void StartFindFish(Board board)
		{
			new List<Cell>();
			foreach (KeyValuePair<int, Element> item in board.FishCellDic)
			{
				if (item.Value != null && item.Value.FindFishCount > 0)
				{
					item.Value.transform.DOGameTweenScale(Vector3.one, 0.2f);
					MoveFinsh(board, item.Value);
				}
			}
		}

		public void MoveFinsh(Board board, Element fish)
		{
			float duration2 = 0.2f;
			fish.FindFishCount--;
			Cell fishCell = board.cells[fish.row, fish.col];
			Cell nextCell = fishCell.CatNextCell;
			Element nextElement = nextCell.element;
			if (!nextCell.isTopElementClear())
			{
				DealElementTool.RemoveElement(board, nextCell);
				float time2 = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time2 > 0.1f)
					{
						if (fish.FindFishCount > 0)
						{
							MoveFinsh(board, fish);
						}
						return true;
					}
					time2 += duration;
					return false;
				}));
			}
			else
			{
				if (nextElement != null && !nextCell.isCat())
				{
					nextElement.transform.DOGameTweenMove(fishCell.transform.position, duration2).OnComplete(delegate
					{
						fishCell.element = nextElement;
						nextElement.row = fishCell.row;
						nextElement.col = fishCell.col;
						MovedFishCell.Add(fishCell, false);
					});
				}
				fish.transform.DOGameTweenMove(nextCell.transform.position, duration2).OnComplete(delegate
				{
					if (nextCell.isCat())
					{
						ProcessTool.ProcessCat(board, nextCell);
						UnityEngine.Object.Destroy(fish.gameObject);
						fishCell.element = null;
					}
					else
					{
						if (nextCell.bottomElement != null)
						{
							UnityEngine.Object.Destroy(nextCell.bottomElement.gameObject);
						}
						nextCell.element = fish;
						fish.row = nextCell.row;
						fish.col = nextCell.col;
						if (nextElement == null)
						{
							MovedFishCell.Add(fishCell, true);
						}
						if (fish.FindFishCount > 0)
						{
							MoveFinsh(board, fish);
						}
					}
				});
			}
			if (fish.FindFishCount != 0)
			{
				return;
			}
			float time = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time > 0.23f)
				{
					foreach (KeyValuePair<Cell, bool> item in MovedFishCell)
					{
						if (item.Value)
						{
							item.Key.element = null;
						}
						else if (RemoveMatchTool.RemoveMatch(board, item.Key.element) == ElementType.None)
						{
							DropAndMoveTool.CheckDrop(board, item.Key);
						}
					}
					MovedFishCell.Clear();
					return true;
				}
				time += duration;
				return false;
			}));
		}

		public void StartTransporter(Board board)
		{
			AudioManager.Instance.PlayAudioEffect("CarpetMove", 0.1f);
			board.movingCount++;
			float speed = 0.36f;
			Dictionary<int, List<Cell>> dic = board.transporterCellDic;
			foreach (KeyValuePair<int, List<Cell>> item in dic)
			{
				foreach (Cell item2 in item.Value)
				{
					if (item2.empty)
					{
						continue;
					}
					Cell cell = item2;
					Cell next = cell.TransNextCell;
					Element elem = cell.element;
					Element top = cell.topElement;
					if (next.empty)
					{
						Cell ne = next;
						while (ne.TransNextCell.empty)
						{
							ne = ne.TransNextCell;
						}
						if (top != null)
						{
							GameObject temp = UnityEngine.Object.Instantiate(top.gameObject, top.transform.position, top.transform.localRotation);
							temp.transform.DOGameTweenMove(next.transform.position, speed).OnComplete(delegate
							{
								UnityEngine.Object.Destroy(temp);
							});
							top.transform.position = ne.transform.position;
							top.transform.DOGameTweenMove(ne.TransNextCell.transform.position, speed).OnComplete(delegate
							{
								top.row = ne.TransNextCell.row;
								top.col = ne.TransNextCell.col;
								ne.TransNextCell.topElement = top;
							});
						}
						else
						{
							float time2 = 0f;
							UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
							{
								if (time2 > speed)
								{
									ne.TransNextCell.topElement = null;
									return true;
								}
								time2 += duration;
								return false;
							}));
						}
						if (elem != null)
						{
							GameObject temp2 = UnityEngine.Object.Instantiate(elem.gameObject, elem.transform.position, elem.transform.localRotation);
							temp2.transform.DOGameTweenMove(next.transform.position, speed).OnComplete(delegate
							{
								UnityEngine.Object.Destroy(temp2);
							});
							elem.transform.position = ne.transform.position;
							elem.transform.DOGameTweenMove(ne.TransNextCell.transform.position, speed).OnComplete(delegate
							{
								elem.row = ne.TransNextCell.row;
								elem.col = ne.TransNextCell.col;
								ne.TransNextCell.element = elem;
							});
							continue;
						}
						float time3 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time3 > speed)
							{
								ne.TransNextCell.element = null;
								return true;
							}
							time3 += duration;
							return false;
						}));
						continue;
					}
					if (top != null)
					{
						top.transform.DOGameTweenMove(next.transform.position, speed).OnComplete(delegate
						{
							top.row = next.row;
							top.col = next.col;
							next.topElement = top;
						});
					}
					else
					{
						float time4 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time4 > speed)
							{
								next.topElement = null;
								return true;
							}
							time4 += duration;
							return false;
						}));
					}
					if (elem != null)
					{
						elem.transform.DOGameTweenMove(next.transform.position, speed).OnComplete(delegate
						{
							elem.row = next.row;
							elem.col = next.col;
							next.element = elem;
						});
						continue;
					}
					float time = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time > speed)
						{
							next.element = null;
							return true;
						}
						time += duration;
						return false;
					}));
				}
			}
			Vector2 v2 = Vector2.zero;
			float time5 = 0f;
			UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time5 > speed + 0.05f)
				{
					foreach (KeyValuePair<int, List<Cell>> item3 in dic)
					{
						foreach (Cell item4 in item3.Value)
						{
							if (!item4.Blocked() && item4.element != null && RemoveMatchTool.RemoveMatch(board, item4.element) == ElementType.None)
							{
								DropAndMoveTool.CheckDrop(board, item4);
							}
						}
					}
					board.movingCount--;
					return true;
				}
				if (time5 < speed - 0.05f)
				{
					foreach (Material transMat in board.transMatList)
					{
						transMat.SetTextureOffset("_MainTex", v2);
					}
				}
				v2.x += duration;
				time5 += duration;
				return false;
			}));
		}
	}
}
