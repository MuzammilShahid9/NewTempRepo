using System.Collections.Generic;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.Editor
{
	public class EditorElementGenerator : MonoBehaviour
	{
		private static EditorElementGenerator _instance;

		public GameObject ElementPrefab;

		public Dictionary<int, Sprite> CreaterPictures;

		public static EditorElementGenerator Instance
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
			CreaterPictures = new Dictionary<int, Sprite>();
			CreaterPictures.Add(1, Resources.Load("Editor/build_jewel", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(2, Resources.Load("Editor/build_fly", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(3, Resources.Load("Editor/build_rc", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(4, Resources.Load("Editor/build_area", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(5, Resources.Load("Editor/build_color", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(6, Resources.Load("Editor/build_shell", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(7, Resources.Load("Editor/build_2fs", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(8, Resources.Load("Editor/build_2frc", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(9, Resources.Load("Editor/build_2fa", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(10, Resources.Load("Editor/build_2cs", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(11, Resources.Load("Editor/build_3frca", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(12, Resources.Load("Editor/build_3rcas", typeof(Sprite)) as Sprite);
			CreaterPictures.Add(13, Resources.Load("Editor/build_3rcas", typeof(Sprite)) as Sprite);
		}

		public void Init()
		{
		}

		public GameObject Create(int flag, int row, int col)
		{
			GameObject obj = Object.Instantiate(ElementPrefab);
			obj.GetComponent<EditorElement>().row = row;
			obj.GetComponent<EditorElement>().col = col;
			obj.GetComponent<EditorElement>().CreateStandard(flag);
			return obj;
		}
	}
}
