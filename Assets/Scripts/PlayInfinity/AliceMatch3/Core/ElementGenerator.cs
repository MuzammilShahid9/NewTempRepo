using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Core
{
	public class ElementGenerator : MonoBehaviour
	{
		private static ElementGenerator _instance;

		public GameObject ElementPrefab;

		public Dictionary<int, Sprite> ElementPictures;

		public static ElementGenerator Instance
		{
			get
			{
				return _instance;
			}
		}

		private void Awake()
		{
			_instance = this;
			Init();
		}

		public void Init()
		{
		}

		public GameObject Create(Board board, int flag, int row, int col, bool isAnim = true, bool isUpdateTargetInfo = true)
		{
			if (flag != 99)
			{
				GameObject gameObject = Object.Instantiate(ElementPrefab);
				gameObject.GetComponent<Element>().row = row;
				gameObject.GetComponent<Element>().col = col;
				gameObject.GetComponent<Element>().board = board;
				gameObject.transform.SetParent(board.container.transform, false);
				switch (flag)
				{
				case 10:
					gameObject.GetComponent<Element>().CreateBomb(ElementType.FlyBomb, false, false, true, isAnim, isUpdateTargetInfo);
					return gameObject;
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 37:
				case 38:
				case 39:
				case 40:
				case 41:
				case 42:
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
				case 48:
				case 49:
				case 50:
				case 51:
				case 52:
				case 53:
				case 54:
				case 55:
				case 56:
				case 57:
				case 58:
				case 59:
				case 60:
				case 61:
				case 62:
				case 63:
				case 64:
				case 65:
				case 66:
				case 67:
				case 68:
				case 69:
				case 70:
				case 71:
				case 72:
				case 73:
				case 74:
				case 75:
				case 76:
				case 77:
				case 78:
				case 79:
				case 80:
				case 81:
				case 82:
				case 83:
				case 84:
				case 85:
				case 86:
				case 87:
				case 88:
				case 89:
				case 90:
				case 91:
				case 92:
				case 93:
				case 94:
				case 95:
				case 96:
				case 97:
				case 98:
					gameObject.GetComponent<Element>().CreateStandard(flag, isAnim, isUpdateTargetInfo);
					return gameObject;
				}
				if (flag >= 100 && flag <= 600)
				{
					gameObject.GetComponent<Element>().CreateBottom(flag, isAnim, isUpdateTargetInfo);
					return gameObject;
				}
				if (flag >= 10000 && flag <= 24000)
				{
					gameObject.GetComponent<Element>().CreateTop(flag, isAnim, isUpdateTargetInfo);
					return gameObject;
				}
				if (flag >= 1000000 && flag <= 1500000)
				{
					float xScale = 1f;
					float yScale = 1f;
					float zScale = 1f;
					float xRot = 0f;
					float yRot = 0f;
					float zRot = 0f;
					switch (flag)
					{
					case 1000000:
					case 1300000:
						xScale = 0.33f;
						yScale = 0.33f;
						zScale = 0.33f;
						break;
					case 1100000:
					case 1400000:
						xScale = 0.667f;
						yScale = 0.667f;
						zScale = 0.667f;
						break;
					case 1200000:
					case 1500000:
						xScale = 1f;
						yScale = 1f;
						zScale = 1f;
						break;
					}
					if (flag == 1300000 || flag == 1400000 || flag == 1500000)
					{
						zRot = 90f;
					}
					gameObject.GetComponent<Element>().CreateLast(flag, xScale, yScale, zScale, xRot, yRot, zRot, isAnim, isUpdateTargetInfo);
					return gameObject;
				}
				if (flag >= -3 && flag <= -2)
				{
					gameObject.GetComponent<Element>().CreateTop(flag, isAnim, isUpdateTargetInfo);
					return gameObject;
				}
			}
			return null;
		}

		public Element DeepClone(Element elem)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, elem);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return (Element)binaryFormatter.Deserialize(memoryStream);
		}

		public GameObject CreateObj(int type)
		{
			GameObject gameObject = Object.Instantiate(ElementPrefab);
			if (type == 10)
			{
				gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[10];
				return gameObject;
			}
			return null;
		}
	}
}
