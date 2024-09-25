using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using Umeng;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
	public PolygonCollider2D collider;

	public Transform hand;

	public Transform TipArea;

	public Transform TipText;

	public Transform Role;

	public Transform RoleLeftPos;

	public Transform RoleRightPos;

	public Transform TextLeftPos;

	public Transform TextRightPos;

	public GameObject TargetTip;

	public GameObject StepNumTip;

	public GameObject standard;

	public GameObject oneCell;

	public GameObject flybomb;

	public GameObject flybomb2;

	public GameObject areabomb;

	public GameObject colorbomb;

	public GameObject twoCell;

	public GameObject vhbomb;

	public GameObject dropTipSpoon;

	public GameObject dropTipHammar;

	public GameObject dropTipGlove;

	public GameObject szTip;

	public GameObject skillTip;

	public LocalizationText skillTipText;

	public Transform TextBG;

	public Transform SkillRoot;

	public List<Transform> TextAreaPos;

	public Canvas canvas;

	private float halfX = 0.39f;

	private float halfy = 0.39f;

	private float height;

	private float width;

	private float widthRate;

	private float heightRate;

	public const float canvasScale = 0.01f;

	private float scaleFactor;

	private int level;

	private int _Step;

	private Sequence seq;

	private MessageHandler handler;

	private int Step
	{
		get
		{
			return _Step;
		}
		set
		{
			_Step = value;
		}
	}

	private void Start()
	{
		if (GameConfig.isActiveGuide)
		{
			level = UserDataManager.Instance.GetProgress();
			if (level == 1 || level == 2 || level == 3 || level == 4 || level == 5 || level == 9 || level == 19 || level == 28 || level == 6 || level == 8 || level == 11 || level == 13 || level == 16 || level == 21 || level == 31 || level == 51 || level == 71 || level == 91 || level == 121 || level == 151 || level == 181 || level == 221)
			{
				Step = 0;
				scaleFactor = 0.01f / canvas.gameObject.GetComponent<RectTransform>().localScale.x / GameSceneUIManager.Instance.matchRate;
				DebugUtils.Log(DebugType.Other, GameSceneUIManager.Instance.matchRate);
				width = canvas.gameObject.GetComponent<RectTransform>().sizeDelta.x;
				height = canvas.gameObject.GetComponent<RectTransform>().sizeDelta.y;
				widthRate = width / (float)Screen.width;
				heightRate = height / (float)Screen.height;
				Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(14u, handler);
				Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(12u, StartGuide);
				Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(13u, PauseGuide);
				Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(9u, ActiveGuideListener);
				Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(2u, PauseGuide);
				Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(1u, PauseGuide);
			}
			UserDataManager.Instance.GetService().preLevel = UserDataManager.Instance.GetService().level;
		}
	}

	public void ShowGuidMask(List<Cell> cellList)
	{
		collider.gameObject.SetActive(true);
		List<Vector2> collection = new List<Vector2>
		{
			new Vector2(0f, 0f),
			new Vector2(0f, height),
			new Vector2(width, height),
			new Vector2(width, 0f),
			new Vector2(0f, 0f)
		};
		List<Vector2> list = new List<Vector2>();
		if (cellList != null && cellList.Count > 0)
		{
			float num = 0.39f;
			float num2 = 0.39f;
			foreach (Cell cell in cellList)
			{
				Vector2 vector = cell.transform.position + new Vector3(0f - num, num2, 0f);
				Vector2 vector2 = cell.transform.position + new Vector3(num, num2, 0f);
				Vector2 vector3 = cell.transform.position + new Vector3(num, 0f - num2, 0f);
				Vector2 vector4 = cell.transform.position + new Vector3(0f - num, 0f - num2, 0f);
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector4.x, vector4.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector.x, vector.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector2.x, vector2.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector3.x, vector3.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector4.x, vector4.y)));
				list.AddRange(collection);
			}
		}
		else
		{
			list.AddRange(collection);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 value = list[i];
			value.x *= widthRate;
			value.y *= heightRate;
			list[i] = value;
		}
		collider.pathCount = 1;
		collider.points = list.ToArray();
	}

	public void ShowGuidMask(List<Vector2> vector2List)
	{
		collider.gameObject.SetActive(false);
		List<Vector2> collection = new List<Vector2>
		{
			new Vector2(0f, 0f),
			new Vector2(0f, height),
			new Vector2(width, height),
			new Vector2(width, 0f),
			new Vector2(0f, 0f)
		};
		for (int i = 0; i < vector2List.Count; i++)
		{
			Vector2 value = vector2List[i];
			value.x *= widthRate;
			value.y *= heightRate;
			vector2List[i] = value;
		}
		vector2List.AddRange(collection);
		collider.points = null;
		collider.pathCount = 1;
		collider.points = vector2List.ToArray();
		collider.gameObject.SetActive(true);
	}

	public void StartGuide(uint iMessageType, object arg)
	{
		if (GameLogic.Instance.isFinish || (iMessageType == 6 && !(bool)arg) || GameLogic.Instance.isGuiding || GameLogic.Instance.currentBoard.movingCount != 0)
		{
			return;
		}
		GameLogic.Instance.isGuiding = true;
		Board board = GameLogic.Instance.currentBoard;
		if (level == 1)
		{
			Step++;
			new List<Cell>();
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (Step == 1)
			{
				seq = UpdateManager.Instance.GetSequence().SetLoops(-1, LoopType.Restart);
				List<Cell> obj = new List<Cell>
				{
					board.cells[4, 2],
					board.cells[5, 2]
				};
				zero = board.cells[5, 2].transform.position;
				zero2 = board.cells[4, 2].transform.position;
				hand.gameObject.SetActive(true);
				TipArea.transform.position = TextAreaPos[0].transform.position;
				Role.SetParent(RoleLeftPos, false);
				Vector3 one = Vector3.one;
				one.x = 1f;
				Role.transform.localScale = one;
				Role.transform.localPosition = Vector3.zero;
				TipText.SetParent(TextLeftPos);
				TipText.transform.localPosition = Vector3.zero;
				TipArea.gameObject.SetActive(true);
				standard.SetActive(true);
				standard.transform.position = zero;
				standard.transform.localScale = Vector3.one * scaleFactor;
				List<Vector2> list = new List<Vector2>();
				Vector2 vector = obj[1].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector2 = obj[1].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector3 = obj[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector4 = obj[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector4.x, vector4.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector.x, vector.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector2.x, vector2.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector3.x, vector3.y)));
				list.Add(Camera.main.WorldToScreenPoint(new Vector2(vector4.x, vector4.y)));
				ShowGuidMask(list);
				hand.position = new Vector3(zero.x, zero.y, hand.position.z);
				seq.Append(hand.GetComponentInChildren<Image>().DOFade(0f, 0f));
				seq.Append(hand.DOScale(Vector3.one * 0.8f, 0.3f));
				seq.Join(hand.GetComponentInChildren<Image>().DOFade(1f, 0.3f));
				seq.Append(hand.DOMove(zero2, 1f));
				seq.Append(hand.DOScale(Vector3.one * 1.2f, 0.6f));
				seq.Join(hand.GetComponentInChildren<Image>().DOFade(0f, 0.3f));
				LocalizationText[] componentsInChildren = TipArea.GetComponentsInChildren<LocalizationText>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].SetKeyString("guide_" + level + "_" + Step);
					LayoutRebuilder.ForceRebuildLayoutImmediate(componentsInChildren[i].rectTransform);
				}
				
			}
			else if (Step == 2)
			{
				seq = UpdateManager.Instance.GetSequence().SetLoops(-1, LoopType.Restart);
				List<Cell> obj2 = new List<Cell>
				{
					board.cells[1, 3],
					board.cells[1, 4]
				};
				zero = board.cells[1, 4].transform.position;
				zero2 = board.cells[1, 3].transform.position;
				hand.gameObject.SetActive(true);
				TipArea.transform.position = TextAreaPos[1].transform.position;
				Role.SetParent(RoleLeftPos, false);
				Vector3 one2 = Vector3.one;
				one2.x = 1f;
				Role.transform.localScale = one2;
				Role.transform.localPosition = Vector3.zero;
				TipText.SetParent(TextLeftPos);
				TipText.transform.localPosition = Vector3.zero;
				TipArea.gameObject.SetActive(true);
				standard.SetActive(true);
				standard.transform.localScale = Vector3.one * scaleFactor;
				standard.transform.position = zero;
				standard.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
				List<Vector2> list2 = new List<Vector2>();
				Vector2 vector5 = obj2[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector6 = obj2[1].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector7 = obj2[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector8 = obj2[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list2.Add(Camera.main.WorldToScreenPoint(new Vector2(vector8.x, vector8.y)));
				list2.Add(Camera.main.WorldToScreenPoint(new Vector2(vector5.x, vector5.y)));
				list2.Add(Camera.main.WorldToScreenPoint(new Vector2(vector6.x, vector6.y)));
				list2.Add(Camera.main.WorldToScreenPoint(new Vector2(vector7.x, vector7.y)));
				list2.Add(Camera.main.WorldToScreenPoint(new Vector2(vector8.x, vector8.y)));
				ShowGuidMask(list2);
				hand.position = new Vector3(zero.x, zero.y, hand.position.z);
				hand.localScale = Vector3.one;
				seq.Append(hand.GetComponentInChildren<Image>().DOFade(0f, 0f));
				seq.Append(hand.DOScale(Vector3.one * 0.8f, 0.3f));
				seq.Join(hand.GetComponentInChildren<Image>().DOFade(1f, 0.3f));
				seq.Append(hand.DOMove(zero2, 1f));
				seq.Append(hand.DOScale(Vector3.one * 1.2f, 0.6f));
				seq.Join(hand.GetComponentInChildren<Image>().DOFade(0f, 0.3f));
				LocalizationText[] componentsInChildren2 = TipArea.GetComponentsInChildren<LocalizationText>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].SetKeyString("guide_" + level + "_" + Step);
				}
				
			}
			else if (Step == 3)
			{
				TargetTip.SetActive(true);
				ShowGuidMask(new List<Vector2>());
				TipArea.transform.position = TextAreaPos[2].transform.position;
				Role.SetParent(RoleRightPos, false);
				Vector3 one3 = Vector3.one;
				one3.x = -1f;
				Role.transform.localScale = one3;
				Role.transform.localPosition = Vector3.zero;
				TipText.SetParent(TextRightPos);
				TipText.transform.localPosition = Vector3.zero;
				TipArea.gameObject.SetActive(true);
				LocalizationText[] componentsInChildren3 = TipArea.GetComponentsInChildren<LocalizationText>();
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					componentsInChildren3[k].SetKeyString("guide_" + level + "_" + Step);
				}
				
				float time = 0f;
				UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
				{
					if (time > 6f || Input.GetMouseButtonDown(0))
					{
						TargetTip.SetActive(false);
						StepNumTip.SetActive(true);
						ShowGuidMask(new List<Vector2>());
						TipArea.transform.position = TextAreaPos[3].transform.position;
						Role.SetParent(RoleRightPos, false);
						Vector3 one4 = Vector3.one;
						one4.x = -1f;
						Role.transform.localScale = one4;
						Role.transform.localPosition = Vector3.zero;
						TipText.SetParent(TextRightPos);
						TipText.transform.localPosition = Vector3.zero;
						TipArea.gameObject.SetActive(true);
						LocalizationText[] componentsInChildren4 = TipArea.GetComponentsInChildren<LocalizationText>();
						for (int l = 0; l < componentsInChildren4.Length; l++)
						{
							componentsInChildren4[l].SetKeyString("guide_" + level + "_" + 4);
						}
						
						float time2 = 0f;
						UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration2)
						{
							if (time2 > 6f || Input.GetMouseButtonDown(0))
							{
								PauseGuide(13u, null);
								return true;
							}
							time2 += duration2;
							return false;
						}));
						return true;
					}
					time += duration;
					return false;
				}));
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 2)
		{
			Step++;
			List<Cell> list3 = new List<Cell>();
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			if (Step == 1)
			{
				list3 = new List<Cell>
				{
					board.cells[7, 1],
					board.cells[6, 1]
				};
				zero3 = list3[0].transform.position;
				zero4 = list3[1].transform.position;
				List<Vector2> list4 = new List<Vector2>();
				Vector2 vector9 = list3[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector10 = list3[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector11 = list3[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector12 = list3[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list4.Add(Camera.main.WorldToScreenPoint(new Vector2(vector12.x, vector12.y)));
				list4.Add(Camera.main.WorldToScreenPoint(new Vector2(vector9.x, vector9.y)));
				list4.Add(Camera.main.WorldToScreenPoint(new Vector2(vector10.x, vector10.y)));
				list4.Add(Camera.main.WorldToScreenPoint(new Vector2(vector11.x, vector11.y)));
				list4.Add(Camera.main.WorldToScreenPoint(new Vector2(vector12.x, vector12.y)));
				GeneralTip(zero3, list4, "guide_" + level + "_" + Step, 4, flybomb, new List<Vector3> { zero4 }, false);
			}
			else if (Step == 2)
			{
				list3 = new List<Cell> { board.cells[5, 1] };
				zero3 = list3[0].transform.position;
				zero4 = list3[0].transform.position;
				List<Vector2> list5 = new List<Vector2>();
				Vector2 vector13 = list3[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector14 = list3[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector15 = list3[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector16 = list3[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list5.Add(Camera.main.WorldToScreenPoint(new Vector2(vector16.x, vector16.y)));
				list5.Add(Camera.main.WorldToScreenPoint(new Vector2(vector13.x, vector13.y)));
				list5.Add(Camera.main.WorldToScreenPoint(new Vector2(vector14.x, vector14.y)));
				list5.Add(Camera.main.WorldToScreenPoint(new Vector2(vector15.x, vector15.y)));
				list5.Add(Camera.main.WorldToScreenPoint(new Vector2(vector16.x, vector16.y)));
				if (!board.cells[5, 1].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				GeneralTip(zero3, list5, "guide_" + level + "_" + Step, 5, oneCell, new List<Vector3> { zero4 }, false);
			}
			else if (Step == 3)
			{
				list3 = new List<Cell>
				{
					board.cells[2, 3],
					board.cells[2, 4]
				};
				zero3 = list3[0].transform.position;
				zero4 = list3[1].transform.position;
				List<Vector2> list6 = new List<Vector2>();
				Vector2 vector17 = list3[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector18 = list3[1].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector19 = list3[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector20 = list3[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list6.Add(Camera.main.WorldToScreenPoint(new Vector2(vector20.x, vector20.y)));
				list6.Add(Camera.main.WorldToScreenPoint(new Vector2(vector17.x, vector17.y)));
				list6.Add(Camera.main.WorldToScreenPoint(new Vector2(vector18.x, vector18.y)));
				list6.Add(Camera.main.WorldToScreenPoint(new Vector2(vector19.x, vector19.y)));
				list6.Add(Camera.main.WorldToScreenPoint(new Vector2(vector20.x, vector20.y)));
				GeneralTip(zero3, list6, "guide_" + level + "_" + Step, 6, flybomb2, new List<Vector3> { zero4 });
			}
			else if (Step == 4)
			{
				list3 = new List<Cell>
				{
					board.cells[2, 4],
					board.cells[1, 4],
					board.cells[2, 3],
					board.cells[3, 4],
					board.cells[2, 5]
				};
				if (!board.cells[2, 4].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero3 = list3[0].transform.position;
				List<Vector2> list7 = new List<Vector2>();
				Vector2 vector21 = list3[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector22 = list3[1].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector23 = list3[2].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector24 = list3[2].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector25 = list3[2].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector26 = list3[3].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector27 = list3[3].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector28 = list3[3].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector29 = list3[4].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector30 = list3[4].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector31 = list3[4].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector32 = list3[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				list7.Add(Camera.main.WorldToScreenPoint(vector21));
				list7.Add(Camera.main.WorldToScreenPoint(vector22));
				list7.Add(Camera.main.WorldToScreenPoint(vector23));
				list7.Add(Camera.main.WorldToScreenPoint(vector24));
				list7.Add(Camera.main.WorldToScreenPoint(vector25));
				list7.Add(Camera.main.WorldToScreenPoint(vector26));
				list7.Add(Camera.main.WorldToScreenPoint(vector27));
				list7.Add(Camera.main.WorldToScreenPoint(vector28));
				list7.Add(Camera.main.WorldToScreenPoint(vector29));
				list7.Add(Camera.main.WorldToScreenPoint(vector30));
				list7.Add(Camera.main.WorldToScreenPoint(vector31));
				list7.Add(Camera.main.WorldToScreenPoint(vector32));
				list7.Add(Camera.main.WorldToScreenPoint(vector21));
				GameLogic.Instance.isCanDoubleActiveBomb = false;
				GeneralTip(zero3, list7, "guide_" + level + "_" + Step, 7, szTip, new List<Vector3>
				{
					list3[1].transform.position,
					list3[2].transform.position,
					list3[3].transform.position,
					list3[4].transform.position
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 3)
		{
			Step++;
			List<Cell> list8 = new List<Cell>();
			Vector3 zero5 = Vector3.zero;
			Vector3 zero6 = Vector3.zero;
			if (Step == 1)
			{
				list8 = new List<Cell>
				{
					board.cells[7, 3],
					board.cells[7, 4]
				};
				zero5 = list8[0].transform.position;
				zero6 = list8[1].transform.position;
				List<Vector2> list9 = new List<Vector2>();
				Vector2 vector33 = list8[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector34 = list8[1].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector35 = list8[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector36 = list8[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list9.Add(Camera.main.WorldToScreenPoint(new Vector2(vector36.x, vector36.y)));
				list9.Add(Camera.main.WorldToScreenPoint(new Vector2(vector33.x, vector33.y)));
				list9.Add(Camera.main.WorldToScreenPoint(new Vector2(vector34.x, vector34.y)));
				list9.Add(Camera.main.WorldToScreenPoint(new Vector2(vector35.x, vector35.y)));
				list9.Add(Camera.main.WorldToScreenPoint(new Vector2(vector36.x, vector36.y)));
				vhbomb.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
				GeneralTip(zero5, list9, "guide_" + level + "_" + Step, 8, vhbomb, new List<Vector3> { zero6 });
			}
			else if (Step == 2)
			{
				list8 = new List<Cell> { board.cells[5, 4] };
				if (!board.cells[5, 4].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero5 = list8[0].transform.position;
				zero6 = list8[0].transform.position;
				List<Vector2> list10 = new List<Vector2>();
				Vector2 vector37 = list8[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector38 = list8[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector39 = list8[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector40 = list8[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list10.Add(Camera.main.WorldToScreenPoint(new Vector2(vector40.x, vector40.y)));
				list10.Add(Camera.main.WorldToScreenPoint(new Vector2(vector37.x, vector37.y)));
				list10.Add(Camera.main.WorldToScreenPoint(new Vector2(vector38.x, vector38.y)));
				list10.Add(Camera.main.WorldToScreenPoint(new Vector2(vector39.x, vector39.y)));
				list10.Add(Camera.main.WorldToScreenPoint(new Vector2(vector40.x, vector40.y)));
				GeneralTip(zero5, list10, "guide_" + level + "_" + Step, 9, oneCell, new List<Vector3> { zero6 });
			}
			else if (Step == 3)
			{
				list8 = new List<Cell>
				{
					board.cells[2, 4],
					board.cells[1, 4]
				};
				zero5 = list8[0].transform.position;
				zero6 = list8[1].transform.position;
				List<Vector2> list11 = new List<Vector2>();
				Vector2 vector41 = list8[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector42 = list8[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector43 = list8[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector44 = list8[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list11.Add(Camera.main.WorldToScreenPoint(new Vector2(vector44.x, vector44.y)));
				list11.Add(Camera.main.WorldToScreenPoint(new Vector2(vector41.x, vector41.y)));
				list11.Add(Camera.main.WorldToScreenPoint(new Vector2(vector42.x, vector42.y)));
				list11.Add(Camera.main.WorldToScreenPoint(new Vector2(vector43.x, vector43.y)));
				list11.Add(Camera.main.WorldToScreenPoint(new Vector2(vector44.x, vector44.y)));
				vhbomb.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
				GeneralTip(zero5, list11, "guide_" + level + "_" + Step, 10, vhbomb, new List<Vector3> { zero6 });
			}
			else if (Step == 4)
			{
				list8 = new List<Cell>
				{
					board.cells[1, 4],
					board.cells[0, 4],
					board.cells[1, 3],
					board.cells[2, 4],
					board.cells[1, 5]
				};
				if (!board.cells[1, 4].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero5 = list8[0].transform.position;
				List<Vector2> list12 = new List<Vector2>();
				Vector2 vector45 = list8[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector46 = list8[1].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector47 = list8[2].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector48 = list8[2].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector49 = list8[2].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector50 = list8[3].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector51 = list8[3].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector52 = list8[3].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector53 = list8[4].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector54 = list8[4].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector55 = list8[4].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector56 = list8[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				list12.Add(Camera.main.WorldToScreenPoint(vector45));
				list12.Add(Camera.main.WorldToScreenPoint(vector46));
				list12.Add(Camera.main.WorldToScreenPoint(vector47));
				list12.Add(Camera.main.WorldToScreenPoint(vector48));
				list12.Add(Camera.main.WorldToScreenPoint(vector49));
				list12.Add(Camera.main.WorldToScreenPoint(vector50));
				list12.Add(Camera.main.WorldToScreenPoint(vector51));
				list12.Add(Camera.main.WorldToScreenPoint(vector52));
				list12.Add(Camera.main.WorldToScreenPoint(vector53));
				list12.Add(Camera.main.WorldToScreenPoint(vector54));
				list12.Add(Camera.main.WorldToScreenPoint(vector55));
				list12.Add(Camera.main.WorldToScreenPoint(vector56));
				list12.Add(Camera.main.WorldToScreenPoint(vector45));
				GameLogic.Instance.isCanDoubleActiveBomb = false;
				GeneralTip(zero5, list12, "guide_" + level + "_" + Step, 11, szTip, new List<Vector3>
				{
					list8[1].transform.position,
					list8[2].transform.position,
					list8[3].transform.position,
					list8[4].transform.position
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 4)
		{
			Step++;
			List<Cell> list13 = new List<Cell>();
			Vector3 zero7 = Vector3.zero;
			Vector3 zero8 = Vector3.zero;
			if (Step == 1)
			{
				list13 = new List<Cell>
				{
					board.cells[7, 5],
					board.cells[7, 4]
				};
				zero7 = list13[0].transform.position;
				zero8 = list13[1].transform.position;
				List<Vector2> list14 = new List<Vector2>();
				Vector2 vector57 = list13[1].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector58 = list13[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector59 = list13[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector60 = list13[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list14.Add(Camera.main.WorldToScreenPoint(new Vector2(vector60.x, vector60.y)));
				list14.Add(Camera.main.WorldToScreenPoint(new Vector2(vector57.x, vector57.y)));
				list14.Add(Camera.main.WorldToScreenPoint(new Vector2(vector58.x, vector58.y)));
				list14.Add(Camera.main.WorldToScreenPoint(new Vector2(vector59.x, vector59.y)));
				list14.Add(Camera.main.WorldToScreenPoint(new Vector2(vector60.x, vector60.y)));
				GeneralTip(zero7, list14, "guide_" + level + "_" + Step, 12, areabomb, new List<Vector3> { zero8 });
			}
			else if (Step == 2)
			{
				list13 = new List<Cell> { board.cells[6, 4] };
				if (!board.cells[6, 4].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero7 = list13[0].transform.position;
				zero8 = list13[0].transform.position;
				List<Vector2> list15 = new List<Vector2>();
				Vector2 vector61 = list13[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector62 = list13[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector63 = list13[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector64 = list13[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list15.Add(Camera.main.WorldToScreenPoint(new Vector2(vector64.x, vector64.y)));
				list15.Add(Camera.main.WorldToScreenPoint(new Vector2(vector61.x, vector61.y)));
				list15.Add(Camera.main.WorldToScreenPoint(new Vector2(vector62.x, vector62.y)));
				list15.Add(Camera.main.WorldToScreenPoint(new Vector2(vector63.x, vector63.y)));
				list15.Add(Camera.main.WorldToScreenPoint(new Vector2(vector64.x, vector64.y)));
				GeneralTip(zero7, list15, "guide_" + level + "_" + Step, 13, oneCell, new List<Vector3> { zero8 });
			}
			else if (Step == 3)
			{
				list13 = new List<Cell>
				{
					board.cells[3, 4],
					board.cells[2, 4],
					board.cells[3, 3],
					board.cells[4, 4],
					board.cells[3, 5]
				};
				if (!board.cells[3, 4].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero7 = list13[0].transform.position;
				List<Vector2> list16 = new List<Vector2>();
				Vector2 vector65 = list13[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector66 = list13[1].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector67 = list13[2].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector68 = list13[2].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector69 = list13[2].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector70 = list13[3].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector71 = list13[3].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector72 = list13[3].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector73 = list13[4].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector74 = list13[4].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector75 = list13[4].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				Vector2 vector76 = list13[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				list16.Add(Camera.main.WorldToScreenPoint(vector65));
				list16.Add(Camera.main.WorldToScreenPoint(vector66));
				list16.Add(Camera.main.WorldToScreenPoint(vector67));
				list16.Add(Camera.main.WorldToScreenPoint(vector68));
				list16.Add(Camera.main.WorldToScreenPoint(vector69));
				list16.Add(Camera.main.WorldToScreenPoint(vector70));
				list16.Add(Camera.main.WorldToScreenPoint(vector71));
				list16.Add(Camera.main.WorldToScreenPoint(vector72));
				list16.Add(Camera.main.WorldToScreenPoint(vector73));
				list16.Add(Camera.main.WorldToScreenPoint(vector74));
				list16.Add(Camera.main.WorldToScreenPoint(vector75));
				list16.Add(Camera.main.WorldToScreenPoint(vector76));
				list16.Add(Camera.main.WorldToScreenPoint(vector65));
				GameLogic.Instance.isCanDoubleActiveBomb = false;
				GeneralTip(zero7, list16, "guide_" + level + "_" + Step, 14, szTip, new List<Vector3>
				{
					list13[1].transform.position,
					list13[2].transform.position,
					list13[3].transform.position,
					list13[4].transform.position
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 5)
		{
			Step++;
			new List<Cell>();
			Vector3 zero9 = Vector3.zero;
			Vector3 zero10 = Vector3.zero;
			if (Step == 1)
			{
				List<Cell> obj3 = new List<Cell>
				{
					board.cells[7, 5],
					board.cells[6, 5]
				};
				zero9 = obj3[0].transform.position;
				zero10 = obj3[1].transform.position;
				List<Vector2> list17 = new List<Vector2>();
				Vector2 vector77 = obj3[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector78 = obj3[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector79 = obj3[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector80 = obj3[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list17.Add(Camera.main.WorldToScreenPoint(new Vector2(vector80.x, vector80.y)));
				list17.Add(Camera.main.WorldToScreenPoint(new Vector2(vector77.x, vector77.y)));
				list17.Add(Camera.main.WorldToScreenPoint(new Vector2(vector78.x, vector78.y)));
				list17.Add(Camera.main.WorldToScreenPoint(new Vector2(vector79.x, vector79.y)));
				list17.Add(Camera.main.WorldToScreenPoint(new Vector2(vector80.x, vector80.y)));
				GeneralTip(zero9, list17, "guide_" + level + "_" + Step, 15, colorbomb, new List<Vector3> { zero10 });
			}
			else if (Step == 2)
			{
				List<Cell> obj4 = new List<Cell>
				{
					board.cells[6, 5],
					board.cells[5, 5]
				};
				if (!board.cells[6, 5].element.IsBomb())
				{
					Analytics.Event("Guide_BombError", new Dictionary<string, string> { 
					{
						"Guide_BombError",
						level + " - " + Step
					} });
				}
				zero9 = obj4[0].transform.position;
				zero10 = obj4[1].transform.position;
				List<Vector2> list18 = new List<Vector2>();
				Vector2 vector81 = obj4[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector82 = obj4[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector83 = obj4[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector84 = obj4[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list18.Add(Camera.main.WorldToScreenPoint(new Vector2(vector84.x, vector84.y)));
				list18.Add(Camera.main.WorldToScreenPoint(new Vector2(vector81.x, vector81.y)));
				list18.Add(Camera.main.WorldToScreenPoint(new Vector2(vector82.x, vector82.y)));
				list18.Add(Camera.main.WorldToScreenPoint(new Vector2(vector83.x, vector83.y)));
				list18.Add(Camera.main.WorldToScreenPoint(new Vector2(vector84.x, vector84.y)));
				GameLogic.Instance.isCanDoubleActiveBomb = false;
				GeneralTip(zero9, list18, "guide_" + level + "_" + Step, 16, twoCell, new List<Vector3> { zero10 });
			}
			else if (Step == 3)
			{
				List<Cell> obj5 = new List<Cell> { board.cells[1, 5] };
				zero9 = obj5[0].transform.position;
				zero10 = obj5[0].transform.position;
				List<Vector2> list19 = new List<Vector2>();
				Vector2 vector85 = obj5[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector86 = obj5[0].transform.position + new Vector3(halfX, halfy, 0f);
				Vector2 vector87 = obj5[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector88 = obj5[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
				list19.Add(Camera.main.WorldToScreenPoint(new Vector2(vector88.x, vector88.y)));
				list19.Add(Camera.main.WorldToScreenPoint(new Vector2(vector85.x, vector85.y)));
				list19.Add(Camera.main.WorldToScreenPoint(new Vector2(vector86.x, vector86.y)));
				list19.Add(Camera.main.WorldToScreenPoint(new Vector2(vector87.x, vector87.y)));
				list19.Add(Camera.main.WorldToScreenPoint(new Vector2(vector88.x, vector88.y)));
				GeneralTip(zero9, list19, "guide_" + level + "_" + Step, 17, oneCell, new List<Vector3> { zero10 });
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 9)
		{
			Step++;
			List<Cell> cellList3 = new List<Cell>();
			Vector3 handStartPos3 = Vector3.zero;
			Vector3 handEndPos3 = Vector3.zero;
			if (Step == 1)
			{
				handStartPos3 = dropTipSpoon.transform.position;
				handEndPos3 = handStartPos3;
				GuideDropSetting(0, true);
				List<Vector2> list20 = new List<Vector2>();
				Vector2 vector89 = handStartPos3 + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector90 = handStartPos3 + new Vector3(halfX, halfy, 0f);
				Vector2 vector91 = handStartPos3 + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector92 = handStartPos3 + new Vector3(0f - halfX, 0f - halfy, 0f);
				list20.Add(Camera.main.WorldToScreenPoint(new Vector2(vector92.x, vector92.y)));
				list20.Add(Camera.main.WorldToScreenPoint(new Vector2(vector89.x, vector89.y)));
				list20.Add(Camera.main.WorldToScreenPoint(new Vector2(vector90.x, vector90.y)));
				list20.Add(Camera.main.WorldToScreenPoint(new Vector2(vector91.x, vector91.y)));
				list20.Add(Camera.main.WorldToScreenPoint(new Vector2(vector92.x, vector92.y)));
				GeneralTip(handStartPos3, list20, "guide_" + level + "_" + Step, 18, dropTipSpoon, new List<Vector3> { handEndPos3 });
				handler = Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(14u, delegate(uint iMessageType2, object arg2)
				{
					if ((DropType)arg2 == DropType.Spoon)
					{
						TipArea.gameObject.SetActive(false);
						dropTipSpoon.SetActive(false);
						GuideDropSetting(0, false);
						cellList3 = new List<Cell> { board.cells[8, 5] };
						handStartPos3 = cellList3[0].transform.position;
						handEndPos3 = handStartPos3;
						List<Vector2> list25 = new List<Vector2>();
						Vector2 vector109 = cellList3[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
						Vector2 vector110 = cellList3[0].transform.position + new Vector3(halfX, halfy, 0f);
						Vector2 vector111 = cellList3[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
						Vector2 vector112 = cellList3[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
						list25.Add(Camera.main.WorldToScreenPoint(new Vector2(vector112.x, vector112.y)));
						list25.Add(Camera.main.WorldToScreenPoint(new Vector2(vector109.x, vector109.y)));
						list25.Add(Camera.main.WorldToScreenPoint(new Vector2(vector110.x, vector110.y)));
						list25.Add(Camera.main.WorldToScreenPoint(new Vector2(vector111.x, vector111.y)));
						list25.Add(Camera.main.WorldToScreenPoint(new Vector2(vector112.x, vector112.y)));
						GeneralTip(handStartPos3, list25, "guide_" + level + "_" + Step, 19, oneCell, new List<Vector3> { handEndPos3 });
						Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(14u, handler);
					}
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 19)
		{
			Step++;
			List<Cell> cellList2 = new List<Cell>();
			Vector3 handStartPos2 = Vector3.zero;
			Vector3 handEndPos2 = Vector3.zero;
			if (Step == 1)
			{
				handStartPos2 = dropTipHammar.transform.position;
				handEndPos2 = handStartPos2;
				GuideDropSetting(1, true);
				List<Vector2> list21 = new List<Vector2>();
				Vector2 vector93 = handStartPos2 + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector94 = handStartPos2 + new Vector3(halfX, halfy, 0f);
				Vector2 vector95 = handStartPos2 + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector96 = handStartPos2 + new Vector3(0f - halfX, 0f - halfy, 0f);
				list21.Add(Camera.main.WorldToScreenPoint(new Vector2(vector96.x, vector96.y)));
				list21.Add(Camera.main.WorldToScreenPoint(new Vector2(vector93.x, vector93.y)));
				list21.Add(Camera.main.WorldToScreenPoint(new Vector2(vector94.x, vector94.y)));
				list21.Add(Camera.main.WorldToScreenPoint(new Vector2(vector95.x, vector95.y)));
				list21.Add(Camera.main.WorldToScreenPoint(new Vector2(vector96.x, vector96.y)));
				GeneralTip(handStartPos2, list21, "guide_" + level + "_" + Step, 20, dropTipHammar, new List<Vector3> { handEndPos2 });
				handler = Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(14u, delegate(uint iMessageType2, object arg2)
				{
					if ((DropType)arg2 == DropType.Hammer)
					{
						TipArea.gameObject.SetActive(false);
						dropTipHammar.SetActive(false);
						GuideDropSetting(1, false);
						cellList2 = new List<Cell> { board.cells[6, 5] };
						handStartPos2 = cellList2[0].transform.position;
						handEndPos2 = handStartPos2;
						List<Vector2> list24 = new List<Vector2>();
						Vector2 vector105 = cellList2[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
						Vector2 vector106 = cellList2[0].transform.position + new Vector3(halfX, halfy, 0f);
						Vector2 vector107 = cellList2[0].transform.position + new Vector3(halfX, 0f - halfy, 0f);
						Vector2 vector108 = cellList2[0].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
						list24.Add(Camera.main.WorldToScreenPoint(new Vector2(vector108.x, vector108.y)));
						list24.Add(Camera.main.WorldToScreenPoint(new Vector2(vector105.x, vector105.y)));
						list24.Add(Camera.main.WorldToScreenPoint(new Vector2(vector106.x, vector106.y)));
						list24.Add(Camera.main.WorldToScreenPoint(new Vector2(vector107.x, vector107.y)));
						list24.Add(Camera.main.WorldToScreenPoint(new Vector2(vector108.x, vector108.y)));
						GeneralTip(handStartPos2, list24, "guide_" + level + "_" + Step, 21, oneCell, new List<Vector3> { handEndPos2 });
						Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(14u, handler);
					}
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 28)
		{
			Step++;
			List<Cell> cellList = new List<Cell>();
			Vector3 handStartPos = Vector3.zero;
			Vector3 handEndPos = Vector3.zero;
			if (Step == 1)
			{
				handStartPos = dropTipGlove.transform.position;
				handEndPos = handStartPos;
				GuideDropSetting(2, true);
				List<Vector2> list22 = new List<Vector2>();
				Vector2 vector97 = handStartPos + new Vector3(0f - halfX, halfy, 0f);
				Vector2 vector98 = handStartPos + new Vector3(halfX, halfy, 0f);
				Vector2 vector99 = handStartPos + new Vector3(halfX, 0f - halfy, 0f);
				Vector2 vector100 = handStartPos + new Vector3(0f - halfX, 0f - halfy, 0f);
				list22.Add(Camera.main.WorldToScreenPoint(new Vector2(vector100.x, vector100.y)));
				list22.Add(Camera.main.WorldToScreenPoint(new Vector2(vector97.x, vector97.y)));
				list22.Add(Camera.main.WorldToScreenPoint(new Vector2(vector98.x, vector98.y)));
				list22.Add(Camera.main.WorldToScreenPoint(new Vector2(vector99.x, vector99.y)));
				list22.Add(Camera.main.WorldToScreenPoint(new Vector2(vector100.x, vector100.y)));
				GeneralTip(handStartPos, list22, "guide_" + level + "_" + Step, 22, dropTipGlove, new List<Vector3> { handEndPos });
				handler = Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(14u, delegate(uint iMessageType2, object arg2)
				{
					if ((DropType)arg2 == DropType.Glove)
					{
						dropTipGlove.SetActive(false);
						TipArea.gameObject.SetActive(false);
						GuideDropSetting(2, false);
						cellList = new List<Cell>
						{
							board.cells[3, 5],
							board.cells[2, 5]
						};
						handStartPos = cellList[1].transform.position;
						handEndPos = cellList[0].transform.position;
						List<Vector2> list23 = new List<Vector2>();
						Vector2 vector101 = cellList[0].transform.position + new Vector3(0f - halfX, halfy, 0f);
						Vector2 vector102 = cellList[0].transform.position + new Vector3(halfX, halfy, 0f);
						Vector2 vector103 = cellList[1].transform.position + new Vector3(halfX, 0f - halfy, 0f);
						Vector2 vector104 = cellList[1].transform.position + new Vector3(0f - halfX, 0f - halfy, 0f);
						list23.Add(Camera.main.WorldToScreenPoint(new Vector2(vector104.x, vector104.y)));
						list23.Add(Camera.main.WorldToScreenPoint(new Vector2(vector101.x, vector101.y)));
						list23.Add(Camera.main.WorldToScreenPoint(new Vector2(vector102.x, vector102.y)));
						list23.Add(Camera.main.WorldToScreenPoint(new Vector2(vector103.x, vector103.y)));
						list23.Add(Camera.main.WorldToScreenPoint(new Vector2(vector104.x, vector104.y)));
						twoCell.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
						GeneralTip(handStartPos, list23, "guide_" + level + "_" + Step, 23, twoCell, new List<Vector3> { handEndPos });
						Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(14u, handler);
					}
				});
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 6)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj6 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_lvshu", typeof(GameObject)) as GameObject);
				obj6.transform.SetParent(SkillRoot, false);
				obj6.transform.localScale = Vector3.one * 135f;
				obj6.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 8)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj7 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_shutengtiao", typeof(GameObject)) as GameObject);
				obj7.transform.SetParent(SkillRoot, false);
				obj7.transform.localScale = Vector3.one * 135f;
				obj7.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 11)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj8 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_lvshu_caoping", typeof(GameObject)) as GameObject);
				obj8.transform.SetParent(SkillRoot, false);
				obj8.transform.localScale = Vector3.one * 135f;
				obj8.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 13)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj9 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_xiangzi", typeof(GameObject)) as GameObject);
				obj9.transform.SetParent(SkillRoot, false);
				obj9.transform.localScale = Vector3.one * 135f;
				obj9.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 16)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj10 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_shu", typeof(GameObject)) as GameObject);
				obj10.transform.SetParent(SkillRoot, false);
				obj10.transform.localScale = Vector3.one * 135f;
				obj10.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 21)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj11 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_fengmi_hudie", typeof(GameObject)) as GameObject);
				obj11.transform.SetParent(SkillRoot, false);
				obj11.transform.localScale = Vector3.one * 135f;
				obj11.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 31)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj12 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_tengtiao", typeof(GameObject)) as GameObject);
				obj12.transform.SetParent(SkillRoot, false);
				obj12.transform.localScale = Vector3.one * 135f;
				obj12.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 51)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj13 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_jiezhi", typeof(GameObject)) as GameObject);
				obj13.transform.SetParent(SkillRoot, false);
				obj13.transform.localScale = Vector3.one * 135f;
				obj13.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 71)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj14 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_heidong_pingguo", typeof(GameObject)) as GameObject);
				obj14.transform.SetParent(SkillRoot, false);
				obj14.transform.localScale = Vector3.one * 135f;
				obj14.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 91)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj15 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_1yun", typeof(GameObject)) as GameObject);
				obj15.transform.SetParent(SkillRoot, false);
				obj15.transform.localScale = Vector3.one * 135f;
				obj15.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 121)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj16 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_cuansongmen", typeof(GameObject)) as GameObject);
				obj16.transform.SetParent(SkillRoot, false);
				obj16.transform.localScale = Vector3.one * 135f;
				obj16.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 151)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj17 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_wuyun", typeof(GameObject)) as GameObject);
				obj17.transform.SetParent(SkillRoot, false);
				obj17.transform.localScale = Vector3.one * 135f;
				obj17.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 181)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj18 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_cuansongdai2", typeof(GameObject)) as GameObject);
				obj18.transform.SetParent(SkillRoot, false);
				obj18.transform.localScale = Vector3.one * 135f;
				obj18.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
				
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
		else if (level == 221)
		{
			Step++;
			if (Step == 1)
			{
				hand.gameObject.SetActive(false);
				GameObject obj19 = Object.Instantiate(Resources.Load("Effect/Eff/UI/eff_ui_6gongge_shoushihe", typeof(GameObject)) as GameObject);
				obj19.transform.SetParent(SkillRoot, false);
				obj19.transform.localScale = Vector3.one * 135f;
				obj19.transform.localPosition = Vector3.zero;
				ShowGuidMask(new List<Cell>());
				skillTip.SetActive(true);
				skillTipText.SetKeyString("guide_" + level + "_" + Step);
			
			}
			else
			{
				GameLogic.Instance.isGuiding = false;
				StopGuide();
			}
		}
	}

	public void PauseGuide(uint iMessageType, object arg)
	{
		GameLogic.Instance.isGuiding = false;
		GameLogic.Instance.isCanDoubleActiveBomb = true;
		seq.Kill();
		DOTween.Kill(seq);
		collider.gameObject.SetActive(false);
		TipArea.gameObject.SetActive(false);
		hand.gameObject.SetActive(false);
		if (SkillRoot.childCount > 0)
		{
			Object.Destroy(SkillRoot.GetChild(0).gameObject);
		}
		TargetTip.SetActive(false);
		StepNumTip.SetActive(false);
		standard.SetActive(false);
		oneCell.SetActive(false);
		flybomb.SetActive(false);
		areabomb.SetActive(false);
		colorbomb.SetActive(false);
		vhbomb.SetActive(false);
		dropTipGlove.SetActive(false);
		dropTipHammar.SetActive(false);
		dropTipSpoon.SetActive(false);
		szTip.SetActive(false);
		flybomb2.SetActive(false);
		twoCell.SetActive(false);
		skillTip.SetActive(false);
	}

	public void ActiveGuideListener(uint iMessageType, object arg)
	{
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(6u, StartGuide);
	}

	private void OnDestroy()
	{
		Step = 0;
		StopGuide();
	}

	private void StopGuide()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(12u, StartGuide);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(13u, PauseGuide);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(9u, ActiveGuideListener);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(2u, PauseGuide);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(1u, PauseGuide);
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(6u, StartGuide);
	}

	private void GuideDropSetting(int index, bool isActive)
	{
		collider.transform.GetComponent<Image>().raycastTarget = !isActive;
		Singleton<MessageDispatcher>.Instance().SendMessage(18u, isActive ? index : (-1));
	}

	private void GeneralTip(Vector3 handStartPos, List<Vector2> posList, string tipTextKey, int textPosIndex, GameObject tipArea, List<Vector3> handEndPos, bool isLeft = true)
	{
		seq.Kill();
		DOTween.Kill(seq);
		seq = UpdateManager.Instance.GetSequence().SetLoops(-1, LoopType.Restart).OnKill(delegate
		{
			hand.localScale = Vector3.one;
		});
		hand.gameObject.SetActive(true);
		hand.position = new Vector3(handStartPos.x, handStartPos.y, hand.position.z);
		if (tipTextKey != null)
		{
			TipArea.transform.position = TextAreaPos[textPosIndex].transform.position;
			TipArea.gameObject.SetActive(true);
			LocalizationText[] componentsInChildren = TipArea.GetComponentsInChildren<LocalizationText>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetKeyString(tipTextKey);
			}
			
		}
		Role.SetParent(isLeft ? RoleLeftPos : RoleRightPos, false);
		Vector3 one = Vector3.one;
		one.x = (isLeft ? 1 : (-1));
		Role.transform.localScale = one;
		Role.transform.localPosition = Vector3.zero;
		TipText.SetParent(isLeft ? TextLeftPos : TextRightPos);
		TipText.transform.localPosition = Vector3.zero;
		tipArea.SetActive(true);
		tipArea.transform.position = handStartPos;
		tipArea.transform.localScale = Vector3.one * scaleFactor;
		ShowGuidMask(posList);
		foreach (Vector3 handEndPo in handEndPos)
		{
			seq.Append(hand.DOMove(handStartPos, 0f));
			seq.Append(hand.GetComponentInChildren<Image>().DOFade(0f, 0f));
			seq.Append(hand.DOScale(Vector3.one * 0.8f, 0.3f));
			seq.Join(hand.GetComponentInChildren<Image>().DOFade(1f, 0.3f));
			seq.Append(hand.DOMove(handEndPo, 1f));
			seq.Append(hand.DOScale(Vector3.one * 1.2f, 0.6f));
			seq.Join(hand.GetComponentInChildren<Image>().DOFade(0f, 0.3f));
		}
	}

	public void Pause()
	{
		Singleton<MessageDispatcher>.Instance().SendMessage(13u, null);
		AudioManager.Instance.PlayAudioEffect("general_button");
	}
}
