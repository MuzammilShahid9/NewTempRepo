using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Editor
{
	public class EditorElement : MonoBehaviour
	{
		public GameObject img;

		public int row;

		public int col;

		public int color = -1;

		public bool dragged;

		public Direction dragDirection;

		public Vector3 dragStartPos;

		public bool moving;

		private ElementType type;

		private void Awake()
		{
		}

		public void CreateStandard(int color)
		{
			if (GeneralConfig.ElementPictures.ContainsKey(color))
			{
				img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[color];
				if (color >= 1000000 && color <= 1500000)
				{
					float x = 1f;
					float y = 1f;
					float z = 1f;
					switch (color)
					{
					case 1000000:
					case 1300000:
						x = 0.33f;
						y = 0.33f;
						z = 0.33f;
						break;
					case 1100000:
					case 1400000:
						x = 0.667f;
						y = 0.667f;
						z = 0.667f;
						break;
					case 1200000:
					case 1500000:
						x = 1f;
						y = 1f;
						z = 1f;
						break;
					}
					if (color == 1300000 || color == 1400000 || color == 1500000)
					{
						img.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
					}
					img.transform.localScale = new Vector3(x, y, z);
				}
			}
			this.color = color;
			type = (ElementType)color;
		}

		public void CreateBomb(ElementType type)
		{
			img.GetComponent<SpriteRenderer>().sprite = GeneralConfig.ElementPictures[(int)type];
			color = -1;
			this.type = type;
		}

		public bool IsBomb()
		{
			if (type >= ElementType.FlyBomb && type <= ElementType.ColorBomb)
			{
				return true;
			}
			return false;
		}
	}
}
