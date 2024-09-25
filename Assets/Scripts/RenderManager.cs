using System;
using System.Collections.Generic;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;

public class RenderManager : MonoBehaviour
{
	private static RenderManager _instance;

	private List<RenderMessageEvent> renderMessageList;

	private List<RenderMessageEvent> currentRenderMessageList = new List<RenderMessageEvent>();

	public AnimationCurve curve;

	public AnimationCurve curveScale;

	public AnimationCurve skillCollect;

	public static RenderManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		renderMessageList = new List<RenderMessageEvent>();
	}

	public void ClearData()
	{
		renderMessageList.Clear();
		currentRenderMessageList.Clear();
	}

	public void RenderUpdate()
	{
		if (renderMessageList != null && renderMessageList.Count != 0)
		{
			currentRenderMessageList.AddRange(renderMessageList);
			ClearRenderList();
			foreach (RenderMessageEvent currentRenderMessage in currentRenderMessageList)
			{
				if (currentRenderMessage.type == RenderMessageType.ButterFly)
				{
					continue;
				}
				if (currentRenderMessage.type == RenderMessageType.ProcessBramble)
				{
					ProcessBramble obj = (ProcessBramble)currentRenderMessage.message;
					Element element2 = obj.element;
					Vector3 position4 = obj.position;
					if (element2.color < 10000)
					{
						UnityEngine.Object.Destroy(element2.gameObject);
					}
					else
					{
						element2.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[element2.color];
					}
					AudioManager.Instance.PlayAudioEffect("rattan_match", 0.1f);
					GameObject gameObject = PoolManager.Ins.SpawnEffect(element2.color + 1000, element2.board.container.transform);
					gameObject.transform.position = position4;
					PoolManager.Ins.DeSpawnEffect(gameObject, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessHoney)
				{
					ProcessHoney obj2 = (ProcessHoney)currentRenderMessage.message;
					Element element3 = obj2.element;
					Vector3 position5 = obj2.position;
					AudioManager.Instance.PlayAudioEffect("honey_match", 0.1f);
					if (element3.color < 20000)
					{
						UnityEngine.Object.Destroy(element3.gameObject);
					}
					else
					{
						element3.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[element3.color];
					}
					GameObject gameObject2 = PoolManager.Ins.SpawnEffect(20000, element3.board.container.transform);
					gameObject2.transform.position = position5;
					PoolManager.Ins.DeSpawnEffect(gameObject2, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessIce)
				{
					ProcessIce processIce = (ProcessIce)currentRenderMessage.message;
					Element element4 = processIce.element;
					Vector3 position6 = processIce.position;
					if (element4.color < 100)
					{
						element4.gameObject.SetActive(false);
					}
					else
					{
						element4.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[element4.color];
					}
					AudioManager.Instance.PlayAudioEffect("ice_match", 0.1f);
					GameObject gameObject3 = PoolManager.Ins.SpawnEffect(processIce.type, element4.board.container.transform);
					gameObject3.transform.position = position6;
					PoolManager.Ins.DeSpawnEffect(gameObject3, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessGrass)
				{
					ProcessGrass obj3 = (ProcessGrass)currentRenderMessage.message;
					Board board3 = obj3.board;
					int row = obj3.row;
					int col = obj3.col;
					ElementGenerator.Instance.Create(board3, 600, row, col);
					AudioManager.Instance.PlayAudioEffect("laying_lawn", 0.1f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessShell)
				{
					ProcessShell obj4 = (ProcessShell)currentRenderMessage.message;
					Element element5 = obj4.element;
					Vector3 position7 = obj4.position;
					AudioManager.Instance.PlayAudioEffect("nut_match", 0.1f);
					element5.gameObject.SetActive(false);
					GameObject gameObject4 = PoolManager.Ins.SpawnEffect(21, element5.board.container.transform);
					gameObject4.transform.position = position7;
					PoolManager.Ins.DeSpawnEffect(gameObject4, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessCat)
				{
					ProcessShell obj5 = (ProcessShell)currentRenderMessage.message;
					Element element6 = obj5.element;
					Vector3 position8 = obj5.position;
					AudioManager.Instance.PlayAudioEffect("nut_match", 0.1f);
					GameObject gameObject5 = PoolManager.Ins.SpawnEffect(21, element6.board.container.transform);
					gameObject5.transform.position = position8;
					PoolManager.Ins.DeSpawnEffect(gameObject5, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessFish)
				{
					ProcessShell obj6 = (ProcessShell)currentRenderMessage.message;
					Element element7 = obj6.element;
					Vector3 position9 = obj6.position;
					AudioManager.Instance.PlayAudioEffect("nut_match", 0.1f);
					GameObject gameObject6 = PoolManager.Ins.SpawnEffect(21, element7.board.container.transform);
					gameObject6.transform.position = position9;
					PoolManager.Ins.DeSpawnEffect(gameObject6, 0.6f);
					element7.transform.DOGameTweenScale(element7.transform.localScale * 1.5f, 0.4f).SetEase(Ease.OutBounce);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessBox)
				{
					ProcessBox processBox = (ProcessBox)currentRenderMessage.message;
					Element elem3 = processBox.element;
					Vector3 position = processBox.position;
					ElementType type = processBox.effectType;
					elem3.transform.DOKill();
					elem3.transform.DOGameTweenScale(Vector3.one * 1.15f, 0.08f).SetEase(Ease.InCubic).OnComplete(delegate
					{
						if (type <= ElementType.Box_2)
						{
							AudioManager.Instance.PlayAudioEffect("wood_match", 0.1f);
						}
						else
						{
							AudioManager.Instance.PlayAudioEffect("wood_match_rope ", 0.1f);
						}
						GameObject gameObject42 = PoolManager.Ins.SpawnEffect((int)type, elem3.board.container.transform);
						gameObject42.transform.position = position;
						PoolManager.Ins.DeSpawnEffect(gameObject42, 0.6f);
						if (elem3.color < 31)
						{
							elem3.gameObject.SetActive(false);
						}
						else
						{
							elem3.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[elem3.color];
							elem3.transform.DOScale(Vector3.one, 0.12f).SetEase(Ease.InCubic);
						}
					});
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessButton)
				{
					ProcessBox processBox2 = (ProcessBox)currentRenderMessage.message;
					Element elem4 = processBox2.element;
					Vector3 position2 = processBox2.position;
					ElementType type2 = processBox2.effectType;
					elem4.transform.DOKill();
					elem4.transform.DOGameTweenScale(Vector3.one * 1.15f, 0.08f).SetEase(Ease.InCubic).OnComplete(delegate
					{
						if (type2 <= ElementType.Button_2)
						{
							AudioManager.Instance.PlayAudioEffect("wood_match", 0.1f);
						}
						else
						{
							AudioManager.Instance.PlayAudioEffect("wood_match_rope ", 0.1f);
						}
						GameObject gameObject37 = PoolManager.Ins.SpawnEffect((int)type2, elem4.board.container.transform);
						gameObject37.transform.position = position2;
						PoolManager.Ins.DeSpawnEffect(gameObject37, 0.6f);
						if (elem4.color < 71)
						{
							elem4.gameObject.SetActive(false);
							if (elem4.LeftXian != null)
							{
								GameObject gameObject38 = PoolManager.Ins.SpawnEffect(40000010, elem4.board.container.transform);
								gameObject38.transform.position = position2;
								PoolManager.Ins.DeSpawnEffect(gameObject38, 0.6f);
								gameObject38.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
								UnityEngine.Object.Destroy(elem4.LeftXian);
							}
							if (elem4.RightXian != null)
							{
								GameObject gameObject39 = PoolManager.Ins.SpawnEffect(40000010, elem4.board.container.transform);
								gameObject39.transform.position = position2;
								PoolManager.Ins.DeSpawnEffect(gameObject39, 0.6f);
								gameObject39.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
								UnityEngine.Object.Destroy(elem4.RightXian);
							}
							if (elem4.UpXian != null)
							{
								GameObject gameObject40 = PoolManager.Ins.SpawnEffect(40000010, elem4.board.container.transform);
								gameObject40.transform.position = position2;
								PoolManager.Ins.DeSpawnEffect(gameObject40, 0.6f);
								gameObject40.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
								UnityEngine.Object.Destroy(elem4.UpXian);
							}
							if (elem4.DownXian != null)
							{
								GameObject gameObject41 = PoolManager.Ins.SpawnEffect(40000010, elem4.board.container.transform);
								gameObject41.transform.position = position2;
								PoolManager.Ins.DeSpawnEffect(gameObject41, 0.6f);
								gameObject41.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
								UnityEngine.Object.Destroy(elem4.DownXian);
							}
						}
						else
						{
							elem4.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[elem4.color];
							elem4.transform.DOScale(Vector3.one, 0.12f).SetEase(Ease.InCubic);
						}
					});
				}
				else if (currentRenderMessage.type == RenderMessageType.UpgradeTreasure)
				{
					Element element8 = (Element)currentRenderMessage.message;
					if (element8 != null && element8.color == 52)
					{
						AudioManager.Instance.PlayAudioEffect("JewelryBoxClose", 0.1f);
						DebugUtils.Log(DebugType.Other, "Upgrade Treasure!");
						element8.StandByBomb.GetComponent<StandByTreasureItem>().Play("close1", false);
						element8.StandByBomb.GetComponent<StandByTreasureItem>().HidePearl();
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.UpgradeNullTreasure)
				{
					Element element9 = (Element)currentRenderMessage.message;
					if (element9 != null && element9.color == 62)
					{
						AudioManager.Instance.PlayAudioEffect("JewelryBoxClose", 0.1f);
						DebugUtils.Log(DebugType.Other, "Upgrade Null Treasure!");
						element9.StandByBomb.GetComponent<StandByTreasureItem>().Play("close1", false);
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessTreasure)
				{
					ProcessTreasure obj7 = (ProcessTreasure)currentRenderMessage.message;
					Element element10 = obj7.element;
					Vector3 position10 = obj7.position;
					ElementType effectType = obj7.effectType;
					if (effectType <= ElementType.Treasure_0)
					{
						AudioManager.Instance.PlayAudioEffect("JewelryBoxBroken", 0.1f);
						element10.gameObject.SetActive(false);
						GameObject gameObject7 = PoolManager.Ins.SpawnEffect((int)effectType, element10.board.container.transform);
						gameObject7.transform.position = position10;
						PoolManager.Ins.DeSpawnEffect(gameObject7, 0.6f);
					}
					else
					{
						element10.isCanClose = false;
						AudioManager.Instance.PlayAudioEffect("JewelryBoxOpen ", 0.1f);
						element10.StandByBomb.GetComponent<StandByTreasureItem>().Play("open1", false);
						element10.StandByBomb.GetComponent<StandByTreasureItem>().ShowPearl();
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessNullTreasure)
				{
					ProcessTreasure obj8 = (ProcessTreasure)currentRenderMessage.message;
					Element element11 = obj8.element;
					Vector3 position11 = obj8.position;
					ElementType effectType2 = obj8.effectType;
					if (effectType2 <= ElementType.NullTreasure_0)
					{
						AudioManager.Instance.PlayAudioEffect("JewelryBoxBroken", 0.1f);
						element11.gameObject.SetActive(false);
						GameObject gameObject8 = PoolManager.Ins.SpawnEffect((int)effectType2, element11.board.container.transform);
						gameObject8.transform.position = position11;
						PoolManager.Ins.DeSpawnEffect(gameObject8, 0.6f);
					}
					else
					{
						element11.isCanClose = false;
						AudioManager.Instance.PlayAudioEffect("JewelryBoxOpen ", 0.1f);
						element11.StandByBomb.GetComponent<StandByTreasureItem>().Play("open1", false);
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessWhiteCloud)
				{
					ProcessWhiteCloud obj9 = (ProcessWhiteCloud)currentRenderMessage.message;
					Element element12 = obj9.element;
					Vector3 position12 = obj9.position;
					AudioManager.Instance.PlayAudioEffect("cloud_match", 0.1f);
					element12.gameObject.SetActive(false);
					GameObject gameObject9 = PoolManager.Ins.SpawnEffect(41, element12.board.container.transform);
					gameObject9.transform.position = position12;
					PoolManager.Ins.DeSpawnEffect(gameObject9, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ProcessBlackCloud)
				{
					ProcessBlackCloud obj10 = (ProcessBlackCloud)currentRenderMessage.message;
					Element element13 = obj10.element;
					Vector3 position13 = obj10.position;
					if (element13.color < 42)
					{
						element13.gameObject.SetActive(false);
					}
					else
					{
						element13.img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[element13.color];
					}
					AudioManager.Instance.PlayAudioEffect("cloud_match", 0.1f);
					GameObject gameObject10 = PoolManager.Ins.SpawnEffect(42, element13.board.container.transform);
					gameObject10.transform.position = position13;
					PoolManager.Ins.DeSpawnEffect(gameObject10, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ColorBombExplode)
				{
					ColorBombExplode cbe = (ColorBombExplode)currentRenderMessage.message;
					Vector3 selfPos = cbe.selfPos;
					int targetNum = cbe.bombList.Count - 1;
					ElementType type3 = cbe.type;
					Board board = cbe.board;
					GameObject gameObject11 = PoolManager.Ins.SpawnEffect(50000013, board.container.transform);
					gameObject11.transform.position = selfPos;
					PoolManager.Ins.DeSpawnEffect(gameObject11, 0.06f * (float)cbe.bombList.Count + 0.8f);
					float time8 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time8 > 0.06f * (float)cbe.bombList.Count + 0.8f)
						{
							GameObject gameObject36 = PoolManager.Ins.SpawnEffect(50000026, board.container.transform);
							gameObject36.transform.position = selfPos;
							PoolManager.Ins.DeSpawnEffect(gameObject36, 1f);
							return true;
						}
						time8 += duration;
						return false;
					}));
					float time = 0f;
					int num = 1;
					int index = 0;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time > 0.06f * (float)num)
						{
							num++;
							if (index > targetNum)
							{
								return true;
							}
							Vector3 final = cbe.bombList[index];
							Vector3 normalized = (final - selfPos).normalized;
							GameObject gameObject33 = PoolManager.Ins.SpawnEffect(50000024, board.container.transform);
							AudioManager.Instance.PlayAudioEffect("booster_crown");
							gameObject33.SetActive(false);
							Transform endPos7 = gameObject33.transform.Find("AAA");
							endPos7.localPosition = Vector3.zero;
							gameObject33.transform.position = selfPos;
							gameObject33.transform.right = normalized;
							gameObject33.SetActive(true);
							SkinnedMeshRenderer line = gameObject33.GetComponentInChildren<SkinnedMeshRenderer>();
							PoolManager.Ins.DeSpawnEffect(gameObject33, 0.8f, delegate
							{
								line.material.DOKill();
								endPos7.localPosition = Vector3.zero;
							});
							float time10 = 0f;
							line.material.SetTextureOffset("_MainTex", new Vector2(17f, 0f));
							line.material.DOGameTweenOffset(new Vector2(19f, 0f), 2f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
							Element element17 = ((type3 != ElementType.None) ? cbe.bombElementList[index] : null);
							endPos7.DOGameTweenMove(final, 9f).SetSpeedBased(true).OnUpdate(delegate
							{
								time10 += Time.deltaTime;
								line.material.SetTextureScale("_MainTex", new Vector2(time10, 1f));
							})
								.OnComplete(delegate
								{
									if (type3 != ElementType.None)
									{
										if (!board.cells[element17.row, element17.col].HaveBramble())
										{
											GameObject gameObject34 = PoolManager.Ins.SpawnEffect(50000025, board.container.transform);
											gameObject34.transform.position = final;
											PoolManager.Ins.DeSpawnEffect(gameObject34, 0.06f * (float)cbe.bombList.Count + 0.8f - (float)index * 0.06f);
										}
										ElementType changeToBomb = ((type3 != ElementType.RandomVorHBomb) ? type3 : ((UnityEngine.Random.Range(0, 2) == 0) ? ElementType.VerticalBomb : ElementType.HorizontalBomb));
										element17.moving = false;
										element17.removed = false;
										DealElementTool.RemoveElement(board, board.cells[element17.row, element17.col], false, true, false, Singleton<PlayGameData>.Instance().gameConfig.DropdelayTime, null, changeToBomb, ElementType.ColorBomb, true);
										if (!board.cells[element17.row, element17.col].HaveBramble() && board.cells[element17.row, element17.col].element.type == type3)
										{
											element17.moving = true;
											element17.removed = true;
										}
									}
									else
									{
										GameObject gameObject35 = PoolManager.Ins.SpawnEffect(50000025, board.container.transform);
										gameObject35.transform.position = final;
										PoolManager.Ins.DeSpawnEffect(gameObject35, 0.06f * (float)cbe.bombList.Count + 0.8f - (float)index * 0.06f);
									}
								});
							index++;
						}
						time += duration;
						return false;
					}));
				}
				else if (currentRenderMessage.type == RenderMessageType.AreaBombExplode)
				{
					BombExplode bombExplode = (BombExplode)currentRenderMessage.message;
					Vector3 position14 = bombExplode.position;
					AudioManager.Instance.PlayAudioEffect("booster_bomb", 0.1f);
					GameObject gameObject12 = PoolManager.Ins.SpawnEffect(50000012, bombExplode.parent);
					gameObject12.transform.position = position14;
					PoolManager.Ins.DeSpawnEffect(gameObject12, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.VBombExplode)
				{
					BombExplode bombExplode2 = (BombExplode)currentRenderMessage.message;
					Vector3 position15 = bombExplode2.position;
					AudioManager.Instance.PlayAudioEffect("booster_rocket", 0.1f);
					GameObject gameObject13 = PoolManager.Ins.SpawnEffect(50000015, bombExplode2.parent);
					gameObject13.transform.position = position15;
					PoolManager.Ins.DeSpawnEffect(gameObject13, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.CombinAreaAndVHExplode)
				{
					BombExplode bombExplode3 = (BombExplode)currentRenderMessage.message;
					Vector3 position16 = bombExplode3.position;
					AudioManager.Instance.PlayAudioEffect("boosters_bomb_double");
					GameObject gameObject14 = PoolManager.Ins.SpawnEffect(50000019, bombExplode3.parent);
					gameObject14.transform.position = position16;
					PoolManager.Ins.DeSpawnEffect(gameObject14, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.HBombExplode)
				{
					BombExplode bombExplode4 = (BombExplode)currentRenderMessage.message;
					Vector3 position17 = bombExplode4.position;
					AudioManager.Instance.PlayAudioEffect("booster_rocket", 0.1f);
					GameObject gameObject15 = PoolManager.Ins.SpawnEffect(50000014, bombExplode4.parent);
					gameObject15.transform.position = position17;
					PoolManager.Ins.DeSpawnEffect(gameObject15, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.CombineAreaBombExplode)
				{
					BombExplode bombExplode5 = (BombExplode)currentRenderMessage.message;
					Vector3 position18 = bombExplode5.position;
					AudioManager.Instance.PlayAudioEffect("boosters_bomb_double");
					GameObject gameObject16 = PoolManager.Ins.SpawnEffect(50000016, bombExplode5.parent);
					gameObject16.transform.position = position18;
					PoolManager.Ins.DeSpawnEffect(gameObject16, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.CombineColorBombExplode)
				{
					BombExplode bombExplode6 = (BombExplode)currentRenderMessage.message;
					Vector3 position19 = bombExplode6.position;
					AudioManager.Instance.PlayAudioEffect("boosters_crown_double");
					GameObject gameObject17 = PoolManager.Ins.SpawnEffect(50000017, bombExplode6.parent);
					gameObject17.transform.position = position19;
					PoolManager.Ins.DeSpawnEffect(gameObject17, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.CombinBeesExplode)
				{
					BombExplode bombExplode7 = (BombExplode)currentRenderMessage.message;
					Vector3 position20 = bombExplode7.position;
					AudioManager.Instance.PlayAudioEffect("boosters_bee_double");
					GameObject gameObject18 = PoolManager.Ins.SpawnEffect(50000018, bombExplode7.parent);
					gameObject18.transform.position = position20;
					PoolManager.Ins.DeSpawnEffect(gameObject18, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.DeActiveTipEffect)
				{
					Cell cell = (Cell)currentRenderMessage.message;
					if (cell.UpTipShow != null)
					{
						PoolManager.Ins.DeSpawnEffect(cell.UpTipShow);
					}
					if (cell.DownTipShow != null)
					{
						PoolManager.Ins.DeSpawnEffect(cell.DownTipShow);
					}
					if (cell.LeftTipShow != null)
					{
						PoolManager.Ins.DeSpawnEffect(cell.LeftTipShow);
					}
					if (cell.RightTipShow != null)
					{
						PoolManager.Ins.DeSpawnEffect(cell.RightTipShow);
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.RemoveElement)
				{
					RemoveElement removeElement = (RemoveElement)currentRenderMessage.message;
					Element elem5 = removeElement.element;
					Vector3 position3 = removeElement.position;
					if (PoolManager.Ins.PoolScriptsDic.ContainsKey((int)elem5.type))
					{
						elem5.gameObject.SetActive(true);
						UpdateManager.Instance.GetSequence().Append(elem5.transform.DOScale(new Vector2(0f, 0f), 0.15f).SetEase(Ease.Linear));
						UpdateManager.Instance.GetSequence().SetDelay(0.06f).OnComplete(delegate
						{
							GameObject gameObject32 = PoolManager.Ins.SpawnEffect((int)elem5.type, elem5.board.container.transform);
							gameObject32.transform.position = position3;
							PoolManager.Ins.DeSpawnEffect(gameObject32, 0.6f);
						});
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.MixElement)
				{
					Transform parent2 = ((MixElement)currentRenderMessage.message).parent;
					AudioManager.Instance.PlayAudioEffect("boosters_build");
					GameObject gameObject19 = PoolManager.Ins.SpawnEffect(50000004, parent2);
					gameObject19.transform.localPosition = Vector3.zero;
					PoolManager.Ins.DeSpawnEffect(gameObject19, 3f);
				}
				else if (currentRenderMessage.type == RenderMessageType.WhiteCloudCreate)
				{
					CloudCreate cloudCreate = (CloudCreate)currentRenderMessage.message;
					Vector3 position21 = cloudCreate.position;
					GameObject gameObject20 = PoolManager.Ins.SpawnEffect(40000001, cloudCreate.parent);
					gameObject20.transform.position = position21;
					PoolManager.Ins.DeSpawnEffect(gameObject20, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.BlackCloudCreate)
				{
					CloudCreate cloudCreate2 = (CloudCreate)currentRenderMessage.message;
					Vector3 position22 = cloudCreate2.position;
					GameObject gameObject21 = PoolManager.Ins.SpawnEffect(40000002, cloudCreate2.parent);
					gameObject21.transform.position = position22;
					PoolManager.Ins.DeSpawnEffect(gameObject21, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.BlackCloudLevelUp)
				{
					CloudCreate cloudCreate3 = (CloudCreate)currentRenderMessage.message;
					Vector3 position23 = cloudCreate3.position;
					AudioManager.Instance.PlayAudioEffect("cloud_up", 0.1f);
					GameObject gameObject22 = PoolManager.Ins.SpawnEffect(40000003, cloudCreate3.parent);
					gameObject22.transform.position = position23;
					PoolManager.Ins.DeSpawnEffect(gameObject22, 0.6f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ShowOrHideSprite)
				{
					ShowOrHideSprite showOrHideSprite = (ShowOrHideSprite)currentRenderMessage.message;
					showOrHideSprite.renderer.enabled = showOrHideSprite.flag;
				}
				else if (currentRenderMessage.type == RenderMessageType.ChangeSprite)
				{
					ChangeSprite changeSprite = (ChangeSprite)currentRenderMessage.message;
					changeSprite.renderer.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[changeSprite.sprite];
				}
				else if (currentRenderMessage.type == RenderMessageType.GrassCreate)
				{
					GrassCreate grassCreate = (GrassCreate)currentRenderMessage.message;
					GameObject gameObject23 = PoolManager.Ins.SpawnEffect(600, grassCreate.parent);
					gameObject23.transform.position = grassCreate.position;
					PoolManager.Ins.DeSpawnEffect(gameObject23, 1f);
				}
				else if (currentRenderMessage.type == RenderMessageType.StopTip)
				{
					foreach (Cell item in (List<Cell>)currentRenderMessage.message)
					{
						if (item.seq != null)
						{
							item.seq.Kill();
							item.seq = null;
						}
						if (item.UpTipShow != null)
						{
							PoolManager.Ins.DeSpawnEffect(item.UpTipShow);
						}
						if (item.DownTipShow != null)
						{
							PoolManager.Ins.DeSpawnEffect(item.DownTipShow);
						}
						if (item.LeftTipShow != null)
						{
							PoolManager.Ins.DeSpawnEffect(item.LeftTipShow);
						}
						if (item.RightTipShow != null)
						{
							PoolManager.Ins.DeSpawnEffect(item.RightTipShow);
						}
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.ShowStandbyEffect)
				{
					ShowStandbyEffect showStandbyEffect = (ShowStandbyEffect)currentRenderMessage.message;
					ElementType type5 = showStandbyEffect.type;
					Transform parent3 = showStandbyEffect.parent;
					bool isShowAnim = showStandbyEffect.isShowAnim;
					GameObject StandByBomb = null;
					if (parent3 == null || (type5 >= ElementType.Standard_0 && type5 <= ElementType.Standard_6))
					{
						continue;
					}
					parent3.transform.DOKill();
					if (showStandbyEffect.elem.StandByBomb != null)
					{
						PoolManager.Ins.DeSpawnEffect(showStandbyEffect.elem.StandByBomb);
					}
					SpriteRenderer component = parent3.GetComponent<SpriteRenderer>();
					switch (type5)
					{
					case ElementType.AreaBomb:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000003, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.HorizontalBomb:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000005, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.VerticalBomb:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000006, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.Treasure_0:
					case ElementType.Treasure_1:
					case ElementType.NullTreasure_0:
					case ElementType.NullTreasure_1:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000007, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						StandByBomb.GetComponent<StandByTreasureItem>().PeralActive(false);
						if (type5 == ElementType.Treasure_0 || type5 == ElementType.NullTreasure_0)
						{
							StandByBomb.GetComponent<StandByTreasureItem>().Play("open2", false);
							if (type5 == ElementType.Treasure_0)
							{
								StandByBomb.GetComponent<StandByTreasureItem>().PeralActive(true);
							}
						}
						else
						{
							StandByBomb.GetComponent<StandByTreasureItem>().Play("close2", false);
						}
						break;
					case ElementType.Jewel:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000027, parent3.transform);
						component.enabled = true;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.FlyBomb:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000028, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.ColorBomb:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000034, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.Shell:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000035, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.Cat:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000080, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					case ElementType.Fish:
						StandByBomb = PoolManager.Ins.SpawnEffect(50000081, parent3.transform);
						component.enabled = false;
						StandByBomb.transform.localPosition = Vector3.zero;
						break;
					}
					if (StandByBomb != null && isShowAnim)
					{
						StandByBomb.transform.DOGameLocalPunchScale(Vector3.one * 0.5f, 0.5f, 4).OnKill(delegate
						{
							if (StandByBomb != null)
							{
								StandByBomb.transform.localScale = Vector3.one;
							}
						});
					}
					showStandbyEffect.elem.StandByBomb = StandByBomb;
				}
				else if (currentRenderMessage.type == RenderMessageType.CollectStandardElement)
				{
					CollectStandardElement collectStandardElement = (CollectStandardElement)currentRenderMessage.message;
					Vector3 initPos = collectStandardElement.InitPos;
					int type4 = collectStandardElement.type;
					Vector3 EndPos = collectStandardElement.EndPos;
					float delay3 = collectStandardElement.delay;
					Board board2 = collectStandardElement.board;
					Transform topElement2 = ElementGenerator.Instance.Create(board2, type4, 999, 999).transform;
					topElement2.position = initPos;
					topElement2.GetComponentInChildren<BoxCollider2D>().enabled = false;
					SpriteRenderer componentInChildren = topElement2.GetComponentInChildren<SpriteRenderer>();
					componentInChildren.sortingLayerName = "UI";
					componentInChildren.sortingOrder = collectStandardElement.layer;
					componentInChildren.maskInteraction = SpriteMaskInteraction.None;
					Vector3 position24 = topElement2.transform.position;
					EndPos.z = 0f;
					position24.z = 0f;
					Vector3 newVec = Vector3.right;
					float time6 = 0f;
					GameLogic.Instance.isFinish = ProcessTool.isFinishCheck(board2, type4 - 1);
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (topElement2 == null)
						{
							return true;
						}
						Vector3 position30 = topElement2.transform.position;
						position30.z = 0f;
						Vector3 b = EndPos - position30;
						b.Normalize();
						newVec = Vector3.Lerp(Vector3.right, b, time6);
						topElement2.transform.localScale = new Vector3(curveScale.Evaluate(time6), curveScale.Evaluate(time6), 0f);
						topElement2.transform.position += newVec * curve.Evaluate(time6);
						if (Vector3.Distance(topElement2.transform.position, EndPos) < 0.5f)
						{
							ProcessTool.Statements(board2, type4 - 1);
							UnityEngine.Object.Destroy(topElement2.gameObject);
							return true;
						}
						time6 += duration;
						return false;
					}));
				}
				else if (currentRenderMessage.type == RenderMessageType.CollectVaseElement)
				{
					CollectVaseElement collectVaseElement = (CollectVaseElement)currentRenderMessage.message;
					Transform vase = collectVaseElement.vase;
					Vector3 endPos = collectVaseElement.EndPos;
					float delay = collectVaseElement.delay;
					float para = collectVaseElement.para;
					vase.gameObject.SetActive(true);
					vase.GetComponentInChildren<BoxCollider2D>().enabled = false;
					SpriteRenderer componentInChildren2 = vase.GetComponentInChildren<SpriteRenderer>();
					componentInChildren2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
					componentInChildren2.sortingLayerName = "UI";
					componentInChildren2.sortingOrder = 1;
					componentInChildren2.maskInteraction = SpriteMaskInteraction.None;
					vase.DOGameTweenMove(vase.position + new Vector3(0f, 1f, 0f), 0.2f);
					vase.DOGameLocalPunchRotation(new Vector3(0f, 0f, 20f), 1.4f, 4).SetDelay(0.1f);
					vase.DOGameTweenScale(Vector3.one * Singleton<PlayGameData>.Instance().gameConfig.rate, 0.2f);
					vase.DOGameTweenMove(endPos, delay).SetEase(Ease.InCubic).SetDelay(0.95f);
					vase.DOGameTweenScale(Vector3.one * para, delay).SetEase(Ease.InBack).SetDelay(0.95f);
					AudioManager.Instance.PlayAudioEffect("collect_vase", 0.1f);
					float time2 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time2 > delay + 0.95f)
						{
							if (vase == null)
							{
								DebugUtils.Log(DebugType.Other, "@@@@@@@@@@@@@@@@@@@");
							}
							else
							{
								UnityEngine.Object.Destroy(vase.gameObject);
							}
							return true;
						}
						time2 += duration;
						return false;
					}));
				}
				else if (currentRenderMessage.type == RenderMessageType.CollectPearl)
				{
					CollectVaseElement collectVaseElement2 = (CollectVaseElement)currentRenderMessage.message;
					Vector3 endPos2 = collectVaseElement2.EndPos;
					float delay2 = collectVaseElement2.delay;
					float para2 = collectVaseElement2.para;
					Board board4 = collectVaseElement2.board;
					Transform vase2 = collectVaseElement2.vase;
					Transform pearl = PoolManager.Ins.SpawnEffect(50000008, board4.transform).transform;
					pearl.position = vase2.position;
					pearl.localScale = Vector3.one;
					pearl.rotation = Quaternion.Euler(0f, 0f, 0f);
					pearl.DOGameTweenMove(pearl.position + new Vector3(0f, 1f, 0f), 0.2f);
					pearl.DOGameLocalPunchRotation(new Vector3(0f, 0f, 20f), 1.4f, 4).SetDelay(0.1f);
					pearl.DOGameTweenScale(Vector3.one * Singleton<PlayGameData>.Instance().gameConfig.rate, 0.2f);
					pearl.DOGameTweenMove(endPos2, delay2).SetEase(Ease.InCubic).SetDelay(0.95f);
					pearl.DOGameTweenScale(Vector3.one * para2, delay2).SetEase(Ease.InBack).SetDelay(0.95f);
					AudioManager.Instance.PlayAudioEffect("collect_vase", 0.1f);
					float time3 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time3 > delay2 + 0.95f)
						{
							if (pearl == null)
							{
								DebugUtils.Log(DebugType.Other, "@@@@@@@@@@@@@@@@@@@");
							}
							else
							{
								PoolManager.Ins.DeSpawnEffect(pearl.gameObject);
							}
							return true;
						}
						time3 += duration;
						return false;
					}));
				}
				else if (currentRenderMessage.type == RenderMessageType.StepToBomb)
				{
					StepToBomb stepToBomb = (StepToBomb)currentRenderMessage.message;
					Vector3 one = stepToBomb.one;
					Vector3 two = stepToBomb.two;
					float time4 = stepToBomb.time;
					Transform parent = stepToBomb.parent;
					GameObject elem6 = PoolManager.Ins.SpawnEffect(50000029, parent, false);
					elem6.transform.position = one;
					elem6.transform.rotation = Quaternion.EulerRotation(0f, 0f, 0f);
					elem6.gameObject.SetActive(true);
					TrailRenderer[] componentsInChildren = elem6.GetComponentsInChildren<TrailRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].emitting = true;
					}
					PoolManager.Ins.DeSpawnEffect(elem6, time4 + 0.1f, delegate
					{
						TrailRenderer[] componentsInChildren5 = elem6.GetComponentsInChildren<TrailRenderer>();
						foreach (TrailRenderer obj16 in componentsInChildren5)
						{
							obj16.emitting = false;
							obj16.Clear();
						}
					});
					AudioManager.Instance.PlayAudioEffect("moves_to_booster_fly");
					elem6.transform.DOGameTweenMove(two, time4);
					float time7 = 0f;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time7 > time4)
						{
							AudioManager.Instance.PlayAudioEffect("to_boosters");
							GameObject gameObject31 = PoolManager.Ins.SpawnEffect(50000030, parent);
							gameObject31.transform.position = two;
							PoolManager.Ins.DeSpawnEffect(gameObject31, 2f);
							return true;
						}
						time7 += duration;
						return false;
					}));
				}
				else if (currentRenderMessage.type == RenderMessageType.SwitchElement)
				{
					SwitchElement obj11 = (SwitchElement)currentRenderMessage.message;
					Vector3 one2 = obj11.one;
					Vector3 two2 = obj11.two;
					Transform parent4 = obj11.parent;
					GameObject gameObject24 = PoolManager.Ins.SpawnEffect(50000001, parent4);
					gameObject24.transform.localPosition = (one2 + two2) / 2f;
					gameObject24.transform.up = (one2 - two2).normalized;
					AudioManager.Instance.PlayAudioEffect("elements_moving");
					ParticleSystem[] componentsInChildren2 = gameObject24.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						componentsInChildren2[i].playbackSpeed = 4f;
					}
					PoolManager.Ins.DeSpawnEffect(gameObject24, 1f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ShakeOneElement)
				{
					ShakeOneElement obj12 = (ShakeOneElement)currentRenderMessage.message;
					float duration2 = obj12.duration;
					Transform target = obj12.target;
					float strength = obj12.strength;
					target.DOGameLocalShakeScale(duration2, strength);
				}
				else if (currentRenderMessage.type == RenderMessageType.SingleBeeExplod)
				{
					SingleBeeExplod singleBeeExplod = (SingleBeeExplod)currentRenderMessage.message;
					Transform parent5 = singleBeeExplod.parent;
					Board board5 = singleBeeExplod.board;
					ElementType type6 = singleBeeExplod.type;
					EffectType effectType3 = EffectType.BeeStartAttack;
					EffectType effectType4 = EffectType.Fly;
					switch (type6)
					{
					case ElementType.HorizontalBomb:
					case ElementType.VerticalBomb:
						effectType3 = EffectType.FlyAndVHBombStartAttack;
						effectType4 = EffectType.FlyAndVHBombFly;
						break;
					case ElementType.AreaBomb:
						effectType3 = EffectType.FlyAndAreaBombStartAttack;
						effectType4 = EffectType.FlyAndAreaBombFly;
						break;
					}
					Vector3 effectPos = singleBeeExplod.EffectPos;
					GameObject gameObject25 = PoolManager.Ins.SpawnEffect((int)effectType3, board5.container.transform);
					gameObject25.transform.position = effectPos;
					PoolManager.Ins.DeSpawnEffect(gameObject25, 1f);
					AudioManager.Instance.PlayAudioEffect("booster_bee_fly", 0.05f);
					GameObject elem7 = PoolManager.Ins.SpawnEffect((int)effectType4, parent5, false);
					TrailRenderer[] componentsInChildren = elem7.GetComponentsInChildren<TrailRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].Clear();
					}
					elem7.transform.localPosition = singleBeeExplod.localPostion;
					elem7.SetActive(true);
					Transform mid = elem7.transform.GetChild(0);
					Transform child = mid.transform.GetChild(0);
					elem7.transform.rotation = Quaternion.Euler(Vector3.zero);
					child.transform.rotation = Quaternion.Euler(Vector3.zero);
					elem7.name = "flybomb1_" + singleBeeExplod.rAc.x + "_" + singleBeeExplod.rAc.y;
					Vector3 endPos3 = singleBeeExplod.endPos;
					bool flag = false;
					bool flag2 = false;
					if (endPos3.x < elem7.transform.localPosition.x)
					{
						flag = true;
						elem7.transform.RotateAround(elem7.transform.up, (float)Math.PI);
					}
					float num2 = Vector3.Angle(elem7.transform.right, endPos3 - elem7.transform.localPosition);
					if (endPos3.y > elem7.transform.localPosition.y)
					{
						flag2 = true;
					}
					if ((flag && !flag2) || (!flag && !flag2))
					{
						num2 = 0f - num2;
					}
					elem7.transform.RotateAround(elem7.transform.forward, num2 * (float)Math.PI / 180f);
					int num3 = Mathf.RoundToInt(Vector3.Distance(elem7.transform.localPosition, endPos3));
					float maxValue = (float)num3 * (float)Math.PI;
					float time5 = 0f;
					float initSpeed = singleBeeExplod.initSpeed;
					float totalTime = (float)num3 / initSpeed;
					float value = 0f;
					float yValue = 0f;
					float gradient = 0f;
					float preValue = 0f;
					float preyValue = 0f;
					Vector3 vDir = (endPos3 - elem7.transform.localPosition).normalized;
					Vector3 startPos = elem7.transform.localPosition;
					UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
					{
						if (time5 > totalTime)
						{
							elem7.SetActive(false);
							mid.transform.localPosition = Vector3.zero;
							PoolManager.Ins.DeSpawnEffect(elem7);
							return true;
						}
						time5 += duration;
						value = Mathf.Lerp(0f, maxValue, time5 / totalTime);
						yValue = Mathf.Sin(value);
						gradient = (yValue - preyValue) / (value - preValue);
						mid.transform.localPosition = new Vector3(0f, yValue * 0.3f, 0f);
						preValue = value;
						preyValue = yValue;
						elem7.transform.localPosition = startPos + initSpeed * time5 * vDir;
						return false;
					}));
					elem7.transform.DOGameTweenScale(Vector3.one, totalTime);
				}
				else if (currentRenderMessage.type == RenderMessageType.BeeHitOne)
				{
					BeeHitOne beeHitOne = (BeeHitOne)currentRenderMessage.message;
					EffectType effectType5 = EffectType.BeeHit;
					switch (beeHitOne.type)
					{
					case ElementType.HorizontalBomb:
					case ElementType.VerticalBomb:
						effectType5 = EffectType.FlyAndVHBombHit;
						break;
					case ElementType.AreaBomb:
						effectType5 = EffectType.FlyAndAreaBombHit;
						break;
					}
					GameObject gameObject26 = PoolManager.Ins.SpawnEffect((int)effectType5, beeHitOne.parent);
					AudioManager.Instance.PlayAudioEffect("booster_bee_hit");
					gameObject26.transform.position = beeHitOne.startPos;
					PoolManager.Ins.DeSpawnEffect(gameObject26, 1f);
				}
				else if (currentRenderMessage.type == RenderMessageType.CreateOneMoveToBomb)
				{
					CreateOneMoveToBomb createOneMoveToBomb = (CreateOneMoveToBomb)currentRenderMessage.message;
					Board board6 = createOneMoveToBomb.board;
					int color = createOneMoveToBomb.color;
					Vector3 position25 = createOneMoveToBomb.item.cell.transform.position;
					Vector3 endPos4 = createOneMoveToBomb.endPos;
					float time9 = createOneMoveToBomb.time;
					if (createOneMoveToBomb.item.cell.element != null)
					{
						createOneMoveToBomb.item.cell.element.transform.localScale = Vector3.zero;
					}
					Transform topElement = ElementGenerator.Instance.Create(board6, color, 999, 999).transform;
					topElement.position = position25;
					topElement.GetComponentInChildren<BoxCollider2D>().enabled = false;
					topElement.GetComponentInChildren<SpriteRenderer>().sortingOrder--;
					topElement.DOGameTweenScale(new Vector3(0.8f, 0.8f, 0.8f), time9);
					topElement.DOGameTweenMove(endPos4, time9).OnComplete(delegate
					{
						topElement.gameObject.SetActive(false);
						UnityEngine.Object.Destroy(topElement.gameObject);
					});
				}
				else if (currentRenderMessage.type == RenderMessageType.RefreshElement)
				{
					Board board7 = (Board)currentRenderMessage.message;
					Cell[,] cells = board7.cells;
					GameObject effect = PoolManager.Ins.SpawnEffect(50000031, board7.container.transform);
					AudioManager.Instance.PlayAudioEffect("reshuffle_in");
					effect.transform.position = Vector3.up - Vector3.up * 5f;
					effect.transform.DOGameTweenMove(Vector3.zero, 0.4f).OnComplete(delegate
					{
						effect.transform.DOMove(Vector3.zero + Vector3.up * 5f, 0.4f).SetDelay(0.5f).OnComplete(delegate
						{
							PoolManager.Ins.DeSpawnEffect(effect);
						});
					});
					for (int j = board7.levelRowStart; j <= board7.levelRowEnd; j++)
					{
						for (int k = board7.levelColStart; k <= board7.levelColEnd; k++)
						{
							Cell cell2 = cells[j, k];
							Element element = cell2.element;
							if (!cell2.empty && !cell2.Blocked() && !cell2.HaveJewel() && !cell2.HaveTreasure() && !cell2.HaveShell() && !(cell2.element == null))
							{
								element.img.transform.DOGameTweenMove(Vector3.zero, 9f).SetSpeedBased(true).SetEase(Ease.InCubic)
									.OnComplete(delegate
									{
										element.img.SetActive(false);
									});
							}
						}
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.FinishRefreshElement)
				{
					AudioManager.Instance.PlayAudioEffect("reshuffle_out");
					Board board8 = (Board)currentRenderMessage.message;
					Cell[,] cells2 = board8.cells;
					for (int l = board8.levelRowStart; l <= board8.levelRowEnd; l++)
					{
						for (int m = board8.levelColStart; m <= board8.levelColEnd; m++)
						{
							Cell cell3 = cells2[l, m];
							Element element14 = cell3.element;
							if (!cell3.empty && !cell3.Blocked() && !cell3.HaveJewel() && !cell3.HaveTreasure() && !cell3.HaveShell() && !(cell3.element == null))
							{
								element14.img.SetActive(true);
								element14.img.transform.DOGameTweenLocalMove(Vector3.zero, 11f).SetSpeedBased(true).SetEase(Ease.InCubic)
									.OnComplete(delegate
									{
									});
							}
						}
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.CollectSkillPower)
				{
					CollectSkillPower collectSkillPower = (CollectSkillPower)currentRenderMessage.message;
					Vector3 startPos2 = collectSkillPower.startPos;
					Vector3 endPos5 = collectSkillPower.endPos;
					ElementType type7 = collectSkillPower.type;
					endPos5.z = -7f;
					float flyTime = collectSkillPower.flyTime;
					float speed = collectSkillPower.speed;
					AudioManager.Instance.PlayAudioEffect("book_energy", 0.1f);
					GameObject elem2 = PoolManager.Ins.SpawnEffect(50000047, collectSkillPower.parent, false);
					elem2.GetComponent<Renderer>().enabled = true;
					Color startColor = Color.white;
					switch (type7)
					{
					case ElementType.Standard_0:
						startColor = new Color(0f, 255f, 6f, 127f);
						break;
					case ElementType.Standard_1:
						startColor = new Color(255f, 255f, 255f, 127f);
						break;
					case ElementType.Standard_2:
						startColor = new Color(0f, 255f, 255f, 127f);
						break;
					case ElementType.Standard_3:
						startColor = new Color(255f, 94f, 0f, 127f);
						break;
					case ElementType.Standard_4:
						startColor = new Color(253f, 18f, 4f, 127f);
						break;
					case ElementType.Standard_5:
						startColor = new Color(255f, 251f, 0f, 127f);
						break;
					}
					startColor *= 0.003921569f;
					ParticleSystem[] componentsInChildren2 = elem2.GetComponentsInChildren<ParticleSystem>();
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						componentsInChildren2[i].startColor = startColor;
					}
					elem2.transform.position = startPos2;
					elem2.transform.rotation = Quaternion.EulerRotation(0f, 0f, 0f);
					elem2.gameObject.SetActive(true);
					TrailRenderer[] componentsInChildren = elem2.GetComponentsInChildren<TrailRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].emitting = true;
					}
					PoolManager.Ins.DeSpawnEffect(elem2, flyTime + 1f, delegate
					{
						elem2.GetComponent<Renderer>().enabled = true;
						TrailRenderer[] componentsInChildren4 = elem2.GetComponentsInChildren<TrailRenderer>();
						foreach (TrailRenderer obj15 in componentsInChildren4)
						{
							obj15.emitting = false;
							obj15.Clear();
						}
					});
					ExtendMethod.DOGameLocalPath(path: new Vector3[3]
					{
						startPos2,
						startPos2 + new Vector3(-1.2f, 1.2f, 0f),
						endPos5
					}, transform: elem2.transform, duration: flyTime, pathType: PathType.CatmullRom).SetEase(skillCollect).OnComplete(delegate
					{
						elem2.GetComponent<Renderer>().enabled = false;
					});
				}
				else if (currentRenderMessage.type == RenderMessageType.ActiveSkill)
				{
					CollectSkillPower collectSkillPower2 = (CollectSkillPower)currentRenderMessage.message;
					Vector3 startPos3 = collectSkillPower2.startPos;
					Vector3 endPos6 = collectSkillPower2.endPos;
					float flyTime2 = collectSkillPower2.flyTime;
					GameObject elem = PoolManager.Ins.SpawnEffect(50000029, collectSkillPower2.parent, false);
					startPos3.z = -7f;
					elem.transform.position = startPos3;
					elem.transform.rotation = Quaternion.EulerRotation(0f, 0f, 0f);
					elem.gameObject.SetActive(true);
					TrailRenderer[] componentsInChildren = elem.GetComponentsInChildren<TrailRenderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].emitting = true;
					}
					PoolManager.Ins.DeSpawnEffect(elem, flyTime2 + 0.1f, delegate
					{
						TrailRenderer[] componentsInChildren3 = elem.GetComponentsInChildren<TrailRenderer>();
						foreach (TrailRenderer obj14 in componentsInChildren3)
						{
							obj14.emitting = false;
							obj14.Clear();
						}
					});
					elem.transform.DOGameTweenMove(endPos6, flyTime2).SetEase(Ease.Linear);
				}
				else if (currentRenderMessage.type == RenderMessageType.VHExploed)
				{
					BombExplode bombExplode8 = (BombExplode)currentRenderMessage.message;
					Vector3 position26 = bombExplode8.position;
					GameObject gameObject27 = PoolManager.Ins.SpawnEffect(50000017, bombExplode8.parent);
					gameObject27.transform.position = position26;
					PoolManager.Ins.DeSpawnEffect(gameObject27, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.ActiveShell)
				{
					ActiveShellInfo activeShellInfo = (ActiveShellInfo)currentRenderMessage.message;
					if (activeShellInfo.element != null && activeShellInfo.element.img.GetComponentInChildren<BoxCollider>() != null)
					{
						activeShellInfo.element.img.GetComponentInChildren<BoxCollider>().enabled = activeShellInfo.isActive;
					}
				}
				else if (currentRenderMessage.type == RenderMessageType.CreateOneElementByDrop)
				{
					Vector3 position27 = (Vector3)currentRenderMessage.message;
					AudioManager.Instance.PlayAudioEffect("to_boosters");
					GameObject gameObject28 = PoolManager.Ins.SpawnEffect(50000042);
					gameObject28.transform.position = position27;
					PoolManager.Ins.DeSpawnEffect(gameObject28, 2f);
				}
				else if (currentRenderMessage.type == RenderMessageType.HammerHit)
				{
					Vector3 position28 = (Vector3)currentRenderMessage.message;
					AudioManager.Instance.PlayAudioEffect("booster5_hammer");
					GameObject gameObject29 = PoolManager.Ins.SpawnEffect(50000043);
					PoolManager.Ins.DeSpawnEffect(gameObject29, 2f);
					position28.z = -7f;
					gameObject29.transform.position = position28;
				}
				else if (currentRenderMessage.type == RenderMessageType.SpoonHit)
				{
					Vector3 position29 = (Vector3)currentRenderMessage.message;
					AudioManager.Instance.PlayAudioEffect("booster4_spoon");
					GameObject gameObject30 = PoolManager.Ins.SpawnEffect(50000044);
					PoolManager.Ins.DeSpawnEffect(gameObject30, 2f);
					position29.z = -7f;
					gameObject30.transform.position = position29;
				}
				else if (currentRenderMessage.type == RenderMessageType.ControllBook)
				{
					bool flag3 = (bool)currentRenderMessage.message;
					GameSceneUIManager.Instance.AnimBook["shu"].speed = 0.5f;
					GameSceneUIManager.Instance.AnimBook["shu 2"].speed = 0.5f;
					GameSceneUIManager.Instance.AnimBook.Play(flag3 ? "shu" : "shu 2");
				}
				else if (currentRenderMessage.type == RenderMessageType.GloveMove)
				{
					GloveMove obj13 = (GloveMove)currentRenderMessage.message;
					Element element15 = obj13.element1;
					Element element16 = obj13.element2;
					GameObject spoon = PoolManager.Ins.SpawnEffect(50000046, element15.board.container.transform, false);
					spoon.transform.position = element15.transform.position;
					spoon.transform.localScale = Vector3.one * 4f;
					spoon.transform.GetComponent<SpriteRenderer>().DOGameTweenFade(1f, 0f);
					spoon.SetActive(true);
					float num4 = 0.5f;
					float duration3 = 0.2f;
					float duration4 = 0.5f;
					Sequence sequence = UpdateManager.Instance.GetSequence();
					sequence.Append(spoon.transform.DOScale(Vector3.one, duration4));
					sequence.Append(spoon.transform.DOMove(element16.transform.position, duration3));
					sequence.Append(spoon.transform.DOScale(Vector3.one * 3f, num4));
					sequence.Join(spoon.transform.GetComponent<SpriteRenderer>().DOFade(0f, num4 - 0.1f).OnComplete(delegate
					{
						PoolManager.Ins.DeSpawnEffect(spoon);
					}));
				}
			}
		}
		currentRenderMessageList.Clear();
	}

	private void ClearRenderList()
	{
		if (renderMessageList.Count != 0)
		{
			renderMessageList.Clear();
		}
	}

	public void AddMessageToRenderList(RenderMessageEvent message)
	{
		renderMessageList.Add(message);
	}
}
