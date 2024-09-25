using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayInfinity.AliceMatch3.Core;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Editor
{
	public class Editor : MonoBehaviour
	{
		public InputField levelInput;

		public InputField widthInput;

		public InputField heightInput;

		public InputField pageInput;

		public InputField moveInput;

		public InputField[] markInput;

		public InputField[] targetInputList;

		public InputField[] probabilityInputList;

		public InputField[] skillProbabilityInputList;

		public InputField[] createrInputList;

		public Dropdown hardDrop;

		public Dropdown IceDrop;

		public Dropdown BoxDrop;

		public Dropdown CloudDrop;

		public Dropdown GreenBrambleDrop;

		public Dropdown RedBrambleDrop;

		public Dropdown HoneyDrop;

		public Dropdown CollectDrop;

		public Dropdown DoorDrop;

		public Dropdown ButtonDrop;

		public Dropdown TreasureDrop;

		public Dropdown NullTreasureDrop;

		public Text pageIndex;

		public GameObject boardRoot;

		public GameObject cellPrefab;

		public int currElement;

		public int currType;

		public int currExtra;

		public EditorCell[,] board;

		public List<EditorCell> cellList;

		public Dictionary<int, List<EditorCell>> pathList;

		public Dictionary<int, List<EditorCell>> transList;

		public Dictionary<int, List<EditorCell>> transporterPathList;

		public Dictionary<int, List<EditorCell>> catPathList;

		public Dictionary<int, Area> areaList;

		public Dictionary<int, Creater> createrList;

		public bool drawing;

		public LevelData levelData;

		private static Editor _instance;

		public static Editor Instance
		{
			get
			{
				return _instance;
			}
		}

		private void Awake()
		{
			_instance = this;
			GeneralConfig.Load();
		}

		private void Start()
		{
		}

		private IEnumerator AddLevel()
		{
			for (int i = 1; i <= 160; i++)
			{
				yield return new WaitForSeconds(0.01f);
				levelInput.text = string.Concat(i);
				BtnLoadClicked();
				yield return new WaitForSeconds(0.01f);
				BtnSaveClicked();
			}
		}

		public void RefreshBoardInfo(int index)
		{
			pageIndex.text = (index + 1).ToString();
			widthInput.text = levelData.boardData[index].width.ToString();
			heightInput.text = levelData.boardData[index].height.ToString();
		}

		public void RefreshLevelInfo()
		{
			pageInput.text = levelData.boardData.Length.ToString();
			moveInput.text = levelData.move.ToString();
			hardDrop.value = levelData.hard;
			for (int i = 0; i < levelData.probabilityList.Length; i++)
			{
				probabilityInputList[i].text = levelData.probabilityList[i].ToString();
			}
			for (int j = 0; j < levelData.skillProbabilityList.Length; j++)
			{
				skillProbabilityInputList[j].text = levelData.skillProbabilityList[j].ToString();
			}
			for (int k = 0; k < levelData.targetList.Length; k++)
			{
				targetInputList[k].text = levelData.targetList[k].ToString();
			}
		}

		public void SetBoardCellData(int col, int row, int flag)
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			boardData.map[row * boardData.width + col] = flag;
		}

		public void AddBoardPathData(int col, int row, int pathIndex)
		{
			if (pathList == null)
			{
				pathList = new Dictionary<int, List<EditorCell>>();
			}
			if (!pathList.ContainsKey(pathIndex))
			{
				pathList.Add(pathIndex, new List<EditorCell>());
			}
			pathList[pathIndex].Add(board[row, col]);
		}

		public void AddBoardTransportPathData(int col, int row, int pathIndex)
		{
			if (transporterPathList == null)
			{
				transporterPathList = new Dictionary<int, List<EditorCell>>();
			}
			if (!transporterPathList.ContainsKey(pathIndex))
			{
				transporterPathList.Add(pathIndex, new List<EditorCell>());
			}
			transporterPathList[pathIndex].Add(board[row, col]);
		}

		public void AddBoardCatPathData(int col, int row, int pathIndex)
		{
			if (catPathList == null)
			{
				catPathList = new Dictionary<int, List<EditorCell>>();
			}
			if (!catPathList.ContainsKey(pathIndex))
			{
				catPathList.Add(pathIndex, new List<EditorCell>());
			}
			catPathList[pathIndex].Add(board[row, col]);
		}

		public Direction GetLastBoardTransportPathDirData(int col, int row, int pathIndex)
		{
			if (transporterPathList == null)
			{
				transporterPathList = new Dictionary<int, List<EditorCell>>();
				return Direction.Mix;
			}
			if (!transporterPathList.ContainsKey(pathIndex))
			{
				transporterPathList.Add(pathIndex, new List<EditorCell>());
				return Direction.Mix;
			}
			if (transporterPathList.Count <= 0)
			{
				return Direction.Mix;
			}
			int index = transporterPathList[pathIndex].Count - 1;
			EditorCell editorCell = transporterPathList[pathIndex][index];
			int row2 = editorCell.row;
			int col2 = editorCell.col;
			if (row2 == row && col2 < col)
			{
				return Direction.Right;
			}
			if (row2 == row && col2 > col)
			{
				return Direction.Left;
			}
			if (row2 < row && col2 == col)
			{
				return Direction.Up;
			}
			if (row2 > row && col2 == col)
			{
				return Direction.Down;
			}
			return Direction.RightDown;
		}

		public Direction GetLastBoardCatPathDirData(int col, int row, int pathIndex)
		{
			if (catPathList == null)
			{
				catPathList = new Dictionary<int, List<EditorCell>>();
				return Direction.Mix;
			}
			if (!catPathList.ContainsKey(pathIndex))
			{
				catPathList.Add(pathIndex, new List<EditorCell>());
				return Direction.Mix;
			}
			if (catPathList.Count <= 0)
			{
				return Direction.Mix;
			}
			int index = catPathList[pathIndex].Count - 1;
			EditorCell editorCell = catPathList[pathIndex][index];
			int row2 = editorCell.row;
			int col2 = editorCell.col;
			if (row2 == row && col2 < col)
			{
				return Direction.Right;
			}
			if (row2 == row && col2 > col)
			{
				return Direction.Left;
			}
			if (row2 < row && col2 == col)
			{
				return Direction.Up;
			}
			if (row2 > row && col2 == col)
			{
				return Direction.Down;
			}
			return Direction.RightDown;
		}

		public int GetBoardPathNumber(int pathIndex)
		{
			if (pathList == null)
			{
				pathList = new Dictionary<int, List<EditorCell>>();
			}
			if (pathList.ContainsKey(pathIndex))
			{
				return pathList[pathIndex].Count;
			}
			return 0;
		}

		public int GetBoardTransporterPathNumber(int pathIndex)
		{
			if (transporterPathList == null)
			{
				transporterPathList = new Dictionary<int, List<EditorCell>>();
			}
			if (transporterPathList.ContainsKey(pathIndex))
			{
				return transporterPathList[pathIndex].Count;
			}
			return 0;
		}

		public int GetBoardCatPathNumber(int pathIndex)
		{
			if (catPathList == null)
			{
				catPathList = new Dictionary<int, List<EditorCell>>();
			}
			if (catPathList.ContainsKey(pathIndex))
			{
				return catPathList[pathIndex].Count;
			}
			return 0;
		}

		public void DelBoardPathData(int pathIndex)
		{
			if (!pathList.ContainsKey(pathIndex))
			{
				return;
			}
			foreach (EditorCell item in pathList[pathIndex])
			{
				Object.Destroy(item.pathLayer.gameObject);
				item.pathText.text = "";
				item.pathFlag = 0;
			}
			pathList.Remove(pathIndex);
		}

		public void DelBoardTransporterPathData(int pathIndex)
		{
			if (!transporterPathList.ContainsKey(pathIndex))
			{
				return;
			}
			foreach (EditorCell item in transporterPathList[pathIndex])
			{
				Object.Destroy(item.transporterLayer.gameObject);
				item.transporterFlag = 0;
			}
			transporterPathList.Remove(pathIndex);
		}

		public void DelBoardCatPathData(int pathIndex)
		{
			if (!catPathList.ContainsKey(pathIndex))
			{
				return;
			}
			foreach (EditorCell item in catPathList[pathIndex])
			{
				Object.Destroy(item.catLayer.gameObject);
				item.catFlag = 0;
			}
			catPathList.Remove(pathIndex);
		}

		public void PathDataToSaveData()
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			boardData.pathList = new Path[pathList.Count];
			int num = 0;
			if (pathList == null)
			{
				pathList = new Dictionary<int, List<EditorCell>>();
			}
			foreach (KeyValuePair<int, List<EditorCell>> path in pathList)
			{
				boardData.pathList[num] = new Path();
				boardData.pathList[num].v = new Pos[path.Value.Count];
				for (int i = 0; i < path.Value.Count; i++)
				{
					boardData.pathList[num].v[i] = new Pos();
					boardData.pathList[num].v[i].x = path.Value[i].col;
					boardData.pathList[num].v[i].y = path.Value[i].row;
				}
				num++;
			}
		}

		public void TransporterPathDataToSaveData()
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			boardData.transporterPathList = new Path[transporterPathList.Count];
			int num = 0;
			if (transporterPathList == null)
			{
				transporterPathList = new Dictionary<int, List<EditorCell>>();
			}
			foreach (KeyValuePair<int, List<EditorCell>> transporterPath in transporterPathList)
			{
				boardData.transporterPathList[num] = new Path();
				boardData.transporterPathList[num].v = new Pos[transporterPath.Value.Count];
				for (int i = 0; i < transporterPath.Value.Count; i++)
				{
					boardData.transporterPathList[num].v[i] = new Pos();
					boardData.transporterPathList[num].v[i].x = transporterPath.Value[i].col;
					boardData.transporterPathList[num].v[i].y = transporterPath.Value[i].row;
				}
				num++;
			}
		}

		public void CatPathDataToSaveData()
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			boardData.catPathList = new Path[catPathList.Count];
			int num = 0;
			if (catPathList == null)
			{
				catPathList = new Dictionary<int, List<EditorCell>>();
			}
			foreach (KeyValuePair<int, List<EditorCell>> catPath in catPathList)
			{
				boardData.catPathList[num] = new Path();
				boardData.catPathList[num].v = new Pos[catPath.Value.Count];
				for (int i = 0; i < catPath.Value.Count; i++)
				{
					boardData.catPathList[num].v[i] = new Pos();
					boardData.catPathList[num].v[i].x = catPath.Value[i].col;
					boardData.catPathList[num].v[i].y = catPath.Value[i].row;
				}
				num++;
			}
		}

		public void AddBoardTransData(int col, int row, int transIndex)
		{
			if (transList == null)
			{
				transList = new Dictionary<int, List<EditorCell>>();
			}
			if (!transList.ContainsKey(transIndex))
			{
				transList.Add(transIndex, new List<EditorCell>());
			}
			transList[transIndex].Add(board[row, col]);
		}

		public void DelBoardTransData(int transIndex)
		{
			if (!transList.ContainsKey(transIndex))
			{
				return;
			}
			foreach (EditorCell item in transList[transIndex])
			{
				Object.Destroy(item.transLayer.gameObject);
				item.transFlag = 0;
			}
			transList.Remove(transIndex);
		}

		public void DelBoardCreaterData(int flag)
		{
			DebugUtils.Log(DebugType.Other, "DelBoardCreaterData  " + flag);
			if (createrList.ContainsKey(flag))
			{
				DebugUtils.Log(DebugType.Other, "True  " + flag);
				createrList.Remove(flag);
			}
		}

		public void TransDataToSaveData()
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			boardData.transList = new Trans[transList.Count];
			int num = 0;
			if (transList == null)
			{
				transList = new Dictionary<int, List<EditorCell>>();
			}
			foreach (KeyValuePair<int, List<EditorCell>> trans in transList)
			{
				boardData.transList[num] = new Trans();
				boardData.transList[num].v = new Pos[trans.Value.Count];
				for (int i = 0; i < trans.Value.Count; i++)
				{
					boardData.transList[num].v[i] = new Pos();
					boardData.transList[num].v[i].x = trans.Value[i].col;
					boardData.transList[num].v[i].y = trans.Value[i].row;
				}
				num++;
			}
		}

		public void RemoveBoardAreaData(int startFlag, int endFlag)
		{
			if (areaList.ContainsKey(startFlag))
			{
				areaList[startFlag].start = null;
			}
			if (areaList.ContainsKey(endFlag))
			{
				areaList[endFlag].end = null;
			}
		}

		public void SetBoardAreaData(int col, int row, int startFlag, int endFlag)
		{
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			if (areaList == null)
			{
				areaList = new Dictionary<int, Area>();
			}
			if (startFlag != 0)
			{
				if (!areaList.ContainsKey(startFlag))
				{
					areaList[startFlag] = new Area();
					areaList[startFlag].index = startFlag;
				}
				if (areaList[startFlag].start != null)
				{
					board[areaList[startFlag].start.y, areaList[startFlag].start.x].SetAreaStartElement(startFlag, true);
				}
				areaList[startFlag].start = new Pos();
				areaList[startFlag].start.x = col;
				areaList[startFlag].start.y = row;
			}
			if (endFlag != 0)
			{
				if (!areaList.ContainsKey(endFlag))
				{
					areaList[endFlag] = new Area();
					areaList[endFlag].index = endFlag;
				}
				if (areaList[endFlag].end != null)
				{
					board[areaList[endFlag].end.y, areaList[endFlag].end.x].SetAreaEndElement(endFlag, true);
				}
				areaList[endFlag].end = new Pos();
				areaList[endFlag].end.x = col;
				areaList[endFlag].end.y = row;
			}
			boardData.areaList = new Area[areaList.Count];
			int num = 0;
			foreach (KeyValuePair<int, Area> area in areaList)
			{
				boardData.areaList[num] = new Area();
				boardData.areaList[num] = area.Value;
				num++;
			}
		}

		public void SetBoardCreaterData(int col, int row, int flag, int extra)
		{
			DebugUtils.Log(DebugType.Other, "SetBoardCreaterData " + col + " " + row + " " + flag + " " + extra);
			if (createrList == null)
			{
				createrList = new Dictionary<int, Creater>();
			}
			BoardData boardData = levelData.boardData[int.Parse(pageIndex.text) - 1];
			int key = row * boardData.width + col;
			if (!createrList.ContainsKey(key))
			{
				createrList[key] = new Creater();
				createrList[key].index = flag;
				createrList[key].probability = extra;
				createrList[key].p = new Pos();
				createrList[key].p.x = col;
				createrList[key].p.y = row;
			}
			else
			{
				createrList.Remove(key);
			}
			boardData.createrList = new Creater[createrList.Count];
			DebugUtils.Log(DebugType.Other, "SetBoardCreaterData count " + createrList.Count);
			int num = 0;
			foreach (KeyValuePair<int, Creater> creater in createrList)
			{
				DebugUtils.Log(DebugType.Other, "SetBoardCreaterData KeyValuePair " + creater.Value.index);
				boardData.createrList[num] = creater.Value;
				num++;
			}
		}

		public void ReSetBoardData()
		{
			board = null;
			cellList = null;
			pathList = null;
			transporterPathList = null;
			transList = null;
			areaList = null;
			createrList = null;
			catPathList = null;
		}

		public void RefreshBoard(int index)
		{
			pageIndex.text = (index + 1).ToString();
			foreach (Transform item in boardRoot.transform)
			{
				Object.Destroy(item.gameObject);
			}
			BoardData boardData = new BoardData();
			boardData = levelData.boardData[index];
			float num = 50f / cellPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
			float num2 = 50f;
			float num3 = 50f;
			boardRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(num2 * (float)boardData.width, num3 * (float)boardData.height);
			float num4 = boardRoot.GetComponent<RectTransform>().rect.width / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.width / 2f * num;
			float num5 = boardRoot.GetComponent<RectTransform>().rect.height / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.height / 2f * num;
			board = new EditorCell[boardData.height, boardData.width];
			for (int i = 0; i < boardData.height; i++)
			{
				for (int j = 0; j < boardData.width; j++)
				{
					int num6 = i * boardData.width + j;
					int num7 = Mathf.FloorToInt(num6 % boardData.width);
					int num8 = Mathf.FloorToInt(num6 / boardData.width);
					GameObject gameObject = Object.Instantiate(cellPrefab, boardRoot.transform);
					board[i, j] = gameObject.GetComponent<EditorCell>();
					board[i, j].row = i;
					board[i, j].col = j;
					board[i, j].flag = 0;
					board[i, j].data = boardData;
					gameObject.name = "bg" + num8 + "_" + num7;
					gameObject.transform.localScale *= num * 0.98f;
					gameObject.transform.localPosition = new Vector2((float)num7 * num2 - num4, (float)num8 * num3 - num5);
					board[i, j].SetCellElement(boardData.map[i * boardData.width + j]);
				}
			}
			int num9 = 1;
			Path[] array = boardData.pathList;
			for (int k = 0; k < array.Length; k++)
			{
				Pos[] v = array[k].v;
				foreach (Pos pos in v)
				{
					board[pos.y, pos.x].SetPathElement(num9);
				}
				num9++;
			}
			int num10 = 1;
			array = boardData.transporterPathList;
			for (int k = 0; k < array.Length; k++)
			{
				Pos[] v = array[k].v;
				foreach (Pos pos2 in v)
				{
					board[pos2.y, pos2.x].SetTransporterElement(num10);
				}
				num10++;
			}
			int num11 = 1;
			array = boardData.catPathList;
			for (int k = 0; k < array.Length; k++)
			{
				Pos[] v = array[k].v;
				foreach (Pos pos3 in v)
				{
					board[pos3.y, pos3.x].SetCatElement(num11);
				}
				num11++;
			}
			int num12 = 1;
			Trans[] array2 = boardData.transList;
			for (int k = 0; k < array2.Length; k++)
			{
				Pos[] v = array2[k].v;
				foreach (Pos pos4 in v)
				{
					board[pos4.y, pos4.x].SetTransElement(num12);
				}
				num12++;
			}
			int num13 = 1;
			Area[] array3 = boardData.areaList;
			foreach (Area area in array3)
			{
				board[area.start.y, area.start.x].SetAreaStartElement(num13, false);
				board[area.end.y, area.end.x].SetAreaEndElement(num13, false);
				num13++;
			}
			int num14 = 1;
			Creater[] array4 = boardData.createrList;
			foreach (Creater creater in array4)
			{
				board[creater.p.y, creater.p.x].SetCreaterElement(creater.index, creater.probability);
				num14++;
			}
			RefreshBoardInfo(index);
		}

		public void BtnLoadClicked()
		{
			int num = int.Parse(levelInput.text);
			DebugUtils.Log(DebugType.Other, "Levels/Level_" + num);
			TextAsset textAsset = Resources.Load<TextAsset>("Levels/Level_" + num);
			DebugUtils.Log(DebugType.Other, textAsset.text);
			JSONNode jSONNode = JSONNode.Parse(textAsset.text);
			DebugUtils.Log(DebugType.Other, jSONNode);
			levelData.move = int.Parse(jSONNode["move"]);
			levelData.hard = int.Parse(jSONNode["hard"]);
			levelData.probabilityList = new int[jSONNode["probabilityList"].Count];
			for (int i = 0; i < jSONNode["probabilityList"].Count; i++)
			{
				levelData.probabilityList[i] = int.Parse(jSONNode["probabilityList"][i]);
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
				int num2 = int.Parse(jSONNode["boardData"][l]["width"]);
				levelData.boardData[l].width = num2;
				int num3 = int.Parse(jSONNode["boardData"][l]["height"]);
				levelData.boardData[l].height = num3;
				levelData.boardData[l].pathList = new Path[jSONNode["boardData"][l]["pathList"].Count];
				for (int m = 0; m < jSONNode["boardData"][l]["pathList"].Count; m++)
				{
					levelData.boardData[l].pathList[m] = new Path();
					levelData.boardData[l].pathList[m].v = new Pos[jSONNode["boardData"][l]["pathList"][m]["v"].Count];
					for (int n = 0; n < jSONNode["boardData"][l]["pathList"][m]["v"].Count; n++)
					{
						levelData.boardData[l].pathList[m].v[n] = new Pos();
						levelData.boardData[l].pathList[m].v[n].x = int.Parse(jSONNode["boardData"][l]["pathList"][m]["v"][n]["x"]);
						levelData.boardData[l].pathList[m].v[n].y = int.Parse(jSONNode["boardData"][l]["pathList"][m]["v"][n]["y"]);
					}
				}
				levelData.boardData[l].transporterPathList = new Path[jSONNode["boardData"][l]["transporterPathList"].Count];
				for (int num4 = 0; num4 < jSONNode["boardData"][l]["transporterPathList"].Count; num4++)
				{
					levelData.boardData[l].transporterPathList[num4] = new Path();
					levelData.boardData[l].transporterPathList[num4].v = new Pos[jSONNode["boardData"][l]["transporterPathList"][num4]["v"].Count];
					for (int num5 = 0; num5 < jSONNode["boardData"][l]["transporterPathList"][num4]["v"].Count; num5++)
					{
						levelData.boardData[l].transporterPathList[num4].v[num5] = new Pos();
						levelData.boardData[l].transporterPathList[num4].v[num5].x = int.Parse(jSONNode["boardData"][l]["transporterPathList"][num4]["v"][num5]["x"]);
						levelData.boardData[l].transporterPathList[num4].v[num5].y = int.Parse(jSONNode["boardData"][l]["transporterPathList"][num4]["v"][num5]["y"]);
					}
				}
				levelData.boardData[l].catPathList = new Path[jSONNode["boardData"][l]["catPathList"].Count];
				for (int num6 = 0; num6 < jSONNode["boardData"][l]["catPathList"].Count; num6++)
				{
					levelData.boardData[l].catPathList[num6] = new Path();
					levelData.boardData[l].catPathList[num6].v = new Pos[jSONNode["boardData"][l]["catPathList"][num6]["v"].Count];
					for (int num7 = 0; num7 < jSONNode["boardData"][l]["catPathList"][num6]["v"].Count; num7++)
					{
						levelData.boardData[l].catPathList[num6].v[num7] = new Pos();
						levelData.boardData[l].catPathList[num6].v[num7].x = int.Parse(jSONNode["boardData"][l]["catPathList"][num6]["v"][num7]["x"]);
						levelData.boardData[l].catPathList[num6].v[num7].y = int.Parse(jSONNode["boardData"][l]["catPathList"][num6]["v"][num7]["y"]);
					}
				}
				levelData.boardData[l].transList = new Trans[jSONNode["boardData"][l]["transList"].Count];
				for (int num8 = 0; num8 < jSONNode["boardData"][l]["transList"].Count; num8++)
				{
					levelData.boardData[l].transList[num8] = new Trans();
					levelData.boardData[l].transList[num8].v = new Pos[jSONNode["boardData"][l]["transList"][num8]["v"].Count];
					for (int num9 = 0; num9 < jSONNode["boardData"][l]["transList"][num8]["v"].Count; num9++)
					{
						levelData.boardData[l].transList[num8].v[num9] = new Pos();
						levelData.boardData[l].transList[num8].v[num9].x = int.Parse(jSONNode["boardData"][l]["transList"][num8]["v"][num9]["x"]);
						levelData.boardData[l].transList[num8].v[num9].y = int.Parse(jSONNode["boardData"][l]["transList"][num8]["v"][num9]["y"]);
					}
				}
				levelData.boardData[l].areaList = new Area[jSONNode["boardData"][l]["areaList"].Count];
				for (int num10 = 0; num10 < jSONNode["boardData"][l]["areaList"].Count; num10++)
				{
					levelData.boardData[l].areaList[num10] = new Area();
					levelData.boardData[l].areaList[num10].index = int.Parse(jSONNode["boardData"][l]["areaList"][num10]["index"]);
					levelData.boardData[l].areaList[num10].start = new Pos();
					levelData.boardData[l].areaList[num10].start.x = int.Parse(jSONNode["boardData"][l]["areaList"][num10]["start"]["x"]);
					levelData.boardData[l].areaList[num10].start.y = int.Parse(jSONNode["boardData"][l]["areaList"][num10]["start"]["y"]);
					levelData.boardData[l].areaList[num10].end = new Pos();
					levelData.boardData[l].areaList[num10].end.x = int.Parse(jSONNode["boardData"][l]["areaList"][num10]["end"]["x"]);
					levelData.boardData[l].areaList[num10].end.y = int.Parse(jSONNode["boardData"][l]["areaList"][num10]["end"]["y"]);
				}
				levelData.boardData[l].createrList = new Creater[jSONNode["boardData"][l]["createrList"].Count];
				for (int num11 = 0; num11 < jSONNode["boardData"][l]["createrList"].Count; num11++)
				{
					levelData.boardData[l].createrList[num11] = new Creater();
					levelData.boardData[l].createrList[num11].index = int.Parse(jSONNode["boardData"][l]["createrList"][num11]["index"]);
					levelData.boardData[l].createrList[num11].probability = int.Parse(jSONNode["boardData"][l]["createrList"][num11]["probability"]);
					levelData.boardData[l].createrList[num11].p = new Pos();
					levelData.boardData[l].createrList[num11].p.x = int.Parse(jSONNode["boardData"][l]["createrList"][num11]["p"]["x"]);
					levelData.boardData[l].createrList[num11].p.y = int.Parse(jSONNode["boardData"][l]["createrList"][num11]["p"]["y"]);
				}
				int num12 = num2 * num3;
				levelData.boardData[l].map = new int[num12];
				for (int num13 = 0; num13 < num12; num13++)
				{
					levelData.boardData[l].map[num13] = int.Parse(jSONNode["boardData"][l]["map"][num13]);
				}
			}
			RefreshLevelInfo();
			RefreshBoardInfo(0);
			ReSetBoardData();
			RefreshBoard(0);
		}

		public void BtnSaveClicked()
		{
			int num = int.Parse(levelInput.text);
			levelData.level = num;
			levelData.move = int.Parse(moveInput.text);
			levelData.hard = hardDrop.value;
			levelData.probabilityList = new int[probabilityInputList.Length];
			for (int i = 0; i < probabilityInputList.Length; i++)
			{
				levelData.probabilityList[i] = int.Parse(probabilityInputList[i].text);
			}
			levelData.skillProbabilityList = new int[skillProbabilityInputList.Length];
			for (int j = 0; j < skillProbabilityInputList.Length; j++)
			{
				levelData.skillProbabilityList[j] = int.Parse(skillProbabilityInputList[j].text);
			}
			levelData.targetList = new int[targetInputList.Length];
			levelData.targetListByCollect = new int[targetInputList.Length];
			for (int k = 0; k < targetInputList.Length; k++)
			{
				levelData.targetList[k] = int.Parse(targetInputList[k].text);
				levelData.targetListByCollect[k] = int.Parse(targetInputList[k].text);
			}
			string text = JsonUtility.ToJson(levelData);
			DebugUtils.Log(DebugType.Other, "level output " + text);
			string text2 = Application.dataPath + "/Resources/Levels/Level_" + num + ".txt";
			DebugUtils.Log(DebugType.Other, "fName.dataPath " + text2);
			StreamWriter streamWriter = new FileInfo(text2).CreateText();
			streamWriter.WriteLine(text);
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
		}

		public void BtnPreviousClicked()
		{
			int num = int.Parse(pageIndex.text);
			if (num - 1 >= 1)
			{
				ReSetBoardData();
				RefreshBoard(num - 2);
			}
		}

		public void BtnNextClicked()
		{
			int num = int.Parse(pageIndex.text);
			if (num + 1 <= levelData.boardData.Length)
			{
				ReSetBoardData();
				RefreshBoard(num);
			}
		}

		public void BtnPageSetClicked()
		{
			ReSetBoardData();
			int num = int.Parse(pageInput.text);
			levelData.boardData = new BoardData[num];
			for (int i = 0; i < num; i++)
			{
				int index = i;
				CreateOneEmptyBoard(index);
			}
			foreach (Transform item in boardRoot.transform)
			{
				Object.Destroy(item.gameObject);
			}
			BoardData boardData = new BoardData();
			boardData = levelData.boardData[0];
			float num2 = 50f / cellPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
			float num3 = 50f;
			float num4 = 50f;
			boardRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(num3 * (float)boardData.width, num4 * (float)boardData.height);
			float num5 = boardRoot.GetComponent<RectTransform>().rect.width / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.width / 2f * num2;
			float num6 = boardRoot.GetComponent<RectTransform>().rect.height / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.height / 2f * num2;
			board = new EditorCell[boardData.height, boardData.width];
			for (int j = 0; j < boardData.height; j++)
			{
				for (int k = 0; k < boardData.width; k++)
				{
					int num7 = j * boardData.width + k;
					int num8 = Mathf.FloorToInt(num7 % boardData.width);
					int num9 = Mathf.FloorToInt(num7 / boardData.width);
					GameObject gameObject = Object.Instantiate(cellPrefab, boardRoot.transform);
					board[j, k] = gameObject.GetComponent<EditorCell>();
					board[j, k].row = j;
					board[j, k].col = k;
					board[j, k].flag = 0;
					board[j, k].data = boardData;
					gameObject.name = "bg" + num9 + "_" + num8;
					gameObject.transform.localScale *= num2 * 0.98f;
					gameObject.transform.localPosition = new Vector2((float)num8 * num3 - num5, (float)num9 * num4 - num6);
				}
			}
		}

		public void CreateOneEmptyBoard(int index)
		{
			DebugUtils.Log(DebugType.Other, "CreateEmptyBoard");
			BoardData boardData = new BoardData();
			levelData.boardData[index] = boardData;
			boardData.width = int.Parse(widthInput.text);
			boardData.height = int.Parse(heightInput.text);
			boardData.map = new int[boardData.height * boardData.width];
			for (int i = 0; i < boardData.height; i++)
			{
				for (int j = 0; j < boardData.width; j++)
				{
					boardData.map[i * boardData.width + j] = 0;
				}
			}
		}

		public void BtnSizeSetClicked()
		{
			DebugUtils.Log(DebugType.Other, "ResetOneEmptyBoard");
			ReSetBoardData();
			BoardData boardData = new BoardData();
			int num = int.Parse(pageIndex.text) - 1;
			levelData.boardData[num] = boardData;
			boardData.width = int.Parse(widthInput.text);
			boardData.height = int.Parse(heightInput.text);
			boardData.map = new int[boardData.height * boardData.width];
			foreach (Transform item in boardRoot.transform)
			{
				Object.Destroy(item.gameObject);
			}
			float num2 = 50f / cellPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
			float num3 = 50f;
			float num4 = 50f;
			boardRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(num3 * (float)boardData.width, num4 * (float)boardData.height);
			float num5 = boardRoot.GetComponent<RectTransform>().rect.width / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.width / 2f * num2;
			float num6 = boardRoot.GetComponent<RectTransform>().rect.height / 2f - cellPrefab.GetComponent<Image>().rectTransform.rect.height / 2f * num2;
			board = new EditorCell[boardData.height, boardData.width];
			for (int num7 = boardData.height - 1; num7 >= 0; num7--)
			{
				for (int num8 = boardData.width - 1; num8 >= 0; num8--)
				{
					int num9 = num7 * boardData.width + num8;
					int num10 = Mathf.FloorToInt(num9 % boardData.width);
					int num11 = Mathf.FloorToInt(num9 / boardData.width);
					GameObject gameObject = Object.Instantiate(cellPrefab, boardRoot.transform);
					board[num7, num8] = gameObject.GetComponent<EditorCell>();
					board[num7, num8].row = num7;
					board[num7, num8].col = num8;
					board[num7, num8].flag = 0;
					board[num7, num8].data = boardData;
					gameObject.name = "bg" + num11 + "_" + num10;
					gameObject.transform.localScale *= num2 * 0.98f;
					gameObject.transform.localPosition = new Vector2((float)num10 * num3 - num5, (float)num11 * num4 - num6);
					boardData.map[num7 * boardData.width + num8] = 0;
					board[num7, num8].SetPathElement(num8 + 1);
				}
			}
			Instance.PathDataToSaveData();
		}

		public void ElementClicked(int idx)
		{
			currType = 0;
			currElement = idx;
		}

		public void BtnIceClicked()
		{
			currType = 0;
			switch (IceDrop.value)
			{
			case 0:
				currElement = 100;
				break;
			case 1:
				currElement = 200;
				break;
			case 2:
				currElement = 300;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnBoxClicked()
		{
			currType = 0;
			switch (BoxDrop.value)
			{
			case 0:
				currElement = 31;
				break;
			case 1:
				currElement = 32;
				break;
			case 2:
				currElement = 33;
				break;
			case 3:
				currElement = 34;
				break;
			case 4:
				currElement = 35;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnButtonClicked()
		{
			currType = 0;
			switch (ButtonDrop.value)
			{
			case 0:
				currElement = 71;
				break;
			case 1:
				currElement = 72;
				break;
			case 2:
				currElement = 73;
				break;
			case 3:
				currElement = 74;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnTreasureClicked()
		{
			currType = 0;
			switch (TreasureDrop.value)
			{
			case 0:
				currElement = 52;
				break;
			case 1:
				currElement = 51;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnNullTreasureClicked()
		{
			currType = 0;
			switch (NullTreasureDrop.value)
			{
			case 0:
				currElement = 62;
				break;
			case 1:
				currElement = 61;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnCloudClicked()
		{
			currType = 0;
			int value = CloudDrop.value;
			DebugUtils.Log(DebugType.Other, value);
			switch (value)
			{
			case 0:
				currElement = 42;
				break;
			case 1:
				currElement = 43;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnGreenBrambleClicked()
		{
			currType = 0;
			switch (GreenBrambleDrop.value)
			{
			case 0:
				currElement = 10000;
				break;
			case 1:
				currElement = 11000;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnRedBrambleClicked()
		{
			currType = 0;
			switch (RedBrambleDrop.value)
			{
			case 0:
				currElement = 12000;
				break;
			case 1:
				currElement = 13000;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnHoneyClicked()
		{
			currType = 0;
			switch (HoneyDrop.value)
			{
			case 0:
				currElement = 20000;
				break;
			case 1:
				currElement = 21000;
				break;
			case 2:
				currElement = 22000;
				break;
			case 3:
				currElement = 23000;
				break;
			case 4:
				currElement = 24000;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnCollectClicked()
		{
			currType = 0;
			switch (CollectDrop.value)
			{
			case 0:
				currElement = 1000000;
				break;
			case 1:
				currElement = 1100000;
				break;
			case 2:
				currElement = 1200000;
				break;
			case 3:
				currElement = 1300000;
				break;
			case 4:
				currElement = 1400000;
				break;
			case 5:
				currElement = 1500000;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnDoorClicked()
		{
			currType = 0;
			switch (DoorDrop.value)
			{
			case 0:
				currElement = 10000000;
				break;
			case 1:
				currElement = 20000000;
				break;
			case 2:
				currElement = 30000000;
				break;
			case 3:
				currElement = 40000000;
				break;
			default:
				currElement = 0;
				break;
			}
		}

		public void BtnPathClicked()
		{
			currType = 1;
			currElement = int.Parse(markInput[0].text);
		}

		public void BtnTransClicked()
		{
			currType = 2;
			currElement = int.Parse(markInput[1].text);
		}

		public void BtnAreaClicked()
		{
			currType = 3;
			currElement = int.Parse(markInput[2].text);
		}

		public void BtnTransporterClicked()
		{
			currType = 5;
			currElement = int.Parse(markInput[3].text);
		}

		public void BtnCatClicked()
		{
			currType = 6;
			currElement = int.Parse(markInput[4].text);
		}

		public void BtnCreaterClicked(int idx)
		{
			currType = 4;
			currElement = idx;
			currExtra = int.Parse(createrInputList[idx - 1].text);
		}

		public void BtnHardClicked(int idx)
		{
			levelData.hard = idx;
		}
	}
}
