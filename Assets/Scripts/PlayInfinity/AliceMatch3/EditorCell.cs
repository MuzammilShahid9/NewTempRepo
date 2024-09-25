using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Editor
{
	public class EditorCell : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		public Text pathText;

		public int row;

		public int col;

		public int flag;

		public int pathFlag;

		public int transFlag;

		public int areaStartFlag;

		public int areaEndFlag;

		public int createrFlag;

		public int transporterFlag;

		public int catFlag;

		public int baseFlag;

		public int lastFlag;

		public int bottomFlag;

		public int middleFlag;

		public int topFlag;

		public int firstFlag;

		public EditorElement baseLayer;

		public EditorElement pathLayer;

		public EditorElement transLayer;

		public EditorElement lastLayer;

		public EditorElement bottomLayer;

		public EditorElement middleLayer;

		public EditorElement topLayer;

		public EditorElement firstLayer;

		public EditorElement createrLayer;

		public EditorElement areaStartLayer;

		public EditorElement areaEndLayer;

		public EditorElement transporterLayer;

		public EditorElement catLayer;

		public BoardData data;

		public void SetCellElement(int flagIndex)
		{
			ReSetCellElement();
			if (flagIndex < 0)
			{
				SetBaseElement(flagIndex);
			}
			else if (flagIndex != 0)
			{
				int num = flagIndex % 100;
				flagIndex -= num;
				SetMiddleElement(num);
				int num2 = flagIndex % 1000;
				flagIndex -= num2;
				SetBottomElement(num2);
				int num3 = flagIndex % 100000;
				flagIndex -= num3;
				SetTopElement(num3);
				int num4 = flagIndex % 10000000;
				flagIndex -= num4;
				SetLastElement(num4);
				int firstElement = flagIndex % 100000000;
				SetFirstElement(firstElement);
			}
			Editor.Instance.SetBoardCellData(col, row, flag);
		}

		public void ReSetCellElement()
		{
			flag = 0;
			pathFlag = 0;
			transFlag = 0;
			areaStartFlag = 0;
			areaEndFlag = 0;
			createrFlag = 0;
			baseFlag = 0;
			lastFlag = 0;
			bottomFlag = 0;
			middleFlag = 0;
			topFlag = 0;
			firstFlag = 0;
		}

		public void SetLastElement(int lastFlagTemp)
		{
			if (lastFlag == lastFlagTemp)
			{
				flag -= lastFlag;
				lastFlag = 0;
				if (lastLayer != null)
				{
					UnityEngine.Object.Destroy(lastLayer.gameObject);
				}
			}
			else if (lastFlagTemp != 0)
			{
				flag = flag - lastFlag + lastFlagTemp;
				lastFlag = lastFlagTemp;
				if (lastLayer != null)
				{
					UnityEngine.Object.Destroy(lastLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(lastFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				float x = 0f;
				float y = 0f;
				if (lastFlag == 1000000)
				{
					y = -64f;
				}
				else if (lastFlag == 1100000)
				{
					x = 64f;
					y = -192f;
				}
				else if (lastFlag == 1200000)
				{
					x = 128f;
					y = -320f;
				}
				else if (lastFlag == 1300000)
				{
					x = 64f;
				}
				else if (lastFlag == 1400000)
				{
					x = 192f;
					y = -64f;
				}
				else if (lastFlag == 1500000)
				{
					x = 320f;
					y = -128f;
				}
				gameObject.transform.localPosition = new Vector2(x, y);
				gameObject.transform.localScale = new Vector2(150f, 150f);
				gameObject.name = "LastElement";
				lastLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
			}
		}

		public void SetBottomElement(int bottomFlagTemp)
		{
			if (bottomFlag == bottomFlagTemp)
			{
				flag -= bottomFlag;
				bottomFlag = 0;
				if (bottomLayer != null)
				{
					UnityEngine.Object.Destroy(bottomLayer.gameObject);
				}
			}
			else if (bottomFlagTemp != 0)
			{
				flag = flag - bottomFlag + bottomFlagTemp;
				bottomFlag = bottomFlagTemp;
				if (bottomLayer != null)
				{
					UnityEngine.Object.Destroy(bottomLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(bottomFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.transform.localScale = new Vector2(150f, 150f);
				gameObject.name = "BottomElement";
				bottomLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3;
			}
		}

		public void SetMiddleElement(int middleFlagTemp)
		{
			if (middleFlag == middleFlagTemp)
			{
				flag -= middleFlag;
				middleFlag = 0;
				if (middleLayer != null)
				{
					UnityEngine.Object.Destroy(middleLayer.gameObject);
				}
			}
			else if (middleFlagTemp != 0)
			{
				flag = flag - middleFlag + middleFlagTemp;
				middleFlag = middleFlagTemp;
				if (middleLayer != null)
				{
					UnityEngine.Object.Destroy(middleLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(middleFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.transform.localScale = new Vector2(150f, 150f);
				gameObject.name = "MiddleElement";
				middleLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4;
			}
		}

		public void SetTopElement(int topFlagTemp)
		{
			if (topFlag == topFlagTemp)
			{
				flag -= topFlag;
				topFlag = 0;
				if (topLayer != null)
				{
					UnityEngine.Object.Destroy(topLayer.gameObject);
				}
			}
			else if (topFlagTemp != 0)
			{
				flag = flag - topFlag + topFlagTemp;
				topFlag = topFlagTemp;
				if (topLayer != null)
				{
					UnityEngine.Object.Destroy(topLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(topFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.transform.localScale = new Vector2(150f, 150f);
				gameObject.name = "TopElement";
				topLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 5;
			}
		}

		public void SetFirstElement(int firstFlagTemp)
		{
			if (firstFlag == firstFlagTemp)
			{
				flag -= firstFlag;
				firstFlag = 0;
				if (firstLayer != null)
				{
					UnityEngine.Object.Destroy(firstLayer.gameObject);
				}
			}
			else if (firstFlagTemp != 0)
			{
				flag = flag - firstFlag + firstFlagTemp;
				firstFlag = firstFlagTemp;
				if (firstLayer != null)
				{
					UnityEngine.Object.Destroy(firstLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(firstFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.transform.localScale = new Vector2(150f, 150f);
				gameObject.name = "FirstElement";
				firstLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 6;
			}
		}

		public void SetBaseElement(int baseFlagTemp)
		{
			if (baseFlag == baseFlagTemp)
			{
				flag -= baseFlag;
				baseFlag = 0;
				if (baseLayer != null)
				{
					UnityEngine.Object.Destroy(baseLayer.gameObject);
				}
			}
			else if (baseFlagTemp != 0)
			{
				flag = flag - baseFlag + baseFlagTemp;
				baseFlag = baseFlagTemp;
				if (baseLayer != null)
				{
					UnityEngine.Object.Destroy(baseLayer.gameObject);
				}
				GameObject gameObject = EditorElementGenerator.Instance.Create(baseFlag, row, col);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "BaseElement";
				baseLayer = gameObject.GetComponent<EditorElement>();
				gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
			}
			if (lastLayer != null)
			{
				lastFlag = 0;
				UnityEngine.Object.Destroy(lastLayer.gameObject);
				lastLayer = null;
			}
			if (bottomLayer != null)
			{
				bottomFlag = 0;
				UnityEngine.Object.Destroy(bottomLayer.gameObject);
				bottomLayer = null;
			}
			if (middleLayer != null)
			{
				middleFlag = 0;
				UnityEngine.Object.Destroy(middleLayer.gameObject);
				middleLayer = null;
			}
			if (topLayer != null)
			{
				topFlag = 0;
				UnityEngine.Object.Destroy(topLayer.gameObject);
				topLayer = null;
			}
			if (firstLayer != null)
			{
				firstFlag = 0;
				UnityEngine.Object.Destroy(firstLayer.gameObject);
				firstLayer = null;
			}
		}

		public void SetPathElement(int pathFlagTemp)
		{
			if (pathFlag == pathFlagTemp)
			{
				if (pathLayer != null)
				{
					Editor.Instance.DelBoardPathData(pathFlag);
				}
			}
			else if (pathFlagTemp != 0)
			{
				if (pathLayer != null)
				{
					Editor.Instance.DelBoardPathData(pathFlag);
				}
				pathFlag = pathFlagTemp;
				pathText.text = pathFlag + "-" + Editor.Instance.GetBoardPathNumber(pathFlag);
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.gameObject.SetActive(false);
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "PathElement";
				pathLayer = gameObject.GetComponent<EditorElement>();
				Editor.Instance.AddBoardPathData(col, row, pathFlagTemp);
			}
		}

		public void SetCatElement(int catFlagTemp)
		{
			if (catFlag == catFlagTemp)
			{
				if (catLayer != null)
				{
					Editor.Instance.DelBoardCatPathData(catFlag);
				}
			}
			else if (catFlagTemp != 0)
			{
				if (catLayer != null)
				{
					Editor.Instance.DelBoardCatPathData(catFlag);
				}
				catFlag = catFlagTemp;
				string text = pathFlag + "-" + Editor.Instance.GetBoardCatPathNumber(pathFlag);
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/cat", typeof(Sprite)) as Sprite;
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "CatPathElement";
				catLayer = gameObject.GetComponent<EditorElement>();
				switch (Editor.Instance.GetLastBoardCatPathDirData(col, row, catFlagTemp))
				{
				case Direction.Mix:
					gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/catStart", typeof(Sprite)) as Sprite;
					gameObject.GetComponent<EditorElement>().img.transform.localScale = Vector3.one * 0.5f;
					break;
				case Direction.Up:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				case Direction.Down:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
					break;
				case Direction.Left:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
					break;
				}
				Editor.Instance.AddBoardCatPathData(col, row, catFlagTemp);
			}
		}

		public void SetTransporterElement(int transporterFlagTemp)
		{
			if (transporterFlag == transporterFlagTemp)
			{
				if (transporterLayer != null)
				{
					Editor.Instance.DelBoardTransporterPathData(transporterFlag);
				}
			}
			else if (transporterFlagTemp != 0)
			{
				if (transporterLayer != null)
				{
					Editor.Instance.DelBoardTransporterPathData(transporterFlag);
				}
				transporterFlag = transporterFlagTemp;
				string text = pathFlag + "-" + Editor.Instance.GetBoardTransporterPathNumber(pathFlag);
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/transporter", typeof(Sprite)) as Sprite;
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "TransporterPathElement";
				transporterLayer = gameObject.GetComponent<EditorElement>();
				switch (Editor.Instance.GetLastBoardTransportPathDirData(col, row, transporterFlagTemp))
				{
				case Direction.Mix:
					gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/transporterStart", typeof(Sprite)) as Sprite;
					gameObject.GetComponent<EditorElement>().img.transform.localScale = Vector3.one * 0.5f;
					break;
				case Direction.Up:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					break;
				case Direction.Down:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
					break;
				case Direction.Left:
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
					break;
				}
				Editor.Instance.AddBoardTransportPathData(col, row, transporterFlagTemp);
			}
		}

		public void SetTransElement(int transFlagTemp)
		{
			if (transFlag == transFlagTemp)
			{
				if (transLayer != null)
				{
					Editor.Instance.DelBoardTransData(transFlag);
				}
			}
			else if (transFlagTemp != 0)
			{
				if (transLayer != null)
				{
					Editor.Instance.DelBoardTransData(transFlag);
				}
				transFlag = transFlagTemp;
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("s1/trans", typeof(Sprite)) as Sprite;
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "TransElement";
				transLayer = gameObject.GetComponent<EditorElement>();
				Editor.Instance.AddBoardTransData(col, row, transFlagTemp);
			}
		}

		public void SetAreaStartElement(int areaStartFlagTemp, bool noDataFlag)
		{
			if (areaStartFlag == areaStartFlagTemp)
			{
				Editor.Instance.RemoveBoardAreaData(areaStartFlag, 0);
				areaStartFlag = 0;
				if (areaStartLayer != null)
				{
					UnityEngine.Object.Destroy(areaStartLayer.gameObject);
				}
			}
			else if (areaStartFlagTemp != 0)
			{
				if (areaStartLayer != null)
				{
					UnityEngine.Object.Destroy(areaStartLayer.gameObject);
				}
				areaStartFlag = areaStartFlagTemp;
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/area", typeof(Sprite)) as Sprite;
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "AreaStartElement";
				areaStartLayer = gameObject.GetComponent<EditorElement>();
			}
			if (!noDataFlag)
			{
				Editor.Instance.SetBoardAreaData(col, row, areaStartFlag, areaEndFlag);
			}
		}

		public void SetAreaEndElement(int areaEndFlagTemp, bool noDataFlag)
		{
			if (areaEndFlag == areaEndFlagTemp)
			{
				Editor.Instance.RemoveBoardAreaData(0, areaEndFlag);
				areaEndFlag = 0;
				if (areaEndLayer != null)
				{
					UnityEngine.Object.Destroy(areaEndLayer.gameObject);
				}
			}
			else if (areaEndFlagTemp != 0)
			{
				if (areaEndLayer != null)
				{
					UnityEngine.Object.Destroy(areaEndLayer.gameObject);
				}
				areaEndFlag = areaEndFlagTemp;
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/Elements1/area", typeof(Sprite)) as Sprite;
				gameObject.transform.SetParent(base.transform);
				gameObject.transform.localPosition = new Vector2(0f, 0f);
				gameObject.name = "AreaEndElement";
				areaEndLayer = gameObject.GetComponent<EditorElement>();
			}
			if (!noDataFlag)
			{
				Editor.Instance.SetBoardAreaData(col, row, areaStartFlag, areaEndFlag);
			}
		}

		public void SetCreaterElement(int flag, int extra)
		{
			if (createrFlag != 0)
			{
				int row2 = row;
				int width = data.width;
				int col2 = col;
				createrFlag = 0;
				if (createrLayer != null)
				{
					UnityEngine.Object.Destroy(createrLayer.gameObject);
				}
			}
			else
			{
				createrFlag = flag;
				if (createrLayer != null)
				{
					UnityEngine.Object.Destroy(createrLayer.gameObject);
				}
				string text = "shnegchengqi" + flag;
				GameObject gameObject = UnityEngine.Object.Instantiate(EditorElementGenerator.Instance.ElementPrefab);
				gameObject.GetComponent<EditorElement>().row = row;
				gameObject.GetComponent<EditorElement>().col = col;
				try
				{
					gameObject.GetComponent<EditorElement>().img.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("Textures/GameElements/" + text).GetComponent<SpriteRenderer>().sprite;
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.localPosition = new Vector2(0f, 0f);
					gameObject.name = "CreaterElement";
					createrLayer = gameObject.GetComponent<EditorElement>();
				}
				catch (Exception)
				{
					DebugUtils.LogError(DebugType.Other, "No Picture");
				}
			}
			Editor.Instance.SetBoardCreaterData(col, row, flag, extra);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			DebugUtils.Log(DebugType.Other, "OnPointerDown");
			int currType = Editor.Instance.currType;
			int currElement = Editor.Instance.currElement;
			int currExtra = Editor.Instance.currExtra;
			if (currElement == 0)
			{
				Debug.Log("currFlag == 0");
				return;
			}
			switch (currType)
			{
			case 0:
				if (currElement < 0)
				{
					SetBaseElement(currElement);
				}
				else if (currElement < 100)
				{
					SetMiddleElement(currElement);
				}
				else if (currElement < 1000)
				{
					SetBottomElement(currElement);
				}
				else if (currElement < 100000)
				{
					SetTopElement(currElement);
				}
				else if (currElement < 10000000)
				{
					SetLastElement(currElement);
				}
				else if (currElement < 100000000)
				{
					SetFirstElement(currElement);
				}
				Editor.Instance.SetBoardCellData(col, row, flag);
				DebugUtils.Log(DebugType.Other, "Click EditorCell " + row + ", " + col + " flag " + flag);
				break;
			case 1:
				Editor.Instance.drawing = true;
				SetPathElement(currElement);
				break;
			case 2:
				Editor.Instance.drawing = true;
				SetTransElement(currElement);
				break;
			case 3:
				if (areaStartFlag == currElement)
				{
					SetAreaStartElement(currElement, false);
				}
				else if (areaEndFlag == currElement)
				{
					SetAreaEndElement(currElement, false);
				}
				else if (areaStartFlag == 0 && areaEndFlag == 0)
				{
					Dictionary<int, Area> dictionary = Editor.Instance.areaList;
					if (dictionary == null)
					{
						dictionary = new Dictionary<int, Area>();
					}
					if (!dictionary.ContainsKey(currElement))
					{
						dictionary.Add(currElement, new Area());
						dictionary[currElement].index = currElement;
					}
					if (dictionary[currElement].start == null)
					{
						SetAreaStartElement(currElement, false);
					}
					else
					{
						SetAreaEndElement(currElement, false);
					}
				}
				else if (areaStartFlag == 0 && areaEndFlag != 0)
				{
					SetAreaStartElement(currElement, false);
				}
				else if (areaStartFlag != 0 && areaEndFlag == 0)
				{
					SetAreaEndElement(currElement, false);
				}
				break;
			case 4:
				SetCreaterElement(currElement, currExtra);
				break;
			case 5:
				Editor.Instance.drawing = true;
				SetTransporterElement(currElement);
				break;
			case 6:
				Editor.Instance.drawing = true;
				SetCatElement(currElement);
				break;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (Editor.Instance.drawing)
			{
				int currType = Editor.Instance.currType;
				int currElement = Editor.Instance.currElement;
				switch (currType)
				{
				case 1:
					SetPathElement(currElement);
					break;
				case 2:
					SetTransElement(currElement);
					break;
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			DebugUtils.Log(DebugType.Other, "OnPointerUp");
			Editor.Instance.drawing = false;
			int currType = Editor.Instance.currType;
			int currElement = Editor.Instance.currElement;
			switch (currType)
			{
			case 1:
				Editor.Instance.PathDataToSaveData();
				break;
			case 2:
				Editor.Instance.TransDataToSaveData();
				break;
			case 5:
				Editor.Instance.TransporterPathDataToSaveData();
				break;
			case 6:
				Editor.Instance.CatPathDataToSaveData();
				break;
			}
		}
	}
}
