using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	private static UpdateManager _instance;

	private List<LogicMessageEvent> logicMessageList;

	private List<LogicMessageEvent> currentlogicMessageList;

	private List<IUpdate> UpdateList;

	private List<IUpdate> NormalUpdateList;

	private bool _isCanUpdateInGameScene = true;

	private bool isCanRenderUpdateInGameScene = true;

	private List<IUpdate> logicUpdateList = new List<IUpdate>();

	private List<Tweener> logicTween = new List<Tweener>();

	private List<Sequence> logicSequence = new List<Sequence>();

	private List<IUpdate> normalLogicUpdateList = new List<IUpdate>();

	private bool isP;

	private List<Cell> NextCell = new List<Cell>();

	public bool isPause;

	public static UpdateManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool isCanUpdateInGameScene
	{
		get
		{
			return _isCanUpdateInGameScene;
		}
		set
		{
			_isCanUpdateInGameScene = value;
		}
	}

	private void Awake()
	{
		_instance = this;
		logicMessageList = new List<LogicMessageEvent>();
		UpdateList = new List<IUpdate>();
		NormalUpdateList = new List<IUpdate>();
		currentlogicMessageList = new List<LogicMessageEvent>();
	}

	private void GameLoopUpdate()
	{
		if (UpdateList == null)
		{
			return;
		}
		logicUpdateList.AddRange(UpdateList);
		foreach (IUpdate logicUpdate in logicUpdateList)
		{
			logicUpdate.ToUpdate(Time.deltaTime);
		}
		for (int num = logicUpdateList.Count - 1; num >= 0; num--)
		{
			if (logicUpdateList[num].IsFinish())
			{
				logicUpdateList[num].Finish();
				logicUpdateList.RemoveAt(num);
				UpdateList.RemoveAt(num);
			}
		}
		logicUpdateList.Clear();
	}

	private void NormalLoopUpdate()
	{
		if (NormalUpdateList == null || NormalUpdateList.Count <= 0)
		{
			return;
		}
		normalLogicUpdateList.Clear();
		normalLogicUpdateList.AddRange(NormalUpdateList);
		foreach (IUpdate normalLogicUpdate in normalLogicUpdateList)
		{
			normalLogicUpdate.ToUpdate(Time.deltaTime);
		}
		for (int num = normalLogicUpdateList.Count - 1; num >= 0; num--)
		{
			if (normalLogicUpdateList[num].IsFinish())
			{
				normalLogicUpdateList[num].Finish();
				try
				{
					normalLogicUpdateList.RemoveAt(num);
					NormalUpdateList.RemoveAt(num);
				}
				catch (Exception ex)
				{
					DebugUtils.LogError(DebugType.Other, string.Concat(ex, "\nnormalLogicUpdateList : ", normalLogicUpdateList.Count, "\nNormalUpdateList : ", NormalUpdateList.Count, "\nindex is : ", num));
					throw;
				}
			}
		}
	}

	private void logicMessageUpdate()
	{
		if (logicMessageList == null)
		{
			return;
		}
		currentlogicMessageList.AddRange(logicMessageList);
		ClearLogicList();
		foreach (LogicMessageEvent currentlogicMessage in currentlogicMessageList)
		{
			if (currentlogicMessage.type == LogicMessageType.SwitchElement)
			{
				SwitchMessage switchMessage = (SwitchMessage)currentlogicMessage.message;
				Element elem1 = switchMessage.element1;
				Element elem2 = switchMessage.element2;
				Board board = switchMessage.board;
				bool checkMatch = switchMessage.checkMatch;
				Direction dir = switchMessage.dir;
				Singleton<MessageDispatcher>.Instance().SendMessage(1u, board);
				if (GameLogic.Instance.dropType == DropType.Glove)
				{
					RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.GloveMove, new GloveMove(elem1, elem2)));
					float time = 0f;
					float midTime = 0.5f;
					float MaxTime = 1.2f;
					bool isFirst = true;
					GameLogic.Instance.DropUsed(GameLogic.Instance.dropType, true);
					GameLogic.Instance.currentBoard.movingCount++;
					Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (isFirst && time >= midTime)
						{
							AudioManager.Instance.PlayAudioEffect("booster6_gloves");
							isFirst = false;
							if (!DealElementTool.SwitchElement(board, elem1, elem2, checkMatch, true))
							{
								elem1.MoveFail(dir);
							}
						}
						else if (time >= MaxTime)
						{
							GameLogic.Instance.currentBoard.movingCount--;
							return true;
						}
						time += duration;
						return false;
					}));
				}
				else if (!DealElementTool.SwitchElement(board, elem1, elem2, checkMatch))
				{
					elem1.MoveFail(dir);
				}
			}
			else if (currentlogicMessage.type == LogicMessageType.DoAreaChange)
			{
				Board board2 = (Board)currentlogicMessage.message;
				if (board2.canCheckAreaChange)
				{
					return;
				}
				if (ProcessTool.targetIsZero(board2))
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(7u, new MoveAreaInfo(board2, board2.currArea + 1));
				}
				else
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
				}
			}
			else if (currentlogicMessage.type == LogicMessageType.ShowAllArea)
			{
				Board board3 = (Board)currentlogicMessage.message;
				int num = board3.AreaPosDic.Count - 1;
				Vector3[] array = new Vector3[num + 1];
				for (int i = 0; i <= num; i++)
				{
					array[i] = board3.AreaPosDic[num - i];
				}
				board3.container.transform.DOGameLocalPath(array, (board3.AreaPosDic.Count - 1) * 2, PathType.CatmullRom);
			}
			else if (currentlogicMessage.type == LogicMessageType.ShowAllMap)
			{
				Board board5 = (Board)currentlogicMessage.message;
				int num2 = GameLogic.Instance.levelData.boardData.Length - 1;
				Vector3 endValue = new Vector3(12f, 0f, 0f) * num2 + GameLogic.Instance.BoardParent.transform.localPosition;
				GameLogic.Instance.BoardParent.transform.DOGameTweenLocalMove(endValue, 2f);
			}
			else if (currentlogicMessage.type == LogicMessageType.DoMapChange)
			{
				if (GameLogic.Instance.currentBoard.canCheckMapChange)
				{
					return;
				}
				if (ProcessTool.targetIsZero(GameLogic.Instance.currentBoard))
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(8u, new MoveAreaInfo(GameLogic.Instance.currentBoard, GameLogic.Instance.currMap + 1));
				}
				else
				{
					Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
				}
			}
			else if (currentlogicMessage.type == LogicMessageType.RemoveElementList)
			{
				RemoveElementListMessage removeMessage = (RemoveElementListMessage)currentlogicMessage.message;
				Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					StartCoroutine("DealElment", removeMessage);
					return true;
				}));
			}
			else if (currentlogicMessage.type == LogicMessageType.RemoveElementListToCreateBomb)
			{
				RemoveElementListToBombMessage removeMessage2 = (RemoveElementListToBombMessage)currentlogicMessage.message;
				Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					StartCoroutine("DealElmentToCreateBomb", removeMessage2);
					return true;
				}));
			}
			else if (currentlogicMessage.type == LogicMessageType.CreateBombWhenFinish)
			{
				CreateBombWhenFinish cbwf = (CreateBombWhenFinish)currentlogicMessage.message;
				Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					StartCoroutine("CreateBombWhenFinish", cbwf);
					return true;
				}));
			}
			else if (currentlogicMessage.type == LogicMessageType.GoldFlyToTarget)
			{
				GoldFlyToTargetMessage goldFlyToTargetMessage = (GoldFlyToTargetMessage)currentlogicMessage.message;
				Instance.AddUpdateToManager(new ActionUpdate(delegate
				{
					StartCoroutine("CreateGold", goldFlyToTargetMessage);
					return true;
				}));
			}
			else if (currentlogicMessage.type == LogicMessageType.ButterFlyToTarget)
			{
				ButterFlyToTargetMessage butterFlyToTargetMessage = (ButterFlyToTargetMessage)currentlogicMessage.message;
				Transform element = butterFlyToTargetMessage.element;
				Board board4 = butterFlyToTargetMessage.board;
				Vector3 endPos = butterFlyToTargetMessage.endPos;
				endPos.z = 0f;
				Board butterFlyboard = butterFlyToTargetMessage.board;
				int flag = butterFlyToTargetMessage.flag;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.ButterFly));
				Transform effect = PoolManager.Ins.SpawnEffect(23, board4.container.transform).transform;
				effect.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				effect.transform.position = element.transform.position;
				AudioManager.Instance.PlayAudioEffect("collect_butterfly");
				UnityEngine.Object.Destroy(element.gameObject);
				GameLogic.Instance.isFinish = ProcessTool.isFinishCheck(board4, flag);
				AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
				{
					Vector2 vector = new Vector2(endPos.x, endPos.y);
					Vector2 vector2 = new Vector2(effect.position.x, effect.position.y);
					Vector3 vector3 = (vector - vector2).normalized;
					vector3.z = 0f;
					float num3 = -340f;
					if (Vector3.Angle(vector3, effect.up) > 8f)
					{
						effect.Rotate(0f, 0f, num3 * deltaTime);
					}
					else
					{
						effect.up = vector3;
					}
					effect.transform.position += effect.up * 8f * deltaTime;
					if (Vector3.Distance(effect.position, endPos) < 0.2f)
					{
						PoolManager.Ins.DeSpawnEffect(effect.gameObject);
						ProcessTool.Statements(butterFlyboard, flag);
						return true;
					}
					return false;
				}));
			}
			else
			{
				if (currentlogicMessage.type == LogicMessageType.TipMoveDirection)
				{
					continue;
				}
				if (currentlogicMessage.type == LogicMessageType.UnLock)
				{
					UnLock unLock = (UnLock)currentlogicMessage.message;
					UserDataManager.Instance.UnlockDrop(unLock.type, unLock.isUnLock);
				}
				else if (currentlogicMessage.type == LogicMessageType.Match)
				{
					Cell cell = (Cell)currentlogicMessage.message;
					GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(cell.board, cell.element);
					if (GameLogic.Instance.RemoveTrue == ElementType.None)
					{
						cell.element.transform.DOGameLocalJump(ProcessTool.GetPosition(cell.board, cell.row, cell.col), -0.06f, 1, 0.3f);
					}
				}
				else if (currentlogicMessage.type == LogicMessageType.ProcessDoubleClick)
				{
					((ProcessDoubleClick)currentlogicMessage.message).elem.ProcessDoubleClick();
				}
			}
		}
		currentlogicMessageList.Clear();
	}

	private void CheckSequenceList()
	{
		for (int num = logicSequence.Count - 1; num >= 0; num--)
		{
			Sequence t = logicSequence[num];
			if (t.IsComplete() || !t.IsActive() || !t.IsPlaying())
			{
				logicSequence.RemoveAt(num);
			}
		}
	}

	private void CheckTweenList()
	{
		for (int num = logicTween.Count - 1; num >= 0; num--)
		{
			Tween t = logicTween[num];
			if (t.IsComplete() || !t.IsActive() || !t.IsPlaying())
			{
				logicTween.RemoveAt(num);
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (isP)
			{
				PauseGame();
			}
			else
			{
				PlayGame();
			}
			isP = !isP;
		}
		if (isCanRenderUpdateInGameScene && RenderManager.Instance != null)
		{
			RenderManager.Instance.RenderUpdate();
		}
		if (isCanUpdateInGameScene)
		{
			GameLoopUpdate();
			logicMessageUpdate();
			CheckSequenceList();
			CheckTweenList();
		}
		NormalLoopUpdate();
	}

	private IEnumerator ShowMoveDirection(Board board)
	{
		foreach (Cell item in board.HeadCellCollect)
		{
			if (item.col >= board.levelColStart && item.col <= board.levelColEnd && item.row <= board.levelRowEnd && item.row >= board.levelRowStart)
			{
				NextCell.Add(item);
			}
		}
		bool isCompleteTip = false;
		while (!isCompleteTip)
		{
			isCompleteTip = true;
			for (int i = 0; i < NextCell.Count; i++)
			{
				Cell cell = NextCell[i];
				if (!(cell == null))
				{
					isCompleteTip = false;
					cell.DirTip.DOGameTweenFade(1f, 0.15f).SetLoops(2, LoopType.Yoyo);
					NextCell[i] = cell.NextCell;
				}
			}
			float delay = 0f;
			Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (delay > 0.1f)
				{
					return true;
				}
				delay += duration;
				return false;
			}));
			yield return new WaitUntil(() => delay > 0.09f);
		}
		NextCell.Clear();
	}

	private IEnumerator CreateGold(GoldFlyToTargetMessage goldFlyToTargetMessage)
	{
		Vector3 startPos = goldFlyToTargetMessage.startPos;
		float timeGold = goldFlyToTargetMessage.time;
		Vector3 endPostimeGold = goldFlyToTargetMessage.endPos;
		endPostimeGold.z = 0f;
		GoldFlyToTargetMessage goldFlyToTargetMessage2 = goldFlyToTargetMessage;
		int goldNum = goldFlyToTargetMessage.goldNum;
		int stepNum = Singleton<PlayGameData>.Instance().gameConfig.OneCoinNum;
		int maxCount = Mathf.CeilToInt((float)goldNum / (float)stepNum);
		for (int i = 0; i < maxCount; i++)
		{
			GlobalVariables.FlyingGoldCoinCount++;
			Transform effectGold = PoolManager.Ins.SpawnEffect(50000009, goldFlyToTargetMessage.parent).transform;
			AudioManager.Instance.PlayAudioEffect("booster_to_coins");
			AudioManager.Instance.PlayAudioEffect("coin_fly");
			effectGold.position = startPos;
			float time = 0f;
			AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
			{
				effectGold.position = Vector3.Slerp(startPos, goldFlyToTargetMessage.endPos, time);
				if (Vector3.Distance(effectGold.position, endPostimeGold) < 0.2f)
				{
					AudioManager.Instance.PlayAudioEffect("coin_collect");
					PoolManager.Ins.DeSpawnEffect(effectGold.gameObject);
					GameSceneUIManager.Instance.UpdateCoinNumInGameScene((goldNum - stepNum >= 0) ? stepNum : goldNum);
					goldNum -= stepNum;
					GlobalVariables.FlyingGoldCoinCount--;
					return true;
				}
				time += deltaTime / timeGold;
				return false;
			}));
			float delayTime = 0f;
			AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (delayTime > 0.1f)
				{
					return true;
				}
				delayTime += duration;
				return false;
			}));
			yield return new WaitUntil(() => delayTime > 0.1f);
		}
	}

	private IEnumerator DealElmentToCreateBomb(RemoveElementListToBombMessage message)
	{
		List<ElementRemoveInfo> list = message.list;
		RemoveElementListToBombMessage removeElementListToBombMessage = message;
		Board board = message.board;
		int color = message.color;
		float flyTime = Singleton<PlayGameData>.Instance().gameConfig.CreateBombSpeed;
		foreach (ElementRemoveInfo item in list)
		{
			if (item.cell.isTopElementClear() && item.cell.element != null && item.cell.element.IsStandard())
			{
				item.cell.element.moving = true;
			}
		}
		ElementRemoveInfo BombInfo = null;
		Element Bomb = null;
		AudioManager.Instance.PlayAudioEffect("elements_match");
		foreach (ElementRemoveInfo item2 in list)
		{
			Cell cell = item2.cell;
			if (item2.ChangeToBomb != ElementType.None)
			{
				BombInfo = item2;
				Bomb = item2.cell.element;
				continue;
			}
			if (cell.element != null)
			{
				cell.element.moving = false;
			}
			DealElementTool.RemoveElement(board, cell, item2.force, item2.showAnim, item2.grassFlag, flyTime, item2.bombInfo, item2.ChangeToBomb);
		}
		float delayTime2 = 0f;
		Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (delayTime2 > flyTime - 0.03f)
			{
				return true;
			}
			delayTime2 += duration;
			return false;
		}));
		yield return new WaitUntil(() => delayTime2 > flyTime - 0.03f);
		if (BombInfo.cell.element != null)
		{
			BombInfo.cell.element.moving = false;
			BombInfo.cell.element.isReadyToBomb = false;
		}
		DealElementTool.RemoveElement(board, BombInfo.cell, BombInfo.force, BombInfo.showAnim, BombInfo.grassFlag, BombInfo.delay, BombInfo.bombInfo, BombInfo.ChangeToBomb);
		for (int i = message.count; i > 0; i--)
		{
			if (!ProcessTool.isCollectFinish(color))
			{
				ProcessTool.UpdateTargetState(board, color, i, Bomb);
			}
			float delayTime3 = 0f;
			Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (delayTime3 > 0.08f)
				{
					return true;
				}
				delayTime3 += duration;
				return false;
			}));
			yield return new WaitUntil(() => delayTime3 > 0.08f);
			delayTime3 = 0f;
		}
	}

	private IEnumerator DealElment(RemoveElementListMessage message)
	{
		List<ElementRemoveInfo> list = ProcessTool.DelReapet(message.list);
		float delayTime = message.delayTime;
		Board board = message.board;
		Action action = message.action;
		float dropdelayTime = ((message.removeFromType != ElementType.VH3Bomb) ? Singleton<PlayGameData>.Instance().gameConfig.DropdelayTime : Singleton<PlayGameData>.Instance().gameConfig.BombDropdelayTime);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			ElementRemoveInfo elementRemoveInfo = list[num];
			if (elementRemoveInfo.cell.isTopElementClear() && elementRemoveInfo.cell.element != null && (elementRemoveInfo.cell.element.IsStandard() || elementRemoveInfo.cell.element.IsTreasure()))
			{
				if (elementRemoveInfo.cell.element.removed)
				{
					elementRemoveInfo.cell.element.moving = false;
					list.RemoveAt(num);
				}
				else if (elementRemoveInfo.cell.element.isReadyToBomb)
				{
					list.RemoveAt(num);
				}
				else
				{
					elementRemoveInfo.cell.element.moving = true;
				}
			}
		}
		float delayStartTime = 0f;
		Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (delayStartTime > message.delayStartTime)
			{
				return true;
			}
			delayStartTime += duration;
			return false;
		}));
		yield return new WaitUntil(() => delayStartTime > message.delayStartTime);
		AudioManager.Instance.PlayAudioEffect("elements_match");
		int layer = list.Count + 2;
		foreach (ElementRemoveInfo item in list)
		{
			Cell cell = item.cell;
			if (cell.element != null && !ProcessTool.isCollectFinish((int)cell.element.type) && cell.isTopElementClear())
			{
				layer--;
				if (cell.element.IsJewel() && !cell.element.moving)
				{
					ProcessTool.ProcessJewel(board, cell);
				}
				else if (!cell.element.removed && cell.element.IsStandard())
				{
					ProcessTool.UpdateTargetState(board, (int)cell.element.type, layer, cell.element);
				}
			}
			if (cell.element != null)
			{
				cell.element.moving = false;
			}
			DealElementTool.RemoveElement(board, cell, item.force, item.showAnim, item.grassFlag, dropdelayTime, item.bombInfo, item.ChangeToBomb, item.RemoveFrom);
			if (!(delayTime > 0f))
			{
				continue;
			}
			float delayTime2 = 0f;
			Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (delayTime2 > delayTime)
				{
					return true;
				}
				delayTime2 += duration;
				return false;
			}));
			yield return new WaitUntil(() => delayTime2 > delayTime);
		}
		list.Clear();
		float delayTime3 = 0f;
		Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (delayTime3 > 1f)
			{
				return true;
			}
			delayTime3 += duration;
			return false;
		}));
		yield return new WaitUntil(() => delayTime3 > 1f);
		if (action != null)
		{
			action();
		}
	}

	private IEnumerator CreateBombWhenFinish(CreateBombWhenFinish cbwf)
	{
		int arrivelNum = 0;
		Board board = GameLogic.Instance.currentBoard;
		List<Cell> list = cbwf.bombList;
		if (list.Count > 0)
		{
			int index = -1;
			foreach (Cell cell in list)
			{
				int num = index;
				index = num + 1;
				float waitfor = 0f;
				Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (waitfor > 0.3f)
					{
						return true;
					}
					waitfor += duration;
					return false;
				}));
				yield return new WaitUntil(() => waitfor > 0.3f);
				GameLogic.Instance.TotleMoveCount++;
				Vector3 position = GameSceneUIManager.Instance.moveText.transform.position;
				Vector3 position2 = cell.transform.position;
				position.z = 0f;
				position2.z = 0f;
				float time = Vector3.Distance(position, position2) / Singleton<PlayGameData>.Instance().gameConfig.StepToBoomSpeed;
				RenderManager.Instance.AddMessageToRenderList(new RenderMessageEvent(RenderMessageType.StepToBomb, new StepToBomb(position, position2, time, board.container.transform)));
				float time2 = 0f;
				Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time2 > time)
					{
						int num2 = arrivelNum;
						arrivelNum = num2 + 1;
						ElementType changeToBomb = cbwf.bombType[index];
						DealElementTool.RemoveElement(board, cell, false, true, false, Singleton<PlayGameData>.Instance().gameConfig.DropdelayTime, null, changeToBomb);
						return true;
					}
					time2 += duration;
					return false;
				}));
			}
			yield return new WaitUntil(() => arrivelNum == list.Count);
			GameLogic.Instance.BombAutoBomb = true;
			List<ElementRemoveInfo> list2 = new List<ElementRemoveInfo>();
			foreach (Cell bombCell in GetHaveElementAndCellTool.GetBombCellList(board))
			{
				if (bombCell.element != null)
				{
					list2.Add(new ElementRemoveInfo(bombCell, false, true, false, Singleton<PlayGameData>.Instance().gameConfig.DropdelayTime, null, ElementType.None, ElementType.ColorBomb));
				}
			}
			Action action = delegate
			{
				float maxTime = 0.5f;
				float currentTime = 0f;
				Instance.AddUpdateToManager(new ActionUpdate(delegate(float deltaTime)
				{
					if (board.movingCount == 0 && GlobalVariables.FlyingGoldCoinCount == 0)
					{
						currentTime += deltaTime;
						if (currentTime >= maxTime)
						{
							ProcessTool.GameEndUIDeal(board, delegate
							{
								GameLogic.Instance.BombToGold = false;
								GameLogic.Instance.BombAutoBomb = false;
								Singleton<MessageDispatcher>.Instance().SendMessage(10u, null);
							}, 0.5f);
							return true;
						}
						return false;
					}
					currentTime = 0f;
					return false;
				}));
			};
			Instance.AddMessageToLogicUpdate(new LogicMessageEvent(LogicMessageType.RemoveElementList, new RemoveElementListMessage(list2, 0.07f, board, 0f, action)));
		}
		else
		{
			ProcessTool.GameEndUIDeal(board, delegate
			{
				GameLogic.Instance.BombToGold = false;
				GameLogic.Instance.BombAutoBomb = false;
				Singleton<MessageDispatcher>.Instance().SendMessage(10u, null);
			}, 0f);
		}
	}

	private void ClearLogicList()
	{
		if (logicMessageList.Count != 0)
		{
			logicMessageList.Clear();
		}
	}

	public void AddMessageToLogicUpdate(LogicMessageEvent message)
	{
		logicMessageList.Add(message);
	}

	public void AddUpdateToManager(IUpdate update)
	{
		UpdateList.Add(update);
	}

	public void AddNormalUpdateToManager(IUpdate update)
	{
		NormalUpdateList.Add(update);
	}

	public void LogicUpdatePause()
	{
		isCanUpdateInGameScene = false;
	}

	public void LogicUpdatePlay()
	{
		isCanUpdateInGameScene = true;
	}

	public void RenderUpdatePause()
	{
		isCanRenderUpdateInGameScene = false;
	}

	public void RenderUpdatePlay()
	{
		isCanRenderUpdateInGameScene = true;
	}

	public void ClearLogicUpdata()
	{
		logicUpdateList.Clear();
		UpdateList.Clear();
	}

	public void ClearRenderUpdate()
	{
		if (RenderManager.Instance != null)
		{
			RenderManager.Instance.ClearData();
		}
	}

	public void LogicTweenPause()
	{
		foreach (Tweener item in logicTween)
		{
			if (item != null)
			{
				item.Pause();
			}
		}
	}

	public void LogicTweenPlay()
	{
		foreach (Tweener item in logicTween)
		{
			if (item != null)
			{
				item.Play();
			}
		}
	}

	public void ClearTween()
	{
		logicTween.Clear();
	}

	public void AddTweenToList(Tweener tweener)
	{
		if (!logicTween.Contains(tweener))
		{
			logicTween.Add(tweener);
			if (Instance.isPause)
			{
				tweener.Pause();
			}
		}
	}

	public void ClearSequence()
	{
		logicSequence.Clear();
	}

	public void AddSequenceToList(Sequence sequence)
	{
		if (!logicSequence.Contains(sequence))
		{
			logicSequence.Add(sequence);
			if (Instance.isPause)
			{
				sequence.Pause();
			}
		}
	}

	public void LogicSequencePause()
	{
		foreach (Sequence item in logicSequence)
		{
			if (item != null)
			{
				item.Pause();
			}
		}
	}

	public void LogicSequencePlay()
	{
		foreach (Sequence item in logicSequence)
		{
			if (item != null)
			{
				item.Play();
			}
		}
	}

	public Sequence GetSequence()
	{
		Sequence sequence = DOTween.Sequence();
		AddSequenceToList(sequence);
		return sequence;
	}

	public void PauseGame()
	{
		isPause = true;
		LogicSequencePause();
		LogicTweenPause();
		LogicUpdatePause();
		RenderUpdatePause();
		PoolManager.Ins.PauseEffect();
	}

	public void PlayGame()
	{
		isPause = false;
		LogicSequencePlay();
		LogicTweenPlay();
		LogicUpdatePlay();
		RenderUpdatePlay();
		PoolManager.Ins.PlayEffect();
	}

	public void ClearLoopData()
	{
		ClearSequence();
		ClearTween();
		ClearLogicUpdata();
		ClearRenderUpdate();
		DOTween.Clear();
	}
}
