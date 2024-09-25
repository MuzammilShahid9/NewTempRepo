using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Editor;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public static class AreaMoveTool
	{
		public static void SetAnchorPoint(Board board, int areaIndex, bool isAnim = true)
		{
			Area area = board.data.areaList[areaIndex];
			board.levelColStart = area.start.x;
			board.levelColEnd = area.end.x;
			board.levelRowStart = area.end.y;
			board.levelRowEnd = area.start.y;
			Vector2 vector = board.AreaPosDic[areaIndex];
			GameLogic.Instance.isAreaMoving = true;
			if (isAnim)
			{
				AudioManager.Instance.PlayAudioEffect("board_moving");
				board.container.transform.DOGameTweenLocalMove(vector, 1f).OnComplete(delegate
				{
					for (int k = board.levelRowStart; k <= board.levelRowEnd; k++)
					{
						for (int l = board.levelColStart; l <= board.levelColEnd; l++)
						{
							Cell cell2 = board.cells[k, l];
							if (!cell2.empty && !cell2.Blocked())
							{
								GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, board.cells[k, l].element);
							}
						}
					}
					board.canCheckAreaChange = false;
					GameLogic.Instance.isAreaMoving = false;
					Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
				}).SetDelay(1f);
				return;
			}
			board.container.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			for (int i = board.levelRowStart; i <= board.levelRowEnd; i++)
			{
				for (int j = board.levelColStart; j <= board.levelColEnd; j++)
				{
					Cell cell = board.cells[i, j];
					if (!cell.empty && !cell.Blocked())
					{
						GameLogic.Instance.RemoveTrue = RemoveMatchTool.RemoveMatch(board, board.cells[i, j].element);
					}
				}
			}
			board.canCheckAreaChange = false;
			GameLogic.Instance.isAreaMoving = false;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
		}

		public static void SetAnchorPointByInit(Board board, int areaIndex, bool isAnim = false)
		{
			Area area = board.data.areaList[areaIndex];
			board.levelColStart = area.start.x;
			board.levelColEnd = area.end.x;
			board.levelRowStart = area.end.y;
			board.levelRowEnd = area.start.y;
			Vector2 vector = board.AreaPosDic[areaIndex];
			if (isAnim)
			{
				board.container.transform.DOGameTweenLocalMove(vector, 1f).SetDelay(1f);
			}
			else
			{
				board.container.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
			}
		}

		public static void DoMapChange(Transform mapGrid, Vector3 targetPos, float time, TweenCallback action = null)
		{
			AudioManager.Instance.PlayAudioEffect("board_moving");
			if (action == null)
			{
				mapGrid.DOGameTweenLocalMove(targetPos, time);
			}
			else
			{
				mapGrid.DOGameTweenLocalMove(targetPos, time).OnComplete(action);
			}
		}

		public static Dictionary<int, Vector2> InitAreaPos(Board board)
		{
			Dictionary<int, Vector2> dictionary = new Dictionary<int, Vector2>();
			float num = (float)board.data.width / 2f;
			float num2 = (float)board.data.height / 2f;
			float num3 = 0.78f;
			for (int i = 0; i < board.data.areaList.Length; i++)
			{
				Area area = board.data.areaList[i];
				float num4 = (float)(area.start.x + area.end.x) / 2f + 0.5f;
				float num5 = (float)(area.start.y + area.end.y) / 2f + 0.5f;
				if (num == num4 && num2 == num5)
				{
					dictionary.Add(i, new Vector2(0f, 0f));
				}
				else
				{
					dictionary.Add(i, new Vector2((num - num4) * num3, (num2 - num5) * num3));
				}
			}
			return dictionary;
		}
	}
}
